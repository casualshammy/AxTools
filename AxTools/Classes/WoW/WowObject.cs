using System;
using System.Collections.Generic;
using System.Text;

namespace AxTools.Classes.WoW
{
    internal sealed class WowObject
    {
        internal static readonly Dictionary<ulong, string> Names = new Dictionary<ulong, string>();

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

        private ulong mOwnerGUID;
        internal ulong OwnerGUID
        {
            get
            {
                if (mOwnerGUID == 0)
                {
                    IntPtr tempOwner = WoW.WProc.Memory.Read<IntPtr>(Address + WowBuildInfo.GameObjectOwnerGUIDBase);
                    mOwnerGUID = WoW.WProc.Memory.Read<ulong>(tempOwner + WowBuildInfo.GameObjectOwnerGUIDOffset);
                }
                return mOwnerGUID;
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
                        IntPtr nameBase = WoW.WProc.Memory.Read<IntPtr>(Address + WowBuildInfo.GameObjectNameBase);
                        IntPtr nameAddress = WoW.WProc.Memory.Read<IntPtr>(nameBase + WowBuildInfo.GameObjectNameOffset);
                        temp = Encoding.UTF8.GetString(WoW.WProc.Memory.ReadBytes(nameAddress, 80)).Split('\0')[0];
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
                    mLocation = WoW.WProc.Memory.Read<WowPoint>(Address + WowBuildInfo.GameObjectLocation);
                }
                return mLocation.Value;
            }
        }

        private uint mEntryID;
        internal uint EntryID
        {
            get
            {
                if (mEntryID == 0)
                {
                    IntPtr descriptors = WoW.WProc.Memory.Read<IntPtr>(Address + 0x4);
                    mEntryID = WoW.WProc.Memory.Read<uint>(descriptors + WowBuildInfo.GameObjectEntryID);
                }
                return mEntryID;
            }
        }

        private uint mAnimation;
        internal uint Animation
        {
            get
            {
                if (mAnimation == 0)
                {
                    mAnimation = WoW.WProc.Memory.Read<uint>(Address + WowBuildInfo.GameObjectAnimation);
                }
                return mAnimation;
            }
        }

        internal IntPtr Address;

        internal WowObject(IntPtr pAddress)
        {
            Address = pAddress;
        }

        internal static int SortByDistance(WowObject one, WowObject two)
        {
            double distance1 = one.Location.Distance(LocalPlayer.Location);
            double distance2 = two.Location.Distance(LocalPlayer.Location);
            if (distance1 > distance2)
            {
                return 1;
            }
            if (distance1 < distance2)
            {
                return -1;
            }
            return 0;
        }
    }
}