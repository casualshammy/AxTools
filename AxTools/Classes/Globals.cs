using System.Reflection;
using System.Windows.Forms;
using AxTools.Helpers;
using MouseKeyboardActivityMonitor.WinApi;

namespace AxTools.Classes
{
    internal class Globals
    {
        internal static readonly string TempPath = Application.StartupPath + "\\tmp";
        internal static readonly string CfgPath = Application.StartupPath + "\\cfg";
        internal static readonly string UserfilesPath = Application.StartupPath + "\\userfiles";
        internal static readonly string LogFileName = TempPath + "\\AxTools.log";
        internal static readonly string PluginsPath = Application.StartupPath + "\\plugins";
        internal static readonly string PluginsAssembliesPath = Application.StartupPath + "\\pluginsAssemblies";
        internal static readonly string PluginsSettingsPath = Application.StartupPath + "\\pluginsSettings";
        internal static readonly string DropboxPath = "https://dl.dropboxusercontent.com/u/33646867/axtools";
        internal static readonly string UpdateFilePath = DropboxPath + "/__update";
        internal static readonly VersionExt AppVersion = new VersionExt(Assembly.GetExecutingAssembly().GetName().Version);
        internal static readonly GlobalHooker GlobalHooker = new GlobalHooker();

        internal static readonly SrvAddress[] GameServers =
        {
            new SrvAddress(string.Empty, 0, "Disabled"),
            new SrvAddress("google.com", 80, "Google"),
            new SrvAddress("109.105.134.173", 7777, "Lineage 2 - Athebaldt"),
            new SrvAddress("195.12.246.208", 3724, "World of Warcraft - Blackscar"),
            new SrvAddress("195.12.246.211", 3724, "World of Warcraft - Gordunni"),
            new SrvAddress("195.12.240.179", 3724, "World of Warcraft - Ravencrest (Frankfurt)"),
            new SrvAddress("199.107.24.244", 3724, "World of Warcraft - Area 52 (New York)"),
            new SrvAddress("195.12.235.9", 3724, "World of Warcraft - Argent Dawn (Paris)"),
            new SrvAddress("195.12.246.224", 3724, "World of Warcraft - Deathguard")
        };
    }
}
