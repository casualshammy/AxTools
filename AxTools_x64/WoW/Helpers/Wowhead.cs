using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AxTools.Helpers;
using Newtonsoft.Json;

namespace AxTools.WoW.Helpers
{
    internal static class Wowhead
    {
        private static readonly string _locale;
        private static readonly ConcurrentDictionary<uint, WowheadItemInfo> ItemInfos = new ConcurrentDictionary<uint, WowheadItemInfo>();
        private static readonly ConcurrentDictionary<int, WowheadSpellInfo> SpellInfos = new ConcurrentDictionary<int, WowheadSpellInfo>();
        private static readonly ConcurrentDictionary<uint, string> ZoneInfos = new ConcurrentDictionary<uint, string>();
        private static readonly string CacheDir = Application.StartupPath + "\\wowheadCache";
        private const string UNKNOWN = "UNKNOWN";

        static Wowhead()
        {
            if (!Directory.Exists(CacheDir))
            {
                Directory.CreateDirectory(CacheDir);
            }
            _locale = GetLocale();
        }

        internal static WowheadItemInfo GetItemInfo(uint itemID)
        {
            // <name><![CDATA[Iceblade Arrow]]></name>
            // <class id="1"><![CDATA[Контейнеры]]></class>
            // <level>85</level>
            // <quality id="1">Обычный</quality>
            // "inv_misc_questionmark"
            WowheadItemInfo info;
            if (!ItemInfos.TryGetValue(itemID, out info))
            {
                if ((info = ItemInfo_GetCachedValue(itemID)) == null)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        string xml = webClient.DownloadString("https://" + _locale + ".wowhead.com/item=" + itemID + "&xml");
                        Regex regex = new Regex("<name><!\\[CDATA\\[(.+)\\]\\]></name>.*<level>(\\d+)</level>.*<quality id=\"(\\d+)\">.*</quality>.*<class id=\"(\\d+)\">.*<icon.*>(.+)</icon>");
                        Match match = regex.Match(xml);
                        if (match.Success)
                        {
                            info = new WowheadItemInfo(match.Groups[1].Value, uint.Parse(match.Groups[4].Value), uint.Parse(match.Groups[2].Value), uint.Parse(match.Groups[3].Value));
                            using (MemoryStream ms = new MemoryStream(webClient.DownloadData("https://wow.zamimg.com/images/wow/icons/small/" + match.Groups[5].Value + ".jpg")))
                            {
                                info.ImageBytes = ms.ToArray();
                            }
                            ItemInfo_SaveToCache(itemID, info);
                        }
                        else
                        {
                            info = new WowheadItemInfo(UNKNOWN, 0, 0, 0);
                            Log.Error("[Wowhead] Regex isn't match: " + JsonConvert.SerializeObject(xml));
                        }
                    }
                }
                ItemInfos.TryAdd(itemID, info);
            }
            return info;
        }

        internal static WowheadSpellInfo GetSpellInfo(int spellID)
        {
            WowheadSpellInfo info;
            if (!SpellInfos.TryGetValue(spellID, out info))
            {
                if ((info = SpellInfo_GetCachedValue(spellID)) == null)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        string xml = webClient.DownloadString("https://" + _locale + ".wowhead.com/spell=" + spellID + "&power");
                        Regex regex = new Regex("\\s+name_.+:\\s*'(.+)',\\s+icon: '(.+)',");
                        Match match = regex.Match(xml);
                        if (match.Success)
                        {
                            info = new WowheadSpellInfo(match.Groups[1].Value);
                            using (MemoryStream ms = new MemoryStream(webClient.DownloadData("https://wow.zamimg.com/images/wow/icons/small/" + match.Groups[2].Value + ".jpg")))
                            {
                                info.ImageBytes = ms.ToArray();
                            }
                            SpellInfo_SaveToCache(spellID, info);
                        }
                        else
                        {
                            info = new WowheadSpellInfo(UNKNOWN);
                            Log.Info("[Wowhead] Regex isn't match: " + JsonConvert.SerializeObject(xml));
                        }
                    }
                }
                SpellInfos.TryAdd(spellID, info);
            }
            return info;
        }

        internal static string GetZoneText(uint zoneID)
        {
            string info;
            if (!ZoneInfos.TryGetValue(zoneID, out info))
            {
                if ((info = ZoneInfo_GetCachedValue(zoneID)) == null)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        string xml = webClient.DownloadString(string.Format("https://{0}.wowhead.com/zone={1}&power", _locale, zoneID));
                        Regex regex = new Regex("\\s+name_.+:\\s*'(.+)',");
                        Match match = regex.Match(xml);
                        if (match.Success)
                        {
                            info = match.Groups[1].Value;
                            ZoneInfo_SaveToCache(zoneID, info);
                        }
                        else
                        {
                            info = UNKNOWN;
                            Log.Info("[Wowhead] Regex isn't match: " + JsonConvert.SerializeObject(xml));
                        }
                    }
                }
                ZoneInfos.TryAdd(zoneID, info);
            }
            return info;
        }

        private static string GetLocale()
        {
            string configWtfPath = Settings.Instance.WoWDirectory + "\\WTF\\Config.wtf";
            if (File.Exists(configWtfPath))
            {
                string configWtf = File.ReadAllText(configWtfPath, Encoding.UTF8);
                Regex regex = new Regex("SET textLocale \"(\\w\\w).+\"");
                Match match = regex.Match(configWtf);
                return match.Success ? match.Groups[1].Value : "www";
            }
            return "www";
        }

        private static WowheadItemInfo ItemInfo_GetCachedValue(uint itemID)
        {
            string filepath = string.Format("{0}\\items\\item-{1}.json", CacheDir, itemID);
            if (File.Exists(filepath))
            {
                string rawText = File.ReadAllText(filepath, Encoding.UTF8);
                WowheadItemInfo itemInfo = JsonConvert.DeserializeObject<WowheadItemInfo>(rawText);
                return itemInfo;
            }
            return null;
        }

        private static void ItemInfo_SaveToCache(uint itemID, WowheadItemInfo info)
        {
            string filepath = string.Format("{0}\\items\\item-{1}.json", CacheDir, itemID);
            string s = JsonConvert.SerializeObject(info, Formatting.Indented);
            FileInfo fileInfo = new FileInfo(filepath);
            if (fileInfo.Directory != null) fileInfo.Directory.Create();
            File.WriteAllText(filepath, s, Encoding.UTF8);
        }

        private static WowheadSpellInfo SpellInfo_GetCachedValue(int spellID)
        {
            string filepath = string.Format("{0}\\spells\\spell-{1}.json", CacheDir, spellID);
            if (File.Exists(filepath))
            {
                string rawText = File.ReadAllText(filepath, Encoding.UTF8);
                WowheadSpellInfo spellInfo = JsonConvert.DeserializeObject<WowheadSpellInfo>(rawText);
                return spellInfo;
            }
            return null;
        }

        private static void SpellInfo_SaveToCache(int spellID, WowheadSpellInfo info)
        {
            string filepath = string.Format("{0}\\spells\\spell-{1}.json", CacheDir, spellID);
            string s = JsonConvert.SerializeObject(info, Formatting.Indented);
            FileInfo fileInfo = new FileInfo(filepath);
            if (fileInfo.Directory != null) fileInfo.Directory.Create();
            File.WriteAllText(filepath, s, Encoding.UTF8);
        }

        private static string ZoneInfo_GetCachedValue(uint zoneID)
        {
            string filepath = string.Format("{0}\\zones\\zone-{1}.json", CacheDir, zoneID);
            if (File.Exists(filepath))
            {
                string rawText = File.ReadAllText(filepath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<string>(rawText);
            }
            return null;
        }

        private static void ZoneInfo_SaveToCache(uint zoneID, string zoneText)
        {
            string filepath = string.Format("{0}\\zones\\zone-{1}.json", CacheDir, zoneID);
            string s = JsonConvert.SerializeObject(zoneText, Formatting.Indented);
            FileInfo fileInfo = new FileInfo(filepath);
            if (fileInfo.Directory != null) fileInfo.Directory.Create();
            File.WriteAllText(filepath, s, Encoding.UTF8);
        }

    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class WowheadItemInfo
    {
        [JsonConstructor]
        internal WowheadItemInfo()
        {

        }

        internal WowheadItemInfo(string name, uint @class, uint level, uint quality)
        {
            Name = name;
            Class = @class;
            Level = level;
            Quality = quality;
        }

        [JsonProperty(PropertyName = "Name")]
        internal string Name;

        [JsonProperty(PropertyName = "Class")]
        internal uint Class;

        [JsonProperty(PropertyName = "Level")]
        internal uint Level;

        [JsonProperty(PropertyName = "Quality")]
        internal uint Quality;

        [JsonProperty(PropertyName = "ImageBytes")]
        internal byte[] ImageBytes
        {
            get
            {
                if (Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (Bitmap bitmap = new Bitmap(Image, Image.Size))
                        {
                            bitmap.Save(ms, ImageFormat.Png);
                        }
                        return ms.ToArray();
                    }
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    using (MemoryStream ms = new MemoryStream(value))
                    {
                        Image = Image.FromStream(ms);
                    }
                }
            }
        }

        internal Image Image;

    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class WowheadSpellInfo
    {
        [JsonConstructor]
        internal WowheadSpellInfo()
        {

        }

        internal WowheadSpellInfo(string name)
        {
            Name = name;
        }

        [JsonProperty(PropertyName = "Name")]
        internal string Name;

        [JsonProperty(PropertyName = "ImageBytes")]
        internal byte[] ImageBytes
        {
            get
            {
                if (Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (Bitmap bitmap = new Bitmap(Image, Image.Size))
                        {
                            bitmap.Save(ms, ImageFormat.Png);
                        }
                        return ms.ToArray();
                    }
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    using (MemoryStream ms = new MemoryStream(value))
                    {
                        Image = Image.FromStream(ms);
                    }
                }
            }
        }

        internal Image Image;

    }

}
