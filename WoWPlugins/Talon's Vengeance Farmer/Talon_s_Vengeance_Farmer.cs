﻿using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Talon_s_Vengeance_Farmer
{
    public class TalonsVengeanceFarmer : IPlugin3
    {
        #region Info

        public string Name => "Talon's Vengeance Farmer";

        public Version Version => new Version(1, 0);

        public string Description => "";

        public Image TrayIcon => null;

        public bool ConfigAvailable => false;

        public string[] Dependencies => null;

        public bool DontCloseOnWowShutdown => false;

        #endregion Info

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
            WoWPlayerMe me = game.GetGameObjects(null, players);
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

        public void OnStart(GameInterface game)
        {
            throw new NotImplementedException();
        }

        #endregion Events

        #region Fields, propeties

        private readonly List<WowPlayer> players = new List<WowPlayer>();
        private bool IsMaster = true;
        private SafeTimer timer;

        #endregion Fields, propeties
    }
}