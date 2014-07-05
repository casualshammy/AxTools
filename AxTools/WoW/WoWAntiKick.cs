using System;
using System.Timers;
using AxTools.Classes;

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
                        uint lastHardwareAction = wowProcess.Memory.Read<uint>(wowProcess.Memory.ImageBase + WowBuildInfo.LastHardwareAction);
                        if (Environment.TickCount - lastHardwareAction > maxTime)
                        {
                            maxTime = Utils.Rnd.Next(150000, 280000);
                            wowProcess.Memory.Write(wowProcess.Memory.ImageBase + WowBuildInfo.LastHardwareAction, Environment.TickCount);
                            Log.Print(String.Format("{0}:{1} :: [Anti-AFK] Action emulated, next MaxTime: {2}", wowProcess.ProcessName, wowProcess.ProcessID, maxTime));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Print(String.Format("{0}:{1} :: [Anti-AFK] Can't emulate action: {2}", wowProcess.ProcessName, wowProcess.ProcessID, ex.Message), true);
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
