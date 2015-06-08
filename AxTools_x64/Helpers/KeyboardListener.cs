using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using AxTools.Classes;
using Timer = System.Timers.Timer;

namespace AxTools.Helpers
{
    internal static class KeyboardListener
    {
        private static readonly Timer _timer;
        private static KeyExt[] _keys;
        private static readonly Stopwatch _stopwatch;

        /// <summary>
        ///     Key pressed
        /// </summary>
        internal static event Action<Keys> KeyPressed;

        static KeyboardListener()
        {
            _stopwatch = new Stopwatch();
            _timer = new Timer(50);
            _timer.Elapsed += TimerOnElapsed;
        }

        /// <summary>
        ///     Start keyboar listening
        /// </summary>
        /// <param name="keyArray">Array of <see cref="System.Windows.Forms.Keys"/> to handle</param>
        internal static void Start(Keys[] keyArray)
        {
            List<KeyExt> pKeyExts = new List<KeyExt>();
            foreach (Keys key in keyArray)
            {
                if (pKeyExts.All(i => i.Key != key))
                {
                    pKeyExts.Add(new KeyExt(key));
                }
            }
            _keys = pKeyExts.ToArray();
            _timer.Start();
        }

        /// <summary>
        ///     Stop keyboard listener
        /// </summary>
        internal static void Stop()
        {
            _timer.Stop();
            Log.Print("[KeyboardListener] Total CPU time: " + _stopwatch.ElapsedMilliseconds + "ms", true);
        }

        /// <summary>
        ///     Changes one key to another
        /// </summary>
        /// <param name="delete">Key to delete</param>
        /// <param name="add">Key to add</param>
        internal static void ChangeKey(Keys delete, Keys add)
        {
            for (int i = 0; i < _keys.Length; i++)
            {
                if (_keys[i].Key == delete)
                {
                    _keys[i] = new KeyExt(add);
                    break;
                }
            }
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _stopwatch.Start();
            foreach (KeyExt keyExt in _keys)
            {
                bool pressed = (GetAsyncKeyState((int) keyExt.Key) & 0x8000) != 0;
                if (pressed && !keyExt.Pressed && KeyPressed != null)
                {
                    KeyPressed(keyExt.Key);
                }
                keyExt.Pressed = pressed;
            }
            _stopwatch.Stop();
        }

        private class KeyExt
        {
            internal readonly Keys Key;
            internal bool Pressed;

            internal KeyExt(Keys pKey)
            {
                Key = pKey;
                Pressed = false;
            }

        }

        [DllImport("user32.dll")]
        private static extern ushort GetAsyncKeyState(int vKey);

    }
}
