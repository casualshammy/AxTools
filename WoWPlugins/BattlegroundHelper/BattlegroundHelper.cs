using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlegroundHelper
{
    public class BattlegroundHelper : IPlugin3
    {

        #region IPlugin3 info

        public bool ConfigAvailable => false;

        public string[] Dependencies => null;

        public string Description => "This plugin will automatically return or pickup flags in Warsong Gulch, Twin Peaks and EotS, it will pickup orbs in ToK and return carts in Deepwind Gorge";

        public bool DontCloseOnWowShutdown => false;

        public string Name => nameof(BattlegroundHelper);

        public Image TrayIcon => Resources.Plugin_Bg;

        public Version Version => new Version(1, 0);

        #endregion IPlugin3 info

        #region Variables

        private GameInterface game;
        private uint currentZone;
        private string[] searchingObjects;
        private readonly List<WowObject> wowObjects = new List<WowObject>();
        private SafeTimer timer;

        #endregion Variables

        public void OnConfig()
        {
            throw new NotImplementedException();
        }

        public void OnStart(GameInterface game)
        {
            this.game = game;
            currentZone = 0;
            (timer = this.CreateTimer(50, game, Timer_OnElapsed)).Start();
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        private void Timer_OnElapsed()
        {
            // todo: delete try..catch
            try
            {
                var zone = game.ZoneID;
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
            WoWPlayerMe activePlayer;
            if (searchingObjects.Length > 0)
            {
                try
                {
                    activePlayer = game.GetGameObjects(wowObjects);
                }
                catch (Exception ex)
                {
                    this.LogPrint($"<ObjectMgr.Pulse> error: {ex.Message}");
                    return;
                }
                // todo: delete try..catch
                try
                {
                    foreach (WowObject i in wowObjects.Where(l => searchingObjects.Contains(l.Name) && l.Location.Distance(activePlayer.Location) <= 10))
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
                var zoneText = game.ZoneText;
                this.LogPrint($"We're in {zoneText}, searching for {{{string.Join(", ", searchingObjects)}}}");
                this.ShowNotify($"{zoneText}: {{{string.Join(", ", searchingObjects)}}}", false, true);
            }
            else
            {
                this.LogPrint("Unknown battlefield, ID: " + zone + "; zoneText: " + game.ZoneText);
                this.ShowNotify("Unknown battlefield (" + game.ZoneText + "). I don't know what to do in this zone...", true, true);
            }
        }

    }
}
