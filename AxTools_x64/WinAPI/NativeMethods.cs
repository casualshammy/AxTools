using AxTools.Helpers.MemoryManagement;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace AxTools.WinAPI
{
    internal static class NativeMethods
    {
        //[DllImport("user32", EntryPoint = "GetWindowLongA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        //internal static extern int GetWindowLong32(IntPtr hwnd, int nIndex);

        //[DllImport("user32", EntryPoint = "SetWindowLongA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        //internal static extern int SetWindowLong32(IntPtr hwnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        internal static extern long GetWindowLong64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        internal static extern long SetWindowLong64(IntPtr hWnd, int nIndex, long dwNewLong);

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
        
        [DllImportAttribute("winmm.dll", EntryPoint = "sndPlaySoundW")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool sndPlaySoundW([InAttribute] [MarshalAsAttribute(UnmanagedType.LPWStr)] string pszSound, uint fuSound);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern unsafe int memcmp(byte* b1, byte* b2, UIntPtr count);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool VirtualProtectEx(SafeMemoryHandle hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

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
