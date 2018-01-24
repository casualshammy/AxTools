using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
        private static Log2 log = new Log2("Wowhead");
        private static Timer updateDBEntriesTimer;

        static Wowhead()
        {
            _locale = GetLocale();
            updateDBEntriesTimer = new Timer(UpdateDBEntriesTimer_Elapsed, null, 60 * 60 * 1000, 60 * 60 * 1000); // 1 hour
            MainForm.ClosingEx +=
                delegate
                {
                    if (_numDBAccesses > 100)
                    {
                        log.Error(string.Format("DB usage stats: numDBAccesses: {0}; average access time: {1} ms/call; average access time: {2} ticks/call",
                            _numDBAccesses, _sumDBAccessTime/(_numDBAccesses == 0 ? -1 : _numDBAccesses), _sumDBAccessTicks/(_numDBAccesses == 0 ? -1 : _numDBAccesses)));
                    }
                };
        }

        private static void UpdateDBEntriesTimer_Elapsed(object state)
        {
            log.Info("Updating Wowhead db...");
            Dictionary<uint, string> npcs2 = new Dictionary<uint, string>();
            lock (DBLock)
            {
                try
                {
                    LoadAndUpdateDBFile();
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText = $"SELECT entryID, name FROM npcs2 ORDER BY RANDOM() LIMIT 1000;";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                npcs2.Add((uint)reader.GetInt32(0), reader.GetString(1));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("UpdateDBEntriesTimer_Elapsed() error-0: " + ex.Message);
                    return;
                }
            }
            foreach (var i in npcs2)
            {
                try
                {
                    if (TryGetNpcInfoFromWeb(i.Key, out WowheadNpcInfo info) && info.Name != i.Value)
                    {
                        log.Error($"NPC with id:{i.Key} has changed it's name from {i.Value} to {info.Name}");
                        lock (DBLock)
                        {
                            using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                            {
                                command.CommandText = $"UPDATE npcs2 SET name='{info.Name}' WHERE entryID={i.Key};";
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("UpdateDBEntriesTimer_Elapsed() error-1: " + ex.Message);
                }
            }
            log.Info("Wowhead db is updated");
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
                            log.Info("GetItemInfo(): regex isn't match: " + JsonConvert.SerializeObject(xml));
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
                            log.Info("GetSpellInfo(): regex isn't match: " + JsonConvert.SerializeObject(xml));
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
                    if (TryGetNpcInfoFromWeb(entryID, out info))
                    {
                        NpcInfo_SaveToCache(entryID, info);
                    }
                    else
                    {
                        info = new WowheadNpcInfo(UNKNOWN);
                    }
                }
                NpcInfos.TryAdd(entryID, info);
            }
            return info;
        }

        private static bool TryGetNpcInfoFromWeb(uint entryID, out WowheadNpcInfo info)
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
                    return true;
                }
                else
                {
                    log.Info("GetNpcInfo_Web(): regex isn't match: " + JsonConvert.SerializeObject(xml));
                    info = null;
                    return false;
                }
            }
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
                LoadAndUpdateDBFile();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText = 
                        $"INSERT INTO items2 (itemID, name, class, level, quality, image, datetime_updated) " +
                        $"VALUES ({itemID}, '{info.Name.Replace("'", "''")}', {info.Class}, {info.Level}, {info.Quality}, '{JsonConvert.SerializeObject(info.ImageBytes)}', '{DateTime.UtcNow.ToString("s")}')";
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
                Stopwatch stopwatch = Stopwatch.StartNew();
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
                LoadAndUpdateDBFile();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText = 
                        $"INSERT INTO spells2 (spellID, name, image, datetime_updated) " +
                        $"VALUES ({spellID}, '{info.Name.Replace("'", "''")}', '{JsonConvert.SerializeObject(info.ImageBytes)}', '{DateTime.UtcNow.ToString("s")}')";
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
                Stopwatch stopwatch = Stopwatch.StartNew();
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
                LoadAndUpdateDBFile();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText = $"INSERT INTO zones2 (zoneID, name, datetime_updated) values ({zoneID}, '{zoneText.Replace("'", "''")}', '{DateTime.UtcNow.ToString("s")}');";
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

        private static WowheadNpcInfo NpcInfo_GetCachedValue(uint entryID)
        {
            lock (DBLock)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    LoadAndUpdateDBFile();
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText = $"SELECT name FROM npcs2 WHERE entryID = {entryID} LIMIT 1;";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new WowheadNpcInfo(reader.GetString(0));
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
                    log.Error("NpcInfo_GetCachedValue() error: " + ex.Message);
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
                LoadAndUpdateDBFile();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText = $"INSERT INTO npcs2 (entryID, name, datetime_updated) values ({entryID}, '{info.Name.Replace("'", "''")}', '{DateTime.UtcNow.ToString("s")}');";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        log.Error($"NpcInfo_SaveToCache() error: {ex.Message}\r\n{command.CommandText}");
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
                        "CREATE TABLE IF NOT EXISTS zones2 ( zoneID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL );" +
                        "CREATE TABLE IF NOT EXISTS npcs2 ( entryID INTEGER NOT NULL PRIMARY KEY, name TEXT NOT NULL );";
                    command.ExecuteNonQuery();
                    // checking if old databaseses exist
                    Dictionary<string, string[]> oldTables = new Dictionary<string, string[]>()
                    {
                        { "items", new string[] { "itemID", "name", "class", "level", "quality", "image" } },
                        { "spells", new string[] { "spellID", "name", "image" } },
                        { "zones", new string[] { "zoneID", "name" } },
                        { "npcs", new string[] { "entryID", "name" } },
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
