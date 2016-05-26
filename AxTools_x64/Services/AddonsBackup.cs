using AxTools.Helpers;
using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AxTools.Services
{
    internal static class AddonsBackup
    {
        private static readonly Settings _settings = Settings.Instance;
        private static int _prevProcent = -1;
        private static bool _isBackingUp;
        private static Timer _timer;
        private static bool _serviceIsStarted;
        internal static event Action<bool> IsRunningChanged;
        internal static event Action<int> ProgressPercentageChanged;

        internal static void StartService()
        {
            if (!_serviceIsStarted)
            {
                _timer = new Timer(10000);
                _timer.Elapsed += Timer_OnElapsed;
                _timer.Start();
                _serviceIsStarted = true;
            }
        }

        internal static void StopService()
        {
            if (_serviceIsStarted)
            {
                _timer.Elapsed -= Timer_OnElapsed;
                _timer.Close();
                _serviceIsStarted = false;
            }
        }

        internal static void ManualBackup()
        {
            if (_isBackingUp)
            {
                throw new Exception("Backup process is already started");
            }
            MakeNewArchive();
        }

        internal static void DeployArchive(string path)
        {
            _isBackingUp = true;
            if (IsRunningChanged != null)
            {
                IsRunningChanged(true);
            }
            try
            {
                if (Directory.Exists(_settings.WoWDirectory))
                {
                    if (File.Exists(path))
                    {
                        Log.Info("[BackupAddons] Deploying archive: " + path);
                        Unzip(path);
                        Log.Info("[BackupAddons] Archive is deployed: " + path);
                    }
                    else
                    {
                        Log.Info("[BackupAddons] Archive " + path + " isn't found");
                        Notify.Balloon("Backup error", "Archive " + path + " isn't found", NotifyUserType.Error, true);
                    }
                }
                else
                {
                    Log.Info("[BackupAddons] WoW dir (" + _settings.WoWDirectory + ") isn't found");
                    Notify.Balloon("Backup error", "WoW dir (" + _settings.WoWDirectory + ") isn't found", NotifyUserType.Error, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error("[BackupAddons] Deploying error: " + ex.Message);
                Notify.Balloon("Deploying error", ex.Message, NotifyUserType.Error, true);
            }
            if (IsRunningChanged != null)
            {
                IsRunningChanged(false);
            }
            _isBackingUp = false;
        }

        internal static string[] GetArchives()
        {
            DirectoryInfo backupDirectory = new DirectoryInfo(_settings.WoWAddonsBackupPath);
            return backupDirectory.GetFileSystemInfos().Where(l => l is FileInfo && Regex.IsMatch(l.Name, "AddonsBackup_\\d+_\\d+\\.zip")).Select(l => l.FullName).ToArray();
        }

        private static void Timer_OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            TimeSpan pTimeSpan = DateTime.UtcNow - _settings.WoWAddonsBackupLastDate;
            if (pTimeSpan.TotalHours >= _settings.WoWAddonsBackupMinimumTimeBetweenBackup && !_isBackingUp)
            {
                _settings.WoWAddonsBackupLastDate = DateTime.UtcNow;
                ProcessPriorityClass defaultProcessPriorityClass = Process.GetCurrentProcess().PriorityClass;
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
                Log.Info("[BackupAddons] Process priority is changed to " + Process.GetCurrentProcess().PriorityClass);
                Task.Factory.StartNew(MakeNewArchive)
                    .ContinueWith(l =>
                    {
                        Process.GetCurrentProcess().PriorityClass = defaultProcessPriorityClass;
                        Log.Info("[BackupAddons] Process priority is changed to " + Process.GetCurrentProcess().PriorityClass);
                    });
            }
        }

        private static void MakeNewArchive()
        {
            _isBackingUp = true;
            if (IsRunningChanged != null)
            {
                IsRunningChanged(true);
            }
            if (Directory.Exists(_settings.WoWDirectory + "\\WTF"))
            {
                if (Utils.CalcDirectorySize(_settings.WoWDirectory + "\\WTF") <= 1024*1024*1024)
                {
                    try
                    {
                        Log.Info("[BackupAddons] WTF directory OK");
                        CreateBackupDir();
                        Log.Info("[BackupAddons] Backup directory OK");
                        DeleteOldFiles();
                        Log.Info("[BackupAddons] Old archives are deleted");
                        Zip();
                        Log.Info("[BackupAddons] Backup is successfully created");
                    }
                    catch (Exception ex)
                    {
                        Log.Error("[BackupAddons] Backup error: " + ex.Message);
                        Notify.Balloon("Backup error", ex.Message, NotifyUserType.Error, true);
                    }
                }
                else
                {
                    Log.Info("[BackupAddons] WTF directory is too large");
                    Notify.Balloon("Backup error", "WTF directory is too large (> 1GB)", NotifyUserType.Error, true);
                }
            }
            else
            {
                Log.Info("[BackupAddons] WTF directory isn't found");
                Notify.Balloon("Backup error", "\"WTF\" folder isn't found", NotifyUserType.Error, true);
            }
            if (IsRunningChanged != null)
            {
                IsRunningChanged(false);
            }
            _isBackingUp = false;
        }

        private static void CreateBackupDir()
        {
            if (!Directory.Exists(_settings.WoWAddonsBackupPath))
            {
                Directory.CreateDirectory(_settings.WoWAddonsBackupPath);
            }
        }

        private static void DeleteOldFiles()
        {
            DirectoryInfo backupDirectory = new DirectoryInfo(_settings.WoWAddonsBackupPath);
            List<FileInfo> backupFiles = backupDirectory.GetFileSystemInfos().Where(i => i.Name.Contains("AddonsBackup_") && i is FileInfo).Cast<FileInfo>().ToList();
            Log.Info("[BackupAddons] Total backup files: " + backupFiles.Count);
            if (backupFiles.Count >= _settings.WoWAddonsBackupNumberOfArchives)
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
                for (int i = 0; i < backupFiles.Count - _settings.WoWAddonsBackupNumberOfArchives + 1; i++)
                {
                    try
                    {
                        backupFiles[i].Delete();
                        Log.Info("[BackupAddons] Old backup file is deleted: " + backupFiles[i].Name);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("[BackupAddons] Can't delete old file: " + ex.Message);
                    }
                }
            }
        }

        private static void Zip()
        {
            string zipPath = string.Format("{0}\\AddonsBackup_{1:yyyyMMdd_HHmmss}.zip", _settings.WoWAddonsBackupPath, DateTime.UtcNow);
            Log.Info("[BackupAddons] Zipping to file: " + zipPath);
            using (ZipFile zip = new ZipFile(zipPath, Encoding.UTF8))
            {
                zip.CompressionLevel = (CompressionLevel)_settings.WoWAddonsBackupCompressionLevel;
                zip.AddDirectory(_settings.WoWDirectory + "\\WTF", "\\WTF");
                zip.AddDirectory(_settings.WoWDirectory + "\\Interface", "\\Interface");
                zip.SaveProgress += AddonsBackup_SaveProgress;
                zip.Save();
                zip.SaveProgress -= AddonsBackup_SaveProgress;
            }
            GC.Collect();
        }

        private static void Unzip(string path)
        {
            using (ZipFile zip = new ZipFile(path, Encoding.UTF8))
            {
                zip.ExtractProgress += AddonsBackup_ExtractProgress;
                zip.ExtractAll(_settings.WoWDirectory, ExtractExistingFileAction.OverwriteSilently);
                zip.ExtractProgress -= AddonsBackup_ExtractProgress;
            }
            GC.Collect();
        }

        private static void AddonsBackup_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (e.EntriesTotal != 0)
            {
                int procent = e.EntriesSaved * 100 / e.EntriesTotal;
                if (procent != _prevProcent && procent >= 0 && procent <= 100)
                {
                    _prevProcent = procent;
                    if (ProgressPercentageChanged != null)
                    {
                        ProgressPercentageChanged(procent);
                    }
                }
            }
        }

        private static void AddonsBackup_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EntriesTotal != 0)
            {
                int procent = e.EntriesExtracted * 100 / e.EntriesTotal;
                if (procent != _prevProcent && procent >= 0 && procent <= 100)
                {
                    _prevProcent = procent;
                    if (ProgressPercentageChanged != null)
                    {
                        ProgressPercentageChanged(procent);
                    }
                }
            }
        }
    
    }
}
