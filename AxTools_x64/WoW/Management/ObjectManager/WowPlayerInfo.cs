using System.Runtime.InteropServices;

namespace AxTools.WoW.Management.ObjectManager
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct WowPlayerInfo
    {
        [FieldOffset(WowBuildInfoX64.UnitTargetGUID)]
        internal readonly UInt128 TargetGUID;

        [FieldOffset(WowBuildInfoX64.UnitClass)]
        internal WowPlayerClass Class;

        [FieldOffset(WowBuildInfoX64.UnitHealth)]
        internal readonly uint Health;

        [FieldOffset(WowBuildInfoX64.UnitPower)]
        internal readonly uint Power;

        [FieldOffset(WowBuildInfoX64.UnitHealthMax)]
        internal readonly uint HealthMax;

        [FieldOffset(WowBuildInfoX64.UnitPowerMax)]
        internal readonly uint PowerMax;

        [FieldOffset(WowBuildInfoX64.UnitLevel)]
        internal readonly uint Level;

        [FieldOffset(WowBuildInfoX64.UnitRace)]
        internal readonly uint Race;

        [FieldOffset(WowBuildInfoX64.UnitFlags)]
        internal readonly uint UnitFlags;
    }
}
