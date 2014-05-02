using AxTools.Classes.WoW.PluginSystem;
using AxTools.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace AxTools.Classes.WoW.Plugins
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

        public string TrayDescription
        {
            get { return "Fishing"; }
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
        }

        public void OnPulse()
        {
            if (Environment.TickCount - iterationStartTime > 25000)
            {
                state = 0;
                Log.Print(String.Format("{0}:{1} :: [{2}] Timeout has expired", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Name));
            }
            switch (state)
            {
                case 0:
                    Log.Print(String.Format("{0}:{1} :: [{2}] Cast fishing...", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Name));
                    WoW.LuaDoString(castFishing);
                    Thread.Sleep(1500);
                    state = 1;
                    iterationStartTime = Environment.TickCount;
                    break;
                case 1:
                    try
                    {
                        WoW.Pulse(wowObjects);
                    }
                    catch (Exception ex)
                    {
                        Log.Print(String.Format("{0}:{1} :: [Fishing] Pulse error: {2}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, ex.Message), true);
                        break;
                    }
                    if (LocalPlayer.ChannelSpellID == 0)
                    {
                        Log.Print(String.Format("{0}:{1} :: [{2}] Player isn't fishing, recast...", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Name));
                        state = 0;
                        break;
                    }
                    bobber = wowObjects.FirstOrDefault(i => i.OwnerGUID == LocalPlayer.GUID);
                    if (bobber != null && bobber.Animation == 4456449)
                    {
                        Log.Print(String.Format("{0}:{1} :: [{2}] Got bit!", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Name));
                        Thread.Sleep(250);
                        state = 2;
                    }
                    break;
                case 2:
                    if (bobber != null)
                    {
                        Log.Print(String.Format("{0}:{1} :: [{2}] Interacting...", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Name));
                        WoW.Interact(bobber.GUID);
                        bobber = null;
                        state = 3;
                    }
                    else
                    {
                        Log.Print(String.Format("{0}:{1} :: [{2}] Bobber isn't found, recast...", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Name));
                        state = 0;
                    }
                    break;
                case 3:
                    if (WoW.WProc.PlayerIsLooting)
                    {
                        state = 4;
                        Log.Print(String.Format("{0}:{1} :: [{2}] Looting...", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Name));
                    }
                    break;
                case 4:
                    if (!WoW.WProc.PlayerIsLooting)
                    {
                        Log.Print(String.Format("{0}:{1} :: [{2}] Looted, applying lure...", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Name));
                        WoW.LuaDoString(lure);
                        Thread.Sleep(500);
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
