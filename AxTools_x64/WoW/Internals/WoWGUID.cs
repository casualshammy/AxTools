using System;
using System.Runtime.InteropServices;

namespace AxTools.WoW.Internals
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WoWGUID : IEquatable<WoWGUID>
    {
        public readonly ulong High;
        public readonly ulong Low;

        public static readonly WoWGUID Zero = new WoWGUID(0, 0);
        public static unsafe readonly int Size = sizeof(WoWGUID);

        public WoWGUID(ulong high, ulong low)
        {
            High = high;
            Low = low;
        }

        public static bool operator ==(WoWGUID a, WoWGUID b)
        {
            return a.High == b.High && a.Low == b.Low;
        }

        public static bool operator !=(WoWGUID a, WoWGUID b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is WoWGUID && Equals((WoWGUID)obj);
        }

        public bool Equals(WoWGUID other)
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