using System;
using System.Collections.Generic;
using System.Text;

namespace AxTools.WoW.Management.ObjectManager
{
    /// <summary>
    ///     World of Warcraft game object
    /// </summary>
    public sealed class WowObject
    {
        internal static readonly Dictionary<UInt128, string> Names = new Dictionary<UInt128, string>();

        private UInt128 mGUID;

        internal UInt128 GUID
        {
            get
            {
                if (mGUID == UInt128.Zero)
                {
                    mGUID = WoWManager.WoWProcess.Memory.Read<UInt128>(Address + WowBuildInfo.ObjectGUID);
                }
                return mGUID;
            }
        }

        private UInt128 mOwnerGUID;

        internal UInt128 OwnerGUID
        {
            get
            {
                if (mOwnerGUID == UInt128.Zero)
                {
                    IntPtr tempOwner = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfo.GameObjectOwnerGUIDBase);
                    mOwnerGUID = WoWManager.WoWProcess.Memory.Read<UInt128>(tempOwner + WowBuildInfo.GameObjectOwnerGUIDOffset);
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
                    IntPtr descriptors = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfo.GameObjectOwnerGUIDBase);
                    mEntryID = WoWManager.WoWProcess.Memory.Read<uint>(descriptors + WowBuildInfo.GameObjectEntryID);
                }
                return mEntryID;
            }
        }

        private byte mAnimation;
        internal byte Animation
        {
            get
            {
                if (mAnimation == 0)
                {
                    mAnimation = WoWManager.WoWProcess.Memory.Read<byte>(Address + WowBuildInfo.GameObjectAnimation);
                }
                return mAnimation;
            }
        }

        internal IntPtr Address;

        internal WowObject(IntPtr pAddress)
        {
            Address = pAddress;
        }

    }
}