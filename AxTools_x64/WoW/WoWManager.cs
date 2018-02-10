using System.Diagnostics;
using System.Linq;
using AxTools.Forms;
using AxTools.Helpers;
using FMemory;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

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
        ///     Checks selected process for <see cref="WowProcess.IsValidBuild"/> and <see cref="Info.IsInGame"/> states.
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
                    log.Info(string.Format("{0} Player isn't logged in", wowProcess));
                    Notify.SmartNotify("Injecting error", "Player isn't logged in", NotifyUserType.Error, true);
                    return null;
                }
                log.Error(string.Format("{0} Incorrect WoW build", wowProcess));
                Notify.SmartNotify("Injecting error", "Incorrect WoW build", NotifyUserType.Error, true);
                return null;
            }
            Notify.SmartNotify("Module error", "No WoW process found", NotifyUserType.Error, true);
            return null;
        }
        
        /// <summary>
        ///     Closes //WowRadarOptions, WoWRadar//
        ///     Stops WoW plugin
        ///     Releases DX hook
        /// </summary>
        internal static void Unhook(int processID)
        {
            foreach (var form in Utils.FindForms<WowRadarOptions>().Where(l => l.ProcessID == processID))
            {
                form.Close();
            }
            foreach (var form in Utils.FindForms<WowRadar>().Where(l => l.ProcessID == processID))
            {
                form.Close();
            }
            foreach (var plugin in PluginManagerEx.RunningPlugins)
            {
                if (PluginManagerEx.PluginWoW[plugin.Name] == processID)
                {
                    PluginManagerEx.RemovePluginFromRunning(plugin);
                }
            }
        }

    }
}
