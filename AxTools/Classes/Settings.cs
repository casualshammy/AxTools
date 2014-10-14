using AxTools.Helpers;
using AxTools.WoW;
using MetroFramework;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AxTools.Classes
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Settings
    {
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
                            string settingsFile = Globals.CfgPath + "\\settings.json";
                            if (File.Exists(settingsFile))
                            {
                                string rawText = File.ReadAllText(settingsFile, Encoding.UTF8);
                                _instance = JsonConvert.DeserializeObject<Settings>(rawText);
                                Log.Print("Settings file is loaded");
                            }
                            else
                            {
                                _instance = new Settings();
                                Log.Print("Settings file is not found!");
                            }
                            _instance.ValidateAndFix();
                        }
                    }
                }
                return _instance;
            }
        }

        private Settings()
        {
            
        }

        internal void ValidateAndFix()
        {
            if (MainWindowLocation.X < 0 || MainWindowLocation.Y < 0)
            {
                MainWindowLocation = new Point(100, 100);
            }
            if (WoWLuaConsoleWindowSize.Height < 354)
            {
                WoWLuaConsoleWindowSize.Height = 354;
            }
            if (WoWLuaConsoleWindowSize.Width < 650)
            {
                WoWLuaConsoleWindowSize.Width = 650;
            }
            if (WoWLuaConsoleTimerInterval < 50)
            {
                WoWLuaConsoleTimerInterval = 50;
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
                UserID = Environment.MachineName + "___" + Utils.GetRandomString(10).ToUpper();
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
            Utils.CheckCreateDir();
            File.WriteAllText(Globals.CfgPath + "\\settings.json", json, Encoding.UTF8);
            Log.Print("Settings file has been updated, time: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        #region General

        [JsonProperty(Order = 0, PropertyName = "UserID")]
        internal string UserID = String.Empty;

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

        [JsonProperty(Order = 5, PropertyName = "WoWDirectory")]
        internal string WoWDirectory = String.Empty;

        [JsonProperty(Order = 6, PropertyName = "WoWAccounts")]
        internal byte[] WoWAccounts = new byte[0];

        [JsonProperty(Order = 7, PropertyName = "WoWAntiKick")]
        internal bool WoWAntiKick = true;

        [JsonProperty(Order = 8, PropertyName = "WoWWipeCreatureCache")]
        internal bool WoWWipeCreatureCache = false;

        [JsonProperty(Order = 9, PropertyName = "WoWNotifyIfBigLogs")]
        internal bool WoWNotifyIfBigLogs = false;

        [JsonProperty(Order = 10, PropertyName = "WoWNotifyIfBigLogsSize")]
        internal int WoWNotifyIfBigLogsSize = 500;

        [JsonProperty(Order = 11, PropertyName = "WoWCustomizeWindow")]
        internal bool WoWCustomizeWindow = false;

        [JsonProperty(Order = 12, PropertyName = "WoWCustomWindowSize")]
        internal Point WoWCustomWindowSize = new Point(800, 600);

        [JsonProperty(Order = 13, PropertyName = "WoWCustomWindowNoBorder")]
        internal bool WoWCustomWindowNoBorder = false;

        [JsonProperty(Order = 14, PropertyName = "WoWCustomWindowLocation")]
        internal Point WoWCustomWindowLocation = Point.Empty;

        #endregion

        #region Clicker

        [JsonProperty(Order = 15, PropertyName = "ClickerHotkey")]
        internal Keys ClickerHotkey = Keys.None;

        [JsonProperty(Order = 16, PropertyName = "ClickerInterval")]
        internal int ClickerInterval = 0x3e8;

        [JsonProperty(Order = 17, PropertyName = "ClickerKey")]
        internal Keys ClickerKey = Keys.None;

        #endregion

        #region VoIP

        [JsonProperty(Order = 18, PropertyName = "MumbleDirectory")]
        internal string MumbleDirectory = String.Empty;

        [JsonProperty(Order = 19, PropertyName = "MumbleStartWithWoW")]
        internal bool MumbleStartWithWoW = false;



        [JsonProperty(Order = 20, PropertyName = "RaidcallDirectory")]
        internal string RaidcallDirectory = String.Empty;

        [JsonProperty(Order = 21, PropertyName = "RaidcallStartWithWoW")]
        internal bool RaidcallStartWithWoW = false;



        [JsonProperty(Order = 22, PropertyName = "TS3Directory")]
        internal string TS3Directory = String.Empty;

        [JsonProperty(Order = 23, PropertyName = "TS3StartWithWoW")]
        internal bool TS3StartWithWoW = false;



        [JsonProperty(Order = 24, PropertyName = "VentriloDirectory")]
        internal string VentriloDirectory = String.Empty;

        [JsonProperty(Order = 25, PropertyName = "VentriloStartWithWoW")]
        internal bool VentriloStartWithWoW = false;

        #endregion

        #region LuaConsole

        [JsonProperty(Order = 26, PropertyName = "WoWLuaConsoleWindowSize")]
        internal Size WoWLuaConsoleWindowSize = new Size(650, 354);

        [JsonProperty(Order = 27, PropertyName = "WoWLuaConsoleTimerInterval")]
        internal int WoWLuaConsoleTimerInterval = 1000;

        [JsonProperty(Order = 28, PropertyName = "WoWLuaConsoleTimerRnd")]
        internal bool WoWLuaConsoleTimerRnd = false;

        internal event Action<Keys> LuaTimerHotkeyChanged;
        private Keys luaTimerHotkey = Keys.None;
        [JsonProperty(Order = 29, PropertyName = "WoWLuaConsoleTimerHotkey")]
        internal Keys LuaTimerHotkey
        {
            get
            {
                return luaTimerHotkey;
            }
            set
            {
                luaTimerHotkey = value;
                if (LuaTimerHotkeyChanged != null)
                {
                    LuaTimerHotkeyChanged(value);
                }
            }
        }

        [JsonProperty(Order = 30, PropertyName = "WoWLuaConsoleIgnoreGameState")]
        internal bool WoWLuaConsoleIgnoreGameState = false;

        [JsonProperty(Order = 31, PropertyName = "WoWLuaConsoleShowIngameNotifications")]
        internal bool WoWLuaConsoleShowIngameNotifications = true;

        [JsonProperty(Order = 32, PropertyName = "WoWLuaConsoleLastText")]
        internal string WoWLuaConsoleLastText = string.Empty;

        #endregion

        #region Radar

        internal EventHandler WoWRadarListChanged;
        private List<ObjectToFind> wowRadarList = new List<ObjectToFind>();
        [JsonProperty(Order = 33, PropertyName = "WoWRadarList")]
        internal List<ObjectToFind> WoWRadarList
        {
            get
            {
                return wowRadarList;
            }
            set
            {
                wowRadarList = value;
                if (WoWRadarListChanged != null)
                {
                    WoWRadarListChanged(null, null);
                }
            }
        }

        [JsonProperty(Order = 34, PropertyName = "WoWRadarLocation")]
        internal Point WoWRadarLocation = Point.Empty;

        [JsonProperty(Order = 35, PropertyName = "WoWRadarShowPlayersClasses")]
        internal bool WoWRadarShowPlayersClasses = true;

        [JsonProperty(Order = 36, PropertyName = "WoWRadarShowNPCsNames")]
        internal bool WoWRadarShowNPCsNames = true;

        [JsonProperty(Order = 37, PropertyName = "WoWRadarShowObjectsNames")]
        internal bool WoWRadarShowObjectsNames = true;

        [JsonProperty(Order = 38, PropertyName = "WoWRadarShowMode")]
        internal ulong WoWRadarShowMode = 0;

        [JsonProperty(Order = 39, PropertyName = "WoWRadarFriendColor")]
        internal Color WoWRadarFriendColor = Color.Green;

        [JsonProperty(Order = 40, PropertyName = "WoWRadarEnemyColor")]
        internal Color WoWRadarEnemyColor = Color.Red;

        [JsonProperty(Order = 41, PropertyName = "WoWRadarNPCColor")]
        internal Color WoWRadarNPCColor = Color.GreenYellow;

        [JsonProperty(Order = 42, PropertyName = "WoWRadarObjectColor")]
        internal Color WoWRadarObjectColor = Color.Gold;

        #endregion

        #region AddonsBackup

        [JsonProperty(Order = 43, PropertyName = "WoWAddonsBackupPath")]
        internal string WoWAddonsBackupPath = Globals.UserfilesPath;

        [JsonProperty(Order = 44, PropertyName = "WoWAddonsBackupLastDate")]
        internal DateTime WoWAddonsBackupLastDate = new DateTime(1970, 1, 1);

        [JsonProperty(Order = 45, PropertyName = "WoWAddonsBackupIsActive")]
        internal bool WoWAddonsBackupIsActive = true;

        [JsonProperty(Order = 46, PropertyName = "WoWAddonsBackupNumberOfArchives")]
        internal int WoWAddonsBackupNumberOfArchives = 7;

        [JsonProperty(Order = 47, PropertyName = "WoWAddonsBackupMinimumTimeBetweenBackup")]
        internal int WoWAddonsBackupMinimumTimeBetweenBackup = 24;

        [JsonProperty(Order = 48, PropertyName = "WoWAddonsBackupCompressionLevel")]
        internal int WoWAddonsBackupCompressionLevel = 6;

        #endregion

        #region WoWPlugins

        internal event Action<Keys> WoWPluginHotkeyChanged;
        private Keys wowPluginHotkey = Keys.None;
        [JsonProperty(Order = 49, PropertyName = "WoWPluginHotkey")]
        internal Keys WoWPluginHotkey
        {
            get
            {
                return wowPluginHotkey;
            }
            set
            {
                wowPluginHotkey = value;
                if (WoWPluginHotkeyChanged != null)
                {
                    WoWPluginHotkeyChanged(value);
                }
            }
        }

        [JsonProperty(Order = 50, PropertyName = "WoWPluginShowIngameNotifications")]
        internal bool WoWPluginShowIngameNotifications = true;

        #endregion

        #region Pinger

        [JsonProperty(Order = 51, PropertyName = "PingerServerID")]
        internal int PingerServerID = 1;

        [JsonProperty(Order = 52, PropertyName = "PingerBadPing")]
        internal int PingerBadPing = 125;

        [JsonProperty(Order = 53, PropertyName = "PingerBadPacketLoss")]
        internal int PingerBadPacketLoss = 5;

        [JsonProperty(Order = 54, PropertyName = "PingerVeryBadPing")]
        internal int PingerVeryBadPing = 250;

        [JsonProperty(Order = 55, PropertyName = "PingerVeryBadPacketLoss")]
        internal int PingerVeryBadPacketLoss = 10;

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
                        string raw = regVersion.GetValue("").ToString();
                        return raw.Replace("\"", String.Empty).Replace("\\ts3client_win64.exe %1", String.Empty).Replace("\\ts3client_win32.exe %1", String.Empty);
                    }
                    return String.Empty;
                }
                catch
                {
                    return String.Empty;
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
                        // "C:\Program Files\RaidCall\StartRC.exe" "%1"
                        string raw = regVersion.GetValue("").ToString();
                        return raw.Replace("\"", String.Empty).Replace("\\StartRC.exe %1", String.Empty);
                    }
                    return String.Empty;
                }
                catch
                {
                    return String.Empty;
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
                        string raw = regVersion.GetValue("").ToString();
                        return raw.Replace("\"", String.Empty).Replace("\\Ventrilo.exe -l%1", String.Empty).Replace("PROGRA~1", "Program Files").Replace("PROGRA~2", "Program Files (x86)");
                    }
                    return String.Empty;
                }
                catch
                {
                    return String.Empty;
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
                        string raw = regVersion.GetValue("").ToString();
                        string path = raw.Replace("\\mumble.exe \"%1\"", String.Empty);
                        return path;
                    }
                    return String.Empty;
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        private static string GetWowPath()
        {
            using (RegistryKey regVersion = Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Blizzard Entertainment\\\\World of Warcraft"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("InstallPath") != null)
                    {
                        string raw = regVersion.GetValue("InstallPath").ToString();
                        return raw.Replace("\"", String.Empty).Replace("World of Warcraft\\", "World of Warcraft");
                    }
                    return String.Empty;
                }
                catch
                {
                    return String.Empty;
                }
            }
        }
    
        #endregion

    }
}
