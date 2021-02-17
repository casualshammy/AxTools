using AxTools.Forms;
using AxTools.Helpers;
using AxTools.Services;
using AxTools.Updater;
using AxTools.WoW;
using AxTools.WoW.PluginSystem;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools
{
    internal static class Program
    {
        internal static event Action Exit;

        internal static int UIThread { get; private set; }
        internal static readonly MultiLock ShutdownLock = new MultiLock();
        internal static Task WoWPathSearchTask { get; private set; }
        internal static Task StartWoWProcessManagerTask { get; private set; }
        private static readonly Log2 log = new Log2(nameof(Program));

        [STAThread]
        private static void Main(string[] args)
        {
            UIThread = Thread.CurrentThread.ManagedThreadId;
            if (args.Length == 0)
            {
                if (Process.GetProcessesByName(nameof(AxTools)).Length > 1)
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
                                var pricipal = new WindowsPrincipal(p);
                                if (!pricipal.IsInRole(WindowsBuiltInRole.Administrator))
                                {
                                    TaskDialog.Show("This program requires administrator privileges", nameof(AxTools), "Make sure you have administrator privileges", TaskDialogButton.OK, TaskDialogIcon.SecurityError);
                                    return;
                                }
                            }
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            WebRequest.DefaultWebProxy = null;
                            DeleteTempFolder();
                            Legacy();
                            InstallRootCertificate();
                            log.Info("Adjusting process priorities..."); Utils.SetProcessPrioritiesToNormal(Process.GetCurrentProcess().Id); // in case we'are starting from Task Scheduler priorities can be lower than normal
                            log.Info($"{typeof(WoWAntiKick)} is subscribing for {typeof(WoWProcessManager)}'s events"); WoWAntiKick.StartWaitForWoWProcesses();
                            log.Info($"Registered for: {Settings2.Instance.UserID}");
                            log.Info("Starting to load plugins..."); PluginManagerEx.LoadPluginsAsync();
                            log.Info("Starting WoW process manager..."); StartWoWProcessManagerTask = Task.Run((Action)WoWProcessManager.StartWatcher);
                            log.Info("Looking for WoW client..."); WoWPathSearchTask = Task.Run(delegate { }); // CheckWoWDirectoryIsValid
                            // log.Info("Starting add-ons backup service..."); Task.Run((Action)AddonsBackup.StartService);
                            log.Info("Starting pinger..."); Task.Run(delegate { Pinger.Enabled = Settings2.Instance.PingerServerID != 0; });
                            log.Info("Starting updater service..."); Task.Run((Action)UpdaterService.Start);
                            log.Info($"Constructing MainWindow, app version: {Globals.AppVersion}"); Application.Run(new MainWindow());
                            log.Info("MainWindow is closed, waiting for ShutdownLock..."); ShutdownLock.WaitForLocks();
                            log.Info($"Invoking 'Exit' handlers ({Exit?.GetInvocationList().Length})..."); Exit?.Invoke();
                            log.Info("Application is closed");
                            SendLogToDeveloper();
                        }
                        else
                        {
                            MessageBox.Show("This program works only on Windows 7 or higher", nameof(AxTools), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        TaskDialog.Show("This program is already running", nameof(AxTools), "", TaskDialogButton.OK, TaskDialogIcon.Warning);
                    }
                }
            }
            else
            {
                ProcessArgs();
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
#pragma warning disable CC0004
                catch { /* don't care why */ }
#pragma warning restore CC0004
            }
        }

        private static void Legacy()
        {
            var legacyLog = new Log2(nameof(Legacy));
            // 08.10.2015
            try
            {
                var mySettingsDir = AppFolders.PluginsSettingsDir + "\\Fishing";
                var mySettingsFile = mySettingsDir + "\\FishingSettings.json";
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
                foreach (string hotkeyName in new[] { "ClickerHotkey", "WoWPluginHotkey" })
                {
                    var cfg = File.ReadAllText(AppFolders.ConfigDir + "\\settings.json", Encoding.UTF8);
                    var regex = new Regex($"\"{hotkeyName}\": (\\d+)");
                    var match = regex.Match(cfg);
                    if (match.Success)
                    {
                        var oldKey = JsonConvert.DeserializeObject<Keys>(match.Groups[1].Value);
                        var alt = false;
                        var ctrl = false;
                        var shift = false;
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
                        var newCfg = cfg.Replace(match.Value, $"\"{hotkeyName}\": " + JsonConvert.SerializeObject(new KeyboardWatcher.KeyExt(oldKey, alt, shift, ctrl)));
                        File.WriteAllText(AppFolders.ConfigDir + "\\settings.json", newCfg, Encoding.UTF8);
                    }
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
                    var fileName = AppFolders.DataDir + "\\wowhead.ldb";
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                        legacyLog.Info("Old db file is deleted");
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
                var fileName = AppFolders.DataDir + "\\wowhead.ldb";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    legacyLog.Info($"Old db file is deleted ({fileName})");
                }
                fileName = AppFolders.DataDir + "\\players.ldb";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    legacyLog.Info($"Old db file is deleted ({fileName})");
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
                    var fileName = Path.Combine(AppFolders.ConfigDir, "lua-console.json");
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                        legacyLog.Info($"{fileName} is deleted");
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
                    var fileName = Path.Combine(AppFolders.ConfigDir, "wow-radar.json");
                    if (File.Exists(fileName))
                    {
                        var newFilePath = Path.Combine(AppFolders.PluginsSettingsDir, "Radar\\settings.json");
                        if (File.Exists(newFilePath))
                            File.Delete(newFilePath);
                        var rawConfig = File.ReadAllText(fileName, Encoding.UTF8);
                        var config = rawConfig.Replace(Path.Combine(AppFolders.ResourcesDir, "alarm.wav").Replace(@"\", @"\\"), Path.Combine(Settings2.Instance.PluginSourceFolder, "Radar\\alarm.wav").Replace(@"\", @"\\"));
                        File.WriteAllText(fileName, config, Encoding.UTF8);
                        File.Move(fileName, newFilePath);
                        legacyLog.Info($"{fileName} is moved to {newFilePath}");
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
            var x509 = new X509Certificate2();
            x509.Import(Resources.CACert);
            using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadWrite);
                if (x509.SerialNumber != null && store.Certificates.Find(X509FindType.FindBySerialNumber, x509.SerialNumber, true).Count == 0)
                {
                    log.Info("[AxTools] Certificate isn't found, installing...");
                    store.Add(x509);
                }
                store.Close();
            }
        }

        private static void SendLogToDeveloper()
        {
            if (Settings2.Instance.SendLogToDeveloperOnShutdown && log.HaveErrors && Utils.InternetAvailable && File.Exists(Globals.LogFileName))
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

        private static void CheckWoWDirectoryIsValid()
        {
            if (string.IsNullOrWhiteSpace(Settings2.Instance.WoWDirectory) || !File.Exists(Path.Combine(Settings2.Instance.WoWDirectory, "Wow.exe")) || !File.Exists(Path.Combine(Settings2.Instance.WoWDirectory, "WoW.mfil")))
            {
                foreach (var drive in DriveInfo.GetDrives().Where(l => l.DriveType == DriveType.Fixed))
                {
                    var path = Utils.FindFiles(drive.Name, "Wow.exe", 5).Select(Path.GetDirectoryName).Intersect(Utils.FindFiles(drive.Name, "WoW.mfil", 5).Select(Path.GetDirectoryName)).FirstOrDefault();
                    if (path != null)
                    {
                        Settings2.Instance.WoWDirectory = path;
                    }
                }
            }
        }
        
        #region Arguments

        private static void ProcessArgs()
        {
            var arguments = Environment.CommandLine;
            log.Info("CMD arguments: " + arguments);
            var updatedir = new Regex("-update-dir \"(.+?)\"").Match(arguments);
            var axtoolsdir = new Regex("-axtools-dir \"(.+?)\"").Match(arguments);
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
                while (Process.GetProcessesByName(nameof(AxTools)).Length > 1)
                {
                    log.Info("Waiting for parent AxTools process...");
                    Thread.Sleep(250);
                }
                WoW.PluginSystem.PluginManagerEx.UpdateIsActive = true;
                Main(new string[] { });
            }
            else
            {
                TaskDialog.Show("Invalid command line arguments", nameof(AxTools), "", TaskDialogButton.OK, TaskDialogIcon.Warning);
                Main(new string[] { });
            }
        }

        private static void Update(string updateDir, string axtoolsDir)
        {
            while (Process.GetProcessesByName(nameof(AxTools)).Length > 1)
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
            while (Process.GetProcessesByName(nameof(AxTools)).Length > 1)
            {
                log.Info("Waiting for parent AxTools process...");
                Thread.Sleep(250);
            }
            Process.Start(Application.StartupPath + "\\AxTools.exe");
        }

        #endregion

    }
}