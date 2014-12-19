using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace TestPlugin
{
    class AutoLoot : IPlugin
    {
        
        #region Info

        public string Name { get { return "AutoLoot"; } }

        public Version Version { get { return new Version(1, 0, 0, 0); } }

        public string Author { get { return "Axioma"; } }

        public string Description { get { return "Loots mobs in 100 yd range"; } }

        private Image trayIcon;
        public Image TrayIcon
        {
            get { return trayIcon ?? (trayIcon = new Bitmap(Application.StartupPath + "\\plugins\\AutoLoot\\inv_misc_coin_01.jpg")); }
        }

        public int Interval { get { return 250; } }

        public string WowIcon { get { return "Interface\\\\Icons\\\\inv_misc_coin_01"; } }

        public bool ConfigAvailable { get { return false; } }

        #endregion

        #region Events

        public void OnConfig()
        {

        }

        public void OnStart()
        {
            
        }

        public void OnPulse()
        {
            try
            {
                WoWPlayerMe localPlayer = ObjMgr.Pulse(wowNpcs);
                if (localPlayer.CastingSpellID == 0 && localPlayer.ChannelSpellID == 0 && !localPlayer.IsLooting)
                {
                    double distance = 40;
                    WowNpc npc = null;
                    foreach (WowNpc i in wowNpcs)
                    {
                        if (i.Lootable && i.Health == 0)
                        {
                            if (i.Location.Distance(localPlayer.Location) < distance)
                            {
                                npc = i;
                                distance = i.Location.Distance(localPlayer.Location);
                            }
                        }
                    }
                    if (npc != null)
                    {
                        if (distance > 1)
                        {
                            Functions.MoveTo(npc.Location);
                        }
                        else
                        {
                            npc.Interact();
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }

        public void OnStop()
        {
            
        }

        #endregion
    
        List<WowNpc> wowNpcs = new List<WowNpc>(); 

    }
}
