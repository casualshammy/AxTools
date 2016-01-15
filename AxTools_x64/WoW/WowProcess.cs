﻿using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using AxTools.Helpers;
using AxTools.Helpers.MemoryManagement;

namespace AxTools.WoW
{
    internal class WowProcess : IDisposable
    {
        internal WowProcess(int processID)
        {
            ProcessID = processID;
            mProcess = Process.GetProcessById(processID);
            mProcessName = Process.ProcessName;
            isValidBuild = -1;
            woWAntiKick = new WoWAntiKick(this);
        }

        public void Dispose()
        {
            woWAntiKick.Dispose();
            if (Memory != null) Memory.Dispose();
            Process.Dispose();
        }

        internal int ProcessID;
        internal MemoryManager Memory;
        private readonly WoWAntiKick woWAntiKick;
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
        
        internal IntPtr MainWindowHandle
        {
            get { return Process.MainWindowHandle; }
        }
        
        private Process Process
        {
            get
            {
                mProcess.Refresh();
                return mProcess;
            }
        }

        internal bool IsValidBuild
        {
            get
            {
                if (Memory != null)
                {
                    if (isValidBuild == -1)
                    {
                        lock (isValidBuildLocker)
                        {
                            if (isValidBuild == -1)
                            {
                                try
                                {
                                    Stopwatch stopwatch = Stopwatch.StartNew();
                                    using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
                                    {
                                        using (FileStream fileStream = File.Open(Process.Modules[0].FileName, FileMode.Open, FileAccess.Read))
                                        {
                                            byte[] hash = provider.ComputeHash(fileStream);
                                            isValidBuild = hash.SequenceEqual(WowBuildInfoX64.WoWHash) ? 1 : 0;
                                            Log.Info(string.Format("{0} [WoW hook] Reference hash: {1}", ToString(), BitConverter.ToString(WowBuildInfoX64.WoWHash)));
                                            Log.Info(string.Format("{0} [WoW hook] Actual hash:    {1}", ToString(), BitConverter.ToString(hash)));
                                            Log.Info(string.Format("{0} [WoW hook] Hash is computed, took {1}ms", ToString(), stopwatch.ElapsedMilliseconds));
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(string.Format("{0} [WoW hook] IsValidBuild error: {1}", ToString(), ex.Message));
                                    isValidBuild = 0;
                                }
                            }
                        }
                    }
                    return isValidBuild == 1;
                }
                return false;
            }
        }

        internal bool IsInGame
        {
            get
            {
                try
                {
                    if (Memory == null) return false;
                    return Memory.Read<byte>(Memory.ImageBase + WowBuildInfoX64.GameState) == 1;
                }
                catch
                {
                    return false;
                }
            }
        }

        internal bool IsNotLoadingScreen
        {
            get
            {
                try
                {
                    if (Memory == null) return false;
                    return Memory.Read<byte>(Memory.ImageBase + WowBuildInfoX64.Possible_NotLoadingScreen) == 1;
                }
                catch
                {
                    return false;
                }
            }
        }

        public override string ToString()
        {
            return string.Concat("[", ProcessName, ":", ProcessID, "]");
        }

    }
}
