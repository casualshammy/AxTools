using System;
using System.Collections.Generic;
using System.Reflection;
using AxTools.Helpers.MemoryManagement;
using AxTools.WoW.Management.ObjectManager;

namespace AxTools.WoW.Management
{
    [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
    internal static class ObjectMgr
    {
        private static MemoryManager _memory;
        
        internal static void Initialize(WowProcess process)
        {
            _memory = process.Memory;
        }

        internal static WoWPlayerMe Pulse(List<WowObject> wowObjects, List<WowPlayer> wowUnits, List<WowNpc> wowNpcs)
        {
            wowObjects.Clear();
            wowUnits.Clear();
            wowNpcs.Clear();
            WoWPlayerMe localPlayer = Pulse();
            UInt128 playerGUID = localPlayer.GUID;
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType))
            {
                switch (i)
                {
                    case 3:
                        wowNpcs.Add(new WowNpc(currObject));
                        break;
                    case 4:
                        UInt128 objectGUID = _memory.Read<UInt128>(currObject + WowBuildInfoX64.ObjectGUID);
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
            //WoWPlayerMe localPlayer = null;
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            //UInt128 playerGUID = _memory.Read<UInt128>(_memory.ImageBase + WowBuildInfo.PlayerGUID);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType))
            {
                switch (i)
                {
                    case 3:
                        wowNpcs.Add(new WowNpc(currObject));
                        break;
                    case 4:
                        //if (localPlayer == null)
                        //{
                        //    UInt128 objectGUID = _memory.Read<UInt128>(currObject + WowBuildInfo.ObjectGUID);
                        //    if (objectGUID == playerGUID)
                        //    {
                        //        localPlayer = new WoWPlayerMe(currObject, playerGUID);
                        //    }
                        //}
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
            UInt128 playerGUID = localPlayer.GUID;
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType))
            {
                switch (i)
                {
                    case 3:
                        wowNpcs.Add(new WowNpc(currObject));
                        break;
                    case 4:
                        UInt128 objectGUID = _memory.Read<UInt128>(currObject + WowBuildInfoX64.ObjectGUID);
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
            for (int i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType))
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
            UInt128 playerGUID = localPlayer.GUID;
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType))
            {
                if (i == 4)
                {
                    UInt128 objectGUID = _memory.Read<UInt128>(currObject + WowBuildInfoX64.ObjectGUID);
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
            //WoWPlayerMe localPlayer = null;
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfoX64.ObjectManager);
            //UInt128 playerGUID = _memory.Read<UInt128>(_memory.ImageBase + WowBuildInfo.PlayerGUID);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfoX64.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfoX64.ObjectType))
            {
                switch (i)
                {
                    case 3:
                        wowNpcs.Add(new WowNpc(currObject));
                        break;
                    case 4:
                        //if (localPlayer == null)
                        //{
                        //    UInt128 objectGUID = _memory.Read<UInt128>(currObject + WowBuildInfo.ObjectGUID);
                        //    if (objectGUID == playerGUID)
                        //    {
                        //        localPlayer = new WoWPlayerMe(currObject, playerGUID);
                        //    }
                        //}
                        break;
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
        
    }
}
