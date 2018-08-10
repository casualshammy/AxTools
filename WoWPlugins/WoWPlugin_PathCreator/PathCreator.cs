using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WoWPlugin_PathCreator
{
    public class PathCreator : IPlugin3
    {
        #region Info

        public string Name { get { return "PathCreator"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Creates profiles for PathPlayer"; } }

        private Image trayIcon;
        public Image TrayIcon { get { return trayIcon ?? (trayIcon = Image.FromFile(string.Format("{0}\\plugins\\{1}\\Achievement_faction_lorewalkers.png", Application.StartupPath, Name))); } }

        public string WowIcon { get { return string.Empty; } }

        public bool ConfigAvailable { get { return false; } }

        public string[] Dependencies => new string[] { "LibNavigator" };

        public bool DontCloseOnWowShutdown => false;

        #endregion Info

        #region Methods

        public void OnConfig()
        {
        }

        public void OnStart(GameInterface game)
        {
            this.game = game;
            (mainForm = new MainForm(this, game)).Show();
            (timer = this.CreateTimer(250, game, OnPulse)).Start();
        }

        private void OnPulse()
        {
            WoWPlayerMe me = game.GetGameObjects();
            if (me != null)
            {
                mainForm.labelLocation.Text = "[" + me.Location + "]";
            }
        }

        public void OnStop()
        {
            timer.Dispose();
            mainForm.Hide();
            mainForm.Dispose();
        }

        #endregion Methods

        private SafeTimer timer;
        private MainForm mainForm;
        private GameInterface game;
    }
}