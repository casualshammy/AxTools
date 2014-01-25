﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AxTools.Classes.WoW
{
    internal sealed class WowPlayer
    {
        internal WowPlayer(IntPtr pAddress)
        {
            Address = pAddress;
            IntPtr desc = WoW.WProc.Memory.Read<IntPtr>(pAddress + WowBuildInfo.UnitDescriptors);
            WowPlayerInfo info = WoW.WProc.Memory.Read<WowPlayerInfo>(desc + WowBuildInfo.UnitTargetGUID);
            TargetGUID = info.TargetGUID;
            Health = info.Health;
            HealthMax = info.HealthMax;
            Level = info.Level;
            IsAlliance = info.FactionTemplate == 0x89b || info.FactionTemplate == 0x65d || info.FactionTemplate == 0x73 || info.FactionTemplate == 4 || info.FactionTemplate == 3 || info.FactionTemplate == 1 ||
                         info.FactionTemplate == 2401;
            Class = (WowPlayerClass) info.Class;
        }

        internal static readonly Dictionary<ulong, string> Names = new Dictionary<ulong, string>();

        internal readonly IntPtr Address;

        internal readonly ulong TargetGUID;

        internal readonly bool IsAlliance;

        internal readonly uint Level;

        internal readonly uint Health;

        internal readonly uint HealthMax;

        internal readonly WowPlayerClass Class;

        private ulong mGUID;
        internal ulong GUID
        {
            get
            {
                if (mGUID == 0)
                {
                    mGUID = WoW.WProc.Memory.Read<ulong>(Address + WowBuildInfo.ObjectGUID);
                }
                return mGUID;
            }
        }

        internal string Name
        {
            get
            {
                string temp;
                if (!Names.TryGetValue(GUID, out temp))
                {
                    try
                    {
                        uint nameMask = WoW.WProc.Memory.Read<uint>(WoW.WProc.Memory.ImageBase + WowBuildInfo.UnitNameCachePointer +
                                                                    WowBuildInfo.UnitNameMaskOffset);
                        uint nameBase = WoW.WProc.Memory.Read<uint>(WoW.WProc.Memory.ImageBase + WowBuildInfo.UnitNameCachePointer +
                                                                    WowBuildInfo.UnitNameBaseOffset);
                        uint var4 = nameMask;
                        uint var5 = nameBase;
                        nameMask &= (uint) GUID;
                        nameMask += nameMask*2;
                        nameMask = (nameBase + (nameMask*4) + 4);
                        nameMask = WoW.WProc.Memory.Read<uint>((IntPtr) (nameMask + 4));
                        while (WoW.WProc.Memory.Read<uint>((IntPtr) nameMask) != (uint) GUID)
                        {
                            nameBase = (uint) GUID;
                            nameBase &= var4;
                            nameBase += nameBase*2;
                            nameBase = WoW.WProc.Memory.Read<uint>((IntPtr) (var5 + (nameBase*4)));
                            nameBase += nameMask;
                            nameMask = WoW.WProc.Memory.Read<uint>((IntPtr) (nameBase + 4));
                        }
                        byte[] nameBytes = WoW.WProc.Memory.ReadBytes((IntPtr) (nameMask + WowBuildInfo.UnitNameStringOffset), 80);
                        temp = Encoding.UTF8.GetString(nameBytes).Split('\0')[0];
                        Names.Add(GUID, temp);
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
                return temp;
            }
        }

        private WowPoint? mLocation;
        internal WowPoint Location
        {
            get
            {
                if (!mLocation.HasValue)
                {
                    mLocation = WoW.WProc.Memory.Read<WowPoint>(Address + WowBuildInfo.UnitLocation);
                }
                return mLocation.Value;
            }
        }
        
    }
}