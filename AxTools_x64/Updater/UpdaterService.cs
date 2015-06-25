﻿using AxTools.Forms;
using AxTools.Helpers;
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
            AppSpecUtils.CheckCreateDir();
            string zipFile = Globals.TempPath + "\\__update.zip";
            File.Delete(zipFile);
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFile(Globals.UpdateFilePath + ".zip", zipFile);
                    Log.Info("[Updater] Package is downloaded!");
                }
                catch (Exception ex)
                {
                    Log.Error("[Updater] Can't download archive: " + ex.Message);
                    return;
                }
                try
                {
                    webClient.DownloadFile(Globals.DropboxPath + "/Updater.exe", Application.StartupPath + "\\Updater.exe");
                    Log.Info("[Updater] Updater executable is downloaded!");
                }
                catch (Exception ex)
                {
                    Log.Error("[Updater] Can't download updater: " + ex.Message);
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
                string path = Application.StartupPath + "\\update";
                using (ZipFile zip = new ZipFile(zipFile, Encoding.UTF8))
                {
                    zip.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                }
                Log.Info("[Updater] Package is extracted to " + path);
            }
            catch (Exception ex)
            {
                Log.Info("[Updater] Can't extract archive: " + ex.Message);
                return;
            }
            TaskDialog taskDialog = new TaskDialog("Update is available", "AxTools", "Do you wish to restart now?", (TaskDialogButton) ((int) TaskDialogButton.Yes + (int) TaskDialogButton.No), TaskDialogIcon.Information);
            MainForm.Instance.ShowNotifyIconMessage("Update for AxTools is ready to install", "Click on icon to install", ToolTipIcon.Info);
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
                    updateString = webClient.DownloadString(Globals.UpdateFilePath + ".json");
                }
            }
            catch (WebException webException)
            {
                Log.Info("[Updater] Fetching info error: " + webException.Message);
                return;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("[Updater] Fetching info error ({0}): {1}", ex.GetType(), ex.Message));
                return;
            }
            if (!String.IsNullOrWhiteSpace(updateString))
            {
                try
                {
                    UpdateInfo updateInfo = UpdateInfo.InitializeFromJSON(updateString);
                    if (updateInfo != null)
                    {
                        Log.Info("[Updater] Server version: <" + updateInfo.Version + ">, local version: <" + Globals.AppVersion + ">");
                        if (Globals.AppVersion != updateInfo.Version)
                        {
                            Log.Info("[Updater] Downloading new version...");
                            Timer.Elapsed -= timer_Elapsed;
                            DownloadExtractUpdate();
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
            Process.Start(new ProcessStartInfo {FileName = Application.StartupPath + "\\Updater.exe", WorkingDirectory = Application.StartupPath});
        }

    }
}
