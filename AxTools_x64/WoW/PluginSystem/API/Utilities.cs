using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WoW.Management;

namespace AxTools.WoW.PluginSystem.API
{
    public static class Utilities
    {
        /// <summary>
        /// Makes a record to the log. WoW process name, process id and plugin name included
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="text"></param>
        public static void LogPrint(this IPlugin plugin, object text)
        {
            Log.Info(string.Format("{0} [Plugin: {1}] {2}", WoWManager.WoWProcess, plugin.Name, text));
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

        /// <summary>
        /// 
        /// </summary>
        public static IntPtr MainWindowHandle
        {
            get
            {
                return MainForm.Instance.Handle;
            }
        }

        public static void RequestPluginsStopAsync()
        {
            Task.Factory.StartNew(PluginManager.StopPlugins);
        }

        public static SingleThreadTimer CreateTimer(this IPlugin plugin, int interval, Action action)
        {
            return new SingleThreadTimer(interval, action) { PluginName = plugin.Name };
        }

        public static string GetRandomString(int size)
        {
            return Utils.GetRandomString(size);
        }

    }
}
