using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace PathRecorder
{
    class PathRecorder : IPlugin
    {

        #region Info

        public string Name { get { return "PathRecorder"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Records path (resolution = 0.25 sec)"; } }

        public Image TrayIcon { get { return null; } }

        public string WowIcon { get { return string.Empty; } }

        public bool ConfigAvailable { get { return false; } }

        #endregion

        #region Methods

        public void OnConfig()
        {
            
        }

        public void OnStart()
        {
            File.Delete(string.Format("{0}\\pluginsSettings\\{1}\\path.txt", Application.StartupPath, Name));
            (timer = this.CreateTimer(250, OnPulse)).Start();
        }

        private void OnPulse()
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
            timer.Dispose();
        }

        #endregion
        
        private SingleThreadTimer timer;
    
    }
}
