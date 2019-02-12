using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace WoWPlugin_GarrisonFishingHelper
{
    public class GarrisonFishingHelper : IPlugin3
    {
        #region Properties

        public string Name => "GarrisonFishingHelper";

        public Version Version => new Version(1, 0);

        public string Description => "Uses coins";

        private Image trayIcon;
        public Image TrayIcon => trayIcon ?? (trayIcon = Image.FromFile($"{this.GetPluginSourceFolder()}\\Inv_fishingchair.png"));

        public string WowIcon => null;

        public bool ConfigAvailable => false;

        public string[] Dependencies => null;

        public bool DontCloseOnWowShutdown => false;

        #endregion Properties

        #region Methods

        public void OnConfig()
        {
        }

        public void OnStart(GameInterface game)
        {
            this.game = game;
            (timer = this.CreateTimer(1000, game, OnElapsed)).Start();
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        private void OnElapsed()
        {
            WoWPlayerMe me = game.GetGameObjects(null, null, wowNpcs);
            if (me != null)
            {
                if (!me.InCombat)
                {
                    WowNpc lootableCorpse = wowNpcs.FirstOrDefault(k => (k.Name == "Обитатель пещер Зашедшей Луны" || k.Name == "Обитатель ледяных пещер") && !k.Alive && k.IsLootable_Lua());
                    if (lootableCorpse != null)
                    {
                        if (!game.IsLooting)
                        {
                            lootableCorpse.Interact();
                            this.LogPrint("Looting --> " + lootableCorpse.GUID);
                        }
                    }
                    else if (me.ItemsInBags.FirstOrDefault(l => l.EntryID == 116158) != null)
                    {
                        if (me.CastingSpellID == 0 && me.ChannelSpellID == 0)
                        {
                            Utilities.RemovePluginFromRunning("Fishing");
                            this.LogPrint("Fishing is paused");
                            Thread.Sleep(rnd.Next(2000, 3000));
                            game.UseItemByID(116158);
                            this.LogPrint("Green fish is throwed");
                            Thread.Sleep(rnd.Next(1000, 2000));
                            _isworking = true;
                        }
                    }
                    else
                    {
                        Thread.Sleep(rnd.Next(500, 1500));
                        Utilities.AddPluginToRunning(this, "Fishing");
                        if (_isworking)
                        {
                            this.LogPrint("Fishing is resumed");
                        }
                        _isworking = false;
                    }
                }
                else
                {
                    WowNpc enemy = wowNpcs.FirstOrDefault(k => (k.Name == "Обитатель пещер Зашедшей Луны" || k.Name == "Обитатель ледяных пещер") && k.Alive);
                    if (enemy != null)
                    {
                        if (enemy.GUID != me.TargetGUID)
                        {
                            enemy.Target();
                            this.LogPrint("Target enemy --> " + enemy.GUID);
                        }
                        else
                        {
                            //game.CastSpellByName(killingSpellName);
                            CombatRotation();
                            this.LogPrint("Killing enemy --> " + enemy.GUID);
                        }
                    }
                }
            }
        }

        private void CombatRotation()
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

        #endregion Methods

        #region Variables

        private SafeTimer timer;
        private readonly string killingSpellName = "Молния";
        private readonly List<WowNpc> wowNpcs = new List<WowNpc>();
        private readonly Random rnd = new Random();
        private static bool _isworking;
        private GameInterface game;

        #endregion Variables
    }
}