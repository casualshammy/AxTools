using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace AxTools.WinAPI.TCPTable
{
    internal class TCPConnectionInfo
    {
        internal IPEndPoint EndPoint;
        internal int ProcessID;

        internal TCPConnectionInfo(int processID, IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            ProcessID = processID;
        }

        internal static List<TCPConnectionInfo> GetAllRemoteTcpConnections()
        {
            List<TCPConnectionInfo> list = new List<TCPConnectionInfo>();
            // ReSharper disable once InconsistentNaming
            var AF_INET = 2;    // IP_v4
            var buffSize = 0;
            var ret = NativeMethods.GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
            if (ret != 0 && ret != 122) throw new Exception("bad ret on check " + ret); // 122 insufficient buffer size
            var buffTable = Marshal.AllocHGlobal(buffSize);
            try
            {
                ret = NativeMethods.GetExtendedTcpTable(buffTable, ref buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                if (ret != 0) throw new Exception("bad ret " + ret);
                MIB_TCPTABLE_OWNER_PID tab = (MIB_TCPTABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, typeof(MIB_TCPTABLE_OWNER_PID));
                var rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));
                for (int i = 0; i < tab.dwNumEntries; i++)
                {
                    MIB_TCPROW_OWNER_PID tcpRow = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, typeof(MIB_TCPROW_OWNER_PID));
                    list.Add(new TCPConnectionInfo(tcpRow.owningPid, new IPEndPoint(tcpRow.remoteAddr, tcpRow.RemotePort)));
                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(tcpRow));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffTable);
            }
            return list;
        }
    }
}