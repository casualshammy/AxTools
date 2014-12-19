using System.Collections.Generic;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;

namespace AxTools.WoW.PluginSystem.API
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

        public static WoWPlayerMe Pulse(List<WowPlayer> wowPlayers, List<WowNpc> wowNpcs)
        {
            return ObjectMgr.Pulse(wowPlayers, wowNpcs);
        }
    
    }
}
