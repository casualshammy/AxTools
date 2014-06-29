using AxTools.Classes;
using AxTools.Components;
using AxTools.Components.TaskbarProgressbar;
using AxTools.Properties;
using AxTools.WinAPI;
using AxTools.WoW;
using AxTools.WoW.Management;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.Plugins;
using Ionic.Zip;
using MouseKeyboardActivityMonitor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using CompressionLevel = Ionic.Zlib.CompressionLevel;
using Settings = AxTools.Classes.Settings;
using Timer = System.Timers.Timer;

namespace AxTools.Forms
{
    internal partial class MainForm : BorderedMetroForm
    {
        internal MainForm()
        {
            InitializeComponent();
            Instance = this;
            timerNotifyIcon.Elapsed += TimerNiElapsed;
            Closing += MainFormClosing;
            notifyIconMain.Icon = Resources.AppIcon;

            cmbboxAccSelect.MouseWheel += delegate(object sender, MouseEventArgs args) { ((HandledMouseEventArgs) args).Handled = true; };
            linkOpenBackupFolder.Location = new Point(metroTabPage1.Size.Width/2 - linkOpenBackupFolder.Size.Width/2, linkOpenBackupFolder.Location.Y);
            cmbboxAccSelect.Location = new Point(metroTabPage1.Size.Width/2 - cmbboxAccSelect.Size.Width/2, cmbboxAccSelect.Location.Y);
            linkEditWowAccounts.Location = new Point(metroTabPage1.Size.Width / 2 - linkEditWowAccounts.Size.Width / 2, linkEditWowAccounts.Location.Y);

            metroTabControl1.SelectedIndex = 0;
            metroToolTip1.SetToolTip(labelPingNum, "This is ingame connection info. It's formatted as\r\n" +
                                                   "  [worst ping of the last 10]::[packet loss in the last 200 seconds]  \r\n" +
                                                   "Click to clear statistics");

            progressBarAddonsBackup.Size = linkBackupAddons.Size;
            progressBarAddonsBackup.Location = linkBackupAddons.Location;
            progressBarAddonsBackup.Visible = false;

            if (Directory.Exists(Globals.TempPath))
            {
                try
                {
                    Directory.Delete(Globals.TempPath, true);
                }
                catch
                {
                    File.Delete(Globals.LogFileName);
                }
            }
            Log.Print(String.Format("Launching... ({0})", Globals.AppVersion));
            base.Text = "AxTools " + Globals.AppVersion.Major;
            Icon = Resources.AppIcon;
            Utils.Legacy();
            Settings.Load();
            WowAccount.LoadFromDisk();
            OnSettingsLoaded();
            WowPluginHotkeyChanged();
            WebRequest.DefaultWebProxy = null;
            Task.Factory.StartNew(LoadingStepAsync);
            BeginInvoke((MethodInvoker) delegate
            {
                Location = Settings.Location;
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

        #region Internals

        internal void ShowNotifyIconMessage(string title, string text, ToolTipIcon icon)
        {
            if (InvokeRequired)
                BeginInvoke((MethodInvoker) (() => notifyIconMain.ShowBalloonTip(30000, title, text, icon)));
            else
                notifyIconMain.ShowBalloonTip(30000, title, text, icon);
        }

        internal static bool LuaTimerEnabled = false;

        internal static MainForm Instance;

        #endregion
        
        #region Variables

        //timers
        private readonly Timer timerNotifyIcon = new Timer(1000);
        //another
        private bool isClosing;
        private WaitingOverlay startupOverlay;

        // notifyicon's Icons
        private readonly Icon appIconPluginOnLuaOn = Icon.FromHandle(Resources.AppIconPluginOnLuaOn.GetHicon());
        private readonly Icon appIconPluginOffLuaOn = Icon.FromHandle(Resources.AppIconPluginOffLuaOn.GetHicon());
        private readonly Icon appIconPluginOnLuaOff = Icon.FromHandle(Resources.AppIconPluginOnLuaOff.GetHicon());
        private readonly Icon appIconNormal = Icon.FromHandle(Resources.AppIcon1.GetHicon());

        //
        private readonly List<ToolStripMenuItem> pluginsToolStripMenuItems = new List<ToolStripMenuItem>(); 

        #endregion

        #region Timers

        private void TimerNiElapsed(object sender, ElapsedEventArgs e)
        {
            // ReSharper disable RedundantCheckBeforeAssignment
            BeginInvoke(new Action(() =>
            {
                if (Clicker.Enabled)
                {
                    notifyIconMain.Icon = Utils.EmptyIcon;
                }
                else if (notifyIconMain.Icon != appIconNormal)
                {
                    notifyIconMain.Icon = appIconNormal;
                }
            }));
            Thread.Sleep(500);
            BeginInvoke(new Action(() =>
            {
                if (LuaTimerEnabled && PluginManager.ActivePlugin != null)
                {
                    notifyIconMain.Icon = appIconPluginOnLuaOn;
                }
                else if (LuaTimerEnabled)
                {
                    notifyIconMain.Icon = appIconPluginOffLuaOn;
                }
                else if (PluginManager.ActivePlugin != null)
                {
                    notifyIconMain.Icon = appIconPluginOnLuaOff;
                }
                else if (notifyIconMain.Icon != appIconNormal)
                {
                    notifyIconMain.Icon = appIconNormal;
                }
            }));
            // ReSharper restore RedundantCheckBeforeAssignment
        }

        #endregion

        #region Keyboard hook

        private KeyboardHookListener keyboardHook;

        private void KeyboardHookKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Settings.ClickerHotkey)
            {
                KeyboardHook_ClickerHotkey();
            }
            else if (e.KeyCode == Settings.PrecompiledModulesHotkey)
            {
                KeyboardHook_PrecompiledModulesHotkey();
            }
            else if (e.KeyCode == Settings.LuaTimerHotkey)
            {
                KeyboardHook_LuaTimerHotkey();
            }
        }

        private void KeyboardHook_ClickerHotkey()
        {
            if (Settings.ClickerKey == Keys.None)
            {
                this.ShowTaskDialog("Incorrect input!", "Please select key to be pressed", TaskDialogButton.OK, TaskDialogIcon.Stop);
                return;
            }
            if (Clicker.Enabled)
            {
                Clicker.Stop();
                notifyIconMain.Icon = appIconNormal;
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
                    Clicker.Start(Settings.ClickerInterval, cProcess.MainWindowHandle, (IntPtr) Settings.ClickerKey);
                    Log.Print(string.Format("{0}:{1} :: [Clicker] Enabled, interval {2}ms, window handle 0x{3:X}", cProcess.ProcessName, cProcess.ProcessID,
                        Settings.ClickerInterval, (uint) cProcess.MainWindowHandle));
                }
            }
        }

        private void KeyboardHook_PrecompiledModulesHotkey()
        {
            if (PluginManager.ActivePlugin == null && WowProcess.GetAllWowProcesses().Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
            {
                InvokeOnClick(buttonStartStopPlugin, EventArgs.Empty);
            }
            else if (PluginManager.ActivePlugin != null)
            {
                InvokeOnClick(buttonStartStopPlugin, EventArgs.Empty);
            }
        }

        private void KeyboardHook_LuaTimerHotkey()
        {
            LuaConsole pForm = Utils.FindForm<LuaConsole>();
            if (pForm != null)
            {
                if (!pForm.TimerEnabled && WowProcess.GetAllWowProcesses().Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
                {
                    pForm.SwitchTimer();
                }
                else if (pForm.TimerEnabled)
                {
                    pForm.SwitchTimer();
                }
            }
        }

        #endregion

        #region MainFormEvents

        private void MainFormClosing(Object sender, CancelEventArgs e)
        {
            isClosing = true;
            // Close all children forms
            Form[] forms = Application.OpenForms.Cast<Form>().Where(i => i.GetType() != typeof (MainForm) && i.GetType() != typeof (MetroFlatDropShadow)).ToArray();
            foreach (Form i in forms)
            {
                i.Close();
            }
            //
            Settings.Location = Location;
            //save settings
            Settings.Save();
            WowAccount.SaveToDisk();
            //
            Clicker.Stop();
            //stop timers
            AddonsBackup.StopService();
            timerNotifyIcon.Enabled = false;
            Pinger.Stop();
            //stop watching process trace
            WoWProcessWatcher.Stop();
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
            TaskDialog taskDialog = new TaskDialog("There were errors during runtime", "AxTools", "Do you want to send log file to developer?",
                (TaskDialogButton) ((int) TaskDialogButton.Yes + (int) TaskDialogButton.No), TaskDialogIcon.Warning);
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
            if (Settings.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
            BeginInvoke((MethodInvoker) (() => OnActivated(EventArgs.Empty)));
        }

        private void LoadingStepAsync()
        {

            #region Set registration name

            while (Settings.Regname == String.Empty)
            {
                Settings.Regname = InputBox.Input("Please enter your nickname:");
                if (!string.IsNullOrWhiteSpace(Settings.Regname))
                {
                    Settings.Regname += "_" + Utils.GetRandomString(10).ToUpper();
                }
            }
            Log.Print("Registered for: " + Settings.Regname);

            #endregion

            #region Backup and delete wow logs

            if (Settings.DelWowLog && Directory.Exists(Settings.WowExe + "\\Logs") && WowProcess.GetAllWowProcesses().Count == 0)
            {
                if (File.Exists(Settings.WowExe + "\\Logs\\WoWCombatLog.txt") || Utils.CalcDirectorySize(Settings.WowExe + "\\Logs") > 104857600)
                {
                    Utils.CheckCreateDir();
                    string zipPath = String.Format("{0}\\WoWLogs.zip", Settings.AddonsBackupPath);
                    if (File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                    }
                    try
                    {
                        using (ZipFile zip = new ZipFile(zipPath, Encoding.UTF8))
                        {
                            zip.CompressionLevel = (CompressionLevel) Settings.BackupCompressionLevel;
                            zip.AddDirectory(Settings.WowExe + "\\Logs");
                            zip.Save();
                        }
                        Log.Print(String.Format("[Backup] WoW combat log's backup was placed to \"{0}\"", zipPath));
                        string[] cLogFiles = Directory.GetFiles(Settings.WowExe + "\\Logs");
                        foreach (string i in cLogFiles)
                        {
                            try
                            {
                                File.Delete(i);
                                Log.Print("[WoW logs] Log file deleted: " + i);
                            }
                            catch (Exception ex)
                            {
                                Log.Print(String.Format("[WoW logs] Error deleting log file \"{0}\": {1}", i, ex.Message), true);
                            }
                        }
                        notifyIconMain.ShowBalloonTip(10000, "WoW log files were deleted", "Backup was placed to " + Settings.AddonsBackupPath, ToolTipIcon.Info);
                    }
                    catch (Exception ex)
                    {
                        Log.Print(String.Format("[Backup] Can't backup wow combat log \"{0}\": {1}", zipPath, ex.Message), true);
                    }
                }
            }

            #endregion

            #region Processing creaturecache.wdb

            if (Settings.CreatureCache && Directory.Exists(Settings.WowExe + "\\Cache\\WDB"))
            {
                DirectoryInfo[] directories = new DirectoryInfo(Settings.WowExe + "\\Cache\\WDB").GetDirectories();
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
            if (Settings.EnableCustomPlugins)
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
            WoWProcessWatcher.Start();
            timerNotifyIcon.Enabled = true;
            
            #region Start keyboard hook

            keyboardHook = new KeyboardHookListener(Globals.GlobalHooker);
            keyboardHook.KeyDown += KeyboardHookKeyDown;
            keyboardHook.Enabled = true;

            #endregion

            startupOverlay.Close();

            #region Show update notes

            if (Globals.AppVersion.Major != Settings.LastUsedVersion.Major || Globals.AppVersion.Minor != Settings.LastUsedVersion.Minor)
            {
                Task.Factory.StartNew(() =>
                    {
                        Utils.CheckCreateDir();
                        using (WebClient pWebClient = new WebClient())
                        {
                            pWebClient.DownloadFile(Globals.DropboxPath + "/changes.jpg", Globals.TempPath + "\\changes.jpg");
                        }
                    }).ContinueWith(l =>
                        {
                            if (l.Exception == null)
                            {
                                Invoke(new Action(() => new Changes(Globals.TempPath + "\\changes.jpg").ShowDialog()));
                            }
                        });
            }

            #endregion

            #region Custom commands

            if (Settings.Regname == "Axio-5GDMJHD20R")
            {
                Process.Start("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe", "-noexit \"-file\" \"C:\\Users\\Axioma\\Desktop\\vpn.ps1\"");
            }

            #endregion

            Updater.UpdaterService.Start();

            Log.Print("AxTools started succesfully");
        }

        private void PictureBoxExtSettingsClick(object sender, EventArgs e)
        {
            new AppSettings().Show();
        }

        private void LabelPingNumMouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                case MouseButtons.Right:
                    Pinger.Stop();
                    labelPingNum.Text = "cleared";
                    Pinger.Start();
                    break;
            }
        }

        #endregion

        #region MainNotifyIconContextMenu

        private void UpdatePluginsShortcutsInTrayContextMenu()
        {
            foreach (ToolStripMenuItem i in pluginsToolStripMenuItems)
            {
                i.ShortcutKeyDisplayString = i.Text == comboBoxWowPlugins.Text ? Settings.PrecompiledModulesHotkey.ToString() : null;
                i.Enabled = PluginManager.ActivePlugin == null;
            }
            stopActivePluginorPresshotkeyToolStripMenuItem.Enabled = PluginManager.ActivePlugin != null;
            stopActivePluginorPresshotkeyToolStripMenuItem.ShortcutKeyDisplayString = Settings.PrecompiledModulesHotkey.ToString();
        }

        private void WoWRadarToolStripMenuItemClick(object sender, EventArgs e)
        {
            MetroButtonRadarClick(null, EventArgs.Empty);
        }

        private void blackMarketTrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvokeOnClick(metroButtonBlackMarketTracker, EventArgs.Empty);
        }

        private void LuaConsoleToolStripMenuItemClick(object sender, EventArgs e)
        {
            MetroButtonLuaConsoleClick(null, EventArgs.Empty);
        }

        private void LaunchWoWToolStripMenuItemClick(object sender, EventArgs e)
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
                InvokeOnClick(buttonStartStopPlugin, EventArgs.Empty);
            }
        }

        #endregion

        #region MainTab

        private void CmbboxAccSelectSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbboxAccSelect.SelectedIndex != -1)
            {
                WaitingOverlay waitingOverlay = new WaitingOverlay(this);
                waitingOverlay.Show();
                Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(l => BeginInvoke((MethodInvoker) waitingOverlay.Close));
                if (File.Exists(Settings.WowExe + "\\Wow.exe"))
                {
                    Process process = Process.Start(new ProcessStartInfo
                    {
                        WorkingDirectory = Settings.WowExe,
                        FileName = Settings.WowExe + "\\Wow.exe",
                        Arguments = "-noautolaunch64bit",
                    });
                    WowAccount wowAccount = new WowAccount(WowAccount.AllAccounts[cmbboxAccSelect.SelectedIndex].Login, WowAccount.AllAccounts[cmbboxAccSelect.SelectedIndex].Password);
                    new AutoLogin(wowAccount, process).EnterCredentialsASAPAsync();
                    if (Settings.StartVentriloWithWow && !Process.GetProcessesByName("Ventrilo").Any())
                    {
                        StartVentrilo();
                    }
                    cmbboxAccSelect.SelectedIndex = -1;
                    cmbboxAccSelect.Invalidate();
                }
                else
                {
                    this.ShowTaskDialog("WoW client not found or corrupted", "Can't locate \"Wow.exe\"", TaskDialogButton.OK, TaskDialogIcon.Stop);
                }
            }
        }

        private void linkOpenBackupFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Settings.AddonsBackupPath))
            {
                Process.Start(Settings.AddonsBackupPath);
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
            if (!File.Exists(Settings.WowExe + "\\Wow.exe"))
            {
                this.ShowTaskDialog("WoW client not found or corrupted", "Can't locate \"Wow.exe\"", TaskDialogButton.OK, TaskDialogIcon.Stop);
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Settings.WowExe,
                FileName = Settings.WowExe + "\\Wow.exe",
                Arguments = "-noautolaunch64bit",
            });
            if (Settings.StartVentriloWithWow && !Process.GetProcessesByName("Ventrilo").Any())
            {
                StartVentrilo();
            }
        }

        private void buttonWowUpdater_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Settings.WowExe + "\\World of Warcraft Launcher.exe"))
            {
                this.ShowTaskDialog("WoW client not found or corrupted", "Can't locate \"World of Warcraft Launcher.exe\"", TaskDialogButton.OK, TaskDialogIcon.Stop);
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Settings.WowExe,
                FileName = Settings.WowExe + "\\World of Warcraft Launcher.exe"
            });
        }

        private void linkEditWowAccounts_Click(object sender, EventArgs e)
        {
            new WowAccountsManager().ShowDialog(this);
            OnWowAccountsChanged();
        }

        #endregion

        #region VoipTab

        private void TileVentriloClick(object sender, EventArgs e)
        {
            StartVentrilo();
        }

        private void TileRaidcallClick(object sender, EventArgs e)
        {
            if (!File.Exists(Settings.RaidcallExe + "\\raidcall.exe"))
            {
                new TaskDialog("Executable not found", "AxTools", "Can't locate \"raidcall.exe\". Check paths in settings window", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Settings.RaidcallExe,
                FileName = Settings.RaidcallExe + "\\raidcall.exe"
            });
            Log.Print("Raidcall process started");
        }

        private void TileTeamspeak3Click(object sender, EventArgs e)
        {
            string cPath;
            if (File.Exists(Settings.TeamspeakExe + "\\ts3client_win32.exe"))
            {
                cPath = Settings.TeamspeakExe + "\\ts3client_win32.exe";
            }
            else if (File.Exists(Settings.TeamspeakExe + "\\ts3client_win64.exe"))
            {
                cPath = Settings.TeamspeakExe + "\\ts3client_win64.exe";
            }
            else
            {
                new TaskDialog("Executable not found", "AxTools",
                               "Can't locate \"ts3client_win64.exe\"/\"ts3client_win32.exe\". Check paths in settings window", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Settings.TeamspeakExe,
                FileName = cPath
            });
            Log.Print("TS3 process started");
        }

        private void TileMumbleClick(object sender, EventArgs e)
        {
            if (!File.Exists(Settings.MumbleExe + "\\mumble.exe"))
            {
                new TaskDialog("Executable not found", "AxTools", "Can't locate \"mumble.exe\". Check paths in settings window", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            Process.Start(new ProcessStartInfo {
                WorkingDirectory = Settings.MumbleExe,
                FileName = Settings.MumbleExe + "\\mumble.exe"
            });
            Log.Print("Mumble process started");
        }

        private void checkBoxStartVenriloWithWow_CheckedChanged(object sender, EventArgs e)
        {
            Settings.StartVentriloWithWow = checkBoxStartVenriloWithWow.Checked;
        }

        private void checkBoxStartRaidcallWithWow_CheckedChanged(object sender, EventArgs e)
        {
            Settings.StartRaidcallWithWow = checkBoxStartRaidcallWithWow.Checked;
        }

        private void checkBoxStartMumbleWithWow_CheckedChanged(object sender, EventArgs e)
        {
            Settings.StartMumbleWithWow = checkBoxStartMumbleWithWow.Checked;
        }

        private void checkBoxStartTeamspeak3WithWow_CheckedChanged(object sender, EventArgs e)
        {
            Settings.StartTS3WithWow = checkBoxStartTeamspeak3WithWow.Checked;
        }

        #endregion

        #region WowPluginsTab
        
        private void buttonStartStopPlugin_Click(object sender, EventArgs e)
        {
            buttonStartStopPlugin.Enabled = false;
            if (PluginManager.ActivePlugin == null)
            {
                if (!WoWManager.Hooked && !LoadInjector())
                {
                    buttonStartStopPlugin.Enabled = true;
                    return;
                }
                if (!WoWManager.WoWProcess.IsInGame)
                {
                    ShowNotifyIconMessage("Plugin error", "Player isn't logged in", ToolTipIcon.Error);
                    SystemSounds.Hand.Play();
                    buttonStartStopPlugin.Enabled = true;
                    return;
                }
                if (comboBoxWowPlugins.SelectedIndex == -1)
                {
                    ShowNotifyIconMessage("Plugin error", "You didn't select valid plugin", ToolTipIcon.Error);
                    SystemSounds.Hand.Play();
                    buttonStartStopPlugin.Enabled = true;
                }
                else
                {
                    PluginManager.StartPlugin(PluginManager.Plugins.First(i => i.Name == comboBoxWowPlugins.Text));
                    WowPluginHotkeyChanged();
                }
            }
            else
            {
                try
                {
                    PluginManager.StopPlugin();
                    WowPluginHotkeyChanged();
                    GC.Collect();
                }
                catch
                {
                    Log.Print("Plugin task failed to cancel", true);
                    this.ShowTaskDialog("Plugin error", "Plugin task failed to cancel", TaskDialogButton.OK, TaskDialogIcon.Stop);
                }
            }
            buttonStartStopPlugin.Enabled = true;
        }

        private void metroCheckBoxPluginShowIngameNotification_CheckedChanged(object sender, EventArgs e)
        {
            Settings.WowPluginsShowIngameNotifications = metroCheckBoxPluginShowIngameNotification.Checked;
        }

        private void checkBoxEnableCustomPlugins_CheckedChanged(object sender, EventArgs e)
        {
            Settings.EnableCustomPlugins = checkBoxEnableCustomPlugins.Checked;
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
            if (!WoWManager.Hooked && !LoadInjector())
            {
                new TaskDialog("Plugin error", "AxTools", "Can't inject", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            if (!WoWManager.WoWProcess.IsInGame)
            {
                new TaskDialog("Plugin error", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            new BlackMarket().Show();
        }

        private void MetroButtonRadarClick(object sender, EventArgs e)
        {
            if (WoWManager.Hooked)
            {
                var cForm = Utils.FindForm<WowRadar>();
                if (cForm != null)
                {
                    cForm.Activate();
                }
                else
                {
                    new WowRadar().Show();
                }
                return;
            }
            if (LoadInjector())
            {
                new WowRadar().Show();
            }
        }

        private void MetroButtonLuaConsoleClick(object sender, EventArgs e)
        {
            if (WoWManager.Hooked)
            {
                LuaConsole cForm = Utils.FindForm<LuaConsole>();
                if (cForm != null)
                {
                    cForm.Activate();
                }
                else
                {
                    new LuaConsole().Show();
                }
                return;
            }
            if (LoadInjector())
            {
                new LuaConsole().Show();
            }
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

        private bool LoadInjector()
        {
            int index = WowProcess.GetAllWowProcesses().Count == 1 ? 0 : ProcessSelection.SelectProcess();
            if (index != -1)
            {
                if (WowProcess.GetAllWowProcesses()[index].IsValidBuild)
                {
                    if (WowProcess.GetAllWowProcesses()[index].IsInGame)
                    {
                        switch (WoWManager.Hook(WowProcess.GetAllWowProcesses()[index]))
                        {
                            case HookResult.Successful:
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Injector loaded", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
                                return true;
                            case HookResult.IncorrectDirectXVersion:
                                this.ShowTaskDialog("Injecting error", "Incorrect DirectX version", TaskDialogButton.OK, TaskDialogIcon.SecurityError);
                                return false;
                        }
                    }
                    Log.Print("[WoW hook] Injecting error: Player isn't logged in");
                    this.ShowTaskDialog("Injecting error", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop);
                    return false;
                }
                Log.Print("[WoW hook] Injecting error: Incorrect WoW build", true);
                this.ShowTaskDialog("Injecting error", "Incorrect WoW build", TaskDialogButton.OK, TaskDialogIcon.SecurityError);
                return false;
            }
            return false;
        }

        #endregion

        #region Events()

        private void OnWowAccountsChanged()
        {
            cmbboxAccSelect.Items.Clear();
            if (WowAccount.AllAccounts.Count > 0)
            {
                cmbboxAccSelect.OverlayText = "Click to launch WoW using autopass...";
                cmbboxAccSelect.Enabled = true;
                foreach (WowAccount i in WowAccount.AllAccounts)
                {
                    cmbboxAccSelect.Items.Add(i.Login);
                }
            }
            else
            {
                cmbboxAccSelect.OverlayText = "At least one WoW account is required!";
                cmbboxAccSelect.Enabled = false;
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
                toolStripSeparator3,
                luaConsoleToolStripMenuItem,
                blackMarketTrackerToolStripMenuItem,
                toolStripSeparator2
            });
            Type[] nativePlugins = { typeof(Fishing), typeof(FlagReturner), typeof(GoodsDestroyer) };
            foreach (IPlugin i in PluginManager.Plugins.Where(i => nativePlugins.Contains(i.GetType())))
            {
                IPlugin plugin = i;
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon, delegate
                {
                    comboBoxWowPlugins.SelectedIndex = PluginManager.Plugins.IndexOf(plugin);
                    if (!WoWManager.Hooked && WowProcess.GetAllWowProcesses().Count != 1)
                    {
                        Activate();
                    }
                    InvokeOnClick(buttonStartStopPlugin, EventArgs.Empty);
                });
                pluginsToolStripMenuItems.Add(toolStripMenuItem);
                contextMenuStripMain.Items.Add(toolStripMenuItem);
            }
            if (Settings.EnableCustomPlugins && PluginManager.Plugins.Count > nativePlugins.Length)
            {
                ToolStripMenuItem customPlugins = contextMenuStripMain.Items.Add("Custom plugins") as ToolStripMenuItem;
                if (customPlugins != null)
                {
                    foreach (IPlugin i in PluginManager.Plugins.Where(i => !nativePlugins.Contains(i.GetType())))
                    {
                        IPlugin plugin = i;
                        ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(plugin.Name, plugin.TrayIcon, delegate
                        {
                            comboBoxWowPlugins.SelectedIndex = PluginManager.Plugins.IndexOf(plugin);
                            if (!WoWManager.Hooked && WowProcess.GetAllWowProcesses().Count != 1)
                            {
                                Activate();
                            }
                            InvokeOnClick(buttonStartStopPlugin, EventArgs.Empty);
                        });
                        pluginsToolStripMenuItems.Add(toolStripMenuItem);
                        customPlugins.DropDownItems.Add(toolStripMenuItem);
                    }
                }
            }
            contextMenuStripMain.Items.AddRange(new ToolStripItem[]
            {
                stopActivePluginorPresshotkeyToolStripMenuItem,
                toolStripSeparator1,
                launchWoWToolStripMenuItem
            });

            UpdatePluginsShortcutsInTrayContextMenu();
        }

        private void OnSettingsLoaded()
        {
            metroStyleManager1.Style = Settings.NewStyleColor;
            checkBoxStartVenriloWithWow.Checked = Settings.StartVentriloWithWow;
            checkBoxStartTeamspeak3WithWow.Checked = Settings.StartTS3WithWow;
            checkBoxStartRaidcallWithWow.Checked = Settings.StartRaidcallWithWow;
            checkBoxStartMumbleWithWow.Checked = Settings.StartMumbleWithWow;
            metroCheckBoxPluginShowIngameNotification.Checked = Settings.WowPluginsShowIngameNotifications;
            checkBoxEnableCustomPlugins.Checked = Settings.EnableCustomPlugins;
            OnWowAccountsChanged();
        }

        internal void WowPluginHotkeyChanged()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                buttonStartStopPlugin.Text = string.Format("{0} [{1}]", PluginManager.ActivePlugin == null ? "Start" : "Stop", Settings.PrecompiledModulesHotkey);
                comboBoxWowPlugins.Enabled = PluginManager.ActivePlugin == null;
                UpdatePluginsShortcutsInTrayContextMenu();
            });
        }

        internal void AddonsBackup_OnChangedState(int procent)
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

        internal void Pinger_DataChanged(int ping, int packetLoss)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                if (ping == -1)
                {
                    labelPingNum.Visible = packetLoss != 0;
                    labelPingNum.Text = "[n/a]::[n/a]";
                    TBProgressBar.SetProgressValue(Handle, 1, 1);
                    TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.NoProgress);
                }
                else
                {
                    labelPingNum.Text = string.Format("[{0}]::[{1}%]", ping == -1 || ping == -2 ? "n/a" : ping.ToString(), packetLoss);
                    TBProgressBar.SetProgressValue(Handle, 1, 1);
                    if (packetLoss >= Settings.PingerVeryBadNetworkProcent || ping >= Settings.PingerVeryBadNetworkPing)
                    {
                        TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.Error);
                    }
                    else if (packetLoss >= Settings.PingerBadNetworkProcent || ping >= Settings.PingerBadNetworkPing)
                    {
                        TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.Paused);
                    }
                    else
                    {
                        TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.NoProgress);
                    }
                }
            });
        }

        #endregion

        #region Helpers

        private void StartVentrilo()
        {
            if (!File.Exists(Settings.VtExe + "\\Ventrilo.exe"))
            {
                this.ShowTaskDialog("Executable not found", "Can't locate \"Ventrilo.exe\". Check paths in settings window", TaskDialogButton.OK, TaskDialogIcon.Stop);
                return;
            }
            Process process = Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Settings.VtExe,
                FileName = Settings.VtExe + "\\Ventrilo.exe",
                Arguments = "-m"
            });
            Task.Factory.StartNew(() =>
            {
                int counter = 300;
                while (counter > 0)
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

        #endregion

    }
}