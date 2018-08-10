using AxTools.Helpers;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Diagnostics;
using System.Threading;

namespace AxTools.WoW.Helpers
{
    public class SafeTimer : IDisposable
    {
        private Stopwatch balancingStopwatch;
        private readonly object _lock = new object();
        private int interval;
        private readonly Action action;
        private volatile bool flag;
        private Thread thread;
        private readonly GameInterface info;
        private static readonly Log2 log = new Log2("SafeTimer");

        // optional variable
        public string PluginName;

        internal SafeTimer(int interval, GameInterface game, Action action)
        {
            this.interval = interval;
            this.action = action;
            this.info = game;
        }

        public void Start()
        {
            lock (_lock)
            {
                if (thread == null)
                {
                    thread = new Thread(Loop) { IsBackground = true };
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
                    if (!thread.Join(60 * 1000))
                    {
                        throw new Exception(string.Format("[{0}] SafeTimer: Can't stop!", PluginName ?? "Unknown plug-in"));
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
                if (info.IsInGame)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("[{0}] SafeTimer error: {1}", PluginName ?? "", ex.Message));
                    }
                }
                int shouldWait = (int)(interval - balancingStopwatch.ElapsedMilliseconds);
                while (shouldWait > 0 && flag)
                {
                    Thread.Sleep(Math.Min(shouldWait, 100));
                    shouldWait = (int)(interval - balancingStopwatch.ElapsedMilliseconds);
                }
            }
        }

        public void ChangeInterval(int interval)
        {
            this.interval = interval;
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