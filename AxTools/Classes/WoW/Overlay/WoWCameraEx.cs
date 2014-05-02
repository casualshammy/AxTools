using System;
using System.Runtime.InteropServices;
using AxTools.Classes.WoW.Overlay.Overlay;
using SlimDX;
using SlimDX.Direct3D9;

namespace AxTools.Classes.WoW.Overlay
{
    public class WoWCameraProvider : ICameraProvider
    {
        public Matrix View { get { return WoWCameraEx.View; } }
        public Matrix Projection { get { return WoWCameraEx.Projection; } }
    }
    
    internal static class WoWCameraEx
    {
        private static Device Device { get { return OverlayManager.Device; }}

        /// <summary> Gets the aspect ratio. </summary>
        /// <value> The aspect ratio. </value>
        public static float AspectRatio
        {
            get
            {
                var renderTargetDesc = Device.GetRenderTarget(0).Description;
                int height = renderTargetDesc.Height;
                int width = renderTargetDesc.Width;
                return (float)width / height;
            }
        }

        /// <summary> Gets the position.</summary>
        /// <value> The position.</value>
        public static Vector3 Position { get { return Camera.Position; } }

        /// <summary>Gets the forward vector of the camera.</summary>
        /// <value>The forward.</value>
        public static Vector3 Forward
        {
            get
            {
                Matrix mat = Matrix;
                return new Vector3(mat.M11, mat.M12, mat.M13);
            }
        }

        /// <summary>Gets the right vector of the camera.</summary>
        /// <value>The right.</value>
        public static Vector3 Right
        {
            get
            {
                Matrix mat = Matrix;
                return new Vector3(mat.M21, mat.M22, mat.M23);
            }
        }

        /// <summary>Gets the up vector of the camera.</summary>
        /// <value>The up.</value>
        public static Vector3 Up
        {
            get
            {
                Matrix mat = Matrix;
                return new Vector3(mat.M31, mat.M32, mat.M33);
            }
        }

        /// <summary> Gets the matrix.</summary>
        /// <value> The matrix.</value>
        public static Matrix Matrix { get { return Camera.Matrix; }}

        /// <summary> Gets the field of view.</summary>
        /// <value> The field of view.</value>
        public static float FieldOfView
        {
            get
            {
                float f1 = WoW.WProc.Memory.Read<float>(BaseAddress + (int)CameraOffsets.CGCameraFov1);
                float f2 = WoW.WProc.Memory.Read<float>(BaseAddress + (int)CameraOffsets.CGCameraFov2);
                return f1 + f2;
            }
        }

        ///// <summary> Gets the far clip.</summary>
        ///// <value> The far clip.</value>
        //public static float FarClip
        //{
        //    get { return StyxWoW.Memory.Call<float>(StyxWoW.Offsets.Functions.World__GetFarClip, CallingConvention.Cdecl); }
        //}

        ///// <summary> Gets the near clip.</summary>
        ///// <value> The near clip.</value>
        //public static float NearClip
        //{
        //    get { return StyxWoW.Memory.Call<float>(StyxWoW.Offsets.Functions.World__GetNearClip, CallingConvention.Cdecl); }
        //}

        /// <summary> Gets the projection.</summary>
        /// <value> The projection.</value>
        public static Matrix Projection
        {
            get { return Matrix.PerspectiveFovRH(FieldOfView * 0.6f, AspectRatio, 0.2f, 1000f); }
        }

        /// <summary> Gets the view.</summary>
        /// <value> The view.</value>
        public static Matrix View
        {
            get
            {
                Vector3 eye = Position;
                Vector3 at = (eye + Forward);

                return Matrix.LookAtRH(eye, at, new Vector3(0, 0, 1));
            }
        }

        internal static IntPtr BaseAddress { get; private set; }

        /// <summary> Gets the camera.</summary>
        /// <value> The camera.</value>
        internal static NativeCamera Camera
        {
            get
            {
                IntPtr ptrWorldFrame = WoW.WProc.Memory.Read<IntPtr>((IntPtr)CameraOffsets.WorldFrame, true);
                if (ptrWorldFrame == IntPtr.Zero)
                    throw new Exception("Unable to find world frame!");

                IntPtr ptrCamera = WoW.WProc.Memory.Read<IntPtr>(ptrWorldFrame + (int)CameraOffsets.WorldFrameCameraOffset);
                var ret = WoW.WProc.Memory.Read<NativeCamera>(ptrCamera);

                BaseAddress = ptrCamera;
                return ret;
            }
        }

        #region Nested type: NativeCamera

        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeCamera
        {
            /// <summary>The table.</summary>
            public IntPtr VTable;
            /// <summary>The fourth double-word.</summary>
            public int dword4;
            /// <summary>The position.</summary>
            public Vector3 Position;
            /// <summary>The matrix.</summary>
            public Matrix Matrix;
            /// <summary>The field of view.</summary>
            public float FieldOfView;
            /// <summary>The model.</summary>
            public IntPtr Model;
            /// <summary>The timestamp.</summary>
            public int Timestamp;
            /// <summary>The near z coordinate.</summary>
            public float NearZ;
            /// <summary>The far z coordinate.</summary>
            public float FarZ;
            /// <summary>The aspect.</summary>
            public float Aspect;

            /// <summary>Returns the fully qualified type name of this instance.</summary>
            /// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
            public override string ToString()
            {
                return
                    string.Format(
                        "VTable: {0:X}, Dword4: {1}, Position: {2}, Matrix: {3}, FieldOfView: {4}, Model: {5:X}, Timestamp: {6}, NearZ: {7}, FarZ: {8}, Aspect: {9}",
                        VTable, dword4, Position, Matrix, FieldOfView, Model, Timestamp, NearZ, FarZ, Aspect);
            }
        }

        #endregion
    }

    internal enum CameraOffsets
    {
        WorldFrame = 0x116496C - 0x400000,
        WorldFrameCameraOffset = 0x8208,
        CGCameraFov1 = 0x128,
        CGCameraFov2 = 0x38,
    }
}
