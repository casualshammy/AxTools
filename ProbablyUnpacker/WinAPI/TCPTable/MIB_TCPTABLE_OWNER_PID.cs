using System.Runtime.InteropServices;

namespace AxTools.WinAPI.TCPTable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPTABLE_OWNER_PID
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        public uint dwNumEntries;

        // ReSharper disable once MemberCanBePrivate.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        public MIB_TCPROW_OWNER_PID table;
    }
}