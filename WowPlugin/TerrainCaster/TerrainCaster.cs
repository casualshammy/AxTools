﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace TestPlugin.TerrainCaster
{
    class TerrainCaster : IPlugin
    {

        #region Info

        public string Name
        {
            get { return "TerrainCaster"; }
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
                return "Casts spell on terrain";
            }
        }

        private Image trayIcon;
        public Image TrayIcon
        {
            //get { return trayIcon ?? (trayIcon = new Bitmap(Application.StartupPath + "\\plugins\\Follower\\ability_hunter_posthaste.jpg")); }
            get { return null; }
        }

        public int Interval
        {
            get { return 50; }
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
            locaPlayer = ObjMgr.Pulse(players);
            if (locaPlayer.Health > 0)
            {
                WowPlayer myTarget = players.FirstOrDefault(i => i.Health > 0 && i.GUID == locaPlayer.TargetGUID);
                if (myTarget != null)
                {
                    if (myTarget.Location.Distance(locaPlayer.Location) > 5)
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
        private WoWPlayerMe locaPlayer;
    }
}