using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace AxTools.Services
{
    internal static class HotkeyManager
    {
        private static readonly Timer _timer = new Timer(50);
        private static KeyExt[] _uniqKeys;
        private static readonly object _locker = new object();
        private static int _intLocker;
        private static readonly Dictionary<string, Keys[]> KeysSets = new Dictionary<string, Keys[]>();

        /// <summary>
        ///     ATTENTION!
        ///     <para>As this event is static, it raise with all registered Keys, from all identifiers</para>
        /// </summary>
        internal static event Action<Keys> KeyPressed;

        static HotkeyManager()
        {
            _timer.Elapsed += TimerOnElapsed;
        }

        /// <summary>
        ///     Add Keys to listener
        /// </summary>
        /// <param name="id">Unique identifier</param>
        /// <param name="keysToHandle">Set of Keys to listen to</param>
        internal static void AddKeys(string id, params Keys[] keysToHandle)
        {
            lock (_locker)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("string.IsNullOrWhiteSpace(id) must return false", "id");
                }
                if (KeysSets.Keys.Contains(id))
                {
                    throw new ArgumentException("ID is already present", "id");
                }
                if (keysToHandle == null || keysToHandle.Length == 0)
                {
                    throw new ArgumentException("You should specify at least one key", "keysToHandle");
                }
                _timer.Enabled = false;
                while (_intLocker != 0)
                {
                    Thread.Sleep(1);
                }
                KeysSets[id] = keysToHandle;
                RebuildUniqueKeys();
                _timer.Enabled = true;
            }
        }

        /// <summary>
        ///     Removes set of Keys with certain identifier from listener
        /// </summary>
        /// <returns>True if identifier is present, false otherwise</returns>
        internal static bool RemoveKeys(string id)
        {
            lock (_locker)
            {
                if (KeysSets.ContainsKey(id))
                {
                    _timer.Enabled = false;
                    while (_intLocker != 0)
                    {
                        Thread.Sleep(1);
                    }
                    KeysSets.Remove(id);
                    RebuildUniqueKeys();
                    if (_uniqKeys.Length != 0)
                    {
                        _timer.Enabled = true;
                    }
                    return true;
                }
                return false;
            }
        }

        private static void RebuildUniqueKeys()
        {
            HashSet<Keys> hashSet = new HashSet<Keys>();
            foreach (Keys key in KeysSets.Values.SelectMany(keys => keys))
            {
                hashSet.Add(key);
            }
            _uniqKeys = hashSet.Select(l => new KeyExt(l)).ToArray();
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Interlocked.Increment(ref _intLocker);
            try
            {
                foreach (KeyExt keyExt in _uniqKeys)
                {
                    bool pressed = (GetAsyncKeyState(keyExt.Key) & 0x8000) != 0;
                    if (pressed && !keyExt.Pressed && KeyPressed != null)
                    {
                        if (keyExt.Alt)
                        {
                            if ((GetAsyncKeyState(Keys.Menu) & 0x8000) != 0)
                            {
                                KeyPressed(keyExt.Key | Keys.Alt);
                            }
                        }
                        else if (keyExt.Ctrl)
                        {
                            if ((GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0)
                            {
                                KeyPressed(keyExt.Key | Keys.Control);
                            }
                        }
                        else if (keyExt.Shift)
                        {
                            if ((GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0)
                            {
                                KeyPressed(keyExt.Key | Keys.Shift);
                            }
                        }
                        else
                        {
                            KeyPressed(keyExt.Key);
                        }
                    }
                    keyExt.Pressed = pressed;
                }
            }
            finally
            {
                Interlocked.Decrement(ref _intLocker);
            }
        }

        private class KeyExt
        {
            internal readonly Keys Key;
            internal bool Pressed;
            internal readonly bool Shift;
            internal readonly bool Alt;
            internal readonly bool Ctrl;

            internal KeyExt(Keys pKey)
            {
                Key = pKey;
                Pressed = false;
                if ((Key & Keys.Alt) == Keys.Alt)
                {
                    Alt = true;
                }
                if ((Key & Keys.Control) == Keys.Control)
                {
                    Ctrl = true;
                }
                if ((Key & Keys.Shift) == Keys.Shift)
                {
                    Shift = true;
                }
                Key = Key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;   
            }

        }

        [DllImport("user32.dll")]
        private static extern ushort GetAsyncKeyState(Keys vKey);

    }
}
