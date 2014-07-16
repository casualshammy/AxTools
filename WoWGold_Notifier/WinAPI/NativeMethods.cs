using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WoWGold_Notifier.WinAPI
{
    internal static class NativeMethods
    {
        [DllImport("user32", EntryPoint = "GetWindowLongA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32", EntryPoint = "SetWindowLongA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

        [DllImport("user32", EntryPoint = "SetWindowPos", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags wFlags);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32", EntryPoint = "PostMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern bool PostMessage(IntPtr hwnd, WM_MESSAGE wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        static internal extern IntPtr GetForegroundWindow();

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [SuppressUnmanagedCodeSecurity, DllImport("kernel32")]
        internal static extern IntPtr LoadLibrary(string libraryName);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("winmm.dll", EntryPoint = "sndPlaySoundW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool sndPlaySoundW([In] [MarshalAs(UnmanagedType.LPWStr)] string pszSound, uint fuSound);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // ReSharper disable InconsistentNaming
        internal static int WS_CAPTION = 0xC00000;
        internal static int WS_THICKFRAME = 0x40000;
        internal static int GWL_STYLE = -16;
        internal static int GWL_EXSTYLE = -20;
        internal static int WS_EX_LAYERED = 0x80000;
        internal static int WS_EX_TRANSPARENT = 0x20;
        internal static uint LWA_ALPHA = 0x2;
        // ReSharper restore InconsistentNaming
    }
}
