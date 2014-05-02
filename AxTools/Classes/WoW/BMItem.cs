using System.Runtime.InteropServices;

namespace AxTools.Classes.WoW
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct BlackMarketItem
    {
        internal uint MarketID, unk1;
        internal uint Entry, unk2;
        internal uint Quantity;
        internal uint unk3;
        internal ulong minBid, minIncrement, currBid;
        internal uint TimeLeft;
        [MarshalAs(UnmanagedType.Bool)]
        internal bool YouHaveHighBid;
        internal uint NumBids;
    }
}
