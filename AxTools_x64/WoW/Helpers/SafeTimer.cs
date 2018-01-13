using System;
using System.Diagnostics;
using System.Threading;
using AxTools.Helpers;
using AxTools.WoW.PluginSystem.API;

namespace AxTools.WoW.Helpers
{
    public class SafeTimer : IDisposable
    {
        private Stopwatch balancingStopwatch;
        private readonly object _lock = new object();
        private readonly int interval;
        private readonly Action action;
        private volatile bool flag;
        private Thread thread;
        private static readonly Log2 log = new Log2("SafeTimer");

        // optional variable
        public string PluginName;

        public SafeTimer(int interval, Action action)
        {
            this.interval = interval;
            this.action = action;
        }

        public void Start()
        {
            lock (_lock)
            {
                if (thread == null)
                {
                    thread = new Thread(Loop) { IsBackground = true};
                    balancingStopwatch = new Stopwatch();
                    flag = true;
                    thread.Start();
                }
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (thread != null)
                {
                    flag = false;
                    if (!thread.Join(60*1000))
                    {
                        throw new Exception(string.Format("{0}:{1} :: [{2}] SafeTimer: Can't stop!", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, PluginName ?? ""));
                    }
                    thread = null;
                }
            }
        }

        private void Loop()
        {
            while (flag)
            {
                balancingStopwatch.Restart();
                if (Info.IsInGame)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("{0}:{1} :: [{2}] SafeTimer error: {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, PluginName ?? "", ex.Message));
                    }
                }
                int shouldWait = (int) (interval - balancingStopwatch.ElapsedMilliseconds);
                while (shouldWait > 0 && flag)
                {
                    Thread.Sleep(Math.Min(shouldWait, 100));
                    shouldWait = (int)(interval - balancingStopwatch.ElapsedMilliseconds);
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public bool IsRunning
        {
            get { return flag; }
        }

    }
}
