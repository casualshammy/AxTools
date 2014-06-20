using System.Collections.Generic;
using AxTools.Classes.WoW.Management;
using AxTools.Classes.WoW.Management.ObjectManager;

namespace AxTools.Classes.WoW.PluginSystem.API
{
    public static class ObjMgr
    {

        public static WoWPlayerMe Pulse(List<WowObject> wowObjects, List<WowPlayer> wowUnits, List<WowNpc> wowNpcs)
        {
            return ObjectMgr.Pulse(wowObjects, wowUnits, wowNpcs);
        }

        public static WoWPlayerMe Pulse(List<WowObject> wowObjects, List<WowNpc> wowNpcs)
        {
            return ObjectMgr.Pulse(wowObjects, wowNpcs);
        }

        public static WoWPlayerMe Pulse(List<WowObject> wowObjects)
        {
            return ObjectMgr.Pulse(wowObjects);
        }

        public static WoWPlayerMe Pulse(List<WowPlayer> wowPlayers)
        {
            return ObjectMgr.Pulse(wowPlayers);
        }

        public static WoWPlayerMe Pulse(List<WowNpc> wowNpcs)
        {
            return ObjectMgr.Pulse(wowNpcs);
        }

        public static WoWPlayerMe Pulse()
        {
            return ObjectMgr.Pulse();
        }
    
    }
}
