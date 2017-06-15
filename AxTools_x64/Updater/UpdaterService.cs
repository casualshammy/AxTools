using AxTools.Forms;
using AxTools.Helpers;
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
        private static readonly System.Timers.Timer _timer = new System.Timers.Timer(600000);
        private static readonly string DistrDirectory = AppFolders.TempDir + "\\update";
        private static readonly string[] JunkFiles = {};
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
                Log.Info("Copying distr directory, updateDir: " + updateDir + "; axtoolsDir: " + axtoolsDir);
                Utils.DirectoryCopy(updateDir, axtoolsDir, true);
                DirectoryInfo pluginsDirectoryInfo = new DirectoryInfo(Path.Combine(axtoolsDir, "pluginsAssemblies"));
                if (pluginsDirectoryInfo.Exists)
                {
                    Log.Info("Deleting plugins assemblies directory...");
                    pluginsDirectoryInfo.Delete(true);
                }
                Log.Info("Deleting junk files...");
                foreach (string i in JunkFiles)
                {
                    // ReSharper disable once RedundantEmptyFinallyBlock
                    try
                    {
                        File.Delete(Path.Combine(axtoolsDir, i));
                        Log.Info("Junk file is deleted: " + Path.Combine(axtoolsDir, i));
                    }
                    catch {/* */}
                }
                foreach (string junkFolder in JunkFolders)
                {
                    try
                    {
                        Directory.Delete(Path.Combine(axtoolsDir, junkFolder), true);
                    }
                    catch {/* */}
                }
                Log.Info("Starting AxTools...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.Combine(axtoolsDir, "AxTools.exe"),
                    WorkingDirectory = axtoolsDir
                });
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Critical update error", "AxTools", ex.Message, TaskDialogButton.Close, TaskDialogIcon.Stop);
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
                    Log.Info(string.Format("[Updater] Your credentials for updates: login: {0} password: {1}{2}", Settings.Instance.UserID, new string('*', _hardwareID.Length - 4), _hardwareID.Substring(_hardwareID.Length - 4)));
                }
                if (_updateFileURL == null)
                {
                    _updateFileURL = GetUpdateFileURL(UpdateFileDnsTxt);
                    Log.Info("[Updater] Update file URL: " + (_updateFileURL ?? "UNKNOWN"));
                }
                if (_updateFileURL != null)
                {
                    CheckForUpdates();
                }
                else
                {
                    Log.Info("[Updater] Can't get update file URL from DNS!");
                }
                _timer.Start();
            });
        }

        private static void DownloadExtractUpdate()
        {
            string distrZipFile = AppFolders.TempDir + "\\_distr.zip";
            File.Delete(distrZipFile);
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.ForceBasicAuth(Settings.Instance.UserID, _hardwareID);
                    byte[] distrFile = webClient.UploadData(_updateFileURL, "POST", Encoding.UTF8.GetBytes("get-update-package"));
                    File.WriteAllBytes(distrZipFile, distrFile);
                    Log.Info("[Updater] Packages are downloaded!");
                }
                catch (Exception ex)
                {
                    Log.Error("[Updater] Can't download packages: " + ex.Message);
                    return;
                }
            }
            DirectoryInfo updateDirectoryInfo = new DirectoryInfo(DistrDirectory);
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
                Log.Info("[Updater] Package <distr> is extracted to " + DistrDirectory);
            }
            catch (Exception ex)
            {
                Log.Error("[Updater] Can't extract <distr> package: " + ex.Message);
                return;
            }
            FileInfo updaterExe = new DirectoryInfo(DistrDirectory).GetFileSystemInfos().Where(l => l is FileInfo).Cast<FileInfo>().FirstOrDefault(info => info.Extension == ".exe");
            if (updaterExe == null)
            {
                Log.Error("[Updater] Can't find updater executable! Files: " + string.Join(", ", new DirectoryInfo(AppFolders.TempDir).GetFileSystemInfos().Where(l => l is FileInfo).Cast<FileInfo>().Select(l => l.Name)));
                return;
            }
            TaskDialog taskDialog = new TaskDialog("Update is available", "AxTools", "Do you wish to restart now?", (TaskDialogButton) ((int) TaskDialogButton.Yes + (int) TaskDialogButton.No), TaskDialogIcon.Information);
            Notify.TrayPopup("Update for AxTools is ready to install", "Click here to install", NotifyUserType.Info, false, null, 10, (sender, args) => MainForm.Instance.ActivateBrutal());
            if (taskDialog.Show(MainForm.Instance).CommonButton == Result.Yes)
            {
                try
                {
                    Log.Info("[Updater] Closing for update...");
                    Application.ApplicationExit += ApplicationOnApplicationExit;
                    MainForm.Instance.BeginInvoke(new Action(MainForm.Instance.Close));
                }
                catch (Exception ex)
                {
                    Log.Error("[Updater] Update error: " + ex.Message);
                }
            }
        }

        private static void CheckForUpdates()
        {
            Log.Info("[Updater] Let's check for updates!");
            string updateString;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.ForceBasicAuth(Settings.Instance.UserID, _hardwareID);
                    updateString = webClient.UploadString(_updateFileURL, "POST", "get-update-info");
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response is HttpWebResponse && ((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    Notify.TrayPopup("AxTools update error!", "Your credentials are invalid. Updater is disabled. Please contact dev", NotifyUserType.Error, false);
                    _timer.Elapsed -= Timer_Elapsed;
                    Log.Info($"[Updater] Your credentials are invalid. Updater is disabled. Please contact devs (status {webEx.Status}): {webEx.Message}");
                    Log.Error($"[Updater] Your username: {Settings.Instance.UserID}; your HWID: {_hardwareID}");
                }
                else if (webEx.Status == WebExceptionStatus.TrustFailure)
                {
                    Notify.TrayPopup("AxTools update error!", "Cannot validate remote server. Your internet connection is compromised", NotifyUserType.Error, true);
                    _timer.Elapsed -= Timer_Elapsed;
                    Log.Info($"[Updater] Cannot validate remote server. Your internet connection is compromised (status {webEx.Status}): {webEx.Message}");
                    Log.Info($"[Updater] Inner exception: {webEx.InnerException?.Message}");
                }
                else if (webEx.Status != WebExceptionStatus.NameResolutionFailure &&
                         webEx.Status != WebExceptionStatus.Timeout &&
                         webEx.Status != WebExceptionStatus.ConnectFailure &&
                         webEx.Status != WebExceptionStatus.SecureChannelFailure)
                {
                    Log.Error(string.Format("[Updater] Fetching info error (status {0}): {1}", webEx.Status, webEx.Message));
                }
                return;
            }
            catch (Exception ex)
            {
                Log.Info($"[Updater] Fetching info error ({ex.GetType()}): {ex.Message}");
                return;
            }
            if (!string.IsNullOrWhiteSpace(updateString))
            {
                try
                {
                    UpdateInfo2 updateInfo = UpdateInfo2.FromJSON(updateString);
                    if (updateInfo != null)
                    {
                        if (Globals.AppVersion != updateInfo.Version)
                        {
                            Log.Info($"[Updater] Server version: <{updateInfo.Version}>, local version: <{Globals.AppVersion}>; downloading new version...");
                            _timer.Elapsed -= Timer_Elapsed;
                            DownloadExtractUpdate();
                        }
                        else
                        {
                            Log.Info($"[Updater] Server version: <{updateInfo.Version}>, local version: <{Globals.AppVersion}>; no update is needed");
                        }
                    }
                    else
                    {
                        Log.Error("[Updater] UpdateInfo has invalid format!");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("[Updater] Update file fetched, but it has invalid format: " + ex.Message);
                }
            }
            else
            {
                Log.Error("[Updater] Update file is fetched, but it's empty!");
            }
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs eventArgs)
        {
            Application.ApplicationExit -= ApplicationOnApplicationExit;
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(DistrDirectory, "AxTools.exe"),
                Arguments = string.Format("-update-dir \"{0}\" -axtools-dir \"{1}\"", DistrDirectory, Application.StartupPath) // if you change arguments, don't forget to change regex in <ApplyUpdate> method
            });
        }

        private static string GetUpdateFileURL(string hostname)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("nslookup")
            {
                Arguments = string.Format("-type=TXT {0}", hostname),
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
                        string output = cmd.StandardOutput.ReadToEnd();
                        Match match = Regex.Match(output, "text =\\s*\"(.+)\"");
                        if (match.Success)
                        {
                            bool result = Uri.TryCreate(match.Groups[1].Value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                            return result ? match.Groups[1].Value : null;
                        }
                        return null;
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
        }

    }
}
