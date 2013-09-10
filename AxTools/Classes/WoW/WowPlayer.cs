using System;
using System.Collections.Generic;
using System.Text;

namespace AxTools.Classes.WoW
{
    internal sealed class WowPlayer
    {
        internal WowPlayer(IntPtr pAddress)
        {
            Address = pAddress;
            Descriptors = WoW.WProc.Memory.Read<IntPtr>(pAddress + WowBuildInfo.UnitDescriptors);
        }

        internal static readonly Dictionary<ulong, string> Names = new Dictionary<ulong, string>();

        internal IntPtr Address;
        internal IntPtr Descriptors;

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

        private int mIsAlliance = -1;
        internal bool IsAlliance
        {
            get
            {
                if (mIsAlliance == -1)
                {
                    uint mFactionTemplate = WoW.WProc.Memory.Read<uint>(Descriptors + WowBuildInfo.UnitFactionTemplate);
                    if (mFactionTemplate == 0x89b || mFactionTemplate == 0x65d || mFactionTemplate == 0x73 || mFactionTemplate == 4 || mFactionTemplate == 3 || mFactionTemplate == 1 || mFactionTemplate == 2401)
                    {
                        mIsAlliance = 1;
                    }
                    else
                    {
                        mIsAlliance = 2;
                    }
                }
                return mIsAlliance == 1;
            }
        }

        private uint mLevel;
        internal uint Level
        {
            get
            {
                if (mLevel == 0)
                {
                    mLevel = WoW.WProc.Memory.Read<uint>(Descriptors + WowBuildInfo.UnitLevel);
                }
                return mLevel;
            }
        }

        private byte mClass;
        internal WowPlayerClass Class
        {
            get
            {
                if (mClass == 0)
                {
                    IntPtr tempOwner = WoW.WProc.Memory.Read<IntPtr>(Address + WowBuildInfo.UnitDescriptorsBig);
                    mClass = WoW.WProc.Memory.Read<byte>(tempOwner + WowBuildInfo.UnitClass);
                }
                return (WowPlayerClass) mClass;
            }
        }

        private ulong mTargetGUID = ulong.MaxValue;
        internal ulong TargetGUID
        {
            get
            {
                if (mTargetGUID == UInt64.MaxValue)
                {
                    mTargetGUID = WoW.WProc.Memory.Read<ulong>(Descriptors + WowBuildInfo.UnitTargetGUID);
                }
                return mTargetGUID;
            }
        }

        private uint mHealth = UInt32.MaxValue;
        internal uint Health
        {
            get
            {
                if (mHealth == UInt32.MaxValue)
                {
                    mHealth = WoW.WProc.Memory.Read<uint>(Descriptors + WowBuildInfo.UnitHealth);
                }
                return mHealth;
            }
        }

        private uint mHealthMax = UInt32.MaxValue;
        internal uint HealthMax
        {
            get
            {
                if (mHealthMax == UInt32.MaxValue)
                {
                    mHealthMax = WoW.WProc.Memory.Read<uint>(Descriptors + WowBuildInfo.UnitHealthMax);
                }
                return mHealthMax;
            }
        }

        private WowPointF mLocation;
        internal WowPointF Location
        {
            get
            {
                return mLocation ?? (mLocation = new WowPointF(WoW.WProc.Memory.Read<float>(Address + WowBuildInfo.UnitLocationX),
                                                               WoW.WProc.Memory.Read<float>(Address + WowBuildInfo.UnitLocationY),
                                                               WoW.WProc.Memory.Read<float>(Address + WowBuildInfo.UnitLocationZ)));
            }
        }
        
    }
}