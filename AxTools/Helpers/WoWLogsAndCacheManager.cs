using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.Classes;
using AxTools.Forms;
using AxTools.WoW;
using Ionic.Zip;
using Ionic.Zlib;

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
                Log.Print("[WoW logs watcher] Log directory size: " + Math.Round((float)directorySize / 1024 / 1024, 3, MidpointRounding.ToEven) + " MB");
                if (directorySize > Settings.Instance.WoWNotifyIfBigLogsSize * 1024 * 1024)
                {
                    DirectoryInfo rootDir = new DirectoryInfo(Settings.Instance.WoWDirectory + "\\Logs").Root;
                    DriveInfo drive = DriveInfo.GetDrives().FirstOrDefault(i => i.RootDirectory.FullName == rootDir.FullName);
                    if (drive != null)
                    {
                        double freeSpace = Math.Round((float)drive.TotalFreeSpace / 1024 / 1024, 3, MidpointRounding.ToEven);
                        MainForm.Instance.ShowNotifyIconMessage("Log directory is too large!", "Disk free space: " + freeSpace + " MB", ToolTipIcon.Warning);
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
                TaskDialog taskDialog = new TaskDialog("WoW client is blocking logs", "AxTools", "It's strongly recommended to close all WoW clients.\r\nSome files may not be deleted.\r\nProceed?", (int)TaskDialogButton.Yes + TaskDialogButton.No, TaskDialogIcon.Warning);
                if (WowProcess.GetAllWowProcesses().Count == 0 || taskDialog.Show(MainForm.Instance).CommonButton == Result.Yes)
                {
                    string zipPath = String.Format(Settings.Instance.WoWDirectory + "\\Logs\\WoWLogs_{0:yyyyMMdd_HHmmss}.zip", DateTime.UtcNow);
                    try
                    {
                        using (ZipFile zip = new ZipFile(zipPath, Encoding.UTF8))
                        {
                            zip.CompressionLevel = (CompressionLevel)Settings.Instance.WoWAddonsBackupCompressionLevel;
                            foreach (string filePath in Directory.GetFiles(Settings.Instance.WoWDirectory + "\\Logs").Where(k => !k.Contains("WoWLogs_")))
                            {
                                zip.AddFile(filePath);
                            }
                            zip.SaveProgress += SaveProgress;
                            zip.Save();
                            zip.SaveProgress -= SaveProgress;
                        }
                        Log.Print(String.Format("[WoW logs] Logs were saved to [{0}]", zipPath));
                        foreach (string i in Directory.GetFiles(Settings.Instance.WoWDirectory + "\\Logs").Where(k => !k.Contains("WoWLogs_")))
                        {
                            try
                            {
                                File.WriteAllText(i, string.Empty, Encoding.UTF8);
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
        
        private static void SaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (e.EntriesSaved != 0 && e.EntriesTotal != 0 && e.EntriesTotal >= e.EntriesSaved)
            {
                int procent = 100 * e.EntriesSaved / e.EntriesTotal;
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
