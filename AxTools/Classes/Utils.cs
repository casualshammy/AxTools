using System.Collections.Generic;
using System.Drawing;
using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WinAPI;
using System;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.WoW;
using MetroFramework;
using Newtonsoft.Json;

namespace AxTools.Classes
{
    internal static class Utils
    {
        internal static readonly Random Rnd = new Random();

        internal static T FindForm<T>() where T : Form
        {
            foreach (var i in Application.OpenForms)
            {
                if (i.GetType() == typeof (T)) return i as T;
            }
            return null;
        }

        internal static long CalcDirectorySize(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            long num2 = 0L;
            foreach (FileSystemInfo info2 in info.GetFileSystemInfos())
            {
                if (info2 is FileInfo)
                {
                    num2 += (info2 as FileInfo).Length;
                }
                else if (info2 is DirectoryInfo)
                {
                    num2 += CalcDirectorySize((info2 as DirectoryInfo).FullName);
                }
            }
            return num2;
        }

        internal static void CheckCreateDir()
        {
            if (!Directory.Exists(Globals.TempPath))
            {
                Directory.CreateDirectory(Globals.TempPath);
            }
            if (!Directory.Exists(Globals.UserfilesPath))
            {
                Directory.CreateDirectory(Globals.UserfilesPath);
            }
            if (!Directory.Exists(Globals.CfgPath))
            {
                Directory.CreateDirectory(Globals.CfgPath);
            }
            if (!Directory.Exists(Globals.PluginsPath))
            {
                Directory.CreateDirectory(Globals.PluginsPath);
            }
            if (!Directory.Exists(Globals.PluginsAssembliesPath))
            {
                Directory.CreateDirectory(Globals.PluginsAssembliesPath);
            }
            if (!Directory.Exists(Globals.PluginsSettingsPath))
            {
                Directory.CreateDirectory(Globals.PluginsSettingsPath);
            }
        }

        internal static void Legacy()
        {
            try
            {
                Settings settings = Settings.Instance;
                if (File.Exists(Globals.CfgPath + "//.luaconsole3"))
                {
                    settings.WoWLuaConsoleLastText = File.ReadAllText(Globals.CfgPath + "//.luaconsole3", Encoding.UTF8);
                    File.Delete(Globals.CfgPath + "//.luaconsole3");
                    Settings.Instance.SaveJSON();
                }
                if (File.Exists(Globals.CfgPath + "//.radar3"))
                {
                    settings.WoWRadarList = JsonConvert.DeserializeObject<List<ObjectToFind>>(File.ReadAllText(Globals.CfgPath + "//.radar3", Encoding.UTF8));
                    File.Delete(Globals.CfgPath + "//.radar3");
                    Settings.Instance.SaveJSON();
                }
                if (File.Exists(Globals.CfgPath + "//.wowaccounts2"))
                {
                    settings.WoWAccounts = File.ReadAllBytes(Globals.CfgPath + "//.wowaccounts2");
                    File.Delete(Globals.CfgPath + "//.wowaccounts2");
                    Settings.Instance.SaveJSON();
                }
                if (File.Exists(Globals.CfgPath + "//.settings"))
                {
                    using (StringReader reader = new StringReader(File.ReadAllText(Globals.CfgPath + "//.settings", Encoding.UTF8)))
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

                                        case "MinimizeToTray":
                                            settings.MinimizeToTray = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "wow_dir":
                                            settings.WoWDirectory = strArray[1];
                                            break;
                                        case "vt_dir":
                                            settings.VentriloDirectory = strArray[1];
                                            break;
                                        case "mumble_dir":
                                            settings.MumbleDirectory = strArray[1];
                                            break;
                                        case "raidcall_dir":
                                            settings.RaidcallDirectory = strArray[1];
                                            break;
                                        case "TeamspeakDir":
                                            settings.TS3Directory = strArray[1];
                                            break;
                                        case "addons_backup_lastdate":
                                            settings.WoWAddonsBackupLastDate = Convert.ToDateTime(strArray[1]);
                                            break;
                                        case "creature_cache":
                                            settings.WoWWipeCreatureCache = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "clicker_key":
                                        case "clicker.key":
                                            Enum.TryParse(strArray[1], true, out settings.ClickerKey);
                                            break;
                                        case "clicker_hotkey":
                                        case "clicker.hotkey":
                                            Keys key0;
                                            if (Enum.TryParse(strArray[1], true, out key0))
                                            {
                                                settings.ClickerHotkey = key0;
                                            }
                                            break;
                                        case "clicker_interval":
                                        case "clicker.interval":
                                            settings.ClickerInterval = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "GameServer":
                                        case "Pinger.GameServer":
                                            int oldIndex = new List<SrvAddress>(Globals.GameServers).FindIndex(i => i.Description == strArray[1]);
                                            settings.PingerServerID = oldIndex == -1 ? 1 : oldIndex;
                                            break;
                                        case "Pinger.BadNetworkPing":
                                            settings.PingerBadPing = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "Pinger.BadNetworkProcent":
                                            settings.PingerBadPacketLoss = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "Pinger.VeryBadNetworkPing":
                                            settings.PingerVeryBadPing = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "Pinger.VeryBadNetworkProcent":
                                            settings.PingerVeryBadPacketLoss = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "regname":
                                            settings.UserID = strArray[1];
                                            break;
                                        case "wasd":
                                            settings.WoWAntiKick = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "auto_accept_wnd_setts":
                                        case "wow_wnd.auto_accept":
                                            settings.WoWCustomizeWindow = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "noframe":
                                        case "wow_wnd.noframe":
                                            settings.WoWCustomWindowNoBorder = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "x":
                                        case "wow_wnd.pos.x":
                                            settings.WoWCustomWindowRectangle.X = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "y":
                                        case "wow_wnd.pos.y":
                                            settings.WoWCustomWindowRectangle.Y = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "WowWindowLocation":
                                            settings.WoWCustomWindowRectangle.X = Convert.ToInt32(strArray[2].Substring(0, strArray[2].Length - 2));
                                            settings.WoWCustomWindowRectangle.Y = Convert.ToInt32(strArray[3].Substring(0, strArray[3].Length - 1));
                                            break;
                                        case "cx":
                                        case "wow_wnd.width":
                                            settings.WoWCustomWindowRectangle.Width = Convert.ToUInt16(strArray[1]);
                                            break;
                                        case "cy":
                                        case "wow_wnd.height":
                                            settings.WoWCustomWindowRectangle.Height = Convert.ToUInt16(strArray[1]);
                                            break;
                                        case "WowWindowSize":
                                            settings.WoWCustomWindowRectangle.Width = Convert.ToInt32(strArray[2].Substring(0, strArray[2].Length - 2));
                                            settings.WoWCustomWindowRectangle.Height = Convert.ToInt32(strArray[3].Substring(0, strArray[3].Length - 1));
                                            break;
                                        case "StartVentriloWithWow":
                                            settings.VentriloStartWithWoW = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "StartTS3WithWow":
                                            settings.TS3StartWithWoW = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "StartRaidcallWithWow":
                                            settings.RaidcallStartWithWoW = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "StartMumbleWithWow":
                                            settings.MumbleStartWithWoW = Convert.ToBoolean(strArray[1]);
                                            break;

                                        case "LuaTimerHotkey":
                                            Keys key;
                                            if (Enum.TryParse(strArray[1], true, out key))
                                            {
                                                settings.LuaTimerHotkey = key;
                                            }
                                            break;
                                        case "PrecompiledModulesHotkey":
                                            Keys key1;
                                            if (Enum.TryParse(strArray[1], true, out key1))
                                            {
                                                settings.WoWPluginHotkey = key1;
                                            }
                                            break;
                                        case "LuaConsole.Size.Width":
                                            settings.WoWLuaConsoleWindowSize.Width = Convert.ToInt32(strArray[1]);
                                            if (settings.WoWLuaConsoleWindowSize.Width < 650)
                                            {
                                                settings.WoWLuaConsoleWindowSize.Width = 650;
                                            }
                                            break;
                                        case "LuaConsole.Size.Height":
                                            settings.WoWLuaConsoleWindowSize.Height = Convert.ToInt32(strArray[1]);
                                            if (settings.WoWLuaConsoleWindowSize.Height < 354)
                                            {
                                                settings.WoWLuaConsoleWindowSize.Height = 354;
                                            }
                                            break;
                                        case "LuaConsole.TimerInterval":
                                            settings.WoWLuaConsoleTimerInterval = Convert.ToInt32(strArray[1]);
                                            if (settings.WoWLuaConsoleTimerInterval < 50)
                                            {
                                                settings.WoWLuaConsoleTimerInterval = 50;
                                            }
                                            break;
                                        case "LuaConsole.RandomizeTimer":
                                            settings.WoWLuaConsoleTimerRnd = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "LuaConsole.IgnoreGameState":
                                            settings.WoWLuaConsoleIgnoreGameState = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "LuaConsole.ShowIngameNotifications":
                                            settings.WoWLuaConsoleShowIngameNotifications = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "RadarLocation":
                                            settings.WoWRadarLocation.X = Convert.ToInt32(strArray[2].Substring(0, strArray[2].Length - 2));
                                            settings.WoWRadarLocation.Y = Convert.ToInt32(strArray[3].Substring(0, strArray[3].Length - 1));
                                            break;
                                        case "RadarPlayersDrawingMode":
                                            settings.WoWRadarShowPlayersClasses = Convert.ToInt32(strArray[1]) == 0;
                                            break;
                                        case "RadarNpcsDrawingMode":
                                            settings.WoWRadarShowNPCsNames = Convert.ToInt32(strArray[1]) == 0;
                                            break;
                                        case "RadarObjectsDrawingMode":
                                            settings.WoWRadarShowObjectsNames = Convert.ToInt32(strArray[1]) == 0;
                                            break;
                                        case "RadarShowPlayersClasses":
                                            settings.WoWRadarShowPlayersClasses = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "RadarShowNpcsNames":
                                            settings.WoWRadarShowNPCsNames = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "RadarShowObjectsNames":
                                            settings.WoWRadarShowObjectsNames = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "RadarShow":
                                            settings.WoWRadarShowMode = Convert.ToUInt64(strArray[1]);
                                            break;
                                        case "RadarFriendColor":
                                            settings.WoWRadarFriendColor = Color.FromArgb(Convert.ToInt32(strArray[1]));
                                            break;
                                        case "RadarEnemyColor":
                                            settings.WoWRadarEnemyColor = Color.FromArgb(Convert.ToInt32(strArray[1]));
                                            break;
                                        case "RadarNpcColor":
                                            settings.WoWRadarNPCColor = Color.FromArgb(Convert.ToInt32(strArray[1]));
                                            break;
                                        case "RadarObjectColor":
                                            settings.WoWRadarObjectColor = Color.FromArgb(Convert.ToInt32(strArray[1]));
                                            break;
                                        case "AddonsBackupPath":
                                            settings.WoWAddonsBackupPath = strArray[1];
                                            if (settings.WoWAddonsBackupPath == Application.StartupPath + "\\common")
                                            {
                                                settings.WoWAddonsBackupPath = Globals.UserfilesPath;
                                            }
                                            break;
                                        case "AddonsBackup":
                                            settings.WoWAddonsBackupIsActive = Convert.ToBoolean(strArray[1]);
                                            break;
                                        case "AddonsBackupNum":
                                            settings.WoWAddonsBackupNumberOfArchives = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "AddonsBackupTimer":
                                            settings.WoWAddonsBackupMinimumTimeBetweenBackup = Convert.ToInt32(strArray[1]);
                                            break;
                                        case "BackupCompressionLevel":
                                            settings.WoWAddonsBackupCompressionLevel = Convert.ToInt32(strArray[1]);
                                            if (settings.WoWAddonsBackupCompressionLevel < 0)
                                            {
                                                settings.WoWAddonsBackupCompressionLevel = 0;
                                            }
                                            if (settings.WoWAddonsBackupCompressionLevel > 9)
                                            {
                                                settings.WoWAddonsBackupCompressionLevel = 9;
                                            }
                                            break;
                                        case "NewStyleColor":
                                            int intRepresentation = Convert.ToInt32(strArray[1]);
                                            if (intRepresentation == 1 || intRepresentation > 13 || intRepresentation < 0)
                                            {
                                                intRepresentation = 3;
                                            }
                                            settings.StyleColor = (MetroColorStyle)intRepresentation;
                                            break;
                                        case "WowPluginsShowIngameNotifications":
                                            settings.WoWPluginShowIngameNotifications = Convert.ToBoolean(strArray[1]);
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
                    File.Delete(Globals.CfgPath + "//.settings");
                    Settings.Instance.SaveJSON();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        internal static string GetRandomString(int size)
        {
            StringBuilder builder = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Rnd.NextDouble() + 65)));
                if (Rnd.Next(10) % 2 == 0)
                {
                    ch = ch.ToString().ToLower()[0];
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }
        
        internal static bool InternetAvailable
        {
            get
            {
                try
                {
                    using (Ping ping = new Ping())
                    {
                        PingReply pingReply = ping.Send("google.com", 2000);
                        return pingReply != null && (pingReply.Status == IPStatus.Success);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        internal static bool FontIsInstalled(string fontName)
        {
            using (var fontsCollection = new InstalledFontCollection())
            {
                return fontsCollection.Families.Any(i => i.Name == fontName);
            }
        }

        internal static void PlaySystemNotificationAsync()
        {
            Task.Factory.StartNew(() => NativeMethods.sndPlaySoundW("SystemNotification", 65536 | 2));  //SND_ALIAS = 65536; SND_NODEFAULT = 2;);
        }

        internal static void NotifyUser(string title, string message, NotifyUserType type, bool sound)
        {
            if (NativeMethods.GetForegroundWindow() == MainForm.Instance.Handle)
            {
                switch (type)
                {
                    case NotifyUserType.Error:
                        MainForm.Instance.ShowTaskDialog(title, message, TaskDialogButton.OK, TaskDialogIcon.Stop);
                        break;
                    case NotifyUserType.Warn:
                        MainForm.Instance.ShowTaskDialog(title, message, TaskDialogButton.OK, TaskDialogIcon.Warning);
                        break;
                    default:
                        MainForm.Instance.ShowTaskDialog(title, message, TaskDialogButton.OK, TaskDialogIcon.Information);
                        break;
                }
            }
            else
            {
                MainForm.Instance.ShowNotifyIconMessage(title, message, (ToolTipIcon) type);
                if (sound)
                {
                    if (type == NotifyUserType.Error || type == NotifyUserType.Warn)
                    {
                        SystemSounds.Hand.Play();
                    }
                    else
                    {
                        PlaySystemNotificationAsync();
                    }
                }
            }
        }

    }
}