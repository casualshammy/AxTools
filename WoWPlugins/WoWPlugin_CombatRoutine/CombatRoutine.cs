using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_CombatRoutine
{
    public class CombatRoutine : IPlugin2
    {

        #region Info

        public string Name
        {
            get { return "CombatRoutine"; }
        }

        public Version Version
        {
            get { return new Version(1, 0); }
        }

        public string Description
        {
            get { return "Simple combat routines for all classes"; }
        }

        public Image TrayIcon { get { return null; } }

        public bool ConfigAvailable
        {
            get { return false; }
        }

        public string[] Dependencies => null;

        #endregion

        #region Events

        public void OnConfig()
        {

        }

        public void OnStart(GameInterface game)
        {
            this.game = game;
            timer = this.CreateTimer(100, game, OnPulse);
            timer.Start();
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        private void OnPulse()
        {
            WoWPlayerMe me = game.GetGameObjects(null, null, npcs);
            if (me != null && me.InCombat)
            {
                WowNpc nearestNpc = npcs.FirstOrDefault(l => l.GUID == me.TargetGUID);
                if (nearestNpc == null)
                {
                    game.SendToChat("/targetenemy");
                }
                if (me.Class == WowPlayerClass.Sha)
                {
                    ShamanRoutine(me);
                }
            }
        }

        private void ShamanRoutine(WoWPlayerMe me)
        {
            if (game.LuaGetValue("tostring(UnitDebuff(\"target\", \"Огненный шок\") or \"nil\")") == "nil")
            {
                game.CastSpellByName("Огненный шок");
            }
            else if (float.Parse(game.LuaGetValue("tostring(select(2, GetSpellCooldown(\"Выброс лавы\")))"), CultureInfo.InvariantCulture) <= 1.5)
            {
                game.CastSpellByName("Выброс лавы");
            }
            else
            {
                game.CastSpellByName("Молния");
            }
        }

        #endregion

        #region Fields, propeties

        private readonly List<WowNpc> npcs = new List<WowNpc>();
        private SafeTimer timer;
        private GameInterface game;

        #endregion

    }
}
