using System;

namespace AxTools.Classes.WoW.PluginSystem.API
{
    public static class Utilities
    {

        public static void LogPrint(object text)
        {
            Log.Print(String.Format("{0}:{1} :: [Plugin: {2}] {3}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, PluginManager.ActivePlugin.Name, text));
        }

        public static void StopPlugin()
        {
            PluginManager.StopPluginFromPlugin();
        }

    }
}
