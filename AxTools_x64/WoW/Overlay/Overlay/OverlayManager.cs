using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using SlimDX;
using SlimDX.Direct3D9;
using Styx;
using Styx.CommonBot.Inventory;

namespace AeroOverlayHonorbuddy.Overlay
{
    public static class OverlayManager
    {
        private static ICameraProvider _cameraProvider;
        public static void SetCameraProvider(ICameraProvider cameraProvider)
        {
            _cameraProvider = cameraProvider;
        }

        public static Device Device { get { return _device; } }

        private static StateBlock _stateBlock;
        private static Device _device;
        /// <summary>
        /// Initializes the various Direct3D objects we'll be using.
        /// </summary>
        public static void InitializeD3D(Form window)
        {
            // Create a new PresentParameters object and fill in the necessary fields.
            PresentParameters presentParams = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                DeviceWindowHandle = window.Handle,
                MultisampleQuality = 0,
                BackBufferFormat = Format.A8R8G8B8,
                BackBufferWidth = window.Width,
                BackBufferHeight = window.Height,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = Format.D16
            };

            Direct3D d3d = new Direct3D();

            // Create the device.
            _device = new Device(d3d, 0, DeviceType.Hardware, window.Handle, CreateFlags.HardwareVertexProcessing, presentParams);
        }

        /*
        /// <summary> Frees all resources and resets the device.</summary>
        /// <remarks> Nesox, 2013-12-27.</remarks>
        /// <param name="handle"> The handle. </param>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        public static void Reset(IntPtr handle, int width, int height)
        {
            PresentParameters presentParams = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                DeviceWindowHandle = handle,
                MultisampleQuality = 0,
                BackBufferFormat = Format.A8R8G8B8,
                BackBufferWidth = width,
                BackBufferHeight = height,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = Format.D16
            };

            Device.Reset(presentParams);
        }*/

        /// <summary> Event queue for all listeners interested in OnDrawing events. </summary>
        public static event OnFrameWithDevice OnDrawing;

        private static Thread _thRenderThread;
        public static void InitializeRenderThread()
        {
            if (_thRenderThread == null)
            {
                _thRenderThread = new Thread(Render)
                {
                    IsBackground = true,
                    Name = "RenderThread"
                };

                _thRenderThread.Start();
            }
        }

        //This is our actual loop function
        private static void Render()
        {
            while (true)
            {
                //using (new PerformanceTimerEx("OverlayManager.Render"))
                {
                    if (_device != null)
                    {
                        using (StyxWoW.Memory.AcquireFrame(true))
                        {
                            // Clear the backbuffer to a black color.
                            _device.Clear(ClearFlags.Target, Color.FromArgb(0, 0, 0, 0), 1.0f, 0);

                            // Begin the scene.
                            _device.BeginScene();

                            if (OnDrawing != null /*&& Imports.GetForegroundWindow() == _hwndTrackedWindow*/)
                            {
                                if (_stateBlock == null)
                                    _stateBlock = new StateBlock(_device, StateBlockType.All);

                                // Draw something.
                                _stateBlock.Capture();
                                SetupRenderStates();

                                var vp = _device.Viewport;
                                vp.MinZ = 0f;
                                vp.MaxZ = 0.94f;
                                _device.Viewport = vp;

                                // Draw pl0x
                                OnDrawing(Device);

                                _stateBlock.Apply();
                            }

                            // End the scene.
                            _device.EndScene();

                            // Present the backbuffer contents to the screen.
                            _device.Present();
                        }
                    }
                }
            }
        }


        /// <summary> Sets up the render states.</summary>
        /// <remarks> Nesox, 2013-12-27.</remarks>
        private static void SetupRenderStates()
        {
            Device.SetTransform(TransformState.View, _cameraProvider.View);
            Device.SetTransform(TransformState.Projection, _cameraProvider.Projection);
            Device.SetTransform(TransformState.World, Matrix.Identity);

            // Required to enabled 3D drawing
            Device.SetRenderState(RenderState.ColorWriteEnable, ColorWriteEnable.All);

            Device.VertexShader = null;
            Device.PixelShader = null;

            // Depth
            Device.SetRenderState(RenderState.ZEnable, false);
            Device.SetRenderState(RenderState.ZWriteEnable, true);
            Device.SetRenderState(RenderState.ZFunc, Compare.LessEqual);

            Device.SetRenderState(RenderState.CullMode, Cull.None);

            // Lighting / Alpha Stuff
            Device.SetRenderState(RenderState.Lighting, false);
            Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

            Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);
            Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
            Device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Texture);
        }
    }
}
