﻿using AxTools.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.Forms.Helpers;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.Updater;
using Newtonsoft.Json;
using System.Linq;
using AxTools.WoW;
using AxTools.WoW.PluginSystem;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

namespace AxTools
{
    internal static class Program
    {
        internal static event Action Exit;
        internal static int UIThread;
        internal static MultiLock ShutdownLock = new MultiLock();
        private static readonly Log2 log = new Log2("Program");

        [STAThread]
        private static void Main(string[] args)
        {
            UIThread = Thread.CurrentThread.ManagedThreadId;
            if (args.Length == 0)
            {
                using (new Mutex(true, "AxToolsMainExecutable", out bool newInstance))
                {
                    if (newInstance)
                    {
                        if (Environment.OSVersion.Version >= new Version(6, 1))
                        {
                            using (WindowsIdentity p = WindowsIdentity.GetCurrent())
                            {
                                WindowsPrincipal pricipal = new WindowsPrincipal(p);
                                if (!pricipal.IsInRole(WindowsBuiltInRole.Administrator))
                                {
                                    TaskDialog.Show("This program requires administrator privileges", "AxTools", "Make sure you have administrator privileges", TaskDialogButton.OK, TaskDialogIcon.SecurityError);
                                    return;
                                }
                            }
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            WebRequest.DefaultWebProxy = null;
                            DeleteTempFolder();
                            Legacy();
                            InstallRootCertificate();
                            log.Info($"{typeof(WoWAntiKick).ToString()} is subscribing for {typeof(WoWProcessManager).ToString()}'s events");
                            WoWAntiKick.StartWaitForWoWProcesses();
                            log.Info(string.Format("Registered for: {0}", Settings2.Instance.UserID));
                            log.Info($"Constructing MainWindow, app version: {Globals.AppVersion}");
                            Application.Run(MainForm.Instance = new MainForm());
                            log.Info("MainWindow is closed, waiting for ShutdownLock...");
                            ShutdownLock.WaitForLocks();
                            log.Info($"Invoking 'Exit' handlers ({Exit?.GetInvocationList().Length})...");
                            Exit?.Invoke();
                            log.Info("Application is closed");
                            SendLogToDeveloper();
                        }
                        else
                        {
                            MessageBox.Show("This program works only on Windows 7 or higher", "AxTools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        TaskDialog.Show("This program is already running", "AxTools", "", TaskDialogButton.OK, TaskDialogIcon.Warning);
                    }
                }
            }
            else
            {
                ProcessArgs(args);
            }
        }

        private static void DeleteTempFolder()
        {
            if (Directory.Exists(AppFolders.TempDir))
            {
                try
                {
                    Directory.Delete(AppFolders.TempDir, true);
                }
                catch
                {
                    //
                }
            }
        }

        private static void Legacy()
        {
            Log2 log = new Log2("Legacy");
            // 08.10.2015
            try
            {
                string mySettingsDir = AppFolders.PluginsSettingsDir + "\\Fishing";
                string mySettingsFile = mySettingsDir + "\\FishingSettings.json";
                if (File.Exists(mySettingsFile))
                {
                    File.Move(mySettingsFile, mySettingsDir + "\\settings.json");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // 01.06.2016
            try
            {
                //"WoWRadarShowMode": 2199040098561,
                Regex regex = new Regex("\"WoWRadarShowMode\": (\\d+)");
                string cfg = File.ReadAllText(AppFolders.ConfigDir + "\\settings.json", Encoding.UTF8);
                Match match = regex.Match(cfg);
                if (match.Success)
                {
                    ulong radarShowMode = ulong.Parse(match.Groups[1].Value);
                    byte[] p = BitConverter.GetBytes(radarShowMode);
                    RadarShowMode newRadarShowMode = new RadarShowMode
                    {
                        Friends = p[0] == 1,
                        Enemies = p[1] == 1,
                        Npcs = p[2] == 1,
                        Objects = p[3] == 1,
                        Corpses = p[4] == 1,
                        Zoom = p[5]*0.25F
                    };
                    if (newRadarShowMode.Zoom > 2F || newRadarShowMode.Zoom < 0.25F)
                    {
                        newRadarShowMode.Zoom = 0.5F;
                    }
                    string newCfg = cfg.Replace(match.Value, "\"WoWRadarShowMode\": " + JsonConvert.SerializeObject(newRadarShowMode));
                    File.WriteAllText(AppFolders.ConfigDir + "\\settings.json", newCfg, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // 11.07.2016
            try
            {
                if (Directory.Exists(Application.StartupPath + "\\wowheadCache"))
                {
                    Directory.Delete(Application.StartupPath + "\\wowheadCache", true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // 12.01.2018
            try
            {
                foreach (string hotkeyName in new string[] { "ClickerHotkey", "WoWPluginHotkey" })
                {
                    string cfg = File.ReadAllText(AppFolders.ConfigDir + "\\settings.json", Encoding.UTF8);
                    Regex regex = new Regex($"\"{hotkeyName}\": (\\d+)");
                    Match match = regex.Match(cfg);
                    if (match.Success)
                    {
                        Keys oldKey = JsonConvert.DeserializeObject<Keys>(match.Groups[1].Value);
                        bool alt = false;
                        bool ctrl = false;
                        bool shift = false;
                        if ((oldKey & Keys.Alt) == Keys.Alt)
                        {
                            alt = true;
                        }
                        if ((oldKey & Keys.Control) == Keys.Control)
                        {
                            ctrl = true;
                        }
                        if ((oldKey & Keys.Shift) == Keys.Shift)
                        {
                            shift = true;
                        }
                        oldKey = oldKey & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
                        string newCfg = cfg.Replace(match.Value, $"\"{hotkeyName}\": " + JsonConvert.SerializeObject(new KeyboardWatcher.KeyExt(oldKey, alt, shift, ctrl)));
                        File.WriteAllText(AppFolders.ConfigDir + "\\settings.json", newCfg, Encoding.UTF8);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // 14.03.2018
            try
            {
                string cfg = File.ReadAllText(AppFolders.ConfigDir + "\\settings.json", Encoding.UTF8);
                Regex regex = new Regex("\"WoWAccounts\": (\".*\")");
                Match match = regex.Match(cfg);
                if (match.Success && match.Groups[1].Value.Length > 0)
                {
                    log.Info("Found old WoWAccounts db, migrating...");
                    byte[] oldEncryptedPasswordData = JsonConvert.DeserializeObject<byte[]>(match.Groups[1].Value);
                    #pragma warning disable CS0618 // Type or member is obsolete
                    var oldAccounts = WoWAccount.Load(oldEncryptedPasswordData);
                    #pragma warning restore CS0618 // Type or member is obsolete
                    var newAccounts = new ObservableCollection<WoWAccount2>();
                    foreach (var entry in oldAccounts)
                    {
                        var encryptedLogin = ProtectedData.Protect(Encoding.UTF8.GetBytes(entry.Login), null, DataProtectionScope.CurrentUser);
                        var encryptedPassword = ProtectedData.Protect(Encoding.UTF8.GetBytes(entry.Password), null, DataProtectionScope.CurrentUser);
                        newAccounts.Add(new WoWAccount2() { EncryptedLogin = encryptedLogin, EncryptedPassword = encryptedPassword });
                    }
                    log.Info("Total accounts: " + newAccounts.Count);
                    string newCfg = cfg.Replace(match.Value, $"\"WoWAccounts2\": {JsonConvert.SerializeObject(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(newAccounts)))}");
                    File.WriteAllText(AppFolders.ConfigDir + "\\settings.json", newCfg, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // 17.04.2017
            try
            {
                if (Settings2.Instance.LastUsedVersion <= new VersionExt(12, 2, 46))
                {
                    string fileName = AppFolders.DataDir + "\\wowhead.ldb";
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                        log.Info("Old db file is deleted");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // 17.01.2018
            try
            {
                string fileName = AppFolders.DataDir + "\\wowhead.ldb";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    log.Info($"Old db file is deleted ({fileName})");
                }
                fileName = AppFolders.DataDir + "\\players.ldb";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    log.Info($"Old db file is deleted ({fileName})");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // 09.02.2018
            try
            {
                string cfg = File.ReadAllText(AppFolders.ConfigDir + "\\settings.json", Encoding.UTF8);
                if (cfg.Contains("\"WoWRadarList\": ["))
                {
                    Helpers.Settings oldSettings = JsonConvert.DeserializeObject<Helpers.Settings>(cfg);
                    WoWRadarSettings.Instance.AlarmSoundFile = oldSettings.WoWRadarAlarmSoundFile;
                    WoWRadarSettings.Instance.DisplayCorpses = oldSettings.WoWRadarShowMode.Corpses;
                    WoWRadarSettings.Instance.DisplayEnemies = oldSettings.WoWRadarShowMode.Enemies;
                    WoWRadarSettings.Instance.DisplayFriends = oldSettings.WoWRadarShowMode.Friends;
                    WoWRadarSettings.Instance.DisplayNpcs = oldSettings.WoWRadarShowMode.Npcs;
                    WoWRadarSettings.Instance.DisplayObjects = oldSettings.WoWRadarShowMode.Objects;
                    WoWRadarSettings.Instance.EnemyColor = oldSettings.WoWRadarEnemyColor;
                    WoWRadarSettings.Instance.FriendColor = oldSettings.WoWRadarFriendColor;
                    WoWRadarSettings.Instance.List = oldSettings.WoWRadarList;
                    WoWRadarSettings.Instance.Location = oldSettings.WoWRadarLocation;
                    WoWRadarSettings.Instance.NPCColor = oldSettings.WoWRadarNPCColor;
                    WoWRadarSettings.Instance.ObjectColor = oldSettings.WoWRadarObjectColor;
                    WoWRadarSettings.Instance.ShowLocalPlayerRotationArrowOnTop = oldSettings.WoWRadarShowLocalPlayerRotationArrowOnTop;
                    WoWRadarSettings.Instance.ShowNPCsNames = oldSettings.WoWRadarShowNPCsNames;
                    WoWRadarSettings.Instance.ShowObjectsNames = oldSettings.WoWRadarShowObjectsNames;
                    WoWRadarSettings.Instance.ShowPlayersClasses = oldSettings.WoWRadarShowPlayersClasses;
                    WoWRadarSettings.Instance.Zoom = oldSettings.WoWRadarShowMode.Zoom;
                }
                if (cfg.Contains("\"PluginsUsageStat2\": {"))
                {
                    Helpers.Settings oldSettings = JsonConvert.DeserializeObject<Helpers.Settings>(cfg);
                    PluginManagerEx._pluginsUsageStats = oldSettings.PluginsUsageStat2;
                    PluginManagerEx.SavePluginUsageStats();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void InstallRootCertificate()
        {
            X509Certificate2 x509 = new X509Certificate2();
            x509.Import(Resources.Axio_Lab_CA);
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            if (x509.SerialNumber != null && store.Certificates.Find(X509FindType.FindBySerialNumber, x509.SerialNumber, true).Count == 0)
            {
                log.Info("[AxTools] Certificate isn't found, installing...");
                store.Add(x509);
            }
            store.Close();
        }

        private static void ProcessArgs(string[] args)
        {
            string arguments = Environment.CommandLine;
            log.Info("CMD arguments: " + arguments);
            Match updatedir = new Regex("-update-dir \"(.+?)\"").Match(arguments);
            Match axtoolsdir = new Regex("-axtools-dir \"(.+?)\"").Match(arguments);
            if (updatedir.Success && axtoolsdir.Success)
            {
                log.Info("Parsed update info, processing...");
                Update(updatedir.Groups[1].Value, axtoolsdir.Groups[1].Value);
            }
            else
            {
                TaskDialog.Show("Invalid command line arguments", "AxTools", "", TaskDialogButton.OK, TaskDialogIcon.Warning);
                Main(new string[] {});
            }
        }

        private static void Update(string updateDir, string axtoolsDir)
        {
            while (Process.GetProcessesByName("AxTools").Length > 1)
            {
                log.Info("Waiting for parent AxTools process...");
                Thread.Sleep(500);
            }
            UpdaterService.ApplyUpdate(updateDir, axtoolsDir);
        }

        private static void SendLogToDeveloper()
        {
            TaskDialogButton yesNo = TaskDialogButton.Yes + (int)TaskDialogButton.No;
            TaskDialog taskDialog = new TaskDialog("There were errors during runtime", "AxTools", "Do you want to send log file to developer?", yesNo, TaskDialogIcon.Warning);
            if (log.HaveErrors && Utils.InternetAvailable && taskDialog.Show().CommonButton == Result.Yes && File.Exists(Globals.LogFileName))
            {
                try
                {
                    Log2.UploadLog(null);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Log file sending error", ex.Message);
                }
            }
        }

    }
}
