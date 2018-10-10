using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AxTools.Helpers
{
    internal static class ProcessManager
    {
        private static readonly Dictionary<int, Tuple<int, string>> processHashes = new Dictionary<int, Tuple<int, string>>();
        private static readonly Timer timer = new Timer(1000);
        private static int numSubscribers = 0;
        private static Action<ProcessManagerEvent> processStarted;
        private static Action<ProcessManagerEvent> processStopped;

        static ProcessManager()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Process[] processes = Process.GetProcesses();
            List<int> actualHashes = new List<int>();
            foreach (var process in processes)
            {
                int hash = process.Id ^ process.ProcessName.GetHashCode();
                actualHashes.Add(hash);
                if (!processHashes.ContainsKey(hash))
                {
                    try
                    {
                        string processName = Path.GetFileName(process.MainModule.FileName);
                        processStarted?.Invoke(new ProcessManagerEvent(process.Id, processName));
                        processHashes.Add(hash, new Tuple<int, string>(process.Id, processName));
                    }
                    catch
                    {
                        // can't get main module, skipping process
                    }
                }
                process.Dispose();
            }
            //for (int i = processes.Length-1; i >= 0; i--)
            //{
            //    Process process = processes[i];
            //    int hash = process.Id ^ process.ProcessName.GetHashCode();
            //    actualHashes.Add(hash);
            //    if (!processHashes.ContainsKey(hash))
            //    {
            //        try
            //        {
            //            string processName = Path.GetFileName(process.MainModule.FileName);
            //            processStarted?.Invoke(new ProcessManagerEvent(process.Id, processName));
            //            processHashes.Add(hash, new Tuple<int, string>(process.Id, processName));
            //        }
            //        catch
            //        {
            //            // can't get main module, skipping process
            //        }
            //    }
            //    process.Dispose();
            //}
            foreach (var hash in processHashes.Keys.Except(actualHashes).ToArray())
            {
                processStopped?.Invoke(new ProcessManagerEvent(processHashes[hash].Item1, processHashes[hash].Item2));
                processHashes.Remove(hash);
            }
        }

        public static event Action<ProcessManagerEvent> ProcessStarted
        {
            add
            {
                numSubscribers++;
                if (numSubscribers == 1)
                {
                    timer.Start();
                    Timer_Elapsed(null, null);
                }
                processStarted += value;
            }
            remove
            {
                numSubscribers--;
                if (numSubscribers == 0)
                    timer.Stop();
                processStarted -= value;
            }
        }

        public static event Action<ProcessManagerEvent> ProcessStopped
        {
            add
            {
                numSubscribers++;
                if (numSubscribers == 1)
                {
                    timer.Start();
                    Timer_Elapsed(null, null);
                }
                processStopped += value;
            }
            remove
            {
                numSubscribers--;
                if (numSubscribers == 0)
                    timer.Stop();
                processStopped -= value;
            }
        }

    }

    internal class ProcessManagerEvent
    {
        public int ProcessId;
        public string ProcessName;

        public ProcessManagerEvent(int id, string name)
        {
            ProcessId = id;
            ProcessName = name;
        }
    }

}
