using System;

namespace AxTools.Classes.WoW.Management.ObjectManager
{
    public class LocalPlayer
    {
        internal ulong GUID;
        public WowPoint Location;
        internal IntPtr Address;
        public uint Health;
        internal uint HealthMax;
        public uint CastingSpellID;
        public uint ChannelSpellID;
        internal float Rotation;
        internal uint Level;
        public ulong TargetGUID;
        internal bool IsAlliance;
    }
}
