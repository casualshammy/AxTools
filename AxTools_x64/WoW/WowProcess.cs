using AxTools.Helpers;
using AxTools.WinAPI;
using FMemory;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace AxTools.WoW
{
    public class WowProcess : IDisposable
    {
        private static readonly Log2 log = new Log2(nameof(WowProcess));

        internal WowProcess(int processID)
        {
            ProcessID = processID;
            mProcess = Process.GetProcessById(processID);
            mProcessName = Process.ProcessName;
            isValidBuild = -1;
        }

        public void Dispose()
        {
            Memory?.Dispose();
            Process.Dispose();
        }

        internal int ProcessID;
        internal MemoryManager Memory;
        private string mProcessName;
        private readonly Process mProcess;
        private readonly object isValidBuildLocker = new object();
        private int isValidBuild;

        internal string ProcessName
        {
            get
            {
                if (string.IsNullOrEmpty(mProcessName))
                {
                    mProcessName = Process.ProcessName;
                }
                return mProcessName;
            }
        }

        internal IntPtr MainWindowHandle => Process.MainWindowHandle;

        private Process Process
        {
            get
            {
                mProcess.Refresh();
                return mProcess;
            }
        }

        internal bool IsMinimized
        {
            get
            {
                var style = NativeMethods.GetWindowLong64(MainWindowHandle, Win32Consts.GWL_STYLE);
                return (style & Win32Consts.WS_MINIMIZE) != 0;
            }
        }

        internal bool IsValidBuild
        {
            get
            {
                if (isValidBuild == -1)
                {
                    lock (isValidBuildLocker)
                    {
                        if (isValidBuild == -1)
                        {
                            try
                            {
                                var stopwatch = Stopwatch.StartNew();
                                using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
                                {
                                    byte[] hash;
                                    using (FileStream fileStream = File.Open(Process.Modules[0].FileName, FileMode.Open, FileAccess.Read))
                                        hash = provider.ComputeHash(fileStream);
                                    isValidBuild = hash.SequenceEqual(WowBuildInfoX64.WoWHash) || WowBuildInfoX64.TryFindNewOffsets(Memory, hash) ? 1 : 0;
                                    log.Info($"{ToString()} Reference hash: {BitConverter.ToString(WowBuildInfoX64.WoWHash)}");
                                    log.Info($"{ToString()} Actual hash:    {BitConverter.ToString(hash)}");
                                    log.Info($"{ToString()} Hash is computed, took {stopwatch.ElapsedMilliseconds}ms");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error($"{ToString()} IsValidBuild error: {ex.Message}");
                                isValidBuild = 0;
                            }
                        }
                    }
                }
                return isValidBuild == 1;
            }
        }

        internal int GetExecutableRevision()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Process.MainModule.FileName);
            return versionInfo.FilePrivatePart;
        }

        public override string ToString()
        {
            return string.Concat("[", ProcessName, ":", ProcessID, "]");
        }

        internal void WaitWhileWoWIsMinimized()
        {
            Utils.LogIfCalledFromUIThread();
            if (IsMinimized)
            {
                Notify.TrayPopup("Attention!", "AxTools is stuck because it can't interact with minimized WoW client. Click to activate WoW window", NotifyUserType.Warn, true, null, 10,
                    (sender, args) => NativeMethods.ShowWindow(MainWindowHandle, 9));
                while (IsMinimized)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}