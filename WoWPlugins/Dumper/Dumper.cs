using AxTools.WoW.Helpers;
using AxTools.WoW.PluginSystem;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WoWPlugin_Dumper
{
    public class Dumper : IPlugin3
    {
        #region Info

        public string Name => nameof(Dumper);

        public Version Version => new Version(1, 0);

        public string Description => "Dumps ingame info";

        private Image trayIcon;
        public Image TrayIcon => trayIcon ?? (trayIcon = Image.FromFile($"{this.GetPluginSourceFolder()}\\icon.png"));

        public bool ConfigAvailable => false;

        public string[] Dependencies => null;

        public bool DontCloseOnWowShutdown => false;

        #endregion Info

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

        #endregion Methods

        private SafeTimer timer;
        private DumperForm form;
        private GameInterface game;
    }
}