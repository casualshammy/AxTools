using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AxTools.Classes.WoW
{
    internal class WowProcess : IDisposable
    {
        internal WowProcess(int processID)
        {
            MaxTime = Utils.Rnd.Next(150000, 290000);
            ProcessID = processID;
            mProcess = Process.GetProcessById(processID);
            mProcessName = Process.ProcessName;
            isValidBuild = -1;
        }

        public void Dispose()
        {
            if (Memory != null) Memory.Dispose();
            Process.Dispose();
        }

        private static readonly List<WowProcess> SharedList = new List<WowProcess>();
        private static readonly object SharedLock = new object();
        internal static List<WowProcess> Shared
        {
            get
            {
                lock (SharedLock)
                {
                    return SharedList;
                }
            }
        }

        internal int MaxTime;
        internal int ProcessID;
        internal GreyMagic.ExternalProcessReader Memory;

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
        
        internal string MainWindowTitle
        {
            get { return Process.MainWindowTitle; }
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

        private int isValidBuild;
        internal bool IsValidBuild
        {
            get
            {
                if (Memory == null) return false;
                if (isValidBuild == -1)
                {
                    try
                    {
                        uint variable = Memory.Read<uint>(Memory.ImageBase + WowBuildInfo.WowBuildAddress);
                        isValidBuild = WowBuildInfo.WowBuild == variable ? 1 : 0;
                    }
                    catch
                    {
                        isValidBuild = 0;
                    }
                }
                return isValidBuild == 1;
            }
        }

        internal bool IsInGame
        {
            get
            {
                try
                {
                    if (Memory == null) return false;
                    return Memory.Read<byte>(Memory.ImageBase + WowBuildInfo.IsInGame) == 1;
                }
                catch
                {
                    return false;
                }
            }
        }

        internal uint IsBattlegroundFinished
        {
            get
            {
                return Memory.Read<uint>(Memory.ImageBase + WowBuildInfo.IsBattlegroundFinished);
            }
        }

        internal uint PlayerZoneID
        {
            get
            {
                return Memory.Read<uint>(Memory.ImageBase + WowBuildInfo.PlayerZoneID);
            }
        }

        internal string PlayerRealm
        {
            get
            {
                var textRealmName = Encoding.UTF8.GetString(Memory.ReadBytes(Memory.ImageBase + WowBuildInfo.PlayerRealm, 32));
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
                var txtPlayerName = Encoding.UTF8.GetString(Memory.ReadBytes(Memory.ImageBase + WowBuildInfo.PlayerName, 24));
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
                return Memory.Read<byte>(Memory.ImageBase + WowBuildInfo.PlayerIsLooting) != 0;
            }
        }
    }
}
