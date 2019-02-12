using System.IO;
using System.Windows.Forms;

namespace AxTools.Helpers
{
    internal static class AppFolders
    {
        internal static string UserfilesDir
        {
            get
            {
                string path = Application.StartupPath + "\\userfiles";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        internal static string ConfigDir
        {
            get
            {
                string path = Application.StartupPath + "\\cfg";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        
        internal static string PluginsBinariesDir
        {
            get
            {
                string path = Application.StartupPath + "\\pluginsAssemblies";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        internal static string PluginsSettingsDir
        {
            get
            {
                string path = Application.StartupPath + "\\pluginsSettings";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        internal static string ResourcesDir
        {
            get
            {
                string path = Application.StartupPath + "\\resources";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        internal static string DataDir
        {
            get
            {
                string path = Application.StartupPath + "\\data";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        internal static string TempDir
        {
            get
            {
                string path = Application.StartupPath + "\\tmp";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
    }
}