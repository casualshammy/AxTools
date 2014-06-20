using System;
using System.Collections.Generic;
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

        internal static readonly Dictionary<ulong, string> Names = new Dictionary<ulong, string>();

        internal IntPtr Address;

        private IntPtr mDescriptors = IntPtr.Zero;
        internal IntPtr Descriptors
        {
            get
            {
                if (mDescriptors == IntPtr.Zero)
                {
                    mDescriptors = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfo.UnitDescriptors);
                }
                return mDescriptors;
            }
        }

        private ulong mGUID;
        internal ulong GUID
        {
            get
            {
                if (mGUID == 0)
                {
                    mGUID = WoWManager.WoWProcess.Memory.Read<ulong>(Address + WowBuildInfo.ObjectGUID);
                }
                return mGUID;
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
                        IntPtr nameBase = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfo.NpcNameBase);
                        IntPtr nameAddress = WoWManager.WoWProcess.Memory.Read<IntPtr>(nameBase + WowBuildInfo.NpcNameOffset);
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
                    mLocation = WoWManager.WoWProcess.Memory.Read<WowPoint>(Address + WowBuildInfo.UnitLocation);
                    mLocationRead = true;
                }
                return mLocation;
            }
        }

        private uint mHealth = UInt32.MaxValue;
        internal uint Health
        {
            get
            {
                if (mHealth == UInt32.MaxValue)
                {
                    mHealth = WoWManager.WoWProcess.Memory.Read<uint>(Descriptors + WowBuildInfo.UnitHealth);
                }
                return mHealth;
            }
        }

        private uint mHealthMax = UInt32.MaxValue;
        internal uint HealthMax
        {
            get
            {
                if (mHealthMax == UInt32.MaxValue)
                {
                    mHealthMax = WoWManager.WoWProcess.Memory.Read<uint>(Descriptors + WowBuildInfo.UnitHealthMax);
                }
                return mHealthMax;
            }
        }

    }
}