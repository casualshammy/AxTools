using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace AutoLoot
{
    internal class AutoLoot : IPlugin
    {

        #region Info

        public string Name { get { return "AutoLoot"; } }

        public Version Version { get { return new Version(1, 0, 0, 0); } }

        public string Author { get { return "Axioma"; } }

        public string Description { get { return "Loots mobs in 100 yd range"; } }

        private Image trayIcon;
        public Image TrayIcon
        {
            get { return trayIcon ?? (trayIcon = new Bitmap(Application.StartupPath + "\\plugins\\AutoLoot\\inv_misc_coin_01.jpg")); }
        }

        public string WowIcon { get { return "Interface\\\\Icons\\\\inv_misc_coin_01"; } }

        public bool ConfigAvailable { get { return false; } }

        #endregion

        #region Events

        public void OnConfig()
        {

        }

        public void OnStart()
        {
            (timer = this.CreateTimer(250, OnPulse)).Start();
        }

        public void OnPulse()
        {
            WoWPlayerMe localPlayer = ObjMgr.Pulse(null, null, wowNpcs);
            if (localPlayer != null && localPlayer.CastingSpellID == 0 && localPlayer.ChannelSpellID == 0 && !Info.IsLooting)
            {
                WowNpc[] npcs = wowNpcs.Where(l => l.Lootable && l.Health == 0 && l.Location.Distance2D(localPlayer.Location) < 40).ToArray();
                if (npcs.Any())
                {
                    WowNpc npc = npcs.Aggregate((current, next) => next.Location.Distance2D(localPlayer.Location) < current.Location.Distance2D(localPlayer.Location) ? next : current);
                    if (npc.Location.Distance2D(localPlayer.Location) > 3f)
                    {
                        MoveMgr.Move2D(npc.Location, 3f, 1000, true, false);
                    }
                    else
                    {
                        npc.Interact();
                    }
                }
            }
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        #endregion

        private readonly List<WowNpc> wowNpcs = new List<WowNpc>();
        private SafeTimer timer;

    }
}
