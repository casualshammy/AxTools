using Components.WinAPI;
using System;
using System.Runtime.InteropServices;

namespace AxTools.WinAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public ABE uEdge;
        public RECT rc;
        public int lParam;
    }
}