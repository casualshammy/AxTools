using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using AxTools.Helpers;

namespace AxTools.WoW.Internals
{
    /// <summary>
    /// Represents a World of Warcraft NPC object
    /// </summary>
    public sealed class WowNpc : WoWObjectBase
    {
        internal WowNpc(IntPtr pAddress)
        {
            Address = pAddress;
        }

        internal static readonly Dictionary<uint, string> Names = new Dictionary<uint, string>();

        private static int _maxNameLength = 125;
        private static Log2 log = new Log2("WowNpc");

        public IntPtr Address;

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

        private WoWGUID mGUID;
        public override WoWGUID GUID
        {
            get
            {
                if (mGUID == WoWGUID.Zero)
                {
                    mGUID = WoWManager.WoWProcess.Memory.Read<WoWGUID>(Address + WowBuildInfoX64.ObjectGUID);
                }
                return mGUID;
            }
        }

        public override string Name
        {
            get
            {
                if (!Names.TryGetValue(EntryID, out string temp))
                {
                    try
                    {
                        IntPtr nameBase = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfoX64.NpcNameBase);
                        IntPtr nameAddress = WoWManager.WoWProcess.Memory.Read<IntPtr>(nameBase + WowBuildInfoX64.NpcNameOffset);
                        byte[] nameBytes = WoWManager.WoWProcess.Memory.ReadBytes(nameAddress, _maxNameLength);
                        while (!nameBytes.Contains((byte)0))
                        {
                            _maxNameLength += 1;
                            log.Error("Max length for NPC names is increased to " + _maxNameLength);
                            nameBytes = WoWManager.WoWProcess.Memory.ReadBytes(nameAddress, _maxNameLength);
                        }
                        temp = Encoding.UTF8.GetString(nameBytes.TakeWhile(l => l != 0).ToArray());
                        Names.Add(EntryID, temp);
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
        public uint EntryID
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