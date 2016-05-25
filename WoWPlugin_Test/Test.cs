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
            //GameFunctions.NewChatMessage += GameFunctionsOnNewChatMessage;
            t.Start();
            t.Elapsed += OnPulse;
            try
            {
                //WoWUIFrame.ReloadFrames();
                //foreach (WoWUIFrame frame in WoWUIFrame.GetFrames)
                //{
                //    File.AppendAllText(Application.StartupPath + "\\1.txt", string.Format("{0}::{1}\r\n", frame.GetName, frame.IsVisible));
                //}
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            beep(beepType.Asterisk);
            Thread.Sleep(1000);
            beep(beepType.Exclamation);
            Thread.Sleep(1000);
            beep(beepType.OK);
            Thread.Sleep(1000);
            beep(beepType.Question);
            Thread.Sleep(1000);
            beep(beepType.SimpleBeep);
        }

        public enum beepType
        {
            /// <summary>
            /// A simple windows beep
            /// </summary>            
            SimpleBeep  = -1,
            /// <summary>
            /// A standard windows OK beep
            /// </summary>
            OK    = 0x00,
            /// <summary>
            /// A standard windows Question beep
            /// </summary>
            Question  = 0x20,
            /// <summary>
            /// A standard windows Exclamation beep
            /// </summary>
            Exclamation  = 0x30,
            /// <summary>
            /// A standard windows Asterisk beep
            /// </summary>
            Asterisk  = 0x40,
        }

        [DllImport("User32.dll", ExactSpelling=true)]
        private static extern bool MessageBeep(uint type);

        
        public static void beep(beepType type)
        {
            MessageBeep((uint)type);
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
            WoWUIFrame frame = WoWUIFrame.GetFrameByName("ChatFrame1EditBox");
            frm.label1.Text = frame != null ? string.Format("Visible: {0}; Editbox text:{1}", frame.IsVisible, frame.EditboxText) : "null";
            frm.Invalidate();
        }

        public void OnStop()
        {
            t.Elapsed -= OnPulse;
            t.Stop();
            //GameFunctions.NewChatMessage -= GameFunctionsOnNewChatMessage;
            frm.Dispose();
        }

        #endregion

        //private SafeTimer timer;
        private readonly System.Timers.Timer t = new System.Timers.Timer(50);
        private MainForm frm;
    }
}
