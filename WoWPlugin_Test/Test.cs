using System;
using System.Drawing;
using System.Globalization;
using System.IO;
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

        private Image trayIcon = null;
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
            //GameFunctions.NewChatMessage += GameFunctionsOnNewChatMessage;
            t.Start();
            t.Elapsed += OnPulse;
            try
            {
                WoWUIFrame.ReloadFrames();
                foreach (WoWUIFrame frame in WoWUIFrame.GetFrames)
                {
                    File.AppendAllText(Application.StartupPath + "\\1.txt", string.Format("{0}::{1}\r\n", frame.GetName, frame.IsVisible));
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void GameFunctionsOnNewChatMessage(ChatMsg chatMsg)
        {
            if (chatMsg.Type != 0x11)
            {
                frm.label1.Text = string.Format("{0}::{1}::{2}::{3}", chatMsg.Type, chatMsg.Sender, chatMsg.Channel, chatMsg.Text);
                frm.Invalidate();
            }
        }

        private void OnPulse(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            //GameFunctions.ReadChat();
        }

        public void OnStop()
        {
            t.Elapsed -= OnPulse;
            t.Stop();
            //GameFunctions.NewChatMessage -= GameFunctionsOnNewChatMessage;
            frm.Dispose();
        }

        #endregion

        //private SingleThreadTimer timer;
        private System.Timers.Timer t = new System.Timers.Timer(50);
        private MainForm frm;
    }
}
