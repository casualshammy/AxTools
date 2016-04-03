using System;
using System.Threading;
using System.Windows.Forms;
using AxTools.WinAPI;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;

namespace AxTools.WoW.Helpers
{
    internal static class MoveHelper
    {
        internal static float AngleHorizontal(WowPoint point)
        {
            WoWPlayerMe me = ObjectMgr.Pulse();
            float angle = Convert.ToSingle(Math.Atan2(Convert.ToDouble(point.Y) - Convert.ToDouble(me.Location.Y), Convert.ToDouble(point.X) - Convert.ToDouble(me.Location.X)));
            angle = NegativeAngle(angle);
            return angle;
        }

        internal static float NegativeAngle(float angle)
        {
            //if the turning angle is negative
            if (angle < 0)
                //add the maximum possible angle (PI x 2) to normalize the negative angle
                angle += (float)(Math.PI * 2);
            return angle;
        }

        internal static void FaceHorizontalWithTimer(float radius, Keys key, bool moving)
        {
            if (radius < 0.1f)
                return;
            int turnTime = moving ? 1328 : 980;
            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
            Thread.Sleep((int)((radius * turnTime * Math.PI) / 10));
            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)key, IntPtr.Zero);
        }

    }
}
