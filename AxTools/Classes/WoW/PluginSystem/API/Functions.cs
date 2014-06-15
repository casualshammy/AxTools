using AxTools.Classes.WoW.Management;

namespace AxTools.Classes.WoW.PluginSystem.API
{
    public static class Functions
    {
        public static void Interact(ulong guid)
        {
            WoWDXInject.Interact(guid);
        }

        public static void MoveTo(WowPoint point)
        {
            WoWDXInject.MoveTo(point);
        }
    }
}
