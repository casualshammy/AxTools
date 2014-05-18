namespace AxTools.Classes.WoW.PluginSystem.API
{
    public class LocalPlayer
    {
        internal ulong GUID;
        public WowPoint Location;
        public uint Health;
        internal uint HealthMax;
        internal uint CastingSpellID;
        internal uint ChannelSpellID;
        internal float Rotation;
        internal uint Level;
        public ulong TargetGUID;
        internal bool IsAlliance;
    }
}