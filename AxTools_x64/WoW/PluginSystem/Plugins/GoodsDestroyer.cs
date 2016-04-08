using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using AxTools.Properties;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem.API;

namespace AxTools.WoW.PluginSystem.Plugins
{
    internal class GoodsDestroyer : IPlugin
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

        public string Author
        {
            get { return "CasualShammy"; }
        }

        public string Description
        {
            get
            {
                return "This plugin will mill/prospect any herbs/ores and disenchant greens in your bags";
            }
        }

        public Image TrayIcon { get { return Resources.inv_misc_gem_bloodgem_01; } }

        public string WowIcon
        {
            get { return "Interface\\\\Icons\\\\inv_misc_gem_bloodgem_01"; }
        }

        public bool ConfigAvailable
        {
            get { return false; }
        }

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

        public void OnStart()
        {
            SettingsInstance = this.LoadSettingsJSON<GoodsDestroyerSettings>();
            (timer = this.CreateTimer(50, OnPulse)).Start();
        }

        public void OnPulse()
        {
            WoWPlayerMe me = ObjectMgr.Pulse();
            if (me != null)
            {
                if (!GameFunctions.IsLooting)
                {
                    if (me.CastingSpellID == 0 && me.ChannelSpellID == 0)
                    {
                        if (GameFunctions.IsSpellKnown(51005)) // mill
                        {
                            if (me.ItemsInBags.Any(l => fastMillHerbs.Contains(l.EntryID) && l.StackSize >= 20))
                            {
                                string s = "/run if(GetNumTradeSkills()>0) then for i=1,GetNumTradeSkills() do local n,_,a=GetTradeSkillInfo(i);if(strfind(n,\"Массовое измельчение\") and a>0) then DoTradeSkill(i,a);return;end end end";
                                GameFunctions.SendToChat(s);
                                Thread.Sleep(2000);
                                return;
                            }
                            WoWItem herb = me.ItemsInBags.FirstOrDefault(l => herbs.Contains(l.EntryID) && l.StackSize >= 5);
                            if (herb != null)
                            {
                                GameFunctions.CastSpellByName(Wowhead.GetSpellInfo(51005).Name);
                                GameFunctions.UseItem(herb.BagID, herb.SlotID);
                                Thread.Sleep(500);
                                return;
                            }
                        }
                        if (GameFunctions.IsSpellKnown(31252)) // prospect
                        {
                            DoProspect();
                        }
                        if (GameFunctions.IsSpellKnown(13262)) // disenchant
                        {
                            Thread.Sleep(1000); // pause to prevent disenchanting nonexistent item 
                            DoDisenchant();
                        }
                        //else
                        //{
                        //    Log.Info("This character can't mill, prospect or disenchant!");
                        //}
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
            WoWPlayerMe me = ObjectMgr.Pulse();
            WoWItem itemToDesenchant = me.ItemsInBags.FirstOrDefault(l => (l.Class == 2 || l.Class == 4) && l.Quality == 2 && l.Level > 1);
            if (itemToDesenchant != null)
            {
                GameFunctions.CastSpellByName(Wowhead.GetSpellInfo(13262).Name);
                GameFunctions.UseItem(itemToDesenchant.BagID, itemToDesenchant.SlotID);
            }
        }

        private void DoProspect()
        {
            WoWPlayerMe me = ObjectMgr.Pulse();
            WoWItem ore = me.ItemsInBags.FirstOrDefault(l => ores.Contains(l.EntryID) && l.StackSize >= 5);
            if (ore != null)
            {
                GameFunctions.CastSpellByName(Wowhead.GetSpellInfo(31252).Name);
                GameFunctions.UseItem(ore.BagID, ore.SlotID);
            }
        }

        #endregion

        #region Variables

        private SingleThreadTimer timer;

        internal GoodsDestroyerSettings SettingsInstance;

        private readonly uint[] herbs =
        {
            785, 2449, 2447, 765, 2450, 2453, 3820, 2452, 3369, 3356, 3357, 3355, 3819, 3818, 3821, 3358, 8836, 8839, 4625, 8846, 8831, 8838, 13463, 13464, 13465, 13466, 13467, 22786, 22785, 22793, 22791, 22792,
            22787, 22789, 36907, 36903, 36906, 36904, 36905, 36901, 39970, 37921, 52983, 52987, 52984, 52986, 52985, 52988, 22790, 72235, 72234, 72237, 79010, 79011, 89639, 109129, 109128, 109127, 109126, 109125, 109124, 8845
        };

        private readonly uint[] fastMillHerbs =
        {
            109126, // Горгрондская мухоловка
            109127, // Звездоцвет
            109124, // Морозноцвет
            109128, // Награндский стрелоцвет
            109125, // Пламецвет
            109129 // Таладорская орхидея
        };

        private readonly uint[] ores = {2770, 2771, 2772, 10620, 3858, 23424, 23425, 36909, 36912, 36910, 52185, 53038, 52183, 72093, 72094, 72103, 72092};

        #endregion

    }
}
