using System.Windows.Forms;

namespace AxTools.Classes
{
    class Globals
    {
        internal static readonly string TempPath = Application.StartupPath + "\\tmp";
        internal static readonly string CfgPath = Application.StartupPath + "\\cfg";
        internal static readonly string UserfilesPath = Application.StartupPath + "\\userfiles";
        internal static readonly string SettingsFilePath = Application.StartupPath + "\\cfg\\.settings";
        internal static readonly string WowAccountsFilePath = Application.StartupPath + "\\cfg\\.wowaccounts2";
        internal static readonly string LogFileName = TempPath + "\\AxTools.log";
        internal static readonly string DropboxPath = "https://dl.dropboxusercontent.com/u/33646867/axtools";
        internal static readonly string UpdateFilePath = DropboxPath + "/update!push";

        internal static readonly SrvAddress[] GameServers =
        {
            new SrvAddress("195.12.246.207", 3724, "World of Warcraft - Blackscar"),
            new SrvAddress("109.105.134.173", 7777, "Lineage 2 - Athebaldt"),
            new SrvAddress("195.12.240.179", 3724, "World of Warcraft - Ravencrest (Frankfurt)"),
            new SrvAddress("199.107.24.244", 3724, "World of Warcraft - Area 52 (New York)"),
            new SrvAddress("195.12.235.9", 3724, "World of Warcraft - Argent Dawn (Paris)"),
            new SrvAddress("195.12.246.224", 3724, "World of Warcraft - Deathguard")
        };
    }
}
