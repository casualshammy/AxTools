using System;
using System.Collections.Generic;
using System.Text;

namespace AxTools.WoW.Management.ObjectManager
{
    /// <summary>
    ///     World of Warcraft player
    /// </summary>
    public class WowPlayer
    {
        internal WowPlayer(IntPtr pAddress, UInt128 guid) : this(pAddress)
        {
            MGUID = guid;
        }

        internal WowPlayer(IntPtr pAddress)
        {
            Address = pAddress;
            IntPtr desc = WoWManager.WoWProcess.Memory.Read<IntPtr>(pAddress + WowBuildInfo.UnitDescriptors);
            WowPlayerInfo info = WoWManager.WoWProcess.Memory.Read<WowPlayerInfo>(desc);
            TargetGUID = info.TargetGUID;
            Health = info.Health;
            HealthMax = info.HealthMax;
            Level = info.Level;
            IsAlliance = info.Race == 0x89b || info.Race == 0x65d || info.Race == 0x73 || info.Race == 4 || info.Race == 3 || info.Race == 1 || info.Race == 2401;
            Class = info.Class;
        }

        internal static readonly Dictionary<UInt128, string> Names = new Dictionary<UInt128, string>();

        //static WowPlayer()
        //{
        //    
        //    if (Settings.Instance.UserID == "Axio-5GDMJHD20R")
        //    {
        //        Stopwatch stopwatch = Stopwatch.StartNew();
        //        if (File.Exists(Globals.CfgPath + "\\WoWNames.json"))
        //        {
        //            string rawText = File.ReadAllText(Globals.CfgPath + "\\WoWNames.json", Encoding.UTF8);
        //            Names = JsonConvert.DeserializeObject<Dictionary<ulong, string>>(rawText);
        //            Log.Print("WowPlayer names cache loading finished, loaded " + Names.Count + " entries, this took " + stopwatch.ElapsedMilliseconds + "ms");
        //        }
        //        Application.ApplicationExit += ApplicationOnApplicationExit;
        //    }
        //}

        //private static void ApplicationOnApplicationExit(object sender, EventArgs eventArgs)
        //{
        //    Application.ApplicationExit -= ApplicationOnApplicationExit;
        //    try
        //    {
        //        Stopwatch stopwatch = Stopwatch.StartNew();
        //        string json = JsonConvert.SerializeObject(Names, Formatting.Indented);
        //        try
        //        {
        //            File.WriteAllText(Globals.CfgPath + "\\WoWNames.json", json, Encoding.UTF8);
        //            Log.Print("WowPlayer names cache is saved, total " + Names.Count + " entries, this took " + stopwatch.ElapsedMilliseconds + "ms");
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Print("WowPlayer names cache writing error: " + ex.Message, true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Print("WowPlayer names cache serialization error: " + ex.Message, true);
        //    }
        //}

        internal readonly IntPtr Address;

        /// <summary>
        ///     The GUID of the object this unit is targeting.
        /// </summary>
        public readonly UInt128 TargetGUID;

        internal readonly bool IsAlliance;

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
        internal readonly uint HealthMax;

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
                    MGUID = WoWManager.WoWProcess.Memory.Read<UInt128>(Address + WowBuildInfo.ObjectGUID);
                }
                return MGUID;
            }
        }

        //[StructLayout(LayoutKind.Explicit)]
        //private struct UnitCacheEntry
        //{
        //    [FieldOffset(16)]
        //    internal readonly UInt128 GUID;
        //    [FieldOffset(112)]
        //    private readonly uint Race;
        //    [FieldOffset(120)]
        //    private readonly uint Class;
        //}

        private IntPtr GetUnitCachePointer()
        {
            IntPtr next = WoWManager.WoWProcess.Memory.Read<IntPtr>((IntPtr)(WowBuildInfo.UnitNameCachePointer), true);
            IntPtr ptr = next;
            while (true)
            {
                UInt128 guid = WoWManager.WoWProcess.Memory.Read<UInt128>(ptr + WowBuildInfo.UnitNameCacheGUIDOffset);
                //UnitCacheEntry tempUnitCacheEntry = WoWManager.WoWProcess.Memory.Read<UnitCacheEntry>(ptr);
                if (guid == GUID)
                {
                    return ptr;
                }
                ptr = WoWManager.WoWProcess.Memory.Read<IntPtr>(ptr);
                if (ptr == next) break;
            }
            return IntPtr.Zero;
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
                        IntPtr unitCachePoiter = GetUnitCachePointer();
                        if (unitCachePoiter != IntPtr.Zero)
                        {
                            temp = WoWManager.WoWProcess.Memory.ReadString(unitCachePoiter + WowBuildInfo.UnitNameCacheNameOffset, Encoding.UTF8, 80);
                            //byte[] name = WoWManager.WoWProcess.Memory.ReadBytes(unitCachePoiter + WowBuildInfo.UnitNameCacheNameOffset, 80);
                            //temp = Encoding.UTF8.GetString(name).Split('\0')[0];
                            if (!string.IsNullOrWhiteSpace(temp))
                            {
                                Names.Add(GUID, temp);
                            }
                        }
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
                return temp;
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
                    mLocation = WoWManager.WoWProcess.Memory.Read<WowPoint>(Address + WowBuildInfo.UnitLocation);
                    mLocationRead = true;
                }
                return mLocation;
            }
        }
        
    }
}