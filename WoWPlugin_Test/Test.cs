using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using Timer = System.Windows.Forms.Timer;

namespace WoWPlugin_Test
{
    public class Test : IPlugin
    {

        #region Info

        public string Name { get { return "Test"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Test plugin"; } }

        private readonly Image trayIcon = null;
        public Image TrayIcon { get { return trayIcon; } }

        public string WowIcon { get { return string.Empty; } }

        public bool ConfigAvailable { get { return false; } }

        #endregion

        #region Methods

        public void OnConfig()
        {

        }

        public void OnStart()
        {
            (frm = new MainForm()).Show();
            GameFunctions.ReadChat();
            GameFunctions.NewChatMessage += GameFunctionsOnNewChatMessage;
            t.Start();
            t.Elapsed += OnPulse;
        }

        private void OnPulse(object sender, ElapsedEventArgs e)
        {
            GameFunctions.ReadChat();
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
            GameFunctions.NewChatMessage -= GameFunctionsOnNewChatMessage;
            frm.Dispose();
        }

        #endregion

        //private SafeTimer timer;
        private readonly System.Timers.Timer t = new System.Timers.Timer(50);
        private MainForm frm;
    }
}
