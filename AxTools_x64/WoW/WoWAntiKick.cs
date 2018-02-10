using System;
using System.Timers;
using AxTools.Helpers;
using AxTools.WoW.PluginSystem.API;
using AxTools.WinAPI;
using System.Windows.Forms;
using KeyboardWatcher;
using System.Linq;
using System.Collections.Generic;

namespace AxTools.WoW
{
    internal class WoWAntiKick : IDisposable
    {
        private static readonly Log2 logger = new Log2("WoW anti-kick");
        private readonly Settings2 settings = Settings2.Instance;
        private readonly WowProcess wowProcess;
        private readonly System.Timers.Timer timer;
        private int maxTime;
        private int fallback_lastTimeActionEmulated;
        private bool fallback_keyboardWatcherInitialized = false;
        private GameInterface info;
        internal static event Action<IntPtr> ActionEmulated;
        private static KeyExt[] moveKeys = new KeyExt[] { new KeyExt(Keys.W), new KeyExt(Keys.A), new KeyExt(Keys.S), new KeyExt(Keys.D), new KeyExt(Keys.Space) };
        private static Dictionary<int, WoWAntiKick> instanses = new Dictionary<int, WoWAntiKick>();

        internal static void StartWaitForWoWProcesses()
        {
            WoWProcessManager.WoWProcessReadyForInteraction += WoWProcessManager_WoWProcessReadyForInteraction;
            WoWProcessManager.WoWProcessClosed += WoWProcessManager_WoWProcessClosed;
        }

        private static void WoWProcessManager_WoWProcessClosed(int obj)
        {
            if (instanses.ContainsKey(obj))
            {
                WoWAntiKick wak = instanses[obj];
                instanses.Remove(obj);
                wak.Dispose();
            }
            else
            {
                logger.Error($"Cannot find instance for process with id:{obj}");
            }
        }

        private static void WoWProcessManager_WoWProcessReadyForInteraction(WowProcess obj)
        {
            instanses.Add(obj.ProcessID, new WoWAntiKick(obj));
        }

        internal WoWAntiKick(WowProcess wowProcess)
        {
            this.wowProcess = wowProcess;
            info = new GameInterface(wowProcess);
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
                    if (info.IsInGame)
                    {
                        try
                        {
                            int lastHardwareAction = wowProcess.Memory.Read<int>(wowProcess.Memory.ImageBase + WowBuildInfoX64.LastHardwareAction);
                            int tickCount = wowProcess.Memory.Read<int>(wowProcess.Memory.ImageBase + WowBuildInfoX64.TickCount);
                            if (tickCount - lastHardwareAction > maxTime)
                            {
                                maxTime = Utils.Rnd.Next(150000, 280000);
                                wowProcess.Memory.Write(wowProcess.Memory.ImageBase + WowBuildInfoX64.LastHardwareAction, tickCount);
                                logger.Info(string.Format("{0} Action emulated, next MaxTime: {1}", wowProcess, maxTime));
                                if (settings.WoW_AntiKick_SetAfkState)
                                {
                                    if (!wowProcess.IsMinimized)
                                    {
                                        if (!info.IsAfk)
                                        {
                                            info.IsAfk = true;
                                            logger.Info($"{wowProcess} /afk status is set");
                                        }
                                        else
                                        {
                                            logger.Info($"{wowProcess} /afk status is already set");
                                        }
                                    }
                                    else
                                    {
                                        logger.Info($"{wowProcess} Can't set /afk status because WoW client is minimized");
                                    }
                                }
                                ActionEmulated?.Invoke(wowProcess.MainWindowHandle);
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
