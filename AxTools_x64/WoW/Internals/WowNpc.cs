using AxTools.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace AxTools.WoW.Internals
{
    /// <summary>
    /// Represents a World of Warcraft NPC object
    /// </summary>
    public sealed class WowNpc : WoWObjectBase
    {
        internal WowNpc(IntPtr pAddress, WowProcess wow) : base(wow)
        {
            Address = pAddress;
        }

        internal static readonly Dictionary<uint, string> Names = new Dictionary<uint, string>();

        private static int _maxNameLength = 125;
        private static readonly Log2 log = new Log2(nameof(WowNpc));

        public IntPtr Address;

        private IntPtr mDescriptors = IntPtr.Zero;

        internal IntPtr Descriptors
        {
            get
            {
                if (mDescriptors == IntPtr.Zero)
                {
                    mDescriptors = memory.Read<IntPtr>(Address + WowBuildInfoX64.UnitDescriptors);
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
                    mGUID = memory.Read<WoWGUID>(Address + WowBuildInfoX64.ObjectGUID);
                }
                return mGUID;
            }
        }

        public unsafe byte[] GetGUIDBytes()
        {
            return memory.ReadBytes(Address + WowBuildInfoX64.ObjectGUID, sizeof(WoWGUID));
        }

        public override string Name
        {
            get
            {
                if (!Names.TryGetValue(EntryID, out string temp))
                {
                    try
                    {
                        var nameBase = memory.Read<IntPtr>(Address + WowBuildInfoX64.NpcNameBase);
                        var nameAddress = memory.Read<IntPtr>(nameBase + WowBuildInfoX64.NpcNameOffset);
                        byte[] nameBytes = memory.ReadBytes(nameAddress, _maxNameLength);
                        while (!nameBytes.Contains((byte)0))
                        {
                            _maxNameLength += 1;
                            log.Error("Max length for NPC names is increased to " + _maxNameLength);
                            nameBytes = memory.ReadBytes(nameAddress, _maxNameLength);
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
                    mLocation = memory.Read<WowPoint>(Address + WowBuildInfoX64.UnitLocation);
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
                    mHealth = memory.Read<uint>(Descriptors + WowBuildInfoX64.UnitHealth);
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
                    mHealthMax = memory.Read<uint>(Descriptors + WowBuildInfoX64.UnitHealthMax);
                }
                return mHealthMax;
            }
        }

        public bool Alive => Health > 1;

        private int lootable = -1;

        public bool Lootable
        {
            get
            {
                if (lootable == -1)
                {
                    BitVector32 dynamicFlags = memory.Read<BitVector32>(Descriptors + WowBuildInfoX64.NpcDynamicFlags);
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
                    var descriptors = memory.Read<IntPtr>(Address + WowBuildInfoX64.GameObjectOwnerGUIDBase);
                    mEntryID = memory.Read<uint>(descriptors + WowBuildInfoX64.GameObjectEntryID);
                }
                return mEntryID;
            }
        }

        private List<WoWAura> auras;
        public List<WoWAura> Auras => auras ?? (auras = WoWAura.GetAurasForMemoryAddress(memory, Address));

        public string GetGameGUID()
        {
            var serverID = (ushort)((GUID.Low >> 42) & 0x1FFF);
            var instanceID = (ushort)((GUID.Low >> 29) & 0x1FFF);
            var guidBytes = GetGUIDBytes();
            string spawnID = BitConverter.ToString(new[] { guidBytes[4], guidBytes[3], guidBytes[2], guidBytes[1], guidBytes[0] }).Replace("-", "");
            var zoneID = BitConverter.ToUInt16(new[] { guidBytes[5], guidBytes[6] }, 0);
            return $"Creature-0-{serverID}-{instanceID}-{zoneID}-{EntryID}-{spawnID}";
        }
    }
}