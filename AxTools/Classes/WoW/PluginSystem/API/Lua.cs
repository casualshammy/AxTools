namespace AxTools.Classes.WoW.PluginSystem.API
{
    public static class Lua
    {
        public static bool LuaDoString(string command)
        {
            return WoW.LuaDoString(command);
        }

        public static string LuaGetLocalizedText(string commandLine)
        {
            return WoW.GetLocalizedText(commandLine);
        }

        public static string GetFunctionReturn(string function)
        {
            return WoW.GetFunctionReturn(function);
        }
    }
}
