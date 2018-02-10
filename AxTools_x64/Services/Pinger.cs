using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using AxTools.Helpers;
using AxTools.Services.PingerHelpers;
using AxTools.WinAPI.TCPTable;
using Timer = System.Timers.Timer;

namespace AxTools.Services
{
    internal static class Pinger
    {
        private static readonly Log2 log = new Log2("Pinger");
        private static readonly Settings2 _settings = Settings2.Instance;
        private static Timer _timer;
        private static readonly object Lock = new object();
        private static Stopwatch _stopwatch;
        private static List<PingerReply> _pingList;
        private static int _lastPing;
        private static int _lastPacketLoss;
        private const int IncorrectPing = -1;
        private static SrvAddress _cachedServer = new SrvAddress(string.Empty, 0, string.Empty);
        private static readonly Timer AutodetectedIPUpdateTimer = new Timer(60000);

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
                AutodetectedIPUpdateTimer.Elapsed += AutodetectedIPUpdateTimerOnElapsed;
                AutodetectedIPUpdateTimer.Start();
                AutodetectedIPUpdateTimerOnElapsed(null, null);
                _pingList = Enumerable.Repeat(new PingerReply(0, true), 10).ToList();
                _stopwatch = new Stopwatch();
                _timer = new Timer(2000);
                _timer.Elapsed += TimerOnElapsed;
                _timer.Start();
                IsEnabledChanged?.Invoke(true);
            }
        }

        private static void Stop()
        {
            lock (Lock)
            {
                if (_timer != null)
                {
                    AutodetectedIPUpdateTimer.Elapsed -= AutodetectedIPUpdateTimerOnElapsed;
                    AutodetectedIPUpdateTimer.Stop();
                    _timer.Elapsed -= TimerOnElapsed;
                    _timer.Stop();
                    _timer.Close();
                }
                IsEnabledChanged?.Invoke(false);
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
                        bool result = pSocket.BeginConnect(_cachedServer.Ip, _cachedServer.Port, null, null).AsyncWaitHandle.WaitOne(1000, false);
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
                            StatChanged?.Invoke(new PingerStat(ping, packetLoss, ping != IncorrectPing));
                            _lastPing = ping;
                            _lastPacketLoss = packetLoss;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
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

        private static string GetWoWRemoteIP()
        {
            Process[] wowProcesses = Process.GetProcessesByName("Wow-64");
            TCPConnectionInfo connectionInfo = TCPConnectionInfo.GetAllRemoteTcpConnections().FirstOrDefault(l => l.EndPoint.Port == 3724 && wowProcesses.Any(k => k.Id == l.ProcessID));
            return connectionInfo?.EndPoint.Address.ToString();
        }

        private static void AutodetectedIPUpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (GameServers.Entries[_settings.PingerServerID].Description == GameServers.WoWAutodetect)
            {
                string remoteIP = GetWoWRemoteIP();
                if (remoteIP != null)
                {
                    SrvAddress newRemoteAddress = new SrvAddress(remoteIP, GameServers.Entries[_settings.PingerServerID].Port, "Autodetected WoW server");
                    if (_cachedServer.Ip != newRemoteAddress.Ip)
                    {
                        _cachedServer = newRemoteAddress;
                        _settings.PingerLastWoWServerIP = remoteIP;
                        log.Info("WoW remote IP is detected: " + remoteIP);
                    }
                }
                else
                {
                    SrvAddress srvAddress = new SrvAddress(_settings.PingerLastWoWServerIP, 3724, "Last seen WoW server");
                    if (srvAddress.Ip != _cachedServer.Ip)
                    {
                        _cachedServer = srvAddress;
                        log.Info("Can't find WoW remote IP address, using last seen IP: " + _cachedServer.Ip);
                    }
                }
            }
            else
            {
                _cachedServer = GameServers.Entries[_settings.PingerServerID];
            }
        }

    }
}
