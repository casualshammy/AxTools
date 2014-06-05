using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using WindowsFormsAero.TaskDialog;
using AxTools.Forms;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace AxTools.Classes
{
    internal class Updater
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

        private static void DownloadAndInstallMainExecutable(IEnumerable<string> filesToDownload)
        {
            Log.Print("[Updater] Update for main executable is available");
            DirectoryInfo updateDirectoryInfo = new DirectoryInfo(Application.StartupPath + "\\update");
            if (updateDirectoryInfo.Exists)
            {
                updateDirectoryInfo.Delete(true);
            }
            updateDirectoryInfo.Create();
            using (WebClient webClient = new WebClient())
            {
                foreach (string i in filesToDownload)
                {
                    string fullpath = updateDirectoryInfo.FullName + "\\" + i;
                    File.Delete(fullpath);
                    webClient.DownloadFile(Globals.DropboxPath + "/" + i, fullpath);
                }
            }
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(Globals.DropboxPath + "/Updater.exe", Application.StartupPath + "\\Updater.exe");
                }
            }
            catch (Exception ex)
            {
                Log.Print("[Updater] Can't download updater: " + ex.Message, true);
                return;
            }
            TaskDialog taskDialog = new TaskDialog("Update is available", "AxTools", "Do you wish to restart now?", (TaskDialogButton) ((int) TaskDialogButton.Yes + (int) TaskDialogButton.No),
                TaskDialogIcon.Information);
            MainForm.Instance.ShowNotifyIconMessage("Update for AxTools is ready to install", "Click on icon to install", ToolTipIcon.Info);
            if (taskDialog.Show(MainForm.Instance).CommonButton == Result.Yes)
            {
                try
                {
                    Log.Print("[Updater] Closing for update...");
                    Program.IsRestarting = true;
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
                    updateString = webClient.DownloadString(Globals.UpdateFilePath);
                }
            }
            catch (WebException webException)
            {
                Log.Print("[Updater] Fetching error: " + webException.Message);
                return;
            }
            catch (Exception ex)
            {
                Log.Print("[Updater] Fetching error: " + ex.Message + " :: " + ex.GetType(), true);
                return;
            }
            
            if (!String.IsNullOrWhiteSpace(updateString))
            {
                bool updateForMainExecutableIsAvailable = false;
                string[] filesToDownload = null;
                using (StringReader stringReader = new StringReader(updateString))
                {
                    while (stringReader.Peek() != -1)
                    {
                        try
                        {
                            string nextString = stringReader.ReadLine();
                            if (nextString != null)
                            {
                                string[] pair = nextString.Split(new[] { ":::::" }, StringSplitOptions.None);
                                switch (pair[0])
                                {
                                    case "CurrentAxToolsVersion":
                                        Version localVersion = Assembly.GetExecutingAssembly().GetName().Version;
                                        Version serverVersion = new Version(pair[1]);
                                        if (localVersion.Major != serverVersion.Major || localVersion.Minor != serverVersion.Minor || localVersion.Build != serverVersion.Build)
                                        {
                                            updateForMainExecutableIsAvailable = true;
                                        }
                                        break;
                                    case "FilesToDownload":
                                        filesToDownload = pair[1].Split(',');
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Print("[Updater] Parsing error: " + ex.Message, true);
                        }
                    }
                }
                if (updateForMainExecutableIsAvailable && filesToDownload != null && filesToDownload.Count(i => !string.IsNullOrWhiteSpace(i)) > 0)
                {
                    Timer.Elapsed -= timer_Elapsed;
                    DownloadAndInstallMainExecutable(filesToDownload);
                }
            }
            else
            {
                Log.Print("[Updater] Update file fetched, but it's empty!", true);
            }
        }

    }
}
