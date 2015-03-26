using System;
using System.Runtime.InteropServices;

namespace AxTools.WoW.Management.ObjectManager
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WowPoint
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

        public double Distance(WowPoint p)
        {
            return Math.Sqrt((X - p.X)*(X - p.X) + (Y - p.Y)*(Y - p.Y) + (Z - p.Z)*(Z - p.Z));
        }

        public override string ToString()
        {
            return String.Format("{0},{1},{2}", (int) X, (int) Y, (int) Z);
        }
    
    }
}