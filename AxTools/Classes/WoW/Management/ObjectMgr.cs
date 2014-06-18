using System;
using System.Collections.Generic;
using AxTools.Classes.WoW.Management.ObjectManager;
using GreyMagic;

namespace AxTools.Classes.WoW.Management
{
    internal static class ObjectMgr
    {
        private static ExternalProcessReader _memory;

        /// <summary>
        /// You should call Pulse (any overload) to refresh local player info
        /// </summary>
        //internal static volatile WoWPlayerMe LocalPlayer;

        internal static void Initialize(WowProcess process)
        {
            _memory = process.Memory;
        }

        internal static WoWPlayerMe Pulse(List<WowObject> wowObjects, List<WowPlayer> wowUnits, List<WowNpc> wowNpcs)
        {
            wowObjects.Clear();
            wowUnits.Clear();
            wowNpcs.Clear();
            WoWPlayerMe localPlayer = null;
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
                            localPlayer = new WoWPlayerMe(currObject, playerGUID);
                        }
                        else
                        {
                            wowUnits.Add(new WowPlayer(currObject, objectGUID));
                        }
                        break;
                    case 5:
                        wowObjects.Add(new WowObject(currObject));
                        break;
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
            }
            return localPlayer;
        }

        internal static WoWPlayerMe Pulse(List<WowObject> wowObjects, List<WowNpc> wowNpcs)
        {
            wowObjects.Clear();
            wowNpcs.Clear();
            WoWPlayerMe localPlayer = null;
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
                        if (localPlayer == null)
                        {
                            ulong objectGUID = _memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID);
                            if (objectGUID == playerGUID)
                            {
                                localPlayer = new WoWPlayerMe(currObject, playerGUID);
                            }
                        }
                        break;
                    case 5:
                        wowObjects.Add(new WowObject(currObject));
                        break;
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
            }
            return localPlayer;
        }

        internal static WoWPlayerMe Pulse(List<WowObject> wowObjects)
        {
            wowObjects.Clear();
            WoWPlayerMe localPlayer = null;
            IntPtr manager = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfo.ObjectManager);
            ulong playerGUID = _memory.Read<ulong>(manager + WowBuildInfo.LocalGUID);
            IntPtr currObject = _memory.Read<IntPtr>(manager + WowBuildInfo.ObjectManagerFirstObject);
            for (int i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType);
                (i < 10) && (i > 0);
                i = _memory.Read<int>(currObject + WowBuildInfo.ObjectType))
            {
                switch (i)
                {
                    case 4:
                        if (localPlayer == null)
                        {
                            ulong objectGUID = _memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID);
                            if (objectGUID == playerGUID)
                            {
                                localPlayer = new WoWPlayerMe(currObject, playerGUID);
                            }
                        }
                        break;
                    case 5:
                        wowObjects.Add(new WowObject(currObject));
                        break;
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
            }
            return localPlayer;
        }

        internal static WoWPlayerMe Pulse(List<WowPlayer> wowPlayers)
        {
            wowPlayers.Clear();
            WoWPlayerMe localPlayer = null;
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
                        localPlayer = new WoWPlayerMe(currObject, playerGUID);
                    }
                    else
                    {
                        wowPlayers.Add(new WowPlayer(currObject, objectGUID));
                    }
                }
                currObject = _memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
            }
            return localPlayer;
        }

        internal static WoWPlayerMe Pulse()
        {
            IntPtr localPlayerPtr = _memory.Read<IntPtr>(_memory.ImageBase + WowBuildInfo.PlayerPtr);
            return new WoWPlayerMe(localPlayerPtr);
        }
        
    }
}
