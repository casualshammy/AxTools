﻿using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.Forms;
using AxTools.Properties;
using AxTools.WinAPI;

namespace AxTools.Helpers
{
    internal static class Notify
    {

        internal static void SmartNotify(string title, string message, NotifyUserType type, bool sound, bool showOnlyBallonTip = false)
        {
            if (!showOnlyBallonTip && NativeMethods.GetForegroundWindow() == MainForm.Instance.Handle)
            {
                TaskDialog taskDialog = new TaskDialog(title, "AxTools", message, TaskDialogButton.OK);
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
                Balloon(title, message, type, sound);
            }
        }

        internal static void Balloon(string title, string message, NotifyUserType type, bool sound)
        {
            MainForm.Instance.PostInvoke(() => { MainForm.Instance.notifyIconMain.ShowBalloonTip(30000, title, message, (ToolTipIcon) type); });
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

        internal static void TaskDialog(string title, string message, NotifyUserType type, EventHandler<HyperlinkEventArgs> onHyperlinkClick = null)
        {
            TaskDialog(MainForm.Instance, title, message, type, onHyperlinkClick);
        }

        internal static void TaskDialog(this Form form, string title, string message, NotifyUserType type, EventHandler<HyperlinkEventArgs> onHyperlinkClick = null)
        {
            TaskDialog taskDialog = new TaskDialog(title, "AxTools", message, TaskDialogButton.OK);
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

        internal static void TrayPopup(string title, string message, NotifyUserType type, bool sound, Image image = null, int timeout = 10, EventHandler onClick = null)
        {
            MainForm.Instance.BeginInvoke((MethodInvoker) delegate
            {
                TrayPopup trayPopup = new TrayPopup(title, message, image);
                if (image == null)
                {
                    if (type == NotifyUserType.Error)
                    {
                        trayPopup.Icon = Resources.dialog_error;
                    }
                    else if (type == NotifyUserType.Warn)
                    {
                        trayPopup.Icon = Resources.dialog_warning;
                    }
                    else
                    {
                        trayPopup.Icon = Resources.dialog_information;
                    }
                }
                if (onClick != null)
                {
                    trayPopup.Click += onClick;
                    trayPopup.Click += (sender, args) => trayPopup.Close();
                }
                trayPopup.Show(timeout);
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
            });
        }

    }
}