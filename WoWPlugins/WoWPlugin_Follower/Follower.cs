﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace Follower
{
    internal class Follower : IPlugin
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

        public string WowIcon
        {
            get { return string.Empty; }
        }

        public bool ConfigAvailable
        {
            get { return true; }
        }

        #endregion

        #region Events

        public void OnConfig()
        {
            if (SettingsInstance == null)
            {
                SettingsInstance = this.LoadSettingsJSON<Settings>();
            }
            new SettingsForm(SettingsInstance).ShowDialog();
            this.SaveSettingsJSON(SettingsInstance);
        }

        public void OnStart()
        {
            SettingsInstance = this.LoadSettingsJSON<Settings>();
            WoWPlayerMe locaPlayer = ObjMgr.Pulse(null, players, npcs);
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
                this.LogPrint("Unit isn't found!");
            }
            libNavigator = Utilities.GetReferenceOfPlugin("LibNavigator");
            if (libNavigator == null)
            {
                this.ShowNotify("LibNavigator isn't found! Plugin will not work.", true, true);
                this.LogPrint("LibNavigator isn't found! Plugin will not work.");
                return;
            }
            (timer = this.CreateTimer(500, OnPulse)).Start();
        }

        public void OnPulse()
        {
            WoWPlayerMe locaPlayer = ObjMgr.Pulse(null, players, npcs);
            if (locaPlayer.Alive)
            {
                WowPlayer unitPlayer = players.FirstOrDefault(i => i.Alive && i.GUID == guid);
                WowNpc unitNpc = npcs.FirstOrDefault(i => i.Alive && i.GUID == guid);
                WowPoint? POILocation = unitPlayer != null ? unitPlayer.Location : unitNpc?.Location;
                if (POILocation.HasValue && POILocation.Value.Distance(locaPlayer.Location) > SettingsInstance.MaxDistance)
                {
                    // GameFunctions.Move2D(WowPoint.GetNearestPoint(locaPlayer.Location, POILocation.Value, 1f), SettingsInstance.Precision, 1000, true, false);
                    libNavigator.Go(WowPoint.GetNearestPoint(locaPlayer.Location, POILocation.Value, 1f), (float)SettingsInstance.Precision);
                }
            }
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        #endregion

        private readonly List<WowPlayer> players = new List<WowPlayer>();
        private readonly List<WowNpc> npcs = new List<WowNpc>();
        private WoWGUID guid;
        private SafeTimer timer;
        private Settings SettingsInstance;
        private dynamic libNavigator;

    }
}
