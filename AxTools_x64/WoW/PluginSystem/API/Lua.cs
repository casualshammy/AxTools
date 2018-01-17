using AxTools.Helpers;
using AxTools.WoW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AxTools.WoW.PluginSystem.API
{
    public static class Lua
    {
        private static Log2 log = new Log2("Lua");
        private static object _luaLock = new object();
        private static readonly string LuaReturnFrameName = Utilities.GetRandomString(5, true);
        private static readonly string LuaReturnTokenName = Utilities.GetRandomString(5, true);
        private static readonly string LuaReturnVarName = Utilities.GetRandomString(5, true);

        public static string GetValue(string function)
        {
            return GetValue(WoWManager.WoWProcess, function);
        }

        public static bool IsTrue(string condition)
        {
            return IsTrue(WoWManager.WoWProcess, condition);
        }

        internal static string GetValue(WowProcess process, string function)
        {
            lock (_luaLock)
            {
                WoWUIFrame myEditbox = WoWUIFrame.GetFrameByName(process, LuaReturnFrameName);
                if (myEditbox == null)
                {
                    GameFunctions.SendToChat(process, $"/run if(not {LuaReturnFrameName})then CreateFrame(\"EditBox\", \"{LuaReturnFrameName}\", UIParent);{LuaReturnFrameName}:SetAutoFocus(false);{LuaReturnFrameName}:ClearFocus(); end");
                }
                GameFunctions.SendToChat(process, $"/run {LuaReturnVarName}={function};{LuaReturnFrameName}:SetText(\"{LuaReturnTokenName}=\"..{LuaReturnVarName});");
                int counter = 2000;
                while (counter > 0)
                {
                    WoWUIFrame frame = WoWUIFrame.GetFrameByName(process, LuaReturnFrameName);
                    if (frame != null)
                    {
                        string editboxText = frame.EditboxText;
                        if (editboxText != null)
                        {
                            if (editboxText.Contains(LuaReturnTokenName))
                            {
                                return editboxText.Remove(0, LuaReturnTokenName.Length + 1); // 1 - length of "="
                            }
                        }
                    }
                    else
                    {
                        log.Error($"GetValue: Can't find frame ({LuaReturnFrameName})");
                    }
                    Thread.Sleep(10);
                    counter -= 10;
                }
                return null;
            }
        }

        internal static bool IsTrue(WowProcess process, string condition)
        {
            return GetValue(process, $"tostring({condition})") == "true";
        }

    }
}
