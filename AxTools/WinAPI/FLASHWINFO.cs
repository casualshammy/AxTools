using System;
using System.Runtime.InteropServices;

namespace AxTools.Classes.WinAPI
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct FLASHWINFO
    {
        /// <summary>
        /// The size of the structure in bytes.
        /// </summary>
        internal uint cbSize;
        /// <summary>
        /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
        /// </summary>
        internal IntPtr hwnd;
        /// <summary>
        /// The Flash Status.
        /// </summary>
        internal FlashWindowFlags dwFlags; //uint
        /// <summary>
        /// The number of times to Flash the window.
        /// </summary>
        internal uint uCount;
        /// <summary>
        /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
        /// </summary>
        internal uint dwTimeout;
    }
}