using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW.Internals;
using System;
using System.Threading;
using System.Windows.Forms;

namespace AxTools.WoW.PluginSystem.API
{
    public class MoveMgr
    {
        
        public static void Move2D(WowPoint point, float precision, int timeoutInMs, bool continueMovingIfFailed, bool continueMovingIfSuccessful)
        {
            GameFunctions.WaitWhileWoWIsMinimized();
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
                        Log.Info(string.Format("{0} [GameFunctions.MoveTo] Okay, we're moving; current position: [{1}]; distance to dest: [{2}]", WoWManager.WoWProcess, me.Location, me.Location.Distance2D(point))); // todo: remove
                    }
                    else if (!me.IsMoving)
                    {
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                        Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is released: {1}", WoWManager.WoWProcess, point)); // todo: remove
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                        Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is pressed: {1}", WoWManager.WoWProcess, point)); // todo: remove
                    }
                }
                if ((!continueMovingIfFailed || timeoutInMs > 0) && (!continueMovingIfSuccessful || timeoutInMs <= 0))
                {
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                    Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is released2: {1}", WoWManager.WoWProcess, point)); // todo: remove
                }
                Log.Info(string.Format("{0} [GameFunctions.MoveTo] return; distance to dest: [{1}]", WoWManager.WoWProcess, me.Location.Distance2D(point))); // todo: remove
            }
        }
        
        public static void Move3D(WowPoint point, float precision2D, float precisionZ, int timeoutInMs, bool continueMoving)
        {
            GameFunctions.WaitWhileWoWIsMinimized();
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
                            Log.Info(string.Format("{0} [GameFunctions.MoveTo] Okay, we're moving XY; current position: [{1}]; distance2D to dest: [{2}]", WoWManager.WoWProcess, me.Location, me.Location.Distance2D(point))); // todo: remove
                        }
                        else if (!me.IsMoving)
                        {
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                            Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is released: {1}", WoWManager.WoWProcess, point)); // todo: remove
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                            Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is pressed: {1}", WoWManager.WoWProcess, point)); // todo: remove
                        }
                    }
                    else
                    {
                        if (zDiff > precisionZ || !continueMoving)
                        {
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                            Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is released3: {1}", WoWManager.WoWProcess, point)); // todo: remove
                        }
                    }
                    if (me.IsFlying && me.IsMounted && zDiff > precisionZ)
                    {
                        if (zDiff < oldZDiff)
                        {
                            Log.Info(string.Format("{0} [GameFunctions.MoveTo] Okay, we're moving Z; current position: [{1}]; distance to dest: [{2}]; zDiff: {3}; oldZDiff: {4}", WoWManager.WoWProcess, me.Location,
                                me.Location.Distance(point), zDiff, oldZDiff)); // todo: remove
                        }
                        else
                        {
                            if (me.Location.Z - point.Z > precisionZ)
                            {
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.X, IntPtr.Zero);
                                Log.Info(string.Format("{0} [GameFunctions.MoveTo] X is pressed: {1}", WoWManager.WoWProcess, point)); // todo: remove
                            }
                            else if (point.Z - me.Location.Z > precisionZ)
                            {
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.X, IntPtr.Zero);
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
                                Log.Info(string.Format("{0} [GameFunctions.MoveTo] Space is pressed: {1}", WoWManager.WoWProcess, point)); // todo: remove
                            }
                        }
                    }
                    else
                    {
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.X, IntPtr.Zero);
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
                        Log.Info(string.Format("{0} [GameFunctions.MoveTo] X is released: {1}", WoWManager.WoWProcess, point)); // todo: remove
                        Log.Info(string.Format("{0} [GameFunctions.MoveTo] Space is released: {1}", WoWManager.WoWProcess, point)); // todo: remove
                    }
                }
                Log.Info(string.Format("{0} [GameFunctions.Move3D] Return, timeout: {1}, diffXY: {2}, diffZ: {3}", WoWManager.WoWProcess, timeoutInMs, me.Location.Distance2D(point), zDiff)); // todo: remove
            }
        }
        
        public static void Jump()
        {
            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
        }

    }
}
