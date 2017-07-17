using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace WoWPatternUpdater
{
    public sealed unsafe class MemoryManager : IDisposable
    {
        /// <summary>
        ///     Gets or sets the process handle.
        /// </summary>
        /// <value>
        ///     The process handle.
        /// </value>
        /// <remarks>Created 2012-02-15</remarks>
        public SafeMemHandle ProcessHandle;

        /// <summary>
        ///     Gets the process.
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        ///     Gets the image base.
        /// </summary>
        public readonly IntPtr ImageBase;

        /// <summary>
        ///     Gets a value indicating whether the process is open for memory manipulation.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the process is open for memory manipulation; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Created 2012-04-23</remarks>
        //public bool IsProcessOpen
        //{
        //    get { return ProcessHandle != null && !ProcessHandle.IsClosed && !ProcessHandle.IsInvalid; }
        //}

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemoryManager" /> class.
        /// </summary>
        /// <param name="proc">The proc.</param>
        internal MemoryManager(Process proc)
        {
            if (proc.HasExited)
            {
                throw new AccessViolationException("Process: " + proc.Id + " has already exited. Can not attach to it.");
            }
            Process.EnterDebugMode();
            Process = proc;
            const ProcessAccessFlags a = ProcessAccessFlags.PROCESS_CREATE_THREAD |
                                         ProcessAccessFlags.PROCESS_QUERY_INFORMATION |
                                         ProcessAccessFlags.PROCESS_SET_INFORMATION | ProcessAccessFlags.PROCESS_TERMINATE |
                                         ProcessAccessFlags.PROCESS_VM_OPERATION | ProcessAccessFlags.PROCESS_VM_READ |
                                         ProcessAccessFlags.PROCESS_VM_WRITE | ProcessAccessFlags.SYNCHRONIZE;
            ProcessHandle = Imports.OpenProcess(a, false, proc.Id);
            ImageBase = Process.MainModule.BaseAddress;
        }

        /// <summary>
        ///     Reads a specific number of bytes from memory.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public byte[] ReadBytes(IntPtr address, int count)
        {
            if (count != 0)
            {
                // Yes, this *can* be valid. But in 99.9999% of cases, its not. If it ever is, you should be smart enough
                // to remove this check.
                if (address == IntPtr.Zero)
                {
                    throw new ArgumentException("Address cannot be zero.", "address");
                }
                byte[] buffer = new byte[count];
                fixed (byte* buf = buffer)
                {
                    int numRead;
                    if (ReadProcessMemory(ProcessHandle, address, buf, count, out numRead) && numRead == count)
                    {
                        return buffer;
                    }
                }
                throw new AccessViolationException(string.Format("Could not read bytes from {0} [{1}]!", address.ToString("X8"), Marshal.GetLastWin32Error()));
            }
            return new byte[0];
        }

        [HandleProcessCorruptedStateExceptions]
        private T InternalRead<T>(IntPtr address) where T : struct
        {
            try
            {
                // Optimize this more. The boxing/unboxing required tends to slow this down.
                // It may be worth it to simply use memcpy to avoid it, but I doubt thats going to give any noticeable increase in speed.
                if (address == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Cannot retrieve a value at address 0");
                }

                object ret;
                switch (MarshalCache<T>.TypeCode)
                {
                    case TypeCode.Object:

                        if (MarshalCache<T>.RealType == typeof(IntPtr))
                        {
                            return (T)(object)*(IntPtr*)address;
                        }

                        // If the type doesn't require an explicit Marshal call, then ignore it and memcpy the fuckin thing.
                        if (!MarshalCache<T>.TypeRequiresMarshal)
                        {
                            T o = default(T);
                            void* ptr = MarshalCache<T>.GetUnsafePtr(ref o);

                            MoveMemory(ptr, (void*)address, MarshalCache<T>.Size);

                            return o;
                        }

                        // All System.Object's require marshaling!
                        ret = Marshal.PtrToStructure(address, typeof(T));
                        break;
                    case TypeCode.Boolean:
                        ret = *(byte*)address != 0;
                        break;
                    case TypeCode.Char:
                        ret = *(char*)address;
                        break;
                    case TypeCode.SByte:
                        ret = *(sbyte*)address;
                        break;
                    case TypeCode.Byte:
                        ret = *(byte*)address;
                        break;
                    case TypeCode.Int16:
                        ret = *(short*)address;
                        break;
                    case TypeCode.UInt16:
                        ret = *(ushort*)address;
                        break;
                    case TypeCode.Int32:
                        ret = *(int*)address;
                        break;
                    case TypeCode.UInt32:
                        ret = *(uint*)address;
                        break;
                    case TypeCode.Int64:
                        ret = *(long*)address;
                        break;
                    case TypeCode.UInt64:
                        ret = *(ulong*)address;
                        break;
                    case TypeCode.Single:
                        ret = *(float*)address;
                        break;
                    case TypeCode.Double:
                        ret = *(double*)address;
                        break;
                    case TypeCode.Decimal:
                        // Probably safe to remove this. I'm unaware of anything that actually uses "decimal" that would require memory reading...
                        ret = *(decimal*)address;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return (T)ret;
            }
            catch (AccessViolationException)
            {
                return default(T);
            }
        }

        /// <summary> Reads a value from the specified address in memory. </summary>
        /// <remarks> Created 3/26/2012. </remarks>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="address"> The address. </param>
        /// <returns> . </returns>
        public T Read<T>(IntPtr address) where T : struct
        {
            fixed (byte* b = ReadBytes(address, MarshalCache<T>.Size))
            {
                return InternalRead<T>((IntPtr)b);
            }
        }

        /// <summary> Writes a value specified to the address in memory. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="address"> The address. </param>
        /// <param name="value"> The value. </param>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Write<T>(IntPtr address, T value)
        {
            byte[] buffer;
            IntPtr hObj = Marshal.AllocHGlobal(MarshalCache<T>.Size);
            try
            {
                Marshal.StructureToPtr(value, hObj, false);

                buffer = new byte[MarshalCache<T>.Size];
                Marshal.Copy(hObj, buffer, 0, MarshalCache<T>.Size);
            }
            finally
            {
                Marshal.FreeHGlobal(hObj);
            }

            int numWritten;
            uint oldProtect;
            // Fix the protection flags to EXECUTE_READWRITE to ensure we're not going to cause issues.
            // make sure we put back the old protection when we're done!
            // dwSize should be IntPtr or UIntPtr because the underlying type is SIZE_T and varies with the platform.
            VirtualProtectEx(ProcessHandle, address, (IntPtr)MarshalCache<T>.Size, 0x40, out oldProtect);
            bool ret = WriteProcessMemory(ProcessHandle, address, buffer, MarshalCache<T>.Size, out numWritten);
            VirtualProtectEx(ProcessHandle, address, (IntPtr)MarshalCache<T>.Size, oldProtect, out oldProtect);

            return ret;
        }

        /// <summary>
        ///     Writes a set of bytes to memory.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="bytes">The bytes.</param>
        /// <returns>
        ///     Number of bytes written.
        /// </returns>
        public int WriteBytes(IntPtr address, byte[] bytes)
        {
            int numWritten;
            bool success = WriteProcessMemory(ProcessHandle, address, bytes, bytes.Length, out numWritten);
            if (!success || numWritten != bytes.Length)
            {
                throw new AccessViolationException(string.Format("Could not write the specified bytes! {0} to {1} [{2}]", bytes.Length, address.ToString("X8"), new Win32Exception(Marshal.GetLastWin32Error()).Message));
            }
            return numWritten;
        }

        /// <summary>
        ///     Allocates memory inside the opened process.
        /// </summary>
        /// <param name="size">Number of bytes to allocate.</param>
        /// <param name="allocationType">Type of memory allocation.  See <see cref="MemoryAllocationType" />.</param>
        /// <param name="protect">Type of memory protection.  See <see cref="MemoryProtectionType" /></param>
        /// <returns>Returns NULL on failure, or the base address of the allocated memory on success.</returns>
        public IntPtr AllocateMemory(int size, MemoryAllocationType allocationType = MemoryAllocationType.MEM_COMMIT, MemoryProtectionType protect = MemoryProtectionType.PAGE_EXECUTE_READWRITE)
        {
            return Imports.VirtualAllocEx(ProcessHandle, 0, size, allocationType, protect);
        }

        /// <summary>
        ///     Frees an allocated block of memory in the opened process.
        /// </summary>
        /// <param name="address">Base address of the block of memory to be freed.</param>
        /// <returns>Returns true on success, false on failure.</returns>
        /// <remarks>
        ///     Frees a block of memory using <see cref="MemoryFreeType.MEM_RELEASE" />.
        /// </remarks>
        public bool FreeMemory(IntPtr address)
        {
            return FreeMemory(address, /*size must be 0 for MEM_RELEASE*/ 0, MemoryFreeType.MEM_RELEASE);
        }

        /// <summary>
        ///     Frees an allocated block of memory in the opened process.
        /// </summary>
        /// <param name="address">Base address of the block of memory to be freed.</param>
        /// <param name="size">
        ///     Number of bytes to be freed.  This must be zero (0) if using
        ///     <see cref="MemoryFreeType.MEM_RELEASE" />.
        /// </param>
        /// <param name="freeType">Type of free operation to use.  See <see cref="MemoryFreeType" />.</param>
        /// <returns>Returns true on success, false on failure.</returns>
        public bool FreeMemory(IntPtr address, int size, MemoryFreeType freeType)
        {
            if (freeType == MemoryFreeType.MEM_RELEASE)
            {
                size = 0;
            }
            return Imports.VirtualFreeEx(ProcessHandle, address, size, freeType);
        }

        public void Dispose()
        {
            try
            {
                if (ProcessHandle != null)
                {
                    ProcessHandle.Dispose();
                    ProcessHandle = null;
                }
                try
                {
                    Process.LeaveDebugMode();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool ReadProcessMemory(SafeMemHandle hProcess, IntPtr lpBaseAddress, byte* lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static extern void MoveMemory(void* dest, void* src, int size);

        [DllImport("kernel32.dll")]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool VirtualProtectEx(SafeMemHandle hProcess, IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool WriteProcessMemory(SafeMemHandle hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
    }
}
