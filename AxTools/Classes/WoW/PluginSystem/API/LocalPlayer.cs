namespace AxTools.Classes.WoW.PluginSystem.API
{
    public class LocalPlayer
    {
        internal ulong GUID;
        internal WowPoint Location;
        internal uint Health;
        internal uint HealthMax;
        internal uint CastingSpellID;
        internal uint ChannelSpellID;
        internal float Rotation;
        internal uint Level;
        internal ulong TargetGUID;
        internal bool IsAlliance;
    }
}