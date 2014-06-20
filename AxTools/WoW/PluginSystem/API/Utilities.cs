using System;
using AxTools.Classes;
using AxTools.WoW.Management;

namespace AxTools.WoW.PluginSystem.API
{
    public static class Utilities
    {

        public static void LogPrint(object text)
        {
            Log.Print(String.Format("{0}:{1} :: [Plugin: {2}] {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, PluginManager.ActivePlugin.Name, text));
        }

        public static void StopPlugin()
        {
            PluginManager.StopPlugin(true, true);
        }

    }
}
