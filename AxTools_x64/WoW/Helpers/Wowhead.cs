using System;
using System.Collections.Concurrent;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AxTools.Forms;
using AxTools.Helpers;
using Newtonsoft.Json;

namespace AxTools.WoW.Helpers
{
    internal static class Wowhead
    {
        private static readonly string _locale;
        private static readonly ConcurrentDictionary<uint, WowheadItemInfo> ItemInfos = new ConcurrentDictionary<uint, WowheadItemInfo>();
        private static readonly ConcurrentDictionary<int, WowheadSpellInfo> SpellInfos = new ConcurrentDictionary<int, WowheadSpellInfo>();
        private static readonly ConcurrentDictionary<uint, WowheadNpcInfo> NpcInfos = new ConcurrentDictionary<uint, WowheadNpcInfo>();
        private static readonly ConcurrentDictionary<uint, string> ZoneInfos = new ConcurrentDictionary<uint, string>();
        private const string UNKNOWN = "UNKNOWN";
        private static readonly object DBLock = new object();
        private static long _sumDBAccessTicks;
        private static long _sumDBAccessTime;
        private static int _numDBAccesses;
        private static SQLiteConnection dbConnection;

        static Wowhead()
        {
            _locale = GetLocale();
            MainForm.ClosingEx +=
                delegate
                {
                    if (_numDBAccesses > 100)
                    {
                        Log.Error(string.Format("[Wowhead] DB usage stats: numDBAccesses: {0}; average access time: {1} ms/call; average access time: {2} ticks/call",
                            _numDBAccesses, _sumDBAccessTime/(_numDBAccesses == 0 ? -1 : _numDBAccesses), _sumDBAccessTicks/(_numDBAccesses == 0 ? -1 : _numDBAccesses)));
                    }
                };
        }

        internal static WowheadItemInfo GetItemInfo(uint itemID)
        {
            // <name><![CDATA[Iceblade Arrow]]></name>
            // <class id="1"><![CDATA[Контейнеры]]></class>
            // <level>85</level>
            // <quality id="1">Обычный</quality>
            // "inv_misc_questionmark"
            if (!ItemInfos.TryGetValue(itemID, out WowheadItemInfo info))
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
                            Log.Info("[Wowhead] Regex isn't match: " + JsonConvert.SerializeObject(xml));
                        }
                    }
                }
                ItemInfos.TryAdd(itemID, info);
            }
            return info;
        }

        internal static WowheadSpellInfo GetSpellInfo(int spellID)
        {
            if (!SpellInfos.TryGetValue(spellID, out WowheadSpellInfo info))
            {
                if ((info = SpellInfo_GetCachedValue(spellID)) == null)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        string xml = webClient.DownloadString("https://" + _locale + ".wowhead.com/spell=" + spellID + "&power");
                        Regex regex = new Regex("\\s+name_.+:\\s*'(.+)',\\s+icon: '(.*)',");
                        Match match = regex.Match(xml);
                        if (match.Success)
                        {
                            info = new WowheadSpellInfo(match.Groups[1].Value);
                            if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
                            {
                                using (MemoryStream ms = new MemoryStream(webClient.DownloadData("https://wow.zamimg.com/images/wow/icons/small/" + match.Groups[2].Value + ".jpg")))
                                {
                                    info.ImageBytes = ms.ToArray();
                                }
                            }
                            else
                            {
                                info.Image = AxTools.Properties.Resources.dialog_error;
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

        internal static WowheadNpcInfo GetNpcInfo(uint entryID)
        {
            if (!NpcInfos.TryGetValue(entryID, out WowheadNpcInfo info))
            {
                if ((info = NpcInfo_GetCachedValue(entryID)) == null)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        string xml = webClient.DownloadString("https://" + _locale + ".wowhead.com/npc=" + entryID + "&power");
                        Regex regex = new Regex("\\s+name_.+:\\s*'(.+)'");
                        Match match = regex.Match(xml);
                        if (match.Success)
                        {
                            info = new WowheadNpcInfo(match.Groups[1].Value);
                            NpcInfo_SaveToCache(entryID, info);
                        }
                        else
                        {
                            info = new WowheadNpcInfo(UNKNOWN);
                            Log.Info("[Wowhead] Regex isn't match: " + JsonConvert.SerializeObject(xml));
                        }
                    }
                }
                NpcInfos.TryAdd(entryID, info);
            }
            return info;
        }

        internal static string GetZoneText(uint zoneID)
        {
            if (!ZoneInfos.TryGetValue(zoneID, out string info))
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
            lock (DBLock)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    if (dbConnection == null)
                        dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\wowhead.sqlite;Version=3;").OpenAndReturn();
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText =
                            "CREATE TABLE IF NOT EXISTS items ( itemID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL, class INTEGER NOT NULL, level INTEGER NOT NULL, quality INTEGER NOT NULL, image TEXT NOT NULL );" +
                            $"SELECT * FROM items WHERE itemID = {itemID};";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new WowheadItemInfo(reader["name"].ToString(), uint.Parse(reader["class"].ToString()), uint.Parse(reader["level"].ToString()), uint.Parse(reader["quality"].ToString()))
                                { ImageBytes = JsonConvert.DeserializeObject<byte[]>(reader["image"].ToString()) };
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("[Wowhead.ItemInfo_GetCachedValue] Error: " + ex.Message);
                    return null;
                }
                finally
                {
                    _sumDBAccessTicks += stopwatch.ElapsedTicks;
                    _sumDBAccessTime += stopwatch.ElapsedMilliseconds;
                    _numDBAccesses++;
                }
            }
        }

        private static void ItemInfo_SaveToCache(uint itemID, WowheadItemInfo info)
        {
            lock (DBLock)
            {
                if (dbConnection == null)
                    dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\wowhead.sqlite;Version=3;").OpenAndReturn();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS items ( itemID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL, class INTEGER NOT NULL, level INTEGER NOT NULL, quality INTEGER NOT NULL, image TEXT NOT NULL );" +
                        $"INSERT INTO items (itemID, name, class, level, quality, image) values ({itemID}, '{info.Name.Replace("'", "''")}', {info.Class}, {info.Level}, {info.Quality}, '{JsonConvert.SerializeObject(info.ImageBytes)}')";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("[Wowhead.ItemInfo_SaveToCache] Error-0: " + ex.Message +
                             $"\r\nINSERT INTO items (itemID, name, class, level, quality, image) values ({itemID}, '{info.Name.Replace("'", "''")}', {info.Class}, {info.Level}, {info.Quality}, '{JsonConvert.SerializeObject(info.ImageBytes)}')");
                    }

                }
            }
        }

        private static WowheadSpellInfo SpellInfo_GetCachedValue(int spellID)
        {
            lock (DBLock)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    if (dbConnection == null)
                        dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\wowhead.sqlite;Version=3;").OpenAndReturn();
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText =
                            "CREATE TABLE IF NOT EXISTS spells ( spellID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL, image TEXT NOT NULL );" +
                            $"SELECT * FROM spells WHERE spellID = {spellID};";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new WowheadSpellInfo(reader["name"].ToString()) { ImageBytes = JsonConvert.DeserializeObject<byte[]>(reader["image"].ToString()) };
                            }
                            else
                            {
                                return null;
                            }
                        } 
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("[Wowhead.SpellInfo_GetCachedValue] Error: " + ex.Message);
                    return null;
                }
                finally
                {
                    _sumDBAccessTicks += stopwatch.ElapsedTicks;
                    _sumDBAccessTime += stopwatch.ElapsedMilliseconds;
                    _numDBAccesses++;
                }
            }
        }
        
        private static void SpellInfo_SaveToCache(int spellID, WowheadSpellInfo info)
        {
            lock (DBLock)
            {
                if (dbConnection == null)
                    dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\wowhead.sqlite;Version=3;").OpenAndReturn();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS spells ( spellID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL, image TEXT NOT NULL );" +
                        $"INSERT INTO spells (spellID, name, image) values ({spellID}, '{info.Name.Replace("'", "''")}', '{JsonConvert.SerializeObject(info.ImageBytes)}')";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("[Wowhead.SpellInfo_SaveToCache] Error-0: " + ex.Message +
                            $"\r\nINSERT INTO spells (spellID, name, image) values ({spellID}, '{info.Name.Replace("'", "''")}', '{JsonConvert.SerializeObject(info.ImageBytes)}')");
                    }

                }
            }
        }

        private static string ZoneInfo_GetCachedValue(uint zoneID)
        {
            lock (DBLock)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    if (dbConnection == null)
                        dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\wowhead.sqlite;Version=3;").OpenAndReturn();
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText =
                            "CREATE TABLE IF NOT EXISTS zones ( zoneID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL );" +
                            $"SELECT * FROM zones WHERE zoneID = {zoneID};";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader["name"].ToString();
                            }
                            else
                            {
                                return null;
                            }
                        }  
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("[Wowhead.ZoneInfo_GetCachedValue] Error: " + ex.Message);
                    return null;
                }
                finally
                {
                    _sumDBAccessTicks += stopwatch.ElapsedTicks;
                    _sumDBAccessTime += stopwatch.ElapsedMilliseconds;
                    _numDBAccesses++;
                }
            }
        }

        private static void ZoneInfo_SaveToCache(uint zoneID, string zoneText)
        {
            lock (DBLock)
            {
                if (dbConnection == null)
                    dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\wowhead.sqlite;Version=3;").OpenAndReturn();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS zones ( zoneID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL );" +
                        $"INSERT INTO zones (zoneID, name) values ({zoneID}, '{zoneText.Replace("'", "''")}');";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("[Wowhead.ZoneInfo_SaveToCache] Error-0: " + ex.Message + $"\r\nINSERT INTO zones (zoneID, name) values ({zoneID}, '{zoneText.Replace("'", "''")}')");
                    }

                }
            }
        }

        private static WowheadNpcInfo NpcInfo_GetCachedValue(uint entryID)
        {
            lock (DBLock)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    if (dbConnection == null)
                        dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\wowhead.sqlite;Version=3;").OpenAndReturn();
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText =
                            "CREATE TABLE IF NOT EXISTS npcs ( entryID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL );" +
                            $"SELECT * FROM npcs WHERE entryID = {entryID};";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new WowheadNpcInfo(reader["name"].ToString());
                            }
                            else
                            {
                                return null;
                            }
                        } 
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("[Wowhead.NpcInfo_GetCachedValue] Error: " + ex.Message);
                    return null;
                }
                finally
                {
                    _sumDBAccessTicks += stopwatch.ElapsedTicks;
                    _sumDBAccessTime += stopwatch.ElapsedMilliseconds;
                    _numDBAccesses++;
                }
            }
        }

        private static void NpcInfo_SaveToCache(uint entryID, WowheadNpcInfo info)
        {
            lock (DBLock)
            {
                if (dbConnection == null)
                    dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\wowhead.sqlite;Version=3;").OpenAndReturn();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS npcs ( entryID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL );" +
                        $"INSERT INTO npcs (entryID, name) values ({entryID}, '{info.Name.Replace("'", "''")}');";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("[Wowhead.NpcInfo_SaveToCache] Error-0: " + ex.Message + $"\r\nINSERT INTO npcs (entryID, name) values ({entryID}, '{info.Name}')");
                    }

                }
            }
        }
        
    }
    
    internal class WowheadItemInfo
    {
        public WowheadItemInfo()
        {

        }

        internal WowheadItemInfo(string name, uint @class, uint level, uint quality)
        {
            Name = name;
            Class = @class;
            Level = level;
            Quality = quality;
        }
        
        internal string Name;
        
        internal uint Class;
        
        internal uint Level;
        
        internal uint Quality;
        
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

    internal class WowheadSpellInfo
    {
        internal WowheadSpellInfo()
        {

        }

        internal WowheadSpellInfo(string name)
        {
            Name = name;
        }
        
        internal string Name;
        
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
    
    internal class WowheadNpcInfo
    {
        internal WowheadNpcInfo()
        {

        }

        internal WowheadNpcInfo(string name)
        {
            Name = name.Replace(@"\'", "'");
        }
        
        internal string Name;

    }

}
