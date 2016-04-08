using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Timers;
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
            t.Start();
            t.Elapsed += OnPulse;
        }

        private void OnPulse(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            byte r0, r1, r2, r3;
            bool b;
            Utilities.TestFunc(out r0, out r1, out r2, out r3, out b);
            frm.label1.Text = b.ToString();
            frm.label5.Text = r0.ToString();
            frm.label6.Text = r1.ToString();
            frm.label7.Text = r2.ToString();
            frm.label8.Text = r3.ToString();
            frm.Invalidate();
        }

        public void OnStop()
        {
            t.Elapsed -= OnPulse;
            t.Stop();
            frm.Dispose();
        }

        #endregion

        //private SingleThreadTimer timer;
        private System.Timers.Timer t = new System.Timers.Timer(50);
        private MainForm frm;
    }
}
