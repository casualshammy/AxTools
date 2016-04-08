using System.Runtime.InteropServices;

namespace AxTools.WoW.Internals
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PlayerInventoryAndContainers
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)] internal WoWGUID[] InvSlots;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] internal WoWGUID[] Containers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] internal WoWGUID[] Backpack;
    }
}