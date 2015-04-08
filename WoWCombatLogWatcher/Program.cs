using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace WoWCombatLogWatcher
{
    class Program
    {
        private static readonly FileSystemWatcher FileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WoWCombatLog.txt");
        private static readonly List<int> DateTimes = new List<int>();
        private static readonly Timer Timer = new Timer(1000);
        private static int _eventsNum = -1;
        private static int _maxEventsNum;

        static void Main()
        {
            FileSystemWatcher.IncludeSubdirectories = false;
            FileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            FileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            FileSystemWatcher.EnableRaisingEvents = true;

            Timer.Elapsed += timer_Elapsed;
            Timer.Start();

            Console.ReadLine();

            FileSystemWatcher.EnableRaisingEvents = false;
            FileSystemWatcher.Dispose();

            Timer.Stop();
            Timer.Dispose();
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int now = Environment.TickCount;
            for (int i = 0; i < DateTimes.Count; i++)
            {
                if (now - DateTimes[i] > 60000)
                {
                    DateTimes.RemoveAt(i);
                }
            }
            if (DateTimes.Count != _eventsNum)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Writes/min: " + DateTimes.Count);
                Console.WriteLine("Max writes/min: " + _maxEventsNum);
                _eventsNum = DateTimes.Count;
                if (_maxEventsNum < DateTimes.Count)
                {
                    _maxEventsNum = DateTimes.Count;
                }
            }
            
        }

        private static void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            DateTimes.Add(Environment.TickCount);
        }
    }
}
