using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AxTools.WoW.PluginSystem.Plugins
{
    internal class FlagReturner : IPlugin
    {

        #region Info

        public string Name
        {
            get { return "Capture flags/orbs on the battlefields"; }
        }

        public Version Version { get { return new Version(1, 2); } }

        public string Author
        {
            get { return "CasualShammy"; }
        }

        public string Description
        {
            get
            {
                return "This plugin will automatically return or pickup flags in Warsong Gulch, Twin Peaks and EotS, also it will pickup orbs in ToK";
            }
        }

        public Image TrayIcon { get { return Resources.achievement_bg_winwsg; } }

        public string WowIcon
        {
            get { return "Interface\\\\Icons\\\\achievement_bg_winwsg"; }
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
            currentZone = 0;
            (timer = this.CreateTimer(50, OnPulse)).Start();
        }

        public void OnPulse()
        {
            // todo: delete try..catch
            try
            {
                uint zone = WoWManager.WoWProcess.PlayerZoneID;
                if (zone != currentZone)
                {
                    OnZoneChanged(zone);
                    currentZone = zone;
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("{0}:{1} :: [{2}] TODO error0: {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, ex.Message));
            }
            if (searchingObjects.Length > 0)
            {
                WoWPlayerMe localPlayer;
                try
                {
                    localPlayer = ObjectMgr.Pulse(wowObjects);
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("{0}:{1} :: [{2}] Pulse error: {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, ex.Message));
                    return;
                }
                // todo: delete try..catch
                try
                {
                    foreach (WowObject i in wowObjects.Where(l => searchingObjects.Contains(l.Name) && l.Location.Distance(localPlayer.Location) <= 10))
                    {
                        // todo: delete try..catch
                        try
                        {
                            WoWDXInject.Interact(i.GUID);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("{0}:{1} :: [{2}] TODO error1: {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, ex.Message));
                        }
                        Log.Info(string.Format("{0}:{1} :: [{2}] Interacting with {3} ({4})", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, i.Name, i.GUID));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("{0}:{1} :: [{2}] TODO error2: {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, ex.Message));
                }
            }
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        #endregion

        #region Variables

        private uint currentZone;
        private string[] searchingObjects;
        private readonly List<WowObject> wowObjects = new List<WowObject>();
        private SingleThreadTimer timer;

        #endregion

        private void OnZoneChanged(uint zone)
        {
            switch (zone)
            {
                case 3277:
                case 5031:
                    searchingObjects = new[] { "Флаг Альянса", "Флаг Орды" };
                    break;
                case 3820:
                case 5799:
                    searchingObjects = new[] { "Флаг Пустоверти" };
                    break;
                case 6051:
                    searchingObjects = new[] { "Сфера могущества" };
                    break;
                case 6665:
                    searchingObjects = new[] { "Вагонетка Альянса", "Вагонетка Орды" };
                    break;
                case 3703:
                    searchingObjects = new[] { "Хранилище гильдии" };
                    break;
                default:
                    searchingObjects = new string[] { };
                    break;
            }
            if (searchingObjects.Length > 0)
            {
                string zoneText = GameFunctions.Lua_GetFunctionReturn("GetZoneText()");
                this.LogPrint("We're in " + zoneText + ", searching for " + searchingObjects.AsString());
                Utilities.ShowNotifyMessage("[" + Name + "] ", zoneText + ": " + searchingObjects.AsString(), ToolTipIcon.Info);
                Utils.PlaySystemNotificationAsync();
            }
            else
            {
                this.LogPrint("Unknown battlefield, ID:" + zone);
                Utilities.ShowNotifyMessage("[" + Name + "]", "Unknown battlefield. I don't know what to do in this zone...", ToolTipIcon.Warning);
                Utils.PlaySystemNotificationAsync();
            }
        }

    }
}
