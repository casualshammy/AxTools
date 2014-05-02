using System;
using System.Drawing;
using AxTools.Classes.WoW.PluginSystem;

namespace TestPlugin
{
    class TestPlugin : IPlugin
    {

        #region Info

        public string Name
        {
            get { return "TestPlugin"; }
        }

        public Version Version
        {
            get { return new Version(1, 1); }
        }

        public string Author
        {
            get { return "CasualShammy"; }
        }

        public string Description
        {
            get
            {
                return "Test plugin";
            }
        }

        public string TrayDescription
        {
            get { return string.Empty; }
        }

        public Image TrayIcon
        {
            get { return null; }
        }

        public int Interval
        {
            get { return 1000; }
        }

        public string WowIcon
        {
            get { return string.Empty; }
        }

        #endregion

        #region Events

        public void OnConfig()
        {
            throw new NotImplementedException();
        }

        public void OnStart()
        {
            mainForm = new MainForm();
            mainForm.Show();
            API.LogPrint(String.Format("{0}:{1} :: [{2}] Plugin is started", API.WProc.ProcessName, API.WProc.ProcessID, Name));
        }

        public void OnPulse()
        {
            string text = API.GetFunctionReturn("GetTime()");
            API.LuaDoString("print(" + text + ");");
            mainForm.label1.Text = text;
        }

        public void OnStop()
        {
            mainForm.Close();
            mainForm.Dispose();
            API.LogPrint(API.WProc != null ? String.Format("{0}:{1} :: [{2}] Plugin is stopped", API.WProc.ProcessName, API.WProc.ProcessID, Name) : string.Format("UNKNOWN:null :: [{0}] Plugin is stopped", Name));
        }

        #endregion

        private MainForm mainForm;

    }
}
