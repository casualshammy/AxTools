using AxTools.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace AxTools.WoW.Helpers
{
    public static class Wowhead
    {
        private static readonly string _locale;
        private static readonly ConcurrentDictionary<uint, WowheadItemInfo> ItemInfos = new ConcurrentDictionary<uint, WowheadItemInfo>();
        private static readonly ConcurrentDictionary<int, WowheadSpellInfo> SpellInfos = new ConcurrentDictionary<int, WowheadSpellInfo>();
        private static readonly ConcurrentDictionary<uint, string> ZoneInfos = new ConcurrentDictionary<uint, string>();
        private const string UNKNOWN = "UNKNOWN";
        private static readonly object DBLock = new object();
        private static SQLiteConnection dbConnection;
        private static readonly Log2 log = new Log2(nameof(Wowhead));

        static Wowhead()
        {
            _locale = GetLocale();
        }

        public static WowheadItemInfo GetItemInfo(uint itemID)
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
                            info = new WowheadItemInfo(Regex.Unescape(match.Groups[1].Value), uint.Parse(match.Groups[4].Value), uint.Parse(match.Groups[2].Value), uint.Parse(match.Groups[3].Value));
                            using (MemoryStream ms = new MemoryStream(webClient.DownloadData("https://wow.zamimg.com/images/wow/icons/small/" + match.Groups[5].Value + ".jpg")))
                            {
                                info.ImageBytes = ms.ToArray();
                            }
                            ItemInfo_SaveToCache(itemID, info);
                        }
                        else
                        {
                            info = new WowheadItemInfo(UNKNOWN, 0, 0, 0);
                            log.Info("GetItemInfo(): regex isn't match: " + JsonConvert.SerializeObject(xml));
                        }
                    }
                }
                ItemInfos.TryAdd(itemID, info);
            }
            return info;
        }

        public static WowheadSpellInfo GetSpellInfo(int spellID)
        {
            if (!SpellInfos.TryGetValue(spellID, out WowheadSpellInfo info))
            {
                if ((info = SpellInfo_GetCachedValue(spellID)) == null)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        string xml = webClient.DownloadString("https://" + _locale + ".wowhead.com/spell=" + spellID + "&power");
                        Regex regex = new Regex("\"name_.+\":\"(.+)\",\"icon\":\"?(.+?)\"?,");
                        Match match = regex.Match(xml);
                        if (match.Success)
                        {
                            info = new WowheadSpellInfo(Regex.Unescape(match.Groups[1].Value));
                            if (!string.IsNullOrWhiteSpace(match.Groups[2].Value) && match.Groups[2].Value != "null")
                            {
                                using (MemoryStream ms = new MemoryStream(webClient.DownloadData("https://wow.zamimg.com/images/wow/icons/small/" + match.Groups[2].Value + ".jpg")))
                                {
                                    info.ImageBytes = ms.ToArray();
                                }
                            }
                            else
                            {
                                info.Image = Resources.DialogError;
                            }

                            SpellInfo_SaveToCache(spellID, info);
                        }
                        else
                        {
                            info = new WowheadSpellInfo(UNKNOWN);
                            log.Info("GetSpellInfo(): regex isn't match: " + JsonConvert.SerializeObject(xml));
                        }
                    }
                }
                SpellInfos.TryAdd(spellID, info);
            }
            return info;
        }

        public static string GetZoneText(uint zoneID)
        {
            if (!ZoneInfos.TryGetValue(zoneID, out string info))
            {
                if ((info = ZoneInfo_GetCachedValue(zoneID)) == null)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = Encoding.UTF8;
                        string xml = webClient.DownloadString($"https://{_locale}.wowhead.com/zone={zoneID}&power");
                        Regex regex = new Regex("\\s+name_.+:\\s*'(.+)',");
                        Match match = regex.Match(xml);
                        if (match.Success)
                        {
                            info = Regex.Unescape(match.Groups[1].Value);
                            ZoneInfo_SaveToCache(zoneID, info);
                        }
                        else
                        {
                            info = UNKNOWN;
                            log.Info("GetZoneText(): regex isn't match: " + JsonConvert.SerializeObject(xml));
                        }
                    }
                }
                ZoneInfos.TryAdd(zoneID, info);
            }
            return info;
        }

        private static string GetLocale()
        {
            string configWtfPath = Settings2.Instance.WoWDirectory + "\\WTF\\Config.wtf";
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
                try
                {
                    LoadAndUpdateDBFile();
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText = $"SELECT name,class,level,quality,image FROM items2 WHERE itemID={itemID} LIMIT 1;";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new WowheadItemInfo(reader.GetString(0), (uint)reader.GetInt32(1), (uint)reader.GetInt32(2), (uint)reader.GetInt32(3))
                                { ImageBytes = JsonConvert.DeserializeObject<byte[]>(reader.GetString(4)) };
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
                    log.Error("ItemInfo_GetCachedValue() error: " + ex.Message);
                    return null;
                }
            }
        }

        private static void ItemInfo_SaveToCache(uint itemID, WowheadItemInfo info)
        {
            lock (DBLock)
            {
                LoadAndUpdateDBFile();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText =
                        "INSERT INTO items2 (itemID, name, class, level, quality, image) " +
                        $"VALUES ({itemID}, '{info.Name.Replace("'", "''")}', {info.Class}, {info.Level}, {info.Quality}, '{JsonConvert.SerializeObject(info.ImageBytes)}')";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        log.Error($"ItemInfo_SaveToCache() error: {ex.Message}\r\n{command.CommandText}");
                    }
                }
            }
        }

        private static WowheadSpellInfo SpellInfo_GetCachedValue(int spellID)
        {
            lock (DBLock)
            {
                try
                {
                    LoadAndUpdateDBFile();
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText = $"SELECT name,image FROM spells2 WHERE spellID = {spellID} LIMIT 1;";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new WowheadSpellInfo(reader.GetString(0)) { ImageBytes = JsonConvert.DeserializeObject<byte[]>(reader.GetString(1)) };
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
                    log.Error("SpellInfo_GetCachedValue() error: " + ex.Message);
                    return null;
                }
            }
        }

        private static void SpellInfo_SaveToCache(int spellID, WowheadSpellInfo info)
        {
            lock (DBLock)
            {
                LoadAndUpdateDBFile();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText =
                        "INSERT INTO spells2 (spellID, name, image) " +
                        $"VALUES ({spellID}, '{info.Name.Replace("'", "''")}', '{JsonConvert.SerializeObject(info.ImageBytes)}')";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        log.Error($"SpellInfo_SaveToCache() error: {ex.Message}\r\n{command.CommandText}");
                    }
                }
            }
        }

        private static string ZoneInfo_GetCachedValue(uint zoneID)
        {
            lock (DBLock)
            {
                try
                {
                    LoadAndUpdateDBFile();
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText = $"SELECT name FROM zones2 WHERE zoneID = {zoneID} LIMIT 1;";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader.GetString(0);
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
                    log.Error("ZoneInfo_GetCachedValue() error: " + ex.Message);
                    return null;
                }
            }
        }

        private static void ZoneInfo_SaveToCache(uint zoneID, string zoneText)
        {
            lock (DBLock)
            {
                LoadAndUpdateDBFile();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText = $"INSERT INTO zones2 (zoneID, name) values ({zoneID}, '{zoneText.Replace("'", "''")}');";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        log.Error($"ZoneInfo_SaveToCache() error: {ex.Message}\r\n{command.CommandText}");
                    }
                }
            }
        }

        private static void LoadAndUpdateDBFile()
        {
            if (dbConnection == null)
            {
                dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\wowhead.sqlite;Version=3;").OpenAndReturn();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    // creating tables
                    command.CommandText =
                        "CREATE TABLE IF NOT EXISTS items2 ( itemID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL, class INTEGER NOT NULL, level INTEGER NOT NULL, quality INTEGER NOT NULL, image TEXT NOT NULL );" +
                        "CREATE TABLE IF NOT EXISTS spells2 ( spellID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL, image TEXT NOT NULL );" +
                        "CREATE TABLE IF NOT EXISTS zones2 ( zoneID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL );";
                    command.ExecuteNonQuery();
                    // checking if old databaseses exist
                    Dictionary<string, string[]> oldTables = new Dictionary<string, string[]>()
                    {
                        { "items", new[] { "itemID", "name", "class", "level", "quality", "image" } },
                        { "spells", new[] { "spellID", "name", "image" } },
                        { "zones", new[] { "zoneID", "name" } },
                    };
                    string migrationCommand = "BEGIN TRANSACTION;";
                    foreach (var s in oldTables)
                    {
                        command.CommandText = $"SELECT * FROM sqlite_master WHERE name ='{s.Key}' and type='table';";
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                log.Info($"Old table {s.Key} is found, migrating...");
                                migrationCommand += $"INSERT INTO {s.Key}2 ({string.Join(",", s.Value)}) SELECT {string.Join(",", s.Value)} FROM {s.Key};";
                                migrationCommand += $"DROP TABLE {s.Key};";
                            }
                        }
                    }
                    command.CommandText = migrationCommand + "COMMIT;";
                    command.ExecuteNonQuery();
                    // delete npcs2 table
                    foreach (var tableName in new[] { "npcs2", "npcs" })
                    {
                        command.CommandText = $"SELECT * FROM sqlite_master WHERE name ='{tableName}' and type='table';";
                        migrationCommand = "";
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                log.Info($"Old table '{tableName}' is found, let's drop it...");
                                migrationCommand = $"DROP TABLE {tableName};";
                            }
                        }
                        if (migrationCommand != "")
                        {
                            command.CommandText = migrationCommand;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }

    public class WowheadItemInfo
    {
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

        public string Name;

        public uint Class;

        public uint Level;

        public uint Quality;

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

        public Image Image;
    }

    public class WowheadSpellInfo
    {
        internal WowheadSpellInfo()
        {
        }

        internal WowheadSpellInfo(string name)
        {
            Name = name;
        }

        public string Name;

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

        public Image Image;
    }

}