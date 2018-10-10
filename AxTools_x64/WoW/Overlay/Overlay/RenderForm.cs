using System;
using System.Threading;
using System.Windows.Forms;
using SlimDX.Direct3D9;

namespace AxTools.Classes.WoW.Overlay.Overlay
{
    public delegate void OnFrameWithDevice(Device device);

    public partial class RenderForm : Form
    {
        private readonly IntPtr _hwndTrackedWindow;
        /// <summary> Constructor.</summary>
        /// <remarks> Nesox, 2013-12-25.</remarks>
        /// <param name="hwndTrackedWindow"> The tracked window. </param>
        public RenderForm(IntPtr hwndTrackedWindow)
        {
            InitializeComponent();
            _hwndTrackedWindow = hwndTrackedWindow;
        }

        //private int _previousWidth, _previousHeight;
        /// <summary> Sets window to overlap the target window.</summary>
        /// <remarks> Nesox, 2013-12-27.</remarks>
        /// <param name="param"> The parameter. </param>
        private void SetWindowToTarget(object param)
        {
            while (true)
            {
                Tuple<IntPtr, IntPtr> @params = (Tuple<IntPtr, IntPtr>) param;

                IntPtr handle = @params.Item1;
                IntPtr hwndWindow = @params.Item2;

                if (hwndWindow != IntPtr.Zero)
                {
                    Rect windowRect;
                    Imports.GetWindowRect(hwndWindow, out windowRect);

                    int width = windowRect.Right - windowRect.Left;
                    int height = windowRect.Bottom - windowRect.Top;

                    int style = Imports.GetWindowLong(hwndWindow, Constants.GWL_STYLE);
                    if ((style & Constants.WS_BORDER) != 0)
                    {
                        windowRect.Top += 29;
                        height -= 29;
                    }

                    //if (_previousHeight != 0 && _previousHeight != height || 
                    //    _previousWidth != 0 && _previousWidth != height)
                    //{
                    //    if (OverlayManager.Device != null)
                    //    {
                    //        Logging.Write("[Window Overlay]: Window resized, resetting backbuffer width and height.");
                    //        Logging.Write("[Window Overlay]: Width: {0}, Height: {1}", width, height);
                    //        OverlayManager.Reset(Handle, width, height);
                    //    }
                    //}

                    Imports.MoveWindow(handle, windowRect.Left, windowRect.Top, width, height, true);

                    //_previousWidth = width;
                    //_previousHeight = height;

                    IntPtr hwndTopmost = new IntPtr(-1);
                    Imports.SetWindowPos(handle, hwndTopmost, 0, 0, 0, 0, Constants.TOPMOST_FLAGS);
                }
                else
                {
                }

                Thread.Sleep(100);
            }
        }

        private Thread _thSetWindowTarget;
        /// <summary> Event handler. Called by RenderForm for load events.</summary>
        /// <remarks> Nesox, 2013-12-27.</remarks>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Event information. </param>
        private void RenderForm_Load(object sender, EventArgs e)
        {
            if (_thSetWindowTarget == null)
            {
                _thSetWindowTarget = new Thread(SetWindowToTarget)
                {
                    IsBackground = true,
                    Name = "SetWindowTargetThread"
                };

                _thSetWindowTarget.Start(new Tuple<IntPtr, IntPtr>(Handle, _hwndTrackedWindow));
            }

            //Make the window's border completely transparant
            Imports.SetWindowLong(Handle, Constants.GWL_EXSTYLE, (IntPtr)(Imports.GetWindowLong(Handle, Constants.GWL_EXSTYLE) ^ Constants.WS_EX_LAYERED ^ Constants.WS_EX_TRANSPARENT));

            Imports.SetLayeredWindowAttributes(Handle, 0, 0, Constants.LWA_ALPHA);
            Imports.SetLayeredWindowAttributes(Handle, 0, 0, Constants.LWA_COLORKEY);

            // Make sure we set the Height/Width of the window before initialize the present parameters for the device.
            Rect targetWindowRect;
            Imports.GetWindowRect(_hwndTrackedWindow, out targetWindowRect);
            Width = targetWindowRect.Right - targetWindowRect.Left;
            Height = targetWindowRect.Bottom - targetWindowRect.Top;


            // Expand the Aero Glass Effect Border to the WHOLE form.
            // since we have already had the border invisible we now
            // have a completely invisible window - apart from the DirectX
            // renders NOT in black.
            Margins marg = new Margins
            {
                Left = 0,
                Top = 0,
                Right = Width,
                Bottom = Height
            };

            Imports.DwmExtendFrameIntoClientArea(Handle, ref marg);

            OverlayManager.InitializeD3D(this);
            OverlayManager.InitializeRenderThread();
        }
    }
}
