using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AxTools.WoW.Management.ObjectManager
{
    /// <summary>
    ///     World of Warcraft player
    /// </summary>
    public class WowPlayer
    {

        #region Static fields

        internal static readonly Dictionary<UInt128, string> Names = new Dictionary<UInt128, string>();

        internal static readonly uint[] HordeRaces = // just because Enum.IsDefined is sooooooo slow
        {
            0x2, // Orc
            0x5, // Undead
            0x6, // Tauren
            0x74, // Troll
            0x64A, // BloodElf
            0x89C, // Goblin
            0x962 // Pandaren
        };

        internal static readonly uint[] AllianceRaces = // just because Enum.IsDefined is sooooooo slow
        {
            0x1, // Human
            0x3, // Dwarf
            0x4, // NightElf
            0x73, // Gnome
            0x65d, // Draenei
            0x89b, // Worgen
            2401 // Pandaren
        };

        #endregion

        internal WowPlayer(IntPtr pAddress, UInt128 guid) : this(pAddress)
        {
            MGUID = guid;
        }

        internal WowPlayer(IntPtr pAddress)
        {
            Address = pAddress;
            IntPtr desc = WoWManager.WoWProcess.Memory.Read<IntPtr>(pAddress + WowBuildInfoX64.UnitDescriptors);
            WowPlayerInfo info = WoWManager.WoWProcess.Memory.Read<WowPlayerInfo>(desc);
            TargetGUID = info.TargetGUID;
            Health = info.Health;
            HealthMax = info.HealthMax;
            Alive = info.Health > 1;
            Level = info.Level;
            InCombat = ((info.UnitFlags >> 19) & 1) == 1; // Script_UnitAffectingCombat
            if (AllianceRaces.Contains(info.Race))
            {
                Faction = Faction.Alliance;
                IsAlliance = true;
            }
            else if (HordeRaces.Contains(info.Race))
            {
                Faction = Faction.Horde;
                IsAlliance = false;
            }
            else
            {
                Faction = Faction.Unknown;
                IsAlliance = false;
            }
            
            Class = info.Class;
        }

        internal readonly IntPtr Address;

        /// <summary>
        ///     The GUID of the object this unit is targeting.
        /// </summary>
        public readonly UInt128 TargetGUID;

        public readonly bool IsAlliance;

        public readonly Faction Faction;

        /// <summary>
        ///     The unit's level.
        /// </summary>
        internal readonly uint Level;

        /// <summary>
        ///     The unit's health.
        /// </summary>
        public readonly uint Health;

        /// <summary>
        ///     The unit's maximum health.
        /// </summary>
        public readonly uint HealthMax;

        public readonly bool InCombat;

        public bool Alive;

        /// <summary>
        ///     Gets the class of the unit.
        /// </summary>
        internal readonly WowPlayerClass Class;
        
        protected UInt128 MGUID;
        public UInt128 GUID
        {
            get
            {
                if (MGUID == UInt128.Zero)
                {
                    MGUID = WoWManager.WoWProcess.Memory.Read<UInt128>(Address + WowBuildInfoX64.ObjectGUID);
                }
                return MGUID;
            }
        }

        internal string Name
        {
            get
            {
                string temp;
                if (!Names.TryGetValue(GUID, out temp))
                {
                    IntPtr firstEntry = WoWManager.WoWProcess.Memory.Read<IntPtr>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.NameCacheBase + WowBuildInfoX64.NameCacheNext);
                    IntPtr nextEntry = firstEntry;
                    while (true)
                    {
                        if (WoWManager.WoWProcess.Memory.Read<UInt128>(nextEntry + WowBuildInfoX64.NameCacheGuid) == GUID)
                        {
                            byte[] nameBytes = WoWManager.WoWProcess.Memory.ReadBytes(nextEntry + WowBuildInfoX64.NameCacheName, 48).TakeWhile(l => l != 0).ToArray();
                            temp = Encoding.UTF8.GetString(nameBytes);
                            break;
                        }
                        nextEntry = WoWManager.WoWProcess.Memory.Read<IntPtr>(nextEntry);
                        if (nextEntry == firstEntry) break;
                    }
                    if (!string.IsNullOrWhiteSpace(temp))
                    {
                        Names.Add(GUID, temp);
                    }
                }
                return temp ?? "UNKNOWN";
            }
        }

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

        private List<WoWAura> auras;
        public List<WoWAura> Auras
        {
            get
            {
                if (auras == null)
                {
                    int auraStructSize;
                    unsafe
                    {
                        auraStructSize = sizeof (WoWAura);
                    }
                    auras = new List<WoWAura>();
                    IntPtr table = Address + WowBuildInfoX64.AuraTable1;
                    int auraCount = WoWManager.WoWProcess.Memory.Read<int>(Address + WowBuildInfoX64.AuraCount1);
                    if (auraCount == -1)
                    {
                        table = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfoX64.AuraTable2);
                        auraCount = WoWManager.WoWProcess.Memory.Read<int>(Address + WowBuildInfoX64.AuraCount2);
                    }
                    for (int i = 0; i < auraCount; i++)
                    {
                        WoWAura rawAura = WoWManager.WoWProcess.Memory.Read<WoWAura>(table + i * auraStructSize);
                        if (rawAura.SpellId > 0)
                        {
                            WoWAura aura = new WoWAura(rawAura.OwnerGUID, rawAura.SpellId, rawAura.Stack, rawAura.TimeLeftInMs - Environment.TickCount);
                            auras.Add(aura);
                        }
                    }
                }
                return auras;
            }
        }

    }

    public enum Faction
    {
        Unknown,
        Alliance,
        Horde
    }

}