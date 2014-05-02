using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ATProject
{
    public class Program
    {
        private static Thread _loopThread;
        private static readonly bool ShouldLoop = true;
        private static Process _thisProcess;

        private static MemoryMappedFile _memoryMappedFile;
        private static MemoryMappedViewAccessor _mmfViewAccessor;

        internal enum MMFCommand
        {
            None = 0,
            SelectTarget = 1,
        }

        internal struct MMFStruct
        {

            internal MMFStruct(int pid, MMFCommand mmfCommand, ulong arg0)
            {
                Command = mmfCommand;
                Arg0 = arg0;
                ProcessID = pid;
            }

            internal int ProcessID;

            internal MMFCommand Command;

            internal ulong Arg0;

        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SelectTargetDelegate(ulong guid);
        public static SelectTargetDelegate SelectTarget;

        public static int DllMain(string args)
        {
            _thisProcess = Process.GetCurrentProcess();
            IntPtr imageBase = _thisProcess.MainModule.BaseAddress;
            _memoryMappedFile = MemoryMappedFile.CreateOrOpen("AxTools_MMF", 1024);
            _mmfViewAccessor = _memoryMappedFile.CreateViewAccessor();
            _mmfMutex = new Mutex(false, "AxTools_MMF_Mutex");
            try
            {
                SelectTarget = Marshal.GetDelegateForFunctionPointer(imageBase + 0x8CE477, typeof (SelectTargetDelegate)) as SelectTargetDelegate;
            }
            catch (Exception ex)
            {
                LogPrint("SelectTarget error: " + ex.Message);
                return 0;
            }
            while (ShouldLoop)
            {
                try
                {
                    //_mmfMutex.WaitOne();
                    //MMFStruct data;
                    //_mmfViewAccessor.Read(0, out data);
                    //if (data.Command != MMFCommand.None && data.ProcessID == _thisProcess.Id)
                    //{
                    //    ProcessClientMessage(data);
                    //    MMFStruct eraser = new MMFStruct(0, MMFCommand.None, 0);
                    //    _mmfViewAccessor.Write(0, ref eraser);
                    //}
                    SelectTarget(0xF130C3DB00006FD7);
                }
                catch (Exception ex)
                {
                    LogPrint("G_ERROR: " + ex.Message);
                }
                finally
                {
                    //_mmfMutex.ReleaseMutex();
                }
                Thread.Sleep(1000);
            }
            OnExit();
            return 0;
        }

        private static Mutex _mmfMutex;

        private static void LoopProc()
        {
            while (ShouldLoop)
            {
                try
                {
                    //_mmfMutex.WaitOne();
                    //MMFStruct data;
                    //_mmfViewAccessor.Read(0, out data);
                    //if (data.Command != MMFCommand.None && data.ProcessID == _thisProcess.Id)
                    //{
                    //    ProcessClientMessage(data);
                    //    MMFStruct eraser = new MMFStruct(0, MMFCommand.None, 0);
                    //    _mmfViewAccessor.Write(0, ref eraser);
                    //}
                    SelectTarget(0xF130C3DB00006FD7);
                }
                catch (Exception ex)
                {
                    LogPrint("G_ERROR: " + ex.Message);
                }
                finally
                {
                    //_mmfMutex.ReleaseMutex();
                }
                Thread.Sleep(1000);
            }
        }

        private static void ProcessClientMessage(MMFStruct mmfStruct)
        {
            LogPrint(mmfStruct.Command + "/" + mmfStruct.Arg0);
            switch (mmfStruct.Command)
            {
                case MMFCommand.None:
                    return;
                case MMFCommand.SelectTarget:
                    SelectTarget(mmfStruct.Arg0);
                    return;
            }
        }

        private static void LogPrint(string message)
        {
            File.AppendAllText("1.txt", DateTime.UtcNow + " :: " + message + "\r\n", Encoding.UTF8);
        }

        private static void OnExit()
        {
            _mmfViewAccessor.Dispose();
            AppDomain.Unload(AppDomain.CurrentDomain);
        }

    }
}
