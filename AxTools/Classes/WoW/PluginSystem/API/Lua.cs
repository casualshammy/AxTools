using AxTools.Classes.WoW.Management;

namespace AxTools.Classes.WoW.PluginSystem.API
{
    public static class Lua
    {
        public static void LuaDoString(string command)
        {
            WoWDXInject.LuaDoString(command);
        }

        public static string LuaGetLocalizedText(string commandLine)
        {
            return WoWDXInject.GetLocalizedText(commandLine);
        }

        public static string GetFunctionReturn(string function)
        {
            return WoWDXInject.GetFunctionReturn(function);
        }
    }
}
