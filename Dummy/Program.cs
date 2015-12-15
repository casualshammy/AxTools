using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MyMemory;
using MyMemory.Hooks;
using Newtonsoft.Json;

namespace Dummy
{
    class Program
    {
        private unsafe static void Main(string[] args)
        {
            IntPtr handle = GetForegroundWindow();
            PostMessage(handle, WM_MESSAGE.WM_KEYDOWN, (IntPtr) 0xB0, IntPtr.Zero);
            PostMessage(handle, WM_MESSAGE.WM_KEYUP, (IntPtr) 0xB0, IntPtr.Zero);
            Console.ReadLine();
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32", EntryPoint = "PostMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern bool PostMessage(IntPtr hwnd, WM_MESSAGE wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        static internal extern IntPtr GetForegroundWindow();
    }

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
