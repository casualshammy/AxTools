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
using WindowsFormsAero.TaskDialog;

namespace AxTools.Helpers
{
    internal class WoWLogsAndCacheManager
    {
        private static int _prevProcent = -1;
        internal static event Action<int> StateChanged;

        internal static void ZipAndCleanLogs()
        {
            if (Directory.Exists(Settings.Instance.WoWDirectory + "\\Logs"))
            {
                TaskDialog taskDialog = new TaskDialog("WoW client is blocking logs", "AxTools", "It's strongly recommended to close all WoW clients.\r\nSome files may not be deleted.\r\nProceed?",
                    (int) TaskDialogButton.Yes + TaskDialogButton.No, TaskDialogIcon.Warning);
                if (WowProcess.GetAllWoWProcesses().Count == 0 || taskDialog.Show(MainForm.Instance).CommonButton == Result.Yes)
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
            if (e.EntriesSaved != 0 && e.EntriesTotal != 0 && e.EntriesTotal >= e.EntriesSaved)
            {
                int procent = 100*e.EntriesSaved/e.EntriesTotal;
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
