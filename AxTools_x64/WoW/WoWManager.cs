using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using FMemory;
using System.Diagnostics;
using System.Linq;

namespace AxTools.WoW
{
    internal static class WoWManager
    {
        private static readonly Log2 log = new Log2("WoWManager");
        //internal static WowProcess WoWProcess;
        //internal static bool Hooked { private set; get; }

        static WoWManager()
        {
            WoWProcessManager.WoWProcessClosed += WoWProcessManager_WoWProcessClosed;
        }

        private static void WoWProcessManager_WoWProcessClosed(int processID)
        {
            Unhook(processID);
        }

        /// <summary>
        ///     Selects <see cref="WowProcess"/> from all available processes (via <see cref="WoWProcessSelector"/>).
        ///     Checks selected process for <see cref="WowProcess.IsValidBuild"/> and <see cref="GameInterface.IsInGame"/> states.
        ///     If something went wrong, informs user via Utils.NotifyUser().
        /// </summary>
        /// <returns>
        ///     Instance of <see cref="WowProcess"/> if successful, null otherwise
        /// </returns>
        internal static WowProcess GetProcess()
        {
            WowProcess wowProcess = WoWProcessSelector.GetWoWProcess();
            if (wowProcess != null)
            {
                if (wowProcess.IsValidBuild)
                {
                    if (new GameInterface(wowProcess).IsInGame)
                    {
                        if (wowProcess.Memory == null)
                        {
                            wowProcess.Memory = new MemoryManager(Process.GetProcessById(wowProcess.ProcessID));
                        }
                        return wowProcess;
                    }
                    log.Info($"{wowProcess} Player isn't logged in");
                    Notify.SmartNotify("Cannot attach to WoW client", "Player isn't logged in", NotifyUserType.Error, true);
                    return null;
                }
                else if (wowProcess.GetExecutableRevision() < WowBuildInfoX64.WoWRevision)
                {
                    log.Error($"{wowProcess} WoW client is outdated: {wowProcess.GetExecutableRevision()}");
                    Notify.SmartNotify("Cannot attach to WoW client", "WoW client is outdated. Please update it with Battle.Net client", NotifyUserType.Error, true);
                    return null;
                }
                else
                {
                    log.Error($"{wowProcess} WoW client is outdated: {wowProcess.GetExecutableRevision()}");
                    Notify.SmartNotify("Cannot attach to WoW client", "AxTools is outdated. Please standby", NotifyUserType.Error, true);
                    return null;
                }
            }
            Notify.SmartNotify("Module error", "No WoW process found", NotifyUserType.Error, true);
            return null;
        }

        /// <summary>
        ///     Stops WoW plugin
        /// </summary>
        internal static void Unhook(int processID)
        {
            // cast 'PluginManagerEx.RunningPlugins' to array because we will get 'collection changed' exception otherwise
            foreach (var plugin in PluginManagerEx.RunningPlugins.ToArray())
            {
                if (PluginManagerEx.PluginWoW[plugin.Name] == processID)
                {
                    PluginManagerEx.RemovePluginFromRunning(plugin);
                }
            }
        }
    }
}