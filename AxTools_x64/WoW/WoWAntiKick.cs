using System;
using System.Timers;
using AxTools.Helpers;

namespace AxTools.WoW
{
    internal class WoWAntiKick : IDisposable
    {
        private readonly Settings settings = Settings.Instance;
        private readonly WowProcess wowProcess;
        private readonly Timer timer;
        private int maxTime;

        internal WoWAntiKick(WowProcess wowProcess)
        {
            this.wowProcess = wowProcess;
            maxTime = Utils.Rnd.Next(150000, 290000);
            timer = new Timer(2000);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (settings.WoWAntiKick)
            {
                if (wowProcess.IsValidBuild && wowProcess.IsInGame)
                {
                    try
                    {
                        int lastHardwareAction = wowProcess.Memory.Read<int>(wowProcess.Memory.ImageBase + WowBuildInfoX64.LastHardwareAction);
                        int tickCount = wowProcess.Memory.Read<int>(wowProcess.Memory.ImageBase + WowBuildInfoX64.TickCount);
                        if (tickCount - lastHardwareAction > maxTime)
                        {
                            maxTime = Utils.Rnd.Next(150000, 280000);
                            wowProcess.Memory.Write(wowProcess.Memory.ImageBase + WowBuildInfoX64.LastHardwareAction, tickCount);
                            Log.Info(String.Format("{0}:{1} :: [Anti-AFK] Action emulated, next MaxTime: {2}", wowProcess.ProcessName, wowProcess.ProcessID, maxTime));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(String.Format("{0}:{1} :: [Anti-AFK] Can't emulate action: {2}", wowProcess.ProcessName, wowProcess.ProcessID, ex.Message));
                    }
                }
            }
        }

        public void Dispose()
        {
            timer.Close();
        }
    
    }
}
