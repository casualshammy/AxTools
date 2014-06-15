﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.Forms;
using Ionic.Zip;
using Ionic.Zlib;
using Timer = System.Timers.Timer;

namespace AxTools.Classes
{
    internal static class AddonsBackup
    {
        private static int _prevProcent = -1;
        private static bool _isBackingUp;
        private static Timer _timer;

        internal static void StartService()
        {
            _timer = new Timer(10000);
            _timer.Elapsed += Timer_OnElapsed;
        }

        internal static void StopService()
        {
            if (_timer != null)
            {
                _timer.Elapsed -= Timer_OnElapsed;
                _timer.Close();
            }
        }

        internal static void StartOnDemand()
        {
            Start(false);
        }

        private static void Timer_OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            TimeSpan pTimeSpan = DateTime.UtcNow - Settings.AddonsBackupLastdate;
            if (pTimeSpan.TotalHours >= Settings.AddonsBackupTimer && !_isBackingUp)
            {
                Settings.AddonsBackupLastdate = DateTime.UtcNow;
                ProcessPriorityClass defaultProcessPriorityClass = Process.GetCurrentProcess().PriorityClass;
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
                MainForm.Instance.AddonsBackup_OnChangedState(-1);
                Task.Factory.StartNew(() => Start(true))
                    .ContinueWith(l =>
                    {
                        Process.GetCurrentProcess().PriorityClass = defaultProcessPriorityClass;
                        MainForm.Instance.AddonsBackup_OnChangedState(101);
                    });
            }
        }

        private static void Start(bool trayMode)
        {
            _isBackingUp = true;
            CheckWTFDirectoryResult checkWTFDirectoryResult = CheckWTFDirectory();
            switch (checkWTFDirectoryResult)
            {
                case CheckWTFDirectoryResult.OK:
                    Log.Print("BackupAddons :: Starting...");
                    Exception backupDir = CreateBackupDir();
                    if (backupDir == null)
                    {
                        Log.Print("BackupAddons :: Backup directory created");
                        DeleteOldFiles();
                        if (trayMode)
                        {
                            MainForm.Instance.ShowNotifyIconMessage("Performing backup operation", "Please don't close AxTools until the operation is completed", ToolTipIcon.Info);
                        }
                        Exception zip = Zip();
                        if (zip == null)
                        {
                            Log.Print("BackupAddons :: Backup successfully created");
                            ReportToUser("Backup successful", "Backup file was stored in " + Settings.AddonsBackupPath, false, trayMode);
                        }
                        else
                        {
                            Log.Print("BackupAddons :: Backup error: Zipping failed: " + zip.Message, true);
                            ReportToUser("Backup error", "Zipping failed\r\n" + zip.Message, true, trayMode);
                        }
                    }
                    else
                    {
                        Log.Print("BackupAddons :: Can't create backup directory: " + backupDir.Message, true);
                        ReportToUser("Backup error", "Can't create backup dir:\r\n" + backupDir.Message, true, trayMode);
                    }
                    break;
                case CheckWTFDirectoryResult.WTFDirIsNotFound:
                    Log.Print("Backup error: WTF directory isn't found");
                    ReportToUser("Backup error", "\"WTF\" folder isn't found", true, trayMode);
                    break;
                case CheckWTFDirectoryResult.WTFDirIsTooLarge:
                    Log.Print("Backup error: WTF directory is too large", true);
                    ReportToUser("Backup error", "WTF directory is too large (>1GB)", true, trayMode);
                    break;
            }
            _isBackingUp = false;
        }

        private static CheckWTFDirectoryResult CheckWTFDirectory()
        {
            if (!Directory.Exists(Settings.WowExe + "\\WTF"))
            {
                return CheckWTFDirectoryResult.WTFDirIsNotFound;
            }
            if (Utils.CalcDirectorySize(Settings.WowExe + "\\WTF") > 1024 * 1024 * 1024)
            {
                return CheckWTFDirectoryResult.WTFDirIsTooLarge;
            }
            return CheckWTFDirectoryResult.OK;
        }

        private static Exception CreateBackupDir()
        {
            DirectoryInfo backupDirectory = new DirectoryInfo(Settings.AddonsBackupPath);
            try
            {
                if (!backupDirectory.Exists)
                {
                    backupDirectory.Create();
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private static void DeleteOldFiles()
        {
            DirectoryInfo backupDirectory = new DirectoryInfo(Settings.AddonsBackupPath);
            List<FileInfo> backupFiles = backupDirectory.GetFileSystemInfos().Where(i => i.Name.Contains("AddonsBackup_") && i is FileInfo).Cast<FileInfo>().ToList();
            Log.Print("BackupAddons :: Total backup files: " + backupFiles.Count);
            if (backupFiles.Count >= Settings.AddonsBackupNum)
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
                for (int i = 0; i < backupFiles.Count - Settings.AddonsBackupNum + 1; i++)
                {
                    try
                    {
                        backupFiles[i].Delete();
                        Log.Print("BackupAddons :: Old backup file is deleted: " + backupFiles[i].Name);
                    }
                    catch (Exception ex)
                    {
                        Log.Print("BackupAddons :: Can't delete old file: " + ex.Message, true);
                    }
                }
            }
        }

        private static Exception Zip()
        {
            string zipPath = String.Format("{0}\\AddonsBackup_{1:yyyyMMdd_HHmmss}.zip", Settings.AddonsBackupPath, DateTime.UtcNow);
            Log.Print("BackupAddons :: Zipping to file: " + zipPath);
            try
            {
                using (ZipFile zip = new ZipFile(zipPath, Encoding.UTF8))
                {
                    zip.CompressionLevel = (CompressionLevel)Settings.BackupCompressionLevel;
                    zip.AddDirectory(Settings.WowExe + "\\WTF", "\\WTF");
                    zip.AddDirectory(Settings.WowExe + "\\Interface", "\\Interface");
                    zip.SaveProgress += AddonsBackup_SaveProgress;
                    zip.Save();
                    zip.SaveProgress -= AddonsBackup_SaveProgress;
                }
                GC.Collect();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private static void AddonsBackup_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (e.EntriesSaved != 0 && e.EntriesTotal != 0 && e.EntriesTotal >= e.EntriesSaved)
            {
                int procent = 100*e.EntriesSaved/e.EntriesTotal;
                if (procent != _prevProcent)
                {
                    _prevProcent = procent;
                    MainForm.Instance.AddonsBackup_OnChangedState(procent);
                }
            }
        }

        private static void ReportToUser(string title, string message,bool isError, bool tray)
        {
            if (tray)
            {
                MainForm.Instance.ShowNotifyIconMessage(title, message, isError ? ToolTipIcon.Error : ToolTipIcon.Info);
            }
            else
            {
                MainForm.Instance.ShowTaskDialog(title, message, TaskDialogButton.OK, isError ? TaskDialogIcon.Stop : TaskDialogIcon.Information);
            }
        }

        private enum CheckWTFDirectoryResult
        {
            OK,
            WTFDirIsNotFound,
            WTFDirIsTooLarge,
        }
    }
}