using System.Diagnostics;
using AxTools.Classes.WoW.Management.ObjectManager;
using AxTools.Forms;
using GreyMagic;

namespace AxTools.Classes.WoW.Management
{
    internal static class WoWManager
    {
        internal static WowProcess WoWProcess;
        internal static bool Hooked { private set; get; }

        internal static HookResult Hook(WowProcess process)
        {
            WoWProcess = process;
            if (WoWProcess.Memory == null)
            {
                WoWProcess.Memory = new ExternalProcessReader(Process.GetProcessById(WoWProcess.ProcessID));
            }
            if (!WoWDXInject.Apply(WoWProcess))
            {
                return HookResult.IncorrectDirectXVersion;
            }
            ObjectMgr.Initialize(WoWProcess);
            Hooked = true;
            return HookResult.Successful;
        }

        internal static void Unhook()
        {
            WowRadarOptions pWowRadarOptions = Utils.FindForm<WowRadarOptions>();
            if (pWowRadarOptions != null) pWowRadarOptions.Close();
            WowRadar pWowRadar = Utils.FindForm<WowRadar>();
            if (pWowRadar != null) pWowRadar.Close();
            LuaConsole pLuaInjector = Utils.FindForm<LuaConsole>();
            if (pLuaInjector != null) pLuaInjector.Close();

            WoWDXInject.Release();
            Log.Print(string.Format("{0}:{1} :: [WoW hook] Total objects cached: {2}", WoWProcess.ProcessName, WoWProcess.ProcessID, WowObject.Names.Count), false, false);
            WowObject.Names.Clear();
            Log.Print(string.Format("{0}:{1} :: [WoW hook] Total players cached: {2}", WoWProcess.ProcessName, WoWProcess.ProcessID, WowPlayer.Names.Count), false, false);
            WowPlayer.Names.Clear();
            Log.Print(string.Format("{0}:{1} :: [WoW hook] Total NPC cached: {2}", WoWProcess.ProcessName, WoWProcess.ProcessID, WowNpc.Names.Count));
            WowNpc.Names.Clear();
            Hooked = false;
        }

    }
}
