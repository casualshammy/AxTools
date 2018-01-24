using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW.Internals;
using System;
using System.Threading;
using System.Windows.Forms;

namespace AxTools.WoW.PluginSystem.API
{
    public static class MoveMgr
    {

        private static readonly Log2 log = new Log2($"MoveMgr");

        public static void Move2D(WowPoint point, float precision, int timeoutInMs, bool continueMovingIfFailed, bool continueMovingIfSuccessful)
        {
            WoWManager.WoWProcess.WaitWhileWoWIsMinimized();
            if (Info.IsInGame && !Info.IsLoadingScreen)
            {
                WoWPlayerMe me = ObjMgr.Pulse();
                WowPoint oldPos = me.Location;
                while (Info.IsInGame && !Info.IsLoadingScreen && timeoutInMs > 0 && me.Location.Distance2D(point) > precision)
                {
                    Thread.Sleep(100);
                    timeoutInMs -= 100;
                    me = ObjMgr.Pulse();
                    point.Face();
                    if (me.Location.Distance2D(oldPos) > 1f)
                    {
                        oldPos = me.Location;
                        log.Info(string.Format("[Move2D] Okay, we're moving; current position: [{0}]; distance to dest: [{1}]", me.Location, me.Location.Distance2D(point))); // todo: remove
                    }
                    else if (!me.IsMoving)
                    {
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                        log.Info(string.Format("[Move2D] W is released: {0}", point)); // todo: remove
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                        log.Info(string.Format("[Move2D] W is pressed: {0}", point)); // todo: remove
                    }
                }
                if ((!continueMovingIfFailed || timeoutInMs > 0) && (!continueMovingIfSuccessful || timeoutInMs <= 0))
                {
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                    log.Info(string.Format("[Move2D] W is released2: {0}", point)); // todo: remove
                }
                log.Info(string.Format("[Move2D] return; distance to dest: [{0}]", me.Location.Distance2D(point))); // todo: remove
            }
        }
        
        public static void Move3D(WowPoint point, float precision2D, float precisionZ, int timeoutInMs, bool continueMoving)
        {
            WoWManager.WoWProcess.WaitWhileWoWIsMinimized();
            if (Info.IsInGame && !Info.IsLoadingScreen)
            {
                WoWPlayerMe me = ObjMgr.Pulse();
                WowPoint oldPos = me.Location;
                float zDiff = Math.Abs(me.Location.Z - point.Z);
                while (Info.IsInGame && !Info.IsLoadingScreen && timeoutInMs > 0 && (me.Location.Distance2D(point) > precision2D || zDiff > precisionZ))
                {
                    Thread.Sleep(100);
                    timeoutInMs -= 100;
                    me = ObjMgr.Pulse();
                    float oldZDiff = zDiff;
                    zDiff = Math.Abs(me.Location.Z - point.Z);
                    if (me.Location.Distance2D(point) > precision2D)
                    {
                        point.Face();
                        if (me.Location.Distance2D(oldPos) > 1f)
                        {
                            oldPos = me.Location;
                            log.Info(string.Format("[Move3D] Okay, we're moving XY; current position: [{0}]; distance2D to dest: [{1}]", me.Location, me.Location.Distance2D(point))); // todo: remove
                        }
                        else if (!me.IsMoving)
                        {
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                            log.Info(string.Format("[Move3D] W is released: {0}", point)); // todo: remove
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                            log.Info(string.Format("[Move3D] W is pressed: {0}", point)); // todo: remove
                        }
                    }
                    else
                    {
                        if (zDiff > precisionZ || !continueMoving)
                        {
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                            log.Info(string.Format("[Move3D] W is released3: {0}", point)); // todo: remove
                        }
                    }
                    if (me.IsFlying && me.IsMounted && zDiff > precisionZ)
                    {
                        if (zDiff < oldZDiff)
                        {
                            log.Info(string.Format("[Move3D] Okay, we're moving Z; current position: [{0}]; distance to dest: [{1}]; zDiff: {2}; oldZDiff: {3}", me.Location,
                                me.Location.Distance(point), zDiff, oldZDiff)); // todo: remove
                        }
                        else
                        {
                            if (me.Location.Z - point.Z > precisionZ)
                            {
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.X, IntPtr.Zero);
                                log.Info(string.Format("[Move3D] X is pressed: {0}", point)); // todo: remove
                            }
                            else if (point.Z - me.Location.Z > precisionZ)
                            {
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.X, IntPtr.Zero);
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
                                log.Info(string.Format("[Move3D] Space is pressed: {0}", point)); // todo: remove
                            }
                        }
                    }
                    else
                    {
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.X, IntPtr.Zero);
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
                        log.Info(string.Format("[Move3D] X is released: {0}", point)); // todo: remove
                        log.Info(string.Format("[Move3D] Space is released: {0}", point)); // todo: remove
                    }
                }
                log.Info(string.Format("[GameFunctions.Move3D] Return, timeout: {0}, diffXY: {1}, diffZ: {2}", timeoutInMs, me.Location.Distance2D(point), zDiff)); // todo: remove
            }
        }
        
        public static void Jump()
        {
            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
        }

    }
}
