using System;
using AxTools.WoW.Helpers;

namespace AxTools.WoW.Internals
{
    public class WoWItem : WowObject
    {
        public WoWItem(IntPtr pAddress, WowProcess wow) : base(pAddress, wow)
        {
            IntPtr descriptors = memory.Read<IntPtr>(pAddress + WowBuildInfoX64.GameObjectOwnerGUIDBase);
            ContainedIn = memory.Read<WoWGUID>(descriptors + WowBuildInfoX64.WoWItemContainedIn);
            StackSize = memory.Read<uint>(descriptors + WowBuildInfoX64.WoWItemStackCount);
            Enchant = memory.Read<uint>(descriptors + WowBuildInfoX64.WoWItemEnchantment);
        }

        public readonly WoWGUID ContainedIn;
        public readonly uint StackSize;
        public readonly uint Enchant;
        public int BagID = 0;
        public int SlotID = 0;
        private WowheadItemInfo itemInfo;

        public new string Name
        {
            get
            {
                if (itemInfo == null)
                {
                    itemInfo = Wowhead.GetItemInfo(EntryID);
                }
                return itemInfo.Name;
            }
        }

        public uint Class
        {
            get
            {
                if (itemInfo == null)
                {
                    itemInfo = Wowhead.GetItemInfo(EntryID);
                }
                return itemInfo.Class;
            }
        }

        public uint Quality
        {
            get
            {
                if (itemInfo == null)
                {
                    itemInfo = Wowhead.GetItemInfo(EntryID);
                }
                return itemInfo.Quality;
            }
        }

        public uint Level
        {
            get
            {
                if (itemInfo == null)
                {
                    itemInfo = Wowhead.GetItemInfo(EntryID);
                }
                return itemInfo.Level;
            }
        }

    }
}
