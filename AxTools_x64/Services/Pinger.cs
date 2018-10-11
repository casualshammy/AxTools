using AxTools.Helpers;
using AxTools.Services.PingerHelpers;
using AxTools.WinAPI.TCPTable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AxTools.Services
{
    internal static class Pinger
    {
        private const int INTERVAL = 5000;
        private static readonly Log2 log = new Log2("Pinger");
        private static readonly Settings2 _settings = Settings2.Instance;
        private static Task timerTask;
        private static volatile bool timerTaskIsRunning = false;
        private static readonly object Lock = new object();
        private static Stopwatch _stopwatch;
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
            get => timerTask != null && timerTaskIsRunning;
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
                if (Enabled)
                {
                    throw new Exception("Pinger is already running!");
                }
                AutodetectedIPUpdateTimer.Elapsed += AutodetectedIPUpdateTimerOnElapsed;
                AutodetectedIPUpdateTimer.Start();
                AutodetectedIPUpdateTimerOnElapsed(null, null);
                _stopwatch = new Stopwatch();
                timerTaskIsRunning = true;
                timerTask = Task.Run(() => {
                    while (timerTaskIsRunning)
                    {
                        TimerOnElapsed();
                        Thread.Sleep(INTERVAL);
                    }
                });
                IsEnabledChanged?.Invoke(true);
            }
        }

        private static void Stop()
        {
            lock (Lock)
            {
                if (timerTask != null)
                {
                    AutodetectedIPUpdateTimer.Elapsed -= AutodetectedIPUpdateTimerOnElapsed;
                    AutodetectedIPUpdateTimer.Stop();
                    timerTaskIsRunning = false;
                    timerTask.Wait(INTERVAL * 2);
                    timerTask.Dispose();
                    timerTask = null;
                }
                IsEnabledChanged?.Invoke(false);
            }
        }

        private static void TimerOnElapsed()
        {
            try
            {
                int failedNum = 0;
                int maxPing = 0;
                for (int i = 0; i < 10; i++)
                {
                    using (Socket pSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        _stopwatch.Restart();
                        var result = pSocket.BeginConnect(_cachedServer.Ip, _cachedServer.Port, null, null).AsyncWaitHandle.WaitOne(1000, false);
                        var elapsed = _stopwatch.ElapsedMilliseconds;
                        if (result && pSocket.Connected)
                            maxPing = Math.Max(maxPing, (int)elapsed);
                        else
                            failedNum++;
                    }
                }
                StatChanged?.Invoke(new PingerStat(maxPing, failedNum));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        private static int Max(IEnumerable<int> seq)
        {
            int[] array = seq.ToArray();
            return array.Any() ? array.Max() : IncorrectPing;
        }

        private static string GetWoWRemoteIP()
        {
            Process[] wowProcesses = Process.GetProcessesByName("Wow");
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