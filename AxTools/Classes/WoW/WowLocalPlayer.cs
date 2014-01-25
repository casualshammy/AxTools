using System;

namespace AxTools.Classes.WoW
{
    internal static class LocalPlayer
    {
        internal static ulong GUID;
        internal static WowPoint Location;
        internal static IntPtr Address;
        internal static uint Health;
        internal static uint HealthMax;
        internal static uint CastingSpellID;
        internal static uint ChannelSpellID;
        internal static float Rotation;
        internal static uint Level;
        internal static ulong TargetGUID;
        internal static bool IsAlliance;
    }
}
