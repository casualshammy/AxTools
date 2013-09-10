using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;
using AxTools.Forms;

namespace AxTools
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Process.GetProcessesByName("AxTools").Length > 1)
            {
                MessageBox.Show("Another instance of AxTools is running already", "AxTools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Environment.OSVersion.Version < new Version(6, 1))
            {
                MessageBox.Show("AxTools only works on Windows 7 or higher", "AxTools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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
            Application.Run(new MainForm());
        }

        internal static bool IsRestarting = false;

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
