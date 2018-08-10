using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WoWPlugin_RareReporter
{
    public class RareReporter : IPlugin3
    {
        #region Info

        public string Name { get { return "RareReporter"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Reports about rare respawn via SMS (need LibSMS)"; } }

        private Image trayIcon;
        public Image TrayIcon { get { return trayIcon ?? (trayIcon = null); } }

        public string WowIcon { get { return string.Empty; } }

        public bool ConfigAvailable { get { return false; } }

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
            dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
            if (libSMS == null)
            {
                this.ShowNotify("LibSMS isn't found! Plugin will not work.", true, true);
                this.LogPrint("LibSMS isn't found! Plugin will not work.");
                return;
            }
            if (libSMS.GetType().GetMethod("SendSMS") == null)
            {
                this.ShowNotify("LibSMS.SendSMS method isn't found! Plugin will not work.", true, true);
                this.LogPrint("LibSMS.SendSMS method isn't found! Plugin will not work.");
                return;
            }
            this.LogPrint("LibSMS is OK");
            lastTimeSmsSent = DateTime.MinValue;
            (timer = this.CreateTimer(50, game, TimerElapsed)).Start();
        }

        public void OnStop()
        {
            if (timer != null) timer.Dispose();
        }

        public void TimerElapsed()
        {
            WoWPlayerMe me = game.GetGameObjects(objects, null, npcs);
            if (me != null)
            {
                WowNpc npc = npcs.FirstOrDefault(l => l.Alive && POI.Contains(l.Name));
                WowObject wowObject = objects.FirstOrDefault(l => POI.Contains(l.Name));
                if (npc != null || wowObject != null)
                {
                    string name = npc != null ? npc.Name : wowObject.Name;
                    this.LogPrint("Found new POI: " + name);
                    if ((DateTime.UtcNow - lastTimeSmsSent).TotalSeconds >= timeout)
                    {
                        lastTimeSmsSent = DateTime.UtcNow;
                        this.LogPrint("Timing is OK");
                        dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                        if (libSMS != null)
                        {
                            libSMS.SendSMS($"Rare spawned: {name}\r\nTime: {DateTime.Now}");
                            Utilities.ShowNotify(this, $"Rare spawned: {name}\r\nTime: {DateTime.Now}", true, true, int.MaxValue); // $"Rare spawned: {name}" // string.Format("Rare spawned: {0}", name)
                            this.LogPrint($"Rare spawned: {name}; message is sent");
                        }
                    }
                }
            }
        }

        #endregion Methods

        #region Fields

        private SafeTimer timer;
        private DateTime lastTimeSmsSent;
        private readonly List<WowNpc> npcs = new List<WowNpc>();
        private readonly List<WowObject> objects = new List<WowObject>();
        private readonly string[] POI = new[] { "Скалгулот" };
        private const int timeout = 120;
        private GameInterface game;

        #endregion Fields
    }
}