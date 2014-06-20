using AxTools.Classes.WoW.PluginSystem;
using AxTools.Classes.WoW.PluginSystem.API;
using AxTools.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace AxTools.Classes.WoW.Plugins
{
    internal class FishingExternal : IPlugin
    {

        #region Info

        public string Name
        {
            get { return "Fishing_external"; }
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

        public string TrayDescription
        {
            get { return "Fishing (External)"; }
        }

        public Image TrayIcon { get { return Resources.trade_fishing; } }

        public int Interval
        {
            get { return 100; }
        }

        public string WowIcon
        {
            get { return "Interface\\\\Icons\\\\trade_fishing"; }
        }

        #endregion

        #region Events

        public void OnConfig()
        {
            throw new NotImplementedException();
        }

        public void OnStart()
        {
            state = 0;
            bobber = null;
            iterationStartTime = Environment.TickCount;
            Utilities.LogPrint("Plugin is started");
        }

        public void OnPulse()
        {
            if (Environment.TickCount - iterationStartTime > 25000)
            {
                state = 0;
                Utilities.LogPrint("Timeout has expired");
            }
            switch (state)
            {
                case 0:
                    Utilities.LogPrint("Cast fishing...");
                    Lua.LuaDoString(castFishing);
                    Thread.Sleep(1500);
                    state = 1;
                    iterationStartTime = Environment.TickCount;
                    break;
                case 1:
                    try
                    {
                        EnvironmentObjects.Pulse(wowObjects);
                    }
                    catch (Exception ex)
                    {
                        Log.Print(String.Format("{0}:{1} :: [Fishing] Pulse error: {2}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, ex.Message), true);
                        break;
                    }
                    if (EnvironmentObjects.Me.ChannelSpellID == 0)
                    {
                        Utilities.LogPrint("Player isn't fishing, recast...");
                        state = 0;
                        break;
                    }
                    bobber = wowObjects.FirstOrDefault(i => i.OwnerGUID == EnvironmentObjects.Me.GUID);
                    if (bobber != null && bobber.Animation == 4456449)
                    {
                        Utilities.LogPrint("Got bit!");
                        Thread.Sleep(250);
                        state = 2;
                    }
                    break;
                case 2:
                    if (bobber != null)
                    {
                        Utilities.LogPrint("Interacting...");
                        Functions.Interact(bobber.GUID);
                        bobber = null;
                        state = 3;
                    }
                    else
                    {
                        Utilities.LogPrint("Bobber isn't found, recast...");
                        state = 0;
                    }
                    break;
                case 3:
                    if (Utilities.WProc.PlayerIsLooting)
                    {
                        state = 4;
                        Utilities.LogPrint("Looting...");
                    }
                    break;
                case 4:
                    if (!Utilities.WProc.PlayerIsLooting)
                    {
                        Utilities.LogPrint("Looted, applying lure...");
                        Lua.LuaDoString(lure);
                        Thread.Sleep(500);
                        state = 0;
                    }
                    break;
            }
        }

        public void OnStop()
        {
            Utilities.LogPrint("Plugin is stopped");
        }

        #endregion

        #region Variables

        private int state;

        private readonly string lure = "if (GetInventoryItemCooldown(\"player\", 1) == 0 and not GetWeaponEnchantInfo()) then " +
                                       "if (IsEquippedItem(33820)) then UseItemByName(33820) elseif (IsEquippedItem(88710)) then UseItemByName(88710) end end";

        private readonly string castFishing = "if (not UnitAffectingCombat(\"player\")) then CastSpellByName(\"Рыбная ловля\") end";

        private WowObject bobber;

        private readonly List<WowObject> wowObjects = new List<WowObject>();

        private int iterationStartTime;

        #endregion

    }
}
