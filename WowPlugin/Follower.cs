using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AxTools.Classes.WoW;
using AxTools.Classes.WoW.PluginSystem;
using AxTools.Classes.WoW.PluginSystem.API;

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

        public Image TrayIcon
        {
            get { return null; }
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
        }

        public void OnPulse()
        {
            EnvironmentObjects.Pulse();
            EnvironmentObjects.Pulse(players);
            if (EnvironmentObjects.Me.Health > 0)
            {
                WowPlayer myTarget = players.FirstOrDefault(i => i.Health > 0 && i.GUID == EnvironmentObjects.Me.TargetGUID);
                if (myTarget != null)
                {
                    if (myTarget.Location.Distance(EnvironmentObjects.Me.Location) > 5)
                    {
                        Functions.MoveTo(myTarget.Location);
                    }
                }
            }
        }

        public void OnStop()
        {

        }

        #endregion

        private List<WowPlayer> players;

    }
}
