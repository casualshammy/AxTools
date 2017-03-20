using System;
using System.Timers;
using AxTools.Helpers;
using AxTools.WoW.PluginSystem.API;

namespace AxTools.WoW
{
    internal class WoWAntiKick : IDisposable
    {
        private readonly Settings settings = Settings.Instance;
        private readonly WowProcess wowProcess;
        private readonly Timer timer;
        private int maxTime;
        internal static event Action<IntPtr> ActionEmulated;

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
                if (wowProcess.IsValidBuild && GameFunctions.IsProcessInGame(wowProcess))
                {
                    try
                    {
                        int lastHardwareAction = wowProcess.Memory.Read<int>(wowProcess.Memory.ImageBase + WowBuildInfoX64.LastHardwareAction);
                        int tickCount = wowProcess.Memory.Read<int>(wowProcess.Memory.ImageBase + WowBuildInfoX64.TickCount);
                        if (tickCount - lastHardwareAction > maxTime)
                        {
                            maxTime = Utils.Rnd.Next(150000, 280000);
                            wowProcess.Memory.Write(wowProcess.Memory.ImageBase + WowBuildInfoX64.LastHardwareAction, tickCount);
                            if (ActionEmulated != null)
                            {
                                ActionEmulated(wowProcess.MainWindowHandle);
                            }
                            Log.Info(string.Format("{0} [Anti-AFK] Action emulated, next MaxTime: {1}", wowProcess, maxTime));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("{0} [Anti-AFK] Can't emulate action: {1}", wowProcess, ex.Message));
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
