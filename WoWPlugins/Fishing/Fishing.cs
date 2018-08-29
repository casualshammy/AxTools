using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fishing
{
    public class Fishing : IPlugin3
    {

        #region IPlugin3 game

        public bool ConfigAvailable => true;

        public string[] Dependencies => null;

        public string Description => "This is a very simple fish bot. It supports baits and special WoD baits; also it makes breaks to avoid being too tireless";

        public bool DontCloseOnWowShutdown => false;

        public string Name => nameof(Fishing);

        public System.Drawing.Image TrayIcon => Resources.Plugin_Fishing;

        public Version Version => new Version(1, 0);

        #endregion IPlugin3 game

        #region Variables

        private Settings settings;
        private GameInterface game;
        private bool pluginIsActive;
        private SafeTimer timer;
        private DateTime lastTimeLureApplied;
        private readonly string FISHING_SPELL_NAME = Wowhead.GetSpellInfo(7620).Name;
        private readonly string WATER_WALKING_NAME = Wowhead.GetSpellInfo(546).Name; // Water Walking https://wowhead.com/spell=546
        private readonly uint[] FISHING_RODS =
        {
            133755, // Удочка Темносвета
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
        private readonly uint[] BAITS =
        {
            // order is important: better is first
            118391, // Королевский червяк
            68049,  // Термостойкая вращающаяся наживка
            34861,  // Заостренный рыболовный крючок
            6529,   // Блесна
        };
        private readonly Dictionary<uint, uint> SPECIAL_BAITS = new Dictionary<uint, uint>
        {
            {110293, 111664},
            {110291, 111666},
            {110289, 111668},
            {110290, 111667},
            {110274, 111669},
            {110294, 111663},
            {110292, 111665}, // Морской скорпион
        };
        private readonly List<LegionSpecialLuresForZone> LEGION_SPECIAL_LURES_BY_ZONE = new List<LegionSpecialLuresForZone>
        {
            new LegionSpecialLuresForZone {ZoneID = 7731, Lures = new uint[] {133710, 133711, 133712}},   // Громовой Тотем
            new LegionSpecialLuresForZone {ZoneID = 7503, Lures = new uint[] {133710, 133711, 133712}}    // Крутогорье
        };
        private readonly List<DalaranAchivementTradeItems> DALARAN_ACHIVEMENT_TRADE_ITEMS = new List<DalaranAchivementTradeItems>
        {
            new DalaranAchivementTradeItems {ItemID = 138956, AuraID = 217835},
            new DalaranAchivementTradeItems {ItemID = 138957, AuraID = 217836},
            new DalaranAchivementTradeItems {ItemID = 138958, AuraID = 217837},
            new DalaranAchivementTradeItems {ItemID = 138959, AuraID = 217838},
            new DalaranAchivementTradeItems {ItemID = 138960, AuraID = 217839},
            new DalaranAchivementTradeItems {ItemID = 138961, AuraID = 217840},
            new DalaranAchivementTradeItems {ItemID = 138962, AuraID = 217842},
            new DalaranAchivementTradeItems {ItemID = 138963, AuraID = 217844}
        };
        private readonly Dictionary<uint, LegionRepPoint> LEGION_REP_ZONES = new Dictionary<uint, LegionRepPoint> {
            { 7543, new LegionRepPoint { StartingPlayerPoint = new WowPoint(-1286.44f, 3622.32f, 0.26f), StartingFacingPoint = new WowPoint(-1299.87f, 3619.78f, -2.54f)} }, // Расколотый берег
            { 7334, new LegionRepPoint { StartingPlayerPoint = new WowPoint(29.59f, 6975.71f, 21.33f), StartingFacingPoint = new WowPoint(36.61f, 6987.4f, 20.66f) } }, // Азсуна
            { 7558, new LegionRepPoint { StartingPlayerPoint = new WowPoint(2314.55f, 6668.39f, 133.63f), StartingFacingPoint = new WowPoint(2301.23f, 6671.69f, 129.85f)} }, // Вальшара
        };
        private readonly uint[] LEGION_REP_SUMMON_ITEMS = {
            141975, // Mark of Aquaos
            146966, // Фигурка тотема воды
            146967, // Белая сверкающая безделушка
            146969, // Слабо пульсирующий камень Скверны
            146964, // Наконечник копья клана Колец Ненависти
            146965, // Отвратительный слизнюк
        };
        private readonly uint[] LEGION_REP_CURRENCY_ITEMS = {
            138777, // Утопленная мана
            146960, // Фрагмент древнего тотема
            146961, // Блестящая побрякушка
            146848, // Расколотые чары
            146963, // Нечистая водоросль
            146962, // Золотистая рыбешка
            146959, // Оскверненная капля
        };
        
        #endregion Variables

        public void OnConfig()
        {
            settings = settings ?? this.LoadSettingsJSON<Settings>();
            using (var optionsWindow = new OptionsWindow(settings))
            {
                optionsWindow.ShowDialog();
                this.SaveSettingsJSON(settings);
            }  
        }

        public void OnStart(GameInterface game)
        {
            this.game = game;
            settings = this.LoadSettingsJSON<Settings>();
            pluginIsActive = true;
            this.LogPrint("Fishing spell name: " + FISHING_SPELL_NAME);
            timer = this.CreateTimer(100, game, Timer_OnElapsed);
            timer.Start();
        }

        public void OnStop()
        {
            pluginIsActive = false;
            timer.Dispose();
        }

        private void Timer_OnElapsed()
        {
            var wowObjects = new List<WowObject>();
            var activePlayer = game.GetGameObjects(wowObjects);
            var bobberThatBobbing = wowObjects.FirstOrDefault(i => i.OwnerGUID == activePlayer.GUID && i.Bobbing);
            if (bobberThatBobbing != null)
            {
                this.LogPrint("Got bit!");
                Thread.Sleep(Utilities.Rnd.Next(500, 1000));
                this.LogPrint("Interacting with bobber --> " + bobberThatBobbing.GUID);
                bobberThatBobbing.Interact();
                Thread.Sleep(Utilities.Rnd.Next(500, 1000));
            }
            if (!game.IsLooting && !activePlayer.InCombat)
            {
                if (activePlayer.CastingSpellID == 0 && activePlayer.ChannelSpellID == 0)
                {
                    if (ApplyWaterWalkingIfNeeded(activePlayer))
                        return;
                    if (DoBreakIfNeeded())
                        return;
                    if (ApplyBestLureIfNeeded(activePlayer))
                        return;
                    if (ApplySpecialLureIfNeeded(activePlayer))
                        return;
                    if (ApplyArcaneLureIfNeeded(activePlayer))
                        return;
                    if (ApplyLegionSpecialLureIfNeeded(activePlayer))
                        return;
                    if (DoDalaranAchievementIfNeeded(activePlayer))
                        return;
                    if (DoMargossRepIfNeeded(activePlayer))
                        return;
                    Thread.Sleep(Utilities.Rnd.Next(500, 1000));
                    this.LogPrint($"Cast fishing --> ({FISHING_SPELL_NAME})");
                    game.CastSpellByName(FISHING_SPELL_NAME);
                    Thread.Sleep(1500);
                }
            }
        }

        private bool ApplyWaterWalkingIfNeeded(WoWPlayerMe activePlayer)
        {
            if (settings.UseWaterWalking)
            {
                if (activePlayer.Class == WowPlayerClass.Sha)
                {
                    var wowAura = activePlayer.Auras.FirstOrDefault(l => l.Name == WATER_WALKING_NAME);
                    if (wowAura == default(WoWAura) || wowAura.TimeLeftInMs < 30 * 1000) // 30 sec
                    {
                        this.LogPrint($"Getting water walking buff");
                        var wowNpcs = new List<WowNpc>();
                        var wowPlayers = new List<WowPlayer>();
                        var me = game.GetGameObjects(npcs: wowNpcs, players: wowPlayers);
                        var myTarget = me.TargetGUID;
                        me.Target();
                        game.CastSpellByName(WATER_WALKING_NAME);
                        if (myTarget != WoWGUID.Zero)
                        {
                            var npc = wowNpcs.FirstOrDefault(l => l.GUID == myTarget);
                            if (npc != null)
                            {
                                npc.Target();
                            }
                            else
                            {
                                var player = wowPlayers.FirstOrDefault(l => l.GUID == myTarget);
                                if (player != null)
                                    player.Target();
                            }
                        }
                        Thread.Sleep(Utilities.Rnd.Next(500, 1000));
                        return true;
                    }
                }
            }
            return false;
        }

        private bool DoBreakIfNeeded()
        {
            if (settings.EnableBreaks && Utilities.Rnd.Next(0, 30) == 0)
            {
                var breakTime = Utilities.Rnd.Next(15, 45);
                this.LogPrint($"I'm human! Let's have a break ({breakTime} sec)");
                var breakStartTime = Environment.TickCount;
                while ((Environment.TickCount - breakStartTime < breakTime * 1000) && pluginIsActive)
                    Thread.Sleep(1000);
                return true;
            }
            return false;
        }

        private bool ApplyBestLureIfNeeded(WoWPlayerMe me)
        {
            if (settings.UseBestBait)
            {
                uint baitID = 0;
                if (me.Inventory.Length > 15 && me.Inventory.Any(l => FISHING_RODS.Contains(l.EntryID)) && (DateTime.UtcNow - lastTimeLureApplied).TotalMinutes > 10)
                {
                    baitID = BAITS.FirstOrDefault(l => me.ItemsInBags.Any(k => k.EntryID == l));
                }
                if (baitID != 0)
                {
                    this.LogPrint($"Applying lure --> ({Wowhead.GetItemInfo(baitID).Name})");
                    game.UseItemByID(baitID);
                    lastTimeLureApplied = DateTime.UtcNow;
                    Thread.Sleep(Utilities.Rnd.Next(250, 750));
                    return true;
                }
            }
            return false;
        }

        private bool ApplySpecialLureIfNeeded(WoWPlayerMe me)
        {
            if (settings.UseSpecialBait)
            {
                uint baitID = 0;
                try
                {
                    if (me.Auras.All(l => l.Name != settings.SpecialBait))
                    {
                        var item = me.ItemsInBags.FirstOrDefault(l => l.Name == settings.SpecialBait);
                        if (item == null)
                        {
                            if (settings.GetSpecialBaitFromNatPagle)
                            {
                                GetSpecialBaitBuffFromNatPagle();
                            }
                            else if (settings.UseAnySpecialBaitIfPreferredIsNotAvailable)
                            {
                                var haveBuff = me.Auras.Any(aura => SPECIAL_BAITS.Keys.Select(l => Wowhead.GetItemInfo(l).Name).Contains(aura.Name));
                                if (!haveBuff)
                                {
                                    baitID = SPECIAL_BAITS.Keys.FirstOrDefault(itemID => me.ItemsInBags.Select(itemInBag => itemInBag.EntryID).Contains(itemID));
                                }
                            }
                        }
                        baitID = item?.EntryID ?? 0;
                    }
                }
                catch (Exception ex)
                {
                    this.LogPrint(ex.Message + "\r\n" + ex.StackTrace);
                    baitID = 0;
                }
                if (baitID != 0)
                {
                    this.LogPrint($"Applying special lure --> ({Wowhead.GetItemInfo(baitID).Name})");
                    game.UseItemByID(baitID);
                    Thread.Sleep(Utilities.Rnd.Next(250, 750));
                    return true;
                }
            }
            return false;
        }

        private void GetSpecialBaitBuffFromNatPagle()
        {
            this.LogPrint("Searching for Nat Pagle, id:85984");
            var npcs = new List<WowNpc>();
            game.GetGameObjects(null, null, npcs);
            var natPagle = npcs.FirstOrDefault(npc => npc.EntryID == 85984);
            if (natPagle != null)
            {
                this.LogPrint($"Nat Pagle is found, guid: {natPagle.GUID}, moving to him...");
                game.Move2D(natPagle.Location, 4f, 2000, false, false);
                Thread.Sleep(500);
                this.LogPrint("Opening dialog window with Nat...");
                natPagle.Interact();
                Thread.Sleep(2000);
                game.SelectDialogOption("Обычная приманка для рыбы?"); // todo: is it possible to localize it?
                Thread.Sleep(1500);
                var gossipText = Wowhead.GetItemInfo(SPECIAL_BAITS.First(baitFish => Wowhead.GetItemInfo(baitFish.Key).Name == settings.SpecialBait).Value).Name;
                game.SelectDialogOption(gossipText);
                Thread.Sleep(1500);
                var goodFishingPoint = new WowPoint(2024.49f, 191.33f, 83.86f);
                this.LogPrint($"Moving to fishing point [{goodFishingPoint}]");
                game.Move2D(goodFishingPoint, 2f, 2000, false, false);
                var water = new WowPoint((float)(Utilities.Rnd.NextDouble() * 5 + 2032.5f), (float)(Utilities.Rnd.NextDouble() * 5 + 208.5f), 82f);
                this.LogPrint($"Facing water [{water}]");
                game.Face(water);
            }
        }

        private bool ApplyArcaneLureIfNeeded(WoWPlayerMe me)
        {
            if (settings.UseArcaneLure)
            {
                uint itemID = 0;
                var arcaneLureSpellName = Wowhead.GetSpellInfo(218861).Name;
                if (me.Auras.All(l => l.Name != arcaneLureSpellName))
                {
                    var item = me.ItemsInBags.FirstOrDefault(l => l.Name == arcaneLureSpellName);
                    if (item != null)
                    {
                        itemID = item.EntryID;
                    }
                }
                if (itemID != 0)
                {
                    this.LogPrint($"Applying arcane lure --> ({Wowhead.GetItemInfo(itemID).Name})");
                    game.UseItemByID(itemID);
                    Thread.Sleep(Utilities.Rnd.Next(250, 750));
                    return true;
                }
            }
            return false;
        }

        private bool ApplyLegionSpecialLureIfNeeded(WoWPlayerMe me)
        {
            if (settings.LegionUseSpecialLure)
            {
                uint baitID = 0;
                try
                {
                    var zone = game.ZoneID;
                    var allBuffNames = LEGION_SPECIAL_LURES_BY_ZONE.SelectMany(l => l.Lures).Select(l => Wowhead.GetItemInfo(l).Name).ToArray();
                    if (me.Auras.All(k => !allBuffNames.Contains(k.Name)))
                    {
                        var info = LEGION_SPECIAL_LURES_BY_ZONE.FirstOrDefault(l => l.ZoneID == zone);
                        if (info != null)
                        {
                            var item = me.ItemsInBags.FirstOrDefault(l => info.Lures.Contains(l.EntryID));
                            if (item != null)
                                baitID = item.EntryID;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.LogPrint(ex.Message + "\r\n" + ex.StackTrace);
                    baitID = 0;
                }
                if (baitID != 0)
                {
                    this.LogPrint($"Applying Legion special lure --> ({Wowhead.GetItemInfo(baitID).Name})");
                    game.UseItemByID(baitID);
                    Thread.Sleep(Utilities.Rnd.Next(250, 750));
                    return true;
                }
            }
            return false;
        }

        private bool DoDalaranAchievementIfNeeded(WoWPlayerMe me)
        {
            if (settings.DalaranAchievement)
            {
                uint itemID = 0;
                if (me.Auras.All(l => !DALARAN_ACHIVEMENT_TRADE_ITEMS.Select(k => k.AuraID).Contains(l.SpellId) || l.TimeLeftInMs < 20000))
                {
                    var lureInBags = me.ItemsInBags.FirstOrDefault(l => DALARAN_ACHIVEMENT_TRADE_ITEMS.Select(k => k.ItemID).Contains(l.EntryID));
                    if (lureInBags != null)
                    {
                        itemID = lureInBags.EntryID;
                    }
                    this.LogPrint("Searching for Marcia Chase, id:95844");
                    var npcs = new List<WowNpc>();
                    game.GetGameObjects(null, null, npcs);
                    var marciaChase = npcs.FirstOrDefault(npc => npc.EntryID == 95844);
                    if (marciaChase != null)
                    {
                        this.LogPrint($"Marcia Chase is found, guid: {marciaChase.GUID}, interacting...");
                        marciaChase.Interact();
                        Thread.Sleep(Utilities.Rnd.Next(750, 1250));
                        this.LogPrint("I: I wish to buy something...");
                        game.SelectDialogOption("Мне бы хотелось купить что-нибудь у вас."); // todo: is it possible to localize it?
                        Thread.Sleep(Utilities.Rnd.Next(750, 1250));
                        var newLure = DALARAN_ACHIVEMENT_TRADE_ITEMS[Utilities.Rnd.Next(0, DALARAN_ACHIVEMENT_TRADE_ITEMS.Count)];
                        this.LogPrint("Buying lure " + Wowhead.GetItemInfo(newLure.ItemID).Name + "...");
                        game.BuyMerchantItem(newLure.ItemID, 1);
                        Thread.Sleep(Utilities.Rnd.Next(750, 1250));
                        this.LogPrint("Closing dialog window...");
                        game.SendToChat("/run CloseMerchant()");
                        itemID = newLure.ItemID;
                        this.LogPrint($"Applying lure for Dalaran achievement --> ({Wowhead.GetItemInfo(itemID).Name})");
                        game.UseItemByID(itemID);
                        Thread.Sleep(Utilities.Rnd.Next(250, 750));
                        return true;
                    }
                }
            }
            return false;
        }

        private bool DoMargossRepIfNeeded(WoWPlayerMe me)
        {
            if (LEGION_REP_ZONES.TryGetValue(game.ZoneID, out LegionRepPoint value))
            {
                var timeout = 5000;
                while (timeout > 0 && me.Location.Distance2D(value.StartingPlayerPoint) > 3)
                {
                    game.Move2D(value.StartingPlayerPoint, 3f, 1000, true, false);
                    timeout -= 1000;
                }
                game.Face(value.StartingFacingPoint);
            }
            foreach (var itemID in LEGION_REP_SUMMON_ITEMS)
            {
                if (me.ItemsInBags.Any(l => l.EntryID == itemID))
                {
                    this.ShowNotify($"Click this message to use your {Wowhead.GetItemInfo(itemID).Name}", false, false, 2, (obj, args) => { game.UseItemByID(itemID); });
                    break;
                }
            }
            foreach (var itemID in LEGION_REP_CURRENCY_ITEMS)
            {
                if (me.ItemsInBags.Where(l => l.EntryID == itemID).Sum(l => l.StackSize) >= 100)
                {
                    this.ShowNotify($"You cannot get more {Wowhead.GetItemInfo(itemID).Name}", false, true);
                    Thread.Sleep(Utilities.Rnd.Next(1000, 2000));
                    return true;
                }
            }
            return false;
        }

    }
}
