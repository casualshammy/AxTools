using System.Runtime.InteropServices;

namespace AxTools.WoW.Management.ObjectManager
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct WoWObjectsInfo
    {
        [FieldOffset(WowBuildInfo.ObjectGUID)]
        internal readonly UInt128 GUID;

        [FieldOffset(WowBuildInfo.GameObjectIsBobbing)]
        internal readonly byte Bobbing;

        [FieldOffset(WowBuildInfo.GameObjectLocation)]
        internal readonly WowPoint Location;
    }
}