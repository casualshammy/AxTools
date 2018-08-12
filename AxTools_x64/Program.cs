using AxTools.Forms;
using AxTools.Helpers;

using AxTools.Updater;
using AxTools.WoW;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

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
                if (Process.GetProcessesByName("AxTools").Length > 1)
                {
                    log.Info("Waiting for parent AxTools process (1000 ms)");
                    Thread.Sleep(1000);
                }
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

            // 10.08.2018
            try
            {
                if (Settings2.Instance.LastUsedVersion <= new VersionExt(12, 3, 46))
                {
                    string fileName = Path.Combine(AppFolders.ConfigDir, "lua-console.json");
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                        log.Info($"{fileName} is deleted");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // 10.08.2018
            try
            {
                if (Settings2.Instance.LastUsedVersion <= new VersionExt(12, 3, 46))
                {
                    string fileName = Path.Combine(AppFolders.ConfigDir, "wow-radar.json");
                    if (File.Exists(fileName))
                    {
                        string newFilePath = Path.Combine(AppFolders.PluginsSettingsDir, "Radar\\settings.json");
                        if (File.Exists(newFilePath))
                            File.Delete(newFilePath);
                        string rawConfig = File.ReadAllText(fileName, Encoding.UTF8);
                        string config = rawConfig.Replace(Path.Combine(AppFolders.ResourcesDir, "alarm.wav").Replace(@"\", @"\\"), Path.Combine(AppFolders.PluginsDir, "Radar\\alarm.wav").Replace(@"\", @"\\"));
                        File.WriteAllText(fileName, config, Encoding.UTF8);
                        File.Move(fileName, newFilePath);
                        log.Info($"{fileName} is moved to {newFilePath}");
                    }
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
            x509.Import(Helpers.Resources.CACert);
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            if (x509.SerialNumber != null && store.Certificates.Find(X509FindType.FindBySerialNumber, x509.SerialNumber, true).Count == 0)
            {
                log.Info("[AxTools] Certificate isn't found, installing...");
                store.Add(x509);
            }
            store.Close();
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

        #region Arguments

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
            else if (arguments.Contains("-restart"))
            {
                RestartApplication();
            }
            else if (arguments.Contains("-update-plugins"))
            {
                while (Process.GetProcessesByName("AxTools").Length > 1)
                {
                    log.Info("Waiting for parent AxTools process...");
                    Thread.Sleep(250);
                }
                WoW.PluginSystem.PluginManagerEx.UpdateIsActive = true;
                Main(new string[] { });
            }
            else
            {
                TaskDialog.Show("Invalid command line arguments", "AxTools", "", TaskDialogButton.OK, TaskDialogIcon.Warning);
                Main(new string[] { });
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
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(axtoolsDir, "AxTools.exe"),
                WorkingDirectory = axtoolsDir,
                Arguments = "-update-plugins",
            });
        }

        private static void RestartApplication()
        {
            while (Process.GetProcessesByName("AxTools").Length > 1)
            {
                log.Info("Waiting for parent AxTools process...");
                Thread.Sleep(250);
            }
            Process.Start(Application.StartupPath + "\\AxTools.exe");
        }
        
        #endregion

    }
}