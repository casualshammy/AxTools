using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace TestPlugin
{
    class Follower : IPlugin
    {

        #region Info

        public string Name
        {
            get { return "Follower"; }
        }

        public Version Version
        {
            get { return new Version(2, 0); }
        }

        public string Author
        {
            get { return "CasualShammy"; }
        }

        public string Description
        {
            get
            {
                return "Follows target";
            }
        }

        private Image trayIcon;
        public Image TrayIcon
        {
            get { return trayIcon ?? (trayIcon = new Bitmap(Application.StartupPath + "\\plugins\\Follower\\ability_hunter_posthaste.jpg")); }
        }

        public int Interval
        {
            get { return 500; }
        }

        public string WowIcon
        {
            get { return string.Empty; }
        }

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
            WoWPlayerMe locaPlayer = ObjMgr.Pulse(players, npcs);
            WowPlayer myTargetPlayer = players.FirstOrDefault(i => i.Health > 0 && i.GUID == locaPlayer.TargetGUID);
            WowNpc myTargetNpc = npcs.FirstOrDefault(i => i.Health > 0 && i.GUID == locaPlayer.TargetGUID);
            if (myTargetPlayer != null)
            {
                guid = myTargetPlayer.GUID;
            }
            else if (myTargetNpc != null)
            {
                guid = myTargetNpc.GUID;
            }
            else
            {
                Utilities.LogPrint("Unit isn't found!");
            }
        }

        public void OnPulse()
        {
            WoWPlayerMe locaPlayer = ObjMgr.Pulse(players, npcs);
            if (locaPlayer.Health > 0)
            {
                WowPlayer unitPlayer = players.FirstOrDefault(i => i.Health > 0 && i.GUID == guid);
                WowNpc unitNpc = npcs.FirstOrDefault(i => i.Health > 0 && i.GUID == guid);
                if (unitPlayer != null)
                {
                    if (unitPlayer.Location.Distance(locaPlayer.Location) > 5)
                    {
                        Functions.MoveTo(unitPlayer.Location);
                    }
                }
                else if (unitNpc != null)
                {
                    if (unitNpc.Location.Distance(locaPlayer.Location) > 5)
                    {
                        Functions.MoveTo(unitNpc.Location);
                    }
                }
            }
        }

        public void OnStop()
        {

        }

        #endregion

        private readonly List<WowPlayer> players = new List<WowPlayer>();
        private readonly List<WowNpc> npcs = new List<WowNpc>();
        private UInt128 guid;

    }
}
