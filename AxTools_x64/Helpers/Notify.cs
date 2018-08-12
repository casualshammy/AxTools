using AxTools.Forms;

using AxTools.WinAPI;
using Components.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Helpers
{
    internal static class Notify
    {
        internal static void SmartNotify(string title, string message, NotifyUserType type, bool sound, bool showOnlyTrayPopup = false)
        {
            if (!showOnlyTrayPopup && NativeMethods.GetForegroundWindow() == MainForm.Instance.Handle)
            {
                var taskDialog = new TaskDialog(title, nameof(AxTools), message, TaskDialogButton.OK);
                switch (type)
                {
                    case NotifyUserType.Error:
                        taskDialog.CommonIcon = TaskDialogIcon.Stop;
                        break;

                    case NotifyUserType.Warn:
                        taskDialog.CommonIcon = TaskDialogIcon.Warning;
                        break;

                    default:
                        taskDialog.CommonIcon = TaskDialogIcon.Information;
                        break;
                }
                MainForm.Instance.PostInvoke(() =>
                {
                    MainForm.Instance.Activate();
                    taskDialog.Show(MainForm.Instance);
                });
            }
            else
            {
                TrayPopup(title, message, type, sound);
            }
        }

        internal static void TaskDialog(string title, string message, NotifyUserType type, EventHandler<HyperlinkEventArgs> onHyperlinkClick = null)
        {
            TaskDialog(MainForm.Instance, title, message, type, onHyperlinkClick);
        }

        internal static void TaskDialog(this Form form, string title, string message, NotifyUserType type, EventHandler<HyperlinkEventArgs> onHyperlinkClick = null)
        {
            var taskDialog = new TaskDialog(title, nameof(AxTools), message, TaskDialogButton.OK);
            if (onHyperlinkClick != null)
            {
                taskDialog.EnableHyperlinks = true;
                taskDialog.HyperlinkClick += onHyperlinkClick;
            }
            if (type == NotifyUserType.Error)
            {
                taskDialog.CommonIcon = TaskDialogIcon.Stop;
            }
            else if (type == NotifyUserType.Warn)
            {
                taskDialog.CommonIcon = TaskDialogIcon.Warning;
            }
            else
            {
                taskDialog.CommonIcon = TaskDialogIcon.Information;
            }
            form.Invoke((MethodInvoker)(() =>
            {
                form.Activate();
                taskDialog.Show(form);
            }));
            if (onHyperlinkClick != null)
            {
                taskDialog.HyperlinkClick -= onHyperlinkClick;
            }
        }

        internal static void TrayPopup(string title, string message, NotifyUserType type, bool sound, Image image = null, int timeoutSec = 10, MouseEventHandler onClick = null)
        {
            MainForm.Instance.BeginInvoke((MethodInvoker)delegate
           {
#pragma warning disable CC0022
               var trayPopup = new PopupNotification(title, message, image, Settings2.Instance.StyleColor);
#pragma warning restore CC0022
               if (image == null)
               {
                   if (type == NotifyUserType.Error)
                   {
                       trayPopup.Icon = Resources.DialogError;
                   }
                   else if (type == NotifyUserType.Warn)
                   {
                       trayPopup.Icon = Resources.DialogWarning;
                   }
                   else
                   {
                       trayPopup.Icon = Resources.DialogInfo;
                   }
               }
               if (onClick != null)
               {
                   trayPopup.Click += onClick;
                   trayPopup.Click += (sender, args) => trayPopup.Close();
               }
               trayPopup.Show(timeoutSec);
               if (sound)
               {
                   if (type == NotifyUserType.Error || type == NotifyUserType.Warn)
                   {
                       Utils.PlaySystemExclamationAsync();
                   }
                   else
                   {
                       Utils.PlaySystemNotificationAsync();
                   }
               }
           });
        }
    }
}