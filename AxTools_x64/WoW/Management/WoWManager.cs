using AxTools.Classes;
using AxTools.Forms;
using AxTools.Helpers;
using AxTools.Helpers.MemoryManagement;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using System;
using System.Diagnostics;

namespace AxTools.WoW.Management
{
    internal static class WoWManager
    {
        internal static WowProcess WoWProcess;
        internal static bool Hooked { private set; get; }

        /// <summary>
        ///     Selects <see cref="WowProcess"/> from all available processes (via <see cref="ProcessSelection"/>).
        ///     Checks selected process for IncorrectDirectXVersion, InvalidWoWBuild and NotInGame states.
        ///     If something went wrong, informs user via Utils.NotifyUser().
        /// </summary>
        /// <returns>
        ///     True if hook was successful, false otherwise
        /// </returns>
        internal static bool HookWoWAndNotifyUserIfError()
        {
            int index = WowProcess.GetAllWoWProcesses().Count == 1 ? 0 : ProcessSelection.SelectProcess();
            if (index != -1)
            {
                WowProcess wowProcess = WowProcess.GetAllWoWProcesses()[index];
                if (!Hooked)
                {
                    if (wowProcess.IsValidBuild)
                    {
                        if (wowProcess.IsInGame)
                        {
                            WoWProcess = wowProcess;
                            if (WoWProcess.Memory == null)
                            {
                                WoWProcess.Memory = new MemoryManager(Process.GetProcessById(WoWProcess.ProcessID));
                            }
                            try
                            {
                                WoWDXInject.Apply(WoWProcess);
                                ObjectMgr.Initialize(WoWProcess);
                                Hooked = true;
                                return true;
                            }
                            catch (Exception ex)
                            {
                                Utils.NotifyUser("Injecting error", ex.Message + "\r\n\r\nSee log for info", NotifyUserType.Error, true);   // WoWDXInject.Apply() writes error to log by himself;
                                return false;                                                                                               // ObjectMgr.Initialize() can't throw an error;
                            }
                        }
                        Log.Print(String.Format("{0}:{1} :: [WoW hook] Player isn't logged in", wowProcess.ProcessName, wowProcess.ProcessID));
                        Utils.NotifyUser("Injecting error", "Player isn't logged in", NotifyUserType.Error, true);
                        return false;
                    }
                    Log.Print(String.Format("{0}:{1} :: [WoW hook] Incorrect WoW build", wowProcess.ProcessName, wowProcess.ProcessID), true);
                    Utils.NotifyUser("Injecting error", "Incorrect WoW build", NotifyUserType.Error, true);
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Closes //WoWRadar, LuaConsole//
        ///     Stops WoW plugin
        ///     Releases DX hook
        /// </summary>
        internal static void Unhook()
        {
            WowRadarOptions pWowRadarOptions = Utils.FindForm<WowRadarOptions>();
            if (pWowRadarOptions != null) pWowRadarOptions.Close();
            WowRadar pWowRadar = Utils.FindForm<WowRadar>();
            if (pWowRadar != null) pWowRadar.Close();
            LuaConsole pLuaInjector = Utils.FindForm<LuaConsole>();
            if (pLuaInjector != null) pLuaInjector.Close();

            if (PluginManager.ActivePlugin != null)
            {
                PluginManager.StopPlugin();
            }

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
