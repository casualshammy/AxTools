using AxTools.WoW.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AxTools.WoW.Internals
{
    public static class BlackMarketMgr
    {
        private static readonly unsafe int sizeofBmItem = sizeof(BlackMarketItemInternal);

        public static IEnumerable<BlackMarketItem> GetAllItems(GameInterface game)
        {
            if (game.IsInGame)
            {
                var numItems = game.Memory.Read<uint>(game.Memory.ImageBase + WowBuildInfoX64.BlackMarketNumItems);
                if (numItems != 0)
                {
                    var baseAddr = game.Memory.Read<IntPtr>(game.Memory.ImageBase + WowBuildInfoX64.BlackMarketItems);
                    for (uint i = 0; i < numItems; ++i)
                    {
                        var finalAddr = (int)(baseAddr + (int)(i * sizeofBmItem));
                        BlackMarketItemInternal item = game.Memory.Read<BlackMarketItemInternal>(new IntPtr(finalAddr));
                        yield return new BlackMarketItem
                        {
                            Name = Wowhead.GetItemInfo(item.Entry).Name,
                            Image = Wowhead.GetItemInfo(item.Entry).Image,
                            TimeLeft = TimeSpan.FromSeconds(item.TimeLeft),
                            LastBidGold = (int)(item.currBid > 0 ? (uint)(item.currBid / 10000) : item.NextBid / 10000),
                            NumBids = (int)item.NumBids
                        };
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 0xA0)]
        private struct BlackMarketItemInternal
        {
            [FieldOffset(0x8)]
            public readonly uint Entry;

            [FieldOffset(0x80)]
            public readonly ulong NextBid;

            [FieldOffset(0x90)]
            public readonly ulong currBid;

            [FieldOffset(0x98)]
            public readonly uint TimeLeft;

            [MarshalAs(UnmanagedType.Bool)]
            [FieldOffset(0x9C)]
            private readonly bool YouHaveHighBid;

            [FieldOffset(0xA0)]
            public readonly uint NumBids;
        }
    }

    public class BlackMarketItem
    {
        public string Name;
        public Image Image;
        public TimeSpan TimeLeft;
        public int LastBidGold;
        public int NumBids;
    }
}