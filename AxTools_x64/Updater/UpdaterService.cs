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
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Updater
{
    [Obfuscation(Exclude = false, Feature = "constants")]
    internal static class UpdaterService
    {
        private static readonly System.Timers.Timer _timer = new System.Timers.Timer(900000);
        private static readonly string DistrDirectory = Globals.TempPath + "\\update";
        private static string _updateFileURL;
        private const string UpdateFileDnsTxt = "axtools-update-file-1.axio.name";
        private static string _hardwareID;

        internal static void Start()
        {
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
            OnElapsed();
        }

        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
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
                    Log.Info(string.Format("[Updater] Your credentials for updates: login: {0} password: {1}", Settings.Instance.UserID, _hardwareID));
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
                    Log.Error("[Updater] Can't get update file URL from DNS!");
                }
                _timer.Start();
            });
        }

        private static void DownloadExtractUpdate(UpdateInfo0 updateInfo)
        {
            AppSpecUtils.CheckCreateTempDir();
            string distrZipFile = Globals.TempPath + "\\_distr.zip";
            string updaterZipFile = Globals.TempPath + "\\_updater.zip";
            File.Delete(distrZipFile);
            File.Delete(updaterZipFile);
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.ForceBasicAuth(Settings.Instance.UserID, _hardwareID);
                    webClient.DownloadFile(updateInfo.DistrZipURL, distrZipFile);
                    webClient.DownloadFile(updateInfo.UpdaterZipURL, updaterZipFile);
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
            try
            {
                using (ZipFile zip = new ZipFile(updaterZipFile, Encoding.UTF8))
                {
                    zip.Password = "3aTaTre6agA$-E+e";
                    zip.ExtractAll(Globals.TempPath, ExtractExistingFileAction.OverwriteSilently);
                }
                Log.Info("[Updater] Package <updater> is extracted to " + Globals.TempPath);
            }
            catch (Exception ex)
            {
                Log.Error("[Updater] Can't extract <updater> package: " + ex.Message);
                return;
            }
            FileInfo updaterExe = new DirectoryInfo(Globals.TempPath).GetFileSystemInfos().Where(l => l is FileInfo).Cast<FileInfo>().FirstOrDefault(info => info.Extension == ".exe");
            if (updaterExe == null)
            {
                Log.Error("[Updater] Can't find updater executable! Files: " + string.Join(", ", new DirectoryInfo(Globals.TempPath).GetFileSystemInfos().Where(l => l is FileInfo).Cast<FileInfo>().Select(l => l.Name)));
                return;
            }
            TaskDialog taskDialog = new TaskDialog("Update is available", "AxTools", "Do you wish to restart now?", (TaskDialogButton) ((int) TaskDialogButton.Yes + (int) TaskDialogButton.No), TaskDialogIcon.Information);
            AppSpecUtils.NotifyUser("Update for AxTools is ready to install", "Click on icon to install", NotifyUserType.Info, true);
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
                    updateString = webClient.DownloadString(_updateFileURL);
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response is HttpWebResponse && ((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    AppSpecUtils.NotifyUser("AxTools update error!", "Your credentials are invalid. Updater is disabled. Please contact devs", NotifyUserType.Error, false);
                    _timer.Elapsed -= timer_Elapsed;
                }
                else if (webEx.Status == WebExceptionStatus.TrustFailure || webEx.Status == WebExceptionStatus.SecureChannelFailure)
                {
                    AppSpecUtils.NotifyUser("AxTools update error!", "Cannot validate remote server. Your internet connection is compromised", NotifyUserType.Error, false);
                    _timer.Elapsed -= timer_Elapsed;
                }
                else
                {
                    Log.Error(string.Format("[Updater] Fetching info error (status {0}): {1}", webEx.Status, webEx.Message));
                }
                return;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("[Updater] Fetching info error ({0}): {1}", ex.GetType(), ex.Message));
                return;
            }
            if (!string.IsNullOrWhiteSpace(updateString))
            {
                try
                {
                    UpdateInfo0 updateInfo = UpdateInfo0.FromJSON(updateString);
                    if (updateInfo != null)
                    {
                        Log.Info("[Updater] Server version: <" + updateInfo.Version + ">, local version: <" + Globals.AppVersion + ">");
                        if (Globals.AppVersion != updateInfo.Version)
                        {
                            Log.Info("[Updater] Downloading new version...");
                            _timer.Elapsed -= timer_Elapsed;
                            DownloadExtractUpdate(updateInfo);
                        }
                        else
                        {
                            Log.Info("[Updater] No update is needed");
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
                Log.Error("[Updater] Update file fetched, but it's empty!");
            }
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs eventArgs)
        {
            Application.ApplicationExit -= ApplicationOnApplicationExit;
            FileInfo updaterExe = new DirectoryInfo(Globals.TempPath).GetFileSystemInfos().Where(l => l is FileInfo).Cast<FileInfo>().FirstOrDefault(info => info.Extension == ".exe");
            if (updaterExe != null)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterExe.FullName,
                    Arguments = string.Format("-update-dir \"{0}\" -axtools-dir \"{1}\"", DistrDirectory, Application.StartupPath)
                });
            }
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
                        string rawResponse = output.Split(new[] { hostname + "\ttext =\r\n" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        string response = rawResponse.Trim().Trim('"');
                        Uri uriResult;
                        bool result = Uri.TryCreate(response, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                        return result ? response : null;
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
