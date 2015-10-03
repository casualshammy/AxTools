using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMemory;
using MyMemory.Hooks;

namespace Dummy
{
    class Program
    {
        static void Main(string[] args)
        {
            int processID = Process.GetProcessesByName("wow-64")[0].Id;
            using (var process = new RemoteProcess((uint) processID))
            {
                Console.WriteLine("RemoteProcess is initialized, press any key to continue");
                Console.ReadLine();
                int CGWorldFrame_Render = 0x7010D0;
                int length = 18;
                string[] code = { "mov rax, 698", "retn" };
                long result = 0;
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    IntPtr address = process.ModulesManager.MainModule.BaseAddress + CGWorldFrame_Render;
                    HookJmp hook = process.HooksManager.CreateAndApplyJmpHook(address, length);
                    Console.WriteLine("Hook applied (0x{0:X}), press any key to continue...", address.ToInt64());
                    Console.ReadLine();
                    result = hook.Executor.Execute<long>(code);
                    hook.Remove();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("!!! ERROR: " + ex.Message);
                }

                Console.WriteLine("Code is executed, result: {0}; time (ms): {1}", result, stopwatch.ElapsedMilliseconds);
                Console.WriteLine("Press any key to clealup and quit");
                Console.ReadLine();
            }
        }
    }
}
