using AxTools.Classes.WoW.Management;
using AxTools.Classes.WoW.Management.ObjectManager;

namespace AxTools.Classes.WoW.PluginSystem.API
{
    public static class Functions
    {
        public static void Interact(this WowObject wowObject)
        {
            WoWDXInject.Interact(wowObject.GUID);
        }

        public static void Interact(this WowNpc wowNpc)
        {
            WoWDXInject.Interact(wowNpc.GUID);
        }

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
