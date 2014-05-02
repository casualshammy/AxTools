using System;

namespace AxTools.Classes.WoW
{
    [Flags]
    internal enum MouseButton : uint
    {
        Left = 1,
        Middle = 2,
        None = 0,
        Right = 4,
        XButton1 = 8,
        XButton2 = 0x10
    }
}