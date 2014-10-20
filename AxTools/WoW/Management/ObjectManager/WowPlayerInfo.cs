using System.Runtime.InteropServices;

namespace AxTools.WoW.Management.ObjectManager
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct WowPlayerInfo
    {
        [FieldOffset(0xA0)]
        internal readonly UInt128 TargetGUID;
        [FieldOffset(0xE1)]
        internal WowPlayerClass Class;
        [FieldOffset(0xEC)]
        internal readonly uint Health;
        [FieldOffset(0xF0)]
        internal readonly uint Power;
        [FieldOffset(0x108)]
        internal readonly uint HealthMax;
        [FieldOffset(0x10C)]
        internal readonly uint PowerMax;
        [FieldOffset(0x154)]
        internal readonly uint Level;
        [FieldOffset(0x15C)]
        internal readonly uint Race;
    }
}
