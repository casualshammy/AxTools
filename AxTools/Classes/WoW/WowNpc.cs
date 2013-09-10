using System;
using System.Collections.Generic;
using System.Text;

namespace AxTools.Classes.WoW
{
    internal sealed class WowNpc
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
                    mDescriptors = WoW.WProc.Memory.Read<IntPtr>(Address + WowBuildInfo.UnitDescriptors);
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
                    mGUID = WoW.WProc.Memory.Read<ulong>(Address + WowBuildInfo.ObjectGUID);
                }
                return mGUID;
            }
        }

        //private string mName;
        internal string Name
        {
            get
            {
                string temp;
                if (!Names.TryGetValue(GUID, out temp))
                {
                    try
                    {
                        IntPtr nameBase = WoW.WProc.Memory.Read<IntPtr>(Address + WowBuildInfo.NpcNameBase);
                        IntPtr nameAddress = WoW.WProc.Memory.Read<IntPtr>(nameBase + WowBuildInfo.NpcNameOffset);
                        temp = Encoding.UTF8.GetString(WoW.WProc.Memory.ReadBytes(nameAddress, 80)).Split('\0')[0];
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

        private WowPointF mLocation;
        internal WowPointF Location
        {
            get
            {
                return mLocation ?? (mLocation = new WowPointF(WoW.WProc.Memory.Read<float>(Address + WowBuildInfo.UnitLocationX),
                                                               WoW.WProc.Memory.Read<float>(Address + WowBuildInfo.UnitLocationY),
                                                               WoW.WProc.Memory.Read<float>(Address + WowBuildInfo.UnitLocationZ)));
            }
        }

        private uint mHealth = UInt32.MaxValue;
        internal uint Health
        {
            get
            {
                if (mHealth == UInt32.MaxValue)
                {
                    mHealth = WoW.WProc.Memory.Read<uint>(Descriptors + WowBuildInfo.UnitHealth);
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
                    mHealthMax = WoW.WProc.Memory.Read<uint>(Descriptors + WowBuildInfo.UnitHealthMax);
                }
                return mHealthMax;
            }
        }
        
        internal static int SortByDistance(WowNpc x, WowNpc y)
        {
            double distance1 = x.Location.Distance(WoW.LocalPlayer.Location);
            double distance2 = y.Location.Distance(WoW.LocalPlayer.Location);
            if (distance1 > distance2)
            {
                return 1;
            }
            if (distance1 < distance2)
            {
                return -1;
            }
            return 0;
        }
    }
}