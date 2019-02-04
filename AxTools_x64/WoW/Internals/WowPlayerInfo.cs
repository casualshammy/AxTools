using System.Runtime.InteropServices;

namespace AxTools.WoW.Internals
{
    [StructLayout(LayoutKind.Explicit, Size = WowBuildInfoX64.UnitInfoSize)]
    internal struct WowPlayerInfo
    {
        [FieldOffset(WowBuildInfoX64.UnitTargetGUID - WowBuildInfoX64.UnitInfoStart)]
        internal readonly WoWGUID TargetGUID;

        [FieldOffset(WowBuildInfoX64.UnitClass - WowBuildInfoX64.UnitInfoStart)]
        internal WowPlayerClass Class;

        [FieldOffset(WowBuildInfoX64.UnitHealth - WowBuildInfoX64.UnitInfoStart)]
        internal readonly uint Health;

        [FieldOffset(WowBuildInfoX64.UnitPower - WowBuildInfoX64.UnitInfoStart)]
        internal readonly uint Power;

        [FieldOffset(WowBuildInfoX64.UnitHealthMax - WowBuildInfoX64.UnitInfoStart)]
        internal readonly uint HealthMax;

        [FieldOffset(WowBuildInfoX64.UnitPowerMax - WowBuildInfoX64.UnitInfoStart)]
        internal readonly uint PowerMax;

        [FieldOffset(WowBuildInfoX64.UnitLevel - WowBuildInfoX64.UnitInfoStart)]
        internal readonly uint Level;

        [FieldOffset(WowBuildInfoX64.UnitRace - WowBuildInfoX64.UnitInfoStart)]
        internal readonly Race Race;

        [FieldOffset(WowBuildInfoX64.UnitFlags - WowBuildInfoX64.UnitInfoStart)]
        internal readonly uint UnitFlags;

        [FieldOffset(WowBuildInfoX64.UnitMountDisplayID - WowBuildInfoX64.UnitInfoStart)]
        internal readonly int MountDisplayID;
    }
}