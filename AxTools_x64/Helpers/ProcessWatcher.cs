using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace AxTools.Helpers
{
    /// <summary>
    ///     Just attach your handler to events and enjoy!
    /// </summary>
    internal static class ProcessWatcher
    {
        private static readonly Timer _timer = new Timer(1000);
        private static Dictionary<int, string> _cache = new Dictionary<int, string>();
        private static readonly object _lock = new object();

        private static Action<Process> _processStarted;
        /// <summary>
        ///     You should dispose this <see cref="Process"/> instance
        /// </summary>
        internal static event Action<Process> ProcessStarted
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

        private static Action<int,string> _processExited;
        internal static event Action<int, string> ProcessExited
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

        private static Stopwatch stopwatch = new Stopwatch();
        private static int counter = 0;

        static ProcessWatcher()
        {
            _timer.Elapsed += _timer_Elapsed;
            Application.ApplicationExit += ApplicationOnApplicationExit;
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs eventArgs)
        {
            Application.ApplicationExit -= ApplicationOnApplicationExit;
            Log.Info("ProcessWatcher: Elpsd: " + stopwatch.ElapsedMilliseconds + "ms, counter: " + counter);
        }

        private static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            stopwatch.Start();
            Dictionary<int, string> actualProcessInfos = new Dictionary<int, string>();
            foreach (Process process in Process.GetProcesses())
            {
                actualProcessInfos.Add(process.Id, process.ProcessName);
                if (!_cache.ContainsKey(process.Id) && _processStarted != null)
                {
                    _processStarted(process);
                }
                else
                {
                    process.Dispose();
                }
            }
            if (_processExited != null)
            {
                foreach (KeyValuePair<int, string> processInfo in _cache.Except(actualProcessInfos))
                {
                    if (_processExited != null)
                    {
                        _processExited(processInfo.Key, processInfo.Value);
                    }
                }
            }
            _cache = actualProcessInfos;
            stopwatch.Stop();
            counter++;
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

        //private class KeyValuePairEqualityComparer : IEqualityComparer<KeyValuePair<int, string>>
        //{

        //    public bool Equals(KeyValuePair<int, string> b1, KeyValuePair<int, string> b2)
        //    {
        //        if (b1.Height == b2.Height & b1.Length == b2.Length
        //                            & b1.Width == b2.Width)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }


        //    public int GetHashCode(KeyValuePair<int, string> bx)
        //    {
        //        return bx.GetHashCode();
        //    }

        //}

    }
}
