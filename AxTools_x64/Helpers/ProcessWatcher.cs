using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;

namespace AxTools.Helpers
{
    internal class ProcessWatcher : IDisposable
    {
        private static readonly Timer _timer = new Timer(1000);
        private static Dictionary<string, Process[]> lib = new Dictionary<string, Process[]>();
        private string processName;

        static ProcessWatcher()
        {
            _timer.Elapsed += _timer_Elapsed;
        }

        private static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
        }

        internal ProcessWatcher(string procName)
        {
            lib.Add(procName, Process.GetProcessesByName(procName));
            if (!_timer.Enabled)
            {
                _timer.Start();
            }
        }

        public void Dispose()
        {
            
        }

        private static readonly uint processEntrySize = (UInt32) Marshal.SizeOf(typeof (PROCESSENTRY32));
        internal static List<uint> GetAllProcesses()
        {
            List<uint> list = new List<uint>();
            IntPtr handleToSnapshot = IntPtr.Zero;
            try
            {
                PROCESSENTRY32 procEntry = new PROCESSENTRY32 {dwSize = processEntrySize};
                handleToSnapshot = CreateToolhelp32Snapshot(SnapshotFlags.Process | SnapshotFlags.NoHeaps, 0);
                if (Process32First(handleToSnapshot, ref procEntry))
                {
                    do
                    {
                        list.Add(procEntry.th32ProcessID);
                    } while (Process32Next(handleToSnapshot, ref procEntry));
                }
                else
                {
                    throw new Exception(string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error()));
                }
            }
            finally
            {
                CloseHandle(handleToSnapshot);
            }
            return list;
        }

        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            internal UInt32 dwSize;
            private UInt32 cntUsage;
            internal UInt32 th32ProcessID;
            private IntPtr th32DefaultHeapID;
            private UInt32 th32ModuleID;
            private UInt32 cntThreads;
            private UInt32 th32ParentProcessID;
            private Int32 pcPriClassBase;
            private UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr CreateToolhelp32Snapshot([In]SnapshotFlags dwFlags, [In]UInt32 th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool Process32First([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool Process32Next([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle([In] IntPtr hObject);
    }
}
