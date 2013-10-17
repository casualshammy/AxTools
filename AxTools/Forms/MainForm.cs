using AxTools.Classes;
using AxTools.Classes.WinAPI;
using AxTools.Classes.WoW;
using AxTools.Properties;
using GreyMagic;
using Ionic.Zip;
using MetroFramework.Drawing;
using MetroFramework.Forms;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
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
    internal partial class MainForm : MetroForm
    {
        public MainForm()
        {
            //File.WriteAllLines("1.txt", new[] { BitConverter.ToString(Encoding.UTF8.GetBytes("Ugh9HG*(rh9hR*(h9")), BitConverter.ToString(Encoding.UTF8.GetBytes("XC#%SC54SC%$sc5s4c%$sc46S")) });
            InitializeComponent();
            AccessibleName = "MainForm";
            pingCallback = WoWPingerCallback;
            timerNotifyIcon.Elapsed += TimerNiElapsed;
            timerPinger.Elapsed += timerPinger_Elapsed;
            Resize += MainResize;
            Closing += MainFormClosing;
            notifyIconMain.Icon = Resources.AppIcon;

            cmbboxAccSelect.Location = new Point(3, 51);
            cmbboxAccSelect.Visible = false;
            labelAccSelect.Location = new Point(109, 29);
            labelAccSelect.Visible = false;

            labelLoading.Location = new Point(Convert.ToInt32(Size.Width/2) - Convert.ToInt32(labelLoading.Size.Width/2),
                                              Convert.ToInt32(Size.Height/2) - Convert.ToInt32(labelLoading.Size.Height/2) - 20);
            labelLoading.Visible = false;
            progressBarLoading.Location = new Point(Convert.ToInt32(Size.Width/2) - Convert.ToInt32(progressBarLoading.Size.Width/2) - 5,
                                                    Convert.ToInt32(Size.Height/2) - Convert.ToInt32(progressBarLoading.Size.Height/2) + 10);
            progressBarLoading.Visible = false;

            metroTabControl1.SelectedIndex = 0;
            toggleWowPlugins.Tag = true;
            metroToolTip1.SetToolTip(labelPingNum, "This is ingame connection info. It's formatted as\r\n" +
                                                   "  [worst ping of the last 10]::[packet loss in the last 200 seconds]  \r\n" +
                                                   "Leftclick to show widget\r\n" +
                                                   "Rightclick to clear statistics");
            metroLabelSelectPlugin.Text = "Select a plugin...";
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
            Log.Print(String.Format("Launching... ({0})", version), false);
            base.Text = "AxTools " + version.Major;
            Icon = Resources.AppIcon;
            foreach (Control i in Controls)
            {
                i.Visible = false;
            }
            labelLoading.Visible = true;
            progressBarLoading.Visible = true;
            Utils.Legacy();
            Settings.Load();
            LoadWowAccounts();
            OnSettingsLoaded();
            HotkeysChanged();
            WebRequest.DefaultWebProxy = null;
            Task.Factory.StartNew(LoadingStepAsync);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (SolidBrush styleBrush = MetroPaint.GetStyleBrush(Style))
            {
                Rectangle rectRight = new Rectangle(Width - 1, 0, 1, Height);
                e.Graphics.FillRectangle(styleBrush, rectRight);
                Rectangle rectLeft = new Rectangle(0, 0, 1, Height);
                e.Graphics.FillRectangle(styleBrush, rectLeft);
                Rectangle rectBottom = new Rectangle(0, Height - 1, Width, 1);
                e.Graphics.FillRectangle(styleBrush, rectBottom);
            }
        }

        #region Internals

        internal delegate void PingStatisticsEvent(int ping, int packetLoss);
        internal static event PingStatisticsEvent PingStatisticsChanged;

        internal void HotkeysChanged()
        {
            toggleWowPlugins.ExtraText = Settings.PrecompiledModulesHotkey.ToString();
            foreach (ToolStripItem i in from ToolStripItem i in contextMenuStripMain.Items where i.Text.Contains("Stop active plug-in") select i)
            {
                i.Text = "Stop active plug-in (or press <" + Settings.PrecompiledModulesHotkey + ">)";
            }
        }

        internal void ShowNotifyIconMessage(string title, string text, ToolTipIcon icon)
        {
            if (InvokeRequired)
                Invoke(new Action(() => notifyIconMain.ShowBalloonTip(30000, title, text, icon)));
            else
                notifyIconMain.ShowBalloonTip(30000, title, text, icon);
        }

        internal static bool LuaTimerEnabled = false;

        #endregion
        
        #region Variables
        
        //List of WoW processes
        private readonly List<WowProcess> wowProcessesPrivateList = new List<WowProcess>();
        private readonly object wowProcessesLock = new object();
        private List<WowProcess> WowProcesses
        {
            get
            {
                lock (wowProcessesLock)
                {
                    return wowProcessesPrivateList;
                }
            }
        } 
        private ManagementEventWatcher wowWatcherStart;
        private ManagementEventWatcher wowWatcherStop;
        private int wowKillerCountdown = Environment.TickCount;
        //clicker
        private IntPtr clickerWindow = IntPtr.Zero;
        //timers
        private readonly Timer timerNotifyIcon = new Timer(1000);
        private readonly Timer timerPinger = new Timer(2000);
        //another
        private bool isClosing;
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
                foreach (WowProcess i in WowProcesses.Where(i => i.IsValidBuild && i.IsInGame))
                {
                    try
                    {
                        uint lastHardwareAction = i.Memory.Read<uint>(i.Memory.ImageBase + WowBuildInfo.LastHardwareAction);
                        if (Environment.TickCount - lastHardwareAction > i.MaxTime)
                        {
                            i.MaxTime = Utils.Rnd.Next(150000, 280000);
                            i.Memory.Write(i.Memory.ImageBase + WowBuildInfo.LastHardwareAction, Environment.TickCount);
                            Log.Print(String.Format("{0}:{1} :: [Anti-AFK] Action emulated, next MaxTime: {2}", i.ProcessName, i.ProcessID, i.MaxTime), false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Print(String.Format("{0}:{1} :: [Anti-AFK] Can't emulate action: {2}", i.ProcessName, i.ProcessID, ex.Message), true);
                    }
                }
            }

            #endregion

            #region WoW killer

            if (Environment.TickCount - wowKillerCountdown > 180000)
            {
                try
                {
                    foreach (var i in WowProcesses.Where(i => i.MainWindowTitle.ToLower() != "world of warcraft"))
                    {
                        Process.GetProcessById(i.ProcessID).Kill();
                        Log.Print(String.Format("{0}:{1} :: Process killed (reason: haven't window)", i.ProcessName, i.ProcessID), false);
                    }
                    wowKillerCountdown = Environment.TickCount;
                }
                catch (Exception ex)
                {
                    Log.Print("Search for incorrect WoW process failed: " + ex.Message, true);
                }
            }

            #endregion

            #region AddOns backup

            TimeSpan pTimeSpan = DateTime.UtcNow - Settings.AddonsBackupLastdate;
            if (pTimeSpan.TotalHours > Settings.AddonsBackupTimer)
            {
                Settings.AddonsBackupLastdate = DateTime.UtcNow;
                Task.Factory.StartNew(BackupAddons, true);
            }

            #endregion

            #region Regular update check

            if (Environment.TickCount - updaterLastTimeChecked >= 900000)
            {
                updaterLastTimeChecked = Environment.TickCount;
                Task.Factory.StartNew(CheckAndInstallUpdates);
            }

            #endregion
        }

        private void TimerClickerTick(object sender, EventArgs e)
        {
            NativeMethods.PostMessage(clickerWindow, WM_MESSAGE.WM_KEYDOWN, (IntPtr) Settings.ClickerKey, IntPtr.Zero);
            NativeMethods.PostMessage(clickerWindow, WM_MESSAGE.WM_KEYUP, (IntPtr) Settings.ClickerKey, IntPtr.Zero);
        }

        private void TimerNiElapsed(object sender, ElapsedEventArgs e)
        {
            BeginInvoke(new Action(() => SetIcon(false)));
            Thread.Sleep(500);
            BeginInvoke(new Action(() => SetIcon(true)));

            //if (timerClicker.Enabled)
            //{
            //    notifyIconMain.Icon = Utils.EmptyIcon;
            //}
            //if (moduleEnabled)
            //{
            //    notifyIconModule.Icon = Utils.EmptyIcon;
            //}

            //if (timerClicker.Enabled)
            //{
            //    notifyIconMain.Icon = Resources.AppIcon;
            //}
            //if (moduleEnabled)
            //{
            //    notifyIconModule.Icon = wowPluginIcon;
            //}
        }

        private void timerPinger_Elapsed(object sender, ElapsedEventArgs e)
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
                    Log.Print("[Pinger] Thread error: " + ex.Message, true);
                }
            }
        }

        #endregion

        #region Pinger

        private readonly Action pingCallback;
        private List<int> pingList = new List<int>(100) {-2, -2, -2, -2, -2, -2, -2, -2, -2, -2};
        private readonly object pingLock = new object();
        private int pingPing;
        private int pingPacketLoss;

        private void WoWPingerCallback()
        {
            labelPingNum.Text = string.Format("[{0}]::[{1}%]", pingPing == -1 || pingPing == -2 ? "n/a" : pingPing.ToString(), pingPacketLoss);
            if (PingStatisticsChanged != null)
            {
                PingStatisticsChanged(pingPing, pingPacketLoss);
            }
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
                    Log.Print("WoW accounts was loaded", false);
                }
                else
                {
                    Log.Print("WoW accounts file not found", false);
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
                        WowProcesses.Add(wowProcess);
                        Log.Print(String.Format("{0}:{1} :: [Process watcher] Process started, {2} total", wowProcess.ProcessName, wowProcess.ProcessID, WowProcesses.Count), false);
                        wowKillerCountdown = Environment.TickCount;
                        Task.Factory.StartNew(OnWowProcessStartup, wowProcess);
                        if (Settings.AutoPingWidget)
                        {
                            Invoke(new Action(() =>
                            {
                                if (Utils.FindForm<PingWidget>() == null)
                                {
                                    new PingWidget().Show();
                                }
                            }));
                        }
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
                    WowProcess pWowProcess = WowProcesses.FirstOrDefault(x => x.ProcessID == pid);
                    if (pWowProcess != null)
                    {
                        if (WoW.Hooked && WoW.WProc.ProcessID == pWowProcess.ProcessID)
                        {
                            UnloadInjector();
                            Log.Print(String.Format("{0}:{1} :: [WoW hook] Injector unloaded", name, pid), false);
                        }
                        pWowProcess.Dispose();
                        Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager disposed", name, pid), false);
                        if (WowProcesses.Remove(pWowProcess))
                        {
                            Log.Print(String.Format("{0}:{1} :: [Process watcher] Process closed, {2} total", name, pid, WowProcesses.Count), false);
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
                            Log.Print("[Cache cleaner] " + i.FullName + "\\creaturecache.wdb was deleted", false);
                        }
                    }
                    if (WowProcesses.Count == 0)
                    {
                        PingWidget pingWidget = Utils.FindForm<PingWidget>();
                        if (pingWidget != null) pingWidget.Close();
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
                Log.Print(String.Format("{0}:{1} :: [WoW hook] Attaching...", process.ProcessName, process.ProcessID), false);
                for (int i = 0; i < 40; i++)
                {
                    Thread.Sleep(1500);
                    if (process.MainWindowHandle != (IntPtr) 0) // && p.MainWindowTitle.ToLower() == "world of warcraft"
                    {
                        if (Settings.AutoAcceptWndSetts)
                        {
                            try
                            {
                                if (Settings.Noframe)
                                {
                                    int styleWow = NativeMethods.GetWindowLong(process.MainWindowHandle, NativeMethods.GWL_STYLE);
                                    styleWow = styleWow & ~(NativeMethods.WS_CAPTION | NativeMethods.WS_THICKFRAME);
                                    NativeMethods.SetWindowLong(process.MainWindowHandle, NativeMethods.GWL_STYLE, styleWow);
                                }
                                NativeMethods.SetWindowPos(process.MainWindowHandle, (IntPtr) SpecialWindowHandles.HWND_NOTOPMOST, Settings.WowWindowLocation.X,
                                                           Settings.WowWindowLocation.Y, Settings.WowWindowSize.X, Settings.WowWindowSize.Y,
                                                           SetWindowPosFlags.SWP_SHOWWINDOW);
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Window style is changed", process.ProcessName, process.ProcessID), false);
                            }
                            catch (Exception ex)
                            {
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Window changing failed with error: {2}",
                                                        process.ProcessName, process.ProcessID, ex.Message), true);
                            }
                        }
                        try
                        {
                            process.Memory = new ExternalProcessReader(Process.GetProcessById(process.ProcessID));
                            Log.Print(
                                String.Format("{0}:{1} :: [WoW hook] Memory manager initialized, base address 0x{2:X}", process.ProcessName, process.ProcessID,
                                              (uint) process.Memory.ImageBase), false);
                            if (!process.IsValidBuild)
                            {
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager: invalid WoW executable", process.ProcessName, process.ProcessID),
                                          true);
                                this.ShowTaskDialog("Anti-AFK blocked", "Invalid WoW executable", TaskDialogButton.OK, TaskDialogIcon.SecurityError);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager initialization failed with error: {2}",
                                                    process.ProcessName, process.ProcessID, ex.Message), true);
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
                if (wowAccountSelected != -1)
                {
                    IntPtr cHWND = NativeMethods.GetForegroundWindow();
                    WowProcess process = WowProcesses.FirstOrDefault(x => x.MainWindowHandle == cHWND);
                    if (process != null)
                    {
                        Log.Print(
                            String.Format("{0}:{1} :: [Account manager] Logging in with pre-entered credentials [{2}]", process.ProcessName, process.ProcessID,
                                          wowAccounts[wowAccountSelected].Login), false);
                        foreach (char i in wowAccounts[wowAccountSelected].Login)
                        {
                            NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_CHAR, (IntPtr) i, IntPtr.Zero);
                            Thread.Sleep(5);
                        }
                        NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_KEYDOWN, (IntPtr) 0x09, IntPtr.Zero);
                        NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_KEYUP, (IntPtr) 0x09, IntPtr.Zero);
                        Thread.Sleep(5);
                        foreach (char i in wowAccounts[wowAccountSelected].Password)
                        {
                            NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_CHAR, (IntPtr) i, IntPtr.Zero);
                            Thread.Sleep(5);
                        }
                        NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_KEYDOWN, (IntPtr) 0x0D, IntPtr.Zero);
                        NativeMethods.PostMessage(cHWND, WM_MESSAGE.WM_KEYUP, (IntPtr) 0x0D, IntPtr.Zero);
                        Log.Print(String.Format("{0}:{1} :: [Account manager] Credendials have been entered [{2}]", process.ProcessName, process.ProcessID,
                                                wowAccounts[wowAccountSelected].Login), false);
                        wowAccountSelected = -1;
                    }
                }
            }
            else if (e.KeyCode == Settings.ClickerHotkey)
            {
                if (Settings.ClickerKey == Keys.None)
                {
                    this.ShowTaskDialog("Incorrect input!", "Please select key to be pressed", TaskDialogButton.OK, TaskDialogIcon.Stop);
                    return;
                }
                if (timerClicker.Enabled)
                {
                    timerClicker.Enabled = false;
                    notifyIconMain.Icon = appIconNormal;
                    WowProcess cProcess = WowProcesses.FirstOrDefault(i => i.MainWindowHandle == clickerWindow);
                    Log.Print(cProcess != null
                                  ? String.Format("{0}:{1} :: [Clicker] Disabled", cProcess.ProcessName, cProcess.ProcessID)
                                  : "UNKNOWN:null :: [Clicker] Disabled", false);
                }
                else
                {
                    var cProcess = WowProcesses.FirstOrDefault(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow());
                    if (cProcess != null)
                    {
                        clickerWindow = cProcess.MainWindowHandle;
                        timerClicker.Interval = Settings.ClickerInterval;
                        timerClicker.Enabled = true;
                        TimerClickerTick(null, null);
                        Log.Print(
                            String.Format("{0}:{1} :: [Clicker] Enabled, interval {2}ms, window handle 0x{3:X}", cProcess.ProcessName, cProcess.ProcessID,
                                          Settings.ClickerInterval, (uint) clickerWindow), false);
                    }
                }
            }
            else if (e.KeyCode == Settings.PrecompiledModulesHotkey)
            {
                if (!toggleWowPlugins.Checked && WowProcesses.Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
                {
                    InvokeOnClick(toggleWowPlugins, EventArgs.Empty);
                }
                else if (toggleWowPlugins.Checked)
                {
                    InvokeOnClick(toggleWowPlugins, EventArgs.Empty);
                }
            }
            else if (e.KeyCode == Settings.LuaTimerHotkey)
            {
                LuaConsole pForm = Utils.FindForm<LuaConsole>();
                if (pForm != null)
                {
                    if (!pForm.TimerEnabled && WowProcesses.Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
                    {
                        pForm.SwitchTimer();
                    }
                    else if (pForm.TimerEnabled)
                    {
                        pForm.SwitchTimer();
                    }
                }
            }
        }

        #endregion

        #region Backup Add-ons

        private void BackupAddons(object trayMode)
        {
            if (!Directory.Exists(Settings.WowExe + "\\WTF"))
            {
                Log.Print("Backup error: WTF directory isn't found", false);
                if ((bool) trayMode)
                {
                    ShowNotifyIconMessage("Backup error", "\"WTF\" folder isn't found", ToolTipIcon.Error);
                }
                else
                {
                    this.ShowTaskDialog("Backup error", "\"WTF\" folder isn't found", TaskDialogButton.OK, TaskDialogIcon.Stop);
                }
                return;
            }
            if (Utils.CalcDirectorySize(Settings.WowExe + "\\WTF") > 1048576000)
            {
                Log.Print("Backup error: WTF directory is too large", true);
                if ((bool) trayMode)
                {
                    ShowNotifyIconMessage("Backup error", "WTF directory is too large", ToolTipIcon.Error);
                }
                else
                {
                    this.ShowTaskDialog("Backup error", "WTF directory is too large", TaskDialogButton.OK, TaskDialogIcon.Stop);
                }
                return;
            }
            Log.Print("BackupAddons :: Starting...", false);
            DirectoryInfo backupDirectory = new DirectoryInfo(Settings.AddonsBackupPath);
            if (!backupDirectory.Exists)
            {
                backupDirectory.Create();
                Log.Print("BackupAddons :: Backup directory created", false);
            }
            FileInfo[] backupFiles =
                backupDirectory.GetFileSystemInfos().Where(i => i.Name.Contains("AddonsBackup_") && i is FileInfo).Cast<FileInfo>().ToArray();
            Log.Print("BackupAddons :: Total backup files: " + backupFiles.Length, false);
            if (backupFiles.Length >= Settings.AddonsBackupNum)
            {
                FileInfo oldestFile = null;
                DateTime oldestFileCreationTimeUtc = DateTime.UtcNow;
                foreach (FileInfo i in backupFiles)
                {
                    if (i.CreationTimeUtc < oldestFileCreationTimeUtc)
                    {
                        oldestFileCreationTimeUtc = i.CreationTimeUtc;
                        oldestFile = i;
                    }
                }
                if (oldestFile != null)
                {
                    oldestFile.Delete();
                    Log.Print("BackupAddons :: Old backup file is deleted: " + oldestFile.Name, false);
                }
            }
            try
            {
                if ((bool) trayMode)
                {
                    ShowNotifyIconMessage("Performing backup operation", "Please don't close AxTools until the operation is completed", ToolTipIcon.Info);
                }
                string zipPath = String.Format("{0}\\AddonsBackup_{1:yyyyMMdd_HHmmss}.zip", Settings.AddonsBackupPath, DateTime.UtcNow);
                Log.Print("BackupAddons :: Zipping to file: " + zipPath, false);
                using (ZipFile zip = new ZipFile(zipPath, Encoding.UTF8))
                {
                    zip.CompressionLevel = (CompressionLevel) Settings.BackupCompressionLevel;
                    zip.AddDirectory(Settings.WowExe + "\\WTF", "\\WTF");
                    zip.AddDirectory(Settings.WowExe + "\\Interface", "\\Interface");
                    zip.Save();
                }
                Log.Print("BackupAddons :: Backup successfully created: " + zipPath, false);
                if ((bool) trayMode)
                {
                    ShowNotifyIconMessage("AddOns backup operation was successfully completed", "Backup file was stored in " + Settings.AddonsBackupPath,
                                          ToolTipIcon.Info);
                }
                else
                {
                    this.ShowTaskDialog("Backup successful", "Backup file was stored in " + Settings.AddonsBackupPath, TaskDialogButton.OK,
                                        TaskDialogIcon.Information);
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                Log.Print("BackupAddons :: Backup error: Zipping failed: " + ex.Message, true);
                this.ShowTaskDialog("Backup error", "Zipping failed\r\n" + ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
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
            SaveWowAccounts();
            //stop timers
            timerAntiAfk.Enabled = false;
            timerClicker.Enabled = false;
            timerNotifyIcon.Enabled = false;
            timerPinger.Enabled = false;
            //stop watching process trace
            if (wowWatcherStart != null)
            {
                wowWatcherStart.Stop();
                wowWatcherStart.Dispose();
                Log.Print("Starting processes trace watching is stopped", false);
            }
            if (wowWatcherStop != null)
            {
                wowWatcherStop.Stop();
                wowWatcherStop.Dispose();
                Log.Print("Stopping processes trace watching is stopped", false);
            }
            // release hook 
            if (WoW.Hooked)
            {
                UnloadInjector();
                Log.Print(String.Format("{0}:{1} :: [WoW hook] Injector unloaded", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
            }
            foreach (WowProcess i in WowProcesses)
            {
                string name = i.ProcessName;
                i.Dispose();
                Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager disposed", name, i.ProcessID), false);
            }
            if (keyboardHook != null && keyboardHook.Enabled)
            {
                keyboardHook.Dispose();
                Log.Print("Keyboard hook disposed", false);
            }
            Log.Print("AxTools closed", false);
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

        private void MainResize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) Hide();
        }

        private void LoadingStepAsync()
        {
            #region Set form location

            Invoke(new Action(() =>
                {
                    Location = Settings.Location;
                    //metroFlatShadowForm.Location = Settings.Location;
                    OnActivated(EventArgs.Empty);
                }));

            #endregion

            #region Set registration name

            while (Settings.Regname == String.Empty)
            {
                InputBox.Input("Please enter your nickname:", out Settings.Regname);
                if (Settings.Regname != String.Empty)
                {
                    Settings.Regname += String.Format("-{0}", Utils.GetRandomString(10)).ToUpper();
                }
            }
            Log.Print("Registered for: " + Settings.Regname, false);

            #endregion

            Task.Factory.StartNew(CheckAndDownloadPrerequisites);

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
                        WowProcesses.Add(process);
                        Log.Print(String.Format("{0}:{1} :: [Process watcher] Process added", i.ProcessName, i.Id), false);
                        Task.Factory.StartNew(OnWowProcessStartup, process);
                        break;
                }
            }

            #endregion

            #region Backup and delete wow logs

            if (Settings.DelWowLog && Directory.Exists(Settings.WowExe + "\\Logs") && WowProcesses.Count == 0)
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
                        Log.Print(String.Format("[Backup] WoW combat log's backup was placed to \"{0}\"", zipPath), false);
                        string[] cLogFiles = Directory.GetFiles(Settings.WowExe + "\\Logs");
                        foreach (string i in cLogFiles)
                        {
                            try
                            {
                                File.Delete(i);
                                Log.Print("[WoW logs] Log file deleted: " + i, false);
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
                    Log.Print(String.Format("[WoW cache] Directory \"{0}\\Cache\\WDB\" created", Settings.WowExe), false);
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
                                Log.Print("[WoW cache] " + i.FullName + "\\creaturecache.wdb was deleted", false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Print(String.Format("[WoW cache] Can't delete cache file ({0}): {1}", ex.Message, i.FullName), false);
                        }
                    }
                }
            }

            #endregion

            //continue starting...
            BeginInvoke(new Action(LoadingStepSync));
            Log.Print("AxTools :: preparation completed", false);
        }

        private void LoadingStepSync()
        {
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

            keyboardHook = new KeyboardHookListener(new GlobalHooker());
            keyboardHook.KeyDown += KeyboardHookKeyDown;
            keyboardHook.Enabled = true;

            #endregion

            #region Show ping widget if we have WoW client launched

            if (Settings.AutoPingWidget && WowProcesses.Any())
            {
                try
                {
                    new PingWidget().Show();
                }
                catch (Exception ex)
                {
                    Log.Print("[Pinger] Can't load ping widget: " + ex.Message, false);
                }
            }

            #endregion

            #region Dispose temp controls and show rest

            labelLoading.Dispose();
            progressBarLoading.Dispose();
            foreach (Control i in Controls)
            {
                i.Visible = true;
            }

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

            Log.Print("AxTools started succesfully", false);
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
                    var cForm = Utils.FindForm<PingWidget>();
                    if (cForm == null)
                    {
                        new PingWidget().Show();
                    }
                    else
                    {
                        cForm.TopMost = true;
                        cForm.Activate();
                    }
                    break;
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
            timerClicker.Interval = Settings.ClickerInterval;
            checkBoxStartVenriloWithWow.Checked = Settings.StartVentriloWithWow;
            checkBoxStartTeamspeak3WithWow.Checked = Settings.StartTS3WithWow;
            checkBoxStartRaidcallWithWow.Checked = Settings.StartRaidcallWithWow;
            checkBoxStartMumbleWithWow.Checked = Settings.StartMumbleWithWow;
            metroCheckBoxPluginShowIngameNotification.Checked = Settings.WowPluginsShowIngameNotifications;

            comboBoxWowAccounts.Items.Clear();
            foreach (WowAccount i in wowAccounts)
            {
                comboBoxWowAccounts.Items.Add(i.Login);
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }

        private void SetIcon(bool phase)
        {
            // ReSharper disable RedundantCheckBeforeAssignment
            if (!phase)
            {
                if (timerClicker.Enabled)
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
                if (LuaTimerEnabled && moduleEnabled)
                {
                    notifyIconMain.Icon = appIconPluginOnLuaOn;
                }
                else if (LuaTimerEnabled)
                {
                    notifyIconMain.Icon = appIconPluginOffLuaOn;
                }
                else if (moduleEnabled)
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
        private bool updaterAddonComponentLock = true;
        private string[] updaterFilesToDownload;

        private Enums.UpdateResult GetUpdateInfo()
        {
            Enums.UpdateResult result = Enums.UpdateResult.None;
            string updateString;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    updateString = webClient.DownloadString(Globals.UpdateFilePath);
                }
            }
            catch (Exception ex)
            {
                Log.Print("[Updater] Fetching error: " + ex.Message, true);
                return result;
            }
            if (!String.IsNullOrWhiteSpace(updateString))
            {
                using (StringReader stringReader = new StringReader(updateString))
                {
                    while (stringReader.Peek() != -1)
                    {
                        try
                        {
                            string nextString = stringReader.ReadLine();
                            if (nextString != null)
                            {
                                string[] pair = nextString.Split(new[] { ":::::" }, StringSplitOptions.None);
                                switch (pair[0])
                                {
                                    case "CurrentAxToolsVersion":
                                        Version localVersion = Assembly.GetExecutingAssembly().GetName().Version;
                                        Version serverVersion = new Version(pair[1]);
                                        if (localVersion.Major != serverVersion.Major || localVersion.Minor != serverVersion.Minor || localVersion.Build != serverVersion.Build)
                                        {
                                            result |= Enums.UpdateResult.UpdateForMainExecutableIsAvailable;
                                        }
                                        break;
                                    case "CurrentAddonVersion":
                                        if (Directory.Exists(Settings.WowExe + "\\Interface\\AddOns"))
                                        {
                                            string localAddonVersionFile = Settings.WowExe + "\\Interface\\AddOns\\ax_tools\\ax_tools.toc";
                                            string localAddonVersion = string.Empty;
                                            if (File.Exists(localAddonVersionFile))
                                            {
                                                localAddonVersion = File.ReadAllLines(localAddonVersionFile)[1];
                                            }
                                            string serverAddonVersion = pair[1];
                                            if (!File.Exists(localAddonVersionFile) || serverAddonVersion != localAddonVersion)
                                            {
                                                result |= Enums.UpdateResult.UpdateForAddonIsAvailable;
                                            }
                                        }
                                        break;
                                    case "FilesToDownload":
                                        updaterFilesToDownload = pair[1].Split(',');
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Print("[Updater] Parsing error: " + ex.Message, true);
                        }
                    }
                }
            }
            else
            {
                Log.Print("[Updater] Update file fetched, but it's empty!", true);
            }
            return result;
        }

        private void CheckAndInstallUpdates()
        {
            Log.Print("[Updater] Checking for updates...", false);
            Enums.UpdateResult updateInfo = GetUpdateInfo();
            if (updateInfo.HasFlag(Enums.UpdateResult.UpdateForMainExecutableIsAvailable) && !updaterUserAlreadyAsked)
            {
                updaterUserAlreadyAsked = true;
                Log.Print("[Updater] Update for main executable is available", false);
                if (updaterFilesToDownload != null && updaterFilesToDownload.Count(i => !string.IsNullOrWhiteSpace(i)) > 0)
                {
                    DirectoryInfo updateDirectoryInfo = new DirectoryInfo(Application.StartupPath + "\\update");
                    if (updateDirectoryInfo.Exists)
                    {
                        updateDirectoryInfo.Delete(true);
                    }
                    updateDirectoryInfo.Create();
                    using (WebClient webClient = new WebClient())
                    {
                        foreach (string i in updaterFilesToDownload)
                        {
                            string fullpath = updateDirectoryInfo.FullName + "\\" + i;
                            File.Delete(fullpath);
                            webClient.DownloadFile(Globals.DropboxPath + "/" + i, fullpath);
                        }
                    }
                }
                DownloadUpdater();
                TaskDialog taskDialog = new TaskDialog("Update is available", "AxTools", "Do you wish to restart now?", (TaskDialogButton) ((int) TaskDialogButton.Yes + (int) TaskDialogButton.No),
                    TaskDialogIcon.Information);
                ShowNotifyIconMessage("Update for AxTools is ready to install", "Click on icon to install", ToolTipIcon.Info);
                if (taskDialog.Show(this).CommonButton == Result.Yes)
                {
                    try
                    {
                        Log.Print("[Updater] Closing for update...", false);
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
            if (updateInfo.HasFlag(Enums.UpdateResult.UpdateForAddonIsAvailable) && Settings.AxToolsAddon && updaterAddonComponentLock)
            {
                updaterAddonComponentLock = false;
                Log.Print("[Updater] Update for addon component is available", false);
                DownloadAddon();
                string text = WowProcesses.Count > 0 ? "It's recommended to restart WoW client" : "...and it's ready to work!";
                if (NativeMethods.GetForegroundWindow() == Handle)
                {
                    this.ShowTaskDialog("Addon component has been updated", text, TaskDialogButton.OK, TaskDialogIcon.Information);
                }
                else
                {
                    ShowNotifyIconMessage("Addon component has been updated", text, ToolTipIcon.Info);
                }
                Log.Print("[Updater] Update for addon component has been successfully installed", false);
                updaterAddonComponentLock = true;
            }
        }

        private void DownloadUpdater()
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(Globals.DropboxPath + "/Updater.exe", Application.StartupPath + "\\Updater.exe");
            }
        }

        private void CheckAndDownloadPrerequisites()
        {
            DirectoryInfo directory = new DirectoryInfo(Application.StartupPath + "\\redist");
            if (!directory.Exists)
            {
                directory.Create();
            }
            string[] files = { "dotNetFx40_Full_setup.exe", "dxwebsetup.exe", "vcredist_x86.exe" };
            foreach (string i in files.Where(i => !File.Exists(directory.FullName + "\\" + i)))
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(Globals.DropboxPath + "/" + i, directory.FullName + "\\" + i);
                }
            }
        }

        private void DownloadAddon()
        {
            string zipPath = Globals.TempPath + "\\ax_tools.zip";
            Utils.CheckCreateDir();
            File.Delete(zipPath);
            try
            {
                using (WebClient pWebClient = new WebClient())
                {
                    pWebClient.DownloadFile(Globals.DropboxPath + "/ax_tools.zip", zipPath);
                }
                using (ZipFile zipFile = ZipFile.Read(zipPath, new ReadOptions { Encoding = Encoding.UTF8 }))
                {
                    zipFile.ExtractAll(Settings.WowExe + "\\Interface\\AddOns", ExtractExistingFileAction.OverwriteSilently);
                }
                Log.Print("AddOn component successfully updated", false);
            }
            catch (Exception ex)
            {
                Log.Print("Download addon error: " + ex.Message, true);
            }
        }

        #endregion

        #region MainNotifyIconContextMenu

        private void FishingToolStripMenuItemClick(object sender, EventArgs e)
        {
            comboBoxWowPlugins.SelectedIndex = 0;
            if (!WoW.Hooked && WowProcesses.Count != 1)
            {
                Activate();
            }
            InvokeOnClick(toggleWowPlugins, EventArgs.Empty);
        }

        private void CaptureFlagsorbsOnTheBattlefieldsToolStripMenuItemClick(object sender, EventArgs e)
        {
            comboBoxWowPlugins.SelectedIndex = 1;
            if (!WoW.Hooked && WowProcesses.Count != 1)
            {
                Activate();
            }
            InvokeOnClick(toggleWowPlugins, EventArgs.Empty);
        }

        private void MillingdisenchantingprospectingToolStripMenuItemClick(object sender, EventArgs e)
        {
            comboBoxWowPlugins.SelectedIndex = 2;
            if (!WoW.Hooked && WowProcesses.Count != 1)
            {
                Activate();
            }
            InvokeOnClick(toggleWowPlugins, EventArgs.Empty);
        }

        private void WoWRadarToolStripMenuItemClick(object sender, EventArgs e)
        {
            TileRadarClick(null, EventArgs.Empty);
        }

        private void blackMarketTrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvokeOnClick(metroButtonBlackMarketTracker, EventArgs.Empty);
        }

        private void LuaConsoleToolStripMenuItemClick(object sender, EventArgs e)
        {
            TileLuaConsoleClick(null, EventArgs.Empty);
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
            if (moduleEnabled)
            {
                InvokeOnClick(toggleWowPlugins, EventArgs.Empty);
            }
        }

        #endregion

        #region WowTab

        private void CmbboxAccSelectSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbboxAccSelect.SelectedIndex != -1)
            {
                wowAccountSelected = cmbboxAccSelect.SelectedIndex;
                cmbboxAccSelect.Visible = false;
                labelAccSelect.Visible = false;
                tileWowAutopass.Visible = true;
                if (!File.Exists(Settings.WowExe + "\\Wow.exe"))
                {
                    this.ShowTaskDialog("WoW client not found or corrupted", "Can't locate \"Wow.exe\"",
                        TaskDialogButton.OK, TaskDialogIcon.Stop);
                    return;
                }
                if (File.Exists(Settings.WowExe + "\\Wow-64.exe"))
                {
                    File.Delete(Settings.WowExe + "\\Wow-64.exe");
                    Log.Print("File Wow-64.exe was deleted", false);
                }
                Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = Settings.WowExe,
                    FileName = Settings.WowExe + "\\Wow.exe"
                });
                if (Settings.StartVentriloWithWow && !Process.GetProcessesByName("Ventrilo").Any())
                {
                    TileVentriloClick(null, EventArgs.Empty);
                }
            }
        }

        private void CmbboxAccSelectKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                cmbboxAccSelect.Visible = false;
                labelAccSelect.Visible = false;
                tileWowAutopass.Visible = true;
            }
        }

        private void TileWowAutopassClick(object sender, EventArgs e)
        {
            cmbboxAccSelect.Items.Clear();
            foreach (WowAccount i in wowAccounts)
            {
                cmbboxAccSelect.Items.Add(i.Login);
            }
            tileWowAutopass.Visible = false;
            labelAccSelect.Visible = true;
            cmbboxAccSelect.Visible = true;
            cmbboxAccSelect.Focus();
        }

        private void TileWowUpdaterClick(object sender, EventArgs e)
        {
            if (!File.Exists(Settings.WowExe + "\\World of Warcraft Launcher.exe"))
            {
                this.ShowTaskDialog("WoW client not found or corrupted", "Can't locate \"World of Warcraft Launcher.exe\"", TaskDialogButton.OK,
                                    TaskDialogIcon.Stop);
                return;
            }
            Process.Start(new ProcessStartInfo {
                WorkingDirectory = Settings.WowExe,
                FileName = Settings.WowExe + "\\World of Warcraft Launcher.exe"
            });
        }

        private void TileWowClick(object sender, EventArgs e)
        {
            if (!File.Exists(Settings.WowExe + "\\Wow.exe"))
            {
                this.ShowTaskDialog("WoW client not found or corrupted", "Can't locate \"Wow.exe\"", TaskDialogButton.OK, TaskDialogIcon.Stop);
                return;
            }
            if (File.Exists(Settings.WowExe + "\\Wow-64.exe"))
            {
                File.Delete(Settings.WowExe + "\\Wow-64.exe");
                Log.Print("File Wow-64.exe was deleted", false);
            }
            Process.Start(new ProcessStartInfo {
                WorkingDirectory = Settings.WowExe,
                FileName = Settings.WowExe + "\\Wow.exe"
            });
        }

        private void TileRadarClick(object sender, EventArgs e)
        {
            MetroButtonRadarClick(null, EventArgs.Empty);
        }

        private void TileLuaConsoleClick(object sender, EventArgs e)
        {
            MetroButtonLuaConsoleClick(null, EventArgs.Empty);
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
            Log.Print("Ventrilo process started", false);
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
            Log.Print("Raidcall process started", false);
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
            Log.Print("TS3 process started", false);
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
            Log.Print("Mumble process started", false);
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

        private CancellationTokenSource moduleToken;
        private NamedTask moduleTask;
        private bool moduleEnabled;

        private void FishingFunc()
        {
            int state = 0;
            string lure = "if (GetInventoryItemCooldown(\"player\", 1) == 0 and not GetWeaponEnchantInfo()) then " +
                          "if (IsEquippedItem(33820)) then UseItemByName(33820) elseif (IsEquippedItem(88710)) then UseItemByName(88710) end end";
            WowObject bobber = null;
            List<WowObject> wowObjects = new List<WowObject>();
            Log.Print(String.Format("{0}:{1} :: [Fishing] Plugin is started", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
            while (!moduleToken.IsCancellationRequested)
            {
                int startTime = Environment.TickCount;
                if (!WoW.Hooked || !WoW.WProc.IsInGame)
                {
                    Log.Print(String.Format("{0}:{1} :: [Fishing] Plugin is stopped: the player isn't active or not in the game", WoW.WProc.ProcessName,
                                            WoW.WProc.ProcessID), false);
                    BeginInvoke(new Action(() =>
                                           notifyIconMain.ShowBalloonTip(10000, "[Fishing] Plugin is stopped", "The player isn't active or not in the game",
                                                                         ToolTipIcon.Error)));
                    BeginInvoke(new Action(() => InvokeOnClick(toggleWowPlugins, EventArgs.Empty)));
                    moduleToken.Token.ThrowIfCancellationRequested();
                    return;
                }
                switch (state)
                {
                    case 0:
                        Log.Print(String.Format("{0}:{1} :: [Fishing] Cast fishing...", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
                        WoW.LuaDoString("if (not UnitAffectingCombat(\"player\")) then CastSpellByName(\"Рыбная ловля\") end");
                        Thread.Sleep(1500);
                        state = 1;
                        break;
                    case 1:
                        try
                        {
                            WoW.Pulse(wowObjects);
                        }
                        catch (Exception ex)
                        {
                            Log.Print(String.Format("{0}:{1} :: [Fishing] Pulse error: {2}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, ex.Message), true);
                            break;
                        }
                        if (WoW.LocalPlayer.ChannelSpellID == 0)
                        {
                            Log.Print(String.Format("{0}:{1} :: [Fishing] Player isn't fishing, recast...", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
                            state = 0;
                            break;
                        }
                        bobber = wowObjects.FirstOrDefault(i => i.OwnerGUID == WoW.LocalPlayer.GUID);
                        if (bobber != null)
                        {
                            if (bobber.Animation == 4456449)
                            {
                                Log.Print(String.Format("{0}:{1} :: [Fishing] Got bit!", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
                                Thread.Sleep(250);
                                state = 2;
                            }
                        }
                        break;
                    case 2:
                        if (bobber != null)
                        {
                            Log.Print(String.Format("{0}:{1} :: [Fishing] Interacting...", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
                            WoW.Interact(bobber.GUID);
                            bobber = null;
                            state = 3;
                        }
                        else
                        {
                            Log.Print(String.Format("{0}:{1} :: [Fishing] Bobber isn't found, recast...", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
                            state = 0;
                        }
                        break;
                    case 3:
                        if (WoW.WProc.PlayerIsLooting)
                        {
                            state = 4;
                            Log.Print(String.Format("{0}:{1} :: [Fishing] Looting...", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
                        }
                        break;
                    case 4:
                        if (!WoW.WProc.PlayerIsLooting)
                        {
                            Log.Print(String.Format("{0}:{1} :: [Fishing] Looted, applying lure...", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
                            WoW.LuaDoString(lure);
                            Thread.Sleep(500);
                            state = 0;
                        }
                        break;
                }
                int counter = 100 - Environment.TickCount + startTime;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (counter > 0 && !moduleToken.IsCancellationRequested)
                {
                    Thread.Sleep(counter);
                }
                //while (counter > 0 && fishingEnabled)
                //{
                //    Thread.Sleep(10);
                //    counter -= 10;
                //}
            }
            Log.Print(WoW.WProc != null
                          ? String.Format("{0}:{1} :: [Fishing] Plugin is stopped", WoW.WProc.ProcessName, WoW.WProc.ProcessID)
                          : "UNKNOWN:null :: [Fishing] Plugin is stopped", false);
            moduleToken.Token.ThrowIfCancellationRequested();
        }

        private void FlagReturnFunc()
        {
            uint searchingZone = WoW.WProc.PlayerZoneID;
            string[] searchingObjects;
            switch (searchingZone)
            {
                case 3277:
                case 5031:
                    searchingObjects = new[] {"Флаг Альянса", "Флаг Орды"};
                    break;
                case 3820:
                case 5799:
                    searchingObjects = new[] {"Флаг Пустоверти"};
                    break;
                case 6051:
                    searchingObjects = new[] {"Сфера могущества"};
                    break;
                case 6665:
                    searchingObjects = new[] {"Вагонетка Альянса", "Вагонетка Орды"};
                    break;
                case 3703:
                    searchingObjects = new[] {"Хранилище гильдии"};
                    break;
                default:
                    Log.Print(
                        String.Format("{0}:{1} :: [Battlefield outlaw] Unknown battlefield (zoneID: {2})", WoW.WProc.ProcessName, WoW.WProc.ProcessID,
                                      searchingZone), false);
                    BeginInvoke(new Action(() =>
                                           notifyIconMain.ShowBalloonTip(10000, "[Battlefield outlaw] Unknown battlefield",
                                                                         "I don't know what to do in this zone...", ToolTipIcon.Error)));
                    BeginInvoke(new Action(() => InvokeOnClick(toggleWowPlugins, EventArgs.Empty)));
                    moduleToken.Token.ThrowIfCancellationRequested();
                    return;
            }
            List<WowObject> wowObjects = new List<WowObject>();
            Log.Print(String.Format("{0}:{1} :: [Battlefield outlaw] Plugin is started, let's take away their {2} in {3} ({4})!",
                                    WoW.WProc.ProcessName, WoW.WProc.ProcessID, searchingObjects.AsString(), WoW.WProc.PlayerZoneText, searchingZone), false);
            while (!moduleToken.IsCancellationRequested)
            {
                int startTime = Environment.TickCount;
                if (!WoW.Hooked || !WoW.WProc.IsInGame)
                {
                    Log.Print(
                        String.Format("{0}:{1} :: [Battlefield outlaw] Plugin is stopped: the player isn't active or not in the game", WoW.WProc.ProcessName,
                                      WoW.WProc.ProcessID), false);
                    BeginInvoke(new Action(() =>
                                           notifyIconMain.ShowBalloonTip(10000, "[Battlefield outlaw] Plugin is stopped",
                                                                         "The player isn't active or not in the game", ToolTipIcon.Error)));
                    BeginInvoke(new Action(() => InvokeOnClick(toggleWowPlugins, EventArgs.Empty)));
                    moduleToken.Token.ThrowIfCancellationRequested();
                    return;
                }
                if (WoW.WProc.IsBattlegroundFinished != 0)
                {
                    Log.Print(
                        String.Format("{0}:{1} :: [Battlefield outlaw] Plugin is stopped: the battle has ended", WoW.WProc.ProcessName, WoW.WProc.ProcessID),
                        false);
                    BeginInvoke(
                        new Action(() => notifyIconMain.ShowBalloonTip(10000, "[Battlefield outlaw] Plugin is stopped", "The battle has ended", ToolTipIcon.Info)));
                    BeginInvoke(new Action(() => InvokeOnClick(toggleWowPlugins, EventArgs.Empty)));
                    moduleToken.Token.ThrowIfCancellationRequested();
                    return;
                }
                var zone = WoW.WProc.PlayerZoneID;
                if (zone != searchingZone)
                {
                    Log.Print(String.Format("{0}:{1} :: [Battlefield outlaw] Plugin is stopped: zone changed to {2} ({3})", WoW.WProc.ProcessName,
                                            WoW.WProc.ProcessID, WoW.WProc.PlayerZoneText, zone), false);
                    BeginInvoke(new Action(() =>
                                           notifyIconMain.ShowBalloonTip(10000, "[Battlefield outlaw] Plugin is stopped",
                                                                         String.Format("Zone changed to {0} ({1})", WoW.WProc.PlayerZoneText, zone),
                                                                         ToolTipIcon.Info)));
                    BeginInvoke(new Action(() => InvokeOnClick(toggleWowPlugins, EventArgs.Empty)));
                    moduleToken.Token.ThrowIfCancellationRequested();
                    return;
                }
                try
                {
                    WoW.Pulse(wowObjects);
                }
                catch (Exception ex)
                {
                    Log.Print(String.Format("{0}:{1} :: [Battlefield outlaw] Pulse error: {2}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, ex.Message), true);
                    continue;
                }
                foreach (var i in wowObjects.Where(l => searchingObjects.Contains(l.Name) && l.Location.Distance(WoW.LocalPlayer.Location) <= 10))
                {
                    WoW.Interact(i.GUID);
                    Log.Print(
                        String.Format("{0}:{1} :: [Battlefield outlaw] Interacting with {2} (0x{3:X})", WoW.WProc.ProcessName, WoW.WProc.ProcessID, i.Name, i.GUID),
                        false, false);
                }
                int counter = 50 - Environment.TickCount + startTime;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (counter > 0 && !moduleToken.IsCancellationRequested)
                {
                    Thread.Sleep(counter);
                }
            }
            Log.Print(WoW.WProc != null
                          ? String.Format("{0}:{1} :: [Battlefield outlaw] Plugin is stopped ({2})",
                                          WoW.WProc.ProcessName, WoW.WProc.ProcessID, searchingObjects.AsString())
                          : String.Format("UNKNOWN:null :: [Battlefield outlaw] Plugin is stopped ({0})", searchingObjects.AsString()), false);
            moduleToken.Token.ThrowIfCancellationRequested();
        }

        private void DestroyingFunc()
        {
            WoW.LuaDoString(
                "AxToolsHerbsIDs = {785, 2449, 2447, 765, 2450, 2453, 3820, 2452, 3369, 3356, 3357, 3355, 3819, 3818, 3821, 3358, 8836, 8839, 4625, 8846, 8831, 8838, 13463, 13464, 13465, 13466, 13467, 22786, 22785, 22793, 22791, 22792, 22787, 22789, 36907, 36903, 36906, 36904, 36905, 36901, 39970, 37921, 52983, 52987, 52984, 52986, 52985, 52988, 22790, 72235, 72234, 72237, 79010, 79011, 89639};" +
                "AxToolsDisenchantWeapon = \"Оружие\";" +
                "AxToolsDisenchantArmor = \"Доспехи\";" +
                "AxToolsOreIDs = {2770, 2771, 2772, 10620, 3858, 23424, 23425, 36909, 36912, 36910, 52185, 53038, 52183, 72093, 72094, 72103, 72092};");
            string mill =
                "if (IsSpellKnown(51005)) then for bag = 0, 4 do for bag_slot = 1, GetContainerNumSlots(bag) do local name, cCount = GetContainerItemInfo(bag, bag_slot); local id = GetContainerItemID(bag, bag_slot); if (name) then if (tContains(AxToolsHerbsIDs, id) and cCount >= 5) then CastSpellByID(51005); UseContainerItem(bag, bag_slot); return; end end end end end";
            string disenchant =
                "if (IsSpellKnown(13262)) then for bag = 0, 4 do for bag_slot = 1, GetContainerNumSlots(bag) do local itemLink = select(7, GetContainerItemInfo(bag, bag_slot)); if (itemLink) then local _, _ , cQuality, cLevel, _, cClass = GetItemInfo(itemLink); if ((cClass == AxToolsDisenchantWeapon or cClass == AxToolsDisenchantArmor) and cLevel > 1 and cQuality == 2) then CastSpellByID(13262); UseContainerItem(bag, bag_slot); return; end end end end end";
            string prospecting =
                "if (IsSpellKnown(31252)) then for bag = 0, 4 do for bag_slot = 1, GetContainerNumSlots(bag) do local name, cCount = GetContainerItemInfo(bag, bag_slot); local id = GetContainerItemID(bag, bag_slot); if (name) then if (tContains(AxToolsOreIDs, id) and cCount >= 5) then CastSpellByID(31252); UseContainerItem(bag, bag_slot); return; end end end end end";
            int state = 0;
            int skillStartTime = 0;
            Log.Print(String.Format("{0}:{1} :: [Goods destroyer] Plugin is started", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
            while (!moduleToken.IsCancellationRequested)
            {
                int startTime = Environment.TickCount;
                if (!WoW.Hooked || !WoW.WProc.IsInGame)
                {
                    Log.Print(
                        String.Format("{0}:{1} :: [Goods destroyer] Plugin is stopped: the player isn't active or not in the game", WoW.WProc.ProcessName,
                                      WoW.WProc.ProcessID), false);
                    BeginInvoke(new Action(() =>
                                           notifyIconMain.ShowBalloonTip(10000, "[Goods destroyer] Plugin is stopped",
                                                                         "The player isn't active or not in the game", ToolTipIcon.Error)));
                    BeginInvoke(new Action(() => InvokeOnClick(toggleWowPlugins, EventArgs.Empty)));
                    moduleToken.Token.ThrowIfCancellationRequested();
                    return;
                }
                switch (state)
                {
                    case 0:
                        WoW.LuaDoString(mill);
                        WoW.LuaDoString(prospecting);
                        Thread.Sleep(1000); // pause to prevent disenchanting unreal item 
                        WoW.LuaDoString(disenchant);
                        skillStartTime = Environment.TickCount;
                        state = 1;
                        break;
                    case 1:
                        //WoW.Pulse();
                        if (WoW.WProc.PlayerIsLooting)
                        {
                            state = 2;
                        }
                        else if (Environment.TickCount - skillStartTime > 5000)
                        {
                            state = 0;
                            Log.Print(String.Format("{0}:{1} :: [Goods destroyer] Smth went wrong or we haven't goods to destroy, restarting...",
                                                    WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
                        }
                        break;
                    case 2:
                        if (!WoW.WProc.PlayerIsLooting)
                        {
                            state = 0;
                        }
                        break;
                }
                int counter = 100 - Environment.TickCount + startTime;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (counter > 0 && !moduleToken.IsCancellationRequested)
                {
                    Thread.Sleep(counter);
                }
            }
            Log.Print(WoW.WProc != null
                          ? String.Format("{0}:{1} :: [Goods destroyer] Plugin is stopped", WoW.WProc.ProcessName, WoW.WProc.ProcessID)
                          : "UNKNOWN:null :: [Goods destroyer] Plugin is stopped", false);
            moduleToken.Token.ThrowIfCancellationRequested();
        }

        private void ToggleWowPluginsCheckedChanged(object sender, EventArgs e)
        {
            if (!(bool) toggleWowPlugins.Tag)
            {
                toggleWowPlugins.Tag = true;
                return;
            }
            toggleWowPlugins.Enabled = false;
            if (!moduleEnabled)
            {
                if (!WoW.Hooked && !LoadInjector())
                {
                    toggleWowPlugins.Tag = false;
                    toggleWowPlugins.Checked = false;
                    toggleWowPlugins.Enabled = true;
                    return;
                }
                if (!WoW.WProc.IsInGame)
                {
                    this.ShowTaskDialog("Module error", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop);
                    toggleWowPlugins.Tag = false;
                    toggleWowPlugins.Enabled = true;
                    toggleWowPlugins.Checked = false;
                    return;
                }
                switch (comboBoxWowPlugins.SelectedIndex)
                {
                    case 0:
                        moduleToken = new CancellationTokenSource();
                        moduleTask = new NamedTask(FishingFunc, moduleToken.Token, TaskCreationOptions.LongRunning, "Fishing", "Interface\\\\Icons\\\\trade_fishing");
                        moduleTask.Start();
                        moduleEnabled = true;
                        break;
                    case 1:
                        moduleToken = new CancellationTokenSource();
                        moduleTask = new NamedTask(FlagReturnFunc, moduleToken.Token, TaskCreationOptions.LongRunning, "BG Outlaw", "Interface\\\\Icons\\\\achievement_bg_winwsg");
                        moduleTask.Start();
                        moduleEnabled = true;
                        break;
                    case 2:
                        moduleToken = new CancellationTokenSource();
                        moduleTask = new NamedTask(DestroyingFunc, moduleToken.Token, TaskCreationOptions.LongRunning, "Goods Destroyer", "Interface\\\\Icons\\\\inv_misc_gem_bloodgem_01");
                        moduleTask.Start();
                        moduleEnabled = true;
                        break;
                    default:
                        this.ShowTaskDialog("Module error", "Please select module", TaskDialogButton.OK, TaskDialogIcon.Stop);
                        toggleWowPlugins.Tag = false;
                        toggleWowPlugins.Checked = false;
                        toggleWowPlugins.Enabled = true;
                        break;
                }
                if (moduleEnabled)
                {
                    comboBoxWowPlugins.Enabled = false;
                    foreach (ToolStripItem i in contextMenuStripMain.Items)
                    {
                        if (i.Text == "Fishing" || i.Text == "Capture flags/orbs on the battlefields" || i.Text == "Milling/disenchanting/prospecting")
                        {
                            i.Enabled = false;
                        }
                        else if (i.Text.Contains("Stop active plug-in"))
                        {
                            i.Enabled = true;
                        }
                    }
                    if (Settings.WowPluginsShowIngameNotifications)
                    {
                        WoW.ShowOverlayText("Plugin <" + moduleTask.Name + "> is started", moduleTask.WowIcon, Color.FromArgb(255, 102, 0));
                        //WoW.LuaDoString("UIErrorsFrame:AddMessage(\"Plugin <" + moduleTask.Name + "> is started\", 0.0, 1.0, 0.0)");
                    }
                }
            }
            else
            {
                moduleToken.Cancel();
                // ReSharper disable EmptyGeneralCatchClause
                try
                {
                    moduleTask.Wait(5000);
                }
                catch
                {
                    // Successful
                }
                // ReSharper restore EmptyGeneralCatchClause
                if (moduleTask.Status == TaskStatus.Canceled || moduleTask.Status == TaskStatus.RanToCompletion || moduleTask.Status == TaskStatus.Faulted)
                {
                    moduleEnabled = false;
                    comboBoxWowPlugins.Enabled = true;
                    foreach (ToolStripItem i in contextMenuStripMain.Items)
                    {
                        if (i.Text == "Fishing" || i.Text == "Capture flags/orbs on the battlefields" || i.Text == "Milling/disenchanting/prospecting")
                        {
                            i.Enabled = true;
                        }
                        else if (i.Text.Contains("Stop active plug-in"))
                        {
                            i.Enabled = false;
                        }
                    }
                    if (Settings.WowPluginsShowIngameNotifications && WoW.Hooked && WoW.WProc.IsInGame)
                    {
                        WoW.ShowOverlayText("Plugin <" + moduleTask.Name + "> is stopped", moduleTask.WowIcon, Color.FromArgb(255, 0, 0));
                        //WoW.LuaDoString("UIErrorsFrame:AddMessage(\"Plugin <" + moduleTask.Name + "> is stopped\", 1.0, 0.4, 0.0)");
                    }
                    moduleTask.Dispose();
                    moduleToken.Dispose();
                    GC.Collect();
                }
                else
                {
                    Log.Print("Module task failed to cancel", true);
                    this.ShowTaskDialog("Module error", "Module task failed to cancel", TaskDialogButton.OK, TaskDialogIcon.Stop);
                    toggleWowPlugins.Tag = false;
                    toggleWowPlugins.Checked = true;
                }
            }
            toggleWowPlugins.Enabled = true;
        }

        private void metroCheckBoxPluginShowIngameNotification_CheckedChanged(object sender, EventArgs e)
        {
            Settings.WowPluginsShowIngameNotifications = metroCheckBoxPluginShowIngameNotification.Checked;
        }

        private void ComboBoxWowPluginsSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxWowPlugins.SelectedIndex)
            {
                case 0: // Fishing
                    metroToolTip1.SetToolTip(metroLabelSelectPlugin, "This is a very simple fish bot.\r\n" +
                                                                 "It supports Nat's Hat and\r\n" +
                                                                 "Weather-Beaten Fishing Hat if equipped");
                    metroLabelSelectPlugin.Text = "Select a plugin...and hover o'er me to see plugin description";
                    break;
                case 1: // Capture flags/orbs on the battlefields
                    metroToolTip1.SetToolTip(metroLabelSelectPlugin, "This plugin will automatically return or pickup flags in\r\n" +
                                                                 "Warsong Gulch, Twin Peaks and EotS,\r\n" +
                                                                 "also it will pickup orbs in ToK");
                    metroLabelSelectPlugin.Text = "Select a plugin...and hover o'er me to see plugin description";
                    break;
                case 2: // Milling/disenchanting/prospecting
                    metroToolTip1.SetToolTip(metroLabelSelectPlugin, "This plugin will mill/prospect any herbs/ores\r\n" +
                                                                 "and disenchant greens in your bags");
                    metroLabelSelectPlugin.Text = "Select a plugin...and hover o'er me to see plugin description";
                    break;
                default:
                    metroToolTip1.SetToolTip(metroLabelSelectPlugin, string.Empty);
                    metroLabelSelectPlugin.Text = "Select a plugin...";
                    break;
            }
        }

        private void MetroButtonBlackMarketTrackerClick(object sender, EventArgs e)
        {
            if (!WoW.Hooked && !LoadInjector())
            {
                new TaskDialog("Module error", "AxTools", "Can't inject", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            if (!WoW.WProc.IsInGame)
            {
                new TaskDialog("Module error", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
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
                Log.Print(String.Format("{0}:{1} :: Injector unloaded", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
            }
            else
            {
                Log.Print("Injector error: WoW injector isn't active", true);
                this.ShowTaskDialog("Injector error", "WoW injector isn't active", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private bool LoadInjector()
        {
            int index;
            if (WowProcesses.Count == 1)
            {
                index = 0;
            }
            else
            {
                ProcessSelection.SelectProcess(WowProcesses, out index);
            }
            if (index != -1)
            {
                if (WowProcesses[index].IsValidBuild)
                {
                    if (WowProcesses[index].IsInGame)
                    {
                        switch (WoW.Hook(WowProcesses[index]))
                        {
                            case HookResult.Successful:
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Injector loaded", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
                                return true;
                            case HookResult.IncorrectDirectXVersion:
                                this.ShowTaskDialog("Injecting error", "Incorrect DirectX version", TaskDialogButton.OK, TaskDialogIcon.SecurityError);
                                return false;
                        }
                    }
                    Log.Print("[WoW hook] Injecting error: Player isn't logged in", false);
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
            BlackMarket pBlackMarket = Utils.FindForm<BlackMarket>();
            if (pBlackMarket != null) pBlackMarket.Close();
            WoW.Unhook();
        }

        #endregion

        #region WowAccountsTab

        private void MetroButtonWowAccountSaveUpdateClick(object sender, EventArgs e)
        {
            WowAccount wowAccount = wowAccounts.FirstOrDefault(i => i.Login == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                wowAccount.Password = textBoxWowAccountPassword.Text;
            }
            else
            {
                wowAccounts.Add(new WowAccount(textBoxWowAccountLogin.Text,textBoxWowAccountPassword.Text));
            }
            comboBoxWowAccounts.Items.Clear();
            foreach (WowAccount i in wowAccounts)
            {
                comboBoxWowAccounts.Items.Add(i.Login);
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }

        private void MetroButtonWowAccountDeleteClick(object sender, EventArgs e)
        {
            WowAccount wowAccount = wowAccounts.FirstOrDefault(i => i.Login == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                wowAccounts.Remove(wowAccount);
            }
            comboBoxWowAccounts.Items.Clear();
            foreach (WowAccount i in wowAccounts)
            {
                comboBoxWowAccounts.Items.Add(i.Login);
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }

        private void TextBoxWowAccountLoginTextChanged(object sender, EventArgs e)
        {
            if (wowAccounts.Any(i => i.Login == textBoxWowAccountLogin.Text))
            {
                metroButtonWowAccountSaveUpdate.Text = "Update";
                metroButtonWowAccountDelete.Enabled = true;
            }
            else
            {
                metroButtonWowAccountSaveUpdate.Text = "Add";
                metroButtonWowAccountDelete.Enabled = false;
            }
            metroButtonWowAccountSaveUpdate.Enabled = textBoxWowAccountLogin.Text.Contains('@') && textBoxWowAccountLogin.Text.Contains('.') && textBoxWowAccountPassword.Text.Trim().Length != 0;
        }

        private void ComboBoxWowAccountsSelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxWowAccountLogin.Text = wowAccounts[comboBoxWowAccounts.SelectedIndex].Login;
        }

        #endregion

        #region MiscTab

        private void TileBackupAddonsClick(object sender, EventArgs e)
        {
            WaitingOverlay waitingOverlay = new WaitingOverlay(this);
            waitingOverlay.Show();
            Task.Factory.StartNew(BackupAddons, false).ContinueWith(l => Invoke(new Action(waitingOverlay.Close)));
        }

        private void TileOpenBackupsFolderClick(object sender, EventArgs e)
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

        private void TileShowLogClick(object sender, EventArgs e)
        {
            if (File.Exists(Globals.LogFileName))
            {
                try
                {
                    Process.Start(Globals.LogFileName);
                }
                catch (Exception ex)
                {
                    this.ShowTaskDialog("Cannot open log file", ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
                }
            }
            else
            {
                this.ShowTaskDialog("Cannot open log file", "It doesn't exist", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private void metroTileSendLogToDev_Click(object sender, EventArgs e)
        {
            try
            {
                string subject;
                InputBox.Input("Any comment? (optional)", out subject);
                WaitingOverlay waitingOverlay = new WaitingOverlay(this);
                waitingOverlay.Show();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Log.SendViaEmail(subject);
                    }
                    catch (Exception ex)
                    {
                        Log.Print("Can't send log: " + ex.Message, true);
                        this.ShowTaskDialog("Can't send log", ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
                    }
                    finally
                    {
                        Invoke(new Action(waitingOverlay.Close));
                    }
                });
            }
            catch (Exception ex)
            {
                this.ShowTaskDialog("Log file sending error", ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        #endregion

    }
}
