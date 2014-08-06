using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AxTools.Classes;
using Newtonsoft.Json;

namespace AxTools.WoW.Management.ObjectManager
{
    /// <summary>
    ///     World of Warcraft player
    /// </summary>
    public class WowPlayer
    {
        internal WowPlayer(IntPtr pAddress, ulong guid)
        {
            Address = pAddress;
            MGUID = guid;
            IntPtr desc = WoWManager.WoWProcess.Memory.Read<IntPtr>(pAddress + WowBuildInfo.UnitDescriptors);
            WowPlayerInfo info = WoWManager.WoWProcess.Memory.Read<WowPlayerInfo>(desc + WowBuildInfo.UnitTargetGUID);
            TargetGUID = info.TargetGUID;
            Health = info.Health;
            HealthMax = info.HealthMax;
            Level = info.Level;
            IsAlliance = info.FactionTemplate == 0x89b || info.FactionTemplate == 0x65d || info.FactionTemplate == 0x73 || info.FactionTemplate == 4 || info.FactionTemplate == 3 || info.FactionTemplate == 1 ||
                         info.FactionTemplate == 2401;
            Class = (WowPlayerClass) info.Class;
        }

        internal static readonly Dictionary<ulong, string> Names;

        static WowPlayer()
        {
            Names = new Dictionary<ulong, string>();
            // todo
            if (Settings.Instance.UserID == "Axio-5GDMJHD20R")
            {
                if (File.Exists(Application.StartupPath + "\\WoWNames.json"))
                {
                    Names = JsonConvert.DeserializeObject<Dictionary<ulong, string>>(File.ReadAllText(Application.StartupPath + "\\WoWNames.json", Encoding.UTF8));
                    Log.Print("WowPlayer names cache loading finished, loaded " + Names.Count + " entries");
                }
                Application.ApplicationExit += ApplicationOnApplicationExit;
            }
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs eventArgs)
        {
            Application.ApplicationExit -= ApplicationOnApplicationExit;
            try
            {
                string json = JsonConvert.SerializeObject(Names, Formatting.Indented);
                try
                {
                    File.WriteAllText(Application.StartupPath + "\\WoWNames.json", json, Encoding.UTF8);
                    Log.Print("WowPlayer names cache is saved, total " + Names.Count + " entries");
                }
                catch (Exception ex)
                {
                    Log.Print("WowPlayer names cache writing error: " + ex.Message, true);
                }
            }
            catch (Exception ex)
            {
                Log.Print("WowPlayer names cache serialization error: " + ex.Message, true);
            }
        }

        internal readonly IntPtr Address;

        /// <summary>
        ///     The GUID of the object this unit is targeting.
        /// </summary>
        public readonly ulong TargetGUID;

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
        internal WowPlayerClass Class;

        protected ulong MGUID;
        public ulong GUID
        {
            get
            {
                if (MGUID == 0)
                {
                    MGUID = WoWManager.WoWProcess.Memory.Read<ulong>(Address + WowBuildInfo.ObjectGUID);
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
                    try
                    {
                        uint nameMask = WoWManager.WoWProcess.Memory.Read<uint>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfo.UnitNameCachePointer +
                                                                    WowBuildInfo.UnitNameMaskOffset);
                        uint nameBase = WoWManager.WoWProcess.Memory.Read<uint>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfo.UnitNameCachePointer +
                                                                    WowBuildInfo.UnitNameBaseOffset);
                        uint var4 = nameMask;
                        uint var5 = nameBase;
                        nameMask &= (uint) GUID;
                        nameMask += nameMask*2;
                        nameMask = (nameBase + (nameMask*4) + 4);
                        nameMask = WoWManager.WoWProcess.Memory.Read<uint>((IntPtr) (nameMask + 4));
                        while (WoWManager.WoWProcess.Memory.Read<uint>((IntPtr) nameMask) != (uint) GUID)
                        {
                            nameBase = (uint) GUID;
                            nameBase &= var4;
                            nameBase += nameBase*2;
                            nameBase = WoWManager.WoWProcess.Memory.Read<uint>((IntPtr) (var5 + (nameBase*4)));
                            nameBase += nameMask;
                            nameMask = WoWManager.WoWProcess.Memory.Read<uint>((IntPtr) (nameBase + 4));
                        }
                        byte[] nameBytes = WoWManager.WoWProcess.Memory.ReadBytes((IntPtr) (nameMask + WowBuildInfo.UnitNameStringOffset), 80);
                        temp = Encoding.UTF8.GetString(nameBytes).Split('\0')[0];
                        if (!string.IsNullOrWhiteSpace(temp))
                        {
                            Names.Add(GUID, temp);
                        }
                        else
                        {
                            Log.Print("Null name!", true);
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

        // We don't use System.Nullable<> because it's for 40% slower
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