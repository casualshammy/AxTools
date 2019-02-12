using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WoWPlugin_PathCreator
{
    public class PathCreator : IPlugin3
    {
        #region Info

        public string Name => nameof(PathCreator);

        public Version Version => new Version(1, 0);

        public string Description => "Creates profiles for PathPlayer";

        private Image trayIcon;
        public Image TrayIcon => trayIcon ?? (trayIcon = Image.FromFile($"{this.GetPluginSourceFolder()}\\Achievement_faction_lorewalkers.png"));

        public string WowIcon => string.Empty;

        public bool ConfigAvailable => false;

        public string[] Dependencies => new[] { "LibNavigator" };

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