using AxTools.Helpers;
using AxTools.WoW.Management;
using System;
using System.Diagnostics;
using System.Threading;

namespace AxTools.WoW.PluginSystem.API
{
    public class SingleThreadTimer : IDisposable
    {
        private Stopwatch balancingStopwatch;
        private readonly object _lock = new object();
        private readonly int interval;
        private readonly Action action;
        private volatile bool flag;
        private Thread thread;

        // optional variable
        public string PluginName;

        public SingleThreadTimer(int interval, Action action)
        {
            this.interval = interval;
            this.action = action;
        }

        public void Start()
        {
            lock (_lock)
            {
                thread = new Thread(Loop) { IsBackground = true };
                balancingStopwatch = new Stopwatch();
                flag = true;
                thread.Start();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                flag = false;
                if (!thread.Join(5000))
                {
                    throw new Exception(string.Format("{0}:{1} :: [{2}] SingleThreadTimer: Can't stop!", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, PluginName ?? ""));
                }
                thread = null;
            }
        }

        private void Loop()
        {
            while (flag)
            {
                balancingStopwatch.Restart();
                if (WoWManager.WoWProcess.IsInGame)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("{0}:{1} :: [{2}] SingleThreadTimer error: {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, PluginName ?? "", ex.Message));
                    }
                }
                int shouldWait = (int) (interval - balancingStopwatch.ElapsedMilliseconds);
                while (shouldWait > 0 && flag)
                {
                    int t = Math.Min(shouldWait, 100);
                    shouldWait -= t;
                    Thread.Sleep(t);
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }
    
    }
}
