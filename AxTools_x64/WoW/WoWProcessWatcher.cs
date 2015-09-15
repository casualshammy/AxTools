using AxTools.Forms;
using AxTools.Helpers;
using AxTools.Helpers.MemoryManagement;
using AxTools.WinAPI;
using AxTools.WoW.Management;
using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AxTools.WoW
{
    internal static class WoWProcessWatcher
    {
        private static ManagementEventWatcher _wowWatcherStart;
        private static ManagementEventWatcher _wowWatcherStop;
        private static readonly object _lock = new object();

        internal static void StartWatcher()
        {
            lock (_lock)
            {
                _wowWatcherStart = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
                _wowWatcherStop = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
                _wowWatcherStart.EventArrived += WowProcessStarted;
                _wowWatcherStop.EventArrived += WowProcessStopped;
                _wowWatcherStart.Start();
                _wowWatcherStop.Start();
                GetWoWProcesses();
            }
        }

        internal static void StopWatcher()
        {
            lock (_lock)
            {
                if (_wowWatcherStart != null && _wowWatcherStop != null)
                {
                    _wowWatcherStart.EventArrived -= WowProcessStarted;
                    _wowWatcherStop.EventArrived -= WowProcessStopped;
                    _wowWatcherStop.Stop();
                    _wowWatcherStart.Stop();
                    _wowWatcherStop.Dispose();
                    _wowWatcherStart.Dispose();
                }
                else
                {
                    throw new Exception("[WoWProcessWatcher] Stop(): watcher isn't started");
                }
            }
        }

        private static void GetWoWProcesses()
        {
            foreach (Process i in Process.GetProcesses())
            {
                switch (i.ProcessName.ToLower())
                {
                    case "wow":
                        MainForm.Instance.ShowNotifyIconMessage("Unsupported WoW version", "AxTools doesn't support x86 versions of WoW client", ToolTipIcon.Warning);
                        break;
                    case "wow-64":
                        WowProcess process = new WowProcess(i.Id);
                        WowProcess.List.Add(process);
                        Log.Info(string.Format("{0} [Process watcher] Process added", process));
                        Task.Factory.StartNew(OnWowProcessStartup, process);
                        break;
                }
            }
        }

        private static void WowProcessStarted(object sender, EventArrivedEventArgs e)
        {
            try
            {
                int processId = Convert.ToInt32(e.NewEvent["ProcessID"]);
                string processName = e.NewEvent["ProcessName"].ToString().ToLower();
                if (processName == "wow.exe")
                {
                    MainForm.Instance.ShowNotifyIconMessage("Unsupported WoW version", "AxTools doesn't support x86 versions of WoW client", ToolTipIcon.Error);
                    Log.Error(string.Format("[{0}:{1}] [Process watcher] 32bit WoW processes aren't supported", processName, processId));
                }
                else if (processName == "wow-64.exe")
                {
                    WowProcess wowProcess = new WowProcess(processId);
                    WowProcess.List.Add(wowProcess);
                    Log.Info(string.Format("{0} [Process watcher] Process started, {1} total", wowProcess, WowProcess.List.Count));
                    Task.Factory.StartNew(OnWowProcessStartup, wowProcess);
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("[{0}:{1}] [Process watcher] Process started with error: {2}", e.NewEvent["ProcessName"], e.NewEvent["ProcessID"], ex.Message));
            }
            finally
            {
                e.NewEvent.Dispose();
            }
        }

        private static void WowProcessStopped(object sender, EventArrivedEventArgs e)
        {
            try
            {
                if (e.NewEvent["ProcessName"].ToString().ToLower() == "wow-64.exe")
                {
                    int pid = Convert.ToInt32(e.NewEvent["ProcessID"]);
                    string name = e.NewEvent["ProcessName"].ToString().Substring(0, e.NewEvent["ProcessName"].ToString().Length - 4);
                    WowProcess pWowProcess = WowProcess.List.FirstOrDefault(x => x.ProcessID == pid);
                    if (pWowProcess != null)
                    {
                        if (WoWManager.Hooked && WoWManager.WoWProcess.ProcessID == pWowProcess.ProcessID)
                        {
                            WoWManager.Unhook();
                            Log.Info(string.Format("{0} [WoW hook] Injector unloaded", pWowProcess));
                        }
                        pWowProcess.Dispose();
                        Log.Info(string.Format("[{0}:{1}] [WoW hook] Memory manager disposed", name, pid));
                        if (WowProcess.List.Remove(pWowProcess))
                        {
                            Log.Info(string.Format("[{0}:{1}] [Process watcher] Process closed, {2} total", name, pid, WowProcess.List.Count));
                        }
                        else
                        {
                            Log.Error(string.Format("[{0}:{1}] [Process watcher] Can't delete WowProcess instance", name, pid));
                        }
                    }
                    else
                    {
                        Log.Error(string.Format("[{0}:{1}] [Process watcher] Closed WoW process not found", name, pid));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("[{0}:{1}] [Process watcher] Process stopped with error: {2}", e.NewEvent["ProcessName"], e.NewEvent["ProcessID"], ex.Message));
            }
            finally
            {
                e.NewEvent.Dispose();
            }
        }

        private static void OnWowProcessStartup(object wowProcess)
        {
            try
            {
                WowProcess process = (WowProcess)wowProcess;
                Log.Info(string.Format("{0} [WoW hook] Attaching...", process));
                for (int i = 0; i < 600; i++)
                {
                    Thread.Sleep(100);
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        if (Settings.Instance.WoWCustomizeWindow)
                        {
                            try
                            {
                                if (Settings.Instance.WoWCustomWindowNoBorder)
                                {
                                    long styleWow = NativeMethods.GetWindowLong64(process.MainWindowHandle, NativeMethods.GWL_STYLE) & ~(NativeMethods.WS_CAPTION | NativeMethods.WS_THICKFRAME);
                                    NativeMethods.SetWindowLong64(process.MainWindowHandle, NativeMethods.GWL_STYLE, styleWow);
                                }
                                NativeMethods.MoveWindow(process.MainWindowHandle, Settings.Instance.WoWCustomWindowRectangle.X, Settings.Instance.WoWCustomWindowRectangle.Y,
                                    Settings.Instance.WoWCustomWindowRectangle.Width, Settings.Instance.WoWCustomWindowRectangle.Height, false);
                                Log.Info(string.Format("{0} [WoW hook] Window style is changed", process));
                            }
                            catch (Exception ex)
                            {
                                Log.Error(string.Format("{0} [WoW hook] Window changing failed with error: {1}", process, ex.Message));
                            }
                        }
                        try
                        {
                            process.Memory = new MemoryManager(Process.GetProcessById(process.ProcessID));
                            Log.Info(string.Format("{0} [WoW hook] Memory manager initialized, base address 0x{1:X}", process, process.Memory.ImageBase.ToInt64()));
                            if (!process.IsValidBuild)
                            {
                                Log.Info(string.Format("{0} [WoW hook] Memory manager: invalid WoW executable", process));
                                MainForm.Instance.ShowNotifyIconMessage("Incorrect WoW version", "Injector is locked, please wait for update", ToolTipIcon.Warning);
                                Utils.PlaySystemNotificationAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("{0} [WoW hook] Memory manager initialization failed with error: {1}", process, ex.Message));
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("MainForm.AttachToWow: general error: " + ex.Message);
            }
        }
        
    }
}
