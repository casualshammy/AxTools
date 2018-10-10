namespace WoWGold_Notifier.WinAPI
{
    // ReSharper disable InconsistentNaming
    internal enum WM_MESSAGE : uint
    {
        WM_NULL = 0x0,
        WM_QUERYENDSESSION = 0x11,
        WM_KEYDOWN = 256,
        WM_KEYUP = 257,
        WM_CHAR = 0x0102,
        WM_BM_CLICK = 0x00F5,
    }
    // ReSharper restore InconsistentNaming
}