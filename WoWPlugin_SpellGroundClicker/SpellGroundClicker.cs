using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_SpellGroundClicker
{
    public class SpellGroundClicker : IPlugin
    {

        #region Info

        public string Name { get { return "SpellGroundClicker"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Spams one AoE spell at the feet"; } }

        private Image trayIcon;
        public Image TrayIcon { get { return trayIcon ?? (trayIcon = Image.FromFile(string.Format("{0}\\plugins\\{1}\\Spell_shaman_earthquake.png", Application.StartupPath, Name))); } }

        public string WowIcon { get { return "Interface\\\\Icons\\\\spell_shaman_earthquake"; } }

        public bool ConfigAvailable { get { return false; } }

        #endregion

        #region Methods

        public void OnConfig()
        {
            
        }

        public void OnStart()
        {
            clickPoint = WinAPI.GetCursorPosition();
            (timer = this.CreateTimer(250, OnElapsed)).Start();
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        private void OnElapsed()
        {
            string p = GameFunctions.Lua_GetFunctionReturn("tostring(select(1, GetSpellCooldown(\"" + spellName + "\")))");
            if (p == "0")
            {
                WoWPlayerMe localPlayer = ObjMgr.Pulse();
                if (localPlayer != null && localPlayer.CastingSpellID == 0 && localPlayer.ChannelSpellID == 0)
                {
                    GameFunctions.Lua_DoString("CastSpellByName(\"" + spellName + "\")");
                    Thread.Sleep(250);
                    WinAPI.SetCursorPosition(clickPoint);
                    WinAPI.MouseEvent(WinAPI.MouseEventFlags.LeftDown | WinAPI.MouseEventFlags.LeftUp);
                    Thread.Sleep(1000);
                }
            }
        }

        #endregion

        #region Variables

        private SingleThreadTimer timer;
        private readonly string spellName = "Землетрясение";
        private Point clickPoint;

        #endregion

    }
}
