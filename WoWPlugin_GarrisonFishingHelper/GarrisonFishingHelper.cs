using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AxTools.WoW.Management.ObjectManager;
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
            if (state == State.Fishing || state == State.None)
            {
                this.LogPrint("State: Fishing or None");
                if (GameFunctions.Lua_GetFunctionReturn(luaShouldPull) == "true")
                {
                    GameFunctions.Lua_DoString("UseItemByName(116158)");
                    state = State.Targeting;
                    Utilities.RequestStopPlugin("Fishing");
                }
                else
                {
                    Utilities.RequestStartPlugin("Fishing");
                }
            }
            else if (state == State.Targeting)
            {
                this.LogPrint("State: Targeting");
                if (GameFunctions.Lua_GetFunctionReturn(luaIsInCombat) == "true")
                {
                    WoWPlayerMe localPlayer0 = ObjMgr.Pulse(wowNpcs);
                    if (localPlayer0 != null)
                    {
                        WowNpc creature = wowNpcs.FirstOrDefault(k => k.Name == "Обитатель пещер Зашедшей Луны" || k.Name == "Обитатель ледяных пещер");
                        if (creature != null)
                        {
                            creature.Target();
                            state = State.Killing;
                        }
                    }
                }
                else
                {
                    state = State.Fishing;
                }
            }
            else if (state == State.Killing)
            {
                this.LogPrint("State: Killing");
                WoWPlayerMe localPlayer1 = ObjMgr.Pulse(wowNpcs);
                if (localPlayer1 != null)
                {
                    WowNpc creature = wowNpcs.FirstOrDefault(k => k.Name == "Обитатель пещер Зашедшей Луны" || k.Name == "Обитатель ледяных пещер");
                    if (creature != null)
                    {
                        if (creature.Health == 0)
                        {
                            if (creature.Lootable)
                            {
                                state = State.Looting;
                            }
                        }
                        else
                        {
                            if (localPlayer1.TargetGUID == creature.GUID)
                            {
                                GameFunctions.Lua_DoString(luaKillingRotation);
                            }
                            else
                            {
                                state = State.Targeting;
                            }
                        }
                    }
                    else
                    {
                        state = State.Fishing;
                    }
                }
            }
            else if (state == State.Looting)
            {
                this.LogPrint("State: Looting");
                WoWPlayerMe localPlayer2 = ObjMgr.Pulse(wowNpcs);
                if (localPlayer2 != null)
                {
                    WowNpc creature = wowNpcs.FirstOrDefault(k => k.Name == "Обитатель пещер Зашедшей Луны" || k.Name == "Обитатель ледяных пещер");
                    if (creature != null && creature.Lootable)
                    {
                        if (!localPlayer2.IsLooting)
                        {
                            creature.Interact();
                        }
                    }
                    else
                    {
                        state = State.Fishing;
                    }
                }
            }
        }

        #endregion

        #region Variables

        private SingleThreadTimer timer;
        private readonly string luaShouldPull = "tostring(GetItemCount(116158) > 0)";
        private readonly string luaIsInCombat = "tostring(InCombatLockdown())";
        private readonly string luaKillingRotation = "CastSpellByName(\"Молния\")";
        private State state = State.None;
        private readonly List<WowNpc> wowNpcs = new List<WowNpc>();

        #endregion

        private enum State
        {
            None,
            Fishing,
            Targeting,
            Killing,
            Looting
        }
    
    }
}
