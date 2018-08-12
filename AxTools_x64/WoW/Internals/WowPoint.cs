using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.InteropServices;

namespace AxTools.WoW.Internals
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WowPoint : IEquatable<WowPoint>
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        public WowPoint(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [Pure]
        public double Distance(WowPoint p)
        {
            return Math.Sqrt((X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y) + (Z - p.Z) * (Z - p.Z));
        }

        [Pure]
        public double Distance2D(WowPoint p)
        {
            return Math.Sqrt((X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y));
        }

        public static WowPoint GetNearestPoint(WowPoint start, WowPoint end, float distanceFromEnd)
        {
            WowPoint dir = end - start;
            float magnitude = (float)Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y + dir.Z * dir.Z);
            WowPoint point = new WowPoint(dir.X / magnitude, dir.Y / magnitude, dir.Z / magnitude);
            return end - point * distanceFromEnd;
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
            return new WowPoint(wowPoint.X * v, wowPoint.Y * v, wowPoint.Z * v);
        }

        public bool IsEmpty => Math.Abs(X) < 0.001 && Math.Abs(Y) < 0.001 && Math.Abs(Z) < 0.001;

        public bool Equals(WowPoint other)
        {
            return Math.Abs(X - other.X) < 0.001 && Math.Abs(Y - other.Y) < 0.001 && Math.Abs(Z - other.Z) < 0.001;
        }

        public override bool Equals(object other)
        {
            return other != null && other is WowPoint p && Equals(p);
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