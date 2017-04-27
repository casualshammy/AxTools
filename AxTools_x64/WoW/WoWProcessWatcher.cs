using AxTools.Helpers;
using AxTools.Helpers.MemoryManagement;
using AxTools.WinAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AxTools.WoW
{
    internal static class WoWProcessManager
    {
        /// <summary>
        ///     List of all <see cref="WowProcess"/> running on computer
        /// </summary>
        internal static List<WowProcess> List
        {
            get
            {
                lock (_listLock)
                {
                    return _sharedList;
                }
            }
        }

        private static readonly object _listLock = new object();
        private static readonly List<WowProcess> _sharedList = new List<WowProcess>();
        private static readonly object _lock = new object();

        internal static void StartWatcher()
        {
            lock (_lock)
            {
                ProcessWatcherWMI.ProcessStarted += ProcessWatcher_ProcessStarted;
                ProcessWatcherWMI.ProcessExited += ProcessWatcher_ProcessExited;
                GetWoWProcesses();
            }
        }

        private static void ProcessWatcher_ProcessExited(ProcessInfo obj)
        {
            try
            {
                if (obj.ProcessName.ToLower() == "wow-64.exe")
                {
                    WowProcess pWowProcess = List.FirstOrDefault(x => x.ProcessID == obj.ProcessID);
                    if (pWowProcess != null)
                    {
                        if (WoWManager.Hooked && WoWManager.WoWProcess.ProcessID == pWowProcess.ProcessID)
                        {
                            WoWManager.Unhook();
                        }
                        pWowProcess.Dispose();
                        Log.Info(string.Format("{0} [WoW hook] Memory manager disposed", pWowProcess));
                        if (List.Remove(pWowProcess))
                        {
                            Log.Info(string.Format("{0} [Process watcher] Process closed, {1} total", pWowProcess, List.Count));
                        }
                        else
                        {
                            Log.Error(string.Format("{0} [Process watcher] Can't delete WowProcess instance", pWowProcess));
                        }
                    }
                    else
                    {
                        string name = obj.ProcessName.Substring(0, obj.ProcessName.Length - 4);
                        Log.Error(string.Format("[{0}:{1}] [Process watcher] Closed WoW process not found", name, obj.ProcessID));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("[{0}:{1}] [Process watcher] Process stopped with error: {2}", obj.ProcessName, obj.ProcessID, ex.Message));
            }
        }

        private static void ProcessWatcher_ProcessStarted(ProcessInfo obj)
        {
            try
            {
                string processName = obj.ProcessName.ToLower();
                if (processName == "wow.exe")
                {
                    Notify.TrayPopup("Unsupported WoW version", "AxTools doesn't support x86 versions of WoW client", NotifyUserType.Warn, true);
                    Log.Error(string.Format("[{0}:{1}] [Process watcher] 32bit WoW processes aren't supported", processName, obj.ProcessID));
                }
                else if (processName == "wow-64.exe")
                {
                    WowProcess wowProcess = new WowProcess(obj.ProcessID);
                    List.Add(wowProcess);
                    Log.Info(string.Format("{0} [Process watcher] Process started, {1} total", wowProcess, List.Count));
                    Task.Factory.StartNew(OnWowProcessStartup, wowProcess);
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("[{0}:{1}] [Process watcher] Process started with error: {2}", obj.ProcessName, obj.ProcessID, ex.Message));
            }
        }

        internal static void StopWatcher()
        {
            lock (_lock)
            {
                ProcessWatcherWMI.ProcessStarted -= ProcessWatcher_ProcessStarted;
                ProcessWatcherWMI.ProcessExited -= ProcessWatcher_ProcessExited;
            }
        }

        private static void GetWoWProcesses()
        {
            foreach (Process i in Process.GetProcesses())
            {
                switch (i.ProcessName.ToLower())
                {
                    case "wow":
                        Notify.TrayPopup("Unsupported WoW version", "AxTools doesn't support x86 versions of WoW client", NotifyUserType.Warn, true);
                        break;
                    case "wow-64":
                        WowProcess process = new WowProcess(i.Id);
                        List.Add(process);
                        Log.Info(string.Format("{0} [Process watcher] Process added", process));
                        Task.Factory.StartNew(OnWowProcessStartup, process);
                        break;
                }
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
                            Task.Run(() => {
                                Thread.Sleep(1000); // because sometimes pause is needed
                                try
                                {
                                    if (Settings.Instance.WoWCustomWindowNoBorder)
                                    {
                                        long styleWow = NativeMethods.GetWindowLong64(process.MainWindowHandle, Win32Consts.GWL_STYLE) & ~(Win32Consts.WS_CAPTION | Win32Consts.WS_THICKFRAME);
                                        NativeMethods.SetWindowLong64(process.MainWindowHandle, Win32Consts.GWL_STYLE, styleWow);
                                    }
                                    NativeMethods.MoveWindow(process.MainWindowHandle, Settings.Instance.WoWCustomWindowRectangle.X, Settings.Instance.WoWCustomWindowRectangle.Y,
                                        Settings.Instance.WoWCustomWindowRectangle.Width, Settings.Instance.WoWCustomWindowRectangle.Height, false);
                                    Log.Info(string.Format("{0} [WoW hook] Window style is changed", process));
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(string.Format("{0} [WoW hook] Window changing failed with error: {1}", process, ex.Message));
                                }
                            });
                        }
                        try
                        {
                            process.Memory = new MemoryManager(Process.GetProcessById(process.ProcessID));
                            Log.Info(string.Format("{0} [WoW hook] Memory manager initialized, base address 0x{1:X}", process, process.Memory.ImageBase.ToInt64()));
                            if (!process.IsValidBuild)
                            {
                                Log.Info(string.Format("{0} [WoW hook] Memory manager: invalid WoW executable", process));
                                Notify.TrayPopup("Incorrect WoW version", "Injector is locked, please wait for update", NotifyUserType.Warn, true);
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
