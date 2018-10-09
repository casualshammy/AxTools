using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace WoWPlugin_CombatRoutine
{
    public class CombatRoutine : IPlugin3
    {
        #region Info

        public string Name => nameof(CombatRoutine);

        public Version Version => new Version(1, 0);

        public string Description => "Simple combat routines for all classes";

        public Image TrayIcon => null;

        public bool ConfigAvailable => false;

        public string[] Dependencies => null;

        public bool DontCloseOnWowShutdown => false;

        #endregion Info

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
                if (specialization == 0)
                {
                    specialization = int.Parse(game.LuaGetValue("GetSpecialization()"));
                }
                WowNpc nearestNpc = npcs.FirstOrDefault(l => l.GUID == me.TargetGUID);
                if (nearestNpc == null)
                {
                    game.SendToChat("/targetenemy");
                }
                me = game.GetGameObjects(null, null, npcs);
                nearestNpc = npcs.FirstOrDefault(l => l.GUID == me.TargetGUID);
                if (nearestNpc != null)
                {
                    game.Face(nearestNpc.Location);
                    if (me.Class == WowPlayerClass.Sha)
                    {
                        ShamanRoutine(me);
                    }
                }
            }
            else
            {
                specialization = 0;
            }
        }

        private void ShamanRoutine(WoWPlayerMe me)
        {
            var myEditbox = WoWUIFrame.GetFrameByName(game, frameName);
            //if (myEditbox == null)
            //{
            //    var funcName = Utilities.GetRandomString(6, true);
            //    var commands = new string[] {
            //        $"/run if(not {frameName})then CreateFrame(\"EditBox\", \"{frameName}\", UIParent);{frameName}:SetAutoFocus(false);{frameName}:ClearFocus(); end",
            //        $"/run {funcName}=\"{frameName}:SetText(tostring(GetSpellCooldown(\\\"Удар бури\\\"))..\\\"#\\\"..tostring(GetSpellCooldown(\\\"Камнедробитель\\\"))\"",
            //        $"/run {funcName}={funcName}..\"..\\\"#\\\"..tostring(GetSpellCooldown(\\\"Язык пламени\\\"))..\\\"#\\\"..tostring(UnitPower(\\\"player\\\", 11)))\"",
            //        $"/run C_Timer.NewTicker(.25, function() loadstring({funcName}); end)"
            //    };
            //    foreach (var command in commands)
            //        game.SendToChat(command);
            //}
            
            if (specialization == 1)
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
            else if (specialization == 2)
            {
                myEditbox = WoWUIFrame.GetFrameByName(game, frameName);

                var regex = new Regex("(\\d+)#(\\d+)#(\\d+)#(\\d+)");
                this.LogPrint(myEditbox.EditboxText);
                var match = regex.Match(myEditbox.EditboxText);
                var stormstrikeHasCd = match.Groups[1].Value != "0";
                var maelstrom = int.Parse(match.Groups[4].Value);
                var rockbiterHasCd = match.Groups[2].Value != "0";
                var flameTongueHasCd = match.Groups[3].Value != "0";
                if (!stormstrikeHasCd && maelstrom >= 30)
                {
                    game.CastSpellByName("Удар бури");
                    return;
                }
                if (!rockbiterHasCd)
                {
                    game.CastSpellByName("Камнедробитель");
                    return;
                }
                if (!flameTongueHasCd)
                {
                    game.CastSpellByName("Язык пламени");
                    return;
                }
                if (maelstrom >= 40)
                {
                    game.CastSpellByName("Вскипание лавы");
                }
            }
        }

        #endregion Events

        #region Fields, propeties

        private readonly List<WowNpc> npcs = new List<WowNpc>();
        private SafeTimer timer;
        private GameInterface game;
        private string frameName = "Knvskvkvbe";//Utilities.GetRandomString(8, true);
        private int specialization = 0;

        #endregion Fields, propeties
    }
}