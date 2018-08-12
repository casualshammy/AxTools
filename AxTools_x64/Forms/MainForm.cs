using AxTools.Helpers;

using AxTools.Services;
using AxTools.Services.PingerHelpers;
using AxTools.Updater;
using AxTools.WinAPI;
using AxTools.WoW;
using AxTools.WoW.PluginSystem;
using BrightIdeasSoftware;
using Components.Forms;
using Components.TaskbarProgressbar;
using KeyboardWatcher;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using Settings2 = AxTools.Helpers.Settings2;

namespace AxTools.Forms
{
    internal partial class MainForm : BorderedMetroForm
    {
        internal static MainForm Instance;
        internal MultiLock WoWLaunchLock = new MultiLock();
        private bool isClosing;
        private readonly Settings2 settings = Settings2.Instance;
        private static readonly Log2 log = new Log2("MainWindow");
        private System.Threading.Timer nextBackupTimer;

        internal MainForm()
        {
            log.Info("Initializing main window...");
            InitializeComponent();
            StyleManager.Style = Settings2.Instance.StyleColor;
            Icon = AxTools.Helpers.Resources.ApplicationIcon;
            Closing += MainFormClosing;
            notifyIconMain.Icon = AxTools.Helpers.Resources.ApplicationIcon;
            tabControl.SelectedIndex = 0;
            linkEditWowAccounts.Location = new Point(metroTabPage1.Size.Width / 2 - linkEditWowAccounts.Size.Width / 2, linkEditWowAccounts.Location.Y);
            cmbboxAccSelect.Location = new Point(metroTabPage1.Size.Width / 2 - cmbboxAccSelect.Size.Width / 2, cmbboxAccSelect.Location.Y);
            progressBarAddonsBackup.Size = linkBackup.Size;
            progressBarAddonsBackup.Location = linkBackup.Location;
            progressBarAddonsBackup.Visible = false;
            SetupControls();
            SetupEvents();
            SetTooltips();
            PostInvoke(AfterInitializingAsync);
            log.Info("MainWindow is constructed");
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

        private void SetTooltips()
        {
            metroToolTip1.SetToolTip(buttonStartStopPlugin, "Click this button to set up individual hotkey for each plugin");
        }

        private void SetupControls()
        {
            checkBoxTwitch.Checked = settings.StartTwitchWithWoW;
            nextBackupTimer = new System.Threading.Timer(delegate
            {
                TimeSpan duration = new TimeSpan(settings.WoWAddonsBackupMinimumTimeBetweenBackup, 0, 0) - (DateTime.UtcNow - settings.WoWAddonsBackupLastDate);
                menuItemNextBackupTime.Text = duration.Ticks <= 0 ? "Backup is running or deferred" : $"Next backup in {duration:%d} day(s), {duration:hh\\:mm\\:ss}";
            }, null, 0, 1000);
        }

        private void SetupEvents()
        {
            cmbboxAccSelect.MouseWheel += delegate (object sender, MouseEventArgs args) { ((HandledMouseEventArgs)args).Handled = true; };
            cmbboxAccSelect.KeyDown += delegate (object sender, KeyEventArgs args) { args.SuppressKeyPress = true; };
            settings.PluginHotkeysChanged += WoWPluginHotkeyChanged;
            Pinger.StatChanged += Pinger_DataChanged;
            Pinger.IsEnabledChanged += PingerOnStateChanged;
            AddonsBackup.IsRunningChanged += AddonsBackup_IsRunningChanged;
            WoWAccount2.AllAccounts.CollectionChanged += WoWAccounts_CollectionChanged;
            olvPlugins.CellToolTipShowing += ObjectListView1_CellToolTipShowing;
            olvPlugins.DoubleClick += OlvPluginsOnDoubleClick;
            PluginManagerEx.PluginStateChanged += PluginManagerOnPluginStateChanged;
            PluginManagerEx.PluginLoaded += PluginManagerExOnPluginLoaded;
            PluginManagerEx.PluginUnloaded += PluginManagerExOnPluginLoaded;
            HotkeyManager.KeyPressed += KeyboardHookKeyDown;
        }

        private void MainFormClosing(object sender, CancelEventArgs e)
        {
            isClosing = true;
            // Close all children forms
            Form[] forms = Application.OpenForms.Cast<Form>().Where(i => i.GetType() != typeof(MainForm) && i.GetType() != typeof(MetroFlatDropShadow)).ToArray();
            foreach (Form i in forms)
            {
                i.Close();
            }
            //
            settings.PluginHotkeysChanged -= WoWPluginHotkeyChanged;
            PluginManagerEx.PluginStateChanged -= PluginManagerOnPluginStateChanged;
            Pinger.StatChanged -= Pinger_DataChanged;
            Pinger.IsEnabledChanged -= PingerOnStateChanged;
            AddonsBackup.IsRunningChanged -= AddonsBackup_IsRunningChanged;
            AddonsBackup.ProgressPercentageChanged -= AddonsBackup_ProgressPercentageChanged;
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
            log.Info("WoW processes trace watching is stopped");
            HotkeyManager.KeyPressed -= KeyboardHookKeyDown;
            HotkeyManager.RemoveKeys(typeof(PluginManagerEx).ToString());
            log.Info("Closed");
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

        private async void AfterInitializingAsync()
        {
            Task pluginsLoader = PluginManagerEx.LoadPluginsAsync();    // start loading plugins
            Location = settings.MainWindowLocation;                     // should do it here...
            OnActivated(EventArgs.Empty);                               // ...and calling OnActivated is necessary
            // form is visible, placing overlay
            WaitingOverlay startupOverlay = new WaitingOverlay(this, "Load WoW accounts...", settings.StyleColor).Show();
            WoWAccounts_CollectionChanged(null, null);                  // initial load wowaccounts
            // searching for wow client
            startupOverlay.Label = "Looking for WoW client...";
            await Task.Run((Action)CheckWoWDirectoryIsValid);
            // styling, events attaching...
            checkBoxStartVenriloWithWow.Checked = settings.VentriloStartWithWoW;
            checkBoxStartTeamspeak3WithWow.Checked = settings.TS3StartWithWoW;
            checkBoxStartRaidcallWithWow.Checked = settings.RaidcallStartWithWoW;
            checkBoxStartMumbleWithWow.Checked = settings.MumbleStartWithWoW;
            // end of styling, events attaching...
            startupOverlay.Label = "Starting addons backup service...";
            AddonsBackup.StartService();                                // start backup service
            startupOverlay.Label = "Starting pinger...";
            Pinger.Enabled = settings.PingerServerID != 0;              // set pinger
            startupOverlay.Label = "Starting WoW process manager...";
            await Task.Run((Action)WoWProcessManager.StartWatcher);     // start WoW spy
            startupOverlay.Label = "Setting tray animation...";
            TrayIconAnimation.Initialize(notifyIconMain);               // initialize tray animation
            //startupOverlay.Label = "Waiting for plug-ins...";
            //await pluginsLoader;                                      // waiting for plugins to be loaded
            startupOverlay.Close();                                     // close startup overkay
            Changes.ShowChangesIfNeeded();                              // show changes overview dialog if needed
            UpdaterService.Start();                                     // start updater service
            log.Info("All start-up routines are finished");             // situation normal :)
        }

        private void LinkSettings_Click(object sender, EventArgs e)
        {
            AppSettings appSettings = Utils.FindForms<AppSettings>().FirstOrDefault();
            if (appSettings != null)
            {
                appSettings.Activate();
            }
            else
            {
                new AppSettings().Show();
            }
        }

        private void LinkTitle_Click(object sender, EventArgs e)
        {
            contextMenuStripMain.Show(linkTitle, linkTitle.Size.Width, 0);
        }

        private void LinkPing_MouseDown(object sender, MouseEventArgs e)
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

        private void CheckWoWDirectoryIsValid()
        {
            if (string.IsNullOrWhiteSpace(settings.WoWDirectory) || !File.Exists(Path.Combine(settings.WoWDirectory, "Wow.exe")) || !File.Exists(Path.Combine(settings.WoWDirectory, "WoW.mfil")))
            {
                foreach (var drive in DriveInfo.GetDrives().Where(l => l.DriveType == DriveType.Fixed))
                {
                    var path = Utils.FindFiles(drive.Name, "Wow.exe", 5).Select(Path.GetDirectoryName).Intersect(Utils.FindFiles(drive.Name, "WoW.mfil", 5).Select(Path.GetDirectoryName)).FirstOrDefault();
                    if (path != null)
                    {
                        settings.WoWDirectory = path;
                    }
                }
            }
        }

        #endregion MainFormEvents

        #region TrayContextMenu

        private void RebuildTrayContextMenu()
        {
            foreach (ToolStripItem item in contextMenuStripMain.Items.GetAllToolStripItems().ToArray())
                item.Dispose();
            contextMenuStripMain.Items.Clear();
            IPlugin3[] sortedPlugins = PluginManagerEx.GetSortedByUsageListOfPlugins().ToArray();
            IPlugin3[] topUsedPlugins = sortedPlugins.Take(3).ToArray();
            foreach (IPlugin3 i in topUsedPlugins)
            {
                IPlugin3 plugin = i;
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon) { Tag = plugin };
                toolStripMenuItem.MouseDown += delegate (object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Left) olvPlugins.CheckObject(plugin);
                    else if (plugin.ConfigAvailable) plugin.OnConfig();
                    contextMenuStripMain.Hide();
                };
                toolStripMenuItem.ToolTipText = plugin.ConfigAvailable ? "Left click to start this plugin\r\nRight click to open settings" : "Left click to start this plugin";
                toolStripMenuItem.ShortcutKeyDisplayString = settings.PluginHotkeys.ContainsKey(plugin.Name) ? "[" + settings.PluginHotkeys[plugin.Name] + "]" : null;
                toolStripMenuItem.Enabled = PluginManagerEx.RunningPlugins.All(l => l.Name != toolStripMenuItem.Text);
                contextMenuStripMain.Items.Add(toolStripMenuItem);
            }
            if (sortedPlugins.Length > topUsedPlugins.Length)
            {
                if (contextMenuStripMain.Items.Add("Other plugins") is ToolStripMenuItem customPlugins)
                {
                    foreach (IPlugin3 i in sortedPlugins.Where(i => !topUsedPlugins.Select(l => l.Name).Contains(i.Name)))
                    {
                        IPlugin3 plugin = i;
                        try
                        {
                            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon) { Tag = plugin };
                            toolStripMenuItem.MouseDown += delegate (object sender, MouseEventArgs args)
                            {
                                if (args.Button == MouseButtons.Left) olvPlugins.CheckObject(plugin);
                                else if (plugin.ConfigAvailable) plugin.OnConfig();
                                contextMenuStripMain.Hide();
                            };
                            toolStripMenuItem.ToolTipText = plugin.ConfigAvailable ? "Left click to start this plugin\r\nRight click to open settings" : "Left click to start this plugin";
                            toolStripMenuItem.ShortcutKeyDisplayString = settings.PluginHotkeys.ContainsKey(plugin.Name) ? "[" + settings.PluginHotkeys[plugin.Name] + "]" : null;
                            toolStripMenuItem.Enabled = PluginManagerEx.RunningPlugins.All(l => l.Name != toolStripMenuItem.Text);
                            customPlugins.DropDownItems.Add(toolStripMenuItem);
                        }
                        catch (Exception ex)
                        {
                            log.Error($"Error occured while building tray icon for plugin <{plugin.Name}>: {ex.Message}");
                        }
                    }
                }
            }
            ToolStripMenuItem launchWoW = new ToolStripMenuItem("World of Warcraft", null, delegate
            {
                contextMenuStripMain.Hide();
                StartWoW();
            }, "World of Warcraft");
            foreach (WoWAccount2 wowAccount in WoWAccount2.AllAccounts)
            {
                WoWAccount2 account = wowAccount;
                launchWoW.DropDownItems.Add(new ToolStripMenuItem(wowAccount.GetLogin(), null, delegate
                {
                    StartWoW(account);
                }));
            }
            contextMenuStripMain.Items.Add(new ToolStripSeparator());
            contextMenuStripMain.Items.Add(launchWoW);
            contextMenuStripMain.Items.Add(new ToolStripSeparator());
            contextMenuStripMain.Items.Add(new ToolStripMenuItem("Exit", AxTools.Helpers.Resources.Close, (o, e) =>
            {
                if (!isClosing)
                {
                    if (InvokeRequired) Invoke(new Action(Close));
                    else Close();
                }
            }));
        }

        #endregion TrayContextMenu

        #region Tab: Home

        private void StartWoW(WoWAccount2 wowAccount = null)
        {
            new WaitingOverlay(this, "Please wait...", settings.StyleColor, 1000).Show();
            if (File.Exists(settings.WoWDirectory + "\\Wow.exe"))
            {
                Process process = Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = settings.WoWDirectory,
                    FileName = settings.WoWDirectory + "\\Wow.exe",
                });
                if (wowAccount != null)
                {
                    new AutoLogin(wowAccount, process).EnterCredentialsASAPAsync();
                }
            }
            else
            {
                this.TaskDialog("WoW client not found or corrupted", "Can't locate \"Wow.exe\"", NotifyUserType.Error);
            }
        }

        private async void CmbboxAccSelectSelectedIndexChanged(object sender, EventArgs e)
        {
            WaitingOverlay overlay = new WaitingOverlay(this, "Waiting for background tasks (60 sec)", settings.StyleColor).Show();
            // wait 60 sec
            await WoWLaunchLock.WaitForLocksAsync(1000 * 60);
            overlay.Close();
            if (WoWLaunchLock.HasLocks)
            {
                Notify.TrayPopup("Warning", "Some background task trying to prevent WoW client from starting", NotifyUserType.Warn, true);
            }
            if (cmbboxAccSelect.SelectedIndex != -1)
            {
                if (settings.VentriloStartWithWoW && !Process.GetProcessesByName("Ventrilo").Any())
                {
                    InvokeOnClick(tileVentrilo, null);
                }
                if (settings.TS3StartWithWoW && !Process.GetProcessesByName("ts3client_win64").Any() && !Process.GetProcessesByName("ts3client_win32").Any())
                {
                    InvokeOnClick(tileTeamspeak3, null);
                }
                if (settings.RaidcallStartWithWoW && !Process.GetProcessesByName("raidcall").Any())
                {
                    InvokeOnClick(tileRaidcall, null);
                }
                if (settings.MumbleStartWithWoW && !Process.GetProcessesByName("mumble").Any())
                {
                    InvokeOnClick(tileMumble, null);
                }
                if (settings.StartTwitchWithWoW && !Process.GetProcessesByName("Twitch").Any())
                {
                    InvokeOnClick(tileExtTwitch, null);
                }
                StartWoW(WoWAccount2.AllAccounts[cmbboxAccSelect.SelectedIndex]);
                cmbboxAccSelect.SelectedIndex = -1;
                cmbboxAccSelect.Invalidate();
            }
        }

        private void LinkBackupAddons_Click(object sender, EventArgs e)
        {
            contextMenuStripBackupAndClean.Show(linkBackup, linkBackup.Size.Width, 0);
        }

        private void LinkClickerSettings_Click(object sender, EventArgs e)
        {
            if (Clicker.Enabled)
            {
                this.TaskDialog("Clicker settings", "Please switch clicker off before", NotifyUserType.Warn);
            }
            else
            {
                ClickerSettings clickerSettings = Utils.FindForms<ClickerSettings>().FirstOrDefault();
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

        private void LinkEditWowAccounts_Click(object sender, EventArgs e)
        {
            new WowAccountsManager().ShowDialog(this);
        }

        private void ToolStripMenuItemBackupWoWAddOns_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(AddonsBackup.ManualBackup)
                .ContinueWith(l => this.TaskDialog("Backup is complete", "New archive is placed to [" + settings.WoWAddonsBackupPath + "]", NotifyUserType.Info));
        }

        private void ToolStripMenuItemDeployArchive_Click(object sender, EventArgs e)
        {
            AddonsBackupDeploy form = Utils.FindForms<AddonsBackupDeploy>().FirstOrDefault();
            if (form != null)
            {
                form.ActivateBrutal();
            }
            else
            {
                new AddonsBackupDeploy().Show();
            }
        }

        private void ToolStripMenuItemOpenWoWLogsFolder_Click(object sender, EventArgs e)
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

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(settings.WoWAddonsBackupPath))
            {
                Process.Start(settings.WoWAddonsBackupPath);
            }
            else
            {
                Notify.TaskDialog("Can't open backup folder", "It doesn't exist", NotifyUserType.Error);
            }
        }

        #endregion Tab: Home

        #region Tab: VoIP

        private void TileVentriloClick(object sender, EventArgs e)
        {
            try
            {
                VoIP.StartVoIPClient("Ventrilo");
            }
            catch
            {
                Notify.SmartNotify("Cannot launch VoIP client", "Maybe you haven't it installed?", NotifyUserType.Error, false);
            }
        }

        private void TileRaidcallClick(object sender, EventArgs e)
        {
            try
            {
                VoIP.StartVoIPClient("Raidcall");
            }
            catch
            {
                Notify.SmartNotify("Cannot launch VoIP client", "Maybe you haven't it installed?", NotifyUserType.Error, false);
            }
        }

        private void TileTeamspeak3Click(object sender, EventArgs e)
        {
            try
            {
                VoIP.StartVoIPClient("Teamspeak 3");
            }
            catch
            {
                Notify.SmartNotify("Cannot launch VoIP client", "Maybe you haven't it installed?", NotifyUserType.Error, false);
            }
        }

        private void TileMumbleClick(object sender, EventArgs e)
        {
            try
            {
                VoIP.StartVoIPClient("Mumble");
            }
            catch
            {
                Notify.SmartNotify("Cannot launch VoIP client", "Maybe you haven't it installed?", NotifyUserType.Error, false);
            }
        }

        private void TileExtDiscord_Click(object sender, EventArgs e)
        {
            try
            {
                VoIP.StartVoIPClient("Discord");
            }
            catch
            {
                Notify.SmartNotify("Cannot launch VoIP client", "Maybe you haven't it installed?", NotifyUserType.Error, false);
            }
        }

        private void TileExtTwitch_Click(object sender, EventArgs e)
        {
            try
            {
                VoIP.StartVoIPClient("Twitch");
            }
            catch
            {
                Notify.SmartNotify("Cannot launch VoIP client", "Maybe you haven't it installed?", NotifyUserType.Error, false);
            }
        }

        private void CheckBoxStartVenriloWithWow_CheckedChanged(object sender, EventArgs e)
        {
            settings.VentriloStartWithWoW = checkBoxStartVenriloWithWow.Checked;
        }

        private void CheckBoxStartRaidcallWithWow_CheckedChanged(object sender, EventArgs e)
        {
            settings.RaidcallStartWithWoW = checkBoxStartRaidcallWithWow.Checked;
        }

        private void CheckBoxStartMumbleWithWow_CheckedChanged(object sender, EventArgs e)
        {
            settings.MumbleStartWithWoW = checkBoxStartMumbleWithWow.Checked;
        }

        private void CheckBoxStartTeamspeak3WithWow_CheckedChanged(object sender, EventArgs e)
        {
            settings.TS3StartWithWoW = checkBoxStartTeamspeak3WithWow.Checked;
        }

        private void CheckBoxTwitch_CheckedChanged(object sender, EventArgs e)
        {
            settings.StartTwitchWithWoW = checkBoxTwitch.Checked;
        }

        #endregion Tab: VoIP

        #region Tab: Plug-ins
        
        private void ButtonStartStopPlugin_Click(object sender, EventArgs e)
        {
            MainForm_PluginHotkeys form = Utils.FindForms<MainForm_PluginHotkeys>().FirstOrDefault();
            if (form == null)
            {
                new MainForm_PluginHotkeys(PluginManagerEx.GetSortedByUsageListOfPlugins().ToArray()).Show();
            }
            else
            {
                form.Activate();
            }
        }

        private void ObjectListView1_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
        {
            if (e.Model is IPlugin3 plugin)
            {
                e.IsBalloon = true;
                e.StandardIcon = ToolTipControl.StandardIcons.InfoLarge;
                e.Title = plugin.Name;
                e.Text = $"{plugin.Description}\r\nVersion: {plugin.Version}";
            }
        }

        private void OlvPluginsOnDoubleClick(object sender, EventArgs eventArgs)
        {
            if (olvPlugins.SelectedObject is IPlugin3 plugin)
            {
                if (PluginManagerEx.RunningPlugins.All(l => l.Name != plugin.Name))
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
                else
                {
                    this.TaskDialog(plugin.Name, "Unable to edit settings of running plugin", NotifyUserType.Info);
                }
            }
        }

        private bool OlvPlugins_BooleanCheckStateGetter(object rowObject)
        {
            if (rowObject is IPlugin3 plugin)
            {
                return PluginManagerEx.RunningPlugins.Contains(plugin);
            }
            return false;
        }

        private bool OlvPlugins_BooleanCheckStatePutter(object rowObject, bool newValue)
        {
            olvPlugins.Enabled = false;
            try
            {
                if (rowObject is IPlugin3 plugin)
                {
                    if (newValue)
                    {
                        WowProcess process = WoWManager.GetProcess();
                        if (process != null)
                        {
                            if (!PluginManagerEx.RunningPlugins.Contains(plugin))
                            {
                                PluginManagerEx.AddPluginToRunning(plugin, process);
                            }
                            else
                            {
                                this.TaskDialog("Plugin Manager", "You can't launch multiple instances of the same plug-in", NotifyUserType.Error);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        WowProcess process = WoWManager.GetProcess();
                        if (process != null)
                        {
                            if (PluginManagerEx.RunningPlugins.Contains(plugin))
                                PluginManagerEx.RemovePluginFromRunning(plugin);
                            else
                                return true;
                        }
                    }
                    PostInvoke(() => { labelTotalPluginsEnabled.Text = "Plugins running: " + PluginManagerEx.RunningPlugins.Count(); });
                    BeginInvoke(new MethodInvoker(RebuildTrayContextMenu));
                }
                return newValue;
            }
            finally
            {
                olvPlugins.Enabled = true;
            }
        }

        private void LinkDownloadPlugins_Click(object sender, EventArgs e)
        {
            Process.Start(Globals.PluginsURL);
        }

        private void LinkUpdatePlugins_Click(object sender, EventArgs e)
        {
            TaskDialog taskDialog = new TaskDialog("Do you really want to update all plugins?", "AxTools", "AxTools will be restarted", TaskDialogButton.Yes | TaskDialogButton.No, TaskDialogIcon.Warning);
            if (taskDialog.Show(this).CommonButton == Result.Yes)
            {
                Program.Exit += delegate { Process.Start(Application.StartupPath + "\\AxTools.exe", "-update-plugins"); };
                Close();
            }
        }

        #endregion Tab: Plug-ins

        #region Events handlers

        private void WoWAccounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            cmbboxAccSelect.Items.Clear();
            if (WoWAccount2.AllAccounts.Count > 0)
            {
                cmbboxAccSelect.OverlayText = "Click to launch WoW using autopass...";
                cmbboxAccSelect.Enabled = true;
                foreach (WoWAccount2 i in WoWAccount2.AllAccounts)
                {
                    cmbboxAccSelect.Items.Add(i.GetLogin());
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
                ToolStripMenuItem launchWoW = (ToolStripMenuItem)items[0];
                launchWoW.DropDownItems.Cast<ToolStripMenuItem>().ToList().ForEach(l => l.Dispose());
                foreach (WoWAccount2 wowAccount in WoWAccount2.AllAccounts)
                {
                    WoWAccount2 account = wowAccount;
                    launchWoW.DropDownItems.Add(new ToolStripMenuItem(account.GetLogin(), null, delegate
                    {
                        StartWoW(account);
                    }));
                }
            }
        }

        private void PluginManagerOnPluginStateChanged()
        {
            BeginInvoke((MethodInvoker)RebuildTrayContextMenu);
        }

        private void PluginManagerExOnPluginLoaded(IPlugin3 plugin)
        {
            BeginInvoke((MethodInvoker)delegate
           {
               string checkMark = Encoding.UTF8.GetString(new byte[] { 0xE2, 0x9C, 0x93 });
               olvColumn2.AspectToStringConverter = value => (bool)value ? checkMark : "";
               olvColumn2.TextAlign = HorizontalAlignment.Center;
               olvPlugins.SetObjects(PluginManagerEx.GetSortedByUsageListOfPlugins());
               olvPlugins.BooleanCheckStateGetter = OlvPlugins_BooleanCheckStateGetter;
               olvPlugins.BooleanCheckStatePutter = OlvPlugins_BooleanCheckStatePutter;
               RebuildTrayContextMenu();
               labelTotalPluginsEnabled.Text = "Plugins running: 0";
           });
        }

        private void WoWPluginHotkeyChanged()
        {
            BeginInvoke(new MethodInvoker(RebuildTrayContextMenu));
        }

        private void AddonsBackup_IsRunningChanged(bool isRunning)
        {
            BeginInvoke((MethodInvoker)delegate
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
            BeginInvoke((MethodInvoker)delegate
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
            BeginInvoke((MethodInvoker)delegate
           {
               linkPing.Text = $"[{(pingResult.PingDataIsRelevant ? pingResult.Ping + "ms" : "n/a")}]::[{pingResult.PacketLoss}%]  |";
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

        private void KeyboardHookKeyDown(KeyExt key)
        {
            if (WoWProcessManager.Processes.Any(l => l.Value.MainWindowHandle == NativeMethods.GetForegroundWindow()))
            {
                foreach (var pair in settings.PluginHotkeys)
                {
                    if (pair.Value == key)
                    {
                        IPlugin3 plugin = PluginManagerEx.LoadedPlugins.FirstOrDefault(l => l.Name == pair.Key);
                        bool pluginRunning = PluginManagerEx.RunningPlugins.Any(l => l.Name == pair.Key);
                        if (plugin != null)
                        {
                            if (pluginRunning)
                            {
                                PostInvoke(delegate { olvPlugins.UncheckObject(plugin); });
                            }
                            else
                            {
                                PostInvoke(delegate { olvPlugins.CheckObject(plugin); });
                            }
                        }
                    }
                }
            }
        }

        #endregion Events handlers

        
    }
}