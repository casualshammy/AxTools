using AxTools.Helpers;
using AxTools.WinAPI;
using FMemory;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AxTools.WoW
{
    internal static class WoWProcessManager
    {
        /// <summary>
        ///     List of all <see cref="WowProcess"/> running on computer
        /// </summary>
        internal static ConcurrentDictionary<int, WowProcess> Processes = new ConcurrentDictionary<int, WowProcess>();

        internal static event Action WoWProcessStartedOrClosed;

        internal static event Action<int> WoWProcessClosed;

        internal static event Action<WowProcess> WoWProcessReadyForInteraction;

        private static readonly object _listLock = new object();
        
        private static readonly object _lock = new object();

        private static readonly Log2 log = new Log2("WoWProcessManager");

        internal static void StartWatcher()
        {
            lock (_lock)
            {
                ProcessWatcherWMI.ProcessStarted += ProcessWatcher_ProcessStarted;
                ProcessWatcherWMI.ProcessExited += ProcessWatcher_ProcessExited;
                GetWoWProcesses();
                Program.Exit += Program_Exit;
            }
        }

        private static void Program_Exit()
        {
            foreach (var i in Processes)
            {
                string name = i.Value.ProcessName;
                i.Value.Dispose();
                log.Info($"{name}:{i.Key} :: [WoW hook] Memory manager disposed");
            }
        }

        private static void ProcessWatcher_ProcessExited(ProcessInfo obj)
        {
            try
            {
                if (obj.ProcessName.ToLower() == "wow.exe")
                {
                    if (Processes.TryRemove(obj.ProcessID, out WowProcess pWowProcess))
                    {
                        pWowProcess.Dispose();
                        log.Info($"{pWowProcess} Process closed, {Processes.Count} total");
                        WoWProcessStartedOrClosed?.Invoke();
                        WoWProcessClosed?.Invoke(obj.ProcessID);
                    }
                    else
                    {
                        string name = obj.ProcessName.Substring(0, obj.ProcessName.Length - 4);
                        log.Error($"[{name}:{obj.ProcessID}] Closed WoW process not found");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"[{obj.ProcessName}:{obj.ProcessID}] Process stopped with error: {ex.Message}");
            }
        }

        private static void ProcessWatcher_ProcessStarted(ProcessInfo obj)
        {
            try
            {
                string processName = obj.ProcessName.ToLower();
                if (processName == "wow.exe")
                {
                    WowProcess wowProcess = new WowProcess(obj.ProcessID);
                    Processes.TryAdd(obj.ProcessID, wowProcess);
                    log.Info($"{wowProcess} Process started, {Processes.Count} total");
                    WoWProcessStartedOrClosed?.Invoke();
                    Task.Factory.StartNew(OnWowProcessStartup, wowProcess);
                }
            }
            catch (Exception ex)
            {
                log.Error($"[{obj.ProcessName}:{obj.ProcessID}] Process started with error: {ex.Message}");
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
                        WowProcess process = new WowProcess(i.Id);
                        Processes.TryAdd(i.Id, process);
                        log.Info($"{process} Process added");
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
                log.Info($"{process} Attaching...");
                for (int i = 0; i < 600; i++)
                {
                    Thread.Sleep(100);
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        if (Settings2.Instance.WoWCustomizeWindow)
                        {
                            Task.Run(() =>
                            {
                                Thread.Sleep(1000); // because sometimes pause is needed
                                try
                                {
                                    if (Settings2.Instance.WoWCustomWindowNoBorder)
                                    {
                                        var styleWow = NativeMethods.GetWindowLong64(process.MainWindowHandle, Win32Consts.GWL_STYLE) & ~(Win32Consts.WS_CAPTION | Win32Consts.WS_THICKFRAME);
                                        NativeMethods.SetWindowLong64(process.MainWindowHandle, Win32Consts.GWL_STYLE, styleWow);
                                    }
                                    NativeMethods.MoveWindow(process.MainWindowHandle, Settings2.Instance.WoWCustomWindowRectangle.X, Settings2.Instance.WoWCustomWindowRectangle.Y,
                                        Settings2.Instance.WoWCustomWindowRectangle.Width, Settings2.Instance.WoWCustomWindowRectangle.Height, false);
                                    log.Info($"{process} Window style is changed");
                                }
                                catch (Exception ex)
                                {
                                    log.Error($"{process} Window changing failed with error: {ex.Message}");
                                }
                            });
                        }
                        try
                        {
                            process.Memory = new MemoryManager(Process.GetProcessById(process.ProcessID));
                            log.Info($"{process} Memory manager initialized, base address 0x{process.Memory.ImageBase.ToInt64():X}");
                            if (!process.IsValidBuild)
                            {
                                if (process.GetExecutableRevision() > WowBuildInfoX64.WoWRevision)
                                    log.Error($"AxTools is outdated! WowBuildInfoX64.WoWRevision: {WowBuildInfoX64.WoWRevision}; actual revision: {process.GetExecutableRevision()}");
                                log.Info($"{process} Memory manager: invalid WoW executable");
                                Notify.TrayPopup("Incorrect WoW version", "Injector is locked, please wait for update", NotifyUserType.Warn, true);
                            }
                            WoWProcessReadyForInteraction?.Invoke(process);
                        }
                        catch (Exception ex)
                        {
                            log.Error($"{process} Memory manager initialization failed with error: {ex.Message}");
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("General error: " + ex.Message);
            }
        }
    }
}