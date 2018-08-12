using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AxTools.WoW.PluginSystem.Plugins
{
    internal class FlagReturner : IPlugin3
    {
        #region Info

        public string Name => "Capture flags/orbs on the battlefields";

        public Version Version => new Version(1, 2);

        public string Description => "This plugin will automatically return or pickup flags in Warsong Gulch, Twin Peaks and EotS, it will pickup orbs in ToK and return carts in Deepwind Gorge";

        public Image TrayIcon => AxTools.Helpers.Resources.Plugin_Bg;

        public bool ConfigAvailable => false;

        public string[] Dependencies => null;
        public bool DontCloseOnWowShutdown => false;

        #endregion Info

        #region Events

        public void OnConfig()
        {
        }

        public void OnStart(GameInterface inf)
        {
            this.info = inf;
            currentZone = 0;
            (timer = this.CreateTimer(50, inf, OnPulse)).Start();
        }

        public void OnPulse()
        {
            // todo: delete try..catch
            try
            {
                uint zone = info.ZoneID;
                if (zone != currentZone)
                {
                    OnZoneChanged(zone);
                    currentZone = zone;
                }
            }
            catch (Exception ex)
            {
                this.LogPrint($"TODO error0: {ex.Message}");
            }
            WoWPlayerMe localPlayer;
            if (searchingObjects.Length > 0)
            {
                try
                {
                    localPlayer = info.GetGameObjects(wowObjects);
                }
                catch (Exception ex)
                {
                    this.LogPrint($"<ObjectMgr.Pulse> error: {ex.Message}");
                    return;
                }
                // todo: delete try..catch
                try
                {
                    foreach (WowObject i in wowObjects.Where(l => searchingObjects.Contains(l.Name) && l.Location.Distance(localPlayer.Location) <= 10))
                    {
                        i.Interact();
                        this.LogPrint($"Interacting with {i.Name} ({i.GUID})");
                    }
                }
                catch (Exception ex)
                {
                    this.LogPrint($"TODO error2: {ex.Message}");
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
                    searchingObjects = new[] { Wowhead.GetSpellInfo(34976).Name, "Флаг Ока Бури" }; // Флаг Пустоверти
                    break;

                case 6051:
                    searchingObjects = new[] { Wowhead.GetSpellInfo(121164).Name }; // entry ids: 212091, 212092, 212093, 212094; Сфера могущества
                    break;

                case 6665: // Каньон Суровых Ветров
                    searchingObjects = new[] { Wowhead.GetSpellInfo(140876).Name, Wowhead.GetSpellInfo(141210).Name }; // Вагонетка Альянса, Вагонетка Орды
                    break;

                case 9136: // Бурлящий берег
                    searchingObjects = new[] { Wowhead.GetSpellInfo(273459).Name }; // Азерит
                    break;

                default:
                    searchingObjects = new string[] { };
                    break;
            }
            if (searchingObjects.Length > 0)
            {
                string zoneText = info.ZoneText;
                this.LogPrint($"We're in {zoneText}, searching for {{{string.Join(", ", searchingObjects)}}}");
                this.ShowNotify($"{zoneText}: {{{string.Join(", ", searchingObjects)}}}", false, true);
            }
            else
            {
                this.LogPrint("Unknown battlefield, ID: " + zone + "; zoneText: " + info.ZoneText);
                this.ShowNotify("Unknown battlefield (" + info.ZoneText + "). I don't know what to do in this zone...", true, true);
            }
        }

        #endregion Events

        #region Variables

        private uint currentZone;
        private string[] searchingObjects;
        private readonly List<WowObject> wowObjects = new List<WowObject>();
        private SafeTimer timer;
        private GameInterface info;

        #endregion Variables
    }
}