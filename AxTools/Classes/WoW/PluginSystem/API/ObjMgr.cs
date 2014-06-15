using System.Collections.Generic;
using AxTools.Classes.WoW.Management;
using AxTools.Classes.WoW.Management.ObjectManager;

namespace AxTools.Classes.WoW.PluginSystem.API
{
    public static class ObjMgr
    {

        public static void Pulse(List<WowObject> wowObjects, List<WowPlayer> wowUnits, List<WowNpc> wowNpcs)
        {
            ObjectMgr.Pulse(wowObjects, wowUnits, wowNpcs);
        }

        public static void Pulse(List<WowObject> wowObjects, List<WowNpc> wowNpcs)
        {
            ObjectMgr.Pulse(wowObjects, wowNpcs);
        }

        public static void Pulse(List<WowObject> wowObjects)
        {
            ObjectMgr.Pulse(wowObjects);
        }

        public static void Pulse(List<WowPlayer> wowPlayers)
        {
            ObjectMgr.Pulse(wowPlayers);
        }

        public static void Pulse()
        {
            ObjectMgr.Pulse();
        }

        /// <summary>
        /// You should call Pulse (any overload) to refresh local player info
        /// </summary>
        public static WoWPlayerMe Me
        {
            get
            {
                return ObjectMgr.LocalPlayer;
            }
        }
    
    }
}
