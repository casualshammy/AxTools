using System;

namespace AxTools.WoW.Management.ObjectManager
{
    public class WoWItem : WowObject
    {
        public WoWItem(IntPtr pAddress) : base(pAddress)
        {
            IntPtr descriptors = WoWManager.WoWProcess.Memory.Read<IntPtr>(pAddress + WowBuildInfoX64.GameObjectOwnerGUIDBase);
            ContainedIn = WoWManager.WoWProcess.Memory.Read<UInt128>(descriptors + WowBuildInfoX64.WoWItemContainedIn);
            StackSize = WoWManager.WoWProcess.Memory.Read<uint>(descriptors + WowBuildInfoX64.WoWItemStackCount);
            Enchant = WoWManager.WoWProcess.Memory.Read<uint>(descriptors + WowBuildInfoX64.WoWItemEnchantment);
        }

        public readonly UInt128 ContainedIn;
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
