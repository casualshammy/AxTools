using System.Net.Http;
using System.Reflection;

namespace AxTools.Helpers
{
    internal static class Globals
    {
        internal static readonly string LogFileName = AppFolders.TempDir + "\\AxTools.log";
        internal static readonly VersionExt AppVersion = new VersionExt(Assembly.GetExecutingAssembly().GetName().Version);
        internal static readonly string PluginsURL = "https://axio.name/axtools/plugins/";
        
    }
}
