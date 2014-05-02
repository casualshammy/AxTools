using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AxTools.Classes.WoW.DX
{
    internal abstract class D3DDevice : IDisposable
    {
        protected readonly IntPtr D3DDevicePtr;
        protected readonly Process TargetProcess;
        private readonly string d3DDllName;
        private readonly List<IntPtr> loadedLibraries = new List<IntPtr>();

        private IntPtr myD3DDll;
        private IntPtr theirD3DDll;

        protected D3DDevice(Process targetProcess, string d3DDllName)
        {
            TargetProcess = targetProcess;
            this.d3DDllName = d3DDllName;
            Form = new Form();
            LoadDll();
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            InitD3D(out D3DDevicePtr);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        protected Form Form { get; private set; }

        internal abstract int BeginSceneVtableIndex { get; }

        internal abstract int EndSceneVtableIndex { get; }

        internal abstract int PresentVtableIndex { get; }

        /// <summary>
        /// initiializes d3d and sets device pointer.
        /// </summary>
        protected abstract void InitD3D(out IntPtr d3DDevicePtr);

        /// <summary>
        /// Cleanup should be done in here.
        /// </summary>
        protected abstract void CleanD3D();

        private void LoadDll()
        {
            myD3DDll = LoadLibrary(d3DDllName);
            if (myD3DDll == IntPtr.Zero)
                throw new Exception(String.Format("Could not load {0}", d3DDllName));

            theirD3DDll = TargetProcess.Modules.Cast<ProcessModule>().First(m => m.ModuleName == d3DDllName).BaseAddress;
        }

        protected IntPtr LoadLibrary(string library)
        {
            // Attempt to grab the module handle if its loaded already.
            IntPtr ret = WinAPI.NativeMethods.GetModuleHandle(library);
            if (ret == IntPtr.Zero)
            {
                // Load the lib manually if its not, storing it in a list so we can free it later.
                ret = WinAPI.NativeMethods.LoadLibrary(library);
                loadedLibraries.Add(ret);
            }
            return ret;
        }

        protected unsafe IntPtr GetVTableFuncAddress(IntPtr obj, int funcIndex)
        {
            IntPtr pointer = *(IntPtr*)((void*)obj);
            return *(IntPtr*)((void*)((int)pointer + funcIndex * 4));
        }

        internal unsafe IntPtr GetDeviceVTableFuncAbsoluteAddress(int funcIndex)
        {
            IntPtr pointer = *(IntPtr*)((void*)D3DDevicePtr);
            pointer = *(IntPtr*)((void*)((int)pointer + funcIndex * 4));
            IntPtr offset = IntPtr.Subtract(pointer, myD3DDll.ToInt32());
            return IntPtr.Add(theirD3DDll, offset.ToInt32());
        }

        protected T GetDelegate<T>(IntPtr address) where T : class
        {
            return Marshal.GetDelegateForFunctionPointer(address, typeof(T)) as T;
        }

        #region IDisposable Pattern Implementation

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    CleanD3D();
                    if (Form != null)
                        Form.Dispose();

                    foreach (IntPtr loadedLibrary in loadedLibraries)
                    {
                        WinAPI.NativeMethods.FreeLibrary(loadedLibrary);
                    }
                }
                disposed = true;
            }
        }

        ~D3DDevice()
        {
            Dispose(false);
        }

        #endregion IDisposable Pattern Implementation

        #region Nested type: VTableFuncDelegate

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        protected delegate void VTableFuncDelegate(IntPtr instance);

        #endregion Nested type: VTableFuncDelegate
    }
}