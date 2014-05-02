using AxTools.Classes;
using AxTools.Classes.TaskbarProgressbar;
using AxTools.Classes.WinAPI;
using AxTools.Classes.WoW;
using AxTools.Classes.WoW.PluginSystem;
using AxTools.Components;
using AxTools.Properties;
using GreyMagic;
using Ionic.Zip;
using MouseKeyboardActivityMonitor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
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
            pingCallback = WoWPingerCallback;
            pingCallbackOnDisabled = WoWPingerCallbackOnDisabled;
            timerNotifyIcon.Elapsed += TimerNiElapsed;
            timerPinger.Elapsed += timerPinger_Elapsed;
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
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Log.Print(String.Format("Launching... ({0})", version));
            base.Text = "AxTools " + version.Major;
            Icon = Resources.AppIcon;
            Utils.Legacy();
            Settings.Load();
            LoadWowAccounts();
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

        internal void WowPluginHotkeyChanged()
        {
            BeginInvoke((MethodInvoker) delegate
            {
                buttonStartStopPlugin.Text = string.Format("{0} [{1}]", PluginManager.ActivePlugin == null ? "Start" : "Stop", Settings.PrecompiledModulesHotkey);
                comboBoxWowPlugins.Enabled = PluginManager.ActivePlugin == null;
                UpdatePluginsShortcutsInTrayContextMenu();
            });
        }

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

        private ManagementEventWatcher wowWatcherStart;
        private ManagementEventWatcher wowWatcherStop;
        //clicker
        private readonly Clicker clicker = new Clicker();
        //timers
        private readonly Timer timerNotifyIcon = new Timer(1000);
        private readonly Timer timerPinger = new Timer(2000);
        //another
        private bool isClosing;
        private WaitingOverlay startupOverlay;
        // wow accounts
        private List<WowAccount> wowAccounts = new List<WowAccount>();
        private int wowAccountSelected = -1;

        // notifyicon's Icons
        private readonly Icon appIconPluginOnLuaOn = Icon.FromHandle(Resources.AppIconPluginOnLuaOn.GetHicon());
        private readonly Icon appIconPluginOffLuaOn = Icon.FromHandle(Resources.AppIconPluginOffLuaOn.GetHicon());
        private readonly Icon appIconPluginOnLuaOff = Icon.FromHandle(Resources.AppIconPluginOnLuaOff.GetHicon());
        private readonly Icon appIconNormal = Icon.FromHandle(Resources.AppIcon1.GetHicon());

        #endregion

        #region Timers

        private void Timer2000Tick(object sender, EventArgs e)
        {

            #region AntiAFK

            if (Settings.Wasd)
            {
                foreach (WowProcess i in WowProcess.Shared.Where(i => i.IsValidBuild && i.IsInGame))
                {
                    try
                    {
                        uint lastHardwareAction = i.Memory.Read<uint>(i.Memory.ImageBase + WowBuildInfo.LastHardwareAction);
                        if (Environment.TickCount - lastHardwareAction > i.MaxTime)
                        {
                            i.MaxTime = Utils.Rnd.Next(150000, 280000);
                            i.Memory.Write(i.Memory.ImageBase + WowBuildInfo.LastHardwareAction, Environment.TickCount);
                            Log.Print(String.Format("{0}:{1} :: [Anti-AFK] Action emulated, next MaxTime: {2}", i.ProcessName, i.ProcessID, i.MaxTime));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Print(String.Format("{0}:{1} :: [Anti-AFK] Can't emulate action: {2}", i.ProcessName, i.ProcessID, ex.Message), true);
                    }
                }
            }

            #endregion

            #region AddOns backup

            TimeSpan pTimeSpan = DateTime.UtcNow - Settings.AddonsBackupLastdate;
            if (pTimeSpan.TotalHours > Settings.AddonsBackupTimer)
            {
                Settings.AddonsBackupLastdate = DateTime.UtcNow;
                ProcessPriorityClass defaultProcessPriorityClass = Process.GetCurrentProcess().PriorityClass;
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
                AddonsBackup addonsBackup = new AddonsBackup();
                addonsBackup.Show(this);
                Task.Factory.StartNew(() => addonsBackup.Start(true)).ContinueWith(l =>
                {
                    addonsBackup.Invoke((MethodInvoker) addonsBackup.Close);
                    Process.GetCurrentProcess().PriorityClass = defaultProcessPriorityClass;
                });
            }

            #endregion

            #region Regular update check

            if (Environment.TickCount - updaterLastTimeChecked >= 900000)
            {
                updaterLastTimeChecked = Environment.TickCount;
                if (!updaterShouldSkipUpdateCheck)
                {
                    Task.Factory.StartNew(CheckAndInstallUpdates);
                }
            }

            #endregion

        }

        private void TimerNiElapsed(object sender, ElapsedEventArgs e)
        {
            BeginInvoke(new Action(() => SetIcon(false)));
            Thread.Sleep(500);
            BeginInvoke(new Action(() => SetIcon(true)));
        }

        private void timerPinger_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Settings.GameServer.Port == 0)
            {
                Invoke(pingCallbackOnDisabled);
            }
            else
            {
                using (Socket pSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        lock (pingLock)
                        {
                            Stopwatch stopwatch = Stopwatch.StartNew();
                            bool result = pSocket.BeginConnect(Settings.GameServer.Ip, Settings.GameServer.Port, null, null).AsyncWaitHandle.WaitOne(1000, false);
                            long ping = stopwatch.ElapsedMilliseconds;
                            if (pingList.Count == 100)
                            {
                                pingList.RemoveAt(0);
                            }
                            pingList.Add((int)(!result || !pSocket.Connected ? -1 : ping));
                            pingPing = pingList.GetRange(pingList.Count - 10, 10).Max();
                            pingPacketLoss = pingList.Count(x => x == -1);
                            Invoke(pingCallback);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Print("[Pinger] " + ex.Message, true);
                    }
                }
            }
        }

        #endregion

        #region Pinger

        private readonly Action pingCallback;
        private readonly Action pingCallbackOnDisabled;
        private List<int> pingList = new List<int>(100) {-2, -2, -2, -2, -2, -2, -2, -2, -2, -2};
        private readonly object pingLock = new object();
        private int pingPing;
        private int pingPacketLoss;

        private void WoWPingerCallback()
        {
            labelPingNum.Text = string.Format("[{0}]::[{1}%]", pingPing == -1 || pingPing == -2 ? "n/a" : pingPing.ToString(), pingPacketLoss);
            TBProgressBar.SetProgressValue(Handle, 1, 1);
            if (pingPacketLoss >= Settings.PingerVeryBadNetworkProcent || pingPing >= Settings.PingerVeryBadNetworkPing)
            {
                TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.Error);
            }
            else if (pingPacketLoss >= Settings.PingerBadNetworkProcent || pingPing >= Settings.PingerBadNetworkPing)
            {
                TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.Paused);
            }
            else
            {
                TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.NoProgress);
            }
        }

        private void WoWPingerCallbackOnDisabled()
        {
            labelPingNum.Text = "[n/a]::[n/a]";
            TBProgressBar.SetProgressValue(Handle, 1, 1);
            TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.NoProgress);
        }

        #endregion

        #region WowAccounts

        private void LoadWowAccounts()
        {
            try
            {
                if (File.Exists(Globals.WowAccountsFilePath))
                {
                    byte[] strangeBytes = {0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48};
                    byte[] bytes = Crypt.Decrypt<RijndaelManaged>(File.ReadAllBytes(Globals.WowAccountsFilePath), strangeBytes);
                    using (MemoryStream memoryStream = new MemoryStream(bytes))
                    {
                        wowAccounts = (List<WowAccount>) new DataContractJsonSerializer(wowAccounts.GetType()).ReadObject(memoryStream);
                    }
                    Log.Print("WoW accounts was loaded");
                }
                else
                {
                    Log.Print("WoW accounts file not found");
                }
            }
            catch (Exception ex)
            {
                Log.Print("WoW accounts loading failed: " + ex.Message, true);
            }
        }

        private void SaveWowAccounts()
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    new DataContractJsonSerializer(wowAccounts.GetType()).WriteObject(memoryStream, wowAccounts);
                    byte[] strangeBytes = { 0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48 };
                    byte[] bytes = Crypt.Encrypt<RijndaelManaged>(memoryStream.ToArray(), strangeBytes);
                    File.WriteAllBytes(Globals.WowAccountsFilePath, bytes);
                }
            }
            catch (Exception ex)
            {
                Log.Print("WoW accounts saving failed: " + ex.Message, true);
            }
        }

        #endregion

        #region WoW client startup/closing handlers

        private void WowProcessStarted(object sender, EventArrivedEventArgs e)
        {
            try
            {
                int processId = Convert.ToInt32(e.NewEvent["ProcessID"]);
                string processName = e.NewEvent["ProcessName"].ToString().ToLower();
                switch (processName)
                {
                    case "wow-64.exe":
                        notifyIconMain.ShowBalloonTip(10000, "Unsupported WoW version", "AxTools doesn't support x64 versions of WoW client", ToolTipIcon.Error);
                        Log.Print(String.Format("{0}:{1} :: [Process watcher] 64bit WoW processes aren't supported", processName, processId), true);
                        break;
                    case "wow.exe":
                        WowProcess wowProcess = new WowProcess(processId);
                        WowProcess.Shared.Add(wowProcess);
                        Log.Print(String.Format("{0}:{1} :: [Process watcher] Process started, {2} total", wowProcess.ProcessName, wowProcess.ProcessID, WowProcess.Shared.Count));
                        Task.Factory.StartNew(OnWowProcessStartup, wowProcess);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Print(String.Format("{0}:{1} :: [Process watcher] Process started with error: {2}", e.NewEvent["ProcessName"], e.NewEvent["ProcessID"], ex.Message), true);
            }
            finally
            {
                e.NewEvent.Dispose();
            }
        }

        private void WowProcessStopped(object sender, EventArrivedEventArgs e)
        {
            try
            {
                if (e.NewEvent["ProcessName"].ToString().ToLower() == "wow.exe")
                {
                    int pid = Convert.ToInt32(e.NewEvent["ProcessID"]);
                    string name = e.NewEvent["ProcessName"].ToString().Substring(0, e.NewEvent["ProcessName"].ToString().Length - 4);
                    WowProcess pWowProcess = WowProcess.Shared.FirstOrDefault(x => x.ProcessID == pid);
                    if (pWowProcess != null)
                    {
                        if (WoW.Hooked && WoW.WProc.ProcessID == pWowProcess.ProcessID)
                        {
                            UnloadInjector();
                            Log.Print(String.Format("{0}:{1} :: [WoW hook] Injector unloaded", name, pid));
                        }
                        pWowProcess.Dispose();
                        Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager disposed", name, pid));
                        if (WowProcess.Shared.Remove(pWowProcess))
                        {
                            Log.Print(String.Format("{0}:{1} :: [Process watcher] Process closed, {2} total", name, pid, WowProcess.Shared.Count));
                        }
                        else
                        {
                            Log.Print(String.Format("{0}:{1} :: [Process watcher] Can't delete WowProcess instance", name, pid), true);
                        }
                    }
                    else
                    {
                        Log.Print(String.Format("{0}:{1} :: [Process watcher] Closed WoW process not found", name, pid), true);
                    }
                    if (Settings.CreatureCache && Directory.Exists(Settings.WowExe + "\\Cache\\WDB"))
                    {
                        foreach (DirectoryInfo i in new DirectoryInfo(Settings.WowExe + "\\Cache\\WDB").GetDirectories().Where(i => File.Exists(i.FullName + "\\creaturecache.wdb")))
                        {
                            File.Delete(i.FullName + "\\creaturecache.wdb");
                            Log.Print("[Cache cleaner] " + i.FullName + "\\creaturecache.wdb was deleted");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [Process watcher] Process stopped with error: {2}", e.NewEvent["ProcessName"], e.NewEvent["ProcessID"], ex.Message), true);
            }
            finally
            {
                e.NewEvent.Dispose();
            }
        }

        private void OnWowProcessStartup(object wowProcess)
        {
            try
            {
                WowProcess process = (WowProcess) wowProcess;
                Log.Print(String.Format("{0}:{1} :: [WoW hook] Attaching...", process.ProcessName, process.ProcessID));
                for (int i = 0; i < 40; i++)
                {
                    Thread.Sleep(1500);
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        if (Settings.AutoAcceptWndSetts)
                        {
                            try
                            {
                                if (Settings.Noframe)
                                {
                                    int styleWow = NativeMethods.GetWindowLong(process.MainWindowHandle, NativeMethods.GWL_STYLE) & ~(NativeMethods.WS_CAPTION | NativeMethods.WS_THICKFRAME);
                                    NativeMethods.SetWindowLong(process.MainWindowHandle, NativeMethods.GWL_STYLE, styleWow);
                                }
                                NativeMethods.SetWindowPos(process.MainWindowHandle, (IntPtr) SpecialWindowHandles.HWND_NOTOPMOST,
                                    Settings.WowWindowLocation.X, Settings.WowWindowLocation.Y, Settings.WowWindowSize.X, Settings.WowWindowSize.Y,
                                    SetWindowPosFlags.SWP_SHOWWINDOW);
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Window style is changed", process.ProcessName, process.ProcessID));
                            }
                            catch (Exception ex)
                            {
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Window changing failed with error: {2}", process.ProcessName, process.ProcessID, ex.Message), true);
                            }
                        }
                        try
                        {
                            process.Memory = new ExternalProcessReader(Process.GetProcessById(process.ProcessID));
                            Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager initialized, base address 0x{2:X}", process.ProcessName, process.ProcessID, (uint) process.Memory.ImageBase));
                            if (!process.IsValidBuild)
                            {
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager: invalid WoW executable", process.ProcessName, process.ProcessID), true);
                                this.ShowTaskDialog("Injector is locked", "Invalid WoW executable", TaskDialogButton.OK, TaskDialogIcon.SecurityError);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager initialization failed with error: {2}", process.ProcessName, process.ProcessID, ex.Message), true);
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Print("MainForm.AttachToWow: general error: " + ex.Message, true);
            }
        }

        #endregion

        #region Keyboard hook

        private KeyboardHookListener keyboardHook;

        private void KeyboardHookKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Settings.WowLoginHotkey)
            {
                KeyboardHook_WowLoginHotkey();
            }
            else if (e.KeyCode == Settings.ClickerHotkey)
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
            else if (e.KeyCode == Keys.Escape)
            {
                KeyboardHook_Escape();
            }
            else if (e.KeyCode == Keys.L)
            {
                KeyboardHook_L();
            }
        }

        private void KeyboardHook_WowLoginHotkey()
        {
            if (wowAccountSelected != -1)
            {
                IntPtr cHWND = NativeMethods.GetForegroundWindow();
                WowProcess process = WowProcess.Shared.FirstOrDefault(x => x.MainWindowHandle == cHWND);
                if (process != null)
                {
                    foreach (char i in wowAccounts[wowAccountSelected].Login)
                    {
                        NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_CHAR, (IntPtr) i, IntPtr.Zero);
                        Thread.Sleep(5);
                    }
                    IntPtr tabCode = new IntPtr(0x09);
                    NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_KEYDOWN, tabCode, IntPtr.Zero);
                    NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_KEYUP, tabCode, IntPtr.Zero);
                    Thread.Sleep(5);
                    foreach (char i in wowAccounts[wowAccountSelected].Password)
                    {
                        NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_CHAR, (IntPtr) i, IntPtr.Zero);
                        Thread.Sleep(5);
                    }
                    IntPtr enterCode = new IntPtr(0x0D);
                    NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_KEYDOWN, enterCode, IntPtr.Zero);
                    NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_KEYUP, enterCode, IntPtr.Zero);
                    Log.Print(string.Format("{0}:{1} :: [Account manager] Credendials have been entered [{2}]", process.ProcessName, process.ProcessID, wowAccounts[wowAccountSelected].Login));
                    wowAccountSelected = -1;
                }
            }
        }

        private void KeyboardHook_ClickerHotkey()
        {
            if (Settings.ClickerKey == Keys.None)
            {
                this.ShowTaskDialog("Incorrect input!", "Please select key to be pressed", TaskDialogButton.OK, TaskDialogIcon.Stop);
                return;
            }
            if (clicker.Enabled)
            {
                clicker.Stop();
                notifyIconMain.Icon = appIconNormal;
                WowProcess cProcess = WowProcess.Shared.FirstOrDefault(i => i.MainWindowHandle == clicker.Handle);
                Log.Print(cProcess != null
                    ? String.Format("{0}:{1} :: [Clicker] Disabled", cProcess.ProcessName, cProcess.ProcessID)
                    : "UNKNOWN:null :: [Clicker] Disabled");
            }
            else
            {
                WowProcess cProcess = WowProcess.Shared.FirstOrDefault(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow());
                if (cProcess != null)
                {
                    clicker.Start(Settings.ClickerInterval, cProcess.MainWindowHandle, (IntPtr) Settings.ClickerKey);
                    Log.Print(string.Format("{0}:{1} :: [Clicker] Enabled, interval {2}ms, window handle 0x{3:X}", cProcess.ProcessName, cProcess.ProcessID,
                        Settings.ClickerInterval, (uint) cProcess.MainWindowHandle));
                }
            }
        }

        private void KeyboardHook_PrecompiledModulesHotkey()
        {
            if (PluginManager.ActivePlugin == null && WowProcess.Shared.Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
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
                if (!pForm.TimerEnabled && WowProcess.Shared.Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
                {
                    pForm.SwitchTimer();
                }
                else if (pForm.TimerEnabled)
                {
                    pForm.SwitchTimer();
                }
            }
        }

        private void KeyboardHook_Escape()
        {
            //if (NativeMethods.GetForegroundWindow() == Handle && cmbboxAccSelect.Visible)
            //{
            //    cmbboxAccSelect.Visible = false;
            //    tileWowAutopass.Visible = true;
            //    linkEditWowAccounts.Visible = false;
            //    buttonWowUpdater.Visible = false;
            //    buttonLaunchWowWithoutAutopass.Visible = false;
            //}
        }

        private void KeyboardHook_L()
        {
            //if (NativeMethods.GetForegroundWindow() == Handle)
            //{
            //    Stopwatch stopwatch = Stopwatch.StartNew();
            //    if (memoryMappedFile == null)
            //    {
            //        memoryMappedFile = MemoryMappedFile.CreateOrOpen("AxTools_MMF", 1024);
            //    }
            //    using (MemoryMappedViewAccessor accessor = memoryMappedFile.CreateViewAccessor())
            //    {
            //        MMFStruct data = new MMFStruct(Process.GetProcessesByName("Wow")[0].Id, MMFCommand.SelectTarget, 0xF130C3DB00006FD7);
            //        mmfMutex.WaitOne();
            //        accessor.Write(0, ref data);
            //        accessor.Flush();
            //        mmfMutex.ReleaseMutex();
            //    }
            //    Log.Print(stopwatch.ElapsedMilliseconds);
            //}
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
            SaveWowAccounts();
            //
            clicker.Dispose();
            //stop timers
            timerAntiAfk.Enabled = false;
            timerNotifyIcon.Enabled = false;
            timerPinger.Enabled = false;
            //stop watching process trace
            if (wowWatcherStart != null)
            {
                wowWatcherStart.Stop();
                wowWatcherStart.Dispose();
                Log.Print("Starting processes trace watching is stopped");
            }
            if (wowWatcherStop != null)
            {
                wowWatcherStop.Stop();
                wowWatcherStop.Dispose();
                Log.Print("Stopping processes trace watching is stopped");
            }
            // release hook 
            if (WoW.Hooked)
            {
                UnloadInjector();
                Log.Print(String.Format("{0}:{1} :: [WoW hook] Injector unloaded", WoW.WProc.ProcessName, WoW.WProc.ProcessID));
            }
            foreach (WowProcess i in WowProcess.Shared)
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

            #region Get WoW processes already running

            foreach (Process i in Process.GetProcesses())
            {
                switch (i.ProcessName.ToLower())
                {
                    case "wow-64":
                        notifyIconMain.ShowBalloonTip(10000, "Unsupported WoW version", "AxTools doesn't support x64 versions of WoW client", ToolTipIcon.Warning);
                        break;
                    case "wow":
                        WowProcess process = new WowProcess(i.Id);
                        WowProcess.Shared.Add(process);
                        Log.Print(String.Format("{0}:{1} :: [Process watcher] Process added", i.ProcessName, i.Id));
                        Task.Factory.StartNew(OnWowProcessStartup, process);
                        break;
                }
            }

            #endregion

            #region Backup and delete wow logs

            if (Settings.DelWowLog && Directory.Exists(Settings.WowExe + "\\Logs") && WowProcess.Shared.Count == 0)
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

            if (Settings.CreatureCache && Directory.Exists(Settings.WowExe))
            {
                if (!Directory.Exists(Settings.WowExe + "\\Cache\\WDB"))
                {
                    Directory.CreateDirectory(Settings.WowExe + "\\Cache\\WDB");
                    Log.Print(String.Format("[WoW cache] Directory \"{0}\\Cache\\WDB\" created", Settings.WowExe));
                }
                var cDirectories = new DirectoryInfo(Settings.WowExe + "\\Cache\\WDB").GetDirectories();
                if (cDirectories.Length > 0)
                {
                    foreach (DirectoryInfo i in cDirectories)
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

            //continue starting...
            BeginInvoke(new Action(LoadingStepSync));
            Log.Print("AxTools :: preparation completed");
        }

        private void LoadingStepSync()
        {

            LoadPluginsAndSetupCtrls();

            #region Run timers

            timerAntiAfk.Enabled = true;
            timerNotifyIcon.Enabled = true;
            timerPinger.Enabled = true;

            #endregion

            #region Start process watcher

            wowWatcherStart = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            wowWatcherStop = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            wowWatcherStart.EventArrived += WowProcessStarted;
            wowWatcherStop.EventArrived += WowProcessStopped;
            wowWatcherStart.Start();
            wowWatcherStop.Start();

            #endregion

            #region Start keyboard hook

            keyboardHook = new KeyboardHookListener(Globals.GlobalHooker);
            keyboardHook.KeyDown += KeyboardHookKeyDown;
            keyboardHook.Enabled = true;

            #endregion

            #region Dispose temp controls and show rest

            startupOverlay.Close();

            #endregion

            #region Show update notes

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version.Major != Settings.LastUsedVersion.Major || version.Minor != Settings.LastUsedVersion.Minor)
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

            Task.Factory.StartNew(CheckAndInstallUpdates);

            Log.Print("AxTools started succesfully");
        }

        private void LoadPluginsAndSetupCtrls()
        {
            PluginManager.LoadPlugins();
            if (Settings.EnableCustomPlugins)
            {
                PluginManager.LoadPluginsFromDisk();
            }

            comboBoxWowPlugins.Items.Clear();
            comboBoxWowPlugins.Items.AddRange(PluginManager.Plugins.Where(i => i.TrayDescription != string.Empty).Select(i => i.TrayDescription).Cast<object>().ToArray());

            contextMenuStripMain.Items.Clear();
            contextMenuStripMain.Items.AddRange(new ToolStripItem[]
            {
                woWRadarToolStripMenuItem,
                toolStripSeparator3,
                luaConsoleToolStripMenuItem,
                blackMarketTrackerToolStripMenuItem,
                toolStripSeparator2
            });
            foreach (IPlugin i in PluginManager.Plugins.Where(i => i.TrayDescription != string.Empty))
            {
                IPlugin plugin = i;
                contextMenuStripMain.Items.Add(new ToolStripMenuItem(plugin.TrayDescription, plugin.TrayIcon, delegate
                {
                    comboBoxWowPlugins.SelectedIndex = PluginManager.Plugins.IndexOf(plugin);
                    if (!WoW.Hooked && WowProcess.Shared.Count != 1)
                    {
                        Activate();
                    }
                    InvokeOnClick(buttonStartStopPlugin, EventArgs.Empty);
                }));
            }
            contextMenuStripMain.Items.AddRange(new ToolStripItem[]
            {
                stopActivePluginorPresshotkeyToolStripMenuItem,
                toolStripSeparator1,
                woWAutopassToolStripMenuItem,
                customizeWoTWindowToolStripMenuItem,
                launchWoWToolStripMenuItem
            });

            UpdatePluginsShortcutsInTrayContextMenu();
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
                    lock (pingLock)
                    {
                        labelPingNum.Text = "cleared";
                        pingList = new List<int>(100) {-2, -2, -2, -2, -2, -2, -2, -2, -2, -2};
                    }
                    break;
            }
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

        private void SetIcon(bool phase)
        {
            // ReSharper disable RedundantCheckBeforeAssignment
            if (!phase)
            {
                if (clicker.Enabled)
                {
                    notifyIconMain.Icon = Utils.EmptyIcon;
                }
                else if (notifyIconMain.Icon != appIconNormal)
                {
                    notifyIconMain.Icon = appIconNormal;
                }
            }
            else
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
            }
            // ReSharper restore RedundantCheckBeforeAssignment
        }

        #endregion

        #region Update stuff

        private int updaterLastTimeChecked = Environment.TickCount;
        private bool updaterUserAlreadyAsked;
        private bool updaterShouldSkipAddonCheck;
        private bool updaterShouldSkipUpdateCheck;
        
        private void CheckAndInstallUpdates()
        {
            updaterShouldSkipUpdateCheck = true;
            Log.Print("[Updater] Checking for updates");
            Updater updateInfo = Updater.GetUpdateInfo();
            if (updateInfo.UpdateForMainExecutableIsAvailable && !updaterUserAlreadyAsked)
            {
                updaterUserAlreadyAsked = true;
                Log.Print("[Updater] Update for main executable is available");
                if (updateInfo.FilesToDownload != null && updateInfo.FilesToDownload.Count(i => !string.IsNullOrWhiteSpace(i)) > 0)
                {
                    DirectoryInfo updateDirectoryInfo = new DirectoryInfo(Application.StartupPath + "\\update");
                    if (updateDirectoryInfo.Exists)
                    {
                        updateDirectoryInfo.Delete(true);
                    }
                    updateDirectoryInfo.Create();
                    using (WebClient webClient = new WebClient())
                    {
                        foreach (string i in updateInfo.FilesToDownload)
                        {
                            string fullpath = updateDirectoryInfo.FullName + "\\" + i;
                            File.Delete(fullpath);
                            webClient.DownloadFile(Globals.DropboxPath + "/" + i, fullpath);
                        }
                    }
                }
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadFile(Globals.DropboxPath + "/Updater.exe", Application.StartupPath + "\\Updater.exe");
                    }
                }
                catch (Exception ex)
                {
                    Log.Print("[Updater] Can't download updater: " + ex.Message, true);
                    updaterShouldSkipUpdateCheck = false;
                    return;
                }
                try
                {
                    Updater.DownloadAndExtractTestPlugin();
                }
                catch (Exception ex)
                {
                    Log.Print("[Updater] Can't download test plugin: " + ex.Message, true);
                }
                TaskDialog taskDialog = new TaskDialog("Update is available", "AxTools", "Do you wish to restart now?", (TaskDialogButton) ((int) TaskDialogButton.Yes + (int) TaskDialogButton.No),
                    TaskDialogIcon.Information);
                ShowNotifyIconMessage("Update for AxTools is ready to install", "Click on icon to install", ToolTipIcon.Info);
                if (taskDialog.Show(this).CommonButton == Result.Yes)
                {
                    try
                    {
                        Log.Print("[Updater] Closing for update...");
                        Program.IsRestarting = true;
                        BeginInvoke(new Action(Close));
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log.Print("[Updater] Update error: " + ex.Message, true);
                    }
                }
            }
            if (updateInfo.UpdateForAddonIsAvailable && Settings.AxToolsAddon && !updaterShouldSkipAddonCheck)
            {
                updaterShouldSkipAddonCheck = true;
                Log.Print("[Updater] Update for addon component is available");
                Updater.DownloadAndExtractAddon();
                string text = WowProcess.Shared.Count > 0 ? "It's recommended to restart WoW client" : "...and it's ready to work!";
                if (NativeMethods.GetForegroundWindow() == Handle)
                {
                    this.ShowTaskDialog("Addon component has been updated", text, TaskDialogButton.OK, TaskDialogIcon.Information);
                }
                else
                {
                    ShowNotifyIconMessage("Addon component has been updated", text, ToolTipIcon.Info);
                }
                Log.Print("[Updater] Update for addon component has been successfully installed");
                updaterShouldSkipAddonCheck = false;
            }
            updaterShouldSkipUpdateCheck = false;
        }

        #endregion

        #region MainNotifyIconContextMenu

        private void UpdatePluginsShortcutsInTrayContextMenu()
        {
            string appendix = "    [" + Settings.PrecompiledModulesHotkey + "]";
            foreach (ToolStripItem i in contextMenuStripMain.Items)
            {
                i.Text = i.Text.Replace(appendix, string.Empty);

                if (PluginManager.Plugins.Any(k => k.TrayDescription == i.Text))
                {
                    i.Enabled = PluginManager.ActivePlugin == null;
                }
                else if (i.Text.Contains("Stop active plug-in"))
                {
                    i.Enabled = PluginManager.ActivePlugin != null;
                    i.Text = i.Text + appendix;
                }
                if (i.Text == comboBoxWowPlugins.Text)
                {
                    i.Text = i.Text + appendix;
                }
            }
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
                //buttonLaunchWowWithoutAutopass.Enabled = false;
                //buttonWowUpdater.Enabled = false;
                //Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(l => BeginInvoke((MethodInvoker) delegate
                //{
                //    buttonLaunchWowWithoutAutopass.Enabled = true;
                //    buttonWowUpdater.Enabled = true;
                //}));
                WaitingOverlay waitingOverlay = new WaitingOverlay(this);
                waitingOverlay.Show();
                Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(l => BeginInvoke((MethodInvoker) waitingOverlay.Close));
                wowAccountSelected = cmbboxAccSelect.SelectedIndex;
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
                    TileVentriloClick(null, EventArgs.Empty);
                }
                cmbboxAccSelect.SelectedIndex = -1;
                cmbboxAccSelect.Invalidate();
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
            WaitingOverlay waitingOverlay = new WaitingOverlay(this);
            waitingOverlay.Show();
            AddonsBackup addonsBackup = new AddonsBackup();
            addonsBackup.Show(this);
            Task.Factory.StartNew(() => addonsBackup.Start(false)).ContinueWith(l => Invoke(new Action(waitingOverlay.Close))).ContinueWith(l => addonsBackup.Invoke((MethodInvoker) addonsBackup.Close));
        }

        private void linkClickerSettings_Click(object sender, EventArgs e)
        {
            if (clicker.Enabled)
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
            new WowAccountsManager(wowAccounts).Show(this);
            OnWowAccountsChanged();
        }

        private void OnWowAccountsChanged()
        {
            cmbboxAccSelect.Items.Clear();
            woWAutopassToolStripMenuItem.DropDownItems.Clear();
            foreach (WowAccount i in wowAccounts)
            {
                cmbboxAccSelect.Items.Add(i.Login);
                int index = wowAccounts.IndexOf(i);
                ToolStripItem item = new ToolStripMenuItem(i.Login, null, delegate { wowAccountSelected = index; });
                woWAutopassToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        #endregion

        #region VoipTab

        private void TileVentriloClick(object sender, EventArgs e)
        {
            if (!File.Exists(Settings.VtExe + "\\Ventrilo.exe"))
            {
                this.ShowTaskDialog("Executable not found", "Can't locate \"Ventrilo.exe\". Check paths in settings window", TaskDialogButton.OK, TaskDialogIcon.Stop);
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Settings.VtExe,
                FileName = Settings.VtExe + "\\Ventrilo.exe",
                Arguments = "-m"
            });
            Log.Print("Ventrilo process started");
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
                if (!WoW.Hooked && !LoadInjector())
                {
                    buttonStartStopPlugin.Enabled = true;
                    return;
                }
                if (!WoW.WProc.IsInGame)
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
                    PluginManager.ActivePlugin = PluginManager.Plugins.First(i => i.TrayDescription == comboBoxWowPlugins.Text);
                    WowPluginHotkeyChanged();
                }
            }
            else
            {
                try
                {
                    PluginManager.ActivePlugin = null;
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
                string[] arr = PluginManager.Plugins.First(i => i.TrayDescription == comboBoxWowPlugins.Text).Description.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
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
            if (!WoW.Hooked && !LoadInjector())
            {
                new TaskDialog("Plugin error", "AxTools", "Can't inject", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            if (!WoW.WProc.IsInGame)
            {
                new TaskDialog("Plugin error", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            new BlackMarket().Show();
        }

        private void MetroButtonRadarClick(object sender, EventArgs e)
        {
            if (WoW.Hooked)
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
            if (WoW.Hooked)
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
            if (WoW.Hooked)
            {
                UnloadInjector();
                Log.Print(String.Format("{0}:{1} :: Injector unloaded", WoW.WProc.ProcessName, WoW.WProc.ProcessID));
            }
            else
            {
                Log.Print("Injector error: WoW injector isn't active", true);
                this.ShowTaskDialog("Injector error", "WoW injector isn't active", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private bool LoadInjector()
        {
            int index = WowProcess.Shared.Count == 1 ? 0 : ProcessSelection.SelectProcess();
            if (index != -1)
            {
                if (WowProcess.Shared[index].IsValidBuild)
                {
                    if (WowProcess.Shared[index].IsInGame)
                    {
                        switch (WoW.Hook(WowProcess.Shared[index]))
                        {
                            case HookResult.Successful:
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Injector loaded", WoW.WProc.ProcessName, WoW.WProc.ProcessID));
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

        private void UnloadInjector()
        {
            WowRadarOptions pWowRadarOptions = Utils.FindForm<WowRadarOptions>();
            if (pWowRadarOptions != null) pWowRadarOptions.Close();
            WowRadar pWowRadar = Utils.FindForm<WowRadar>();
            if (pWowRadar != null) pWowRadar.Close();
            LuaConsole pLuaInjector = Utils.FindForm<LuaConsole>();
            if (pLuaInjector != null) pLuaInjector.Close();
            //BlackMarket pBlackMarket = Utils.FindForm<BlackMarket>();
            //if (pBlackMarket != null) pBlackMarket.Close();
            WoW.Unhook();
        }

        #endregion
        
        private void customizeWoTWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (Process p in Process.GetProcessesByName("worldoftanks"))
                    {
                        for (int i = 0; i < 40; i++)
                        {
                            Thread.Sleep(1500);
                            p.Refresh();
                            if (p.MainWindowHandle != (IntPtr) 0)
                            {
                                if (Settings.AutoAcceptWndSetts)
                                {
                                    try
                                    {
                                        if (Settings.Noframe)
                                        {
                                            int styleWow = NativeMethods.GetWindowLong(p.MainWindowHandle, NativeMethods.GWL_STYLE);
                                            styleWow = styleWow & ~(NativeMethods.WS_CAPTION | NativeMethods.WS_THICKFRAME);
                                            NativeMethods.SetWindowLong(p.MainWindowHandle, NativeMethods.GWL_STYLE, styleWow);
                                        }
                                        NativeMethods.SetWindowPos(p.MainWindowHandle, (IntPtr) SpecialWindowHandles.HWND_NOTOPMOST, Settings.WowWindowLocation.X,
                                            Settings.WowWindowLocation.Y, Settings.WowWindowSize.X, Settings.WowWindowSize.Y,
                                            SetWindowPosFlags.SWP_SHOWWINDOW);
                                        Log.Print(String.Format("{0}:{1} :: [WoT] Window style is changed", p.ProcessName, p.Id));
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Print(String.Format("{0}:{1} :: [WoT] Window changing failed with error: {2}", p.ProcessName, p.Id, ex.Message), true);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Print("MainForm.AttachToWow: general error: " + ex.Message, true);
                }
            });
        }

    }
}