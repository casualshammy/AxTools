using System;
using System.Collections.Generic;
using System.Text;

namespace AxTools.Classes.WoW.Management.ObjectManager
{
    /// <summary>
    ///     World of Warcraft game object
    /// </summary>
    public sealed class WowObject
    {
        internal static readonly Dictionary<ulong, string> Names = new Dictionary<ulong, string>();

        private ulong mGUID;

        internal ulong GUID
        {
            get
            {
                if (mGUID == 0)
                {
                    mGUID = WoWManager.WoWProcess.Memory.Read<ulong>(Address + WowBuildInfo.ObjectGUID);
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
                    IntPtr tempOwner = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfo.GameObjectOwnerGUIDBase);
                    mOwnerGUID = WoWManager.WoWProcess.Memory.Read<ulong>(tempOwner + WowBuildInfo.GameObjectOwnerGUIDOffset);
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
                        IntPtr nameBase = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfo.GameObjectNameBase);
                        IntPtr nameAddress = WoWManager.WoWProcess.Memory.Read<IntPtr>(nameBase + WowBuildInfo.GameObjectNameOffset);
                        temp = Encoding.UTF8.GetString(WoWManager.WoWProcess.Memory.ReadBytes(nameAddress, 80)).Split('\0')[0];
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

        // We don't use System.Nullable<> because it's for 40% slower
        private bool mLocationRead;
        private WowPoint mLocation;
        internal WowPoint Location
        {
            get
            {
                if (!mLocationRead)
                {
                    mLocation = WoWManager.WoWProcess.Memory.Read<WowPoint>(Address + WowBuildInfo.GameObjectLocation);
                    mLocationRead = true;
                }
                return mLocation;
            }
        }

        private uint mEntryID;
        internal uint EntryID
        {
            get
            {
                if (mEntryID == 0)
                {
                    IntPtr descriptors = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + 0x4);
                    mEntryID = WoWManager.WoWProcess.Memory.Read<uint>(descriptors + WowBuildInfo.GameObjectEntryID);
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
                    mAnimation = WoWManager.WoWProcess.Memory.Read<uint>(Address + WowBuildInfo.GameObjectAnimation);
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
            double distance1 = one.Location.Distance(ObjectMgr.LocalPlayer.Location);
            double distance2 = two.Location.Distance(ObjectMgr.LocalPlayer.Location);
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