using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetroFramework;

namespace AxTools.Classes
{
    internal static class Settings
    {
        internal static Point Location = new Point(100, 100);
        internal static bool AutoAcceptWndSetts = false;
        internal static bool AxToolsAddon = false;
        internal static Keys ClickerHotkey = Keys.None;
        internal static int ClickerInterval = 0x3e8;
        internal static Keys ClickerKey = Keys.None;
        internal static bool CreatureCache = false;
        internal static Keys WowLoginHotkey = Keys.None;
        internal static Point WowWindowSize = new Point(800, 600);
        internal static bool DelWowLog = false;
        internal static string MumbleExe = string.Empty;
        internal static bool Noframe = false;
        internal static string RaidcallExe = string.Empty;
        internal static string Regname = string.Empty;
        internal static SrvAddress GameServer;
        internal static string TeamspeakExe = string.Empty;
        internal static string VtExe = string.Empty;
        internal static bool Wasd = false;
        internal static string WowExe = string.Empty;
        internal static Point WowWindowLocation = Point.Empty;
        internal static bool StartVentriloWithWow = false;
        internal static bool StartTS3WithWow = false;
        internal static bool StartRaidcallWithWow = false;
        internal static bool StartMumbleWithWow = false;
        internal static Version LastUsedVersion = new Version(0, 0, 0, 0);
        internal static Keys LuaTimerHotkey = Keys.None;
        internal static Keys PrecompiledModulesHotkey = Keys.None;
        internal static Size LuaConsoleSize = new Size(650, 354);
        internal static int LuaConsoleTimerInterval = 1000;
        internal static bool LuaConsoleRandomizeTimer = false;
        internal static bool LuaConsoleIgnoreGameState = false;
        internal static bool LuaConsoleShowIngameNotifications = true;
        internal static Point RadarLocation = Point.Empty;
        internal static bool RadarShowPlayersClasses = true;
        internal static bool RadarShowNpcsNames = true;
        internal static bool RadarShowObjectsNames = true;
        internal static ulong RadarShow = 0;
        internal static Color RadarFriendColor = Color.Green;
        internal static Color RadarEnemyColor = Color.Red;
        internal static Color RadarNpcColor = Color.GreenYellow;
        internal static Color RadarObjectColor = Color.Gold;
        internal static string AddonsBackupPath = Globals.UserfilesPath;
        internal static DateTime AddonsBackupLastdate = new DateTime(1970, 1, 1);
        internal static bool AddonsBackup = true;
        internal static int AddonsBackupNum = 7;
        internal static int AddonsBackupTimer = 24;
        internal static int BackupCompressionLevel = 6;
        internal static MetroColorStyle NewStyleColor = MetroColorStyle.Blue;
        internal static bool WowPluginsShowIngameNotifications = true;
        
        internal static void Load()
        {
            try
            {
                if (File.Exists(Globals.SettingsFilePath))
                {
                    using (StringReader reader = new StringReader(File.ReadAllText(Globals.SettingsFilePath, Encoding.UTF8)))
                    {
                        while (reader.Peek() != -1)
                        {
                            string str = reader.ReadLine();
                            if (str != null && str.Contains("=") && !str.Contains("##"))
                            {
                                string[] strArray = str.Split('=');
                                try
                                {
                                    switch (strArray[0])
                                    {
                                        case "Location":
                                            Location.X = Convert.ToInt32(strArray[2].Substring(0, strArray[2].Length - 2));
                                            Location.Y = Convert.ToInt32(strArray[3].Substring(0, strArray[3].Length - 1));
                                            if (Location.X < 0 || Location.Y < 0)
                                            {
                                                Location = new Point(100, 100);
                                            }
                                            break;
                                        case "wow_dir":
                                            WowExe = strArray[1];
                                            break;
                                        case "vt_dir":
                                            VtExe = strArray[1];
                                            break;
                                        case "mumble_dir":
                                            MumbleExe = strArray[1];
                                            break;
                                        case "raidcall_dir":
                                            RaidcallExe = strArray[1];
                                            break;
                                        case "TeamspeakDir":
                                            TeamspeakExe = strArray[1];
                                            break;
                                        case "addons_backup_lastdate":
                                            AddonsBackupLastdate = Convert.ToDateTime(strArray[1]);
                                            break;
                                        case "del_wow_log":
                                            DelWowLog = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "creature_cache":
                                            CreatureCache = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "clicker_key":
                                        case "clicker.key":
                                            Enum.TryParse(strArray[1], true, out ClickerKey);
                                            break;
                                        case "clicker_hotkey":
                                        case "clicker.hotkey":
                                            Enum.TryParse(strArray[1], true, out ClickerHotkey);
                                            break;
                                        case "clicker_interval":
                                        case "clicker.interval":
                                            ClickerInterval = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "GameServer":
                                            GameServer = Globals.GameServers.FirstOrDefault(i => i.Description == strArray[1]) ?? Globals.GameServers[0];
                                            break;
                                        case "regname":
                                            Regname = strArray[1];
                                            break;
                                        case "wasd":
                                            Wasd = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "auto_accept_wnd_setts":
                                        case "wow_wnd.auto_accept":
                                            AutoAcceptWndSetts = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "noframe":
                                        case "wow_wnd.noframe":
                                            Noframe = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "x":
                                        case "wow_wnd.pos.x":
                                            WowWindowLocation.X = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "y":
                                        case "wow_wnd.pos.y":
                                            WowWindowLocation.Y = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "WowWindowLocation":
                                            WowWindowLocation.X = Convert.ToInt32(strArray[2].Substring(0, strArray[2].Length - 2));
                                            WowWindowLocation.Y = Convert.ToInt32(strArray[3].Substring(0, strArray[3].Length - 1));
                                            break;
                                        case "cx":
                                        case "wow_wnd.width":
                                            WowWindowSize.X = Convert.ToUInt16(strArray[1]);
                                            break;
                                        case "cy":
                                        case "wow_wnd.height":
                                            WowWindowSize.Y = Convert.ToUInt16(strArray[1]);
                                            break;
                                        case "WowWindowSize":
                                            WowWindowSize.X = Convert.ToInt32(strArray[2].Substring(0, strArray[2].Length - 2));
                                            WowWindowSize.Y = Convert.ToInt32(strArray[3].Substring(0, strArray[3].Length - 1));
                                            break;
                                        case "ax_tools":
                                            AxToolsAddon = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "StartVentriloWithWow":
                                            StartVentriloWithWow = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "StartTS3WithWow":
                                            StartTS3WithWow = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "StartRaidcallWithWow":
                                            StartRaidcallWithWow = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "StartMumbleWithWow":
                                            StartMumbleWithWow = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "WowLoginHotkey":
                                            Enum.TryParse(strArray[1], true, out WowLoginHotkey);
                                            break;
                                        case "LastUsedVersion":
                                            LastUsedVersion = new Version(strArray[1]);
                                            break;
                                        case "LuaTimerHotkey":
                                            Enum.TryParse(strArray[1], true, out LuaTimerHotkey);
                                            break;
                                        case "PrecompiledModulesHotkey":
                                            Enum.TryParse(strArray[1], true, out PrecompiledModulesHotkey);
                                            break;
                                        case "LuaConsole.Size.Width":
                                            LuaConsoleSize.Width = Convert.ToInt32(strArray[1]);
                                            if (LuaConsoleSize.Width < 650)
                                            {
                                                LuaConsoleSize.Width = 650;
                                            }
                                            break;
                                        case "LuaConsole.Size.Height":
                                            LuaConsoleSize.Height = Convert.ToInt32(strArray[1]);
                                            if (LuaConsoleSize.Height < 354)
                                            {
                                                LuaConsoleSize.Height = 354;
                                            }
                                            break;
                                        case "LuaConsole.TimerInterval":
                                            LuaConsoleTimerInterval = Convert.ToInt32(strArray[1]);
                                            if (LuaConsoleTimerInterval < 50)
                                            {
                                                LuaConsoleTimerInterval = 50;
                                            }
                                            break;
                                        case "LuaConsole.RandomizeTimer":
                                            LuaConsoleRandomizeTimer = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "LuaConsole.IgnoreGameState":
                                            LuaConsoleIgnoreGameState = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "LuaConsole.ShowIngameNotifications":
                                            LuaConsoleShowIngameNotifications = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "RadarLocation":
                                            RadarLocation.X = Convert.ToInt32(strArray[2].Substring(0, strArray[2].Length - 2));
                                            RadarLocation.Y = Convert.ToInt32(strArray[3].Substring(0, strArray[3].Length - 1));
                                            break;
                                        case "RadarPlayersDrawingMode":
                                            RadarShowPlayersClasses = Convert.ToInt32(strArray[1]) == 0;
                                            break;
                                        case "RadarNpcsDrawingMode":
                                            RadarShowNpcsNames = Convert.ToInt32(strArray[1]) == 0;
                                            break;
                                        case "RadarObjectsDrawingMode":
                                            RadarShowObjectsNames = Convert.ToInt32(strArray[1]) == 0;
                                            break;
                                        case "RadarShowPlayersClasses":
                                            RadarShowPlayersClasses = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "RadarShowNpcsNames":
                                            RadarShowNpcsNames = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "RadarShowObjectsNames":
                                            RadarShowObjectsNames = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "RadarShow":
                                            RadarShow = Convert.ToUInt64(strArray[1]);
                                            break;
                                        case "RadarFriendColor":
                                            RadarFriendColor = Color.FromArgb(Convert.ToInt32(strArray[1]));
                                            break;
                                        case "RadarEnemyColor":
                                            RadarEnemyColor = Color.FromArgb(Convert.ToInt32(strArray[1]));
                                            break;
                                        case "RadarNpcColor":
                                            RadarNpcColor = Color.FromArgb(Convert.ToInt32(strArray[1]));
                                            break;
                                        case "RadarObjectColor":
                                            RadarObjectColor = Color.FromArgb(Convert.ToInt32(strArray[1]));
                                            break;
                                        case "AddonsBackupPath":
                                            AddonsBackupPath = strArray[1];
                                            if (AddonsBackupPath == Application.StartupPath + "\\common")
                                            {
                                                AddonsBackupPath = Globals.UserfilesPath;
                                            }
                                            break;
                                        case "AddonsBackup":
                                            AddonsBackup = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "AddonsBackupNum":
                                            AddonsBackupNum = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "AddonsBackupTimer":
                                            AddonsBackupTimer = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "BackupCompressionLevel":
                                            BackupCompressionLevel = Convert.ToInt32(strArray[1]);
                                            if (BackupCompressionLevel < 0)
                                            {
                                                BackupCompressionLevel = 0;
                                            }
                                            if (BackupCompressionLevel > 9)
                                            {
                                                BackupCompressionLevel = 9;
                                            }
                                            break;
                                        case "NewStyleColor":
                                            int intRepresentation = Convert.ToInt32(strArray[1]);
                                            if (intRepresentation == 1 || intRepresentation > 13 || intRepresentation < 0)
                                            {
                                                intRepresentation = 3;
                                            }
                                            NewStyleColor = (MetroColorStyle) intRepresentation;
                                            break;
                                        case "WowPluginsShowIngameNotifications":
                                            WowPluginsShowIngameNotifications = Convert.ToBoolean(strArray[1]);
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Print("Settings reading error (" + strArray.AsString() + "): " + ex.Message, true);
                                }
                            }
                        }
                    }
                    Log.Print("Settings are loaded");
                }
                else
                {
                    Log.Print("Settings file isn't found");
                }
            }
            catch (Exception exception1)
            {
                Log.Print("Reading settings failed: " + exception1.Message, true);
            }
        }
        internal static void Save()
        {
            try
            {
                StringBuilder builder = new StringBuilder("## AxTools settings file\r\n");
                builder.AppendLine(string.Empty);
                builder.AppendLine("## General settings ##");
                builder.AppendLine("Location=" + Location);
                builder.AppendLine("GameServer=" + GameServer.Description);
                builder.AppendLine("regname=" + Regname);
                builder.AppendLine("LastUsedVersion=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                builder.AppendLine("NewStyleColor=" + Convert.ToString((int)NewStyleColor));
                builder.AppendLine(string.Empty);

                builder.AppendLine("WowPluginsShowIngameNotifications=" + Convert.ToString(WowPluginsShowIngameNotifications));
                builder.AppendLine("del_wow_log=" + Convert.ToString(DelWowLog));
                builder.AppendLine("creature_cache=" + Convert.ToString(CreatureCache));
                builder.AppendLine("clicker.key=" + Convert.ToString(ClickerKey));
                builder.AppendLine("clicker.hotkey=" + Convert.ToString(ClickerHotkey));
                builder.AppendLine("clicker.interval=" + Convert.ToString(ClickerInterval));
                builder.AppendLine("wasd=" + Convert.ToString(Wasd));
                builder.AppendLine("wow_wnd.auto_accept=" + Convert.ToString(AutoAcceptWndSetts));
                builder.AppendLine("wow_wnd.noframe=" + Convert.ToString(Noframe));
                builder.AppendLine("WowWindowLocation=" + WowWindowLocation);
                builder.AppendLine("WowWindowSize=" + WowWindowSize);
                builder.AppendLine("ax_tools=" + Convert.ToString(AxToolsAddon));
                builder.AppendLine("WowLoginHotkey=" + Convert.ToString(WowLoginHotkey));
                builder.AppendLine("LuaTimerHotkey=" + Convert.ToString(LuaTimerHotkey));
                builder.AppendLine("PrecompiledModulesHotkey=" + Convert.ToString(PrecompiledModulesHotkey));

                builder.AppendLine(string.Empty);
                builder.AppendLine("## Paths ##");
                if (Directory.Exists(WowExe)) builder.AppendLine("wow_dir=" + WowExe);
                if (Directory.Exists(VtExe)) builder.AppendLine("vt_dir=" + VtExe);
                if (Directory.Exists(MumbleExe)) builder.AppendLine("mumble_dir=" + MumbleExe);
                if (Directory.Exists(RaidcallExe)) builder.AppendLine("raidcall_dir=" + RaidcallExe);
                if (Directory.Exists(TeamspeakExe)) builder.AppendLine("TeamspeakDir=" + TeamspeakExe);
                builder.AppendLine(string.Empty);
                builder.AppendLine("## VoIP settings ##");
                builder.AppendLine("StartVentriloWithWow=" + Convert.ToString(StartVentriloWithWow));
                builder.AppendLine("StartTS3WithWow=" + Convert.ToString(StartTS3WithWow));
                builder.AppendLine("StartRaidcallWithWow=" + Convert.ToString(StartRaidcallWithWow));
                builder.AppendLine("StartMumbleWithWow=" + Convert.ToString(StartMumbleWithWow));
                builder.AppendLine(string.Empty);
                builder.AppendLine("## Lua console settings ##");
                builder.AppendLine("LuaConsole.Size.Width=" + LuaConsoleSize.Width);
                builder.AppendLine("LuaConsole.Size.Height=" + LuaConsoleSize.Height);
                builder.AppendLine("LuaConsole.TimerInterval=" + LuaConsoleTimerInterval);
                builder.AppendLine("LuaConsole.RandomizeTimer=" + LuaConsoleRandomizeTimer);
                builder.AppendLine("LuaConsole.IgnoreGameState=" + LuaConsoleIgnoreGameState);
                builder.AppendLine("LuaConsole.ShowIngameNotifications=" + LuaConsoleShowIngameNotifications);
                builder.AppendLine(string.Empty);
                builder.AppendLine("## Radar settings ##");
                builder.AppendLine("RadarLocation=" + RadarLocation);
                builder.AppendLine("RadarShowPlayersClasses=" + Convert.ToString(RadarShowPlayersClasses));
                builder.AppendLine("RadarShowNpcsNames=" + Convert.ToString(RadarShowNpcsNames));
                builder.AppendLine("RadarShowObjectsNames=" + Convert.ToString(RadarShowObjectsNames));
                builder.AppendLine("RadarShow=" + Convert.ToString(RadarShow));
                builder.AppendLine("RadarFriendColor=" + RadarFriendColor.ToArgb());
                builder.AppendLine("RadarEnemyColor=" + RadarEnemyColor.ToArgb());
                builder.AppendLine("RadarNpcColor=" + RadarNpcColor.ToArgb());
                builder.AppendLine("RadarObjectColor=" + RadarObjectColor.ToArgb());
                builder.AppendLine(string.Empty);
                builder.AppendLine("## Backup settings ##");
                builder.AppendLine("AddonsBackupPath=" + AddonsBackupPath);
                builder.AppendLine("AddonsBackup=" + Convert.ToString(AddonsBackup));
                builder.AppendLine("AddonsBackupNum=" + Convert.ToString(AddonsBackupNum));
                builder.AppendLine("AddonsBackupTimer=" + Convert.ToString(AddonsBackupTimer));
                builder.AppendLine("BackupCompressionLevel=" + Convert.ToString(BackupCompressionLevel));
                builder.Append("addons_backup_lastdate=" + Convert.ToString(AddonsBackupLastdate));
                File.WriteAllText(Globals.SettingsFilePath, builder.ToString(), Encoding.UTF8);
            }
            catch (Exception exception1)
            {
                Log.Print("Error: " + exception1.Message, true);
            }
        }

    }
}
