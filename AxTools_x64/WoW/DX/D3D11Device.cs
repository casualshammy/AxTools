using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace AxTools.WoW.DX
{
    internal sealed class D3D11Device : D3DDevice
    {
        private const int DXGI_FORMAT_R8G8B8A8_UNORM = 0x1C;
        private const int DXGI_USAGE_RENDER_TARGET_OUTPUT = 0x20;
        private const int D3D11_SDK_VERSION = 7;
        private const int D3D_DRIVER_TYPE_HARDWARE = 1;

        internal D3D11Device(Process targetProc)
            : base(targetProc, "d3d11.dll")
        {
        }

        private IntPtr _swapChain;
        private IntPtr _device;

        private IntPtr _myDxgiDll;
        private IntPtr _theirDxgiDll;

        private VTableFuncDelegate _deviceRelease;
        private VTableFuncDelegate _deviceContextRelease;
        private VTableFuncDelegate _swapchainRelease;

        protected override void InitD3D(out IntPtr d3DDevicePtr)
        {
            LoadDxgiDll();
            var scd = new SwapChainDescription
                          {
                              BufferCount = 1,
                              ModeDescription = new ModeDescription {Format = DXGI_FORMAT_R8G8B8A8_UNORM},
                              Usage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
                              OutputHandle = Form.Handle,
                              SampleDescription = new SampleDescription {Count = 1},
                              IsWindowed = true
                          };

            unsafe
            {
                IntPtr pSwapChain = IntPtr.Zero;
                IntPtr pDevice = IntPtr.Zero;
                IntPtr pImmediateContext = IntPtr.Zero;
                int ret = D3D11CreateDeviceAndSwapChain((void*) IntPtr.Zero, D3D_DRIVER_TYPE_HARDWARE,
                                                        (void*) IntPtr.Zero, 0, (void*) IntPtr.Zero, 0,
                                                        D3D11_SDK_VERSION, &scd, &pSwapChain, &pDevice,
                                                        (void*) IntPtr.Zero, &pImmediateContext);
                _swapChain = pSwapChain;
                _device = pDevice;
                d3DDevicePtr = pImmediateContext;

                if (ret >= 0)
                {
                    _swapchainRelease =
                        GetDelegate<VTableFuncDelegate>(GetVTableFuncAddress(_swapChain,
                                                                             VTableIndexes.DXGISwapChainRelease));
                    _deviceRelease =
                        GetDelegate<VTableFuncDelegate>(GetVTableFuncAddress(_device, VTableIndexes.D3D11DeviceRelease));
                    _deviceContextRelease =
                        GetDelegate<VTableFuncDelegate>(GetVTableFuncAddress(d3DDevicePtr,
                                                                             VTableIndexes.D3D11DeviceContextRelease));
                }
            }
        }

        private void LoadDxgiDll()
        {
            _myDxgiDll = LoadLibrary("dxgi.dll");
            if (_myDxgiDll == IntPtr.Zero)
                throw new Exception(String.Format("Could not load {0}", "dxgi.dll"));

            _theirDxgiDll =
                TargetProcess.Modules.Cast<ProcessModule>().First(m => m.ModuleName == "dxgi.dll").BaseAddress;
        }

        internal unsafe IntPtr GetSwapVTableFuncAbsoluteAddress(int funcIndex)
        {
            IntPtr pointer = *(IntPtr*) ((void*) _swapChain);
            pointer = *(IntPtr*) ((void*) ((int) pointer + funcIndex*4));
            IntPtr offset = IntPtr.Subtract(pointer, _myDxgiDll.ToInt32());
            return IntPtr.Add(_theirDxgiDll, offset.ToInt32());
        }

        protected override void CleanD3D()
        {
            if (_swapChain != IntPtr.Zero)
                _swapchainRelease(_swapChain);

            if (_device != IntPtr.Zero)
                _deviceRelease(_device);

            if (D3DDevicePtr != IntPtr.Zero)
                _deviceContextRelease(D3DDevicePtr);
        }

        internal override int BeginSceneVtableIndex
        {
            get { return VTableIndexes.D3D11DeviceContextBegin; }
        }

        internal override int EndSceneVtableIndex
        {
            get { return VTableIndexes.D3D11DeviceContextEnd; }
        }

        internal override int PresentVtableIndex
        {
            get { return VTableIndexes.DXGISwapChainPresent; }
        }

        #region Embedded Types

#pragma warning disable 169

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rational
        {
            internal int Numerator;
            internal int Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ModeDescription
        {
            internal int Width;
            internal int Height;
            internal Rational RefreshRate;
            internal int Format;
            internal int ScanlineOrdering;
            internal int Scaling;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SampleDescription
        {
            internal int Count;
            internal int Quality;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SwapChainDescription
        {
            internal ModeDescription ModeDescription;
            internal SampleDescription SampleDescription;
            internal int Usage;
            internal int BufferCount;
            internal IntPtr OutputHandle;
            [MarshalAs(UnmanagedType.Bool)] internal bool IsWindowed;
            internal int SwapEffect;
            internal int Flags;
        }

        internal struct VTableIndexes
        {
            internal const int DXGISwapChainRelease = 2;
            internal const int D3D11DeviceRelease = 2;
            internal const int D3D11DeviceContextRelease = 2;

            internal const int DXGISwapChainPresent = 8;

            internal const int D3D11DeviceContextBegin = 0x1B;
            internal const int D3D11DeviceContextEnd = 0x1C;
        }


#pragma warning restore 169

        #endregion

        [DllImport("d3d11.dll")]
        private static extern unsafe int D3D11CreateDeviceAndSwapChain(void* pAdapter, int driverType, void* Software,
                                                                       int flags, void* pFeatureLevels,
                                                                       int FeatureLevels, int SDKVersion,
                                                                       void* pSwapChainDesc, void* ppSwapChain,
                                                                       void* ppDevice, void* pFeatureLevel,
                                                                       void* ppImmediateContext);
    }
}

// ReSharper restore InconsistentNaming