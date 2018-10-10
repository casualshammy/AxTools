using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace AxTools.Helpers
{
    internal class MemoryHelper
    {
        internal readonly IntPtr ImageBase;
        internal readonly SafeMemoryHandle ProcessHandle;
        
        internal MemoryHelper(Process externalProcess)
        {
            ProcessHandle = OpenProcess(ProcessAccessFlags.SYNCHRONIZE | ProcessAccessFlags.PROCESS_CREATE_THREAD | ProcessAccessFlags.PROCESS_QUERY_INFORMATION | ProcessAccessFlags.PROCESS_SET_INFORMATION | ProcessAccessFlags.PROCESS_TERMINATE | ProcessAccessFlags.PROCESS_VM_OPERATION | ProcessAccessFlags.PROCESS_VM_READ | ProcessAccessFlags.PROCESS_VM_WRITE, false, externalProcess.Id);
            ImageBase = externalProcess.MainModule.BaseAddress;
        }

        public int WriteBytes(IntPtr address, byte[] bytes)
        {
            int lpNumberOfBytesWritten;
            if (!WriteProcessMemory(ProcessHandle, address, bytes, bytes.Length, out lpNumberOfBytesWritten) || lpNumberOfBytesWritten != bytes.Length)
            {
                throw new AccessViolationException(string.Format("Could not write the specified bytes! {0} to {1} [{2}]", bytes.Length, address.ToString("X8"), new Win32Exception(Marshal.GetLastWin32Error()).Message));
            }
            return lpNumberOfBytesWritten;
        }

        public bool Write<T>(IntPtr address, T value)
        {
            IntPtr num = Marshal.AllocHGlobal(MarshalCache<T>.Size);
            byte[] numArray;
            try
            {
                Marshal.StructureToPtr(value, num, false);
                numArray = new byte[MarshalCache<T>.Size];
                Marshal.Copy(num, numArray, 0, MarshalCache<T>.Size);
            }
            finally
            {
                Marshal.FreeHGlobal(num);
            }
            uint lpflOldProtect;
            VirtualProtectEx(ProcessHandle, address, (IntPtr)MarshalCache<T>.Size, 64U, out lpflOldProtect);
            int lpNumberOfBytesWritten;
            bool flag = WriteProcessMemory(ProcessHandle, address, numArray, MarshalCache<T>.Size, out lpNumberOfBytesWritten);
            VirtualProtectEx(ProcessHandle, address, (IntPtr)MarshalCache<T>.Size, lpflOldProtect, out lpflOldProtect);
            return flag;
        }

        public unsafe byte[] ReadBytes(IntPtr address, int count)
        {
            if (count == 0)
                return new byte[0];
            if (address == IntPtr.Zero)
                throw new ArgumentException("Address cannot be zero.", "address");
            byte[] numArray = new byte[count];
            fixed (byte* lpBuffer = numArray)
            {
                int lpNumberOfBytesRead;
                if (ReadProcessMemory(ProcessHandle, address, lpBuffer, count, out lpNumberOfBytesRead) && lpNumberOfBytesRead == count)
                    return numArray;
            }
            throw new AccessViolationException(string.Format("Could not read bytes from {0} [{1}]!", address.ToString("X8"), Marshal.GetLastWin32Error()));
        }

        public new unsafe int ReadBytes(IntPtr address, void* buffer, int count)
        {
            int lpBytesRead;
            if (!ReadProcessMemory(this.ProcessHandle, address, new IntPtr(buffer), count, out lpBytesRead))
                throw new AccessViolationException(string.Format("Could not read {2} byte(s) from {0} [{1}]!", (object)address.ToString("X8"), (object)Marshal.GetLastWin32Error(), (object)count));
            else
                return lpBytesRead;
        }

        public unsafe T Read<T>(IntPtr address, bool isRelative = false)
        {
            fixed (byte* numPtr = ReadBytes(address, MarshalCache<T>.Size, isRelative))
                return base.Read<T>((IntPtr)((void*)numPtr), false);
        }

        public IntPtr AllocateMemory(int size, MemoryAllocationType allocationType = MemoryAllocationType.MEM_COMMIT, MemoryProtectionType protect = MemoryProtectionType.PAGE_EXECUTE_READWRITE)
        {
            return VirtualAllocEx(ProcessHandle, 0U, size, allocationType, protect);
        }

        public bool FreeMemory(IntPtr address)
        {
            return FreeMemory(address, 0, MemoryFreeType.MEM_RELEASE);
        }

        public bool FreeMemory(IntPtr address, int size, MemoryFreeType freeType)
        {
            if (freeType == MemoryFreeType.MEM_RELEASE)
            {
                size = 0;
            }
            return VirtualFreeEx(ProcessHandle, address, size, freeType);
        }




        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(SafeMemoryHandle hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern unsafe bool ReadProcessMemory(SafeMemoryHandle hProcess, IntPtr lpBaseAddress, byte* lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeMemoryHandle OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32")]
        private static extern IntPtr VirtualAllocEx(SafeMemoryHandle hProcess, uint dwAddress, int nSize, MemoryAllocationType dwAllocationType, MemoryProtectionType dwProtect);

        [DllImport("kernel32")]
        private static extern bool VirtualFreeEx(SafeMemoryHandle hProcess, IntPtr dwAddress, int nSize, MemoryFreeType dwFreeType);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(SafeMemoryHandle hProcess, IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [Flags]
        public enum ProcessAccessFlags
        {
            DELETE = 65536,
            READ_CONTROL = 131072,
            SYNCHRONIZE = 1048576,
            WRITE_DAC = 262144,
            WRITE_OWNER = 524288,
            PROCESS_ALL_ACCESS = 2035711,
            PROCESS_CREATE_PROCESS = 128,
            PROCESS_CREATE_THREAD = 2,
            PROCESS_DUP_HANDLE = 64,
            PROCESS_QUERY_INFORMATION = 1024,
            PROCESS_QUERY_LIMITED_INFORMATION = 4096,
            PROCESS_SET_INFORMATION = 512,
            PROCESS_SET_QUOTA = 256,
            PROCESS_SUSPEND_RESUME = 2048,
            PROCESS_TERMINATE = 1,
            PROCESS_VM_OPERATION = 8,
            PROCESS_VM_READ = 16,
            PROCESS_VM_WRITE = 32,
        }

        public enum MemoryAllocationType
        {
            MEM_COMMIT = 4096,
            MEM_RESERVE = 8192,
            MEM_RESET = 524288,
            MEM_TOP_DOWN = 1048576,
            MEM_PHYSICAL = 4194304,
            MEM_LARGE_PAGES = 536870912,
        }

        [Flags]
        public enum MemoryProtectionType
        {
            PAGE_EXECUTE = 16,
            PAGE_EXECUTE_READ = 32,
            PAGE_EXECUTE_READWRITE = 64,
            PAGE_EXECUTE_WRITECOPY = 128,
            PAGE_NOACCESS = 1,
            PAGE_READONLY = 2,
            PAGE_READWRITE = 4,
            PAGE_WRITECOPY = 8,
            PAGE_GUARD = 256,
            PAGE_NOCACHE = 512,
            PAGE_WRITECOMBINE = 1024,
        }

        [Flags]
        public enum MemoryFreeType
        {
            MEM_DECOMMIT = 16384,
            MEM_RELEASE = 32768,
        }
    
    }

    [SuppressUnmanagedCodeSecurity]
    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public class SafeMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeMemoryHandle() : base(true)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public SafeMemoryHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        protected internal extern static bool CloseHandle(IntPtr hObject);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            Console.WriteLine("Releasing handle on " + handle.ToString("X"));
            return CloseHandle(handle);
        }
    }

    public static class MarshalCache<T>
    {
        /// <summary> The size of the Type </summary>
        public static int Size;

        /// <summary> The real, underlying type. </summary>
        public static Type RealType;

        /// <summary> The type code </summary>
        public static TypeCode TypeCode;

        /// <summary> True if this type requires the Marshaler to map variables. (No direct pointer dereferencing) </summary>
        public static bool TypeRequiresMarshal;

        internal static readonly GetUnsafePtrDelegate GetUnsafePtr;
        internal unsafe delegate void* GetUnsafePtrDelegate(ref T value);

        static MarshalCache()
        {
            TypeCode = Type.GetTypeCode(typeof(T));


            // Bools = 1 char.
            if (typeof(T) == typeof(bool))
            {
                Size = 1;
                RealType = typeof(T);
            }
            else if (typeof(T).IsEnum)
            {
                Type underlying = typeof(T).GetEnumUnderlyingType();
                Size = GetSizeOf(underlying);
                RealType = underlying;
                TypeCode = Type.GetTypeCode(underlying);
            }
            else
            {
                Size = GetSizeOf(typeof(T));
                RealType = typeof(T);
            }


            // Basically, if any members of the type have a MarshalAs attrib, then we can't just pointer deref. :(
            // This literally means any kind of MarshalAs. Strings, arrays, custom type sizes, etc.
            // Ideally, we want to avoid the Marshaler as much as possible. It causes a lot of overhead, and for a memory reading
            // lib where we need the best speed possible, we do things manually when possible!
            TypeRequiresMarshal = RequiresMarshal(RealType);
            //Debug.WriteLine("Type " + typeof(T).Name + " requires marshaling: " + TypeRequiresMarshal);


            // Generate a method to get the address of a generic type. We'll be using this for RtlMoveMemory later for much faster structure reads.
            var method = new DynamicMethod(
                string.Format("GetPinnedPtr<{0}>", typeof(T).FullName.Replace(".", "<>")), typeof(void*), new[] { typeof(T).MakeByRefType() },
                typeof(MarshalCache<>).Module);
            ILGenerator generator = method.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Conv_U);
            generator.Emit(OpCodes.Ret);
            GetUnsafePtr = (GetUnsafePtrDelegate)method.CreateDelegate(typeof(GetUnsafePtrDelegate));
        }

        private static int GetSizeOf(Type t)
        {
            try
            {
                // Note: This is in a try/catch for a reason.


                // A structure doesn't have to be marked as generic, to have generic types INSIDE of it.
                // Marshal.SizeOf will toss an exception when it can't find a size due to a generic type inside it.
                // Also... this just makes sure we can handle any other shenanigans the marshaler does.
                return Marshal.SizeOf(t);
            }
            catch
            {
                // So, chances are, we're using generic sub-types.
                // This is a good, and bad thing.
                // Good for STL implementations, bad for most everything else.
                // But for the sake of completeness, lets make this work.


                int totalSize = 0;


                foreach (FieldInfo field in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    // Check if its a fixed-size-buffer. Eg; fixed byte Pad[50];
                    object[] attr = field.GetCustomAttributes(typeof(FixedBufferAttribute), false);
                    if (attr.Length > 0)
                    {
                        var fba = attr[0] as FixedBufferAttribute;
                        totalSize += GetSizeOf(fba.ElementType) * fba.Length;
                    }


                    // Recursive. We want to allow ourselves to dive back into this function if we need to!
                    totalSize += GetSizeOf(field.FieldType);
                }
                return totalSize;
            }
        }

        private static bool RequiresMarshal(Type t)
        {
            foreach (FieldInfo fieldInfo in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                bool requires = fieldInfo.GetCustomAttributes(typeof(MarshalAsAttribute), true).Any();


                if (requires)
                {
                    Debug.WriteLine(fieldInfo.FieldType.Name + " requires marshaling.");
                    return true;
                }


                // Nope
                if (t == typeof(IntPtr) || t == typeof(string))
                    continue;


                // If it's a custom object, then check it separately for marshaling requirements.
                if (Type.GetTypeCode(t) == TypeCode.Object)
                    requires |= RequiresMarshal(fieldInfo.FieldType);


                // if anything requires a marshal, period, no matter where/what it is.
                // just return true. Hop out of this func as early as possible.
                if (requires)
                {
                    Debug.WriteLine(fieldInfo.FieldType.Name + " requires marshaling.");
                    return true;
                }
            }
            return false;
        }

    }

}
