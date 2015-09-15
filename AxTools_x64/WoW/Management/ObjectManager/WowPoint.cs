using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace AxTools.WoW.Management.ObjectManager
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WowPoint
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
            return string.Format("{0},{1},{2}", (int) X, (int) Y, (int) Z);
        }

        public static WowPoint operator -(WowPoint left, WowPoint right)
        {
            return new WowPoint(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static WowPoint operator *(WowPoint wowPoint, float v)
        {
            return new WowPoint(wowPoint.X*v, wowPoint.Y*v, wowPoint.Z*v);
        }

    }
}