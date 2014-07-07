using AxTools.Forms;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace AxTools
{
    static class Program
    {
        internal static bool IsRestarting = false;

        [STAThread]
        static void Main()
        {
            bool newInstance;
            using (new Mutex(true, "AxToolsMainExecutable", out newInstance))
            {
                if (newInstance)
                {
                    if (Environment.OSVersion.Version >= new Version(6, 1))
                    {
                        using (WindowsIdentity p = WindowsIdentity.GetCurrent())
                        {
                            if (p != null)
                            {
                                WindowsPrincipal pricipal = new WindowsPrincipal(p);
                                if (!pricipal.IsInRole(WindowsBuiltInRole.Administrator))
                                {
                                    MessageBox.Show("This program requires administrator privileges", "AxTools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.ApplicationExit += ApplicationOnApplicationExit;
                        Application.Run((MainForm.Instance = new MainForm()));
                    }
                    else
                    {
                        MessageBox.Show("This program works only on Windows 7 or higher", "AxTools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("This program is already running", "AxTools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs eventArgs)
        {
            Application.ApplicationExit -= ApplicationOnApplicationExit;
            if (IsRestarting)
            {
                Process.Start(new ProcessStartInfo {FileName = Application.StartupPath + "\\Updater.exe", WorkingDirectory = Application.StartupPath});
            }
        }
    
    }
}
