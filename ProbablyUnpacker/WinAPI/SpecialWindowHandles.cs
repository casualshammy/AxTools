﻿namespace AxTools.WinAPI
{
    /// <summary>
    ///     Special window handles
    /// </summary>
    internal enum SpecialWindowHandles
    {
        /// <summary>
        ///     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
        /// </summary>
        HWND_TOP = 0,
        /// <summary>
        ///     Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        HWND_BOTTOM = 1,
        /// <summary>
        ///     Places the window at the top of the Z order.
        /// </summary>
        HWND_TOPMOST = -1,
        /// <summary>
        ///     Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
        /// </summary>
        HWND_NOTOPMOST = -2
    }
}