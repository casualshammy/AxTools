using AxTools.Classes;
using AxTools.Forms;
using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Updater
{
    internal class UpdaterService
    {
        private static readonly System.Timers.Timer Timer = new System.Timers.Timer(900000);

        internal static void Start()
        {
            Timer.Elapsed += timer_Elapsed;
            Timer.Start();
            OnElapsed();
        }

        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnElapsed();
        }

        private static void OnElapsed()
        {
            Task.Factory.StartNew(() =>
            {
                Timer.Stop();
                CheckForUpdates();
                Timer.Start();
            });
        }

        private static void DownloadExtractUpdate()
        {
            Utils.CheckCreateDir();
            string zipFile = Globals.TempPath + "\\__update.zip";
            File.Delete(zipFile);
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFile(Globals.UpdateFilePath + ".zip", zipFile);
                }
                catch (Exception ex)
                {
                    Log.Print("[Updater] Can't download archive: " + ex.Message, true);
                    return;
                }
                try
                {
                    webClient.DownloadFile(Globals.DropboxPath + "/Updater.exe", Application.StartupPath + "\\Updater.exe");
                }
                catch (Exception ex)
                {
                    Log.Print("[Updater] Can't download updater: " + ex.Message, true);
                    return;
                }
            }
            DirectoryInfo updateDirectoryInfo = new DirectoryInfo(Application.StartupPath + "\\update");
            if (updateDirectoryInfo.Exists)
            {
                updateDirectoryInfo.Delete(true);
            }
            updateDirectoryInfo.Create();
            try
            {
                using (ZipFile zip = new ZipFile(zipFile, Encoding.UTF8))
                {
                    zip.ExtractAll(Application.StartupPath + "\\update", ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch (Exception ex)
            {
                Log.Print("[Updater] Can't extract archive: " + ex.Message);
                return;
            }
            TaskDialog taskDialog = new TaskDialog("Update is available", "AxTools", "Do you wish to restart now?", (TaskDialogButton) ((int) TaskDialogButton.Yes + (int) TaskDialogButton.No), TaskDialogIcon.Information);
            MainForm.Instance.ShowNotifyIconMessage("Update for AxTools is ready to install", "Click on icon to install", ToolTipIcon.Info);
            if (taskDialog.Show(MainForm.Instance).CommonButton == Result.Yes)
            {
                try
                {
                    Log.Print("[Updater] Closing for update...");
                    Application.ApplicationExit += ApplicationOnApplicationExit;
                    MainForm.Instance.BeginInvoke(new Action(MainForm.Instance.Close));
                }
                catch (Exception ex)
                {
                    Log.Print("[Updater] Update error: " + ex.Message, true);
                }
            }
        }

        private static void CheckForUpdates()
        {
            Log.Print("[Updater] Checking for updates");
            string updateString;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    updateString = webClient.DownloadString(Globals.UpdateFilePath + ".json");
                }
            }
            catch (WebException webException)
            {
                Log.Print("[Updater] Fetching info error: " + webException.Message);
                return;
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("[Updater] Fetching info error ({0}): {1}", ex.GetType(), ex.Message), true);
                return;
            }
            if (!String.IsNullOrWhiteSpace(updateString))
            {
                UpdateInfo updateInfo = UpdateInfo.InitializeFromJSON(updateString);
                if (updateInfo != null)
                {
                    if (Globals.AppVersion.Build != updateInfo.Version.Build || Globals.AppVersion.Minor != updateInfo.Version.Minor || Globals.AppVersion.Major != updateInfo.Version.Major)
                    {
                        Log.Print("[Updater] New version found: " + updateInfo.Version);
                        Timer.Elapsed -= timer_Elapsed;
                        DownloadExtractUpdate();
                    }
                }
            }
            else
            {
                Log.Print("[Updater] Update file fetched, but it's empty!", true);
            }
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs eventArgs)
        {
            Application.ApplicationExit -= ApplicationOnApplicationExit;
            Process.Start(new ProcessStartInfo {FileName = Application.StartupPath + "\\Updater.exe", WorkingDirectory = Application.StartupPath});
        }

    }
}
