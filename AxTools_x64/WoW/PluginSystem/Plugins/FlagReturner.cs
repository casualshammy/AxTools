using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;

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

        public string Description
        {
            get
            {
                return "This plugin will automatically return or pickup flags in Warsong Gulch, Twin Peaks and EotS, it will pickup orbs in ToK and return carts in Deepwind Gorge";
            }
        }

        public Image TrayIcon { get { return Resources.achievement_bg_winwsg; } }

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
                uint zone = GameFunctions.ZoneID;
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
                            GameFunctions.Interact(i.GUID);
                            Log.Error(string.Format("NAME: {0}; ENTRY ID: {1}; ZONE ID: {2}", i.Name, i.EntryID, currentZone));
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

        private void OnZoneChanged(uint zone)
        {
            switch (zone)
            {
                case 3277:
                case 5031:
                    searchingObjects = new[] { Wowhead.GetSpellInfo(23335).Name, Wowhead.GetSpellInfo(23333).Name }; // Флаг Альянса, Horde Flag
                    break;
                case 3820:
                case 5799:
                    searchingObjects = new[] {  Wowhead.GetSpellInfo(34976).Name }; // Флаг Пустоверти
                    break;
                case 6051:
                    searchingObjects = new[] { Wowhead.GetSpellInfo(121164).Name }; // entry ids: 212091, 212092, 212093, 212094; Сфера могущества
                    break;
                case 6665:
                    searchingObjects = new[] { Wowhead.GetSpellInfo(140876).Name, Wowhead.GetSpellInfo(141210).Name }; // Вагонетка Альянса, Вагонетка Орды
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
                string zoneText = GameFunctions.ZoneText;
                this.LogPrint(string.Format("We're in {0}, searching for {{{1}}}", zoneText, string.Join(", ", searchingObjects)));
                this.ShowNotify(string.Format("{0}: {{{1}}}", zoneText, string.Join(", ", searchingObjects)), false, true);
            }
            else
            {
                this.LogPrint("Unknown battlefield, ID: " + zone);
                this.ShowNotify("Unknown battlefield (" + GameFunctions.ZoneText + "). I don't know what to do in this zone...", true, true);
            }
        }

        #endregion

        #region Variables

        private uint currentZone;
        private string[] searchingObjects;
        private readonly List<WowObject> wowObjects = new List<WowObject>();
        private SafeTimer timer;

        #endregion

    }
}
