using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxTools.Helpers;
using AxTools.WoW.Helpers;
using AxTools.WoW.PluginSystem.API;

namespace AxTools.WoW.Internals
{
    /// <summary>
    ///     World of Warcraft player
    /// </summary>
    public class WowPlayer : WoWObjectBase
    {
        internal static readonly Dictionary<WoWGUID, string> Names = new Dictionary<WoWGUID, string>();
        private static readonly Log2 log = new Log2($"WowPlayer");

        internal WowPlayer(IntPtr pAddress, WoWGUID guid) : this(pAddress)
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
            Class = info.Class;
            IsMounted = info.MountDisplayID != 0;
            switch (info.Race)
            {
                case 0x2: // Orc
                case 0x5: // Undead
                case 0x6: // Tauren
                case 0x74: // Troll
                case 0x64A: // BloodElf
                case 0x89C: // Goblin
                case 0x962: // Pandaren
                    Faction = Faction.Horde;
                    break;
                case 0x1: // Human
                case 0x3: // Dwarf
                case 0x4: // NightElf
                case 0x73: // Gnome
                case 0x65d: // Draenei
                case 0x89b: // Worgen
                case 2401: // Pandaren
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
                    castingSpellID = WoWManager.WoWProcess.Memory.Read<uint>(Address + WowBuildInfoX64.UnitCastingID);
                }
                return castingSpellID;
            }
        }
        public string CastingSpellName
        {
            get
            {
                return Wowhead.GetSpellInfo((int)CastingSpellID).Name;
            }
        }

        private uint channelSpellID = uint.MaxValue;
        public uint ChannelSpellID
        {
            get
            {
                if (channelSpellID == uint.MaxValue)
                {
                    channelSpellID = WoWManager.WoWProcess.Memory.Read<uint>(Address + WowBuildInfoX64.UnitChannelingID);
                }
                return channelSpellID;
            }
        }
        public string ChannelSpellName
        {
            get
            {
                return Wowhead.GetSpellInfo((int)ChannelSpellID).Name;
            }
        }

        protected WoWGUID MGUID;
        public override WoWGUID GUID
        {
            get
            {
                if (MGUID == WoWGUID.Zero)
                {
                    MGUID = WoWManager.WoWProcess.Memory.Read<WoWGUID>(Address + WowBuildInfoX64.ObjectGUID);
                }
                return MGUID;
            }
        }

        public byte[] GetGUIDBytes()
        {
            unsafe
            {
                return WoWManager.WoWProcess.Memory.ReadBytes(Address + WowBuildInfoX64.ObjectGUID, sizeof(WoWGUID));
            }
        }

        public override string Name
        {
            get
            {
                //return "NOT_IMPLEMENTED_YET";
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
                        Names.Add(GUID, temp);
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
                IntPtr firstEntry = WoWManager.WoWProcess.Memory.Read<IntPtr>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.NameCacheBase + WowBuildInfoX64.NameCacheNext);
                IntPtr nextEntry = firstEntry;
                while (true)
                {
                    if (WoWManager.WoWProcess.Memory.Read<WoWGUID>(nextEntry + WowBuildInfoX64.NameCacheGuid) == GUID)
                    {
                        byte[] nameBytes = WoWManager.WoWProcess.Memory.ReadBytes(nextEntry + WowBuildInfoX64.NameCacheName, 48).TakeWhile(l => l != 0).ToArray();
                        name = Encoding.UTF8.GetString(nameBytes);
                        break;
                    }
                    nextEntry = WoWManager.WoWProcess.Memory.Read<IntPtr>(nextEntry);
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
                ushort serverID = (ushort)((GUID.Low >> 42) & 0x1FFF);
                // ReSharper disable ImpureMethodCallOnReadonlyValueField
                return Lua.GetValue("select(6, GetPlayerInfoByGUID(\"Player-" + serverID + "-" + GUID.High.ToString("X") + "\"))");
                // ReSharper restore ImpureMethodCallOnReadonlyValueField
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
                int isFlyingPointer = 0x210;
                int isFlyingOffset = 0x58;
                uint isFlyingMask = 0x1000000;
                IntPtr p1 = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + isFlyingPointer);
                uint p2 = WoWManager.WoWProcess.Memory.Read<uint>(p1 + isFlyingOffset);
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
                        if (rawAura.SpellId != 0)
                        {
                            WoWAura aura = new WoWAura(rawAura.OwnerGUID, rawAura.SpellId, rawAura.Stack, (uint) (rawAura.TimeLeftInMs - Environment.TickCount));
                            auras.Add(aura);
                        }
                    }
                }
                return auras;
            }
        }

        private static object dbLock = new object();
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

}