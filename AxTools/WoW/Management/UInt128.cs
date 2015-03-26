using System.Collections.Generic;
using System.Runtime.InteropServices;
using AxTools.Classes;

namespace AxTools.WoW.Management
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UInt128
    {
        public readonly ulong High;
        public readonly ulong Low;

        public static readonly UInt128 Zero = new UInt128(0, 0);

        public UInt128(ulong high, ulong low)
        {
            High = high;
            Low = low;
        }

        public static bool operator ==(UInt128 a, UInt128 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(UInt128 a, UInt128 b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            UInt128 int128 = (UInt128) obj;
            return int128.High == High && int128.Low == Low;
        }

        public override int GetHashCode()
        {
            return High.GetHashCode() | Low.GetHashCode();
        }

        public override string ToString()
        {
            // ReSharper disable ImpureMethodCallOnReadonlyValueField
            return "[" + High.ToString("X") + ", " + Low.ToString("X") + "]";
            // ReSharper restore ImpureMethodCallOnReadonlyValueField
        }

    }
}