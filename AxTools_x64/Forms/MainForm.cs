using Components;
using Components.TaskbarProgressbar;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.Services;
using AxTools.Services.PingerHelpers;
using AxTools.Updater;
using AxTools.WinAPI;
using AxTools.WoW;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.Plugins;
using BrightIdeasSoftware;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.WoW.PluginSystem.API;
using Settings = AxTools.Helpers.Settings;

namespace AxTools.Forms
{
    internal partial class MainForm : BorderedMetroForm
    {
        internal static MainForm Instance;
        private bool isClosing;
        private readonly Settings settings = Settings.Instance;
        internal static event Action ClosingEx;
        internal static int UIThreadID;

        internal MainForm()
        {
            Log.Info("[AxTools] Initializing main window...");
            InitializeComponent();
            StyleManager.Style = Settings.Instance.StyleColor;
            Icon = Resources.AppIcon;
            Closing += MainFormClosing;
            notifyIconMain.Icon = Resources.AppIcon;
            tabControl.SelectedIndex = 0;
            linkEditWowAccounts.Location = new Point(metroTabPage1.Size.Width/2 - linkEditWowAccounts.Size.Width/2, linkEditWowAccounts.Location.Y);
            cmbboxAccSelect.MouseWheel += delegate(object sender, MouseEventArgs args) { ((HandledMouseEventArgs) args).Handled = true; };
            cmbboxAccSelect.KeyDown += delegate(object sender, KeyEventArgs args) { args.SuppressKeyPress = true; };
            cmbboxAccSelect.Location = new Point(metroTabPage1.Size.Width/2 - cmbboxAccSelect.Size.Width/2, cmbboxAccSelect.Location.Y);
            progressBarAddonsBackup.Size = linkBackup.Size;
            progressBarAddonsBackup.Location = linkBackup.Location;
            progressBarAddonsBackup.Visible = false;
            metroToolTip1.SetToolTip(buttonStartStopPlugin, "Check plugins you want to enable and\r\nclick this button to launch.\r\nDouble click on a row to open settings dialog");

            UIThreadID = Thread.CurrentThread.ManagedThreadId;
            PostInvoke(AfterInitializing);
            Log.Info(string.Format("[AxTools] Registered for: {0}", Settings.Instance.UserID));
            Log.Info("[AxTools] Initial loading is finished");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32Consts.WM_QUERYENDSESSION)
            {
                if (!isClosing)
                {
                    if (InvokeRequired)
                        Invoke(new Action(Close));
                    else
                        Close();
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }
        
        #region MainFormEvents

        private void MainFormClosing(object sender, CancelEventArgs e)
        {
            isClosing = true;
            if (ClosingEx != null)
            {
                ClosingEx();
            }
            // Close all children forms
            Form[] forms = Application.OpenForms.Cast<Form>().Where(i => i.GetType() != typeof (MainForm) && i.GetType() != typeof (MetroFlatDropShadow)).ToArray();
            foreach (Form i in forms)
            {
                i.Close();
            }
            //
            settings.WoWPluginHotkeyChanged -= WoWPluginHotkeyChanged;
            PluginManagerEx.PluginStateChanged -= PluginManagerOnPluginStateChanged;
            Pinger.StatChanged -= Pinger_DataChanged;
            Pinger.IsEnabledChanged -= PingerOnStateChanged;
            AddonsBackup.IsRunningChanged -= AddonsBackup_IsRunningChanged;
            //
            settings.MainWindowLocation = Location;
            //save settings
            settings.SaveJSON();
            //
            Clicker.Stop();
            //stop timers
            AddonsBackup.StopService();
            TrayIconAnimation.Close();
            Pinger.Enabled = false;
            //stop watching process trace
            WoWProcessManager.StopWatcher();
            Log.Info("WoW processes trace watching is stopped");
            // release hook 
            if (WoWManager.Hooked)
            {
                WoWManager.Unhook();
            }
            foreach (WowProcess i in WoWProcessManager.List)
            {
                string name = i.ProcessName;
                i.Dispose();
                Log.Info(string.Format("{0}:{1} :: [WoW hook] Memory manager disposed", name, i.ProcessID));
            }
            HotkeyManager.KeyPressed -= KeyboardHookKeyDown;
            HotkeyManager.RemoveKeys(typeof(PluginManagerEx).ToString());
            Log.Info("AxTools closed");
            SendLogToDeveloper();
        }

        private void SendLogToDeveloper()
        {
            TaskDialogButton yesNo = TaskDialogButton.Yes + (int) TaskDialogButton.No;
            TaskDialog taskDialog = new TaskDialog("There were errors during runtime", "AxTools", "Do you want to send log file to developer?", yesNo, TaskDialogIcon.Warning);
            if (Log.HaveErrors && Utils.InternetAvailable && taskDialog.Show(this).CommonButton == Result.Yes && File.Exists(Globals.LogFileName))
            {
                try
                {
                    Log.UploadLog(null);
                }
                catch (Exception ex)
                {
                    this.TaskDialog("Log file sending error", ex.Message, NotifyUserType.Error);
                }
            }
        }

        private void NotifyIconMainMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (settings.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private async void AfterInitializing()
        {
            PluginManagerEx.PluginStateChanged += PluginManagerOnPluginStateChanged;
            PluginManagerEx.PluginsLoaded += PluginManagerExOnPluginsLoaded;
            Task pluginsLoader = PluginManagerEx.LoadPluginsAsync();    // start loading plugins
            Location = settings.MainWindowLocation;                     // should do it here...
            OnActivated(EventArgs.Empty);                               // ...and calling OnActivated is necessary
            WaitingOverlay startupOverlay = WaitingOverlay.Show(this);  // form is visible, placing overlay
            WoWAccounts_CollectionChanged(null, null);                  // initial load wowaccounts
            // styling, events attaching...
            checkBoxStartVenriloWithWow.Checked = settings.VentriloStartWithWoW;
            checkBoxStartTeamspeak3WithWow.Checked = settings.TS3StartWithWoW;
            checkBoxStartRaidcallWithWow.Checked = settings.RaidcallStartWithWoW;
            checkBoxStartMumbleWithWow.Checked = settings.MumbleStartWithWoW;
            buttonStartStopPlugin.Text = string.Format("{0} [{1}]", "Start", settings.WoWPluginHotkey);
            settings.WoWPluginHotkeyChanged += WoWPluginHotkeyChanged;
            Pinger.StatChanged += Pinger_DataChanged;
            Pinger.IsEnabledChanged += PingerOnStateChanged;
            AddonsBackup.IsRunningChanged += AddonsBackup_IsRunningChanged;
            WoWAccount.AllAccounts.CollectionChanged += WoWAccounts_CollectionChanged;
            olvPlugins.CellToolTipShowing += objectListView1_CellToolTipShowing;
            olvPlugins.DoubleClick += OlvPluginsOnDoubleClick;
            // end of styling, events attaching...
            AddonsBackup.StartService();                                // start backup service
            Pinger.Enabled = settings.PingerServerID != 0;              // set pinger
            WoWProcessManager.StartWatcher();                           // start WoW spy
            TrayIconAnimation.Initialize(notifyIconMain);               // initialize tray animation
            HotkeyManager.KeyPressed += KeyboardHookKeyDown;            // start keyboard listener
            HotkeyManager.AddKeys(typeof(PluginManagerEx).ToString(), settings.WoWPluginHotkey);
            await pluginsLoader;                                        // waiting for plugins to be loaded
            startupOverlay.Close();                                     // close startup overkay
            Changes.ShowChangesIfNeeded();                              // show changes overview dialog if needed
            UpdaterService.Start();                                     // start updater service
            Log.Info("[AxTools] All start-up routines are finished");   // situation normal :)
        }

        private void linkSettings_Click(object sender, EventArgs e)
        {
            AppSettings appSettings = Utils.FindForm<AppSettings>();
            if (appSettings != null)
            {
                appSettings.Activate();
            }
            else
            {
                new AppSettings().Show();
            }
        }

        private void linkTitle_Click(object sender, EventArgs e)
        {
            contextMenuStripMain.Show(linkTitle, linkTitle.Size.Width, 0);
        }

        private void linkPing_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Pinger.Enabled = false;
                    linkPing.Text = "cleared";
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(1000);
                        Pinger.Enabled = true;
                    });
                    break;
                case MouseButtons.Right:
                    int pingerTabPageIndex = 5;
                    new AppSettings(pingerTabPageIndex).ShowDialog(this);
                    break;
            }
        }

        #endregion

        #region TrayContextMenu

        private void CreateTrayContextMenu()
        {
            contextMenuStripMain.Items.Clear();
            contextMenuStripMain.Items.AddRange(new ToolStripItem[]
            {
                woWRadarToolStripMenuItem,
                luaConsoleToolStripMenuItem,
                blackMarketTrackerToolStripMenuItem,
                toolStripSeparator2
            });
            Type[] nativePlugins = { typeof(Fishing), typeof(FlagReturner), typeof(GoodsDestroyer) };
            foreach (IPlugin i in PluginManagerEx.LoadedPlugins.Where(i => nativePlugins.Contains(i.GetType())))
            {
                IPlugin plugin = i;
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon, null, "NativeN" + plugin.Name);
                toolStripMenuItem.MouseDown += delegate(object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Left) TrayContextMenu_PluginClicked(plugin);
                    else if (plugin.ConfigAvailable) plugin.OnConfig();
                    contextMenuStripMain.Hide();
                };
                toolStripMenuItem.ToolTipText = plugin.ConfigAvailable ? "Left click to start only this plugin\r\nRight click to open settings" : "Left click to start only this plugin";
                contextMenuStripMain.Items.Add(toolStripMenuItem);
            }
            if (PluginManagerEx.LoadedPlugins.Count() > nativePlugins.Length)
            {
                ToolStripMenuItem customPlugins = contextMenuStripMain.Items.Add("Custom plugins") as ToolStripMenuItem;
                if (customPlugins != null)
                {
                    foreach (IPlugin i in PluginManagerEx.LoadedPlugins.Where(i => !nativePlugins.Contains(i.GetType())))
                    {
                        IPlugin plugin = i;
                        try
                        {
                            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon, null, "NativeN" + plugin.Name);
                            toolStripMenuItem.MouseDown += delegate(object sender, MouseEventArgs args)
                            {
                                if (args.Button == MouseButtons.Left) TrayContextMenu_PluginClicked(plugin);
                                else if (plugin.ConfigAvailable) plugin.OnConfig();
                                contextMenuStripMain.Hide();
                            };
                            toolStripMenuItem.ToolTipText = plugin.ConfigAvailable ? "Left click to start only this plugin\r\nRight click to open settings" : "Left click to start only this plugin";
                            customPlugins.DropDownItems.Add(toolStripMenuItem);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("Error occured while building tray icon for plugin <{0}>: {1}", plugin.Name, ex.Message));
                        }
                    }
                }
            }
            ToolStripMenuItem launchWoW = new ToolStripMenuItem("World of Warcraft", null, delegate
            {
                contextMenuStripMain.Hide();
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
            ToolStripMenuItem unloadInject = new ToolStripMenuItem("Detach from current WoW process", null, delegate
            {
                contextMenuStripMain.Hide();
                if (WoWManager.Hooked)
                {
                    WoWManager.Unhook();
                }
                else
                {
                    Notify.Balloon("Warning", "AxTools isn't attached to any WoW process", NotifyUserType.Warn, true);
                }
            }, "Detach from current WoW process");
            contextMenuStripMain.Items.AddRange(new ToolStripItem[]
            {
                stopActivePluginorPresshotkeyToolStripMenuItem,
                toolStripSeparator1,
                unloadInject,
                new ToolStripSeparator(),
                launchWoW,
                new ToolStripSeparator(),
                launchWoWToolStripMenuItem
            });
        }

        private void UpdateTrayContextMenu()
        {
            foreach (IPlugin plugin in PluginManagerEx.LoadedPlugins)
            {
                ToolStripItem[] items = contextMenuStripMain.Items.Find("NativeN" + plugin.Name, true);
                foreach (ToolStripItem toolStripItem in items)
                {
                    ToolStripMenuItem item = toolStripItem as ToolStripMenuItem;
                    if (item != null)
                    {
                        item.ShortcutKeyDisplayString = olvPlugins.CheckedObjects.Cast<IPlugin>().FirstOrDefault(i => i.Name == item.Text) != null ? settings.WoWPluginHotkey.ToString() : null;
                        item.Enabled = !PluginManagerEx.RunningPlugins.Any();
                    }
                }
            }
            stopActivePluginorPresshotkeyToolStripMenuItem.Enabled = PluginManagerEx.RunningPlugins.Any();
            stopActivePluginorPresshotkeyToolStripMenuItem.ShortcutKeyDisplayString = settings.WoWPluginHotkey.ToString();
        }

        private void WoWRadarToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartWoWModule<WowRadar>();
        }

        private void blackMarketTrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartWoWModule<BlackMarket>();
        }

        private void LuaConsoleToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartWoWModule<LuaConsole>();
        }

        private void ExitAxToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isClosing)
            {
                if (InvokeRequired) Invoke(new Action(Close));
                else Close();
            }
        }

        private void stopActivePluginorPresshotkeyToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (PluginManagerEx.RunningPlugins.Any())
            {
                SwitchWoWPlugin();
            }
        }

        private void TrayContextMenu_PluginClicked(IPlugin plugin)
        {
            olvPlugins.UncheckAll();
            olvPlugins.CheckObject(plugin);
            if (!WoWManager.Hooked && WoWProcessManager.List.Count != 1)
            {
                Activate();
            }
            SwitchWoWPlugin();
        }

        #endregion

        #region Tab: Home

        private void StartWoW(WoWAccount wowAccount = null)
        {
            WaitingOverlay.Show(this, 1000);
            if (File.Exists(settings.WoWDirectory + "\\Wow-64.exe"))
            {
                Process process = Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = settings.WoWDirectory,
                    FileName = settings.WoWDirectory + "\\Wow-64.exe",
                });
                if (wowAccount != null)
                {
                    new AutoLogin(wowAccount, process).EnterCredentialsASAPAsync();
                }
            }
            else
            {
                this.TaskDialog("WoW client not found or corrupted", "Can't locate \"Wow-64.exe\"", NotifyUserType.Error);
            }
        }

        private void CmbboxAccSelectSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbboxAccSelect.SelectedIndex != -1)
            {
                WoWAccount wowAccount = new WoWAccount(WoWAccount.AllAccounts[cmbboxAccSelect.SelectedIndex].Login, WoWAccount.AllAccounts[cmbboxAccSelect.SelectedIndex].Password);
                StartWoW(wowAccount);
                if (settings.VentriloStartWithWoW && !Process.GetProcessesByName("Ventrilo").Any())
                {
                    StartVentrilo();
                }
                if (settings.TS3StartWithWoW && !Process.GetProcessesByName("ts3client_win64").Any() && !Process.GetProcessesByName("ts3client_win32").Any())
                {
                    StartTS3();
                }
                if (settings.RaidcallStartWithWoW && !Process.GetProcessesByName("raidcall").Any())
                {
                    StartRaidcall();
                }
                if (settings.MumbleStartWithWoW && !Process.GetProcessesByName("mumble").Any())
                {
                    StartMumble();
                }
                cmbboxAccSelect.SelectedIndex = -1;
                cmbboxAccSelect.Invalidate();
            }
        }

        private void linkBackupAddons_Click(object sender, EventArgs e)
        {
            contextMenuStripBackupAndClean.Show(linkBackup, linkBackup.Size.Width, 0);
        }

        private void linkClickerSettings_Click(object sender, EventArgs e)
        {
            if (Clicker.Enabled)
            {
                this.TaskDialog("Clicker settings", "Please switch clicker off before", NotifyUserType.Warn);
            }
            else
            {
                ClickerSettings clickerSettings = Utils.FindForm<ClickerSettings>();
                if (clickerSettings == null)
                {
                    new ClickerSettings().Show(this);
                }
                else
                {
                    clickerSettings.Activate();
                }
            }
        }

        private void linkEditWowAccounts_Click(object sender, EventArgs e)
        {
            new WowAccountsManager().ShowDialog(this);
        }

        private void toolStripMenuItemBackupWoWAddOns_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(AddonsBackup.ManualBackup)
                .ContinueWith(l => this.TaskDialog("Backup is complete", "New archive is placed to [" + settings.WoWAddonsBackupPath + "]", NotifyUserType.Info));
        }

        private void toolStripMenuItemDeployArchive_Click(object sender, EventArgs e)
        {
            AddonsBackupDeploy form = Utils.FindForm<AddonsBackupDeploy>();
            if (form != null)
            {
                form.Show();
            }
            else
            {
                new AddonsBackupDeploy().Show();
            }
        }

        private void toolStripMenuItemOpenWoWLogsFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(settings.WoWDirectory + "\\Logs"))
            {
                Process.Start(settings.WoWDirectory + "\\Logs");
            }
            else
            {
                this.TaskDialog("Can't open WoW logs folder", "It doesn't exist", NotifyUserType.Error);
            }
        }

        #endregion

        #region Tab: VoIP

        private void StartVentrilo()
        {
            if (File.Exists(settings.VentriloDirectory + "\\Ventrilo.exe"))
            {
                Process process = Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = settings.VentriloDirectory,
                    FileName = settings.VentriloDirectory + "\\Ventrilo.exe",
                    Arguments = "-m"
                });
                Task.Factory.StartNew(() =>
                {
                    int counter = 300;
                    while (counter > 0)
                    {
                        if (process != null)
                        {
                            process.Refresh();
                            if (process.MainWindowHandle != (IntPtr)0)
                            {
                                IntPtr windowHandle = NativeMethods.FindWindow(null, "Ventrilo");
                                if (windowHandle != IntPtr.Zero)
                                {
                                    IntPtr connectButtonHandle = NativeMethods.FindWindowEx(windowHandle, IntPtr.Zero, "Button", "C&onnect");
                                    if (connectButtonHandle != IntPtr.Zero)
                                    {
                                        NativeMethods.PostMessage(connectButtonHandle, Win32Consts.WM_BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                                        break;
                                    }
                                }
                            }
                        }
                        Thread.Sleep(100);
                        counter--;
                    }
                });
                Log.Info("Ventrilo process started");
            }
            else
            {
                this.TaskDialog("Executable not found", "Can't locate \"Ventrilo.exe\". Check paths in settings window", NotifyUserType.Error);
            }
        }

        private void StartTS3()
        {
            string ts3Executable;
            if (File.Exists(settings.TS3Directory + "\\ts3client_win64.exe"))
            {
                ts3Executable = settings.TS3Directory + "\\ts3client_win64.exe";
            }
            else if (File.Exists(settings.TS3Directory + "\\ts3client_win32.exe"))
            {
                ts3Executable = settings.TS3Directory + "\\ts3client_win32.exe";
            }
            else
            {
                this.TaskDialog("Executable not found", "Can't locate \"ts3client_win32.exe\"/\"ts3client_win64.exe\". Check paths in settings dialog", NotifyUserType.Error);
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = settings.TS3Directory,
                FileName = ts3Executable,
                Arguments = "-nosingleinstance"
            });
            Log.Info("TS3 process started");
        }

        private void StartRaidcall()
        {
            if (File.Exists(settings.RaidcallDirectory + "\\raidcall.exe"))
            {
                Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = settings.RaidcallDirectory,
                    FileName = settings.RaidcallDirectory + "\\raidcall.exe"
                });
                Log.Info("Raidcall process started");
            }
            else
            {
                Notify.SmartNotify("Executable not found", "Can't locate \"raidcall.exe\". Check paths in settings window", NotifyUserType.Error, false);
            }
        }

        private void StartMumble()
        {
            if (File.Exists(settings.MumbleDirectory + "\\mumble.exe"))
            {
                Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = settings.MumbleDirectory,
                    FileName = settings.MumbleDirectory + "\\mumble.exe"
                });
                Log.Info("Mumble process started");
            }
            else
            {
                Notify.SmartNotify("Executable not found", "Can't locate \"mumble.exe\". Check paths in settings window", NotifyUserType.Error, false);
            }
        }

        private void TileVentriloClick(object sender, EventArgs e)
        {
            StartVentrilo();
        }

        private void TileRaidcallClick(object sender, EventArgs e)
        {
            StartRaidcall();
        }

        private void TileTeamspeak3Click(object sender, EventArgs e)
        {
            StartTS3();
        }

        private void TileMumbleClick(object sender, EventArgs e)
        {
            StartMumble();
            Notify.TrayPopup("AxTools", "1 - Lorem Ipsum is simply dummy text of the printing and typesetting industry.", NotifyUserType.Info, false);
            Notify.TrayPopup("AxTools", "2 - Lorem Ipsum is simply dummy text of the printing and typesetting industry.", NotifyUserType.Info, false);
            Notify.TrayPopup("AxTools", "3 - Lorem Ipsum is simply dummy text of the printing and typesetting industry.", NotifyUserType.Info, false);
            Notify.TrayPopup("AxTools", "4 - Lorem Ipsum is simply dummy text of the printing and typesetting industry.", NotifyUserType.Info, false);
        }

        private void checkBoxStartVenriloWithWow_CheckedChanged(object sender, EventArgs e)
        {
            settings.VentriloStartWithWoW = checkBoxStartVenriloWithWow.Checked;
        }

        private void checkBoxStartRaidcallWithWow_CheckedChanged(object sender, EventArgs e)
        {
            settings.RaidcallStartWithWoW = checkBoxStartRaidcallWithWow.Checked;
        }

        private void checkBoxStartMumbleWithWow_CheckedChanged(object sender, EventArgs e)
        {
            settings.MumbleStartWithWoW = checkBoxStartMumbleWithWow.Checked;
        }

        private void checkBoxStartTeamspeak3WithWow_CheckedChanged(object sender, EventArgs e)
        {
            settings.TS3StartWithWoW = checkBoxStartTeamspeak3WithWow.Checked;
        }

        #endregion

        #region Tab: Modules

        private void StartWoWModule<T>() where T : Form, IWoWModule, new()
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

        private void tileRadar_Click(object sender, EventArgs e)
        {
            StartWoWModule<WowRadar>();
        }

        private void tileLuaConsole_Click(object sender, EventArgs e)
        {
            StartWoWModule<LuaConsole>();
        }

        private void tileBMTracker_Click(object sender, EventArgs e)
        {
            StartWoWModule<BlackMarket>();
        }

        #endregion

        #region Tab: Plug-ins

        private void SwitchWoWPlugin()
        {
            buttonStartStopPlugin.Enabled = false;
            if (!PluginManagerEx.RunningPlugins.Any())
            {
                if (olvPlugins.CheckedObjects.Count != 0)
                {
                    if (WoWManager.Hooked || WoWManager.HookWoWAndNotifyUserIfError())
                    {
                        if (GameFunctions.IsInGame)
                        {
                            PluginManagerEx.StartPlugins();
                        }
                        else
                        {
                            Notify.SmartNotify("Plugin error", "Player isn't logged in", NotifyUserType.Error, true);
                        }
                    }
                }
                else
                {
                    Notify.SmartNotify("Plugin error", "You didn't select valid plugin", NotifyUserType.Error, true);
                }
            }
            else
            {
                try
                {
                    PluginManagerEx.StopPlugins();
                }
                catch
                {
                    Log.Error("Plugin task failed to cancel");
                    Notify.SmartNotify("Plugin error", "Fatal error: please restart AxTools", NotifyUserType.Error, true);
                }
            }
            buttonStartStopPlugin.Enabled = true;
        }

        private void SetupOLVPlugins()
        {
            olvColumn2.AspectToStringConverter = value => (bool) value ? "DblClick" : "";
            olvPlugins.SetObjects(PluginManagerEx.LoadedPlugins);
            foreach (IPlugin i in PluginManagerEx.EnabledPlugins)
            {
                olvPlugins.CheckObject(i);
            }
            olvPlugins.BooleanCheckStateGetter = OlvPlugins_BooleanCheckStateGetter;
            olvPlugins.BooleanCheckStatePutter = OlvPlugins_BooleanCheckStatePutter;
        }

        private void buttonStartStopPlugin_Click(object sender, EventArgs e)
        {
            SwitchWoWPlugin();
        }

        private void objectListView1_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
        {
            IPlugin plugin = e.Model as IPlugin;
            if (plugin != null)
            {
                e.IsBalloon = true;
                e.StandardIcon = ToolTipControl.StandardIcons.InfoLarge;
                e.Title = plugin.Name;
                e.Text = plugin.Description + "\r\nVersion: " + plugin.Version;
            }
        }

        private void OlvPluginsOnDoubleClick(object sender, EventArgs eventArgs)
        {
            IPlugin plugin = olvPlugins.SelectedObject as IPlugin;
            if (plugin != null)
            {
                if (plugin.ConfigAvailable)
                {
                    plugin.OnConfig();
                }
                else
                {
                    this.TaskDialog(plugin.Name, "This plugin hasn't settings dialog", NotifyUserType.Info);
                }
            }
        }

        private bool OlvPlugins_BooleanCheckStateGetter(object rowObject)
        {
            IPlugin plugin = rowObject as IPlugin;
            if (plugin != null)
            {
                return PluginManagerEx.EnabledPlugins.Contains(plugin) || PluginManagerEx.RunningPlugins.Contains(plugin);
            }
            return false;
        }

        private bool OlvPlugins_BooleanCheckStatePutter(object rowObject, bool newValue)
        {
            IPlugin plugin = rowObject as IPlugin;
            if (plugin != null)
            {
                if (newValue)
                {
                    PluginManagerEx.EnablePlugin(plugin);
                    settings.EnabledPluginsList.Add(plugin.Name);
                }
                else
                {
                    PluginManagerEx.DisablePlugin(plugin);
                    settings.EnabledPluginsList.RemoveAll(l => l == plugin.Name);
                }
                BeginInvoke(new MethodInvoker(UpdateTrayContextMenu));
            }
            return newValue;
        }

        private void linkDownloadPlugins_Click(object sender, EventArgs e)
        {
            Process.Start(Globals.PluginsURL);
        }

        #endregion

        #region Events handlers

        private void WoWAccounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            cmbboxAccSelect.Items.Clear();
            if (WoWAccount.AllAccounts.Count > 0)
            {
                cmbboxAccSelect.OverlayText = "Click to launch WoW using autopass...";
                cmbboxAccSelect.Enabled = true;
                foreach (WoWAccount i in WoWAccount.AllAccounts)
                {
                    cmbboxAccSelect.Items.Add(i.Login);
                }
            }
            else
            {
                cmbboxAccSelect.OverlayText = "At least one WoW account is required!";
                cmbboxAccSelect.Enabled = false;
            }

            ToolStripItem[] items = contextMenuStripMain.Items.Find("World of Warcraft", false);
            if (items.Length > 0)
            {
                ToolStripMenuItem launchWoW = (ToolStripMenuItem) items[0];
                launchWoW.DropDownItems.Cast<ToolStripMenuItem>().ToList().ForEach(l => l.Dispose());
                foreach (WoWAccount wowAccount in WoWAccount.AllAccounts)
                {
                    WoWAccount account = wowAccount;
                    launchWoW.DropDownItems.Add(new ToolStripMenuItem(wowAccount.Login, null, delegate
                    {
                        StartWoW(account);
                    }));
                }
            }
        }

        private void PluginManagerOnPluginStateChanged()
        {
            BeginInvoke((MethodInvoker) delegate
            {
                buttonStartStopPlugin.Text = string.Format("{0} [{1}]", !PluginManagerEx.RunningPlugins.Any() ? "Start" : "Stop", settings.WoWPluginHotkey);
                olvPlugins.Enabled = !PluginManagerEx.RunningPlugins.Any();
                UpdateTrayContextMenu();
            });
        }

        private void PluginManagerExOnPluginsLoaded()
        {
            BeginInvoke((MethodInvoker) delegate
            {
                SetupOLVPlugins();
                CreateTrayContextMenu();
                UpdateTrayContextMenu();
            });
            PluginManagerEx.PluginsLoaded -= PluginManagerExOnPluginsLoaded;
        }

        private void WoWPluginHotkeyChanged(Keys key)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                buttonStartStopPlugin.Text = string.Format("{0} [{1}]", !PluginManagerEx.RunningPlugins.Any() ? "Start" : "Stop", key);
                UpdateTrayContextMenu();
            }));
        }

        private void AddonsBackup_IsRunningChanged(bool isRunning)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                linkBackup.Visible = !isRunning;
                progressBarAddonsBackup.Value = 0;
                progressBarAddonsBackup.Visible = isRunning;
                if (isRunning)
                {
                    AddonsBackup.ProgressPercentageChanged += AddonsBackup_ProgressPercentageChanged;
                }
                else
                {
                    AddonsBackup.ProgressPercentageChanged -= AddonsBackup_ProgressPercentageChanged;
                }
            });
        }

        private void AddonsBackup_ProgressPercentageChanged(int percent)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                progressBarAddonsBackup.Value = percent;
            });
        }

        private void PingerOnStateChanged(bool enabled)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                linkPing.Visible = enabled;
                linkPing.Text = "[n/a]::[n/a]  |";
                TBProgressBar.SetProgressValue(Handle, 1, 1);
                TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.NoProgress);
            }));
        }

        private void Pinger_DataChanged(PingerStat pingResult)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                linkPing.Text = string.Format("[{0}]::[{1}%]  |", pingResult.PingDataIsRelevant ? pingResult.Ping + "ms" : "n/a", pingResult.PacketLoss);
                TBProgressBar.SetProgressValue(Handle, 1, 1);
                if (pingResult.PacketLoss >= settings.PingerVeryBadPacketLoss || pingResult.Ping >= settings.PingerVeryBadPing)
                {
                    TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.Error);
                }
                else if (pingResult.PacketLoss >= settings.PingerBadPacketLoss || pingResult.Ping >= settings.PingerBadPing)
                {
                    TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.Paused);
                }
                else
                {
                    TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.NoProgress);
                }
            });
        }

        private void KeyboardHookKeyDown(Keys key)
        {
            if (key == settings.WoWPluginHotkey)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    if (WoWProcessManager.List.Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
                    {
                        SwitchWoWPlugin();
                    }
                });
            }
        }

        #endregion

    }
}