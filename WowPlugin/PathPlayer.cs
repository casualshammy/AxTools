using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace TestPlugin
{
    class PathPlayer : IPlugin
    {

        #region Info

        public string Name { get { return "PathPlayer"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Author { get { return "Axioma"; } }

        public string Description { get { return "Follows path (resolution = 100 ms)"; } }

        public Image TrayIcon { get { return null; } }

        public int Interval { get { return 100; } }

        public string WowIcon { get { return string.Empty; } }

        public bool ConfigAvailable { get { return false; } }

        #endregion

        public void OnConfig()
        {

        }

        public void OnStart()
        {
            path.Clear();
            string[] pathFile = File.ReadAllLines(string.Format("{0}\\pluginsSettings\\{1}\\path.txt", Application.StartupPath, Name));
            foreach (string s in pathFile)
            {
                string[] ords = s.Split(',');
                if (ords.Length == 3)
                {
                    float x = float.Parse(ords[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(ords[1], CultureInfo.InvariantCulture);
                    float z = float.Parse(ords[2], CultureInfo.InvariantCulture);
                    path.Add(new WowPoint(x, y, z));
                }
            }
        }

        public void OnPulse()
        {
            WoWPlayerMe localPlayer = ObjMgr.Pulse();
            if (localPlayer != null)
            {
                if (localPlayer.Location.Distance(path[counter]) > 3)
                {
                    Functions.MoveTo(path[counter]);
                }
                else
                {
                    if (counter >= path.Count - 1)
                    {
                        counter = 0;
                    }
                    else
                    {
                        counter++;
                    }
                }
            }
        }

        public void OnStop()
        {

        }


        private int counter;
        private readonly List<WowPoint> path = new List<WowPoint>();

    }
}
