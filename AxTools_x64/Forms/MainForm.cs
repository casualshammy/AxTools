using Components.TaskbarProgressbar;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.Services;
using AxTools.Services.PingerHelpers;
using AxTools.Updater;
using AxTools.WinAPI;
using AxTools.WoW;
using AxTools.WoW.PluginSystem;
using BrightIdeasSoftware;
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
using AxTools.WoW.PluginSystem.API;
using Components.Forms;
using Settings = AxTools.Helpers.Settings;
using KeyboardWatcher;

namespace AxTools.Forms
{
    internal partial class MainForm : BorderedMetroForm
    {
        internal static MainForm Instance;
        private bool isClosing;
        private readonly Settings settings = Settings.Instance;
        internal static event Action ClosingEx;
        internal static int UIThreadID;
        internal static MultiLock ShutdownLock = new MultiLock();
        private static readonly Log2 log = new Log2("MainWindow");

        internal MainForm()
        {
            log.Info("Initializing main window...");
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
            log.Info(string.Format("Registered for: {0}", Settings.Instance.UserID));
            log.Info("Initial loading is finished");
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
            ShutdownLock.WaitForLocks();
            ClosingEx?.Invoke();
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
            log.Info("WoW processes trace watching is stopped");
            // release hook 
            if (WoWManager.Hooked)
            {
                WoWManager.Unhook();
            }
            foreach (WowProcess i in WoWProcessManager.List)
            {
                string name = i.ProcessName;
                i.Dispose();
                log.Info(string.Format("{0}:{1} :: [WoW hook] Memory manager disposed", name, i.ProcessID));
            }
            HotkeyManager.KeyPressed -= KeyboardHookKeyDown;
            HotkeyManager.RemoveKeys(typeof(PluginManagerEx).ToString());
            log.Info("Closed");
            SendLogToDeveloper();
        }

        private void SendLogToDeveloper()
        {
            TaskDialogButton yesNo = TaskDialogButton.Yes + (int) TaskDialogButton.No;
            TaskDialog taskDialog = new TaskDialog("There were errors during runtime", "AxTools", "Do you want to send log file to developer?", yesNo, TaskDialogIcon.Warning);
            if (log.HaveErrors && Utils.InternetAvailable && taskDialog.Show(this).CommonButton == Result.Yes && File.Exists(Globals.LogFileName))
            {
                try
                {
                    Log2.UploadLog(null);
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
            startupOverlay.Label = "Load WoW accounts...";
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
            olvPlugins.CellToolTipShowing += ObjectListView1_CellToolTipShowing;
            olvPlugins.DoubleClick += OlvPluginsOnDoubleClick;
            // end of styling, events attaching...
            startupOverlay.Label = "Starting addons backup service...";
            AddonsBackup.StartService();                                // start backup service
            startupOverlay.Label = "Starting pinger...";
            Pinger.Enabled = settings.PingerServerID != 0;              // set pinger
            startupOverlay.Label = "Starting WoW process manager...";
            WoWProcessManager.StartWatcher();                           // start WoW spy
            TrayIconAnimation.Initialize(notifyIconMain);               // initialize tray animation
            HotkeyManager.KeyPressed += KeyboardHookKeyDown;            // start keyboard listener
            HotkeyManager.AddKeys(typeof(PluginManagerEx).ToString(), settings.WoWPluginHotkey);
            startupOverlay.Label = "Waiting for plug-ins...";
            await pluginsLoader;                                        // waiting for plugins to be loaded
            startupOverlay.Close();                                     // close startup overkay
            Changes.ShowChangesIfNeeded();                              // show changes overview dialog if needed
            UpdaterService.Start();                                     // start updater service
            log.Info("All start-up routines are finished");   // situation normal :)
        }

        private void LinkSettings_Click(object sender, EventArgs e)
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

        #endregion

        #region TrayContextMenu

        private void CreateTrayContextMenu()
        {
            contextMenuStripMain.Items.Clear();
            contextMenuStripMain.Items.AddRange(new ToolStripItem[]
            {
                woWRadarToolStripMenuItem,
                blackMarketTrackerToolStripMenuItem,
                toolStripSeparator2
            });
            IPlugin[] sortedPlugins = PluginManagerEx.GetSortedByUsageListOfPlugins().ToArray();
            IPlugin[] topUsedPlugins = sortedPlugins.Take(3).ToArray();
            foreach (IPlugin i in topUsedPlugins)
            {
                IPlugin plugin = i;
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon) {Tag = plugin};
                toolStripMenuItem.MouseDown += delegate(object sender, MouseEventArgs args)
                {
                    if (args.Button == MouseButtons.Left) TrayContextMenu_PluginClicked(plugin);
                    else if (plugin.ConfigAvailable) plugin.OnConfig();
                    contextMenuStripMain.Hide();
                };
                toolStripMenuItem.ToolTipText = plugin.ConfigAvailable ? "Left click to start only this plugin\r\nRight click to open settings" : "Left click to start only this plugin";
                contextMenuStripMain.Items.Add(toolStripMenuItem);
            }
            if (sortedPlugins.Length > topUsedPlugins.Length)
            {
                if (contextMenuStripMain.Items.Add("Other plugins") is ToolStripMenuItem customPlugins)
                {
                    foreach (IPlugin i in sortedPlugins.Where(i => !topUsedPlugins.Select(l => l.Name).Contains(i.Name)))
                    {
                        IPlugin plugin = i;
                        try
                        {
                            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon) { Tag = plugin };
                            toolStripMenuItem.MouseDown += delegate (object sender, MouseEventArgs args)
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
                            log.Error(string.Format("Error occured while building tray icon for plugin <{0}>: {1}", plugin.Name, ex.Message));
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
                    Notify.TrayPopup("Warning", "AxTools isn't attached to any WoW process", NotifyUserType.Warn, true);
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
                if (contextMenuStripMain.Items.GetAllToolStripItems().FirstOrDefault(l => (IPlugin)l.Tag != null && ((IPlugin)l.Tag).Name == plugin.Name) is ToolStripMenuItem item)
                {
                    item.ShortcutKeyDisplayString = olvPlugins.CheckedObjects.Cast<IPlugin>().Any(i => i.Name == item.Text) ? settings.WoWPluginHotkey.ToString() : null;
                    item.Enabled = !PluginManagerEx.RunningPlugins.Any();
                }
                stopActivePluginorPresshotkeyToolStripMenuItem.Enabled = PluginManagerEx.RunningPlugins.Any();
                stopActivePluginorPresshotkeyToolStripMenuItem.ShortcutKeyDisplayString = settings.WoWPluginHotkey.ToString();
            }
            
        }

        private void WoWRadarToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartWoWModule<WowRadar>();
        }

        private void BlackMarketTrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartWoWModule<BlackMarket>();
        }

        private void ExitAxToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isClosing)
            {
                if (InvokeRequired) Invoke(new Action(Close));
                else Close();
            }
        }

        private void StopActivePluginorPresshotkeyToolStripMenuItem_Click_1(object sender, EventArgs e)
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
                if (settings.VentriloStartWithWoW && !Process.GetProcessesByName("Ventrilo").Any())
                {
                    StartVentrilo();
                }
                if (settings.TS3StartWithWoW && !Process.GetProcessesByName("ts3client_win64").Any() && !Process.GetProcessesByName("ts3client_win32").Any())
                {
                    StartTS3Wait();
                }
                if (settings.RaidcallStartWithWoW && !Process.GetProcessesByName("raidcall").Any())
                {
                    StartRaidcall();
                }
                if (settings.MumbleStartWithWoW && !Process.GetProcessesByName("mumble").Any())
                {
                    StartMumble();
                }
                StartWoW(WoWAccount.AllAccounts[cmbboxAccSelect.SelectedIndex]);
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
            AddonsBackupDeploy form = Utils.FindForm<AddonsBackupDeploy>();
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
                log.Info("Ventrilo process started");
            }
            else
            {
                this.TaskDialog("Executable not found", "Can't locate \"Ventrilo.exe\". Check paths in settings window", NotifyUserType.Error);
            }
        }

        private void StartTS3Wait()
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
            }).WaitForInputIdle(10*1000);
            log.Info("TS3 process started");
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
                log.Info("Raidcall process started");
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
                log.Info("Mumble process started");
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
            StartTS3Wait();
        }

        private void TileMumbleClick(object sender, EventArgs e)
        {
            StartMumble();



            //using (IOdb db = OdbFactory.Open("wowhead-items.ndb"))
            //{
            //    for (uint i = 0; i < 5000; i++)
            //    {
            //        db.Store(new NDBEntry((int)i, JsonConvert.SerializeObject(new WowheadItemInfo("n" + i, i + 1, i + 2, i + 3) { Image = Resources.AppIcon1 })));
            //    }
            //}
            //Stopwatch stopwatch = Stopwatch.StartNew();
            //using (IOdb db = OdbFactory.Open("wowhead-items.ndb"))
            //{
            //    NDBEntry cc = db.QueryAndExecute<NDBEntry>().First(l => l.ID == 4950);
            //    WowheadItemInfo c = JsonConvert.DeserializeObject<WowheadItemInfo>(cc.JSONData);
            //    MessageBox.Show(c.Name + "::" + c.Quality + "\r\nElapsed: " + stopwatch.ElapsedMilliseconds);
            //}


            //using (LiteDatabase db = new LiteDatabase("wowhead-items.ldb"))
            //{
            //    db.BeginTrans();
            //    LiteCollection<NDBEntry> collection = db.GetCollection<NDBEntry>("wowhead-items");
            //    for (uint i = 0; i < 5000; i++)
            //    {
            //        collection.Insert(new NDBEntry((int)i, JsonConvert.SerializeObject(new WowheadItemInfo("n" + i, i + 1, i + 2, i + 3) { Image = Resources.AppIcon1 })));
            //    }
            //    collection.EnsureIndex(x => x.ID);
            //    db.Commit();
            //}
            //stopwatch.Restart();
            //using (LiteDatabase db = new LiteDatabase("wowhead-items.ldb"))
            //{
            //    LiteCollection<NDBEntry> collection = db.GetCollection<NDBEntry>("wowhead-items");
            //    collection.EnsureIndex(x => x.ID);
            //    NDBEntry entry = collection.FindOne(l => l.ID == 4950);
            //    WowheadItemInfo c = JsonConvert.DeserializeObject<WowheadItemInfo>(entry.JSONData);
            //    MessageBox.Show(c.Name + "::" + c.Quality + "\r\nElapsed: " + stopwatch.ElapsedMilliseconds);
            //    MessageBox.Show("Entries count: " + collection.Find(l => l.ID == 4950).Count() + "; Total: " + collection.Count());
            //}

        }

        private void TileExtDiscord_Click(object sender, EventArgs e)
        {
            if (VoIP.AvailableVoipClients.TryGetValue("Discord", out VoipInfo discordInfo))
            {
                Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = discordInfo.DirectoryPath,
                    FileName = discordInfo.ExecutablePath,
                    Arguments = discordInfo.ExecutableArguments
                });
                log.Info("Discord process started");
            }
            else
            {
                Notify.SmartNotify("Executable not found", "Can't locate \"Update.exe\". Check paths in settings window", NotifyUserType.Error, false);
            }
        }

        private void TileExtTwitch_Click(object sender, EventArgs e)
        {
            if (VoIP.AvailableVoipClients.TryGetValue("Twitch", out VoipInfo twitchInfo))
            {
                Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = twitchInfo.DirectoryPath,
                    FileName = twitchInfo.ExecutablePath,
                    Arguments = twitchInfo.ExecutableArguments
                });
                log.Info("Twitch process started");
            }
            else
            {
                Notify.SmartNotify("Executable not found", "Can't locate \"Twitch.exe\". Check paths in settings window", NotifyUserType.Error, false);
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

        private void TileRadar_Click(object sender, EventArgs e)
        {
            StartWoWModule<WowRadar>();
        }

        private void TileBMTracker_Click(object sender, EventArgs e)
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
                        if (Info.IsInGame)
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
                    log.Error("Plugin task failed to cancel");
                    Notify.SmartNotify("Plugin error", "Fatal error: please restart AxTools", NotifyUserType.Error, true);
                }
            }
            buttonStartStopPlugin.Enabled = true;
        }

        private void SetupOLVPlugins()
        {
            string checkMark = Encoding.UTF8.GetString(new byte[] {0xE2, 0x9C, 0x93});
            olvColumn2.AspectToStringConverter = value => (bool)value ? checkMark : "";
            olvColumn2.TextAlign = HorizontalAlignment.Center;
            olvPlugins.SetObjects(PluginManagerEx.GetSortedByUsageListOfPlugins());
            foreach (IPlugin i in PluginManagerEx.EnabledPlugins)
            {
                olvPlugins.CheckObject(i);
            }
            PluginManagerEx.PluginDisabled += plugin => olvPlugins.BuildList();
            PluginManagerEx.PluginEnabled += plugin => olvPlugins.BuildList();
            olvPlugins.BooleanCheckStateGetter = OlvPlugins_BooleanCheckStateGetter;
            olvPlugins.BooleanCheckStatePutter = OlvPlugins_BooleanCheckStatePutter;
        }

        private void ButtonStartStopPlugin_Click(object sender, EventArgs e)
        {
            SwitchWoWPlugin();
        }

        private void ObjectListView1_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
        {
            if (e.Model is IPlugin plugin)
            {
                e.IsBalloon = true;
                e.StandardIcon = ToolTipControl.StandardIcons.InfoLarge;
                e.Title = plugin.Name;
                e.Text = $"{plugin.Description}\r\nVersion: {plugin.Version}";
            }
        }

        private void OlvPluginsOnDoubleClick(object sender, EventArgs eventArgs)
        {
            if (olvPlugins.SelectedObject is IPlugin plugin)
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
            if (rowObject is IPlugin plugin)
            {
                return PluginManagerEx.EnabledPlugins.Contains(plugin) || PluginManagerEx.RunningPlugins.Contains(plugin);
            }
            return false;
        }

        private bool OlvPlugins_BooleanCheckStatePutter(object rowObject, bool newValue)
        {
            if (rowObject is IPlugin plugin)
            {
                PluginManagerEx.SetPluginEnabled(plugin, newValue);
                if (newValue)
                {
                    if (PluginManagerEx.RunningPlugins.Any())
                    {
                        PluginManagerEx.AddPluginToRunning(plugin);
                    }
                    settings.EnabledPluginsList.Add(plugin.Name);
                }
                else
                {
                    if (PluginManagerEx.RunningPlugins.Any())
                    {
                        PluginManagerEx.RemovePluginFromRunning(plugin);
                    }
                    settings.EnabledPluginsList.Remove(plugin.Name);
                }
                PostInvoke(() => { labelTotalPluginsEnabled.Text = "Plugins enabled: " + PluginManagerEx.EnabledPlugins.Count; });
                BeginInvoke(new MethodInvoker(UpdateTrayContextMenu));
            }
            return newValue;
        }

        private void LinkDownloadPlugins_Click(object sender, EventArgs e)
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
                //olvPlugins.Enabled = !PluginManagerEx.RunningPlugins.Any();
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
                labelTotalPluginsEnabled.Text = "Plugins enabled: " + PluginManagerEx.EnabledPlugins.Count();
            });
            PluginManagerEx.PluginsLoaded -= PluginManagerExOnPluginsLoaded;
        }

        private void WoWPluginHotkeyChanged(KeyExt key)
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

        private void KeyboardHookKeyDown(KeyExt key)
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