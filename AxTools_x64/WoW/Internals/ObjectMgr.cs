using FMemory;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AxTools.WoW.Internals
{
    [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
    internal class ObjectMgr
    {
        private static byte GetObjectType(MemoryManager memory, IntPtr address)
        {
            return memory.Read<byte>(address + WowBuildInfoX64.ObjectType);
        }

        private static bool TryGetNextObject(WowProcess wow, IntPtr address, out IntPtr nextAddress)
        {
            nextAddress = wow.Memory.Read<IntPtr>(address + WowBuildInfoX64.ObjectManagerNextObject);
            return nextAddress != IntPtr.Zero && (ulong)nextAddress < (ulong)UInt32.MaxValue * 2;
        }

        internal static WoWPlayerMe Pulse(
            WowProcess wow,
            List<WowObject> wowObjects = null,
            List<WowPlayer> wowUnits = null,
            List<WowNpc> wowNpcs = null,
            List<WoWItem> items = null,
            List<WowObject> containers = null)
        {
            wowObjects?.Clear();
            wowUnits?.Clear();
            wowNpcs?.Clear();
            items?.Clear();
            containers?.Clear();
            WoWPlayerMe me = null;
            WoWGUID playerGUID = wow.Memory.Read<WoWGUID>(wow.Memory.ImageBase + WowBuildInfoX64.PlayerGUID);
            IntPtr manager = wow.Memory.Read<IntPtr>(wow.Memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = wow.Memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            byte objType = GetObjectType(wow.Memory, currObject);
            while (objType < (byte)ObjectType.Invalid && objType >= 0)
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

                    case (byte)ObjectType.Container:
                        containers?.Add(new WowObject(currObject, wow));
                        break;

                    case (byte)ObjectType.Item:
                        items?.Add(new WoWItem(currObject, wow));
                        break;
                }
                if (!TryGetNextObject(wow, currObject, out currObject))
                {
                    break;
                }
                objType = GetObjectType(wow.Memory, currObject);
            }
            return me;
        }
    }
}