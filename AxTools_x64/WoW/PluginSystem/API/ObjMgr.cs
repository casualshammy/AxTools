using System.Collections.Generic;
using System.Diagnostics.Contracts;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;

namespace AxTools.WoW.PluginSystem.API
{
    public static class ObjMgr
    {
        public static WoWPlayerMe Pulse(List<WowObject> wowObjects, List<WowPlayer> wowUnits, List<WowNpc> wowNpcs)
        {
            if (WoWManager.WoWProcess.IsInGame)
            {
                return ObjectMgr.Pulse(wowObjects, wowUnits, wowNpcs);
            }
            wowObjects.Clear();
            wowUnits.Clear();
            wowNpcs.Clear();
            return null;
        }

        public static WoWPlayerMe Pulse(List<WowObject> wowObjects, List<WowNpc> wowNpcs)
        {
            if (WoWManager.WoWProcess.IsInGame)
            {
                return ObjectMgr.Pulse(wowObjects, wowNpcs);
            }
            wowObjects.Clear();
            wowNpcs.Clear();
            return null;
        }

        public static WoWPlayerMe Pulse(List<WowObject> wowObjects)
        {
            if (WoWManager.WoWProcess.IsInGame)
            {
                return ObjectMgr.Pulse(wowObjects);
            }
            wowObjects.Clear();
            return null;
        }

        public static WoWPlayerMe Pulse(List<WowPlayer> wowPlayers)
        {
            if (WoWManager.WoWProcess.IsInGame)
            {
                return ObjectMgr.Pulse(wowPlayers);
            }
            wowPlayers.Clear();
            return null;
        }

        public static WoWPlayerMe Pulse(List<WowNpc> wowNpcs)
        {
            if (WoWManager.WoWProcess.IsInGame)
            {
                return ObjectMgr.Pulse(wowNpcs);
            }
            wowNpcs.Clear();
            return null;
        }

        [Pure]
        public static WoWPlayerMe Pulse()
        {
            return WoWManager.WoWProcess.IsInGame ? ObjectMgr.Pulse() : null;
        }

        public static WoWPlayerMe Pulse(List<WowPlayer> wowPlayers, List<WowNpc> wowNpcs)
        {
            if (WoWManager.WoWProcess.IsInGame)
            {
                return ObjectMgr.Pulse(wowPlayers, wowNpcs);
            }
            wowPlayers.Clear();
            wowNpcs.Clear();
            return null;
        }

    }
}
