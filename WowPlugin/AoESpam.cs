using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AxTools.WoW;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace TestPlugin
{
    class AoESpam : IPlugin
    {

        #region Info

        public string Name { get { return "AoESpam"; } }

        public Version Version { get { return new Version(1, 0, 0, 0); } }

        public string Author { get { return "Axioma"; } }

        public string Description { get { return "Spams one AoE spell at the feet"; } }

        private Image trayIcon;
        public Image TrayIcon
        {
            get { return trayIcon ?? (trayIcon = new Bitmap(Application.StartupPath + "\\plugins\\AoESpam\\spell_shaman_earthquake.jpg")); }
        }

        public int Interval { get { return 250; } }

        public string WowIcon { get { return "Interface\\\\Icons\\\\spell_shaman_earthquake"; } }

        public bool ConfigAvailable { get { return false; } }

        #endregion

        #region Events

        public void OnConfig()
        {

        }

        public void OnStart()
        {

        }

        public void OnPulse()
        {
            try
            {
                
            }
            catch (Exception)
            {

            }
        }

        public void OnStop()
        {

        }

        #endregion

		private readonly string spellName = "Землетрясение";

        private bool state = false;
    }
}
