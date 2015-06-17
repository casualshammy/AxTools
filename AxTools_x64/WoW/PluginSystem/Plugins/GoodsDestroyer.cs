using System;
using System.Drawing;
using System.Threading;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WoW.Management;

namespace AxTools.WoW.PluginSystem.Plugins
{
    class GoodsDestroyer : IPlugin
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

        public int Interval
        {
            get { return 50; }
        }

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
            

        }

        public void OnStart()
        {
            state = 0;
            iterationStartTime = Environment.TickCount;
            WoWDXInject.LuaDoString(
                "AxToolsHerbsIDs = {785, 2449, 2447, 765, 2450, 2453, 3820, 2452, 3369, 3356, 3357, 3355, 3819, 3818, 3821, 3358, 8836, 8839, 4625, 8846, 8831, 8838, 13463, 13464, 13465, 13466, 13467, 22786, 22785, 22793, 22791, 22792, 22787, 22789, 36907, 36903, 36906, 36904, 36905, 36901, 39970, 37921, 52983, 52987, 52984, 52986, 52985, 52988, 22790, 72235, 72234, 72237, 79010, 79011, 89639, 109129, 109128, 109127, 109126, 109125, 109124, 8845};" +
                "AxToolsDisenchantWeapon = \"Оружие\";" +
                "AxToolsDisenchantArmor = \"Доспехи\";" +
                "AxToolsOreIDs = {2770, 2771, 2772, 10620, 3858, 23424, 23425, 36909, 36912, 36910, 52185, 53038, 52183, 72093, 72094, 72103, 72092};");
        }

        public void OnPulse()
        {
            if (Environment.TickCount - iterationStartTime > 5000)
            {
                Log.Info(String.Format("{0}:{1} :: [{2}] Timeout has expired (nothing to do?), state: {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, state));
                state = 0;
            }
            switch (state)
            {
                case 0:
                    if (WoWDXInject.GetFunctionReturn("tostring(IsSpellKnown(51005))") == "true") // mill
                    {
                        WoWDXInject.LuaDoString(mill);
                    }
                    else if (WoWDXInject.GetFunctionReturn("tostring(IsSpellKnown(31252))") == "true") // prospect
                    {
                        WoWDXInject.LuaDoString(prospecting);
                    }
                    else if (WoWDXInject.GetFunctionReturn("tostring(IsSpellKnown(13262))") == "true") // disenchant
                    {
                        Thread.Sleep(1000); // pause to prevent disenchanting unreal item 
                        WoWDXInject.LuaDoString(disenchant);
                    }
                    else
                    {
                        Log.Info("This character can't mill, prospect or disenchant!");
                    }
                    iterationStartTime = Environment.TickCount;
                    state = 1;
                    break;
                case 1:
                    if (WoWManager.WoWProcess.PlayerIsLooting)
                    {
                        state = 2;
                    }
                    break;
                case 2:
                    if (!WoWManager.WoWProcess.PlayerIsLooting)
                    {
                        state = 0;
                    }
                    break;
            }
        }

        public void OnStop()
        {
            
        }

        #endregion

        #region Variables

        private readonly string mill =
            "for bag = 0, 4 do for bag_slot = 1, GetContainerNumSlots(bag) do local name, cCount = GetContainerItemInfo(bag, bag_slot); local id = GetContainerItemID(bag, bag_slot); if (name) then if (tContains(AxToolsHerbsIDs, id) and cCount >= 5) then CastSpellByID(51005); UseContainerItem(bag, bag_slot); return; end end end end";

        private readonly string disenchant =
            "for bag = 0, 4 do for bag_slot = 1, GetContainerNumSlots(bag) do local itemLink = select(7, GetContainerItemInfo(bag, bag_slot)); if (itemLink) then local _, _ , cQuality, cLevel, _, cClass = GetItemInfo(itemLink); if ((cClass == AxToolsDisenchantWeapon or cClass == AxToolsDisenchantArmor) and cLevel > 1 and cQuality == 2) then CastSpellByID(13262); UseContainerItem(bag, bag_slot); return; end end end end";

        private readonly string prospecting =
            "for bag = 0, 4 do for bag_slot = 1, GetContainerNumSlots(bag) do local name, cCount = GetContainerItemInfo(bag, bag_slot); local id = GetContainerItemID(bag, bag_slot); if (name) then if (tContains(AxToolsOreIDs, id) and cCount >= 5) then CastSpellByID(31252); UseContainerItem(bag, bag_slot); return; end end end end";

        private int state;

        private int iterationStartTime;

        #endregion

    }
}
