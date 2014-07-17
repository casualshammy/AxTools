using System;
using System.Linq;
using AxTools.Classes;
using AxTools.Forms;
using AxTools.Properties;
using AxTools.WoW;
using AxTools.WoW.Management;
using AxTools.WoW.PluginSystem;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using AxTools.WoW.PluginSystem.Plugins;
using Settings = AxTools.Classes.Settings;
using Timer = System.Timers.Timer;

namespace AxTools.Services
{
    internal static class TrayIcon
    {
        private static NotifyIcon _notifyIcon;
        private static ContextMenuStrip _contextMenuStrip;
        private static readonly Timer _timer = new Timer(500);
        private static readonly object _lock = new object();
        private static Phase _phase = Phase.Clearing;
        private static readonly Icon AppIconPluginOnLuaOn = Icon.FromHandle(Resources.AppIconPluginOnLuaOn.GetHicon());
        private static readonly Icon AppIconPluginOffLuaOn = Icon.FromHandle(Resources.AppIconPluginOffLuaOn.GetHicon());
        private static readonly Icon AppIconPluginOnLuaOff = Icon.FromHandle(Resources.AppIconPluginOnLuaOff.GetHicon());
        private static readonly Icon AppIconNormal = Icon.FromHandle(Resources.AppIcon1.GetHicon());
        private static readonly Icon EmptyIcon = Icon.FromHandle(new Bitmap(1, 1).GetHicon());

        internal static void Initialize()
        {
            lock (_lock)
            {
                _contextMenuStrip = new ContextMenuStrip();
                _contextMenuStrip.Items.AddRange(new ToolStripItem[]
                {
                    new ToolStripMenuItem("WoW Radar", Resources.radar, (sender, args) => StartWoWModule<WowRadar>()),
                    new ToolStripMenuItem("Lua console", Resources.lua1, (sender, args) => StartWoWModule<LuaConsole>()),
                    new ToolStripMenuItem("Black Market tracker", Resources.BlackMarket, (sender, args) => StartWoWModule<BlackMarket>()),
                    new ToolStripSeparator()
                });
                Type[] nativePlugins = { typeof(Fishing), typeof(FlagReturner), typeof(GoodsDestroyer) };
                foreach (IPlugin i in PluginManager.Plugins.Where(i => nativePlugins.Contains(i.GetType())))
                {
                    IPlugin plugin = i;
                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon, (sender, args) => TrayContextMenu_PluginClicked(plugin), plugin.Name);
                    _contextMenuStrip.Items.Add(toolStripMenuItem);
                }
                if (Settings.Instance.WoWPluginEnableCustom && PluginManager.Plugins.Count > nativePlugins.Length)
                {
                    ToolStripMenuItem customPlugins = _contextMenuStrip.Items.Add("Custom plugins") as ToolStripMenuItem;
                    if (customPlugins != null)
                    {
                        foreach (IPlugin i in PluginManager.Plugins.Where(i => !nativePlugins.Contains(i.GetType())))
                        {
                            IPlugin plugin = i;
                            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon, (sender, args) => TrayContextMenu_PluginClicked(plugin), plugin.Name);
                            customPlugins.DropDownItems.Add(toolStripMenuItem);
                        }
                    }
                }
                ToolStripMenuItem launchWoW = new ToolStripMenuItem("World of Warcraft", null, delegate
                {
                    _contextMenuStrip.Hide();
                    StartWoW();
                }, "World of Warcraft");
                foreach (WoWAccount wowAccount in WoWAccount.AllAccounts)
                {
                    WoWAccount account = wowAccount;
                    launchWoW.DropDownItems.Add(new ToolStripMenuItem(wowAccount.Login, null, delegate
                    {
                        StartWoW(account);
                    }));
                }
                _contextMenuStrip.Items.AddRange(new ToolStripItem[]
                {
                    new ToolStripMenuItem("Stop active plug-in", Resources.radar, (sender, args) =>
                    {
                        if (PluginManager.ActivePlugin != null)
                        {
                            SwitchWoWPlugin();
                        }
                    }, "Stop active plug-in"),
                    new ToolStripSeparator(),
                    launchWoW,
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Exit", Resources.close_26px, (sender, args) => {/* todo */})
                });

                _notifyIcon = new NotifyIcon {Icon = AppIconNormal, Text = "AxTools", Visible = true, ContextMenuStrip = _contextMenuStrip};
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
                _notifyIcon.Dispose();
                _contextMenuStrip.Dispose();
            }
        }

        internal static void ShowNotifyIconMessage(string title, string text, ToolTipIcon icon)
        {
            _notifyIcon.ShowBalloonTip(30000, title, text, icon);
        }

        #region WoWModules

        private static void StartWoWModule<T>() where T : Form, IWoWModule, new()
        {
            if (!WoWManager.Hooked)
            {
                if (WoWManager.HookWoWAndNotifyUserIfError())
                {
                    new T().Show();
                }
            }
            else
            {
                T form = Utils.FindForm<T>();
                if (form != null)
                {
                    form.Activate();
                }
                else
                {
                    new T().Show();
                }
            }
        }

        #endregion

        #region Plugins

        private void SwitchWoWPlugin()
        {
            buttonStartStopPlugin.Enabled = false;
            if (PluginManager.ActivePlugin == null)
            {
                if (comboBoxWowPlugins.SelectedIndex != -1)
                {
                    if (WoWManager.Hooked || WoWManager.HookWoWAndNotifyUserIfError())
                    {
                        if (WoWManager.WoWProcess.IsInGame)
                        {
                            PluginManager.StartPlugin(PluginManager.Plugins.First(i => i.Name == comboBoxWowPlugins.Text));
                        }
                        else
                        {
                            Utils.NotifyUser("Plugin error", "Player isn't logged in", NotifyUserType.Error, true);
                        }
                    }
                }
                else
                {
                    Utils.NotifyUser("Plugin error", "You didn't select valid plugin", NotifyUserType.Error, true);
                }
            }
            else
            {
                try
                {
                    PluginManager.StopPlugin();
                    //GC.Collect();
                }
                catch
                {
                    Log.Print("Plugin task failed to cancel", true);
                    Utils.NotifyUser("Plugin error", "Fatal error: please restart AxTools", NotifyUserType.Error, true);
                }
            }
            buttonStartStopPlugin.Enabled = true;
        }

        private static void TrayContextMenu_PluginClicked(IPlugin plugin)
        {
            comboBoxWowPlugins.SelectedIndex = PluginManager.Plugins.IndexOf(plugin);
            if (!WoWManager.Hooked && WowProcess.GetAllWowProcesses().Count != 1)
            {
                Activate();
            }
            SwitchWoWPlugin();
        }

        #endregion

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
