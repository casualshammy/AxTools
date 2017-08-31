using System;
using System.Timers;
using AxTools.Helpers;
using AxTools.WoW.PluginSystem.API;
using AxTools.WinAPI;
using System.Windows.Forms;
using AxTools.Services;

namespace AxTools.WoW
{
    internal class WoWAntiKick : IDisposable
    {
        private readonly Settings settings = Settings.Instance;
        private readonly WowProcess wowProcess;
        private readonly System.Timers.Timer timer;
        private int maxTime;
        private int fallback_lastTimeActionEmulated;
        private bool fallback_keyboardWatcherInitialized = false;
        internal static event Action<IntPtr> ActionEmulated;

        internal WoWAntiKick(WowProcess wowProcess)
        {
            this.wowProcess = wowProcess;
            maxTime = Utils.Rnd.Next(150000, 290000);
            fallback_lastTimeActionEmulated = Environment.TickCount;
            timer = new System.Timers.Timer(2000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (settings.WoWAntiKick)
            {
                if (wowProcess.IsValidBuild)
                {
                    if (Info.IsProcessInGame(wowProcess))
                    {
                        try
                        {
                            int lastHardwareAction = wowProcess.Memory.Read<int>(wowProcess.Memory.ImageBase + WowBuildInfoX64.LastHardwareAction);
                            int tickCount = wowProcess.Memory.Read<int>(wowProcess.Memory.ImageBase + WowBuildInfoX64.TickCount);
                            if (tickCount - lastHardwareAction > maxTime)
                            {
                                maxTime = Utils.Rnd.Next(150000, 280000);
                                wowProcess.Memory.Write(wowProcess.Memory.ImageBase + WowBuildInfoX64.LastHardwareAction, tickCount);
                                ActionEmulated?.Invoke(wowProcess.MainWindowHandle);
                                Log.Info(string.Format("{0} [Anti-AFK] Action emulated, next MaxTime: {1}", wowProcess, maxTime));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("{0} [Anti-AFK] Can't emulate action: {1}", wowProcess, ex.Message));
                        }
                    }
                }
                else
                {
                    if (!fallback_keyboardWatcherInitialized)
                    {
                        HotkeyManager.AddKeys(GetType().ToString() + wowProcess.ProcessID.ToString(), Keys.W, Keys.A, Keys.S, Keys.D, Keys.Space);
                        HotkeyManager.KeyPressed += HotkeyManager_KeyPressed;
                        fallback_keyboardWatcherInitialized = true;
                        Log.Info($"{wowProcess} [Anti-AFK] Unsupported version of WoW client. Adding WASDSpace to hotkeys");
                    }
                    //Log.Info($"{Environment.TickCount}; {fallback_lastTimeActionEmulated}; {maxTime}");
                    if (Environment.TickCount - fallback_lastTimeActionEmulated > maxTime)
                    {
                        maxTime = Utils.Rnd.Next(150000, 280000);
                        fallback_lastTimeActionEmulated = Environment.TickCount;
                        NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
                        NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
                        ActionEmulated?.Invoke(wowProcess.MainWindowHandle);
                        Log.Info($"{wowProcess} [Anti-AFK] Unsupported version of WoW client, fallback to space jumping; next MaxTime: {maxTime}");
                    }
                }
            }
        }

        private void HotkeyManager_KeyPressed(Keys obj)
        {
            if (obj == Keys.W || obj == Keys.A || obj == Keys.S || obj == Keys.D || obj == Keys.Space)
            {
                if (NativeMethods.GetForegroundWindow() == wowProcess.MainWindowHandle)
                {
                    fallback_lastTimeActionEmulated = Environment.TickCount;
                }
            }
        }

        public void Dispose()
        {
            HotkeyManager.KeyPressed -= HotkeyManager_KeyPressed;
            timer.Close();
        }
    
    }
}
