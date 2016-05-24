using System.IO;

namespace AxTools.Helpers
{
    internal class AppFolders
    {
        
        internal static void CreateTempDir()
        {
            if (!Directory.Exists(Globals.TempPath))
            {
                Directory.CreateDirectory(Globals.TempPath);
            }
        }

        internal static void CreateUserfilesDir()
        {
            if (!Directory.Exists(Globals.UserfilesPath))
            {
                Directory.CreateDirectory(Globals.UserfilesPath);
            }
        }

        internal static void CreateConfigDir()
        {
            if (!Directory.Exists(Globals.CfgPath))
            {
                Directory.CreateDirectory(Globals.CfgPath);
            }
        }

        internal static void CreatePluginsDir()
        {
            if (!Directory.Exists(Globals.PluginsPath))
            {
                Directory.CreateDirectory(Globals.PluginsPath);
            }
        }

        internal static void CreatePluginsBinariesDir()
        {
            if (!Directory.Exists(Globals.PluginsAssembliesPath))
            {
                Directory.CreateDirectory(Globals.PluginsAssembliesPath);
            }
        }

        internal static void CreatePluginsSettingsDir()
        {
            if (!Directory.Exists(Globals.PluginsSettingsPath))
            {
                Directory.CreateDirectory(Globals.PluginsSettingsPath);
            }
        }

    }
}
