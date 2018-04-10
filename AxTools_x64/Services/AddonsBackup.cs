using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WoW;
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
        private static readonly Log2 log = new Log2("AddonsBackup");
        private static readonly Settings2 _settings = Settings2.Instance;
        private static int _prevProcent = -1;
        private static bool _isBackingUp;
        private static Timer _timer;
        private static bool _serviceIsStarted;
        private static readonly string[] FoldersToArchive = {"WTF", "Interface"};
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
            IsRunningChanged?.Invoke(true);
            try
            {
                if (Directory.Exists(_settings.WoWDirectory))
                {
                    if (File.Exists(path))
                    {
                        log.Info("Deploying archive: " + path);
                        Unzip(path);
                        log.Info("Archive is deployed: " + path);
                    }
                    else
                    {
                        log.Info("Archive " + path + " isn't found");
                        Notify.TrayPopup("Backup error", "Archive " + path + " isn't found", NotifyUserType.Error, true);
                    }
                }
                else
                {
                    log.Info("WoW dir (" + _settings.WoWDirectory + ") isn't found");
                    Notify.TrayPopup("Backup error", "WoW dir (" + _settings.WoWDirectory + ") isn't found", NotifyUserType.Error, true);
                }
            }
            catch (Exception ex)
            {
                log.Error("Deploying error: " + ex.Message);
                Notify.TrayPopup("Deploying error", ex.Message, NotifyUserType.Error, true);
            }
            IsRunningChanged?.Invoke(false);
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
                if (_settings.WoWAddonsBackup_DoNotCreateBackupWhileWoWClientIsRunning && WoWProcessManager.Processes.Any())
                {
                    _timer.Stop();
                    WoWProcessManager.WoWProcessStartedOrClosed += WoWProcessManager_WoWProcessStartedOrClosed;
                    log.Info("Time to create backup, but WoW client is running. Let's wait...");
                }
                else
                {
                    StartBackupOnSchedule();
                }
            }
        }

        private static void WoWProcessManager_WoWProcessStartedOrClosed()
        {
            if (WoWProcessManager.Processes.Count == 0)
            {
                WoWProcessManager.WoWProcessStartedOrClosed -= WoWProcessManager_WoWProcessStartedOrClosed;
                StartBackupOnSchedule();
                _timer.Start();
                log.Info("WoW client is closed. Backup creation started.");
            }
        }

        private static void StartBackupOnSchedule()
        {
            _settings.WoWAddonsBackupLastDate = DateTime.UtcNow;
            Task.Factory.StartNew(MakeNewArchive);
        }

        private static void MakeNewArchive()
        {
            Guid _lock = Program.ShutdownLock.GetLock();
            Guid _wowLock = MainForm.Instance.WoWLaunchLock.GetLock();
            try
            {
                _isBackingUp = true;
                IsRunningChanged?.Invoke(true);
                if (Directory.Exists(_settings.WoWDirectory + "\\WTF"))
                {
                    if (Utils.CalcDirectorySize(_settings.WoWDirectory + "\\WTF") <= 1024 * 1024 * 1024)
                    {
                        try
                        {
                            log.Info("WTF directory OK");
                            CreateBackupDir();
                            log.Info("Backup directory OK");
                            DeleteOldFiles();
                            log.Info("Old archives are deleted");
                            Zip();
                            log.Info("Backup is successfully created");
                        }
                        catch (Exception ex)
                        {
                            log.Error("Backup error: " + ex.Message);
                            Notify.TrayPopup("Backup error", ex.Message, NotifyUserType.Error, true);
                        }
                        finally
                        {
                            DeleteBrokenFiles();
                        }
                    }
                    else
                    {
                        log.Info("WTF directory is too large");
                        Notify.TrayPopup("Backup error", "WTF directory is too large (> 1GB)", NotifyUserType.Error, true);
                    }
                }
                else
                {
                    log.Info("WTF directory isn't found");
                    Notify.TrayPopup("Backup error", "\"WTF\" folder isn't found", NotifyUserType.Error, true);
                }
                IsRunningChanged?.Invoke(false);
                _isBackingUp = false;
            }
            finally
            {
                Program.ShutdownLock.ReleaseLock(_lock);
                MainForm.Instance.WoWLaunchLock.ReleaseLock(_wowLock);
            }
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
            log.Info("Total backup files: " + backupFiles.Count);
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
                        log.Info("Old backup file is deleted: " + backupFiles[i].Name);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Can't delete old file: " + ex.Message);
                    }
                }
            }
        }

        private static void DeleteBrokenFiles()
        {
            DirectoryInfo backupDirectory = new DirectoryInfo(_settings.WoWAddonsBackupPath);
            foreach (var file in backupDirectory.GetFileSystemInfos().Where(i => i.Name.StartsWith("DotNetZip-") && i is FileInfo).Cast<FileInfo>())
            {
                try
                {
                    file.Delete();
                    log.Info($"Broken file {file.FullName} is deleted");
                }
                catch (Exception ex)
                {
                    log.Error($"Can't delete file {file.FullName}: {ex.Message}");
                }
            }
        }

        private static void Zip()
        {
            string zipPath = string.Format("{0}\\AddonsBackup_{1:yyyyMMdd_HHmmss}.zip", _settings.WoWAddonsBackupPath, DateTime.UtcNow);
            log.Info("Zipping to file: " + zipPath);
            using (ZipFile zip = new ZipFile(zipPath, Encoding.UTF8))
            {
                zip.CompressionLevel = (CompressionLevel)_settings.WoWAddonsBackupCompressionLevel;
                foreach (string dirName in FoldersToArchive)
                {
                    zip.AddDirectory(_settings.WoWDirectory + "\\" + dirName, "\\" + dirName);
                }
                zip.SaveProgress += AddonsBackup_SaveProgress;
                var processPriority = Process.GetCurrentProcess().PriorityClass;
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
                zip.Save();
                Process.GetCurrentProcess().PriorityClass = processPriority;
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
                    ProgressPercentageChanged?.Invoke(procent);
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
                    ProgressPercentageChanged?.Invoke(procent);
                }
            }
        }
    
    }
}
