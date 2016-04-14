using System;

// ReSharper disable InconsistentNaming
namespace AxTools.WoW.Internals
{
    public sealed class WoWPlayerMe : WowPlayer
    {
        internal WoWPlayerMe(IntPtr address) : base(address)
        {
            IntPtr playerDesc = WoWManager.WoWProcess.Memory.Read<IntPtr>(address + WowBuildInfoX64.UnitDescriptors);
            inventoryAndContainers = WoWManager.WoWProcess.Memory.Read<PlayerInventoryAndContainers>(playerDesc + WowBuildInfoX64.PlayerInvSlots);
        }

        private readonly PlayerInventoryAndContainers inventoryAndContainers;

        private uint castingSpellID;
        public uint CastingSpellID
        {
            get
            {
                if (castingSpellID == 0)
                {
                    castingSpellID = WoWManager.WoWProcess.Memory.Read<uint>(Address + WowBuildInfoX64.UnitCastingID);
                }
                return castingSpellID;
            }
        }

        private uint channelSpellID;
        public uint ChannelSpellID
        {
            get
            {
                if (channelSpellID == 0)
                {
                    channelSpellID = WoWManager.WoWProcess.Memory.Read<uint>(Address + WowBuildInfoX64.UnitChannelingID);
                }
                return channelSpellID;
            }
        }

        private bool rotationRead;
        private float rotation;
        public float Rotation
        {
            get
            {
                if (!rotationRead)
                {
                    rotation = WoWManager.WoWProcess.Memory.Read<float>(Address + WowBuildInfoX64.UnitRotation);
                    rotationRead = true;
                }
                return rotation;
            }
        }

        private WoWItem[] inventory;
        /// <summary>
        ///     INVSLOT_HEAD       = 0;
        ///     INVSLOT_NECK       = 1;
        ///     INVSLOT_SHOULDER   = 2;
        ///     INVSLOT_BODY       = 3;
        ///     INVSLOT_CHEST      = 4;
        ///     INVSLOT_WAIST      = 5;
        ///     INVSLOT_LEGS       = 6;
        ///     INVSLOT_FEET       = 7;
        ///     INVSLOT_WRIST      = 8;
        ///     INVSLOT_HAND       = 9;
        ///     INVSLOT_FINGER1    = 10;
        ///     INVSLOT_FINGER2    = 11;
        ///     INVSLOT_TRINKET1   = 12;
        ///     INVSLOT_TRINKET2   = 13;
        ///     INVSLOT_BACK       = 14;
        ///     INVSLOT_MAINHAND   = 15;
        ///     INVSLOT_OFFHAND    = 16;
        ///     INVSLOT_RANGED     = 17;
        ///     INVSLOT_TABARD     = 18;
        /// </summary>
        public WoWItem[] Inventory
        {
            get { return inventory ?? (inventory = ObjectMgr.GetInventory(inventoryAndContainers.InvSlots)); }
        }

        private WoWItem[] itemsInBags;
        public WoWItem[] ItemsInBags
        {
            get { return itemsInBags ?? (itemsInBags = ObjectMgr.GetItemsInBags(inventoryAndContainers)); }
        }

        private float? speed;
        public float Speed
        {
            get
            {
                if (!speed.HasValue)
                {
                    IntPtr speedPtr = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfoX64.PlayerSpeedBase);
                    speed = WoWManager.WoWProcess.Memory.Read<float>(speedPtr + WowBuildInfoX64.PlayerSpeedOffset);
                }
                return speed.Value;
            }
        }

        public bool IsMoving
        {
            get { return Speed > 0f; }
        }
    }
}
// ReSharper restore InconsistentNaming