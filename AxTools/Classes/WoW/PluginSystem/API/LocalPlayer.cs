namespace AxTools.Classes.WoW.PluginSystem.API
{
    public class LocalPlayer
    {
        public ulong GUID;
        public WowPoint Location;
        public uint Health;
        public uint HealthMax;
        public uint CastingSpellID;
        public uint ChannelSpellID;
        public float Rotation;
        public uint Level;
        public ulong TargetGUID;
        public bool IsAlliance;
    }
}