namespace Components.WinAPI
{
    internal static class Win32Consts
    {

        internal const uint WM_NULL = 0x0;
        internal const uint WM_QUERYENDSESSION = 0x11;
        internal const uint WM_KEYDOWN = 0x100;
        internal const uint WM_KEYUP = 0x101;
        internal const uint WM_CHAR = 0x102;
        internal const uint WM_BM_CLICK = 0xF5;
        internal const uint WM_RBUTTONDOWN = 0x204; //Right mousebutton down
        internal const uint WM_RBUTTONUP = 0x205;   //Right mousebutton up
        internal const uint WS_CAPTION = 0xC00000;
        internal const uint WS_THICKFRAME = 0x40000;
        internal const int WS_MINIMIZE = 0x20000000;
        internal const int GWL_STYLE = -16;
        internal const int GWL_EXSTYLE = -20;
        internal const uint WS_EX_LAYERED = 0x80000;
        internal const uint WS_EX_TRANSPARENT = 0x20;
        internal const uint LWA_ALPHA = 0x2;
        internal const uint PAGE_EXECUTE_READWRITE = 64;
        internal const int SND_ALIAS = 65536;
        internal const int SND_NODEFAULT = 2;
        internal const int WS_EX_TOPMOST = 0x00000008;

    }
}
