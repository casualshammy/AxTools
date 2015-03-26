using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace TestPlugin
{
    class PathRecorder : IPlugin
    {

        #region Info

        public string Name { get { return "PathRecorder"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Author { get { return "Axioma"; } }

        public string Description { get { return "Records path (resolution = 0.25 sec)"; } }

        public Image TrayIcon { get { return null; } }

        public int Interval { get { return 250; } }

        public string WowIcon { get { return string.Empty; } }

        public bool ConfigAvailable { get { return false; } }

        #endregion

        public void OnConfig()
        {
            
        }

        public void OnStart()
        {
            File.Delete(string.Format("{0}\\pluginsSettings\\{1}\\path.txt", Application.StartupPath, Name));
        }

        public void OnPulse()
        {
            WoWPlayerMe localPlayer = ObjMgr.Pulse();
            if (localPlayer != null)
            {
                WowPoint location = localPlayer.Location;
                File.AppendAllLines(string.Format("{0}\\pluginsSettings\\{1}\\path.txt", Application.StartupPath, Name),
                    new[]
                    {
                        String.Format("{0},{1},{2}",
                            location.X.ToString("0.000", CultureInfo.InvariantCulture), location.Y.ToString("0.000", CultureInfo.InvariantCulture), location.Z.ToString("0.000", CultureInfo.InvariantCulture))
                    });
            }
        }

        public void OnStop()
        {
            
        }


        
    }
}
