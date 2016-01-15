﻿using AxTools.Forms;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.Helpers;

namespace AxTools
{
    static class Program
    {
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
                                    TaskDialog.Show("This program requires administrator privileges", "AxTools", "Make sure you have administrator privileges", TaskDialogButton.OK, TaskDialogIcon.SecurityError);
                                    return;
                                }
                            }
                        }
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        OnStartup();
                        Log.Info("Loading main window...");
                        Application.Run(MainForm.Instance = new MainForm());
                        Log.Info("Application is closed");
                    }
                    else
                    {
                        MessageBox.Show("This program works only on Windows 7 or higher", "AxTools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    TaskDialog.Show("This program is already running", "AxTools", "", TaskDialogButton.OK, TaskDialogIcon.Warning);
                }
            }
        }

        private static void OnStartup()
        {
            if (Directory.Exists(Globals.TempPath))
            {
                try
                {
                    Directory.Delete(Globals.TempPath, true);
                }
                catch
                {
                    File.Delete(Globals.LogFileName);
                }
            }
        }

    }
}
