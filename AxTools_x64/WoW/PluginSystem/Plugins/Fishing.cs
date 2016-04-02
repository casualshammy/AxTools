using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace AxTools.WoW.PluginSystem.Plugins
{
    internal class Fishing : IPlugin
    {

        #region Info

        public string Name
        {
            get { return "Fishing"; }
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
            get { return "This is a very simple fish bot. It supports Nat's Hat and Weather-Beaten Fishing Hat if equipped"; }
        }

        public Image TrayIcon { get { return Resources.trade_fishing; } }

        public string WowIcon
        {
            get { return "Interface\\\\Icons\\\\trade_fishing"; }
        }

        public bool ConfigAvailable
        {
            get { return true; }
        }

        #endregion

        #region Events

        public void OnConfig()
        {
            if (fishingSettings == null)
            {
                fishingSettings = this.LoadSettingsJSON<FishingSettings>();
            }
            FishingConfig.Open(fishingSettings);
            this.SaveSettingsJSON(fishingSettings);
        }

        public void OnStart()
        {
            fishingSettings = this.LoadSettingsJSON<FishingSettings>();
            pluginIsActive = true;
            fishingSpellName = Wowhead.GetSpellInfo(7620).Name;
            this.LogPrint("Fishing spell name: " + fishingSpellName);
            timer = this.CreateTimer(100, OnPulse);
            timer.Start();
        }

        public void OnStop()
        {
            pluginIsActive = false;
            timer.Dispose();
        }

        private void OnPulse()
        {
            List<WowObject> wowObjects = new List<WowObject>();
            WoWPlayerMe me = ObjectMgr.Pulse(wowObjects);
            if (!GameFunctions.IsLooting && !me.InCombat)
            {
                if (me.CastingSpellID == 0 && me.ChannelSpellID == 0)
                {
                    if (Utils.Rnd.Next(0, 25) == 0)
                    {
                        int breakTime = Utils.Rnd.Next(15, 45);
                        this.LogPrint(string.Format("I'm human! Let's have a break ({0} sec)", breakTime));
                        int breakStartTime = Environment.TickCount;
                        while ((Environment.TickCount - breakStartTime < breakTime*1000) && pluginIsActive)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    else if (fishingSettings.UseBestBait)
                    {
                        uint baitID = GetBestBaitID(me);
                        if (baitID != 0)
                        {
                            this.LogPrint("Applying lure...");
                            GameFunctions.UseItemByID(baitID);
                            lastTimeLureApplied = DateTime.UtcNow;
                            Thread.Sleep(Utils.Rnd.Next(250, 750));
                        }
                    }
                    else if (fishingSettings.UseSpecialBait)
                    {
                        uint specialBaitID = GetSpecialBaitID(me);
                        if (specialBaitID != 0)
                        {
                            this.LogPrint("Applying special lure...");
                            GameFunctions.UseItemByID(specialBaitID);
                            Thread.Sleep(Utils.Rnd.Next(250, 750));
                        }
                    }
                    else
                    {
                        Thread.Sleep(Utils.Rnd.Next(500, 1000));
                        this.LogPrint("Cast fishing...");
                        GameFunctions.CastSpellByName(fishingSpellName);
                        Thread.Sleep(1500);
                    }
                }
                else
                {
                    WowObject bobber = wowObjects.FirstOrDefault(i => i.OwnerGUID == me.GUID);
                    if (bobber != null && bobber.Bobbing)
                    {
                        this.LogPrint("Got bit!");
                        Thread.Sleep(Utils.Rnd.Next(500, 1000));
                        this.LogPrint("Interacting with bobber --> " + bobber.GUID);
                        bobber.Interact();
                        Thread.Sleep(Utils.Rnd.Next(500, 1000));
                    }
                }
            }
        }

        private uint GetSpecialBaitID(WoWPlayerMe me)
        {
            if (me.Auras.All(l => l.Name != fishingSettings.SpecialBait))
            {
                WoWItem item = me.ItemsInBags.FirstOrDefault(l => l.Name == fishingSettings.SpecialBait);
                return item != null ? item.EntryID : 0;
            }
            return 0;
        }

        private uint GetBestBaitID(WoWPlayerMe me)
        {
            if (fishingRods.Contains(me.Inventory[15].EntryID) && (DateTime.UtcNow - lastTimeLureApplied).TotalMinutes > 10)
            {
                return baits.FirstOrDefault(l => me.ItemsInBags.Any(k => k.EntryID == l));
            }
            return 0;
        }

        #endregion

        #region Fields, propeties

        private FishingSettings fishingSettings;
        private SingleThreadTimer timer;
        private bool pluginIsActive;
        private string fishingSpellName;
        private DateTime lastTimeLureApplied = DateTime.MinValue;

        private readonly uint[] fishingRods =
        {
            44050, // Мастерски сделанная калуакская удочка
            25978, // Графитовая удочка Сета
            19022, // Продвинутая рыбалка Ната Пэгла FC-5000
            6367, // Большая железная удочка
            6366, // Удочка из темной древесины
            120163, // Удочка Тукра
            45858, // Счастливая удочка Ната
            19970, // Арканитовая удочка
            84661, // Счастливая удочка драконов
            45991, // Костяная удочка
            118381, // Эфемерная удочка
            45992, // Удочка со стразами
            46337, // Удочка Стаатса
            12225, // Удочка семейства Блумп
            6365, // Крепкая удочка
            116826, // Дренейская удочка
            84660, // Пандаренская удочка
            116825, // Яростная удочка
            6256, // Удочка
        };

        private readonly uint[] baits =
        {
            118391, // Королевский червяк
            68049,  // Термостойкая вращающаяся наживка
            34861,  // Заостренный рыболовный крючок
            6529,   // Блесна
        };

        private readonly uint[] specialWodBaits =
        {
            110293,
            110291,
            110289,
            110290,
            110292,
            110274,
            110294,
        };

        #endregion

    }
}
