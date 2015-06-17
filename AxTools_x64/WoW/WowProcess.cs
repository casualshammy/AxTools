using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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

        private static readonly List<WowProcess> SharedList = new List<WowProcess>();
        private static readonly object SharedLock = new object();

        /// <summary>
        ///     List of all <see cref="WowProcess"/> running on computer
        /// </summary>
        internal static List<WowProcess> List
        {
            get
            {
                lock (SharedLock)
                {
                    return SharedList;
                }
            }
        }

        internal int ProcessID;
        internal MemoryManager Memory;

        private readonly WoWAntiKick woWAntiKick;

        private string mProcessName;

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
        
        private readonly Process mProcess;
        private Process Process
        {
            get
            {
                mProcess.Refresh();
                return mProcess;
            }
        }

        private readonly object isValidBuildLocker = new object();
        private int isValidBuild;
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
                                            Log.Info(String.Format("{0}:{1} :: [WoW hook] Reference hash: {2}", Process.ProcessName, ProcessID, BitConverter.ToString(WowBuildInfoX64.WoWHash)));
                                            Log.Info(String.Format("{0}:{1} :: [WoW hook] Actual hash:    {2}", Process.ProcessName, ProcessID, BitConverter.ToString(hash)));
                                            Log.Info(String.Format("{0}:{1} :: [WoW hook] Hash is computed, took {2}ms", Process.ProcessName, ProcessID, stopwatch.ElapsedMilliseconds));
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(String.Format("{0}:{1} :: [WoW hook] IsValidBuild error: {2}", Process.ProcessName, ProcessID, ex.Message));
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

        internal uint PlayerZoneID
        {
            get
            {
                return Memory.Read<uint>(Memory.ImageBase + WowBuildInfoX64.PlayerZoneID);
            }
        }

        internal string PlayerRealm
        {
            get
            {
                var textRealmName = Encoding.UTF8.GetString(Memory.ReadBytes(Memory.ImageBase + WowBuildInfoX64.PlayerRealm, 32));
                if (textRealmName.Contains("\0"))
                {
                    textRealmName = textRealmName.Split(Convert.ToChar("\0"))[0];
                }
                return textRealmName;
            }
        }

        internal string PlayerName
        {
            get
            {
                var txtPlayerName = Encoding.UTF8.GetString(Memory.ReadBytes(Memory.ImageBase + WowBuildInfoX64.PlayerName, 24));
                if (txtPlayerName.Contains("\0"))
                {
                    txtPlayerName = txtPlayerName.Split(Convert.ToChar("\0"))[0];
                }
                return txtPlayerName;
            }
        }

        internal bool PlayerIsLooting
        {
            get
            {
                return Memory.Read<byte>(Memory.ImageBase + WowBuildInfoX64.PlayerIsLooting) != 0;
            }
        }
    
    }
}
