using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using AxTools.Classes;
using AxTools.Classes.WinAPI;
using AxTools.Classes.WoW.PluginSystem;

namespace WoTWindowStyler
{
    public class Main : IPlugin
    {

        #region Info

        public string Author
        {
            get { return "CasualShammy"; }
        }

        public string Description
        {
            get { return "This plugin restyles World of Tanks client's window same manner as WoW window"; }
        }

        public int Interval
        {
            get { return 10000; }
        }

        public string Name
        {
            get { return "WoTWindowStyler"; }
        }

        public string TrayDescription
        {
            get { throw new NotImplementedException(); }
        }

        public System.Drawing.Image TrayIcon
        {
            get { throw new NotImplementedException(); }
        }

        public Version Version
        {
            get { throw new NotImplementedException(); }
        }

        public string WowIcon
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Events

        public void OnConfig()
        {
            throw new NotImplementedException();
        }

        public void OnPulse()
        {
            
        }

        public void OnStart()
        {
            try
            {
                foreach (Process p in Process.GetProcessesByName("worldoftanks"))
                {
                    for (int i = 0; i < 40; i++)
                    {
                        Thread.Sleep(1500);
                        p.Refresh();
                        if (p.MainWindowHandle != (IntPtr)0)
                        {
                            if (Settings.AutoAcceptWndSetts)
                            {
                                try
                                {
                                    if (Settings.Noframe)
                                    {
                                        int styleWow = NativeMethods.GetWindowLong(p.MainWindowHandle, NativeMethods.GWL_STYLE);
                                        styleWow = styleWow & ~(NativeMethods.WS_CAPTION | NativeMethods.WS_THICKFRAME);
                                        NativeMethods.SetWindowLong(p.MainWindowHandle, NativeMethods.GWL_STYLE, styleWow);
                                    }
                                    NativeMethods.SetWindowPos(p.MainWindowHandle, (IntPtr)SpecialWindowHandles.HWND_NOTOPMOST, Settings.WowWindowLocation.X,
                                        Settings.WowWindowLocation.Y, Settings.WowWindowSize.X, Settings.WowWindowSize.Y,
                                        SetWindowPosFlags.SWP_SHOWWINDOW);
                                    System.Diagnostics.Log.Print(String.Format("{0}:{1} :: [WoT] Window style is changed", p.ProcessName, p.Id));
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Log.Print(String.Format("{0}:{1} :: [WoT] Window changing failed with error: {2}", p.ProcessName, p.Id, ex.Message), true);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Log.Print("MainForm.AttachToWow: general error: " + ex.Message, true);
            }
        }

        public void OnStop()
        {
            
        }

        #endregion

    }
}
