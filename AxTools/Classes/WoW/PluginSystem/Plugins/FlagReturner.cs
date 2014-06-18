using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using AxTools.Classes.WoW.Management;
using AxTools.Classes.WoW.Management.ObjectManager;
using AxTools.Forms;
using AxTools.Properties;

namespace AxTools.Classes.WoW.PluginSystem.Plugins
{
    class FlagReturner : IPlugin
    {

        #region Info

        public string Name
        {
            get { return "Capture flags/orbs on the battlefields"; }
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
            get
            {
                return "This plugin will automatically return or pickup flags in Warsong Gulch, Twin Peaks and EotS, also it will pickup orbs in ToK";
            }
        }

        public Image TrayIcon { get { return Resources.achievement_bg_winwsg; } }

        public int Interval
        {
            get { return 50; }
        }

        public string WowIcon
        {
            get { return "Interface\\\\Icons\\\\achievement_bg_winwsg"; }
        }

        #endregion

        #region Events

        public void OnConfig()
        {
            throw new NotImplementedException();
        }

        public void OnStart()
        {
            searchingZone = WoWManager.WoWProcess.PlayerZoneID;
            zoneText = WoWDXInject.GetFunctionReturn("GetZoneText()");
            switch (searchingZone)
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
                    searchingObjects = new string[] {};
                    Log.Print(String.Format("{0}:{1} :: [{2}] Unknown battlefield ({3}/{4})", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, searchingZone, zoneText));
                    MainForm.Instance.ShowNotifyIconMessage("[" + Name + "] Unknown battlefield", "I don't know what to do in this zone...", ToolTipIcon.Error);
                    SystemSounds.Hand.Play();
                    break;
            }
        }

        public void OnPulse()
        {
            if (WoWManager.WoWProcess.IsBattlegroundFinished != 0)
            {
                Log.Print(string.Format("{0}:{1} :: [{2}] Plugin is stopped: the battle has ended", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name));
                MainForm.Instance.ShowNotifyIconMessage("[" + Name + "] Plugin is stopped", "The battle has ended", ToolTipIcon.Info);
                PluginManager.StopPlugin(true, true);
                return;
            }
            uint zone = WoWManager.WoWProcess.PlayerZoneID;
            if (zone != searchingZone)
            {
                Log.Print(string.Format("{0}:{1} :: [{2}] Plugin is stopped: zone changed to {3} ({4})", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, zoneText, zone));
                MainForm.Instance.ShowNotifyIconMessage("[" + Name + "] Plugin is stopped", string.Format("Zone changed to {0} ({1})", zoneText, zone), ToolTipIcon.Info);
                PluginManager.StopPlugin(true, true);
                return;
            }
            WoWPlayerMe localPlayer;
            try
            {
                localPlayer = ObjectMgr.Pulse(wowObjects);
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [{2}] Pulse error: {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, ex.Message), true);
                return;
            }
            foreach (WowObject i in wowObjects.Where(l => searchingObjects.Contains(l.Name) && l.Location.Distance(localPlayer.Location) <= 10))
            {
                WoWDXInject.Interact(i.GUID);
                Log.Print(string.Format("{0}:{1} :: [{2}] Interacting with {3} (0x{4:X})", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, i.Name, i.GUID), false, false);
            }
        }

        public void OnStop()
        {
            
        }

        #endregion

        #region Variables

        private uint searchingZone;

        private string zoneText;

        private string[] searchingObjects;

        private readonly List<WowObject> wowObjects = new List<WowObject>();

        #endregion

    }
}
