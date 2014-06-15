using System;
using System.Collections.Generic;
using AxTools.Classes.WoW.Management.ObjectManager;
using GreyMagic;

namespace AxTools.Classes.WoW.Management
{
    internal static class ObjectMgr
    {
        private static ExternalProcessReader _memory;
        private static readonly object Lock = new object();
        internal static LocalPlayer LocalPlayer;

        internal static void Initialize(WowProcess process)
        {
            _memory = process.Memory;
            LocalPlayer = new LocalPlayer();
        }

        private static void UpdateLocalPlayerInfo(IntPtr address, ulong guid)
        {
            lock (Lock)
            {
                LocalPlayer.Address = address;
                LocalPlayer.GUID = guid;
                LocalPlayer.Location = _memory.Read<WowPoint>(address + WowBuildInfo.UnitLocation);
                LocalPlayer.Rotation = _memory.Read<float>(address + WowBuildInfo.UnitRotation);
                LocalPlayer.CastingSpellID = _memory.Read<uint>(address + WowBuildInfo.UnitCastingID);
                LocalPlayer.ChannelSpellID = _memory.Read<uint>(address + WowBuildInfo.UnitChannelingID);
                IntPtr descriptors = _memory.Read<IntPtr>(address + WowBuildInfo.UnitDescriptors);
                WowPlayerInfo info = _memory.Read<WowPlayerInfo>(descriptors + WowBuildInfo.UnitTargetGUID);
                LocalPlayer.Health = info.Health;
                LocalPlayer.HealthMax = info.HealthMax;
                LocalPlayer.Level = info.Level;
                LocalPlayer.TargetGUID = info.TargetGUID;
                LocalPlayer.IsAlliance = info.FactionTemplate == 0x89b || info.FactionTemplate == 0x65d || info.FactionTemplate == 0x73 ||
                                         info.FactionTemplate == 4 || info.FactionTemplate == 3 || info.FactionTemplate == 1 || info.FactionTemplate == 2401;
            }
        }

        internal static void Pulse(List<WowObject> wowObjects, List<WowPlayer> wowUnits, List<WowNpc> wowNpcs)
        {
            wowObjects.Clear();
            wowUnits.Clear();
            wowNpcs.Clear();
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfo.ObjectManager);
            ulong playerGUID = _memory.Read<ulong>(manager + WowBuildInfo.LocalGUID);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfo.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType))
            {
                switch (i)
                {
                    case 3:
                        wowNpcs.Add(new WowNpc(currObject));
                        break;
                    case 4:
                        ulong objectGUID = _memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID);
                        if (objectGUID == playerGUID)
                        {
                            UpdateLocalPlayerInfo(currObject, playerGUID);
                        }
                        else
                        {
                            wowUnits.Add(new WowPlayer(currObject));
                        }
                        break;
                    case 5:
                        wowObjects.Add(new WowObject(currObject));
                        break;
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
            }
        }
        
        internal static void Pulse(List<WowObject> wowObjects, List<WowNpc> wowNpcs)
        {
            wowObjects.Clear();
            wowNpcs.Clear();
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfo.ObjectManager);
            ulong playerGUID = _memory.Read<ulong>(manager + WowBuildInfo.LocalGUID);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfo.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType))
            {
                switch (i)
                {
                    case 3:
                        wowNpcs.Add(new WowNpc(currObject));
                        break;
                    case 4:
                        if (_memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID) == playerGUID)
                        {
                            UpdateLocalPlayerInfo(currObject, playerGUID);
                        }
                        break;
                    case 5:
                        wowObjects.Add(new WowObject(currObject));
                        break;
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
            }
        }
        
        internal static void Pulse(List<WowObject> wowObjects)
        {
            wowObjects.Clear();
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfo.ObjectManager);
            ulong playerGUID = _memory.Read<ulong>(manager + WowBuildInfo.LocalGUID);
            bool localPlayerFound = false;
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfo.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType))
            {
                switch (i)
                {
                    case 4:
                        if (!localPlayerFound)
                        {
                            ulong objectGUID = _memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID);
                            if (objectGUID == playerGUID)
                            {
                                UpdateLocalPlayerInfo(currObject, playerGUID);
                                localPlayerFound = true;
                            }
                        }
                        break;
                    case 5:
                        wowObjects.Add(new WowObject(currObject));
                        break;
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
            }
        }
        
        internal static void Pulse(List<WowPlayer> wowPlayers)
        {
            wowPlayers.Clear();
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfo.ObjectManager);
            ulong playerGUID = _memory.Read<ulong>(manager + WowBuildInfo.LocalGUID);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfo.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType))
            {
                if (i == 4)
                {
                    ulong objectGUID = _memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID);
                    if (objectGUID == playerGUID)
                    {
                        UpdateLocalPlayerInfo(currObject, playerGUID);
                    }
                    else
                    {
                        wowPlayers.Add(new WowPlayer(currObject));
                    }
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
            }
        }
        
        internal static void Pulse()
        {
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfo.ObjectManager);
            ulong playerGUID = _memory.Read<ulong>(manager + WowBuildInfo.LocalGUID);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfo.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType))
            {
                if (i == 4 && _memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID) == playerGUID)
                {
                    UpdateLocalPlayerInfo(currObject, playerGUID);
                    return;
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
            }
        }
    
    }
}
