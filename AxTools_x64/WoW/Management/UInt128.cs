using System;
using System.Runtime.InteropServices;

namespace AxTools.WoW.Management
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UInt128 : IEquatable<UInt128>
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
            return a.High == b.High && a.Low == b.Low;
        }

        public static bool operator !=(UInt128 a, UInt128 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return GetType() == obj.GetType() && Equals((UInt128) obj);
        }

        public bool Equals(UInt128 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (High.GetHashCode() * 397) ^ Low.GetHashCode();
            }
        }

        public override string ToString()
        {
            // ReSharper disable ImpureMethodCallOnReadonlyValueField
            return "[" + High.ToString("X") + ", " + Low.ToString("X") + "]";
            // ReSharper restore ImpureMethodCallOnReadonlyValueField
        }

    }
}