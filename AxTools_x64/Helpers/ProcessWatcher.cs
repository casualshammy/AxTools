using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Timers;
using AxTools.Forms;
using Timer = System.Timers.Timer;

namespace AxTools.Helpers
{
    /// <summary>
    ///     Just attach your handler to events and enjoy!
    /// </summary>
    internal static class ProcessWatcher
    {
        private static readonly Timer _timer = new Timer(1000);
        private static Dictionary<int, ProcessInfo> _cache = new Dictionary<int, ProcessInfo>();
        private static readonly object _lock = new object();
        private static Action<ProcessInfo> _processStarted;
        private static Action<ProcessInfo> _processExited;
        private static readonly Stopwatch _stopwatch = new Stopwatch();
        private static int _counter;

        internal static event Action<ProcessInfo> ProcessStarted
        {
            add
            {
                lock (_lock)
                {
                    RefreshCache();
                    _processStarted += value;
                    CheckTimerIsNeeded();
                }
            }
            remove
            {
                lock (_lock)
                {
                    _processStarted -= value;
                    CheckTimerIsNeeded();
                }
            }
        }

        internal static event Action<ProcessInfo> ProcessExited
        {
            add
            {
                lock (_lock)
                {
                    RefreshCache();
                    _processExited += value;
                    CheckTimerIsNeeded();
                }
            }
            remove
            {
                lock (_lock)
                {
                    _processExited -= value;
                    CheckTimerIsNeeded();
                }
            }
        }

        static ProcessWatcher()
        {
            _timer.Elapsed += _timer_Elapsed;
            MainForm.ClosingEx += MainForm_ClosingEx;
        }

        private static void MainForm_ClosingEx()
        {
            MainForm.ClosingEx -= MainForm_ClosingEx;
            Log.Error(string.Format("ProcessWatcher: Elpsd: {0}ms, counter: {1}; so {2}ms/call", _stopwatch.ElapsedMilliseconds, _counter, (float) _stopwatch.ElapsedMilliseconds/_counter));
        }

        private static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _stopwatch.Start();
            Dictionary<int, ProcessInfo> actualProcessInfos = new Dictionary<int, ProcessInfo>();
            foreach (Process process in Process.GetProcesses())
            {
                string processName = GetProcessNameWithExtension(process);
                int hash = process.Id ^ processName.GetHashCode();
                ProcessInfo info = new ProcessInfo(process.Id, processName);
                actualProcessInfos.Add(hash, info);
                if (!_cache.ContainsKey(hash) && _processStarted != null)
                {
                    _processStarted(info);
                }
                process.Dispose();
            }
            if (_processExited != null)
            {
                foreach (KeyValuePair<int, ProcessInfo> processInfo in _cache)
                {
                    if (!actualProcessInfos.ContainsKey(processInfo.Key))
                    {
                        if (_processExited != null)
                        {
                            _processExited(processInfo.Value);
                        }
                    }
                }
            }
            _cache = actualProcessInfos;
            _stopwatch.Stop();
            _counter++;
        }

        private static void CheckTimerIsNeeded()
        {
            if (_processExited == null && _processStarted == null && _timer.Enabled)
            {
                _timer.Stop();
            }
            if ((_processExited != null || _processStarted != null) && !_timer.Enabled)
            {
                _timer.Start();
            }
        }

        private static void RefreshCache()
        {
            _timer_Elapsed(null, null);
        }

        private static string GetProcessNameWithExtension(Process process)
        {
            try
            {
                string moduleName = process.MainModule.ModuleName;
                return Path.GetFileName(moduleName);
            }
            catch
            {
                try
                {
                    return process.ProcessName;
                }
                catch
                {
                    return "";
                }
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
