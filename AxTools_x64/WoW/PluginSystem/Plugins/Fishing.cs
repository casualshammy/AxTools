using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;

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

        public string Description
        {
            get { return "This is a very simple fish bot. It supports baits and special WoD baits; also it makes breaks to avoid being too tireless"; }
        }

        public Image TrayIcon { get { return Resources.trade_fishing; } }

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
                    uint baitItemID;
                    if (Utils.Rnd.Next(0, 30) == 0)
                    {
                        int breakTime = Utils.Rnd.Next(15, 45);
                        this.LogPrint(string.Format("I'm human! Let's have a break ({0} sec)", breakTime));
                        int breakStartTime = Environment.TickCount;
                        while ((Environment.TickCount - breakStartTime < breakTime*1000) && pluginIsActive)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    else if (fishingSettings.UseBestBait && (baitItemID = GetBestBaitID(me)) != 0)
                    {
                        this.LogPrint(string.Format("Applying lure --> ({0})", Wowhead.GetItemInfo(baitItemID).Name));
                        GameFunctions.UseItemByID(baitItemID);
                        lastTimeLureApplied = DateTime.UtcNow;
                        Thread.Sleep(Utils.Rnd.Next(250, 750));
                    }
                    else if (fishingSettings.UseSpecialBait && (baitItemID = GetSpecialBaitID(me)) != 0)
                    {
                        this.LogPrint(string.Format("Applying special lure --> ({0})", Wowhead.GetItemInfo(baitItemID).Name));
                        GameFunctions.UseItemByID(baitItemID);
                        Thread.Sleep(Utils.Rnd.Next(250, 750));
                    }
                    else
                    {
                        Thread.Sleep(Utils.Rnd.Next(500, 1000));
                        this.LogPrint(string.Format("Cast fishing --> ({0})", fishingSpellName));
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
            try
            {
                if (me.Auras.All(l => l.Name != fishingSettings.SpecialBait))
                {
                    WoWItem item = me.ItemsInBags.FirstOrDefault(l => l.Name == fishingSettings.SpecialBait);
                    if (item == null)
                    {
                        if (fishingSettings.GetSpecialBaitFromNatPagle)
                        {
                            GetSpecialBaitBuffFromNatPagle();
                        }
                        else if (fishingSettings.UseAnySpecialBaitIfPreferredIsNotAvailable)
                        {
                            bool haveBuff = me.Auras.Any(aura => specialBaits.Keys.Select(baitID => Wowhead.GetItemInfo(baitID).Name).Contains(aura.Name));
                            if (!haveBuff)
                            {
                                return specialBaits.Keys.FirstOrDefault(itemID => me.ItemsInBags.Select(itemInBag => itemInBag.EntryID).Contains(itemID));
                            }
                        }
                    }
                    return item != null ? item.EntryID : 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                this.LogPrint(ex.Message + "\r\n" + ex.StackTrace);
                return 0;
            }
        }

        private void GetSpecialBaitBuffFromNatPagle()
        {
            this.LogPrint("Searching for Nat Pagle, id:85984");
            List<WowNpc> npcs = new List<WowNpc>();
            ObjMgr.Pulse(npcs);
            WowNpc natPagle = npcs.FirstOrDefault(npc => npc.EntryID == 85984);
            if (natPagle != null)
            {
                this.LogPrint(string.Format("Nat Pagle is found, guid: {0}, moving to him...", natPagle.GUID));
                GameFunctions.Move2D(natPagle.Location, 4f, 2000, false, false);
                Thread.Sleep(500);
                this.LogPrint("Opening dialog window with Nat...");
                natPagle.Interact();
                Thread.Sleep(2000);
                GameFunctions.SelectDialogOption("Обычная приманка для рыбы?"); // todo: is it possible to localize it?
                Thread.Sleep(1500);
                string gossipText = Wowhead.GetItemInfo(specialBaits.First(baitFish => Wowhead.GetItemInfo(baitFish.Key).Name == fishingSettings.SpecialBait).Value).Name;
                GameFunctions.SelectDialogOption(gossipText);
                Thread.Sleep(1500);
                WowPoint goodFishingPoint = new WowPoint(2024.49f, 191.33f, 83.86f);
                this.LogPrint(string.Format("Moving to fishing point [{0}]", goodFishingPoint));
                GameFunctions.Move2D(goodFishingPoint, 2f, 2000, false, false);
                WowPoint water = new WowPoint((float) (Utils.Rnd.NextDouble()*5 + 2032.5f), (float) (Utils.Rnd.NextDouble()*5 + 208.5f), 82f);
                this.LogPrint(string.Format("Facing water [{0}]", water));
                water.Face();
            }
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
        private SafeTimer timer;
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

        private readonly Dictionary<uint, uint> specialBaits = new Dictionary<uint, uint>
        {
            {110293, 111664},
            {110291, 111666},
            {110289, 111668},
            {110290, 111667},
            {110274, 111669},
            {110294, 111663},
            {110292, 111665}, // Морской скорпион
        };

        #endregion

    }
}
