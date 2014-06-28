using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.Classes;
using AxTools.Forms;
using AxTools.WinAPI;
using AxTools.WoW.Management;
using GreyMagic;

namespace AxTools.WoW
{
    internal static class WoWProcessWatcher
    {
        private static ManagementEventWatcher _wowWatcherStart;
        private static ManagementEventWatcher _wowWatcherStop;
        private static readonly object Lock = new object();

        internal static void Start()
        {
            lock (Lock)
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

        internal static void Stop()
        {
            lock (Lock)
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
                    case "wow-64":
                        MainForm.Instance.ShowNotifyIconMessage("Unsupported WoW version", "AxTools doesn't support x64 versions of WoW client", ToolTipIcon.Warning);
                        break;
                    case "wow":
                        WowProcess process = new WowProcess(i.Id);
                        WowProcess.GetAllWowProcesses().Add(process);
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
                switch (processName)
                {
                    case "wow-64.exe":
                        MainForm.Instance.ShowNotifyIconMessage("Unsupported WoW version", "AxTools doesn't support x64 versions of WoW client", ToolTipIcon.Error);
                        Log.Print(String.Format("{0}:{1} :: [Process watcher] 64bit WoW processes aren't supported", processName, processId), true);
                        break;
                    case "wow.exe":
                        WowProcess wowProcess = new WowProcess(processId);
                        WowProcess.GetAllWowProcesses().Add(wowProcess);
                        Log.Print(String.Format("{0}:{1} :: [Process watcher] Process started, {2} total", wowProcess.ProcessName, wowProcess.ProcessID, WowProcess.GetAllWowProcesses().Count));
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

        private static void WowProcessStopped(object sender, EventArrivedEventArgs e)
        {
            try
            {
                if (e.NewEvent["ProcessName"].ToString().ToLower() == "wow.exe")
                {
                    int pid = Convert.ToInt32(e.NewEvent["ProcessID"]);
                    string name = e.NewEvent["ProcessName"].ToString().Substring(0, e.NewEvent["ProcessName"].ToString().Length - 4);
                    WowProcess pWowProcess = WowProcess.GetAllWowProcesses().FirstOrDefault(x => x.ProcessID == pid);
                    if (pWowProcess != null)
                    {
                        if (WoWManager.Hooked && WoWManager.WoWProcess.ProcessID == pWowProcess.ProcessID)
                        {
                            WoWManager.Unhook();
                            Log.Print(String.Format("{0}:{1} :: [WoW hook] Injector unloaded", name, pid));
                        }
                        pWowProcess.Dispose();
                        Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager disposed", name, pid));
                        if (WowProcess.GetAllWowProcesses().Remove(pWowProcess))
                        {
                            Log.Print(String.Format("{0}:{1} :: [Process watcher] Process closed, {2} total", name, pid, WowProcess.GetAllWowProcesses().Count));
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

        private static void OnWowProcessStartup(object wowProcess)
        {
            try
            {
                WowProcess process = (WowProcess)wowProcess;
                Log.Print(String.Format("{0}:{1} :: [WoW hook] Attaching...", process.ProcessName, process.ProcessID));
                for (int i = 0; i < 120; i++)
                {
                    Thread.Sleep(500);
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
                                NativeMethods.SetWindowPos(process.MainWindowHandle, (IntPtr)SpecialWindowHandles.HWND_NOTOPMOST,
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
                            Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager initialized, base address 0x{2:X}", process.ProcessName, process.ProcessID, (uint)process.Memory.ImageBase));
                            if (!process.IsValidBuild)
                            {
                                Log.Print(String.Format("{0}:{1} :: [WoW hook] Memory manager: invalid WoW executable", process.ProcessName, process.ProcessID), true);
                                MainForm.Instance.ShowTaskDialog("Injector is locked", "Invalid WoW executable", TaskDialogButton.OK, TaskDialogIcon.SecurityError);
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
