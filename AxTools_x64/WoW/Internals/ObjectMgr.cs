using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using FMemory;

namespace AxTools.WoW.Internals
{
    [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
    internal class ObjectMgr
    {

        private static ushort GetObjectType(MemoryManager memory, IntPtr address)
        {
            return memory.Read<ushort>(address + WowBuildInfoX64.ObjectType);
        }
        
        internal static WoWPlayerMe Pulse(WowProcess wow, List<WowObject> wowObjects = null, List<WowPlayer> wowUnits = null, List<WowNpc> wowNpcs = null)
        {
            wowObjects?.Clear();
            wowUnits?.Clear();
            wowNpcs?.Clear();
            WoWPlayerMe me = null;
            WoWGUID playerGUID = wow.Memory.Read<WoWGUID>(wow.Memory.ImageBase + WowBuildInfoX64.PlayerGUID);
            IntPtr manager = wow.Memory.Read<IntPtr>(wow.Memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = wow.Memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(wow.Memory, currObject); (i < 10) && (i > 0); i = GetObjectType(wow.Memory, currObject))
            {
                switch (i)
                {
                    case 3:
                        wowNpcs?.Add(new WowNpc(currObject, wow));
                        break;
                    case 4:
                        WoWGUID objectGUID = wow.Memory.Read<WoWGUID>(currObject + WowBuildInfoX64.ObjectGUID);
                        if (objectGUID == playerGUID)
                        {
                            me = new WoWPlayerMe(currObject, wow);
                        }
                        else if (wowUnits != null)
                        {
                            wowUnits.Add(new WowPlayer(currObject, objectGUID, wow));
                        }
                        break;
                    case 5:
                        wowObjects?.Add(new WowObject(currObject, wow));
                        break;
                }
                currObject = wow.Memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return me;
        }
            
        internal static WoWItem[] GetItemsInBags(WowProcess wow, PlayerInventoryAndContainers inventoryAndContainers)
        {
            Dictionary<WoWGUID, List<WoWGUID>> itemCountPerContainer = new Dictionary<WoWGUID, List<WoWGUID>>();
            List<WoWItem> items = new List<WoWItem>();
            IntPtr manager = wow.Memory.Read<IntPtr>(wow.Memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = wow.Memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(wow.Memory, currObject); (i < 10) && (i > 0); i = GetObjectType(wow.Memory, currObject))
            {
                if (i == 2)
                {
                    WowObject p = new WowObject(currObject, wow);
                    if (inventoryAndContainers.Containers.Contains(p.GUID))
                    {
                        IntPtr desc = wow.Memory.Read<IntPtr>(p.Address + WowBuildInfoX64.UnitDescriptors);
                        ContainerStruct itemsInContainer = wow.Memory.Read<ContainerStruct>(desc + WowBuildInfoX64.WoWContainerItems);
                        itemCountPerContainer.Add(p.GUID, new List<WoWGUID>(itemsInContainer.Slots));
                    }
                }
                currObject = wow.Memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            //
            currObject = wow.Memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(wow.Memory, currObject); (i < 10) && (i > 0); i = GetObjectType(wow.Memory, currObject))
            {
                if (i == 1) // WoWItem
                {
                    WoWItem item = new WoWItem(currObject, wow);
                    for (int j = 0; j < inventoryAndContainers.Containers.Length; j++)
                    {
                        if (inventoryAndContainers.Containers[j] == item.ContainedIn)
                        {
                            item.BagID = j + 1;
                            item.SlotID = itemCountPerContainer[item.ContainedIn].IndexOf(item.GUID) + 1;
                            items.Add(item);
                        }
                    }
                    for (int j = 0; j < inventoryAndContainers.Backpack.Length; j++)
                    {
                        if (inventoryAndContainers.Backpack[j] == item.GUID)
                        {
                            item.BagID = 0;
                            item.SlotID = j + 1;
                            items.Add(item);
                        }
                    }
                }
                currObject = wow.Memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return items.ToArray();
        }

        internal static WoWItem[] GetInventory(WowProcess wow, WoWGUID[] inventory)
        {
            List<WoWItem> items = new List<WoWItem>();
            IntPtr manager = wow.Memory.Read<IntPtr>(wow.Memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = wow.Memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(wow.Memory, currObject); (i < 10) && (i > 0); i = GetObjectType(wow.Memory, currObject))
            {
                if (i == 1) // WoWItem
                {
                    WoWItem item = new WoWItem(currObject, wow);
                    if (inventory.Contains(item.GUID))
                    {
                        items.Add(item);
                    }
                }
                currObject = wow.Memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return items.ToArray();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ContainerStruct
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            internal readonly WoWGUID[] Slots;
        }

    }
}
