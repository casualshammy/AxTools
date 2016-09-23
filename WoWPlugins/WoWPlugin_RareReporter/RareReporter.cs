using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_RareReporter
{
    public class RareReporter : IPlugin
    {
        #region Info

        public string Name { get { return "RareReporter"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Reports about rare respawn via SMS (need LibSMS)"; } }

        private Image trayIcon;
        public Image TrayIcon { get { return trayIcon ?? (trayIcon = null); } }

        public string WowIcon { get { return string.Empty; } }

        public bool ConfigAvailable { get { return false; } }

        #endregion

        #region Methods

        public void OnConfig()
        {
            
        }

        public void OnStart()
        {
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
            (timer = this.CreateTimer(50, TimerElapsed)).Start();
        }

        public void OnStop()
        {
            if (timer != null) timer.Dispose();
        }

        public void TimerElapsed()
        {
            WoWPlayerMe me = ObjMgr.Pulse(objects, npcs);
            if (me != null)
            {
                WowNpc npc = npcs.FirstOrDefault(l => l.Name == POI);
                WowObject wowObject = objects.FirstOrDefault(l => l.Name == POI);
                if (npc != null || wowObject != null)
                {
                    string name = npc != null ? npc.Name : wowObject.Name;
                    this.LogPrint("Found new POI: " + name);
                    if ((DateTime.UtcNow - lastTimeSmsSent).TotalSeconds >= 60)
                    {
                        lastTimeSmsSent = DateTime.UtcNow;
                        this.LogPrint("Timing is OK");
                        dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                        if (libSMS != null)
                        {
                            libSMS.SendSMS("New rare found: " + name);
                            this.LogPrint("SMS is sent");
                        }
                    }
                }
            }
        }

        #endregion

        #region Fields

        private SafeTimer timer;
        private DateTime lastTimeSmsSent;
        private readonly List<WowNpc> npcs = new List<WowNpc>();
        private readonly List<WowObject> objects = new List<WowObject>();
        private const string POI = "Лук'хок";

        #endregion
    }
}
