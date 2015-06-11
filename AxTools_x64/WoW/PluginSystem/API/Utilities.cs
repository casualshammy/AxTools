using System;
using System.Drawing;
using System.Windows.Forms;
using AxTools.Classes;
using AxTools.Forms;
using AxTools.WoW.Management;

namespace AxTools.WoW.PluginSystem.API
{
    public static class Utilities
    {
        /// <summary>
        /// Makes a record to the log. WoW process name, process id and plugin name included
        /// </summary>
        /// <param name="text"></param>
        public static void LogPrint(object text)
        {
            Log.Info(String.Format("{0}:{1} :: [Plugin: {2}] {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, PluginManager.ActivePlugin.Name, text));
        }

        /// <summary>
        ///     Shows notify message in tray
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="icon"></param>
        public static void ShowNotifyMessage(string title, string text, ToolTipIcon icon)
        {
            MainForm.Instance.ShowNotifyIconMessage(title, text, icon);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">Any text you want</param>
        /// <param name="wowIcon">Example: "Interface\\\\Icons\\\\achievement_bg_winwsg"</param>
        /// <param name="color">Text color</param>
        public static void ShowIngameNotify(string text, string wowIcon, Color color)
        {
            if (Settings.Instance.WoWPluginShowIngameNotifications)
            {
                WoWDXInject.ShowOverlayText(text, wowIcon, color);
            }
        }

    }
}
