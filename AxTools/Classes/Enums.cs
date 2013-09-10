using System;

namespace AxTools.Classes
{
    internal static class Enums
    {
        [Flags]
        internal enum UpdateResult
        {
            None = 0x0,
            UpdateForMainExecutableIsAvailable = 0x1,
            UpdateForAddonIsAvailable = 0x2
        }
    }
}
