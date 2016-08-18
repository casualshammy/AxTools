using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_GarrisonFishingHelper
{
    public class GarrisonFishingHelper : IPlugin
    {

        #region Properties

        public string Name
        {
            get { return "GarrisonFishingHelper"; }
        }

        public Version Version
        {
            get { return new Version(1, 0); }
        }

        public string Description
        {
            get { return "Uses coins"; }
        }

        private Image trayIcon;
        public Image TrayIcon { get { return trayIcon ?? (trayIcon = Image.FromFile(string.Format("{0}\\plugins\\{1}\\Inv_fishingchair.png", Application.StartupPath, Name))); } }

        public string WowIcon
        {
            get { return null; }
        }

        public bool ConfigAvailable
        {
            get { return false; }
        }

        #endregion

        #region Methods

        public void OnConfig()
        {
            
        }

        public void OnStart()
        {
            (timer = this.CreateTimer(1000, OnElapsed)).Start();
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        private void OnElapsed()
        {
            WoWPlayerMe me = ObjMgr.Pulse(wowNpcs);
            if (me != null)
            {
                if (!me.InCombat)
                {
                    WowNpc lootableCorpse = wowNpcs.FirstOrDefault(k => (k.Name == "Обитатель пещер Зашедшей Луны" || k.Name == "Обитатель ледяных пещер") && k.Lootable && !k.Alive);
                    if (lootableCorpse != null)
                    {
                        if (!GameFunctions.IsLooting)
                        {
                            lootableCorpse.Interact();
                            this.LogPrint("Looting --> " + lootableCorpse.GUID);
                        }
                    }
                    else if (me.ItemsInBags.FirstOrDefault(l => l.EntryID == 116158) != null)
                    {
                        if (me.CastingSpellID == 0 && me.ChannelSpellID == 0)
                        {
                            Utilities.RequestStopPlugin("Fishing");
                            this.LogPrint("Fishing is paused");
                            Thread.Sleep(rnd.Next(2000, 3000));
                            GameFunctions.UseItemByID(116158);
                            this.LogPrint("Green fish is throwed");
                            Thread.Sleep(rnd.Next(1000, 2000));
                            _isworking = true;
                        }
                    }
                    else
                    {
                        Thread.Sleep(rnd.Next(500, 1500));
                        Utilities.RequestStartPlugin("Fishing");
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
                            //GameFunctions.CastSpellByName(killingSpellName);
                            CombatRotation();
                            this.LogPrint("Killing enemy --> " + enemy.GUID);
                        }
                    }
                }
            }
        }

        private void CombatRotation()
        {
            if (GameFunctions.LuaGetFunctionReturn("tostring(UnitDebuff(\"target\", \"Огненный шок\") or \"nil\")") == "nil")
            {
                GameFunctions.CastSpellByName("Огненный шок");
            }
            else if (GameFunctions.LuaGetFunctionReturn("tostring(GetSpellCooldown(\"Выброс лавы\"))") == "0")
            {
                GameFunctions.CastSpellByName("Выброс лавы");
            }
            else
            {
                GameFunctions.CastSpellByName("Молния");
            }
        }

        #endregion

        #region Variables

        private SafeTimer timer;
        private readonly string killingSpellName = "Молния";
        private readonly List<WowNpc> wowNpcs = new List<WowNpc>();
        private readonly Random rnd = new Random();
        private static bool _isworking;

        #endregion
    
    }
}
