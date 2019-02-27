using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WoW;
using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Updater
{
    [Obfuscation(Exclude = false, Feature = "constants")]
    internal static class UpdaterService
    {
        private static readonly Log2 log = new Log2(nameof(UpdaterService));
        private static readonly System.Timers.Timer _timer = new System.Timers.Timer(600000);
        private static readonly string DistrDirectory = AppFolders.TempDir + "\\update";
        private static readonly string[] JunkFiles = { };
        private static readonly string[] JunkFolders = { };
        private static string _updateFileURL;
        private const string UpdateFileDnsTxt = "axtools-update-file-2.axio.name";
        private static string _hardwareID;

        internal static void Start()
        {
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
            OnElapsed();
        }

        internal static void ApplyUpdate(string updateDir, string axtoolsDir)
        {
            try
            {
                log.Info("Copying distr directory, updateDir: " + updateDir + "; axtoolsDir: " + axtoolsDir);
                Utils.DirectoryCopy(updateDir, axtoolsDir, true);
                var pluginsDirectoryInfo = new DirectoryInfo(Path.Combine(axtoolsDir, "pluginsAssemblies"));
                if (pluginsDirectoryInfo.Exists)
                {
                    log.Info("Deleting plug-ins assemblies directory...");
                    pluginsDirectoryInfo.Delete(true);
                }
                log.Info("Deleting junk files...");
                foreach (string i in JunkFiles)
                {
                    try
                    {
                        File.Delete(Path.Combine(axtoolsDir, i));
                        log.Info("Junk file is deleted: " + Path.Combine(axtoolsDir, i));
                    }
#pragma warning disable CC0004
                    catch { /* don't care why */ }
#pragma warning restore CC0004
                }
                foreach (string junkFolder in JunkFolders)
                {
                    try
                    {
                        Directory.Delete(Path.Combine(axtoolsDir, junkFolder), true);
                    }
#pragma warning disable CC0004
                    catch { /* don't care why */ }
#pragma warning restore CC0004
                }
                log.Info("ApplyUpdate: done");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Critical update error", nameof(AxTools), ex.Message, TaskDialogButton.Close, TaskDialogIcon.Stop);
            }
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnElapsed();
        }

        private static void OnElapsed()
        {
            Task.Run(() =>
            {
                _timer.Stop();
                if (_hardwareID == null)
                {
                    _hardwareID = Utils.GetComputerHID();
                    log.Info($"Your credentials for updates: login: {Settings2.Instance.UserID} password: {_hardwareID}");
                }
                if (_updateFileURL == null)
                {
                    _updateFileURL = GetUpdateFileURL(UpdateFileDnsTxt);
                    log.Info("Update file URL: " + (_updateFileURL ?? "UNKNOWN"));
                }
                if (_updateFileURL != null && MainWindow.Instance != null)
                {
                    CheckForUpdates();
                }
                else
                {
                    log.Info("Can't get update file URL from DNS!");
                }
                _timer.Start();
            });
        }

        private static void DownloadExtractUpdate()
        {
            var distrZipFile = AppFolders.TempDir + "\\_distr.zip";
            File.Delete(distrZipFile);
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.ForceBasicAuth(Settings2.Instance.UserID, _hardwareID);
                    var distrFile = webClient.UploadData(_updateFileURL, "POST", Encoding.UTF8.GetBytes("get-update-package"));
                    File.WriteAllBytes(distrZipFile, distrFile);
                    log.Info("Packages are downloaded!");
                }
                catch (Exception ex)
                {
                    log.Error("Can't download packages: " + ex.Message);
                    return;
                }
            }
            var updateDirectoryInfo = new DirectoryInfo(DistrDirectory);
            if (updateDirectoryInfo.Exists)
            {
                updateDirectoryInfo.Delete(true);
            }
            updateDirectoryInfo.Create();
            try
            {
                using (ZipFile zip = new ZipFile(distrZipFile, Encoding.UTF8))
                {
                    zip.Password = "3aTaTre6agA$-E+e";
                    zip.ExtractAll(DistrDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
                log.Info("Package <distr> is extracted to " + DistrDirectory);
            }
            catch (Exception ex)
            {
                log.Error("Can't extract <distr> package: " + ex.Message);
                return;
            }
            var updaterExe = new DirectoryInfo(DistrDirectory).GetFileSystemInfos().Where(l => l is FileInfo).Cast<FileInfo>().FirstOrDefault(info => info.Extension == ".exe");
            if (updaterExe == null)
            {
                log.Error("Can't find updater executable! Files: " + string.Join(", ", new DirectoryInfo(AppFolders.TempDir).GetFileSystemInfos().Where(l => l is FileInfo).Cast<FileInfo>().Select(l => l.Name)));
                return;
            }

            if (WoWProcessManager.Processes.Count > 0)
            {
                WoWProcessManager.WoWProcessClosed += WoWProcessManager_WoWProcessClosed;
                Notify.TrayPopup("Update for AxTools is ready", "AxTools can't update itself while WoW is running. You will be prompted to update as soon as you close all running WoW clients", NotifyUserType.Info, false, null, 60);
                log.Info("WoW client is running, waiting...");
            }
            else
            {
                NotifyAboutUpdate();
            }
        }

        private static void WoWProcessManager_WoWProcessClosed(int obj)
        {
            if (WoWProcessManager.Processes.Count == 0)
            {
                log.Info("All WoW clients are closed");
                WoWProcessManager.WoWProcessClosed -= WoWProcessManager_WoWProcessClosed;
                var mainWindowWowLaunchLock = MainWindow.Instance.WoWLaunchLock.GetLock();
                try
                {
                    NotifyAboutUpdate();
                }
                finally
                {
                    MainWindow.Instance.WoWLaunchLock.ReleaseLock(mainWindowWowLaunchLock);
                }
            }
        }

        private static void NotifyAboutUpdate()
        {
            void CloseAndUpdate()
            {
                try
                {
                    log.Info("Closing for update...");
                    Application.ApplicationExit += ApplicationOnApplicationExit;
                    MainWindow.Instance.BeginInvoke(new Action(MainWindow.Instance.Close));
                }
                catch (Exception ex)
                {
                    log.Error("Update error: " + ex.Message);
                }
            }

            if (WinAPI.NativeMethods.GetForegroundWindow() == MainWindow.Instance.Handle)
            {
                var taskDialog = new TaskDialog("Update is available", nameof(AxTools), "Do you wish to restart now?", (TaskDialogButton)((int)TaskDialogButton.Yes + (int)TaskDialogButton.No), TaskDialogIcon.Information);
                if (taskDialog.Show(MainWindow.Instance).CommonButton == Result.Yes)
                {
                    CloseAndUpdate();
                }
            }
            else
            {
                // 1-hour notification!
                Notify.TrayPopup("Update for AxTools is ready to install", "Click here to install", NotifyUserType.Info, false, null, 60 * 60, (sender, args) => { if (args.Button == MouseButtons.Left) CloseAndUpdate(); });
            }
        }

        private static void CheckForUpdates()
        {
            log.Info("Let's check for updates!");
            string updateString;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.ForceBasicAuth(Settings2.Instance.UserID, _hardwareID);
                    updateString = webClient.UploadString(_updateFileURL, "POST", "get-update-info");
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Notify.TrayPopup("AxTools update error!", "Your credentials are invalid. Updater is disabled. Please contact dev", NotifyUserType.Error, false);
                    _timer.Elapsed -= Timer_Elapsed;
                    log.Info($"Your credentials are invalid. Updater is disabled. Please contact devs (status {webEx.Status}): {webEx.Message}");
                    log.Error($"Your user name: {Settings2.Instance.UserID}; your HWID: {_hardwareID}");
                }
                else if (webEx.Status == WebExceptionStatus.TrustFailure)
                {
                    Notify.TrayPopup("AxTools update error!", "Cannot validate remote server. Your Internet connection is compromised", NotifyUserType.Error, true);
                    _timer.Elapsed -= Timer_Elapsed;
                    log.Info($"Cannot validate remote server. Your Internet connection is compromised (status {webEx.Status}): {webEx.Message}");
                    log.Info($"Inner exception: {webEx.InnerException?.Message}");
                }
                else if (webEx.Status != WebExceptionStatus.NameResolutionFailure &&
                         webEx.Status != WebExceptionStatus.Timeout &&
                         webEx.Status != WebExceptionStatus.ConnectFailure &&
                         webEx.Status != WebExceptionStatus.SecureChannelFailure)
                {
                    log.Error($"Fetching info error (status {webEx.Status}): {webEx.Message}");
                }
                return;
            }
            catch (Exception ex)
            {
                log.Info($"Fetching info error ({ex.GetType()}): {ex.Message}");
                return;
            }
            if (!string.IsNullOrWhiteSpace(updateString))
            {
                try
                {
                    var updateInfo = UpdateInfo2.FromJSON(updateString);
                    if (updateInfo != null)
                    {
                        if (Globals.AppVersion != updateInfo.Version)
                        {
                            log.Info($"Server version: <{updateInfo.Version}>, local version: <{Globals.AppVersion}>; downloading new version...");
                            _timer.Elapsed -= Timer_Elapsed;
                            DownloadExtractUpdate();
                        }
                        else
                        {
                            log.Info($"Server version: <{updateInfo.Version}>, local version: <{Globals.AppVersion}>; no update is needed");
                        }
                    }
                    else
                    {
                        log.Error("UpdateInfo has invalid format!");
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Update file fetched, but it has invalid format: " + ex.Message);
                }
            }
            else
            {
                log.Error("Update file is fetched, but it's empty!");
            }
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs eventArgs)
        {
            Application.ApplicationExit -= ApplicationOnApplicationExit;
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(DistrDirectory, "AxTools.exe"),
                Arguments = $"-update-dir \"{DistrDirectory}\" -axtools-dir \"{Application.StartupPath}\"" // if you change arguments, don't forget to change regex in <ApplyUpdate> method
            });
        }

        private static string GetUpdateFileURL(string hostname)
        {
            var startInfo = new ProcessStartInfo("nslookup")
            {
                Arguments = $"-type=TXT {hostname}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process cmd = Process.Start(startInfo))
            {
                if (cmd != null)
                {
                    try
                    {
                        var output = cmd.StandardOutput.ReadToEnd();
                        var match = Regex.Match(output, "text =\\s*\"(.+)\"");
                        if (match.Success)
                        {
                            var result = Uri.TryCreate(match.Groups[1].Value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                            return result ? match.Groups[1].Value : null;
                        }
                        return null;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                return null;
            }
        }

    }
}