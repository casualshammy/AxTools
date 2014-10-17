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
            get { return new Version(1, 0); }
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

        #endregion

        #region Events

        public void OnConfig()
        {
            throw new NotImplementedException();
        }

        public void OnStart()
        {
            players = new List<WowPlayer>();
            locaPlayer = ObjMgr.Pulse(players);
            WowPlayer myTarget = players.FirstOrDefault(i => i.Health > 0 && i.GUID == locaPlayer.TargetGUID);
            if (myTarget != null)
            {
                guid = myTarget.GUID;
            }
            else
            {
                Utilities.LogPrint("Unit isn't found!");
            }
        }

        public void OnPulse()
        {
            locaPlayer = ObjMgr.Pulse(players);
            if (locaPlayer.Health > 0)
            {
                WowPlayer unit = players.FirstOrDefault(i => i.Health > 0 && i.GUID == guid);
                if (unit != null)
                {
                    if (unit.Location.Distance(locaPlayer.Location) > 5)
                    {
                        Functions.MoveTo(unit.Location);
                    }
                }
            }
        }

        public void OnStop()
        {

        }

        #endregion

        private List<WowPlayer> players;
        private WoWPlayerMe locaPlayer;
        private UInt128 guid;

    }
}
