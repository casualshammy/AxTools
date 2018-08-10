using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using System;

namespace AutoDispel
{
    public class AutoDispel : IPlugin3
    {
        public string[] Dependencies => null;

        public string Name => "AutoDispel";

        public Version Version => new Version(1, 0);

        public string Description => "Just dispels crap from friendly units";

        public System.Drawing.Image TrayIcon => null;

        public bool ConfigAvailable => true;

        public bool DontCloseOnWowShutdown => false;

        public void OnConfig()
        {
        }

        public void OnStart()
        {
            this.ShowNotify("This plugin is not completed", true, true);
        }

        public void OnStart(GameInterface game)
        {
            throw new NotImplementedException();
        }

        public void OnStop()
        {
        }
    }
}