using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using AxTools.Helpers.MemoryManagement;

namespace AxTools.WoW.Internals
{
    [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
    internal static class ObjectMgr
    {
        private static MemoryManager _memory;

        internal static void Initialize(WowProcess process)
        {
            _memory = process.Memory;
        }

        private static ushort GetObjectType(IntPtr address)
        {
            return _memory.Read<ushort>(address + WowBuildInfoX64.ObjectType);
        }

        internal static WoWPlayerMe Pulse(List<WowObject> wowObjects, List<WowPlayer> wowUnits, List<WowNpc> wowNpcs)
        {
            wowObjects.Clear();
            wowUnits.Clear();
            wowNpcs.Clear();
            WoWPlayerMe localPlayer = Pulse();
            WoWGUID playerGUID = localPlayer.GUID;
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            //Log.Info("Pulse-1: address: " + currObject + "; type: " + GetObjectType(currObject));
            for (int i = GetObjectType(currObject); (i < 10) && (i > 0); i = GetObjectType(currObject))
            {
                //Log.Info("Pulse-1: address: " + currObject + "; type: " + GetObjectType(currObject));
                switch (i)
                {
                    case 3:
                        wowNpcs.Add(new WowNpc(currObject));
                        break;
                    case 4:
                        WoWGUID objectGUID = _memory.Read<WoWGUID>(currObject + WowBuildInfoX64.ObjectGUID);
                        if (objectGUID != playerGUID)
                        {
                            wowUnits.Add(new WowPlayer(currObject, objectGUID));
                        }
                        break;
                    case 5:
                        wowObjects.Add(new WowObject(currObject));
                        break;
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return localPlayer;
        }

        internal static WoWPlayerMe Pulse(List<WowObject> wowObjects, List<WowNpc> wowNpcs)
        {
            wowObjects.Clear();
            wowNpcs.Clear();
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(currObject); (i < 10) && (i > 0); i = GetObjectType(currObject))
            {
                switch (i)
                {
                    case 3:
                        wowNpcs.Add(new WowNpc(currObject));
                        break;
                    case 5:
                        wowObjects.Add(new WowObject(currObject));
                        break;
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return Pulse();
        }

        internal static WoWPlayerMe Pulse(List<WowPlayer> wowPlayers, List<WowNpc> wowNpcs)
        {
            wowPlayers.Clear();
            wowNpcs.Clear();
            WoWPlayerMe localPlayer = Pulse();
            WoWGUID playerGUID = localPlayer.GUID;
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(currObject); (i < 10) && (i > 0); i = GetObjectType(currObject))
            {
                switch (i)
                {
                    case 3:
                        wowNpcs.Add(new WowNpc(currObject));
                        break;
                    case 4:
                        WoWGUID objectGUID = _memory.Read<WoWGUID>(currObject + WowBuildInfoX64.ObjectGUID);
                        if (objectGUID != playerGUID)
                        {
                            wowPlayers.Add(new WowPlayer(currObject, objectGUID));
                        }
                        break;
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return localPlayer;
        }

        internal static WoWPlayerMe Pulse(List<WowObject> wowObjects)
        {
            wowObjects.Clear();
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(currObject); (i < 10) && (i > 0); i = GetObjectType(currObject))
            {
                if (i == 5)
                {
                    wowObjects.Add(new WowObject(currObject));
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return Pulse();
        }

        internal static WoWPlayerMe Pulse(List<WowPlayer> wowPlayers)
        {
            wowPlayers.Clear();
            WoWPlayerMe localPlayer = Pulse();
            WoWGUID playerGUID = localPlayer.GUID;
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(currObject); (i < 10) && (i > 0); i = GetObjectType(currObject))
            {
                if (i == 4)
                {
                    WoWGUID objectGUID = _memory.Read<WoWGUID>(currObject + WowBuildInfoX64.ObjectGUID);
                    if (objectGUID != playerGUID)
                    {
                        wowPlayers.Add(new WowPlayer(currObject, objectGUID));
                    }
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return localPlayer;
        }

        internal static WoWPlayerMe Pulse(List<WowNpc> wowNpcs)
        {
            wowNpcs.Clear();
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(currObject); (i < 10) && (i > 0); i = GetObjectType(currObject))
            {
                if (i == 3)
                {
                    wowNpcs.Add(new WowNpc(currObject));
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return Pulse();
        }

        internal static WoWPlayerMe Pulse()
        {
            IntPtr localPlayerPtr = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.PlayerPtr);
            return new WoWPlayerMe(localPlayerPtr);
        }

        internal static WoWItem[] GetItemsInBags(PlayerInventoryAndContainers inventoryAndContainers)
        {
            Dictionary<WoWGUID, List<WoWGUID>> itemCountPerContainer = new Dictionary<WoWGUID, List<WoWGUID>>();
            List<WoWItem> items = new List<WoWItem>();
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            //
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(currObject); (i < 10) && (i > 0); i = GetObjectType(currObject))
            {
                if (i == 2)
                {
                    WowObject p = new WowObject(currObject);
                    if (inventoryAndContainers.Containers.Contains(p.GUID))
                    {
                        IntPtr desc = _memory.Read<IntPtr>(p.Address + WowBuildInfoX64.UnitDescriptors);
                        SixteenUInt128 itemsInContainer = _memory.Read<SixteenUInt128>(desc + WowBuildInfoX64.WoWContainerItems);
                        itemCountPerContainer.Add(p.GUID, new List<WoWGUID>(itemsInContainer.Slots));
                    }
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            //
            currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(currObject); (i < 10) && (i > 0); i = GetObjectType(currObject))
            {
                if (i == 1) // WoWItem
                {
                    WoWItem item = new WoWItem(currObject);
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
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return items.ToArray();
        }

        internal static WoWItem[] GetInventory(WoWGUID[] inventory)
        {
            List<WoWItem> items = new List<WoWItem>();
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = GetObjectType(currObject); (i < 10) && (i > 0); i = GetObjectType(currObject))
            {
                if (i == 1) // WoWItem
                {
                    WoWItem item = new WoWItem(currObject);
                    if (inventory.Contains(item.GUID))
                    {
                        items.Add(item);
                    }
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfoX64.ObjectManagerNextObject);
            }
            return items.ToArray();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SixteenUInt128
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            internal readonly WoWGUID[] Slots;
        }

    }
}
