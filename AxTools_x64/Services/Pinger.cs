using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using AxTools.Helpers;
using AxTools.Services.PingerHelpers;
using Timer = System.Timers.Timer;

namespace AxTools.Services
{
    internal static class Pinger
    {
        private static readonly Settings _settings = Settings.Instance;
        private static Timer _timer;
        private static readonly object Lock = new object();
        private static Stopwatch _stopwatch;
        private static List<PingerReply> _pingList;
        private static int _lastPing;
        private static int _lastPacketLoss;
        private const int IncorrectPing = -1;

        internal static event Action<PingerStat> StatChanged;

        /// <summary>
        ///     True if pinger is active, false otherwise
        /// </summary>
        internal static event Action<bool> IsEnabledChanged;

        internal static bool Enabled
        {
            get 
            {
                return _timer != null && _timer.Enabled;
            }
            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        private static void Start()
        {
            lock (Lock)
            {
                if (_timer != null && _timer.Enabled)
                {
                    throw new Exception("Pinger is already running!");
                }
                _pingList = Enumerable.Repeat(new PingerReply(0, true), 10).ToList();
                _stopwatch = new Stopwatch();
                _timer = new Timer(2000);
                _timer.Elapsed += TimerOnElapsed;
                _timer.Start();
                if (IsEnabledChanged != null)
                {
                    IsEnabledChanged(true);
                }
            }
        }

        private static void Stop()
        {
            lock (Lock)
            {
                if (_timer != null)
                {
                    _timer.Elapsed -= TimerOnElapsed;
                    _timer.Stop();
                    _timer.Close();
                }
                if (IsEnabledChanged != null)
                {
                    IsEnabledChanged(false);
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
                        _pingList.Add(new PingerReply((int) elapsed, result && pSocket.Connected));
                        int ping = Max(_pingList.GetRange(_pingList.Count - 10, 10).Where(l => l.Successful).Select(l => l.PingInMs));
                        int packetLoss = _pingList.Count(l => !l.Successful);
                        if (ping != _lastPing || packetLoss != _lastPacketLoss)
                        {
                            if (StatChanged != null)
                            {
                                StatChanged(new PingerStat(ping, packetLoss, ping != IncorrectPing));
                            }
                            _lastPing = ping;
                            _lastPacketLoss = packetLoss;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("[Pinger] " + ex.Message);
                }
                finally
                {
                    Monitor.Exit(Lock);
                }
            }
        }

        private static int Max(IEnumerable<int> seq)
        {
            int[] array = seq.ToArray();
            return array.Any() ? array.Max() : IncorrectPing;
        }
    }
}
