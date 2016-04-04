using System.Runtime.InteropServices;
using AxTools.WoW.Internals.ObjectManager;

namespace AxTools.WoW.Management.ObjectManager
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct WoWObjectsInfo
    {
        [FieldOffset(WowBuildInfoX64.ObjectGUID)]
        internal readonly WoWGUID GUID;

        [FieldOffset(WowBuildInfoX64.GameObjectIsBobbing)]
        internal readonly byte Bobbing;

        [FieldOffset(WowBuildInfoX64.GameObjectLocation)]
        internal readonly WowPoint Location;
    }
}