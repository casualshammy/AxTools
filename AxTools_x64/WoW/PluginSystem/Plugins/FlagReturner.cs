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
                uint zone = Info.ZoneID;
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
            WoWPlayerMe localPlayer = null;
            if (searchingObjects.Length > 0)
            {
                try
                {
                    localPlayer = ObjectMgr.Pulse(wowObjects);
                }
                catch (Exception ex)
                {
                    Log.Error($"{WoWManager.WoWProcess} [{Name}] <ObjectMgr.Pulse> error: {ex.Message}");
                    return;
                }
                // todo: delete try..catch
                try
                {
                    foreach (WowObject i in wowObjects.Where(l => searchingObjects.Contains(l.Name) && l.Location.Distance(localPlayer.Location) <= 10))
                    {
                        GameFunctions.Interact(i.GUID);
                        this.LogPrint($"Interacting with {i.Name} ({i.GUID})");
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
                case 5031: // Два Пика
                    searchingObjects = new[] { Wowhead.GetSpellInfo(23335).Name, Wowhead.GetSpellInfo(23333).Name }; // Флаг Альянса, Horde Flag
                    break;
                case 3820: // regular Око Бури
                case 5799:
                    searchingObjects = new[] {  Wowhead.GetSpellInfo(34976).Name, "Флаг Ока Бури" }; // Флаг Пустоверти
                    break;
                case 6051:
                    searchingObjects = new[] { Wowhead.GetSpellInfo(121164).Name }; // entry ids: 212091, 212092, 212093, 212094; Сфера могущества
                    break;
                case 6665: // Каньон Суровых Ветров
                    searchingObjects = new[] { Wowhead.GetSpellInfo(140876).Name, Wowhead.GetSpellInfo(141210).Name }; // Вагонетка Альянса, Вагонетка Орды
                    break;
                default:
                    searchingObjects = new string[] { };
                    break;
            }
            if (searchingObjects.Length > 0)
            {
                string zoneText = Info.ZoneText;
                this.LogPrint(string.Format("We're in {0}, searching for {{{1}}}", zoneText, string.Join(", ", searchingObjects)));
                this.ShowNotify(string.Format("{0}: {{{1}}}", zoneText, string.Join(", ", searchingObjects)), false, true);
            }
            else
            {
                this.LogPrint("Unknown battlefield, ID: " + zone + "; zoneText: " + Info.ZoneText);
                this.ShowNotify("Unknown battlefield (" + Info.ZoneText + "). I don't know what to do in this zone...", true, true);
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
