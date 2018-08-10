using System;
using System.Management;

namespace AxTools.Helpers
{
    /// <summary>
    ///     Just attach your handler to events and enjoy!
    /// </summary>
    internal static class ProcessWatcherWMI
    {
        private static readonly object _lock = new object();
        private static readonly ManagementEventWatcher _wowWatcherStart;
        private static readonly ManagementEventWatcher _wowWatcherStop;
        private static Action<ProcessInfo> _processStarted;
        private static Action<ProcessInfo> _processExited;

        internal static event Action<ProcessInfo> ProcessStarted
        {
            add
            {
                lock (_lock)
                {
                    _processStarted += value;
                }
            }
            remove
            {
                lock (_lock)
                {
                    _processStarted -= value;
                }
            }
        }

        internal static event Action<ProcessInfo> ProcessExited
        {
            add
            {
                lock (_lock)
                {
                    _processExited += value;
                }
            }
            remove
            {
                lock (_lock)
                {
                    _processExited -= value;
                }
            }
        }

        static ProcessWatcherWMI()
        {
            _wowWatcherStart = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            _wowWatcherStop = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            _wowWatcherStart.EventArrived += WowProcessStarted;
            _wowWatcherStop.EventArrived += WowProcessStopped;
            _wowWatcherStart.Start();
            _wowWatcherStop.Start();
            Program.Exit += Program_Exit;
        }

        private static void Program_Exit()
        {
            Program.Exit -= Program_Exit;
            if (_wowWatcherStart != null && _wowWatcherStop != null)
            {
                _wowWatcherStart.EventArrived -= WowProcessStarted;
                _wowWatcherStop.EventArrived -= WowProcessStopped;
                _wowWatcherStop.Stop();
                _wowWatcherStart.Stop();
                _wowWatcherStop.Dispose();
                _wowWatcherStart.Dispose();
            }
        }

        private static void WowProcessStopped(object sender, EventArrivedEventArgs e)
        {
            ProcessInfo pInfo = new ProcessInfo(int.Parse(e.NewEvent["ProcessID"].ToString()), e.NewEvent["ProcessName"].ToString());
            if (_processExited != null)
            {
                _processExited(pInfo);
            }
        }

        private static void WowProcessStarted(object sender, EventArrivedEventArgs e)
        {
            ProcessInfo pInfo = new ProcessInfo(int.Parse(e.NewEvent["ProcessID"].ToString()), e.NewEvent["ProcessName"].ToString());
            if (_processStarted != null)
            {
                _processStarted(pInfo);
            }
        }
    }

    internal class ProcessInfo
    {
        internal readonly string ProcessName;
        internal readonly int ProcessID;

        internal ProcessInfo(int id, string name)
        {
            ProcessName = name;
            ProcessID = id;
        }
    }
}