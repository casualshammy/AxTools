using System;
using System.Drawing;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_Dumper
{
    public class Dumper : IPlugin3
    {

        #region Info

        public string Name { get { return "Dumper"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Dumps ingame info"; } }

        private Image trayIcon;
        public Image TrayIcon { get { return trayIcon ?? (trayIcon = Image.FromFile(string.Format("{0}\\plugins\\{1}\\icon.png", Application.StartupPath, Name))); } }

        public bool ConfigAvailable { get { return false; } }

        public string[] Dependencies => null;

        public bool DontCloseOnWowShutdown => false;

        #endregion

        #region Methods

        public void OnConfig()
        {
            
        }

        public void OnStart(GameInterface game)
        {
            this.game = game;
            (form = new DumperForm(this, game)).Show();
            form.Activate();
            (timer = this.CreateTimer(100, game, OnElapsed)).Start();
        }

        public void OnStop()
        {
            timer.Dispose();
            form.Close();
        }

        private void OnElapsed()
        {
            
        }

        #endregion

        private SafeTimer timer;
        private DumperForm form;
        private GameInterface game;

    }
}
