using AxTools.Forms;
using AxTools.Helpers;
using AxTools.Helpers.MemoryManagement;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using System;
using System.Diagnostics;
using System.Linq;

namespace AxTools.WoW.Management
{
    internal static class WoWManager
    {
        internal static WowProcess WoWProcess;
        internal static bool Hooked { private set; get; }

        /// <summary>
        ///     Selects <see cref="WowProcess"/> from all available processes (via <see cref="WoWProcessSelector"/>).
        ///     Checks selected process for IncorrectDirectXVersion, InvalidWoWBuild and NotInGame states.
        ///     If something went wrong, informs user via Utils.NotifyUser().
        /// </summary>
        /// <returns>
        ///     True if hook was successful, false otherwise
        /// </returns>
        internal static bool HookWoWAndNotifyUserIfError()
        {
            WowProcess wowProcess = WoWProcessSelector.GetWoWProcess();
            if (wowProcess != null)
            {
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
                                if (WoWDXInject.Apply(WoWProcess))
                                {
                                    ObjectMgr.Initialize(WoWProcess);
                                    Hooked = true;
                                    return true;
                                }
                                throw new Exception("Hook point has invalid signature! Please restart WoW.");
                            }
                            catch (Exception ex)
                            {
                                AppSpecUtils.NotifyUser("Injecting error", ex.Message + "\r\n\r\nSee log for info", NotifyUserType.Error, true);
                                return false;
                            }
                        }
                        Log.Info(string.Format("{0}:{1} :: [WoW hook] Player isn't logged in", wowProcess.ProcessName, wowProcess.ProcessID));
                        AppSpecUtils.NotifyUser("Injecting error", "Player isn't logged in", NotifyUserType.Error, true);
                        return false;
                    }
                    Log.Error(string.Format("{0}:{1} :: [WoW hook] Incorrect WoW build", wowProcess.ProcessName, wowProcess.ProcessID));
                    AppSpecUtils.NotifyUser("Injecting error", "Incorrect WoW build", NotifyUserType.Error, true);
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

            if (PluginManagerEx.RunningPlugins.Any())
            {
                PluginManagerEx.StopPlugins();
            }

            WoWDXInject.Release();
            Log.Info(string.Format("{0} [WoW hook] Total objects cached: {1}", WoWProcess, WowObject.Names.Count));
            WowObject.Names.Clear();
            Log.Info(string.Format("{0} [WoW hook] Total players cached: {1}", WoWProcess, WowPlayer.Names.Count));
            WowPlayer.Names.Clear();
            Log.Info(string.Format("{0} [WoW hook] Total NPC cached: {1}", WoWProcess, WowNpc.Names.Count));
            WowNpc.Names.Clear();
            Hooked = false;
            Log.Info(string.Format("{0} [WoW hook] Detached", WoWProcess));
        }

        internal enum HookResult
        {
            OK,
            NoWoWProcessSelected,
            PlayerIsNotInGame,
            IncorrectWoWBuild,
            AlreadyInitialized,
            IncorrectHookPointer,
        }

        internal static HookResult Attach(WowProcess wowProcess)
        {
            if (wowProcess != null)
            {
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
                            if (WoWDXInject.Apply(WoWProcess))
                            {
                                ObjectMgr.Initialize(WoWProcess);
                                Hooked = true;
                                return HookResult.OK;
                            }
                            return HookResult.IncorrectHookPointer;
                        }
                        return HookResult.PlayerIsNotInGame;
                    }
                    return HookResult.IncorrectWoWBuild;
                }
                return HookResult.AlreadyInitialized;
            }
            return HookResult.NoWoWProcessSelected;
        }

        internal static void Detach()
        {
            Unhook();
        }

    }
}
