using AxTools.Classes;
using AxTools.Forms;
using AxTools.Properties;
using AxTools.WoW.PluginSystem;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace AxTools.Helpers
{
    internal static class TrayIconAnimation
    {
        private static NotifyIcon _notifyIcon;
        private static readonly Timer _timer = new Timer(500);
        private static readonly object _lock = new object();
        private static Phase _phase = Phase.Clearing;
        private static readonly Icon AppIconPluginOnLuaOn = Icon.FromHandle(Resources.AppIconPluginOnLuaOn.GetHicon());
        private static readonly Icon AppIconPluginOffLuaOn = Icon.FromHandle(Resources.AppIconPluginOffLuaOn.GetHicon());
        private static readonly Icon AppIconPluginOnLuaOff = Icon.FromHandle(Resources.AppIconPluginOnLuaOff.GetHicon());
        private static readonly Icon AppIconNormal = Icon.FromHandle(Resources.AppIcon1.GetHicon());
        private static readonly Icon EmptyIcon = Icon.FromHandle(new Bitmap(1, 1).GetHicon());

        internal static void Initialize(NotifyIcon notifyIcon)
        {
            lock (_lock)
            {
                _notifyIcon = notifyIcon;
                _timer.Elapsed += Timer_OnElapsed;
                _timer.Start();
            }
        }

        internal static void Close()
        {
            lock (_lock)
            {
                _timer.Elapsed -= Timer_OnElapsed;
                _timer.Stop();
                AppIconPluginOnLuaOn.Dispose();
                AppIconPluginOffLuaOn.Dispose();
                AppIconPluginOnLuaOff.Dispose();
                AppIconNormal.Dispose();
                EmptyIcon.Dispose();
            }
        }

        private static void Timer_OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (_lock)
            {
                if (_phase == Phase.Clearing)
                {
                    _phase = Phase.Actualizing;
                    ClearingPhase();
                }
                else
                {
                    _phase = Phase.Clearing;
                    ActualizingPhase();
                }
            }
        }

        private static void ClearingPhase()
        {
            // ReSharper disable RedundantCheckBeforeAssignment
            if (Clicker.Enabled)
            {
                _notifyIcon.Icon = EmptyIcon;
            }
            else if (_notifyIcon.Icon != AppIconNormal)
            {
                _notifyIcon.Icon = AppIconNormal;
            }
            // ReSharper restore RedundantCheckBeforeAssignment
        }

        private static void ActualizingPhase()
        {
            // ReSharper disable RedundantCheckBeforeAssignment
            if (LuaConsole.TimerEnabled && PluginManager.ActivePlugin != null)
            {
                _notifyIcon.Icon = AppIconPluginOnLuaOn;
            }
            else if (LuaConsole.TimerEnabled)
            {
                _notifyIcon.Icon = AppIconPluginOffLuaOn;
            }
            else if (PluginManager.ActivePlugin != null)
            {
                _notifyIcon.Icon = AppIconPluginOnLuaOff;
            }
            else if (_notifyIcon.Icon != AppIconNormal)
            {
                _notifyIcon.Icon = AppIconNormal;
            }
            // ReSharper restore RedundantCheckBeforeAssignment
        }

        private enum Phase
        {
            Clearing,
            Actualizing,
        }

    }
}
