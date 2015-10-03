using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

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
            IntPtr desc = WoWManager.WoWProcess.Memory.Read<IntPtr>(pAddress + WowBuildInfoX64.UnitDescriptors);
            WowPlayerInfo info = WoWManager.WoWProcess.Memory.Read<WowPlayerInfo>(desc);
            TargetGUID = info.TargetGUID;
            Health = info.Health;
            HealthMax = info.HealthMax;
            Level = info.Level;
            IsAlliance = info.Race == 0x89b || info.Race == 0x65d || info.Race == 0x73 || info.Race == 4 || info.Race == 3 || info.Race == 1 || info.Race == 2401;
            Class = info.Class;
        }

        internal static readonly Dictionary<UInt128, string> Names = new Dictionary<UInt128, string>();

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
                    try
                    {
                        ushort serverID = (ushort) ((GUID.Low >> 42) & 0x1FFF);
                        // ReSharper disable ImpureMethodCallOnReadonlyValueField
                        temp = WoWDXInject.GetFunctionReturn("select(6, GetPlayerInfoByGUID(\"Player-" + serverID + "-" + GUID.High.ToString("X") + "\"))");
                        // ReSharper restore ImpureMethodCallOnReadonlyValueField
                        if (!string.IsNullOrWhiteSpace(temp))
                        {
                            Names.Add(GUID, temp);
                        }
                        /*
                        /*      With remote server support
                        /* 
                        ushort serverID = (ushort) ((GUID.Low >> 42) & 0x1FFF);
                        string playerGUID = "Player-" + serverID + "-" + GUID.High.ToString("X");
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.Encoding = Encoding.UTF8;
                            try
                            {
                                temp = webClient.DownloadString("http://25.202.130.226:5000/get_player_name+guid=" + playerGUID);
                            }
                            finally
                            {
                                
                            }
                            if (!string.IsNullOrWhiteSpace(temp) && temp != "UNKNOWN")
                            {
                                Names.Add(GUID, temp);
                            }
                            else
                            {
                                temp = WoWDXInject.GetFunctionReturn("select(6, GetPlayerInfoByGUID(\"" + playerGUID + "\"))");
                                if (!string.IsNullOrWhiteSpace(temp))
                                {
                                    Names.Add(GUID, temp);
                                    webClient.DownloadString("http://25.202.130.226:5000/add_player_name+guid=" + playerGUID + "+name=" + temp);
                                }
                            }
                        }
                        */
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
                    mLocation = WoWManager.WoWProcess.Memory.Read<WowPoint>(Address + WowBuildInfoX64.UnitLocation);
                    mLocationRead = true;
                }
                return mLocation;
            }
        }
        
    }
}