using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using AxTools.WoW.Helpers;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace LootEnemyCorpses
{
    public class LootEnemyCorpses : IPlugin
    {

        #region Properties

        public string Name
        {
            get { return "Loot Enemy Corpses"; }
        }

        public Version Version
        {
            get { return new Version(1, 0); }
        }

        public string Description
        {
            get { return "Loot all enemy corpses (Ashran, BGs)"; }
        }

        public Image TrayIcon { get { return null; } }

        public string WowIcon
        {
            get { return null; }
        }

        public bool ConfigAvailable
        {
            get { return false; }
        }

        #endregion

        #region Methods

        public void OnConfig()
        {

        }

        public void OnStart()
        {
            (timer = this.CreateTimer(1000, OnElapsed)).Start();
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        private void OnElapsed()
        {
            WoWPlayerMe me = ObjMgr.Pulse(players);
            if (me != null)
            { 
                WowPlayer nearestPlayer =
                    players.Where(l => l.Race == 0x89C && l.Alive && (l.Health*100/l.HealthMax <= 75) && l.Location.Distance(me.Location) < 40).OrderBy(l => l.Location.Distance(me.Location)).FirstOrDefault();
                if (nearestPlayer != null)
                {
                    if (me.TargetGUID != nearestPlayer.GUID)
                    {
                        nearestPlayer.Target();
                        this.LogPrint("Targeting nearest PoI: " + nearestPlayer.GUID + ", " + nearestPlayer.Race);
                    }
                }
            }
        }

        #endregion

        #region Variables

        private SingleThreadTimer timer;
        private readonly List<WowPlayer> players = new List<WowPlayer>();

        #endregion

    }
}
