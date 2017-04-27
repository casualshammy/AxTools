using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace Talon_s_Vengeance_Farmer
{
    public class TalonsVengeanceFarmer : IPlugin
    {

        #region Info

        public string Name
        {
            get { return "Talon's Vengeance Farmer"; }
        }

        public Version Version
        {
            get { return new Version(1, 0); }
        }

        public string Description
        {
            get { return ""; }
        }

        public Image TrayIcon { get { return null; } }

        public bool ConfigAvailable
        {
            get { return false; }
        }

        #endregion

        #region Events

        public void OnConfig()
        {

        }

        public void OnStart()
        {
            timer = this.CreateTimer(100, OnPulse);
            timer.Start();
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        private void OnPulse()
        {
            WoWPlayerMe me = ObjMgr.Pulse(null, players);
            if (me != null)
            {
                WowPlayer targetPlayer = players.FirstOrDefault(l => l.GUID == me.TargetGUID);
                if (targetPlayer != null)
                {
                    if (targetPlayer.Auras.All(l => l.Name != "")) // todo
                    {
                        
                    }
                }
            }
        }

        #endregion

        #region Fields, propeties

        private readonly List<WowPlayer> players = new List<WowPlayer>();
        private bool IsMaster = true;
        private SafeTimer timer;

        #endregion

    }
}
