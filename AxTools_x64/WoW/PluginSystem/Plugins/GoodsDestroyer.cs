using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using AxTools.Properties;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem.API;

namespace AxTools.WoW.PluginSystem.Plugins
{
    internal class GoodsDestroyer : IPlugin2
    {

        #region Info

        public string Name
        {
            get { return "Milling/disenchanting/prospecting"; }
        }

        public Version Version
        {
            get { return new Version(1, 1); }
        }

        public string Description
        {
            get
            {
                return "This plugin will mill/prospect any herbs/ores and disenchant greens in your bags";
            }
        }

        public Image TrayIcon { get { return Resources.inv_misc_gem_bloodgem_01; } }

        public bool ConfigAvailable
        {
            get { return true; }
        }

        public string[] Dependencies => null;

        #endregion

        #region Events

        public void OnConfig()
        {
            if (SettingsInstance == null)
            {
                SettingsInstance = this.LoadSettingsJSON<GoodsDestroyerSettings>();
            }
            GoodsDestroyerConfig.Open(SettingsInstance);
            this.SaveSettingsJSON(SettingsInstance);
        }

        public void OnStart(GameInterface info)
        {
            this.info = info;
            SettingsInstance = this.LoadSettingsJSON<GoodsDestroyerSettings>();
            if (SettingsInstance.MillFelwort)
            {
                herbs.Add(124106); // Felwort item id
            }
            lastNotifiedAboutCompletion = DateTime.MinValue;
            if (info.IsSpellKnown(51005)) // mill
            {
                this.LogPrint("Milling: OK");
            }
            if (info.IsSpellKnown(31252)) // prospect
            {
                this.LogPrint("Prospecting: OK");
            }
            if (info.IsSpellKnown(13262)) // disenchant
            {
                this.LogPrint("Disenchanting: OK");
            }
            (timer = this.CreateTimer(50, info, OnPulse)).Start();
        }

        public void OnPulse()
        {
            WoWPlayerMe me = info.GetGameObjects();
            if (me != null)
            {
                if (!info.IsLooting)
                {
                    if (me.CastingSpellID == 0 && me.ChannelSpellID == 0)
                    {
                        if (info.IsSpellKnown(51005)) // mill
                        {
                            if (SettingsInstance.UseFastDraenorMill && me.ItemsInBags.Any(l => fastMillHerbs.Contains(l.EntryID) && l.StackSize >= 20))
                            {
                                info.SendToChat(string.Format("/run {0}={{}}", someRandomString));
                                string prepare = string.Format("/run local s=C_TradeSkillUI.GetFilteredRecipeIDs();local t={{}};if(#s>0) then for i=1,#s do C_TradeSkillUI.GetRecipeInfo(s[i], t);{0}[t.name]={{t.recipeID,t.numAvailable}}; end end", someRandomString);
                                info.SendToChat(prepare);
                                string s = string.Format("/run for n,i in pairs({0}) do if(strfind(n,\"Массовое измельчение\") and i[2]>0) then C_TradeSkillUI.CraftRecipe(i[1],i[2]);return; end end", someRandomString);
                                info.SendToChat(s);
                                Thread.Sleep(2000);
                                return;
                            }
                            if (me.ItemsInBags.Any(l => l.EntryID == 136926)) // Кошмарный стручок
                            {
                                info.UseItemByID(136926);
                                Thread.Sleep(500);
                                return;
                            }
                            WoWItem herb = me.ItemsInBags.FirstOrDefault(l => herbs.Contains(l.EntryID) && l.StackSize >= 5);
                            if (herb != null)
                            {
                                info.CastSpellByName(Wowhead.GetSpellInfo(51005).Name);
                                info.UseItem(herb.BagID, herb.SlotID);
                                Thread.Sleep(500);
                                return;
                            }
                            if (SettingsInstance.LaunchInkCrafter)
                            {
                                Utilities.AddPluginToRunning(this, "InkCrafter");
                            }
                        }
                        if (info.IsSpellKnown(31252)) // prospect
                        {
                            DoProspect();
                        }
                        if (info.IsSpellKnown(13262)) // disenchant
                        {
                            Thread.Sleep(1000); // pause to prevent disenchanting nonexistent item 
                            DoDisenchant();
                        }
                        if ((DateTime.UtcNow - lastNotifiedAboutCompletion).TotalSeconds >= 60)
                        {
                            this.ShowNotify("Task is completed", false, true);
                            lastNotifiedAboutCompletion = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500); // wait for cast 
                    }
                }
            }
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        private void DoDisenchant()
        {
            WoWPlayerMe me = info.GetGameObjects();
            WoWItem itemToDesenchant = me.ItemsInBags.FirstOrDefault(l => (l.Class == 2 || l.Class == 4) && l.Quality == 2 && l.Level > 1);
            if (itemToDesenchant != null)
            {
                info.CastSpellByName(Wowhead.GetSpellInfo(13262).Name);
                info.UseItem(itemToDesenchant.BagID, itemToDesenchant.SlotID);
            }
        }

        private void DoProspect()
        {
            WoWPlayerMe me = info.GetGameObjects();
            WoWItem ore = me.ItemsInBags.FirstOrDefault(l => ores.Contains(l.EntryID) && l.StackSize >= 5);
            if (ore != null)
            {
                info.CastSpellByName(Wowhead.GetSpellInfo(31252).Name);
                info.UseItem(ore.BagID, ore.SlotID);
            }
        }

        #endregion

        #region Variables

        private SafeTimer timer;
        internal GoodsDestroyerSettings SettingsInstance;
        private GameInterface info;

        private readonly List<uint> herbs = new List<uint>
        {
            785, 2449, 2447, 765, 2450, 2453, 3820, 2452, 3369, 3356, 3357, 3355, 3819, 3818, 3821, 3358, 8836, 8839, 4625, 8846, 8831, 8838, 13463, 13464, 13465, 13466, 13467, 22786, 22785, 22793, 22791, 22792,
            22787, 22789, 36907, 36903, 36906, 36904, 36905, 36901, 39970, 37921, 52983, 52987, 52984, 52986, 52985, 52988, 22790, 72235, 72234, 72237, 79010, 79011, 89639, 109129, 109128, 109127, 109126, 109125, 109124, 8845,
            128304, 124101, 124102, 124104, 124103, 124105
        };

        private readonly uint[] fastMillHerbs =
        {
            109126, // Горгрондская мухоловка
            109127, // Звездоцвет
            109124, // Морозноцвет
            109128, // Награндский стрелоцвет
            109125, // Пламецвет
            109129, // Таладорская орхидея
            128304, // Семя Изеры
            124101, // Айтрил
            124102, // Грезолист
            124103, // Лисоцвет
            124104, // Фьярнскаггл
        };

        private readonly uint[] ores = {2770, 2771, 2772, 10620, 3858, 23424, 23425, 36909, 36912, 36910, 52185, 53038, 52183, 72093, 72094, 72103, 72092};

        private readonly string someRandomString = Utilities.GetRandomString(6, true);

        private DateTime lastNotifiedAboutCompletion = DateTime.MinValue;

        #endregion

    }
}
