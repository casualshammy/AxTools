using System;
using System.Runtime.InteropServices;

namespace AxTools.Classes.WoW
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WowPoint
    {
        internal readonly float X;
        internal readonly float Y;
        internal readonly float Z;

        internal WowPoint(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal double Distance(WowPoint p)
        {
            return Math.Sqrt((X - p.X)*(X - p.X) + (Y - p.Y)*(Y - p.Y) + (Z - p.Z)*(Z - p.Z));
        }

        public override string ToString()
        {
            return String.Format("{0},{1},{2}", (int) X, (int) Y, (int) Z);
        }
    }
}