using System.Runtime.InteropServices;
using AxTools.WoW.Management;

namespace AxTools.WoW.Internals.ObjectManager
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PlayerInventoryAndContainers
    {
        //internal UInt128 BackpackHead;
        //internal UInt128 BackpackNECK;
        //internal UInt128 BackpackSHOULDER;
        //internal UInt128 BackpackBODY;
        //internal UInt128 BackpackCHEST;
        //internal UInt128 BackpackWAIST;
        //internal UInt128 BackpackLEGS;
        //internal UInt128 BackpackFEET;
        //internal UInt128 BackpackWRIST;
        //internal UInt128 BackpackHAND;
        //internal UInt128 BackpackFINGER1;
        //internal UInt128 BackpackFINGER2;
        //internal UInt128 BackpackTRINKET1;
        //internal UInt128 BackpackTRINKET2;
        //internal UInt128 BackpackBACK;
        //internal UInt128 BackpackMAINHAND;
        //internal UInt128 BackpackOFFHAND;
        //internal UInt128 BackpackRANGED;
        //internal UInt128 BackpackTABARD;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)] internal WoWGUID[] InvSlots;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] internal WoWGUID[] Containers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] internal WoWGUID[] Backpack;
        //internal UInt128 Backpack1;
        //internal UInt128 Backpack2;
        //internal UInt128 Backpack3;
        //internal UInt128 Backpack4;
        //internal UInt128 Backpack5;
        //internal UInt128 Backpack6;
        //internal UInt128 Backpack7;
        //internal UInt128 Backpack8;
        //internal UInt128 Backpack9;
        //internal UInt128 Backpack10;
        //internal UInt128 Backpack11;
        //internal UInt128 Backpack12;
        //internal UInt128 Backpack13;
        //internal UInt128 Backpack14;
        //internal UInt128 Backpack15;
        //internal UInt128 Backpack16;
    }
}