using System.Diagnostics;
using System.Linq;
using AxTools.Forms;
using AxTools.Helpers;
using AxTools.Helpers.MemoryManagement;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace AxTools.WoW
{
    internal static class WoWManager
    {
        private static readonly Log2 log = new Log2("WoWManager");
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
                        if (Info.IsProcessInGame(wowProcess))
                        {
                            WoWProcess = wowProcess;
                            if (WoWProcess.Memory == null)
                            {
                                WoWProcess.Memory = new MemoryManager(Process.GetProcessById(WoWProcess.ProcessID));
                            }
                            ObjectMgr.Initialize(WoWProcess);
                            Hooked = true;
                            return true;
                        }
                        log.Info(string.Format("{0} Player isn't logged in", wowProcess));
                        Notify.SmartNotify("Injecting error", "Player isn't logged in", NotifyUserType.Error, true);
                        return false;
                    }
                    log.Error(string.Format("{0} Incorrect WoW build", wowProcess));
                    Notify.SmartNotify("Injecting error", "Incorrect WoW build", NotifyUserType.Error, true);
                    return false;
                }
                return true;
            }
            Notify.SmartNotify("Module error", "No WoW process found", NotifyUserType.Error, true);
            return false;
        }

        /// <summary>
        ///     Closes //WowRadarOptions, WoWRadar//
        ///     Stops WoW plugin
        ///     Releases DX hook
        /// </summary>
        internal static void Unhook()
        {
            WowRadarOptions pWowRadarOptions = Utils.FindForm<WowRadarOptions>();
            if (pWowRadarOptions != null) pWowRadarOptions.Close();
            WowRadar pWowRadar = Utils.FindForm<WowRadar>();
            if (pWowRadar != null) pWowRadar.Close();

            if (PluginManagerEx.RunningPlugins.Any())
            {
                PluginManagerEx.StopPlugins();
            }

            log.Info(string.Format("{0} Total objects cached: {1}", WoWProcess, WowObject.Names.Count));
            WowObject.Names.Clear();
            log.Info(string.Format("{0} Total players cached: {1}", WoWProcess, WowPlayer.Names.Count));
            WowPlayer.Names.Clear();
            log.Info(string.Format("{0} Total NPC cached: {1}", WoWProcess, WowNpc.Names.Count));
            WowNpc.Names.Clear();
            Hooked = false;
            log.Info(string.Format("{0} Detached", WoWProcess));
        }

    }
}
