using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using AxTools.Classes;
using Timer = System.Timers.Timer;

namespace AxTools.Services
{
    internal static class Pinger
    {
        private static readonly Settings _settings = Settings.Instance;
        private static Timer _timer;
        private static readonly object Lock = new object();
        private static Stopwatch _stopwatch;

        private static List<int> _pingList;
        private static int _lastPing;
        private static int _lastPacketLoss;

        /// <summary>
        ///     The first parameter is ping
        ///     The second is packet loss
        /// </summary>
        internal static event Action<int, int> DataChanged;

        /// <summary>
        ///     True if pinger is active, false otherwise
        /// </summary>
        internal static event Action<bool> StateChanged;

        internal static bool Enabled
        {
            get 
            {
                return _timer != null && _timer.Enabled;
            }
        }

        internal static void Start()
        {
            lock (Lock)
            {
                if (_timer != null && _timer.Enabled)
                {
                    throw new Exception("Pinger is already running!");
                }
                _pingList = new List<int>(100) { -2, -2, -2, -2, -2, -2, -2, -2, -2, -2 };
                _stopwatch = new Stopwatch();
                _timer = new Timer(2000);
                _timer.Elapsed += TimerOnElapsed;
                _timer.Start();
                if (StateChanged != null)
                {
                    StateChanged(true);
                }
            }
        }

        internal static void Stop()
        {
            lock (Lock)
            {
                if (_timer != null)
                {
                    _timer.Elapsed -= TimerOnElapsed;
                    _timer.Stop();
                    _timer.Close();
                }
                if (StateChanged != null)
                {
                    StateChanged(false);
                }
            }
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (Monitor.TryEnter(Lock))
            {
                try
                {
                    using (Socket pSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        _stopwatch.Restart();
                        bool result = pSocket.BeginConnect(Globals.GameServers[_settings.PingerServerID].Ip, Globals.GameServers[_settings.PingerServerID].Port, null, null).AsyncWaitHandle.WaitOne(1000, false);
                        long elapsed = _stopwatch.ElapsedMilliseconds;
                        if (_pingList.Count == 100)
                        {
                            _pingList.RemoveAt(0);
                        }
                        _pingList.Add((int)(!result || !pSocket.Connected ? -1 : elapsed));
                        int ping = _pingList.GetRange(_pingList.Count - 10, 10).Max();
                        int packetLoss = _pingList.Count(x => x == -1);
                        if (ping != _lastPing || packetLoss != _lastPacketLoss)
                        {
                            if (DataChanged != null)
                            {
                                DataChanged(ping, packetLoss);
                            }
                            _lastPing = ping;
                            _lastPacketLoss = packetLoss;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Print("[Pinger] " + ex.Message, true);
                }
                finally
                {
                    Monitor.Exit(Lock);
                }
            }
        }
    
    }
}
