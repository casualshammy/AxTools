using AxTools.Helpers;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using System;
using System.Diagnostics;
using System.Linq;

namespace AxTools.WoW.PluginSystem.API
{
    class Info
    {
        
        public static string ZoneText
        {
            get
            {
                return Wowhead.GetZoneText(ZoneID);
            }
        }

        public static uint ZoneID
        {
            get
            {
                return WoWManager.WoWProcess.Memory.Read<uint>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.PlayerZoneID);
            }
        }

        public static bool IsLooting
        {
            get
            {
                return WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.PlayerIsLooting) != 0;
            }
        }

        public static bool IsInGame
        {
            get
            {
                try
                {
                    return WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.GameState) == 1;
                }
                catch
                {
                    return false;
                }
            }
        }

        internal static bool IsProcessInGame(WowProcess process)
        {
            if (process.Memory == null) return false;
            return process.Memory.Read<byte>(process.Memory.ImageBase + WowBuildInfoX64.GameState) == 1;
        }

        public static bool IsLoadingScreen
        {
            get
            {
                try
                {
                    return WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.NotLoadingScreen) == 0;
                }
                catch (Exception ex)
                {
                    StackTrace stackTrace = new StackTrace();
                    StackFrame[] stackFrames = stackTrace.GetFrames();
                    string stack = stackFrames != null ? string.Join(" -->> ", stackFrames.Select(l => string.Format("{0}::{1}", l.GetFileName(), l.GetMethod().Name)).Reverse()) : "Stack is null";
                    Log.Error(string.Format("IsLoadingScreen: stack trace: {0}; error message: {1}", stack, ex.Message));
                    return false;
                }
            }
        }

        public static bool IsSpellKnown(uint spellID)
        {
            //return LuaGetFunctionReturn("tostring(IsSpellKnown(" + spellID + "))") == "true";

            uint totalKnownSpells = WoWManager.WoWProcess.Memory.Read<uint>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.KnownSpellsCount);
            IntPtr knownSpells = WoWManager.WoWProcess.Memory.Read<IntPtr>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.KnownSpells);
            for (int i = 0; i < totalKnownSpells; i++)
            {
                IntPtr spellAddress = WoWManager.WoWProcess.Memory.Read<IntPtr>(knownSpells + 8 * i);
                uint pSpellID = WoWManager.WoWProcess.Memory.Read<uint>(spellAddress + 4);
                uint pSpellSuccess = WoWManager.WoWProcess.Memory.Read<uint>(spellAddress);
                if (pSpellSuccess == 1 && pSpellID == spellID)
                {
                    return true;
                }
            }
            return false;
        }

        public static WoWGUID MouseoverGUID
        {
            get { return WoWManager.WoWProcess.Memory.Read<WoWGUID>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.MouseoverGUID); }
        }
        
    }
}
