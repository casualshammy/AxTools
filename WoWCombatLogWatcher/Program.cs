using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace WoWCombatLogWatcher
{
    class Program
    {
        private static FileSystemWatcher fileSystemWatcher = new FileSystemWatcher("C:\\Program Files (x86)\\World of Warcraft\\Logs", "WoWCombatLog.txt");
        private static List<int> dateTimes = new List<int>();
        private static Timer timer = new Timer(1000);
        private static int eventsNum = -1;

        static void Main(string[] args)
        {
            fileSystemWatcher.IncludeSubdirectories = false;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            fileSystemWatcher.EnableRaisingEvents = true;

            timer.Elapsed += timer_Elapsed;
            timer.Start();

            Console.ReadLine();

            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();

            timer.Stop();
            timer.Dispose();
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int now = Environment.TickCount;
            for (int i = 0; i < dateTimes.Count; i++)
            {
                if (now - dateTimes[i] > 60000)
                {
                    dateTimes.RemoveAt(i);
                }
            }
            if (dateTimes.Count != eventsNum)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Writes/min: " + dateTimes.Count);
                eventsNum = dateTimes.Count;
            }
            
        }

        private static void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            dateTimes.Add(Environment.TickCount);
        }
    }
}
