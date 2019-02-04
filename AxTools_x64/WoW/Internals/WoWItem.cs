using AxTools.WoW.Helpers;
using System;

namespace AxTools.WoW.Internals
{
    public class WoWItem : WowObject
    {
        public WoWItem(IntPtr pAddress, WowProcess wow) : base(pAddress, wow)
        {
            ContainedIn = memory.Read<WoWGUID>(Address + WowBuildInfoX64.WoWItemContainedIn);
            StackSize = memory.Read<uint>(Address + WowBuildInfoX64.WoWItemStackCount);
            WeaponEnchant = memory.Read<uint>(Address + WowBuildInfoX64.WoWItem_WeaponEnchant);
        }

        public readonly WoWGUID ContainedIn;
        public readonly uint StackSize;
        public readonly uint WeaponEnchant;
        public int BagID = 0;
        public int SlotID = 0;
        private WowheadItemInfo itemInfo;
        private WoWGUID MGUID;

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
        
        public override WoWGUID GUID
        {
            get
            {
                if (MGUID == WoWGUID.Zero)
                {
                    MGUID = memory.Read<WoWGUID>(Address + WowBuildInfoX64.ObjectGUID);
                }
                return MGUID;
            }
        }

        public byte[] GetGUIDBytes()
        {
            unsafe
            {
                return memory.ReadBytes(Address + WowBuildInfoX64.ObjectGUID, sizeof(WoWGUID));
            }
        }

    }
}