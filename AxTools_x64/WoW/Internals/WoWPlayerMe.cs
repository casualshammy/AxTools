using System;

namespace AxTools.WoW.Internals
{
    public sealed class WoWPlayerMe : WowPlayer
    {
        
        private float? speed;
        private WoWItem[] itemsInBags;
        private WoWItem[] inventory;
        private readonly PlayerInventoryAndContainers inventoryAndContainers;

        internal WoWPlayerMe(IntPtr address, WowProcess wow) : base(address, WoWGUID.Zero, wow)
        {
            IntPtr playerDesc = memory.Read<IntPtr>(address + WowBuildInfoX64.UnitDescriptors);
            inventoryAndContainers = memory.Read<PlayerInventoryAndContainers>(playerDesc + WowBuildInfoX64.PlayerInvSlots);
        }
        
        public WoWItem[] Inventory
        {
            get { return inventory ?? (inventory = ObjectMgr.GetInventory(wowProcess, inventoryAndContainers.InvSlots)); }
        }
        
        public WoWItem[] ItemsInBags
        {
            get { return itemsInBags ?? (itemsInBags = ObjectMgr.GetItemsInBags(wowProcess, inventoryAndContainers)); }
        }
        
        public float Speed
        {
            get
            {
                if (!speed.HasValue)
                {
                    IntPtr speedPtr = memory.Read<IntPtr>(Address + WowBuildInfoX64.PlayerSpeedBase);
                    speed = memory.Read<float>(speedPtr + WowBuildInfoX64.PlayerSpeedOffset);
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