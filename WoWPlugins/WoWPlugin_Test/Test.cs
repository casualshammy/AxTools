using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using Microsoft.Win32;

namespace WoWPlugin_Test
{
    public class Test : IPlugin2
    {

        #region Info

        public string Name { get { return "Test"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Test plugin"; } }

        private readonly Image trayIcon = null;
        public Image TrayIcon { get { return trayIcon; } }

        public string WowIcon { get { return string.Empty; } }

        public bool ConfigAvailable { get { return false; } }

        public string[] Dependencies => null;

        #endregion

        #region Methods

        public void OnConfig()
        {

        }

        public void OnStart(GameInterface game)
        {
            this.game = game;
            (frm = new MainForm()).Show();
            game.ReadChat();
            game.NewChatMessage += GameFunctionsOnNewChatMessage;
            t.Start();
            t.Elapsed += OnPulse;
            using (RegistryKey regVersion = Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Wow6432Node\\\\Blizzard Entertainment\\\\World of Warcraft"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("InstallPath") != null)
                    {
                        string raw = regVersion.GetValue("InstallPath").ToString();
                        MessageBox.Show(raw);
                    }
                    MessageBox.Show(string.Empty);
                }
                catch
                {
                    MessageBox.Show(string.Empty);
                }
            }
        }

        private void OnPulse(object sender, ElapsedEventArgs e)
        {
            game.ReadChat();
        }

        private void GameFunctionsOnNewChatMessage(ChatMsg chatMsg)
        {
            if (chatMsg.Type != WoWChatMsgType.Channel)
            {
                frm.label1.Text = string.Format("Type: {0}; Sender: {1}; SenderGUID: {2}; Channel: {3}; Text: {4}", chatMsg.Type, chatMsg.Sender, chatMsg.SenderGUID, chatMsg.Channel, chatMsg.Text);
                frm.Invalidate();
            }
        }

        public void OnStop()
        {
            t.Elapsed -= OnPulse;
            t.Stop();
            game.NewChatMessage -= GameFunctionsOnNewChatMessage;
            frm.Dispose();
        }

        #endregion

        //private SafeTimer timer;
        private readonly System.Timers.Timer t = new System.Timers.Timer(50);
        private MainForm frm;
        private GameInterface game;
    }
}
