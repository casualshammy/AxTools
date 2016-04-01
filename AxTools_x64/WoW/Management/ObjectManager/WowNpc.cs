using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace AxTools.WoW.Management.ObjectManager
{
    /// <summary>
    /// Represents a World of Warcraft NPC object
    /// </summary>
    public sealed class WowNpc
    {
        internal WowNpc(IntPtr pAddress)
        {
            Address = pAddress;
        }

        internal static readonly Dictionary<UInt128, string> Names = new Dictionary<UInt128, string>();

        internal IntPtr Address;

        private IntPtr mDescriptors = IntPtr.Zero;
        internal IntPtr Descriptors
        {
            get
            {
                if (mDescriptors == IntPtr.Zero)
                {
                    mDescriptors = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfoX64.UnitDescriptors);
                }
                return mDescriptors;
            }
        }

        private UInt128 mGUID;
        public UInt128 GUID
        {
            get
            {
                if (mGUID == UInt128.Zero)
                {
                    mGUID = WoWManager.WoWProcess.Memory.Read<UInt128>(Address + WowBuildInfoX64.ObjectGUID);
                }
                return mGUID;
            }
        }

        public string Name
        {
            get
            {
                string temp;
                if (!Names.TryGetValue(GUID, out temp))
                {
                    try
                    {
                        IntPtr nameBase = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfoX64.NpcNameBase);
                        IntPtr nameAddress = WoWManager.WoWProcess.Memory.Read<IntPtr>(nameBase + WowBuildInfoX64.NpcNameOffset);
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
        public WowPoint Location
        {
            get
            {
                if (!mLocationRead)
                {
                    mLocation = WoWManager.WoWProcess.Memory.Read<WowPoint>(Address + WowBuildInfoX64.UnitLocation);
                    mLocationRead = true;
                }
                return mLocation;
            }
        }

        private uint mHealth = uint.MaxValue;
        public uint Health
        {
            get
            {
                if (mHealth == uint.MaxValue)
                {
                    mHealth = WoWManager.WoWProcess.Memory.Read<uint>(Descriptors + WowBuildInfoX64.UnitHealth);
                }
                return mHealth;
            }
        }

        private uint mHealthMax = uint.MaxValue;
        public uint HealthMax
        {
            get
            {
                if (mHealthMax == uint.MaxValue)
                {
                    mHealthMax = WoWManager.WoWProcess.Memory.Read<uint>(Descriptors + WowBuildInfoX64.UnitHealthMax);
                }
                return mHealthMax;
            }
        }

        public bool Alive
        {
            get { return Health > 1; }
        }

        private int lootable = -1;
        public bool Lootable
        {
            get
            {
                if (lootable == -1)
                {
                    BitVector32 dynamicFlags = WoWManager.WoWProcess.Memory.Read<BitVector32>(Descriptors + WowBuildInfoX64.NpcDynamicFlags);
                    lootable = dynamicFlags[0x2] ? 1 : 0;
                }
                return lootable != 0;
            }
        }

        private uint mEntryID;
        internal uint EntryID
        {
            get
            {
                if (mEntryID == 0)
                {
                    IntPtr descriptors = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfoX64.GameObjectOwnerGUIDBase);
                    mEntryID = WoWManager.WoWProcess.Memory.Read<uint>(descriptors + WowBuildInfoX64.GameObjectEntryID);
                }
                return mEntryID;
            }
        }

    }
}