using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MyMemory;
using MyMemory.Hooks;

namespace Dummy
{
    class Program
    {
        private unsafe static void Main(string[] args)
        {
            Process[] wowProcesses = Process.GetProcessesByName("wow-64");
            if (wowProcesses.Length > 0)
            {
                int processID = wowProcesses[0].Id;
                using (var process = new RemoteProcess((uint) processID))
                {
                    Console.WriteLine("RemoteProcess is initialized, press any key to continue");
                    Console.ReadLine();
                    int CGWorldFrame_Render = 0x7010D0;
                    IntPtr CurrentWorldFrame = new IntPtr(0x16A3A10);
                    IntPtr CGWorldFrame__GetScreenCoordinates = new IntPtr(0x6FA560);
                    int length = 18;
                    Vector3 vector3 = new Vector3 {X = 1864f, Y = 217f, Z = 76f};
                    Vector2 vector2 = new Vector2 {X = 0f, Y = 0f};
                    IntPtr vector3Ptr = process.MemoryManager.AllocateRawMemory((uint) sizeof (Vector3));
                    process.MemoryManager.Write(vector3Ptr, vector3);
                    IntPtr vector2Ptr = process.MemoryManager.AllocateRawMemory((uint)sizeof(Vector2));
                    process.MemoryManager.Write(vector2Ptr, vector2);
                    string[] code =
                    {
                        "sub rsp, 0x28",
                        "xor r9, r9",
                        "mov [rsp+0x20], r9",
                        "mov rcx, " + (process.ModulesManager.MainModule.BaseAddress + (int)CurrentWorldFrame).ToInt64(),
                        "mov rdx, " + vector3Ptr.ToInt64(),
                        "mov r8, " + vector2Ptr.ToInt64(),
                        "xor r9, r9",
                        "call " + (process.ModulesManager.MainModule.BaseAddress + (int)CGWorldFrame__GetScreenCoordinates).ToInt64(),
                        "add rsp, 0x28",
                        "retn"
                    };

                    byte[] rcx = BitConverter.GetBytes((process.ModulesManager.MainModule.BaseAddress + (int) CurrentWorldFrame).ToInt64());
                    byte[] rdx = BitConverter.GetBytes(vector3Ptr.ToInt64());
                    byte[] r8 = BitConverter.GetBytes(vector2Ptr.ToInt64());
                    byte[] rax = BitConverter.GetBytes((process.ModulesManager.MainModule.BaseAddress + (int) CGWorldFrame__GetScreenCoordinates).ToInt64());
                    byte[] bytecode =
                    {
                        0x48, 0x83, 0xEC, 0x28,
                        0x4D, 0x31, 0xC9,
                        0x4C, 0x89, 0x4C, 0x24, 0x20,
                        0x48, 0xB9, rcx[0], rcx[1], rcx[2], rcx[3], rcx[4], rcx[5], rcx[6], rcx[7], // mov rcx, 8bytes
                        0x48, 0xBA, rdx[0], rdx[1], rdx[2], rdx[3], rdx[4], rdx[5], rdx[6], rdx[7], // mov rdx, 8bytes
                        0x49, 0xB8, r8[0], r8[1], r8[2], r8[3], r8[4], r8[5], r8[6], r8[7], // mov r8, 8bytes
                        0x4D, 0x31, 0xC9,
                        0x48, 0xB8, rax[0], rax[1], rax[2], rax[3], rax[4], rax[5], rax[6], rax[7], // movabs rax, 8bytes
                        0xFF, 0xD0, // call rax
                        0x48, 0x83, 0xC4, 0x28,
                        0xC3
                    };

                    long result = 0;
                    IntPtr address = process.ModulesManager.MainModule.BaseAddress + CGWorldFrame_Render;
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    HookJmp hook = process.HooksManager.CreateAndApplyJmpHook(address, length);
                    try
                    {
                        Console.WriteLine("Hook applied (0x{0:X}), press any key to continue...", address.ToInt64());
                        Console.ReadLine();
                        process.Yasm.Assemble(code);
                        result = hook.Executor.ExecuteByteCode<long>(bytecode);
                        Console.WriteLine("Vector3: " + vector3 + "; Vector2: " + vector2);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!!! ERROR: " + ex.Message);
                    }
                    Console.WriteLine("Code is executed, result: {0}; time (ms): {1}", result, stopwatch.ElapsedMilliseconds);
                    Console.WriteLine("Press any key to clealup and quit");
                    hook.Remove();
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("ERROR: no WoW-64 process found");
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Vector3
        {
            internal float X;
            internal float Y;
            internal float Z;

            public override string ToString()
            {
                return string.Format("{{ {0}, {1}, {2} }}", X, Y, Z);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Vector2
        {
            internal float X;
            internal float Y;

            public override string ToString()
            {
                return string.Format("{{ {0}, {1} }}", X, Y);
            }
        }
    }
}
