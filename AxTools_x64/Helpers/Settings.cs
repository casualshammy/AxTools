using AxTools.Services.PingerHelpers;
using KeyboardWatcher;
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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AxTools.Helpers
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Settings2
    {
        private static readonly Log2 logger = new Log2(nameof(Settings2));
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
                            var settingsFile = AppFolders.ConfigDir + "\\settings.json";
                            if (File.Exists(settingsFile))
                            {
                                try
                                {
                                    var rawText = File.ReadAllText(settingsFile, Encoding.UTF8);
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

        internal static void SaveJSON()
        {
            var stopwatch = Stopwatch.StartNew();
            var sb = new StringBuilder(1024);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    var js = new JsonSerializer();
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 4;
                    js.Serialize(jsonWriter, _instance);
                }
            }
            var json = sb.ToString();
            File.WriteAllText(AppFolders.ConfigDir + "\\settings.json", json, Encoding.UTF8);
            logger.Info("Settings2 file has been updated, time: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        #region General

        [JsonProperty(Order = 0, PropertyName = nameof(UserID))]
        internal string UserID = string.Empty;

        [JsonProperty(Order = 1, PropertyName = nameof(LastUsedVersion))]
        internal VersionExt LastUsedVersion = new VersionExt(0, 0, 0);

        [JsonProperty(Order = 2, PropertyName = nameof(MainWindowLocation))]
        internal Point MainWindowLocation = new Point(100, 100);

        [JsonProperty(Order = 3, PropertyName = nameof(StyleColor))]
        internal MetroColorStyle StyleColor = MetroColorStyle.Blue;

        [JsonProperty(Order = 4, PropertyName = nameof(MinimizeToTray))]
        internal bool MinimizeToTray = true;

        [JsonProperty(Order = 5, PropertyName = nameof(SendLogToDeveloperOnShutdown))]
        internal bool SendLogToDeveloperOnShutdown = true;

        [JsonProperty(Order = 6, PropertyName = nameof(UACLevelWarningSuppress))]
        internal bool UACLevelWarningSuppress = false;

        #endregion General

        #region WoW

        [JsonProperty(Order = 10, PropertyName = nameof(WoWDirectory))]
        internal string WoWDirectory = string.Empty;

        [JsonProperty(Order = 11, PropertyName = nameof(WoWAccounts2))]
        internal byte[] WoWAccounts2 = new byte[0];

        [JsonProperty(Order = 13, PropertyName = nameof(WoWAntiKick))]
        internal bool WoWAntiKick = true;

        [JsonProperty(Order = 14, PropertyName = nameof(WoW_AntiKick_SetAfkState))]
        internal bool WoW_AntiKick_SetAfkState;

        [JsonProperty(Order = 15, PropertyName = nameof(WoWInteractMouseover))]
        internal Keys WoWInteractMouseover = Keys.None;

        [JsonProperty(Order = 16, PropertyName = nameof(WoWTargetMouseover))]
        internal Keys WoWTargetMouseover = Keys.None;

        [JsonProperty(Order = 17, PropertyName = nameof(WoWCustomizeWindow))]
        internal bool WoWCustomizeWindow;

        [JsonProperty(Order = 18, PropertyName = nameof(WoWCustomWindowRectangle))]
        internal Rectangle WoWCustomWindowRectangle = new Rectangle(0, 0, 1366, 733);

        [JsonProperty(Order = 19, PropertyName = nameof(WoWCustomWindowNoBorder))]
        internal bool WoWCustomWindowNoBorder;

        [JsonProperty(PropertyName = nameof(WoWClearCache))]
        internal bool WoWClearCache = false;

        #endregion WoW

        #region Clicker

        [JsonProperty(Order = 20, PropertyName = nameof(ClickerHotkey))]
        internal KeyExt ClickerHotkey = new KeyExt(Keys.None);

        [JsonProperty(Order = 21, PropertyName = nameof(ClickerInterval))]
        internal int ClickerInterval = 0x3e8;

        [JsonProperty(Order = 22, PropertyName = nameof(ClickerKey))]
        internal Keys ClickerKey = Keys.None;

        #endregion Clicker
        
        #region WoWPlugins

        [JsonProperty(Order = 70, PropertyName = nameof(UpdatePlugins))]
        internal bool UpdatePlugins = true;

        internal event Action PluginHotkeysChanged;

        internal void InvokePluginHotkeysChanged()
        {
            PluginHotkeysChanged?.Invoke();
        }

        [JsonProperty(Order = 73, PropertyName = nameof(PluginHotkeys))]
        internal Dictionary<string, KeyExt> PluginHotkeys = new Dictionary<string, KeyExt>();

        [JsonProperty(Order = 74, PropertyName = nameof(PluginsLastTimeUpdated))]
        internal DateTime PluginsLastTimeUpdated;

        #endregion WoWPlugins

        #region Pinger

        [JsonProperty(Order = 80, PropertyName = nameof(PingerServerID))]
        internal int PingerServerID = 1;
        
        [JsonProperty(Order = 81, PropertyName = nameof(PingerLastWoWServerIP))]
        internal string PingerLastWoWServerIP = "8.8.8.8";

        #endregion Pinger

        #region AddonsBackup

        [JsonProperty(Order = 90, PropertyName = nameof(WoWAddonsBackupPath))]
        internal string WoWAddonsBackupPath = AppFolders.UserfilesDir;

        [JsonProperty(Order = 91, PropertyName = nameof(WoWAddonsBackupLastDate))]
        internal DateTime WoWAddonsBackupLastDate = new DateTime(1970, 1, 1);

        [JsonProperty(Order = 92, PropertyName = nameof(WoWAddonsBackupIsActive))]
        internal bool WoWAddonsBackupIsActive = true;

        [JsonProperty(Order = 93, PropertyName = nameof(WoWAddonsBackupNumberOfArchives))]
        internal int WoWAddonsBackupNumberOfArchives = 7;

        [JsonProperty(Order = 94, PropertyName = nameof(WoWAddonsBackupMinimumTimeBetweenBackup))]
        internal int WoWAddonsBackupMinimumTimeBetweenBackup = 24;

        [JsonProperty(Order = 95, PropertyName = nameof(WoWAddonsBackupCompressionLevel))]
        internal int WoWAddonsBackupCompressionLevel = 6;

        [JsonProperty(Order = 96, PropertyName = nameof(WoWAddonsBackup_DoNotCreateBackupWhileWoWClientIsRunning))]
        internal bool WoWAddonsBackup_DoNotCreateBackupWhileWoWClientIsRunning = true;

        #endregion AddonsBackup
    }
    
}