using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PathPlayer
{
    internal class PathPlayer : IPlugin3
    {
        #region Info

        public string Name => "PathPlayer";
        public Version Version => new Version(1, 0);
        public string Description => "Wrapper for LibNavigator";
        public bool ConfigAvailable => true;

        private Image trayIcon;
        public Image TrayIcon => trayIcon ?? (trayIcon = Image.FromFile($"{Application.StartupPath}\\plugins\\{Name}\\plainicon.com-50064-256px-4c1.png"));

        public string[] Dependencies => null;

        public bool DontCloseOnWowShutdown => false;

        #endregion Info

        #region Methods

        public void OnConfig()
        {
            if (SettingsInstance == null)
            {
                SettingsInstance = this.LoadSettingsJSON<Settings>();
            }
            SettingsForm.Open(SettingsInstance, this);
            this.SaveSettingsJSON(SettingsInstance);
        }

        public void OnStart(GameInterface game)
        {
            SettingsInstance = this.LoadSettingsJSON<Settings>();
            libNavigator = Utilities.GetReferenceOfPlugin("LibNavigator");

            if (libNavigator == null)
            {
                this.ShowNotify("LibNavigator isn't found! Plugin will not work.", true, true);
                this.LogPrint("LibNavigator isn't found! Plugin will not work.");
            }
            else if (!File.Exists(SettingsInstance.Path))
            {
                this.ShowNotify($"'{SettingsInstance.Path}' isn't found! Plugin will not work.", true, true);
                this.LogPrint($"'{SettingsInstance.Path}' isn't found! Plugin will not work.");
            }
            else
            {
                if (libNavigator.LoadScriptData(File.ReadAllText(SettingsInstance.Path, Encoding.UTF8)))
                {
                    libNavigator.StartLoadedScript(game);
                }
                else
                {
                    this.ShowNotify("Cannot start script. Something went wrong, see log for details", true, true);
                    this.LogPrint("Cannot start script. Something went wrong, see log for details");
                }
            }
        }

        public void OnStop()
        {
            libNavigator.StopLoadedScript();
        }

        #endregion Methods

        internal Settings SettingsInstance;
        private dynamic libNavigator;
    }
}