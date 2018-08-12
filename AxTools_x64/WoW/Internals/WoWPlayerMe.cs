using System;
using System.Collections.Generic;
using System.Linq;

namespace AxTools.WoW.Internals
{
    public sealed class WoWPlayerMe : WowPlayer
    {
        private float? speed;
        private WoWItem[] itemsInBags;
        private WoWItem[] inventory;

        internal WoWPlayerMe(IntPtr address, WowProcess wow) : base(address, WoWGUID.Zero, wow)
        {
        }

        public WoWItem[] Inventory
        {
            get
            {
                if (inventory == null)
                {
                    IntPtr playerDesc = memory.Read<IntPtr>(Address + WowBuildInfoX64.UnitDescriptors);
                    List<WoWGUID> itemsInInventory = new List<WoWGUID>();
                    for (int i = 0; i < 19; i++) // 19 slots in active player's inventory
                    {
                        itemsInInventory.Add(wowProcess.Memory.Read<WoWGUID>(playerDesc + WowBuildInfoX64.ActivePlayer_Inventory + i * WoWGUID.Size));
                    }
                    List<WoWItem> items = new List<WoWItem>();
                    ObjectMgr.Pulse(wowProcess, items: items);
                    inventory = items.Where(item => itemsInInventory.Contains(item.GUID)).ToArray();
                }
                return inventory;
            }
        }

        public unsafe WoWItem[] ItemsInBags
        {
            get
            {
                if (itemsInBags == null)
                {
                    IntPtr playerDesc = memory.Read<IntPtr>(Address + WowBuildInfoX64.UnitDescriptors);
                    List<WoWGUID> containerGUIDs = new List<WoWGUID>();
                    for (int i = 0; i < 4; i++) // 4 bags
                    {
                        containerGUIDs.Add(wowProcess.Memory.Read<WoWGUID>(playerDesc + WowBuildInfoX64.ActivePlayer_Containers + i * WoWGUID.Size));
                    }
                    List<WoWGUID> itemsInBackpack = new List<WoWGUID>();
                    for (int i = 0; i < 20; i++) // 20 items in backpack
                    {
                        itemsInBackpack.Add(wowProcess.Memory.Read<WoWGUID>(playerDesc + WowBuildInfoX64.ActivePlayer_Backpack + i * WoWGUID.Size));
                    }
                    List<WoWItem> resultItems = new List<WoWItem>();
                    List<WoWItem> items = new List<WoWItem>();
                    List<WowObject> containers = new List<WowObject>();
                    Dictionary<WoWGUID, List<WoWGUID>> itemIndexInContainer = new Dictionary<WoWGUID, List<WoWGUID>>();
                    ObjectMgr.Pulse(wowProcess, items: items, containers: containers);
                    foreach (var container in containers.Where(cont => containerGUIDs.Contains(cont.GUID)))
                    {
                        IntPtr containerDescriptors = memory.Read<IntPtr>(container.Address + WowBuildInfoX64.UnitDescriptors);
                        itemIndexInContainer.Add(container.GUID, new List<WoWGUID>());
                        for (int i = 0; i < 36; i++) // max size of container
                        {
                            WoWGUID itemGUID = memory.Read<WoWGUID>(containerDescriptors + WowBuildInfoX64.WoWContainerItems + i * sizeof(WoWGUID));
                            itemIndexInContainer[container.GUID].Add(itemGUID);
                        }
                    }
                    foreach (var item in items)
                    {
                        for (int bagID = 0; bagID < 4; bagID++) // max number of bags
                        {
                            if (containerGUIDs[bagID] == item.ContainedIn)
                            {
                                item.BagID = bagID + 1;
                                item.SlotID = itemIndexInContainer[item.ContainedIn].IndexOf(item.GUID) + 1;
                                resultItems.Add(item);
                            }
                        }
                        for (int j = 0; j < itemsInBackpack.Count; j++)
                        {
                            if (itemsInBackpack[j] == item.GUID)
                            {
                                item.BagID = 0;
                                item.SlotID = j + 1;
                                resultItems.Add(item);
                            }
                        }
                    }
                    itemsInBags = resultItems.ToArray();
                }
                return itemsInBags;
            }
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

        public bool IsMoving => Speed > 0f;
    }
}