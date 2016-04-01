using AxTools.Forms;
using AxTools.WinAPI;
using System;
using System.IO;
using System.Media;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Helpers
{
    internal class AppSpecUtils
    {
        internal static void CheckCreateDir()
        {
            if (!Directory.Exists(Globals.TempPath))
            {
                Directory.CreateDirectory(Globals.TempPath);
            }
            if (!Directory.Exists(Globals.UserfilesPath))
            {
                Directory.CreateDirectory(Globals.UserfilesPath);
            }
            if (!Directory.Exists(Globals.CfgPath))
            {
                Directory.CreateDirectory(Globals.CfgPath);
            }
            if (!Directory.Exists(Globals.PluginsPath))
            {
                Directory.CreateDirectory(Globals.PluginsPath);
            }
            if (!Directory.Exists(Globals.PluginsAssembliesPath))
            {
                Directory.CreateDirectory(Globals.PluginsAssembliesPath);
            }
            if (!Directory.Exists(Globals.PluginsSettingsPath))
            {
                Directory.CreateDirectory(Globals.PluginsSettingsPath);
            }
        }

        internal static void CheckCreateTempDir()
        {
            if (!Directory.Exists(Globals.TempPath))
            {
                Directory.CreateDirectory(Globals.TempPath);
            }
        }

        internal static void Legacy()
        {
            try
            {
                // 08.10.2015
                string mySettingsDir = Globals.PluginsSettingsPath + "\\Fishing";
                string mySettingsFile = mySettingsDir + "\\FishingSettings.json";
                if (File.Exists(mySettingsFile))
                {
                    File.Move(mySettingsFile, mySettingsDir + "\\settings.json");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        internal static void NotifyUser(string title, string message, NotifyUserType type, bool sound, bool showOnlyBallonTip = false)
        {
            if (!showOnlyBallonTip && NativeMethods.GetForegroundWindow() == MainForm.Instance.Handle)
            {
                switch (type)
                {
                    case NotifyUserType.Error:
                        MainForm.Instance.ShowTaskDialog(title, message, TaskDialogButton.OK, TaskDialogIcon.Stop);
                        break;
                    case NotifyUserType.Warn:
                        MainForm.Instance.ShowTaskDialog(title, message, TaskDialogButton.OK, TaskDialogIcon.Warning);
                        break;
                    default:
                        MainForm.Instance.ShowTaskDialog(title, message, TaskDialogButton.OK, TaskDialogIcon.Information);
                        break;
                }
            }
            else
            {
                if (MainForm.Instance.InvokeRequired)
                {
                    MainForm.Instance.BeginInvoke(new MethodInvoker(() => MainForm.Instance.notifyIconMain.ShowBalloonTip(30000, title, message, (ToolTipIcon) type)));
                }
                else
                {
                    MainForm.Instance.notifyIconMain.ShowBalloonTip(30000, title, message, (ToolTipIcon)type);
                }
                if (sound)
                {
                    if (type == NotifyUserType.Error || type == NotifyUserType.Warn)
                    {
                        SystemSounds.Hand.Play();
                    }
                    else
                    {
                        Utils.PlaySystemNotificationAsync();
                    }
                }
            }
        }

    }
}
