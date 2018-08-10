using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_SpellGroundClicker
{
    public class SpellGroundClicker : IPlugin3
    {

        #region Info

        public string Name { get { return "SpellGroundClicker"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Spams one AoE spell at the feet"; } }

        private Image trayIcon;
        public Image TrayIcon { get { return trayIcon ?? (trayIcon = Image.FromFile(string.Format("{0}\\plugins\\{1}\\Spell_shaman_earthquake.png", Application.StartupPath, Name))); } }

        public string WowIcon { get { return "Interface\\\\Icons\\\\spell_shaman_earthquake"; } }

        public bool ConfigAvailable { get { return false; } }

        public string[] Dependencies => null;

        public bool DontCloseOnWowShutdown => false;

        #endregion

        #region Methods

        public void OnConfig()
        {
            
        }

        public void OnStart(GameInterface game)
        {
            this.game = game;
            clickPoint = WinAPI.GetCursorPosition();
            (timer = this.CreateTimer(1000, game, OnElapsed)).Start();
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        private void OnElapsed()
        {
            WoWPlayerMe localPlayer = game.GetGameObjects();
            if (localPlayer != null && localPlayer.CastingSpellID == 0 && localPlayer.ChannelSpellID == 0 && DateTime.UtcNow - spellLastUsed > cooldown)
            {
                spellLastUsed = DateTime.UtcNow;
                game.CastSpellByName(spellName);
                Thread.Sleep(250);
                WinAPI.SetCursorPosition(clickPoint);
                WinAPI.MouseEvent(WinAPI.MouseEventFlags.LeftDown | WinAPI.MouseEventFlags.LeftUp);
                Thread.Sleep(1000);
            }
        }

        #endregion

        #region Variables

        private SafeTimer timer;
        private readonly string spellName = "Целительный ливень";
        private readonly TimeSpan cooldown = TimeSpan.FromSeconds(10);
        private DateTime spellLastUsed = DateTime.MinValue;
        private Point clickPoint;
        private GameInterface game;

        #endregion

    }
}
