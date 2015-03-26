using System.Runtime.InteropServices;

namespace AxTools.WoW.Management.ObjectManager
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct WowPlayerInfo
    {
        [FieldOffset(WowBuildInfo.UnitTargetGUID)]
        internal readonly UInt128 TargetGUID;
        [FieldOffset(WowBuildInfo.UnitClass)]
        internal WowPlayerClass Class;
        [FieldOffset(WowBuildInfo.UnitHealth)]
        internal readonly uint Health;
        [FieldOffset(WowBuildInfo.UnitPower)]
        internal readonly uint Power;
        [FieldOffset(WowBuildInfo.UnitHealthMax)]
        internal readonly uint HealthMax;
        [FieldOffset(WowBuildInfo.UnitPowerMax)]
        internal readonly uint PowerMax;
        [FieldOffset(WowBuildInfo.UnitLevel)]
        internal readonly uint Level;
        [FieldOffset(WowBuildInfo.UnitRace)]
        internal readonly uint Race;
    }
}
