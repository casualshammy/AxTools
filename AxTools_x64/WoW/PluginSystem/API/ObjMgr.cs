using System.Collections.Generic;
using System.Diagnostics.Contracts;
using AxTools.WoW.Internals;

namespace AxTools.WoW.PluginSystem.API
{
    public static class ObjMgr
    {
        public static WoWPlayerMe Pulse(List<WowObject> wowObjects = null, List<WowPlayer> wowUnits = null, List<WowNpc> wowNpcs = null)
        {
            if (Info.IsInGame)
            {
                return ObjectMgr.Pulse(wowObjects, wowUnits, wowNpcs);
            }
            wowObjects?.Clear();
            wowUnits?.Clear();
            wowNpcs?.Clear();
            return null;
        }
        
        [Pure]
        public static WoWPlayerMe Pulse()
        {
            return GameFunctions.IsInGame ? ObjectMgr.Pulse() : null;
        }

    }
}
