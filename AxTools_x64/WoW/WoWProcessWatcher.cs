using AxTools.Helpers;
using AxTools.WinAPI;
using FMemory;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AxTools.WoW
{
    internal static class WoWProcessManager
    {
        /// <summary>
        ///     List of all <see cref="WowProcess"/> running on computer
        /// </summary>
        internal static readonly ConcurrentDictionary<int, WowProcess> Processes = new ConcurrentDictionary<int, WowProcess>();

        internal static event Action WoWProcessStartedOrClosed;

        internal static event Action<int> WoWProcessClosed;

        internal static event Action<WowProcess> WoWProcessReadyForInteraction;
        
        private static readonly Log2 log = new Log2(nameof(WoWProcessManager));

        internal static void StartWatcher()
        {
            ProcessManager.ProcessStarted += ProcessWatcher_ProcessStarted;
            ProcessManager.ProcessStopped += ProcessWatcher_ProcessExited;
            GetWoWProcesses();
            Program.Exit += Program_Exit;
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

        private static void ProcessWatcher_ProcessExited(ProcessManagerEvent obj)
        {
            try
            {
                if (obj.ProcessName.ToLower() == "wow.exe")
                {
                    if (Processes.TryRemove(obj.ProcessId, out WowProcess pWowProcess))
                    {
                        pWowProcess.Dispose();
                        log.Info($"{pWowProcess} Process closed, {Processes.Count} total");
                        OnWowProcessStopped();
                        WoWProcessStartedOrClosed?.Invoke();
                        WoWProcessClosed?.Invoke(obj.ProcessId);
                    }
                    else
                    {
                        string name = obj.ProcessName.Substring(0, obj.ProcessName.Length - 4);
                        log.Error($"[{name}:{obj.ProcessId}] Closed WoW process not found");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"[{obj.ProcessName}:{obj.ProcessId}] Process stopped with error: {ex.Message}");
            }
        }

        private static void ProcessWatcher_ProcessStarted(ProcessManagerEvent obj)
        {
            try
            {
                string processName = obj.ProcessName.ToLower();
                if (processName == "wow.exe")
                {
                    WowProcess wowProcess = new WowProcess(obj.ProcessId);
                    Processes.TryAdd(obj.ProcessId, wowProcess);
                    log.Info($"{wowProcess} Process started, {Processes.Count} total");
                    WoWProcessStartedOrClosed?.Invoke();
                    Task.Factory.StartNew(OnWowProcessStartup, wowProcess);
                }
            }
            catch (Exception ex)
            {
                log.Error($"[{obj.ProcessName}:{obj.ProcessId}] Process started with error: {ex.Message}");
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
                            Utils.SetProcessPrioritiesToNormal(process.ProcessID); // in case we'are starting from Task Scheduler priorities can be lower than normal
                            process.Memory = new MemoryManager(Process.GetProcessById(process.ProcessID));
                            log.Info($"{process} Memory manager initialized, base address 0x{process.Memory.ImageBase.ToInt64():X}");
                            if (!process.IsValidBuild)
                            {
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

        private static void OnWowProcessStopped()
        {
            if (Settings2.Instance.WoWClearCache && Processes.Count == 0)
            {
                string cachePath = Path.Combine(Settings2.Instance.WoWDirectory, "Cache");
                if (Directory.Exists(cachePath))
                {
                    log.Info($"Deleting WoW cache --> {cachePath}");
                    try
                    {
                        Directory.Delete(cachePath, true);
                        log.Info("WoW cache is deleted");
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Exception is thrown while trying to delete WoW cache --> {ex.Message}");
                    }
                }
                else
                {
                    log.Info($"WoW cache is not found --> {cachePath}");
                }
            }
        }

    }
}