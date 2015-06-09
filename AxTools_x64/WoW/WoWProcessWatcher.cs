using AxTools.Classes;
using AxTools.Forms;
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
                        WowProcess.GetAllWoWProcesses().Add(process);
                        Log.Print(String.Format("{0}:{1} :: [Process watcher] Process added", i.ProcessName, i.Id));
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
                    Log.Print(String.Format("{0}:{1} :: [Process watcher] 32bit WoW processes aren't supported", processName, processId), true);
                }
                else if (processName == "wow-64.exe")
                {
                    WowProcess wowProcess = new WowProcess(processId);
                    WowProcess.GetAllWoWProcesses().Add(wowProcess);
                    Log.Print(String.Format("{0}:{1} :: [Process watcher] Process started, {2} total", wowProcess.ProcessName, wowProcess.ProcessID, WowProcess.GetAllWoWProcesses().Count));
                    Task.Factory.StartNew(OnWowProcessStartup, wowProcess);
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

        private static void WowProcessStopped(object sender, EventArrivedEventArgs e)
        {
            try
            {
                if (e.NewEvent["ProcessName"].ToString().ToLower() == "wow-64.exe")
                {
                    int pid = Convert.ToInt32(e.NewEvent["ProcessID"]);
                    string name = e.NewEvent["ProcessName"].ToString().Substring(0, e.NewEvent["ProcessName"].ToString().Length - 4);
                    WowProcess pWowProcess = WowProcess.GetAllWoWProcesses().FirstOrDefault(x => x.ProcessID == pid);
                    if (pWowProcess != null)
                    {
                        if (WoWManager.Hooked && WoWManager.WoWProcess.ProcessID == pWowProcess.ProcessID)
                        {
                            WoWManager.Unhook();
                            Log.Print(String.Format("{0}:{1} :: [WoW hook] Injector unloaded", name, pid));
                        }
                        pWowProcess.Dispose();
                        Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager disposed", name, pid));
                        if (WowProcess.GetAllWoWProcesses().Remove(pWowProcess))
                        {
                            Log.Print(String.Format("{0}:{1} :: [Process watcher] Process closed, {2} total", name, pid, WowProcess.GetAllWoWProcesses().Count));
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

        private static void OnWowProcessStartup(object wowProcess)
        {
            try
            {
                WowProcess process = (WowProcess)wowProcess;
                Log.Print(String.Format("{0}:{1} :: [WoW hook] Attaching...", process.ProcessName, process.ProcessID));
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
                                    int styleWow = NativeMethods.GetWindowLong(process.MainWindowHandle, NativeMethods.GWL_STYLE) & ~(NativeMethods.WS_CAPTION | NativeMethods.WS_THICKFRAME);
                                    NativeMethods.SetWindowLong(process.MainWindowHandle, NativeMethods.GWL_STYLE, styleWow);
                                }
                                NativeMethods.MoveWindow(process.MainWindowHandle, Settings.Instance.WoWCustomWindowRectangle.X, Settings.Instance.WoWCustomWindowRectangle.Y,
                                    Settings.Instance.WoWCustomWindowRectangle.Width, Settings.Instance.WoWCustomWindowRectangle.Height, false);
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Window style is changed", process.ProcessName, process.ProcessID));
                            }
                            catch (Exception ex)
                            {
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Window changing failed with error: {2}", process.ProcessName, process.ProcessID, ex.Message), true);
                            }
                        }
                        try
                        {
                            process.Memory = new MemoryManager(Process.GetProcessById(process.ProcessID));
                            Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager initialized, base address 0x{2:X}", process.ProcessName, process.ProcessID, process.Memory.ImageBase.ToInt64()));
                            if (!process.IsValidBuild)
                            {
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager: invalid WoW executable", process.ProcessName, process.ProcessID), true);
                                MainForm.Instance.ShowNotifyIconMessage("Incorrect WoW version", "Injector is locked, please wait for update", ToolTipIcon.Warning);
                                Utils.PlaySystemNotificationAsync();
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
        
    }
}
