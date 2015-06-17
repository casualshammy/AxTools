using System;
using System.Timers;
using AxTools.WinAPI;

namespace AxTools.Services
{
    internal static class Clicker
    {
        private static Timer _timer;
        private static IntPtr _key;
        private static readonly object Lock = new object();
        internal static IntPtr Handle { get; private set; }

        internal static void Start(double interval, IntPtr hwnd, IntPtr keyToPress)
        {
            lock (Lock)
            {
                if (_timer == null)
                {
                    _timer = new Timer(interval);
                    _timer.Elapsed += timer_Elapsed;
                    _key = keyToPress;
                    Handle = hwnd;
                    _timer.Start();
                }
            }
        }

        internal static void Stop()
        {
            lock (Lock)
            {
                if (_timer != null)
                {
                    _timer.Elapsed -= timer_Elapsed;
                    _timer.Stop();
                    _timer.Close();
                    _timer = null;
                }
            }
        }

        internal static bool Enabled
        {
            get
            {
                return _timer != null && _timer.Enabled;
            }
        }

        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NativeMethods.PostMessage(Handle, WM_MESSAGE.WM_KEYDOWN, _key, IntPtr.Zero);
            NativeMethods.PostMessage(Handle, WM_MESSAGE.WM_KEYUP, _key, IntPtr.Zero);
        }

    }
}
