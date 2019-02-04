using AxTools.Helpers;
using AxTools.WoW.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxTools.WoW.Internals
{
    /// <summary>
    ///     World of Warcraft player
    /// </summary>
    public class WowPlayer : WoWObjectBase
    {
        internal static readonly ConcurrentDictionary<WoWGUID, string> Names = new ConcurrentDictionary<WoWGUID, string>();
        private static readonly Log2 log = new Log2(nameof(WowPlayer));
        private float? rotation;
        private float? pitch;

        internal WowPlayer(IntPtr pAddress, WoWGUID guid, WowProcess wow) : base(wow)
        {
            Address = pAddress;
            MGUID = guid;
            WowPlayerInfo inf = memory.Read<WowPlayerInfo>(Address + WowBuildInfoX64.UnitInfoStart);
            TargetGUID = inf.TargetGUID;
            Health = inf.Health;
            HealthMax = inf.HealthMax;
            Power = inf.Power;
            PowerMax = inf.PowerMax;
            Alive = Health > 1;
            Level = inf.Level;
            InCombat = ((inf.UnitFlags >> 19) & 1) == 1; // Script_UnitAffectingCombat
            Class = inf.Class;
            IsMounted = inf.MountDisplayID != 0;
            Race = inf.Race;
            switch (Race)
            {
                case Race.Orc:
                case Race.Undead:
                case Race.Tauren:
                case Race.Troll:
                case Race.BloodElf:
                case Race.Goblin:
                case Race.PandarenHorde:
                    Faction = Faction.Horde;
                    break;

                case Race.Human:
                case Race.Dwarf:
                case Race.NightElf:
                case Race.Gnome:
                case Race.Draenei:
                case Race.Worgen:
                case Race.PandarenAlliance:
                    Faction = Faction.Alliance;
                    break;

                default:
                    Faction = Faction.Unknown;
                    break;
            }
        }

        public readonly IntPtr Address;

        /// <summary>
        ///     The GUID of the object this unit is targeting.
        /// </summary>
        public readonly WoWGUID TargetGUID;

        public readonly Race Race;

        public readonly Faction Faction;

        /// <summary>
        ///     The unit's level.
        /// </summary>
        public readonly uint Level;

        /// <summary>
        ///     The unit's health.
        /// </summary>
        public readonly uint Health;

        /// <summary>
        ///     The unit's maximum health.
        /// </summary>
        public readonly uint HealthMax;

        public readonly uint Power;

        public readonly uint PowerMax;

        public readonly bool InCombat;

        public readonly bool IsMounted;

        public readonly bool Alive;

        /// <summary>
        ///     Gets the class of the unit.
        /// </summary>
        public readonly WowPlayerClass Class;

        private uint castingSpellID = uint.MaxValue;

        public uint CastingSpellID
        {
            get
            {
                if (castingSpellID == uint.MaxValue)
                {
                    castingSpellID = memory.Read<uint>(Address + WowBuildInfoX64.UnitCastingID);
                }
                return castingSpellID;
            }
        }

        public string CastingSpellName => CastingSpellID != 0 ? Wowhead.GetSpellInfo((int)CastingSpellID).Name : null;

        private uint channelSpellID = uint.MaxValue;

        public uint ChannelSpellID => channelSpellID == uint.MaxValue ? (channelSpellID = memory.Read<uint>(Address + WowBuildInfoX64.UnitChannelingID)) : channelSpellID;

        public string ChannelSpellName => ChannelSpellID != 0 ? Wowhead.GetSpellInfo((int)ChannelSpellID).Name : null;

        protected WoWGUID MGUID;

        public override WoWGUID GUID
        {
            get
            {
                if (MGUID == WoWGUID.Zero)
                {
                    MGUID = memory.Read<WoWGUID>(Address + WowBuildInfoX64.ObjectGUID);
                }
                return MGUID;
            }
        }

        public byte[] GetGUIDBytes()
        {
            unsafe
            {
                return memory.ReadBytes(Address + WowBuildInfoX64.ObjectGUID, sizeof(WoWGUID));
            }
        }

        public override string Name
        {
            get
            {
                if (!Names.TryGetValue(GUID, out string temp))
                {
                    temp = GetNameFromMemorySafe();
                    if (string.IsNullOrWhiteSpace(temp))
                    {
                        temp = GetNameFromDB();
                        if (string.IsNullOrWhiteSpace(temp))
                        {
                            temp = Task.Factory.StartNew(GetNameFromLuaSafe).Result;
                            if (!string.IsNullOrWhiteSpace(temp))
                            {
                                SaveToDB(temp);
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(temp))
                    {
                        Names.TryAdd(GUID, temp);
                    }
                }
                return temp ?? "UNKNOWN";
            }
        }

        private string GetNameFromMemorySafe()
        {
            try
            {
                string name = null;
                var firstEntry = memory.Read<IntPtr>(memory.ImageBase + WowBuildInfoX64.NameCacheBase + WowBuildInfoX64.NameCacheNext);
                var nextEntry = firstEntry;
                while (true)
                {
                    if (memory.Read<WoWGUID>(nextEntry + WowBuildInfoX64.NameCacheGuid) == GUID)
                    {
                        byte[] nameBytes = memory.ReadBytes(nextEntry + WowBuildInfoX64.NameCacheName, 48).TakeWhile(l => l != 0).ToArray();
                        name = Encoding.UTF8.GetString(nameBytes);
                        break;
                    }
                    nextEntry = memory.Read<IntPtr>(nextEntry);
                    if (nextEntry == firstEntry) break;
                }
                return name;
            }
            catch
            {
                return null;
            }
        }

        private string GetNameFromLuaSafe()
        {
            try
            {
                var serverID = (ushort)((GUID.Low >> 42) & 0x1FFF);
                info = info ?? new GameInterface(wowProcess);
                return info.LuaGetValue("select(6, GetPlayerInfoByGUID(\"Player-" + serverID + "-" + GUID.High.ToString("X") + "\"))");
            }
            catch
            {
                return null;
            }
        }

        public bool IsFlying
        {
            get
            {
                var isFlyingPointer = 0xF0;
                var isFlyingOffset = 0x58;
                uint isFlyingMask = 0x1000000;
                var p1 = memory.Read<IntPtr>(Address + isFlyingPointer);
                var p2 = memory.Read<uint>(p1 + isFlyingOffset);
                return (p2 & isFlyingMask) != 0;
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
                    mLocation = memory.Read<WowPoint>(Address + WowBuildInfoX64.UnitLocation);
                    mLocationRead = true;
                }
                return mLocation;
            }
        }

        public float Rotation
        {
            get
            {
                if (!rotation.HasValue)
                {
                    rotation = memory.Read<float>(Address + WowBuildInfoX64.UnitRotation);
                }
                return rotation.Value;
            }
        }

        public float Pitch
        {
            get
            {
                if (!pitch.HasValue)
                {
                    pitch = memory.Read<float>(Address + WowBuildInfoX64.UnitPitch);
                }
                return pitch.Value;
            }
            set
            {
                return; // todo:
#pragma warning disable CS0162 // Unreachable code detected
                memory.Write(Address + WowBuildInfoX64.UnitPitch, value);
                pitch = null;
#pragma warning restore CS0162 // Unreachable code detected
            }
        }

        private List<WoWAura> auras;
        public List<WoWAura> Auras => auras ?? (auras = WoWAura.GetAurasForMemoryAddress(memory, Address));

        private static readonly object dbLock = new object();
        private static SQLiteConnection dbConnection;

        private void SaveToDB(string name)
        {
            lock (dbLock)
            {
                if (dbConnection == null)
                    dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\players.sqlite;Version=3;").OpenAndReturn();
                using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS players ( guid TEXT NOT NULL, name TEXT NOT NULL );" +
                        $"INSERT INTO players (guid, name) values ('{GUID}', '{name.Replace("'", "''")}')";
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        log.Error("[SaveToDB] Error-0: " + ex.Message +
                            $"\r\nINSERT INTO players (guid, name) values ('{GUID}', '{name.Replace("'", "''")}')");
                    }
                }
            }
        }

        private string GetNameFromDB()
        {
            lock (dbLock)
            {
                try
                {
                    if (dbConnection == null)
                        dbConnection = new SQLiteConnection($"Data Source={AppFolders.DataDir}\\players.sqlite;Version=3;").OpenAndReturn();
                    using (SQLiteCommand command = new SQLiteCommand(dbConnection))
                    {
                        command.CommandText =
                            "CREATE TABLE IF NOT EXISTS players ( guid TEXT NOT NULL, name TEXT NOT NULL );" +
                            $"SELECT * FROM players WHERE guid = '{GUID}';";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader["name"].ToString();
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("[GetNameFromDB] Error: " + ex.Message);
                    return null;
                }
            }
        }
    }

    public enum Faction
    {
        Unknown,
        Alliance,
        Horde
    }

    public enum Race : uint
    {
        Human = 0x1,
        Orc = 0x2,
        Dwarf = 0x3,
        NightElf = 0x4,
        Undead = 0x5,
        Tauren = 0x6,
        Gnome = 0x73,
        Troll = 0x74,
        BloodElf = 0x64A,
        Draenei = 0x65d,
        Worgen = 0x89B,
        Goblin = 0x89C,
        PandarenAlliance = 0x961,
        PandarenHorde = 0x962,
    }
}