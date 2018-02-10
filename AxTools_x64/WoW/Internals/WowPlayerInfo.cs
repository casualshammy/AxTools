using System.Runtime.InteropServices;

namespace AxTools.WoW.Internals
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct WowPlayerInfo
    {
        [FieldOffset(WowBuildInfoX64.UnitTargetGUID)]
        internal readonly WoWGUID TargetGUID;

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
        internal readonly Race Race;

        [FieldOffset(WowBuildInfoX64.UnitFlags)]
        internal readonly uint UnitFlags;

        [FieldOffset(WowBuildInfoX64.UnitMountDisplayID)]
        internal readonly int MountDisplayID;
    }
}
