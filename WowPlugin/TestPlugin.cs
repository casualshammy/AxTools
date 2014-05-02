using System;
using System.Drawing;
using AxTools.Classes.WoW.PluginSystem;
using AxTools.Classes.WoW.PluginSystem.API;

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
            Utilities.LogPrint("Plugin is started");
        }

        public void OnPulse()
        {
            string text = Lua.GetFunctionReturn("GetTime()");
            Lua.LuaDoString("print(" + text + ");");
            mainForm.label1.Text = text;
        }

        public void OnStop()
        {
            mainForm.Close();
            mainForm.Dispose();
            Utilities.LogPrint("Plugin is stopped");
        }

        #endregion

        private MainForm mainForm;

    }
}
