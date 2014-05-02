using System;
using System.Timers;
using AxTools.Classes.WinAPI;

namespace AxTools.Classes
{
    class Clicker : IDisposable
    {
        private readonly Timer timer;
        private IntPtr handle;
        private IntPtr key;

        internal Clicker()
        {
            timer = new Timer();
            timer.Elapsed += timer_Elapsed;
        }

        internal void Start(double interval, IntPtr hwnd, IntPtr keyToPress)
        {
            key = keyToPress;
            handle = hwnd;
            timer.Interval = interval;
            timer.Start();
        }

        internal void Stop()
        {
            timer.Stop();
        }

        internal bool Enabled
        {
            get
            {
                return timer.Enabled;
            }
        }

        internal IntPtr Handle
        {
            get
            {
                return handle;
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NativeMethods.PostMessage(handle, WM_MESSAGE.WM_KEYDOWN, key, IntPtr.Zero);
            NativeMethods.PostMessage(handle, WM_MESSAGE.WM_KEYUP, key, IntPtr.Zero);
        }

        public void Dispose()
        {
            timer.Elapsed -= timer_Elapsed;
            timer.Close();
        }
    }
}
