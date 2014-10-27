using System.Runtime.InteropServices;

namespace AxTools.WoW.Management.ObjectManager
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct WoWObjectsInfo
    {
        [FieldOffset(WowBuildInfo.ObjectGUID)]
        internal readonly UInt128 GUID;

        [FieldOffset(0x104)]
        internal readonly byte Bobbing;

        [FieldOffset(0x138)]
        internal readonly WowPoint Location;
    }
}