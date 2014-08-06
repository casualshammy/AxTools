using AxTools.Classes;
using AxTools.Components;
using AxTools.Components.TaskbarProgressbar;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.Services;
using AxTools.Updater;
using AxTools.WinAPI;
using AxTools.WoW;
using AxTools.WoW.Management;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.Plugins;
using MouseKeyboardActivityMonitor;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class MainForm : BorderedMetroForm
    {
        internal static MainForm Instance;

        internal MainForm()
        {
            InitializeComponent();
            Closing += MainFormClosing;
            notifyIconMain.Icon = Resources.AppIcon;

            cmbboxAccSelect.MouseWheel += delegate(object sender, MouseEventArgs args) { ((HandledMouseEventArgs) args).Handled = true; };
            linkOpenBackupFolder.Location = new Point(metroTabPage1.Size.Width/2 - linkOpenBackupFolder.Size.Width/2, linkOpenBackupFolder.Location.Y);
            cmbboxAccSelect.Location = new Point(metroTabPage1.Size.Width/2 - cmbboxAccSelect.Size.Width/2, cmbboxAccSelect.Location.Y);
            linkEditWowAccounts.Location = new Point(metroTabPage1.Size.Width / 2 - linkEditWowAccounts.Size.Width / 2, linkEditWowAccounts.Location.Y);

            tabControl.SelectedIndex = 0;
            metroToolTip1.SetToolTip(labelPingNum, "This is ingame connection info. It's formatted as\r\n" +
                                                   "  [worst ping of the last 10]::[packet loss in the last 200 seconds]  \r\n" +
                                                   "Left-click to clear statistics\r\n"+
                                                   "Right-click to open pinger settings");

            progressBarAddonsBackup.Size = linkBackupAddons.Size;
            progressBarAddonsBackup.Location = linkBackupAddons.Location;
            progressBarAddonsBackup.Visible = false;

            Log.Print(String.Format("Launching... ({0})", Globals.AppVersion));
            Icon = Resources.AppIcon;
            Utils.Legacy();
            OnSettingsLoaded();
            WoWAccounts_CollectionChanged(null, null); // initial load wowaccounts
            WebRequest.DefaultWebProxy = null;

            settings.WoWPluginHotkeyChanged += WoWPluginHotkeyChanged;
            PluginManager.PluginStateChanged += PluginManagerOnPluginStateChanged;
            Pinger.DataChanged += Pinger_DataChanged;
            Pinger.StateChanged += PingerOnStateChanged;
            AddonsBackup.StateChanged += AddonsBackup_OnChangedState;
            WoWAccount.AllAccounts.CollectionChanged += WoWAccounts_CollectionChanged;

            Task.Factory.StartNew(LoadingStepAsync);
            BeginInvoke((MethodInvoker) delegate
            {
                Location = settings.MainWindowLocation;
                OnActivated(EventArgs.Empty);
                startupOverlay = new WaitingOverlay(this);
                startupOverlay.Show();
            });
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (uint) WM_MESSAGE.WM_QUERYENDSESSION)
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

        #region Variables

        //another
        private bool isClosing;
        private WaitingOverlay startupOverlay;

        private readonly Settings settings = Settings.Instance;
        //
        private readonly List<ToolStripMenuItem> pluginsToolStripMenuItems = new List<ToolStripMenuItem>(); 

        #endregion
        
        #region Keyboard hook

        private KeyboardHookListener keyboardHook;

        private void KeyboardHookKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == settings.ClickerHotkey)
            {
                KeyboardHook_ClickerHotkey();
            }
            else if (e.KeyCode == settings.WoWPluginHotkey)
            {
                KeyboardHook_PrecompiledModulesHotkey();
            }
            else if (e.KeyCode == settings.LuaTimerHotkey)
            {
                KeyboardHook_LuaTimerHotkey();
            }
        }

        private void KeyboardHook_ClickerHotkey()
        {
            if (settings.ClickerKey == Keys.None)
            {
                this.ShowTaskDialog("Incorrect input!", "Please select key to be pressed", TaskDialogButton.OK, TaskDialogIcon.Stop);
                return;
            }
            if (Clicker.Enabled)
            {
                Clicker.Stop();
                WowProcess cProcess = WowProcess.GetAllWowProcesses().FirstOrDefault(i => i.MainWindowHandle == Clicker.Handle);
                Log.Print(cProcess != null
                    ? String.Format("{0}:{1} :: [Clicker] Disabled", cProcess.ProcessName, cProcess.ProcessID)
                    : "UNKNOWN:null :: [Clicker] Disabled");
            }
            else
            {
                WowProcess cProcess = WowProcess.GetAllWowProcesses().FirstOrDefault(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow());
                if (cProcess != null)
                {
                    Clicker.Start(settings.ClickerInterval, cProcess.MainWindowHandle, (IntPtr) settings.ClickerKey);
                    Log.Print(string.Format("{0}:{1} :: [Clicker] Enabled, interval {2}ms, window handle 0x{3:X}", cProcess.ProcessName, cProcess.ProcessID,
                        settings.ClickerInterval, (uint) cProcess.MainWindowHandle));
                }
            }
        }

        private void KeyboardHook_PrecompiledModulesHotkey()
        {
            if (WowProcess.GetAllWowProcesses().Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
            {
                SwitchWoWPlugin();
            }
        }

        private void KeyboardHook_LuaTimerHotkey()
        {
            LuaConsole pForm = Utils.FindForm<LuaConsole>();
            if (pForm != null && WowProcess.GetAllWowProcesses().Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
            {
                pForm.SwitchTimer();
            }
        }

        #endregion

        #region MainFormEvents

        private void MainFormClosing(object sender, CancelEventArgs e)
        {
            isClosing = true;
            // Close all children forms
            Form[] forms = Application.OpenForms.Cast<Form>().Where(i => i.GetType() != typeof (MainForm) && i.GetType() != typeof (MetroFlatDropShadow)).ToArray();
            foreach (Form i in forms)
            {
                i.Close();
            }
            //
            settings.WoWPluginHotkeyChanged -= WoWPluginHotkeyChanged;
            PluginManager.PluginStateChanged -= PluginManagerOnPluginStateChanged;
            Pinger.DataChanged -= Pinger_DataChanged;
            Pinger.StateChanged += PingerOnStateChanged;
            AddonsBackup.StateChanged -= AddonsBackup_OnChangedState;
            //
            settings.MainWindowLocation = Location;
            //save settings
            settings.SaveJSON();
            //
            Clicker.Stop();
            //stop timers
            AddonsBackup.StopService();
            TrayIconAnimation.Close();
            Pinger.Stop();
            //stop watching process trace
            WoWProcessWatcher.StopWatcher();
            Log.Print("WoW processes trace watching is stopped");
            // release hook 
            if (WoWManager.Hooked)
            {
                WoWManager.Unhook();
                Log.Print(String.Format("{0}:{1} :: [WoW hook] Injector unloaded", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
            }
            foreach (WowProcess i in WowProcess.GetAllWowProcesses())
            {
                string name = i.ProcessName;
                i.Dispose();
                Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager disposed", name, i.ProcessID));
            }
            if (keyboardHook != null && keyboardHook.Enabled)
            {
                keyboardHook.Dispose();
                Log.Print("Keyboard hook disposed");
            }
            Log.Print("AxTools closed");
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
                    Log.SendViaEmail(null);
                }
                catch (Exception ex)
                {
                    this.ShowTaskDialog("Log file sending error", ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
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

        private void LoadingStepAsync()
        {

            #region Set registration name

            while (string.IsNullOrWhiteSpace(Settings.Instance.UserID))
            {
                Invoke(new MethodInvoker(() => { Settings.Instance.UserID = InputBox.Input("Please enter your nickname:") ?? string.Empty; }));
                if (!string.IsNullOrWhiteSpace(Settings.Instance.UserID))
                {
                    Settings.Instance.UserID += "_" + Utils.GetRandomString(10).ToUpper();
                }
            }
            Log.Print("Registered for: " + Settings.Instance.UserID);

            #endregion

            #region Backup and delete wow logs

            if (settings.WoWNotifyIfBigLogs && Directory.Exists(settings.WoWDirectory + "\\Logs") && WowProcess.GetAllWowProcesses().Count == 0)
            {
                long directorySize = Utils.CalcDirectorySize(settings.WoWDirectory + "\\Logs");
                Log.Print("[WoW logs watcher] Log directory size: " + Math.Round((float) directorySize/1024/1024, 3, MidpointRounding.ToEven) + " MB");
                if (directorySize > settings.WoWNotifyIfBigLogsSize * 1024 * 1024)
                {
                    DirectoryInfo rootDir = new DirectoryInfo(settings.WoWDirectory + "\\Logs").Root;
                    DriveInfo drive = DriveInfo.GetDrives().FirstOrDefault(i => i.RootDirectory.FullName == rootDir.FullName);
                    if (drive != null)
                    {
                        double freeSpace = Math.Round((float) drive.TotalFreeSpace/1024/1024, 3, MidpointRounding.ToEven);
                        Utils.NotifyUser("Log directory is too large!", "Disk free space: " + freeSpace + " MB", NotifyUserType.Warn, true);
                    }
                }
            }

            //if (settings.WoWDeleteLogs && Directory.Exists(settings.WoWDirectory + "\\Logs") && WowProcess.GetAllWowProcesses().Count == 0)
            //{
            //    //if (File.Exists(settings.WoWDirectory + "\\Logs\\WoWCombatLog.txt") || Utils.CalcDirectorySize(settings.WoWDirectory + "\\Logs") > 104857600)
            //    //{
            //    //    Utils.CheckCreateDir();
            //    //    string zipPath = String.Format("{0}\\WoWLogs.zip", settings.WoWAddonsBackupPath);
            //    //    try
            //    //    {
            //    //        if (File.Exists(zipPath))
            //    //        {
            //    //            File.Delete(zipPath);
            //    //        }
            //    //        try
            //    //        {
            //    //            using (ZipFile zip = new ZipFile(zipPath, Encoding.UTF8))
            //    //            {
            //    //                zip.CompressionLevel = (CompressionLevel)settings.WoWAddonsBackupCompressionLevel;
            //    //                zip.AddDirectory(settings.WoWDirectory + "\\Logs");
            //    //                zip.Save();
            //    //            }
            //    //            Log.Print(String.Format("[WoW logs] WoW combat log's backup was placed to \"{0}\"", zipPath));
            //    //            string[] cLogFiles = Directory.GetFiles(settings.WoWDirectory + "\\Logs");
            //    //            foreach (string i in cLogFiles)
            //    //            {
            //    //                try
            //    //                {
            //    //                    File.Delete(i);
            //    //                    Log.Print("[WoW logs] Log file deleted: " + i);
            //    //                }
            //    //                catch (Exception ex)
            //    //                {
            //    //                    Log.Print(String.Format("[WoW logs] Error deleting log file \"{0}\": {1}", i, ex.Message), true);
            //    //                }
            //    //            }
            //    //            notifyIconMain.ShowBalloonTip(10000, "WoW log files were deleted", "Backup was placed to " + settings.WoWAddonsBackupPath, ToolTipIcon.Info);
            //    //        }
            //    //        catch (Exception ex)
            //    //        {
            //    //            Log.Print(String.Format("[WoW logs] Can't backup wow combat log \"{0}\": {1}", zipPath, ex.Message), true);
            //    //        }
            //    //    }
            //    //    catch (Exception ex)
            //    //    {
            //    //        Log.Print(String.Format("[WoW logs] Can't delete \"{0}\": {1}", zipPath, ex.Message), true);
            //    //    }
            //    //}
            //}

            #endregion

            #region Processing creaturecache.wdb

            if (settings.WoWWipeCreatureCache && Directory.Exists(settings.WoWDirectory + "\\Cache\\WDB"))
            {
                DirectoryInfo[] directories = new DirectoryInfo(settings.WoWDirectory + "\\Cache\\WDB").GetDirectories();
                if (directories.Length > 0)
                {
                    foreach (DirectoryInfo i in directories)
                    {
                        try
                        {
                            if (File.Exists(i.FullName + "\\creaturecache.wdb"))
                            {
                                File.Delete(i.FullName + "\\creaturecache.wdb");
                                Log.Print("[WoW cache] " + i.FullName + "\\creaturecache.wdb was deleted");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Print(String.Format("[WoW cache] Can't delete cache file ({0}): {1}", ex.Message, i.FullName));
                        }
                    }
                }
            }

            #endregion

            #region Loading plugins

            PluginManager.LoadPlugins();
            if (settings.WoWPluginEnableCustom)
            {
                PluginManager.LoadPluginsFromDisk();
            }

            #endregion

            //continue starting...
            BeginInvoke(new Action(LoadingStepSync));
            Log.Print("AxTools :: preparation completed");
        }

        private void LoadingStepSync()
        {
            OnPluginsLoaded();
            AddonsBackup.StartService();
            Pinger.Start();
            WoWProcessWatcher.StartWatcher();
            TrayIconAnimation.Initialize(notifyIconMain);

            keyboardHook = new KeyboardHookListener(Globals.GlobalHooker);
            keyboardHook.KeyDown += KeyboardHookKeyDown;
            keyboardHook.Enabled = true;

            startupOverlay.Close();
            Changes.ShowChangesIfNeeded();
            UpdaterService.Start();
            Log.Print("AxTools started succesfully");
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

        private void LabelPingNumMouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Pinger.Stop();
                    labelPingNum.Text = "cleared";
                    Pinger.Start();
                    break;
                case MouseButtons.Right:
                    int pingerTabPageIndex = 5;
                    new AppSettings(pingerTabPageIndex).ShowDialog(this);
                    break;
            }
        }

        #endregion

        #region MainNotifyIconContextMenu

        private void UpdatePluginsShortcutsInTrayContextMenu()
        {
            foreach (IPlugin plugin in PluginManager.Plugins)
            {
                ToolStripItem[] items = contextMenuStripMain.Items.Find("NativeN" + plugin.Name, true);  //contextMenuStripMain.Items["NativeN" + plugin.Name] as ToolStripMenuItem;
                foreach (ToolStripMenuItem item in items)
                {
                    item.ShortcutKeyDisplayString = item.Text == comboBoxWowPlugins.Text ? settings.WoWPluginHotkey.ToString() : null;
                    item.Enabled = PluginManager.ActivePlugin == null;
                }
            }
            stopActivePluginorPresshotkeyToolStripMenuItem.Enabled = PluginManager.ActivePlugin != null;
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
            if (PluginManager.ActivePlugin != null)
            {
                SwitchWoWPlugin();
            }
        }

        private void TrayContextMenu_PluginClicked(IPlugin plugin)
        {
            comboBoxWowPlugins.SelectedIndex = PluginManager.Plugins.IndexOf(plugin);
            if (!WoWManager.Hooked && WowProcess.GetAllWowProcesses().Count != 1)
            {
                Activate();
            }
            SwitchWoWPlugin();
        }

        #endregion

        #region MainTab

        private void CmbboxAccSelectSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbboxAccSelect.SelectedIndex != -1)
            {
                WoWAccount wowAccount = new WoWAccount(WoWAccount.AllAccounts[cmbboxAccSelect.SelectedIndex].Login, WoWAccount.AllAccounts[cmbboxAccSelect.SelectedIndex].Password);
                StartWoW(wowAccount);
                cmbboxAccSelect.SelectedIndex = -1;
                cmbboxAccSelect.Invalidate();
            }
        }

        private void linkOpenBackupFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(settings.WoWAddonsBackupPath))
            {
                Process.Start(settings.WoWAddonsBackupPath);
            }
            else
            {
                this.ShowTaskDialog("Can't open backup folder", "It doesn't exist", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private void linkBackupAddons_Click(object sender, EventArgs e)
        {
            AddonsBackup_OnChangedState(-1);
            Task.Factory.StartNew(AddonsBackup.MakeBackup)
                .ContinueWith(l => AddonsBackup_OnChangedState(101));
        }

        private void linkClickerSettings_Click(object sender, EventArgs e)
        {
            if (Clicker.Enabled)
            {
                this.ShowTaskDialog("Clicker settings", "Please switch clicker off before", TaskDialogButton.OK, TaskDialogIcon.Warning);
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

        private void buttonLaunchWowWithoutAutopass_Click(object sender, EventArgs e)
        {
            StartWoW();
            Log.Print("WOW! buttonLaunchWowWithoutAutopass_Click!", true);
        }

        private void linkEditWowAccounts_Click(object sender, EventArgs e)
        {
            new WowAccountsManager().ShowDialog(this);
        }

        #endregion

        #region VoipTab

        private void TileVentriloClick(object sender, EventArgs e)
        {
            StartVentrilo();
        }

        private void TileRaidcallClick(object sender, EventArgs e)
        {
            if (!File.Exists(settings.RaidcallDirectory + "\\raidcall.exe"))
            {
                new TaskDialog("Executable not found", "AxTools", "Can't locate \"raidcall.exe\". Check paths in settings window", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = settings.RaidcallDirectory,
                FileName = settings.RaidcallDirectory + "\\raidcall.exe"
            });
            Log.Print("Raidcall process started");
        }

        private void TileTeamspeak3Click(object sender, EventArgs e)
        {
            string cPath;
            if (File.Exists(settings.TS3Directory + "\\ts3client_win32.exe"))
            {
                cPath = settings.TS3Directory + "\\ts3client_win32.exe";
            }
            else if (File.Exists(settings.TS3Directory + "\\ts3client_win64.exe"))
            {
                cPath = settings.TS3Directory + "\\ts3client_win64.exe";
            }
            else
            {
                new TaskDialog("Executable not found", "AxTools",
                               "Can't locate \"ts3client_win64.exe\"/\"ts3client_win32.exe\". Check paths in settings window", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = settings.TS3Directory,
                FileName = cPath
            });
            Log.Print("TS3 process started");
        }

        private void TileMumbleClick(object sender, EventArgs e)
        {
            if (!File.Exists(settings.MumbleDirectory + "\\mumble.exe"))
            {
                new TaskDialog("Executable not found", "AxTools", "Can't locate \"mumble.exe\". Check paths in settings window", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            Process.Start(new ProcessStartInfo {
                WorkingDirectory = settings.MumbleDirectory,
                FileName = settings.MumbleDirectory + "\\mumble.exe"
            });
            Log.Print("Mumble process started");
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

        #region WowPluginsTab
        
        private void buttonStartStopPlugin_Click(object sender, EventArgs e)
        {
            SwitchWoWPlugin();
        }

        private void metroCheckBoxPluginShowIngameNotification_CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWPluginShowIngameNotifications = metroCheckBoxPluginShowIngameNotification.Checked;
        }

        private void checkBoxEnableCustomPlugins_CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWPluginEnableCustom = checkBoxEnableCustomPlugins.Checked;
        }

        private void ComboBoxWowPluginsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxWowPlugins.SelectedIndex == -1)
            {
                metroToolTip1.SetToolTip(comboBoxWowPlugins, string.Empty);
            }
            else
            {
                string[] arr = PluginManager.Plugins.First(i => i.Name == comboBoxWowPlugins.Text).Description.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                string text = string.Empty;
                int counter = 0;
                foreach (string i in arr)
                {
                    text += i + " ";
                    counter += i.Length + 1;
                    if (counter >= 50)
                    {
                        text += "\r\n";
                        counter = 0;
                    }
                }
                metroToolTip1.SetToolTip(comboBoxWowPlugins, text);
            }
            UpdatePluginsShortcutsInTrayContextMenu();
        }

        private void MetroButtonBlackMarketTrackerClick(object sender, EventArgs e)
        {
            StartWoWModule<BlackMarket>();
        }

        private void MetroButtonRadarClick(object sender, EventArgs e)
        {
            StartWoWModule<WowRadar>();
        }

        private void MetroButtonLuaConsoleClick(object sender, EventArgs e)
        {
            StartWoWModule<LuaConsole>();
        }

        private void ButtonUnloadInjectorClick(object sender, EventArgs e)
        {
            if (WoWManager.Hooked)
            {
                WoWManager.Unhook();
                Log.Print(String.Format("{0}:{1} :: Injector unloaded", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
            }
            else
            {
                Log.Print("Injector error: WoW injector isn't active", true);
                this.ShowTaskDialog("Injector error", "WoW injector isn't active", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        #endregion

        #region Events()

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
                ToolStripMenuItem launchWoW = (ToolStripMenuItem)items[0];
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

        private void OnPluginsLoaded()
        {
            comboBoxWowPlugins.Items.Clear();
            comboBoxWowPlugins.Items.AddRange(PluginManager.Plugins.Select(i => i.Name).Cast<object>().ToArray());

            contextMenuStripMain.Items.Clear();
            
            contextMenuStripMain.Items.AddRange(new ToolStripItem[]
            {
                woWRadarToolStripMenuItem,
                luaConsoleToolStripMenuItem,
                blackMarketTrackerToolStripMenuItem,
                toolStripSeparator2
            });
            Type[] nativePlugins = {typeof (Fishing), typeof (FlagReturner), typeof (GoodsDestroyer)};
            foreach (IPlugin i in PluginManager.Plugins.Where(i => nativePlugins.Contains(i.GetType())))
            {
                IPlugin plugin = i;
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon, delegate { TrayContextMenu_PluginClicked(plugin); }, "NativeN" + plugin.Name);
                pluginsToolStripMenuItems.Add(toolStripMenuItem);
                contextMenuStripMain.Items.Add(toolStripMenuItem);
            }
            if (settings.WoWPluginEnableCustom && PluginManager.Plugins.Count > nativePlugins.Length)
            {
                ToolStripMenuItem customPlugins = contextMenuStripMain.Items.Add("Custom plugins") as ToolStripMenuItem;
                if (customPlugins != null)
                {
                    foreach (IPlugin i in PluginManager.Plugins.Where(i => !nativePlugins.Contains(i.GetType())))
                    {
                        IPlugin plugin = i;
                        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon, delegate { TrayContextMenu_PluginClicked(plugin); }, "NativeN" + plugin.Name);
                        pluginsToolStripMenuItems.Add(toolStripMenuItem);
                        customPlugins.DropDownItems.Add(toolStripMenuItem);
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
            contextMenuStripMain.Items.AddRange(new ToolStripItem[]
            {
                stopActivePluginorPresshotkeyToolStripMenuItem,
                toolStripSeparator1,
                launchWoW,
                new ToolStripSeparator(),
                launchWoWToolStripMenuItem
            });

            UpdatePluginsShortcutsInTrayContextMenu();
        }

        private void OnSettingsLoaded()
        {
            metroStyleManager1.Style = settings.StyleColor;
            checkBoxStartVenriloWithWow.Checked = settings.VentriloStartWithWoW;
            checkBoxStartTeamspeak3WithWow.Checked = settings.TS3StartWithWoW;
            checkBoxStartRaidcallWithWow.Checked = settings.RaidcallStartWithWoW;
            checkBoxStartMumbleWithWow.Checked = settings.MumbleStartWithWoW;
            metroCheckBoxPluginShowIngameNotification.Checked = settings.WoWPluginShowIngameNotifications;
            checkBoxEnableCustomPlugins.Checked = settings.WoWPluginEnableCustom;
            buttonStartStopPlugin.Text = string.Format("{0} [{1}]", "Start", settings.WoWPluginHotkey);
        }

        private void PluginManagerOnPluginStateChanged()
        {
            BeginInvoke((MethodInvoker) delegate
            {
                buttonStartStopPlugin.Text = string.Format("{0} [{1}]", PluginManager.ActivePlugin == null ? "Start" : "Stop", settings.WoWPluginHotkey);
                comboBoxWowPlugins.Enabled = PluginManager.ActivePlugin == null;
                UpdatePluginsShortcutsInTrayContextMenu();
            });
        }

        private void WoWPluginHotkeyChanged(Keys key)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                buttonStartStopPlugin.Text = string.Format("{0} [{1}]", PluginManager.ActivePlugin == null ? "Start" : "Stop", key);
                UpdatePluginsShortcutsInTrayContextMenu();
            }));
        }

        private void AddonsBackup_OnChangedState(int procent)
        {
            switch (procent)
            {
                case -1:
                    BeginInvoke((MethodInvoker) delegate
                    {
                        linkBackupAddons.Visible = false;
                        progressBarAddonsBackup.Value = 0;
                        progressBarAddonsBackup.Visible = true;
                    });
                    break;
                case 101:
                    BeginInvoke((MethodInvoker) delegate
                    {
                        linkBackupAddons.Visible = true;
                        progressBarAddonsBackup.Visible = false;
                    });
                    break;
                default:
                    BeginInvoke((MethodInvoker) delegate
                    {
                        progressBarAddonsBackup.Value = procent;
                    });
                    break;
            }
        }

        private void PingerOnStateChanged(bool enabled)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                labelPingNum.Visible = enabled;
                labelPingNum.Text = "[n/a]::[n/a]";
                TBProgressBar.SetProgressValue(Handle, 1, 1);
                TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.NoProgress);
            }));
        }

        private void Pinger_DataChanged(int ping, int packetLoss)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                labelPingNum.Text = string.Format("[{0}]::[{1}%]", (ping == -1 || ping == -2) ? "n/a" : ping + "ms", packetLoss);
                labelPingNum.Location = new Point(Size.Width/2 - labelPingNum.Size.Width/2, labelPingNum.Location.Y);
                TBProgressBar.SetProgressValue(Handle, 1, 1);
                if (packetLoss >= settings.PingerVeryBadPacketLoss || ping >= settings.PingerVeryBadPing)
                {
                    TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.Error);
                }
                else if (packetLoss >= settings.PingerBadPacketLoss || ping >= settings.PingerBadPing)
                {
                    TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.Paused);
                }
                else
                {
                    TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.NoProgress);
                }
            });
        }

        #endregion

        #region Helpers

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
                        process.Refresh();
                        if (process.MainWindowHandle != (IntPtr) 0)
                        {
                            IntPtr windowHandle = NativeMethods.FindWindow(null, "Ventrilo");
                            if (windowHandle != IntPtr.Zero)
                            {
                                IntPtr connectButtonHandle = NativeMethods.FindWindowEx(windowHandle, IntPtr.Zero, "Button", "C&onnect");
                                if (connectButtonHandle != IntPtr.Zero)
                                {
                                    NativeMethods.PostMessage(connectButtonHandle, WM_MESSAGE.WM_BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                                    break;
                                }
                            }
                        }
                        Thread.Sleep(100);
                        counter--;
                    }
                });
                Log.Print("Ventrilo process started");
            }
            else
            {
                this.ShowTaskDialog("Executable not found", "Can't locate \"Ventrilo.exe\". Check paths in settings window", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private void StartWoW(WoWAccount wowAccount = null)
        {
            WaitingOverlay waitingOverlay = null;
            Invoke((MethodInvoker) (() =>
            {
                waitingOverlay = new WaitingOverlay(this);
                waitingOverlay.Show();
            }));
            Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(l => BeginInvoke((MethodInvoker) waitingOverlay.Close));
            if (File.Exists(settings.WoWDirectory + "\\Wow.exe"))
            {
                Process process = Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = settings.WoWDirectory,
                    FileName = settings.WoWDirectory + "\\Wow.exe",
                    Arguments = "-noautolaunch64bit",
                });
                if (wowAccount != null)
                {
                    new AutoLogin(wowAccount, process).EnterCredentialsASAPAsync();
                }
                if (settings.VentriloStartWithWoW && !Process.GetProcessesByName("Ventrilo").Any())
                {
                    StartVentrilo();
                }
            }
            else
            {
                this.ShowTaskDialog("WoW client not found or corrupted", "Can't locate \"Wow.exe\"", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

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

        internal void ShowNotifyIconMessage(string title, string text, ToolTipIcon icon)
        {
            if (InvokeRequired)
                BeginInvoke(new MethodInvoker(() => notifyIconMain.ShowBalloonTip(30000, title, text, icon)));
            else
                notifyIconMain.ShowBalloonTip(30000, title, text, icon);
        }

        #endregion
        
    }
}