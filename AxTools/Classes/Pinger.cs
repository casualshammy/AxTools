﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using AxTools.Forms;
using Timer = System.Timers.Timer;

namespace AxTools.Classes
{
    internal static class Pinger
    {
        private static Timer _timer;
        private static readonly object Lock = new object();
        private static Stopwatch _stopwatch;

        private static List<int> _pingList;
        private static int _lastPing;
        private static int _lastPacketLoss;

        internal static void Start()
        {
            lock (Lock)
            {
                _pingList = new List<int>(100) { -2, -2, -2, -2, -2, -2, -2, -2, -2, -2 };
                _stopwatch = new Stopwatch();
                _timer = new Timer(2000);
                _timer.Elapsed += TimerOnElapsed;
                _timer.Start();
                MainForm.Instance.Pinger_DataChanged(-1, 1);
            }
        }

        internal static void Stop()
        {
            lock (Lock)
            {
                _timer.Elapsed -= TimerOnElapsed;
                _timer.Stop();
                _timer.Close();
                MainForm.Instance.Pinger_DataChanged(-1, 0);
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
                        bool result = pSocket.BeginConnect(Settings.GameServer.Ip, Settings.GameServer.Port, null, null).AsyncWaitHandle.WaitOne(1000, false);
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
                            MainForm.Instance.Pinger_DataChanged(ping, packetLoss);
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