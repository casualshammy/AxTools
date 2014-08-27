using AxTools.Classes;
using AxTools.Forms;
using AxTools.WoW;
using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Helpers
{
    internal class WoWLogsAndCacheManager
    {
        private static int _prevProcent = -1;

        internal static event Action<int> StateChanged;

        internal static void StartupCheck()
        {
            if (Settings.Instance.WoWNotifyIfBigLogs && Directory.Exists(Settings.Instance.WoWDirectory + "\\Logs") && WowProcess.GetAllWowProcesses().Count == 0)
            {
                long directorySize = Utils.CalcDirectorySize(Settings.Instance.WoWDirectory + "\\Logs");
                double logsFolserSizeInMegabytes = Math.Round((float)directorySize / 1024 / 1024, 3, MidpointRounding.ToEven);
                Log.Print("[WoW logs watcher] Log directory size: " + logsFolserSizeInMegabytes + " MB");
                if (directorySize > Settings.Instance.WoWNotifyIfBigLogsSize * 1024 * 1024)
                {
                    DirectoryInfo rootDir = new DirectoryInfo(Settings.Instance.WoWDirectory + "\\Logs").Root;
                    DriveInfo drive = DriveInfo.GetDrives().FirstOrDefault(i => i.RootDirectory.FullName == rootDir.FullName);
                    if (drive != null)
                    {
                        double freeSpace = Math.Round((float)drive.TotalFreeSpace / 1024 / 1024, 3, MidpointRounding.ToEven);
                        MainForm.Instance.ShowNotifyIconMessage("Log directory is too large!", string.Format("Logs folder size: {0} MB\r\nDisk free space: {1} MB", logsFolserSizeInMegabytes, freeSpace), ToolTipIcon.Warning);
                        Utils.PlaySystemNotificationAsync();
                    }
                }
            }
            DeleteCreatureCache();
        }

        internal static void DeleteCreatureCache()
        {
            if (Settings.Instance.WoWWipeCreatureCache && Directory.Exists(Settings.Instance.WoWDirectory + "\\Cache\\WDB"))
            {
                foreach (DirectoryInfo i in new DirectoryInfo(Settings.Instance.WoWDirectory + "\\Cache\\WDB").GetDirectories().Where(i => File.Exists(i.FullName + "\\creaturecache.wdb")))
                {
                    try
                    {
                        File.Delete(i.FullName + "\\creaturecache.wdb");
                        Log.Print("[Cache cleaner] " + i.FullName + "\\creaturecache.wdb was deleted");
                    }
                    catch (Exception ex)
                    {
                        Log.Print("[Cache cleaner] Can't delete cache file [" + i.FullName + "\\creaturecache.wdb] :" + ex.Message);
                    }
                }
            }
        }

        internal static void ZipAndCleanLogs()
        {
            if (Directory.Exists(Settings.Instance.WoWDirectory + "\\Logs"))
            {
                TaskDialog taskDialog = new TaskDialog("WoW client is blocking logs", "AxTools", "It's strongly recommended to close all WoW clients.\r\nSome files may not be deleted.\r\nProceed?",
                    (int) TaskDialogButton.Yes + TaskDialogButton.No, TaskDialogIcon.Warning);
                if (WowProcess.GetAllWowProcesses().Count == 0 || taskDialog.Show(MainForm.Instance).CommonButton == Result.Yes)
                {
                    string zipPath = String.Format(Settings.Instance.WoWDirectory + "\\Logs\\WoWLogs_{0:yyyyMMdd_HHmmss}.zip", DateTime.UtcNow);
                    try
                    {
                        using (ZipFile zip = new ZipFile(zipPath, Encoding.UTF8))
                        {
                            zip.CompressionLevel = (CompressionLevel) Settings.Instance.WoWAddonsBackupCompressionLevel;
                            foreach (string filePath in Directory.GetFiles(Settings.Instance.WoWDirectory + "\\Logs").Where(k => !k.Contains("WoWLogs_")))
                            {
                                zip.AddFile(filePath);
                            }
                            zip.SaveProgress += SaveProgress;
                            ProcessPriorityClass defaultProcessPriorityClass = Process.GetCurrentProcess().PriorityClass;
                            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
                            zip.Save();
                            Process.GetCurrentProcess().PriorityClass = defaultProcessPriorityClass;
                            zip.SaveProgress -= SaveProgress;
                        }
                        Log.Print(String.Format("[WoW logs] Logs were saved to [{0}]", zipPath));
                        foreach (string i in Directory.GetFiles(Settings.Instance.WoWDirectory + "\\Logs").Where(k => !k.Contains("WoWLogs_")))
                        {
                            try
                            {
                                File.WriteAllBytes(i, new byte[0]);
                                Log.Print("[WoW logs] Log file erased: " + i);
                            }
                            catch (Exception ex)
                            {
                                Log.Print(String.Format("[WoW logs] Error occured while erasing log file [{0}]: {1}", i, ex.Message));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Print(String.Format("[WoW logs] Can't backup WoW logs [{0}]: {1}", zipPath, ex.Message), true);
                        Utils.NotifyUser("WoW logs", "Can't backup WoW logs [" + zipPath + "]: " + ex.Message, NotifyUserType.Error, true);
                    }
                }
            }
            else
            {
                Utils.NotifyUser("WoW logs", "Logs directory doesn't exist", NotifyUserType.Error, true);
            }
        }

        internal static void DeleteAllLogsArchivesExceptNewest()
        {
            DirectoryInfo backupDirectory = new DirectoryInfo(Settings.Instance.WoWDirectory + "\\Logs");
            List<FileInfo> backupFiles = backupDirectory.GetFileSystemInfos().Where(i => i.Name.Contains("WoWLogs_") && i is FileInfo).Cast<FileInfo>().ToList();
            if (backupFiles.Count > 1)
            {
                // I place newest file to the end of list
                backupFiles.Sort((first, second) =>
                {
                    if (first.CreationTimeUtc > second.CreationTimeUtc)
                    {
                        return 1;
                    }
                    return -1;
                });
                for (int i = 0; i < backupFiles.Count - 1; i++)
                {
                    try
                    {
                        backupFiles[i].Delete();
                        Log.Print("[WoW logs] Old archive has been deleted [" + backupFiles[i].FullName + "]");
                    }
                    catch (Exception ex)
                    {
                        Log.Print("[WoW logs] Can't delete old archive [" + backupFiles[i].FullName + "]: " + ex.Message);
                    }
                }
            }
        }

        private static void SaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (e.BytesTransferred != 0 && e.TotalBytesToTransfer != 0 && e.TotalBytesToTransfer >= e.BytesTransferred)
            {
                int procent = (int) (100*e.BytesTransferred/e.TotalBytesToTransfer);
                if (procent != _prevProcent)
                {
                    _prevProcent = procent;
                    if (StateChanged != null)
                    {
                        StateChanged(procent);
                    }
                }
            }
        }
    
    }
}
