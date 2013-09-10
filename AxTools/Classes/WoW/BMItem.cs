using System.Runtime.InteropServices;

namespace AxTools.Classes.WoW
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BlackMarketItem
    {
        public uint MarketID, unk1;
        public uint Entry, unk2;
        public uint Quantity;
        public uint unk3;
        public ulong minBid, minIncrement, currBid;
        public uint TimeLeft;
        [MarshalAs(UnmanagedType.Bool)]
        public bool YouHaveHighBid;
        public uint NumBids;
    }
}
