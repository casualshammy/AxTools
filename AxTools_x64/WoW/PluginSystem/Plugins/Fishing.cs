using AxTools.Helpers;

using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace AxTools.WoW.PluginSystem.Plugins
{
    internal class Fishing : IPlugin3
    {
        #region Info

        public string Name => "Fishing";

        public Version Version => new Version(1, 0);

        public string Description => "This is a very simple fish bot. It supports baits and special WoD baits; also it makes breaks to avoid being too tireless";

        public Image TrayIcon => AxTools.Helpers.Resources.Plugin_Fishing;

        public bool ConfigAvailable => true;

        public string[] Dependencies => null;

        public bool DontCloseOnWowShutdown => false;

        #endregion Info

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

        public void OnStart(GameInterface inf)
        {
            this.info = inf;
            fishingSettings = this.LoadSettingsJSON<FishingSettings>();
            pluginIsActive = true;
            fishingSpellName = Wowhead.GetSpellInfo(7620).Name;
            this.LogPrint("Fishing spell name: " + fishingSpellName);
            timer = this.CreateTimer(100, inf, OnPulse);
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
            WoWPlayerMe me = info.GetGameObjects(wowObjects);
            WowObject bobberThatBobbing = wowObjects.FirstOrDefault(i => i.OwnerGUID == me.GUID && i.Bobbing);
            if (bobberThatBobbing != null)
            {
                this.LogPrint("Got bit!");
                Thread.Sleep(Utils.Rnd.Next(500, 1000));
                this.LogPrint("Interacting with bobber --> " + bobberThatBobbing.GUID);
                bobberThatBobbing.Interact();
                Thread.Sleep(Utils.Rnd.Next(500, 1000));
            }
            if (!info.IsLooting && !me.InCombat)
            {
                if (me.CastingSpellID == 0 && me.ChannelSpellID == 0)
                {
                    uint baitItemID;
                    if (fishingSettings.EnableBreaks && Utils.Rnd.Next(0, 30) == 0)
                    {
                        int breakTime = Utils.Rnd.Next(15, 45);
                        this.LogPrint($"I'm human! Let's have a break ({breakTime} sec)");
                        int breakStartTime = Environment.TickCount;
                        while ((Environment.TickCount - breakStartTime < breakTime * 1000) && pluginIsActive)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    else if (fishingSettings.UseBestBait && (baitItemID = GetBestBaitID(me)) != 0)
                    {
                        this.LogPrint($"Applying lure --> ({Wowhead.GetItemInfo(baitItemID).Name})");
                        info.UseItemByID(baitItemID);
                        lastTimeLureApplied = DateTime.UtcNow;
                        Thread.Sleep(Utils.Rnd.Next(250, 750));
                    }
                    else if (fishingSettings.UseSpecialBait && (baitItemID = GetSpecialBaitID(me)) != 0)
                    {
                        this.LogPrint($"Applying special lure --> ({Wowhead.GetItemInfo(baitItemID).Name})");
                        info.UseItemByID(baitItemID);
                        Thread.Sleep(Utils.Rnd.Next(250, 750));
                    }
                    else if (fishingSettings.UseArcaneLure && (baitItemID = GetArcaneLure(me)) != 0)
                    {
                        this.LogPrint($"Applying arcane lure --> ({Wowhead.GetItemInfo(baitItemID).Name})");
                        info.UseItemByID(baitItemID);
                        Thread.Sleep(Utils.Rnd.Next(250, 750));
                    }
                    else if (fishingSettings.LegionUseSpecialLure && (baitItemID = GetLegionSpecialLure(me)) != 0)
                    {
                        this.LogPrint($"Applying Legion special lure --> ({Wowhead.GetItemInfo(baitItemID).Name})");
                        info.UseItemByID(baitItemID);
                        Thread.Sleep(Utils.Rnd.Next(250, 750));
                    }
                    else if (fishingSettings.DalaranAchievement && (baitItemID = DalaranAchievement(me)) != 0)
                    {
                        this.LogPrint($"Applying lure for Dalaran achievement --> ({Wowhead.GetItemInfo(baitItemID).Name})");
                        info.UseItemByID(baitItemID);
                        Thread.Sleep(Utils.Rnd.Next(250, 750));
                    }
                    else if (fishingSettings.LegionMargossSupport && GetMargossLure(me) != 0)
                    {
                        Thread.Sleep(Utils.Rnd.Next(1000, 2000));
                    }
                    else
                    {
                        Thread.Sleep(Utils.Rnd.Next(500, 1000));
                        this.LogPrint($"Cast fishing --> ({fishingSpellName})");
                        info.CastSpellByName(fishingSpellName);
                        Thread.Sleep(1500);
                    }
                }
                //else
                //{
                //    WowObject bobber = wowObjects.FirstOrDefault(i => i.OwnerGUID == me.GUID);
                //    if (bobber != null && bobber.Bobbing)
                //    {
                //        this.LogPrint("Got bit!");
                //        Thread.Sleep(Utils.Rnd.Next(500, 1000));
                //        this.LogPrint("Interacting with bobber --> " + bobber.GUID);
                //        bobber.Interact();
                //        Thread.Sleep(Utils.Rnd.Next(500, 1000));
                //    }
                //}
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
                    return item?.EntryID ?? 0;
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
            info.GetGameObjects(null, null, npcs);
            WowNpc natPagle = npcs.FirstOrDefault(npc => npc.EntryID == 85984);
            if (natPagle != null)
            {
                this.LogPrint($"Nat Pagle is found, guid: {natPagle.GUID}, moving to him...");
                info.Move2D(natPagle.Location, 4f, 2000, false, false);
                Thread.Sleep(500);
                this.LogPrint("Opening dialog window with Nat...");
                natPagle.Interact();
                Thread.Sleep(2000);
                info.SelectDialogOption("Обычная приманка для рыбы?"); // todo: is it possible to localize it?
                Thread.Sleep(1500);
                string gossipText = Wowhead.GetItemInfo(specialBaits.First(baitFish => Wowhead.GetItemInfo(baitFish.Key).Name == fishingSettings.SpecialBait).Value).Name;
                info.SelectDialogOption(gossipText);
                Thread.Sleep(1500);
                WowPoint goodFishingPoint = new WowPoint(2024.49f, 191.33f, 83.86f);
                this.LogPrint($"Moving to fishing point [{goodFishingPoint}]");
                info.Move2D(goodFishingPoint, 2f, 2000, false, false);
                WowPoint water = new WowPoint((float)(Utils.Rnd.NextDouble() * 5 + 2032.5f), (float)(Utils.Rnd.NextDouble() * 5 + 208.5f), 82f);
                this.LogPrint($"Facing water [{water}]");
                info.Face(water);
            }
        }

        private uint GetBestBaitID(WoWPlayerMe me)
        {
            //this.LogPrint((me.Inventory.Length > 15) + "::" + me.Inventory[15].EntryID + "::" + fishingRods.Contains(me.Inventory[15].EntryID) + "::" + ((DateTime.UtcNow - lastTimeLureApplied).TotalMinutes > 10));
            if (me.Inventory.Length > 15 && me.Inventory.Any(l => fishingRods.Contains(l.EntryID)) && (DateTime.UtcNow - lastTimeLureApplied).TotalMinutes > 10) //  && info.LuaGetFunctionReturn("tostring(GetWeaponEnchantInfo())") == "false"
            {
                return baits.FirstOrDefault(l => me.ItemsInBags.Any(k => k.EntryID == l));
            }
            return 0;
        }

        private uint GetArcaneLure(WoWPlayerMe me)
        {
            string arcaneLureSpellName = Wowhead.GetSpellInfo(218861).Name;
            if (me.Auras.All(l => l.Name != arcaneLureSpellName))
            {
                WoWItem item = me.ItemsInBags.FirstOrDefault(l => l.Name == arcaneLureSpellName);
                if (item != null)
                {
                    return item.EntryID;
                }
            }
            return 0;
        }

        private uint DalaranAchievement(WoWPlayerMe me)
        {
            if (me.Auras.All(l => !dalaranTradeItems.Select(k => k.AuraID).Contains(l.SpellId) || l.TimeLeftInMs < 20000))
            {
                WoWItem lureInBags = me.ItemsInBags.FirstOrDefault(l => dalaranTradeItems.Select(k => k.ItemID).Contains(l.EntryID));
                if (lureInBags != null)
                {
                    return lureInBags.EntryID;
                }
                this.LogPrint("Searching for Marcia Chase, id:95844");
                List<WowNpc> npcs = new List<WowNpc>();
                info.GetGameObjects(null, null, npcs);
                WowNpc marciaChase = npcs.FirstOrDefault(npc => npc.EntryID == 95844);
                if (marciaChase != null)
                {
                    this.LogPrint($"Marcia Chase is found, guid: {marciaChase.GUID}, interacting...");
                    marciaChase.Interact();
                    Thread.Sleep(Utils.Rnd.Next(750, 1250));
                    this.LogPrint("I: I wish to buy something...");
                    info.SelectDialogOption("Мне бы хотелось купить что-нибудь у вас."); // todo: is it possible to localize it?
                    Thread.Sleep(Utils.Rnd.Next(750, 1250));
                    DalaranTradeItems newLure = dalaranTradeItems[Utils.Rnd.Next(0, dalaranTradeItems.Count)];
                    this.LogPrint("Buying lure " + Wowhead.GetItemInfo(newLure.ItemID).Name + "...");
                    info.BuyMerchantItem(newLure.ItemID, 1);
                    Thread.Sleep(Utils.Rnd.Next(750, 1250));
                    this.LogPrint("Closing dialog window...");
                    info.SendToChat("/run CloseMerchant()");
                    return newLure.ItemID;
                }
                return 0;
            }
            return 0;
        }

        private uint GetLegionSpecialLure(WoWPlayerMe me)
        {
            try
            {
                uint zone = info.ZoneID;
                string[] allBuffNames = legionSpecialLuresByZone.SelectMany(l => l.Lures).Select(l => Wowhead.GetItemInfo(l).Name).ToArray();
                if (me.Auras.All(k => !allBuffNames.Contains(k.Name)))
                {
                    SpecialLuresForZone inf = legionSpecialLuresByZone.FirstOrDefault(l => l.ZoneID == zone);
                    if (inf != null)
                    {
                        WoWItem item = me.ItemsInBags.FirstOrDefault(l => inf.Lures.Contains(l.EntryID));
                        if (item != null)
                        {
                            return item.EntryID;
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                this.LogPrint(ex.Message + "\r\n" + ex.StackTrace);
                return 0;
            }
        }

        private uint GetMargossLure(WoWPlayerMe me)
        {
            LegionRepFarmerStandAtPoint();
            foreach (var itemID in LegionRepSummonItems)
            {
                if (me.ItemsInBags.Any(l => l.EntryID == itemID))
                {
                    this.ShowNotify($"Click this message to use your {Wowhead.GetItemInfo(itemID).Name}", false, false, 2, (obj, args) => { info.UseItemByID(itemID); });
                    break;
                }
            }
            foreach (var itemID in LegionRepCurrencyItems)
            {
                if (me.ItemsInBags.Where(l => l.EntryID == itemID).Sum(l => l.StackSize) >= 100)
                {
                    this.ShowNotify($"You cannot get more {Wowhead.GetItemInfo(itemID).Name}", false, true);
                    return 1;
                }
            }
            return 0;
        }

        private void LegionRepFarmerStandAtPoint()
        {
            if (LegionRepZones.TryGetValue(info.ZoneID, out LegionRepPoint value))
            {
                WoWPlayerMe me = info.GetGameObjects();
                int timeout = 5000;
                while (timeout > 0 && me.Location.Distance2D(value.StartingPlayerPoint) > 3)
                {
                    info.Move2D(value.StartingPlayerPoint, 3f, 1000, true, false);
                    timeout -= 1000;
                }
                info.Face(value.StartingFacingPoint);
            }
        }

        #endregion Events

        #region Fields, propeties

        private FishingSettings fishingSettings;
        private SafeTimer timer;
        private bool pluginIsActive;
        private string fishingSpellName;
        private DateTime lastTimeLureApplied = DateTime.MinValue;
        private GameInterface info;

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

        private readonly List<DalaranTradeItems> dalaranTradeItems = new List<DalaranTradeItems>
        {
            new DalaranTradeItems {ItemID = 138956, AuraID = 217835},
            new DalaranTradeItems {ItemID = 138957, AuraID = 217836},
            new DalaranTradeItems {ItemID = 138958, AuraID = 217837},
            new DalaranTradeItems {ItemID = 138959, AuraID = 217838},
            new DalaranTradeItems {ItemID = 138960, AuraID = 217839},
            new DalaranTradeItems {ItemID = 138961, AuraID = 217840},
            new DalaranTradeItems {ItemID = 138962, AuraID = 217842},
            new DalaranTradeItems {ItemID = 138963, AuraID = 217844}
        };

        private readonly List<SpecialLuresForZone> legionSpecialLuresByZone = new List<SpecialLuresForZone>
        {
            new SpecialLuresForZone {ZoneID = 7731, Lures = new uint[] {133710, 133711, 133712}},   // Громовой Тотем
            new SpecialLuresForZone {ZoneID = 7503, Lures = new uint[] {133710, 133711, 133712}}    // Крутогорье
        };

        private readonly uint[] LegionRepSummonItems = {
            141975, // Mark of Aquaos
            146966, // Фигурка тотема воды
            146967, // Белая сверкающая безделушка
            146969, // Слабо пульсирующий камень Скверны
            146964, // Наконечник копья клана Колец Ненависти
            146965, // Отвратительный слизнюк
        };

        private readonly uint[] LegionRepCurrencyItems = {
            138777, // Утопленная мана
            146960, // Фрагмент древнего тотема
            146961, // Блестящая побрякушка
            146848, // Расколотые чары
            146963, // Нечистая водоросль
            146962, // Золотистая рыбешка
            146959, // Оскверненная капля
        };

        private readonly Dictionary<uint, LegionRepPoint> LegionRepZones = new Dictionary<uint, LegionRepPoint> {
            { 7543, new LegionRepPoint { StartingPlayerPoint = new WowPoint(-1286.44f, 3622.32f, 0.26f), StartingFacingPoint = new WowPoint(-1299.87f, 3619.78f, -2.54f)} }, // Расколотый берег
            { 7334, new LegionRepPoint { StartingPlayerPoint = new WowPoint(29.59f, 6975.71f, 21.33f), StartingFacingPoint = new WowPoint(36.61f, 6987.4f, 20.66f) } }, // Азсуна
            { 7558, new LegionRepPoint { StartingPlayerPoint = new WowPoint(2314.55f, 6668.39f, 133.63f), StartingFacingPoint = new WowPoint(2301.23f, 6671.69f, 129.85f)} }, // Вальшара
        };

        #endregion Fields, propeties

        #region Classes

        private class SpecialLuresForZone
        {
            internal uint ZoneID;
            internal uint[] Lures;
        }

        private class DalaranTradeItems
        {
            internal uint ItemID;
            internal int AuraID;
        }

        private class LegionRepPoint
        {
            internal WowPoint StartingPlayerPoint;
            internal WowPoint StartingFacingPoint;
        }

        #endregion Classes
    }
}