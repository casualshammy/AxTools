using AxTools.Classes;
using AxTools.Helpers;
using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        /// <summary>
        ///     Parameter: -1 means backup is started; 101 means backup is finished; another value means backup progress in %
        /// </summary>
        internal static event Action<int> StateChanged;

        internal static void StartService()
        {
            if (!_serviceIsStarted)
            {
                _timer = new Timer(10000);
                _timer.Elapsed += Timer_OnElapsed;
                _timer.Start();
                _serviceIsStarted = true;
            }
            else
            {
                throw new Exception("AddonBackup service is already started!");
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
            else
            {
                throw new Exception("AddonBackup service isn't running!");
            }
        }

        internal static void MakeBackup()
        {
            Start();
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
                Task.Factory.StartNew(Start)
                    .ContinueWith(l =>
                    {
                        Process.GetCurrentProcess().PriorityClass = defaultProcessPriorityClass;
                        Log.Info("[BackupAddons] Process priority is changed to " + Process.GetCurrentProcess().PriorityClass);
                    });
            }
        }

        private static void Start()
        {
            _isBackingUp = true;
            CheckWTFDirectoryResult checkWTFDirectoryResult = CheckWTFDirectory();
            switch (checkWTFDirectoryResult)
            {
                case CheckWTFDirectoryResult.OK:
                    Log.Info("[BackupAddons] Starting...");
                    Exception backupDir = CreateBackupDir();
                    if (backupDir == null)
                    {
                        Log.Info("[BackupAddons] Backup directory created");
                        DeleteOldFiles();
                        try
                        {
                            Zip();
                            Log.Info("[BackupAddons] Backup is successfully created");
                        }
                        catch (Exception ex)
                        {
                            Log.Error("[BackupAddons] Backup error: Zipping failed: " + ex.Message);
                            Utils.NotifyUser("Backup error", ex.Message, NotifyUserType.Error, true);
                        }
                    }
                    else
                    {
                        Log.Error("[BackupAddons] Can't create backup directory: " + backupDir.Message);
                        Utils.NotifyUser("Backup error", "Can't create backup dir:\r\n" + backupDir.Message, NotifyUserType.Error, true);
                    }
                    break;
                case CheckWTFDirectoryResult.WTFDirIsNotFound:
                    Log.Info("Backup error: WTF directory isn't found");
                    Utils.NotifyUser("Backup error", "\"WTF\" folder isn't found", NotifyUserType.Error, true);
                    break;
                case CheckWTFDirectoryResult.WTFDirIsTooLarge:
                    Log.Info("Backup error: WTF directory is too large");
                    Utils.NotifyUser("Backup error", "WTF directory is too large (> 1GB)", NotifyUserType.Error, true);
                    break;
            }
            _isBackingUp = false;
        }

        private static CheckWTFDirectoryResult CheckWTFDirectory()
        {
            if (!Directory.Exists(_settings.WoWDirectory + "\\WTF"))
            {
                return CheckWTFDirectoryResult.WTFDirIsNotFound;
            }
            if (Utils.CalcDirectorySize(_settings.WoWDirectory + "\\WTF") > 1024 * 1024 * 1024)
            {
                return CheckWTFDirectoryResult.WTFDirIsTooLarge;
            }
            return CheckWTFDirectoryResult.OK;
        }

        private static Exception CreateBackupDir()
        {
            DirectoryInfo backupDirectory = new DirectoryInfo(_settings.WoWAddonsBackupPath);
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
            string zipPath = String.Format("{0}\\AddonsBackup_{1:yyyyMMdd_HHmmss}.zip", _settings.WoWAddonsBackupPath, DateTime.UtcNow);
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

        private static void AddonsBackup_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (e.EntriesTotal != 0)
            {
                int procent = e.EntriesSaved * 100 / e.EntriesTotal;
                if (procent != _prevProcent && procent >= 0 && procent <= 100)
                {
                    _prevProcent = procent;
                    if (StateChanged != null)
                    {
                        StateChanged(procent);
                    }
                }
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
