using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AxTools.Forms.Helpers;
using AxTools.Services.PingerHelpers;
using KeyboardWatcher;
using MetroFramework;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AxTools.Helpers
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Settings2
    {
        private static readonly Log2 logger = new Log2("Settings2");
        private static readonly object _lock = new object();
        private static Settings2 _instance;
        internal static Settings2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            string settingsFile = AppFolders.ConfigDir + "\\settings.json";
                            if (File.Exists(settingsFile))
                            {
                                try
                                {
                                    string rawText = File.ReadAllText(settingsFile, Encoding.UTF8);
                                    _instance = JsonConvert.DeserializeObject<Settings2>(rawText);
                                    logger.Info("Settings2 file is loaded");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Cannot load settings file: " + ex.Message);
                                    return null;
                                }
                            }
                            else
                            {
                                _instance = new Settings2();
                                logger.Info("Settings2 file is not found!");
                            }
                            _instance.ValidateAndFix();
                        }
                    }
                }
                return _instance;
            }
        }

        private Settings2()
        {

        }

        internal void ValidateAndFix()
        {
            if (MainWindowLocation.X < 0 || MainWindowLocation.Y < 0)
            {
                MainWindowLocation = new Point(100, 100);
            }
            if (WoWAddonsBackupCompressionLevel < 0)
            {
                WoWAddonsBackupCompressionLevel = 0;
            }
            if (WoWAddonsBackupCompressionLevel > 9)
            {
                WoWAddonsBackupCompressionLevel = 9;
            }
            if (string.IsNullOrWhiteSpace(UserID))
            {
                UserID = Environment.MachineName + "-" + Utils.GetRandomString(10, false).ToUpper();
            }
            if (PingerServerID > GameServers.Entries.Length - 1)
            {
                PingerServerID = GameServers.Entries.Length - 1;
            }
        }

        internal void SaveJSON()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    JsonSerializer js = new JsonSerializer();
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 4;
                    js.Serialize(jsonWriter, _instance);
                }
            }
            string json = sb.ToString();
            File.WriteAllText(AppFolders.ConfigDir + "\\settings.json", json, Encoding.UTF8);
            logger.Info("Settings2 file has been updated, time: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        #region General

        [JsonProperty(Order = 0, PropertyName = "UserID")]
        internal string UserID = string.Empty;

        [JsonProperty(Order = 1, PropertyName = "LastUsedVersion")]
        internal VersionExt LastUsedVersion = new VersionExt(0, 0, 0);

        [JsonProperty(Order = 2, PropertyName = "MainWindowLocation")]
        internal Point MainWindowLocation = new Point(100, 100);

        [JsonProperty(Order = 3, PropertyName = "StyleColor")]
        internal MetroColorStyle StyleColor = MetroColorStyle.Blue;

        [JsonProperty(Order = 4, PropertyName = "MinimizeToTray")]
        internal bool MinimizeToTray = true;

        #endregion

        #region WoW

        [JsonProperty(Order = 10, PropertyName = "WoWDirectory")]
        internal string WoWDirectory = string.Empty;

        [JsonProperty(Order = 11, PropertyName = "WoWAccounts2")]
        internal byte[] WoWAccounts2 = new byte[0];

        [JsonProperty(Order = 13, PropertyName = "WoWAntiKick")]
        internal bool WoWAntiKick = true;

        [JsonProperty(Order = 14, PropertyName = "WoW_AntiKick_SetAfkState")]
        internal bool WoW_AntiKick_SetAfkState = false;

        [JsonProperty(Order = 15, PropertyName = "WoWInteractMouseover")]
        internal Keys WoWInteractMouseover = Keys.None;

        [JsonProperty(Order = 16, PropertyName = "WoWTargetMouseover")]
        internal Keys WoWTargetMouseover = Keys.None;

        [JsonProperty(Order = 17, PropertyName = "WoWCustomizeWindow")]
        internal bool WoWCustomizeWindow = false;

        [JsonProperty(Order = 18, PropertyName = "WoWCustomWindowRectangle")]
        internal Rectangle WoWCustomWindowRectangle = new Rectangle(0, 0, 1366, 733);

        [JsonProperty(Order = 19, PropertyName = "WoWCustomWindowNoBorder")]
        internal bool WoWCustomWindowNoBorder = false;
        
        #endregion

        #region Clicker

        [JsonProperty(Order = 20, PropertyName = "ClickerHotkey")]
        internal KeyExt ClickerHotkey = new KeyExt(Keys.None);

        [JsonProperty(Order = 21, PropertyName = "ClickerInterval")]
        internal int ClickerInterval = 0x3e8;

        [JsonProperty(Order = 22, PropertyName = "ClickerKey")]
        internal Keys ClickerKey = Keys.None;

        #endregion

        #region VoIP
        
        [JsonProperty(Order = 30, PropertyName = "MumbleDirectory")]
        internal string MumbleDirectory = string.Empty;

        [JsonProperty(Order = 31, PropertyName = "MumbleStartWithWoW")]
        internal bool MumbleStartWithWoW = false;

        [JsonProperty(Order = 32, PropertyName = "RaidcallDirectory")]
        internal string RaidcallDirectory = string.Empty;

        [JsonProperty(Order = 33, PropertyName = "RaidcallStartWithWoW")]
        internal bool RaidcallStartWithWoW = false;

        [JsonProperty(Order = 34, PropertyName = "TS3Directory")]
        internal string TS3Directory = string.Empty;

        [JsonProperty(Order = 35, PropertyName = "TS3StartWithWoW")]
        internal bool TS3StartWithWoW = false;

        [JsonProperty(Order = 36, PropertyName = "VentriloDirectory")]
        internal string VentriloDirectory = string.Empty;

        [JsonProperty(Order = 37, PropertyName = "VentriloStartWithWoW")]
        internal bool VentriloStartWithWoW = false;

        [JsonProperty(Order = 38, PropertyName = "StartTwitchWithWoW")]
        internal bool StartTwitchWithWoW = false;

        #endregion

        #region WoWPlugins

        [JsonProperty(Order = 71, PropertyName = "WoWPluginShowIngameNotifications")]
        internal bool WoWPluginShowIngameNotifications = true;
        
        internal event Action PluginHotkeysChanged;
        internal void InvokePluginHotkeysChanged()
        {
            PluginHotkeysChanged?.Invoke();
        }
        [JsonProperty(Order = 73, PropertyName = "PluginHotkeys")]
        internal Dictionary<string, KeyExt> PluginHotkeys = new Dictionary<string, KeyExt>();

        #endregion

        #region Pinger

        [JsonProperty(Order = 80, PropertyName = "PingerServerID")]
        internal int PingerServerID = 1;

        [JsonProperty(Order = 81, PropertyName = "PingerBadPing")]
        internal int PingerBadPing = 125;

        [JsonProperty(Order = 82, PropertyName = "PingerBadPacketLoss")]
        internal int PingerBadPacketLoss = 5;

        [JsonProperty(Order = 83, PropertyName = "PingerVeryBadPing")]
        internal int PingerVeryBadPing = 250;

        [JsonProperty(Order = 84, PropertyName = "PingerVeryBadPacketLoss")]
        internal int PingerVeryBadPacketLoss = 10;

        [JsonProperty(Order = 85, PropertyName = "PingerLastWoWServerIP")]
        internal string PingerLastWoWServerIP = "8.8.8.8";

        #endregion

        #region AddonsBackup

        [JsonProperty(Order = 90, PropertyName = "WoWAddonsBackupPath")]
        internal string WoWAddonsBackupPath = AppFolders.UserfilesDir;

        [JsonProperty(Order = 91, PropertyName = "WoWAddonsBackupLastDate")]
        internal DateTime WoWAddonsBackupLastDate = new DateTime(1970, 1, 1);

        [JsonProperty(Order = 92, PropertyName = "WoWAddonsBackupIsActive")]
        internal bool WoWAddonsBackupIsActive = true;

        [JsonProperty(Order = 93, PropertyName = "WoWAddonsBackupNumberOfArchives")]
        internal int WoWAddonsBackupNumberOfArchives = 7;

        [JsonProperty(Order = 94, PropertyName = "WoWAddonsBackupMinimumTimeBetweenBackup")]
        internal int WoWAddonsBackupMinimumTimeBetweenBackup = 24;

        [JsonProperty(Order = 95, PropertyName = "WoWAddonsBackupCompressionLevel")]
        internal int WoWAddonsBackupCompressionLevel = 6;

        [JsonProperty(Order = 96, PropertyName = "WoWAddonsBackup_DoNotCreateBackupWhileWoWClientIsRunning")]
        internal bool WoWAddonsBackup_DoNotCreateBackupWhileWoWClientIsRunning = true;

        #endregion
        
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class Settings
    {
        private static readonly Log2 logger = new Log2("Settings");
        private static readonly object _lock = new object();
        private static Settings _instance;
        internal static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            string settingsFile = AppFolders.ConfigDir + "\\settings.json";
                            if (File.Exists(settingsFile))
                            {
                                try
                                {
                                    string rawText = File.ReadAllText(settingsFile, Encoding.UTF8);
                                    _instance = JsonConvert.DeserializeObject<Settings>(rawText);
                                    logger.Info("Settings file is loaded");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Cannot load settings file: " + ex.Message);
                                    return null;
                                }
                            }
                            else
                            {
                                _instance = new Settings();
                                logger.Info("Settings file is not found!");
                            }
                            _instance.ValidateAndFix();
                        }
                    }
                }
                return _instance;
            }
        }

        public Settings()
        {

        }

        internal void ValidateAndFix()
        {
            if (MainWindowLocation.X < 0 || MainWindowLocation.Y < 0)
            {
                MainWindowLocation = new Point(100, 100);
            }
            if (WoWAddonsBackupCompressionLevel < 0)
            {
                WoWAddonsBackupCompressionLevel = 0;
            }
            if (WoWAddonsBackupCompressionLevel > 9)
            {
                WoWAddonsBackupCompressionLevel = 9;
            }
            if (string.IsNullOrWhiteSpace(WoWDirectory))
            {
                WoWDirectory = GetWowPath();
            }
            if (string.IsNullOrWhiteSpace(VentriloDirectory))
            {
                VentriloDirectory = GetVentriloPath();
            }
            if (string.IsNullOrWhiteSpace(MumbleDirectory))
            {
                MumbleDirectory = GetMumblePath();
            }
            if (string.IsNullOrWhiteSpace(RaidcallDirectory))
            {
                RaidcallDirectory = GetRaidcallPath();
            }
            if (string.IsNullOrWhiteSpace(TS3Directory))
            {
                TS3Directory = GetTeamspeakPath();
            }
            if (string.IsNullOrWhiteSpace(UserID))
            {
                UserID = Environment.MachineName + "___" + Utils.GetRandomString(10, false).ToUpper();
            }
            if (PingerServerID > GameServers.Entries.Length - 1)
            {
                PingerServerID = GameServers.Entries.Length - 1;
            }
        }

        internal void SaveJSON()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    JsonSerializer js = new JsonSerializer();
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 4;
                    js.Serialize(jsonWriter, _instance);
                }
            }
            string json = sb.ToString();
            File.WriteAllText(AppFolders.ConfigDir + "\\settings.json", json, Encoding.UTF8);
            logger.Info("Settings file has been updated, time: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        #region General

        [JsonProperty(Order = 0, PropertyName = "UserID")]
        internal string UserID = string.Empty;

        [JsonProperty(Order = 1, PropertyName = "LastUsedVersion")]
        internal VersionExt LastUsedVersion = new VersionExt(0, 0, 0);

        [JsonProperty(Order = 2, PropertyName = "MainWindowLocation")]
        internal Point MainWindowLocation = new Point(100, 100);

        [JsonProperty(Order = 3, PropertyName = "StyleColor")]
        internal MetroColorStyle StyleColor = MetroColorStyle.Blue;

        [JsonProperty(Order = 4, PropertyName = "MinimizeToTray")]
        internal bool MinimizeToTray = true;

        #endregion

        #region WoW

        [JsonProperty(Order = 10, PropertyName = "WoWDirectory")]
        internal string WoWDirectory = string.Empty;

        [JsonProperty(Order = 11, PropertyName = "WoWAccounts")]
        internal byte[] WoWAccounts = new byte[0];

        [JsonProperty(Order = 12, PropertyName = "WoWAntiKick")]
        internal bool WoWAntiKick = true;

        [JsonProperty(Order = 13, PropertyName = "WoW_AntiKick_SetAfkState")]
        internal bool WoW_AntiKick_SetAfkState = false;

        [JsonProperty(Order = 14, PropertyName = "WoWInteractMouseover")]
        internal Keys WoWInteractMouseover = Keys.None;

        [JsonProperty(Order = 15, PropertyName = "WoWTargetMouseover")]
        internal Keys WoWTargetMouseover = Keys.None;

        [JsonProperty(Order = 16, PropertyName = "WoWCustomizeWindow")]
        internal bool WoWCustomizeWindow = false;

        [JsonProperty(Order = 17, PropertyName = "WoWCustomWindowRectangle")]
        internal Rectangle WoWCustomWindowRectangle = new Rectangle(0, 0, 1366, 733);

        [JsonProperty(Order = 18, PropertyName = "WoWCustomWindowNoBorder")]
        internal bool WoWCustomWindowNoBorder = false;

        #endregion

        #region Clicker

        [JsonProperty(Order = 20, PropertyName = "ClickerHotkey")]
        internal KeyExt ClickerHotkey = new KeyExt(Keys.None);

        [JsonProperty(Order = 21, PropertyName = "ClickerInterval")]
        internal int ClickerInterval = 0x3e8;

        [JsonProperty(Order = 22, PropertyName = "ClickerKey")]
        internal Keys ClickerKey = Keys.None;

        #endregion

        #region VoIP

        [JsonProperty(Order = 30, PropertyName = "MumbleDirectory")]
        internal string MumbleDirectory = string.Empty;

        [JsonProperty(Order = 31, PropertyName = "MumbleStartWithWoW")]
        internal bool MumbleStartWithWoW = false;

        [JsonProperty(Order = 32, PropertyName = "RaidcallDirectory")]
        internal string RaidcallDirectory = string.Empty;

        [JsonProperty(Order = 33, PropertyName = "RaidcallStartWithWoW")]
        internal bool RaidcallStartWithWoW = false;

        [JsonProperty(Order = 34, PropertyName = "TS3Directory")]
        internal string TS3Directory = string.Empty;

        [JsonProperty(Order = 35, PropertyName = "TS3StartWithWoW")]
        internal bool TS3StartWithWoW = false;

        [JsonProperty(Order = 36, PropertyName = "VentriloDirectory")]
        internal string VentriloDirectory = string.Empty;

        [JsonProperty(Order = 37, PropertyName = "VentriloStartWithWoW")]
        internal bool VentriloStartWithWoW = false;

        #endregion

        #region Radar

        private readonly ObservableCollection<RadarObject> wowRadarList = new ObservableCollection<RadarObject>();
        [JsonProperty(Order = 50, PropertyName = "WoWRadarList")]
        internal ObservableCollection<RadarObject> WoWRadarList
        {
            get
            {
                return wowRadarList;
            }
            set
            {
                foreach (RadarObject o in value)
                {
                    wowRadarList.Add(o);
                }
            }
        }

        [JsonProperty(Order = 51, PropertyName = "WoWRadarLocation")]
        internal Point WoWRadarLocation = Point.Empty;

        [JsonProperty(Order = 52, PropertyName = "WoWRadarShowPlayersClasses")]
        internal bool WoWRadarShowPlayersClasses = true;

        [JsonProperty(Order = 53, PropertyName = "WoWRadarShowNPCsNames")]
        internal bool WoWRadarShowNPCsNames = true;

        [JsonProperty(Order = 54, PropertyName = "WoWRadarShowObjectsNames")]
        internal bool WoWRadarShowObjectsNames = true;

        [JsonProperty(Order = 55, PropertyName = "WoWRadarShowMode")]
        internal RadarShowMode WoWRadarShowMode = new RadarShowMode { Corpses = true, Enemies = true, Friends = true, Npcs = true, Objects = true, Zoom = 0.5f };

        [JsonProperty(Order = 56, PropertyName = "WoWRadarFriendColor")]
        internal Color WoWRadarFriendColor = Color.Green;

        [JsonProperty(Order = 57, PropertyName = "WoWRadarEnemyColor")]
        internal Color WoWRadarEnemyColor = Color.Red;

        [JsonProperty(Order = 58, PropertyName = "WoWRadarNPCColor")]
        internal Color WoWRadarNPCColor = Color.GreenYellow;

        [JsonProperty(Order = 59, PropertyName = "WoWRadarObjectColor")]
        internal Color WoWRadarObjectColor = Color.Gold;

        [JsonProperty(Order = 60, PropertyName = "WoWRadarAlarmSoundFile")]
        internal string WoWRadarAlarmSoundFile = AppFolders.ResourcesDir + "\\alarm.wav";

        [JsonProperty(Order = 61, PropertyName = "WoWRadarShowLocalPlayerRotationArrowOnTop")]
        internal bool WoWRadarShowLocalPlayerRotationArrowOnTop = false;

        #endregion

        #region WoWPlugins

        [JsonProperty(Order = 71, PropertyName = "WoWPluginShowIngameNotifications")]
        internal bool WoWPluginShowIngameNotifications = true;

        [JsonProperty(Order = 72, PropertyName = "PluginsUsageStat2")]
        internal Dictionary<string, List<DateTime>> PluginsUsageStat2 = new Dictionary<string, List<DateTime>>();

        internal event Action PluginHotkeysChanged;
        internal void InvokePluginHotkeysChanged()
        {
            PluginHotkeysChanged?.Invoke();
        }
        [JsonProperty(Order = 73, PropertyName = "PluginHotkeys")]
        internal Dictionary<string, KeyExt> PluginHotkeys = new Dictionary<string, KeyExt>();

        #endregion

        #region Pinger

        [JsonProperty(Order = 80, PropertyName = "PingerServerID")]
        internal int PingerServerID = 1;

        [JsonProperty(Order = 81, PropertyName = "PingerBadPing")]
        internal int PingerBadPing = 125;

        [JsonProperty(Order = 82, PropertyName = "PingerBadPacketLoss")]
        internal int PingerBadPacketLoss = 5;

        [JsonProperty(Order = 83, PropertyName = "PingerVeryBadPing")]
        internal int PingerVeryBadPing = 250;

        [JsonProperty(Order = 84, PropertyName = "PingerVeryBadPacketLoss")]
        internal int PingerVeryBadPacketLoss = 10;

        [JsonProperty(Order = 85, PropertyName = "PingerLastWoWServerIP")]
        internal string PingerLastWoWServerIP = "8.8.8.8";

        #endregion

        #region AddonsBackup

        [JsonProperty(Order = 90, PropertyName = "WoWAddonsBackupPath")]
        internal string WoWAddonsBackupPath = AppFolders.UserfilesDir;

        [JsonProperty(Order = 91, PropertyName = "WoWAddonsBackupLastDate")]
        internal DateTime WoWAddonsBackupLastDate = new DateTime(1970, 1, 1);

        [JsonProperty(Order = 92, PropertyName = "WoWAddonsBackupIsActive")]
        internal bool WoWAddonsBackupIsActive = true;

        [JsonProperty(Order = 93, PropertyName = "WoWAddonsBackupNumberOfArchives")]
        internal int WoWAddonsBackupNumberOfArchives = 7;

        [JsonProperty(Order = 94, PropertyName = "WoWAddonsBackupMinimumTimeBetweenBackup")]
        internal int WoWAddonsBackupMinimumTimeBetweenBackup = 24;

        [JsonProperty(Order = 95, PropertyName = "WoWAddonsBackupCompressionLevel")]
        internal int WoWAddonsBackupCompressionLevel = 6;

        [JsonProperty(Order = 96, PropertyName = "WoWAddonsBackup_DoNotCreateBackupWhileWoWClientIsRunning")]
        internal bool WoWAddonsBackup_DoNotCreateBackupWhileWoWClientIsRunning = true;

        #endregion

        #region Methods - Paths

        private static string GetTeamspeakPath()
        {
            using (RegistryKey regVersion = Registry.ClassesRoot.CreateSubKey("ts3server\\\\shell\\\\open\\\\command"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("") != null)
                    {
                        Regex regex = new Regex("\"(.+)\" .*");
                        Match match = regex.Match(regVersion.GetValue("").ToString());
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }
                    }
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        private static string GetRaidcallPath()
        {
            using (RegistryKey regVersion = Registry.ClassesRoot.CreateSubKey("raidcall\\\\shell\\\\open\\\\command"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("") != null)
                    {
                        Regex regex = new Regex("\"(.+)\" .*");
                        Match match = regex.Match(regVersion.GetValue("").ToString());
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }
                    }
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        private static string GetVentriloPath()
        {
            using (RegistryKey regVersion = Registry.ClassesRoot.CreateSubKey("Ventrilo\\\\shell\\\\open\\\\command"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("") != null)
                    {
                        Regex regex = new Regex("(.+) .*");
                        Match match = regex.Match(regVersion.GetValue("").ToString());
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }
                    }
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        private static string GetMumblePath()
        {
            using (RegistryKey regVersion = Registry.ClassesRoot.CreateSubKey("mumble\\\\shell\\\\open\\\\command"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("") != null)
                    {
                        Regex regex = new Regex("\"(.+)\" .*");
                        Match match = regex.Match(regVersion.GetValue("").ToString());
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }
                    }
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        private static string GetDiscordPath()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord\\Update.exe");
            logger.Info($"Looking for Discord client in {path}");
            return File.Exists(path) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord") : string.Empty;
        }

        private static string GetWowPath()
        {
            using (RegistryKey regVersion = Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Wow6432Node\\\\Blizzard Entertainment\\\\World of Warcraft"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("InstallPath") != null)
                    {
                        string raw = regVersion.GetValue("InstallPath").ToString();
                        return raw.Remove(raw.Length - 1);
                    }
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        #endregion

    }

}
