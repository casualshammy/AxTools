using System.Runtime.InteropServices;

namespace AxTools.Classes.WoW
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TerrainClickStruct
    {
        internal ulong misc;
        internal WowPoint loc;
        [MarshalAs(UnmanagedType.U4)]
        internal MouseButton button;

        internal static TerrainClickStruct FromWowPoint(WowPoint wowPoint)
        {
            return new TerrainClickStruct {misc = 0, loc = wowPoint, button = MouseButton.None | MouseButton.Left};
        }
    }
}