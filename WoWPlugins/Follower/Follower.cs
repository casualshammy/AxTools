using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Follower
{
    internal class Follower : IPlugin3
    {
        #region Info

        public string Name => "Follower";

        public Version Version => new Version(1, 0);

        public string Author => "CasualShammy";

        public string Description => "Follows target";

        private Image trayIcon;

        public Image TrayIcon => trayIcon ?? (trayIcon = new Bitmap($"{this.GetPluginSourceFolder()}\\ability_hunter_posthaste.jpg"));

        public string WowIcon => string.Empty;

        public bool ConfigAvailable => true;

        public string[] Dependencies => new[] { "LibNavigator" };

        public bool DontCloseOnWowShutdown => false;

        #endregion Info

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

        public void OnStart(GameInterface game)
        {
            this.game = game;
            SettingsInstance = this.LoadSettingsJSON<Settings>();
            WoWPlayerMe locaPlayer = game.GetGameObjects(null, players, npcs);
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
            (timer = this.CreateTimer(500, game, OnPulse)).Start();
        }

        public void OnPulse()
        {
            WoWPlayerMe locaPlayer = game.GetGameObjects(null, players, npcs);
            if (locaPlayer.Alive)
            {
                WowPlayer unitPlayer = players.FirstOrDefault(i => i.Alive && i.GUID == guid);
                WowNpc unitNpc = npcs.FirstOrDefault(i => i.Alive && i.GUID == guid);
                WowPoint? POILocation = unitPlayer != null ? unitPlayer.Location : unitNpc?.Location;
                if (POILocation.HasValue && POILocation.Value.Distance(locaPlayer.Location) > SettingsInstance.MaxDistance)
                {
                    // game.Move2D(WowPoint.GetNearestPoint(locaPlayer.Location, POILocation.Value, 1f), SettingsInstance.Precision, 1000, true, false);
                    libNavigator.Go(WowPoint.GetNearestPoint(locaPlayer.Location, POILocation.Value, 1f), (float)SettingsInstance.Precision, game);
                }
            }
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        #endregion Events

        private readonly List<WowPlayer> players = new List<WowPlayer>();
        private readonly List<WowNpc> npcs = new List<WowNpc>();
        private WoWGUID guid;
        private SafeTimer timer;
        private Settings SettingsInstance;
        private dynamic libNavigator;
        private GameInterface game;
    }
}