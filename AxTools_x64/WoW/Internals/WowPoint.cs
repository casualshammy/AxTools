using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AxTools.WinAPI;
using AxTools.WoW.Helpers;

namespace AxTools.WoW.Internals
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WowPoint : IEquatable<WowPoint>
    {

        public float X;
        public float Y;
        public float Z;

        public WowPoint(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [Pure]
        public double Distance(WowPoint p)
        {
            return Math.Sqrt((X - p.X)*(X - p.X) + (Y - p.Y)*(Y - p.Y) + (Z - p.Z)*(Z - p.Z));
        }

        [Pure]
        public double Distance2D(WowPoint p)
        {
            return Math.Sqrt((X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y));
        }

        public static WowPoint GetNearestPoint(WowPoint start, WowPoint end, float distanceFromEnd)
        {
            WowPoint dir = end - start;
            dir.Normalize();
            return end - dir * distanceFromEnd;
        }

        private void Normalize()
        {
            float magnitude = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            X /= magnitude;
            Y /= magnitude;
            Z /= magnitude;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", Math.Round(X, 2), Math.Round(Y, 2), Math.Round(Z, 2));
        }

        public static WowPoint operator -(WowPoint left, WowPoint right)
        {
            return new WowPoint(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static WowPoint operator *(WowPoint wowPoint, float v)
        {
            return new WowPoint(wowPoint.X*v, wowPoint.Y*v, wowPoint.Z*v);
        }

        public void Face()
        {
            float face;
            WoWPlayerMe me = ObjectMgr.Pulse();
            if (MoveHelper.NegativeAngle(MoveHelper.AngleHorizontal(this) - me.Rotation) < Math.PI)
            {
                face = MoveHelper.NegativeAngle(MoveHelper.AngleHorizontal(this) - me.Rotation);
                bool moving = me.IsMoving;
                if (face > 1)
                {
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.S, IntPtr.Zero);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.S, IntPtr.Zero);
                    moving = false;
                }
                MoveHelper.FaceHorizontalWithTimer(face, Keys.A, moving);
            }
            else
            {
                face = MoveHelper.NegativeAngle(me.Rotation - MoveHelper.AngleHorizontal(this));
                bool moving = me.IsMoving;
                if (face > 1)
                {
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.S, IntPtr.Zero);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.S, IntPtr.Zero);
                    moving = false;
                }
                MoveHelper.FaceHorizontalWithTimer(face, Keys.D, moving);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return X == 0 && Y == 0 && Z == 0;
            }
        }

        public bool Equals(WowPoint other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object other)
        {
            return other != null && other is WowPoint p && X == p.X && Y == p.Y && Z == p.Z;
        }

        public static bool operator ==(WowPoint a, WowPoint b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(WowPoint a, WowPoint b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(X), 0) ^ BitConverter.ToInt32(BitConverter.GetBytes(Y), 0) ^ BitConverter.ToInt32(BitConverter.GetBytes(Z), 0);
        }

    }
}