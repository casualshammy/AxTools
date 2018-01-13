using System;
using System.Timers;
using AxTools.Helpers;
using AxTools.WoW.PluginSystem.API;
using AxTools.WinAPI;
using System.Windows.Forms;
using KeyboardWatcher;
using System.Linq;

namespace AxTools.WoW
{
    internal class WoWAntiKick : IDisposable
    {
        private static readonly Log2 logger = new Log2("WoW anti-kick");
        private readonly Settings settings = Settings.Instance;
        private readonly WowProcess wowProcess;
        private readonly System.Timers.Timer timer;
        private int maxTime;
        private int fallback_lastTimeActionEmulated;
        private bool fallback_keyboardWatcherInitialized = false;
        internal static event Action<IntPtr> ActionEmulated;
        private static KeyExt[] moveKeys = new KeyExt[] { new KeyExt(Keys.W), new KeyExt(Keys.A), new KeyExt(Keys.S), new KeyExt(Keys.D), new KeyExt(Keys.Space) };

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
                                logger.Info(string.Format("{0} Action emulated, next MaxTime: {1}", wowProcess, maxTime));
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(string.Format("{0} Can't emulate action: {1}", wowProcess, ex.Message));
                        }
                    }
                }
                else
                {
                    if (!fallback_keyboardWatcherInitialized)
                    {
                        KeyboardWatcher.HotkeyManager.AddKeys(GetType().ToString() + wowProcess.ProcessID.ToString(), moveKeys);
                        KeyboardWatcher.HotkeyManager.KeyPressed += HotkeyManager_KeyPressed;
                        fallback_keyboardWatcherInitialized = true;
                        logger.Info($"{wowProcess} Unsupported version of WoW client. Adding WASDSpace to hotkeys");
                    }
                    //Log.Info($"{Environment.TickCount}; {fallback_lastTimeActionEmulated}; {maxTime}");
                    if (Environment.TickCount - fallback_lastTimeActionEmulated > maxTime)
                    {
                        maxTime = Utils.Rnd.Next(150000, 280000);
                        fallback_lastTimeActionEmulated = Environment.TickCount;
                        NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
                        NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
                        ActionEmulated?.Invoke(wowProcess.MainWindowHandle);
                        logger.Info($"{wowProcess} Unsupported version of WoW client, fallback to space jumping; next MaxTime: {maxTime}");
                    }
                }
            }
        }

        private void HotkeyManager_KeyPressed(KeyExt obj)
        {
            if (moveKeys.Contains(obj))
            {
                if (NativeMethods.GetForegroundWindow() == wowProcess.MainWindowHandle)
                {
                    fallback_lastTimeActionEmulated = Environment.TickCount;
                }
            }
        }

        public void Dispose()
        {
            KeyboardWatcher.HotkeyManager.KeyPressed -= HotkeyManager_KeyPressed;
            timer.Close();
        }
    
    }
}
