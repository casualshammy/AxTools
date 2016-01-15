using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;

namespace AxTools.WoW.PluginSystem.API
{
    public static class GameFunctions
    {
        public static void Interact(this WowObject wowObject)
        {
            Interact(wowObject.GUID);
        }

        public static void Interact(this WowNpc wowNpc)
        {
            Interact(wowNpc.GUID);
        }

        public static void Interact(UInt128 guid)
        {
            if (WoWManager.WoWProcess.IsInGame)
            {
                WoWDXInject.Interact(guid);
            }
        }

        public static void Target(this WowNpc wowNpc)
        {
            Target(wowNpc.GUID);
        }

        public static void Target(this WowPlayer wowPlayer)
        {
            Target(wowPlayer.GUID);
        }

        public static void Target(UInt128 guid)
        {
            if (WoWManager.WoWProcess.IsInGame)
            {
                WoWDXInject.TargetUnit(guid);
            }
        }

        public static void MoveTo(WowPoint point)
        {
            if (WoWManager.WoWProcess.IsInGame)
            {
                WoWDXInject.MoveTo(point);
            }
        }

        public static void Lua_DoString(string command)
        {
            if (WoWManager.WoWProcess.IsInGame)
            {
                WoWDXInject.LuaDoString(command);
            }
        }

        public static string Lua_GetFunctionReturn(string function)
        {
            return WoWManager.WoWProcess.IsInGame && WoWManager.WoWProcess.IsNotLoadingScreen ? WoWDXInject.GetFunctionReturn(function) : "";
        }

        public static void Lua_EnableCTM()
        {
            Lua_DoString("SetCVar(\"autointeract\", 1, \"CLICK_TO_MOVE\")");
        }

        public static void Lua_DisableCTM()
        {
            Lua_DoString("SetCVar(\"autointeract\", 0, \"CLICK_TO_MOVE\")");
        }

        public static bool Lua_IsCTMEnabled()
        {
            return Lua_GetFunctionReturn("GetCVar(\"autointeract\")") == "1";
        }
    }
}
