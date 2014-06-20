using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace AxTools.Classes.WoW.Overlay.Overlay
{
    internal static class Constants
    {
// ReSharper disable InconsistentNaming

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        public const int LWA_COLORKEY = 0x1;
        public const int LWA_ALPHA = 0x2;

        public const int GWL_ID = (-12);
        public const int GWL_STYLE = (-16);
        public const int GWL_EXSTYLE = (-20);

        // Window Styles
        public const uint WS_OVERLAPPED = 0;
        public const uint WS_POPUP = 0x80000000;
        public const uint WS_CHILD = 0x40000000;
        public const uint WS_MINIMIZE = 0x20000000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_DISABLED = 0x8000000;
        public const uint WS_CLIPSIBLINGS = 0x4000000;
        public const uint WS_CLIPCHILDREN = 0x2000000;
        public const uint WS_MAXIMIZE = 0x1000000;
        public const uint WS_CAPTION = 0xC00000;      // WS_BORDER or WS_DLGFRAME  
        public const uint WS_BORDER = 0x800000;
        public const uint WS_DLGFRAME = 0x400000;
        public const uint WS_VSCROLL = 0x200000;
        public const uint WS_HSCROLL = 0x100000;
        public const uint WS_SYSMENU = 0x80000;
        public const uint WS_THICKFRAME = 0x40000;
        public const uint WS_GROUP = 0x20000;
        public const uint WS_TABSTOP = 0x10000;
        public const uint WS_MINIMIZEBOX = 0x20000;
        public const uint WS_MAXIMIZEBOX = 0x10000;
        public const uint WS_TILED = WS_OVERLAPPED;
        public const uint WS_ICONIC = WS_MINIMIZE;
        public const uint WS_SIZEBOX = WS_THICKFRAME;

        // Extended Window Styles
        public const uint WS_EX_DLGMODALFRAME = 0x0001;
        public const uint WS_EX_NOPARENTNOTIFY = 0x0004;
        public const uint WS_EX_TOPMOST = 0x0008;
        public const uint WS_EX_ACCEPTFILES = 0x0010;
        public const uint WS_EX_TRANSPARENT = 0x0020;
        public const uint WS_EX_MDICHILD = 0x0040;
        public const uint WS_EX_TOOLWINDOW = 0x0080;
        public const uint WS_EX_WINDOWEDGE = 0x0100;
        public const uint WS_EX_CLIENTEDGE = 0x0200;
        public const uint WS_EX_CONTEXTHELP = 0x0400;
        public const uint WS_EX_RIGHT = 0x1000;
        public const uint WS_EX_LEFT = 0x0000;
        public const uint WS_EX_RTLREADING = 0x2000;
        public const uint WS_EX_LTRREADING = 0x0000;
        public const uint WS_EX_LEFTSCROLLBAR = 0x4000;
        public const uint WS_EX_RIGHTSCROLLBAR = 0x0000;
        public const uint WS_EX_CONTROLPARENT = 0x10000;
        public const uint WS_EX_STATICEDGE = 0x20000;
        public const uint WS_EX_APPWINDOW = 0x40000;
        public const uint WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const uint WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const uint WS_EX_LAYERED = 0x00080000;
        public const uint WS_EX_NOINHERITLAYOUT = 0x00100000; // Disable inheritence of mirroring by children
        public const uint WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring
        public const uint WS_EX_COMPOSITED = 0x02000000;
        public const uint WS_EX_NOACTIVATE = 0x08000000;

        // ReSharper restore InconsistentNaming
    }

    internal static class Imports
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out Message msg, IntPtr hWnd, UInt32 msgFilterMin, UInt32 msgFilterMax, UInt32 flags);

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    /// <summary>Windows Message</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Message
    {
        public IntPtr hWnd;
        public UInt32 msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public UInt32 time;
        public Point p;
    };
}
