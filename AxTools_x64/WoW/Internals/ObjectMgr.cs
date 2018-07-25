using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using AxTools.Helpers;
using FMemory;

namespace AxTools.WoW.Internals
{
    [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
    internal class ObjectMgr
    {

        private static byte GetObjectType(MemoryManager memory, IntPtr address)
        {
            return memory.Read<byte>(address + WowBuildInfoX64.ObjectType);
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
            byte objType = GetObjectType(wow.Memory, currObject);
            while (objType < 18 && objType >= 0)
            {
                switch (objType)
                {
                    case (byte)ObjectType.Unit:
                        wowNpcs?.Add(new WowNpc(currObject, wow));
                        break;
                    case (byte)ObjectType.Player:
                        wowUnits?.Add(new WowPlayer(currObject, wow.Memory.Read<WoWGUID>(currObject + WowBuildInfoX64.ObjectGUID), wow));
                        break;
                    case (byte)ObjectType.ActivePlayer:
                        WoWGUID guid = wow.Memory.Read<WoWGUID>(currObject + WowBuildInfoX64.ObjectGUID);
                        if (guid == playerGUID)
                        {
                            me = new WoWPlayerMe(currObject, wow);
                        }
                        break;
                    case (byte)ObjectType.GameObject:
                        wowObjects?.Add(new WowObject(currObject, wow));
                        break;
                }
                currObject = wow.Memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
                objType = GetObjectType(wow.Memory, currObject);
            }
            return me;
        }
        
        internal static WoWItem[] GetItemsInBags(WowProcess wow, PlayerInventoryAndContainers inventoryAndContainers)
        {
            Dictionary<WoWGUID, List<WoWGUID>> itemCountPerContainer = new Dictionary<WoWGUID, List<WoWGUID>>();
            List<WoWItem> items = new List<WoWItem>();
            IntPtr manager = wow.Memory.Read<IntPtr>(wow.Memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = wow.Memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (byte i = GetObjectType(wow.Memory, currObject); (i < 18) && (i >= 0); i = GetObjectType(wow.Memory, currObject))
            {
                if (i == (byte)ObjectType.Container)
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
            for (byte i = GetObjectType(wow.Memory, currObject); (i < 18) && (i >= 0); i = GetObjectType(wow.Memory, currObject))
            {
                if (i == (byte)ObjectType.Item)
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
            for (byte i = GetObjectType(wow.Memory, currObject); (i < 18) && (i >= 0); i = GetObjectType(wow.Memory, currObject))
            {
                if (i == (byte)ObjectType.Item) // WoWItem
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
