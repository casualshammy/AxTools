using System.Runtime.InteropServices;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;

namespace AxTools.WoW
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TerrainClickStruct
    {
        internal UInt128 misc;
        internal WowPoint loc;
        [MarshalAs(UnmanagedType.U4)]
        internal MouseButton button;

        public static TerrainClickStruct FromWowPoint(WowPoint wowPoint)
        {
            return new TerrainClickStruct {misc = UInt128.Zero, loc = wowPoint, button = MouseButton.None | MouseButton.Left};
        }
    }
}