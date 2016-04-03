using System;
using AxTools.WoW.PluginSystem.API;

namespace AxTools.WoW.Management.ObjectManager
{
    public class WoWCorpse
    {
        internal IntPtr Address;
        private UInt128 ownerGUID = UInt128.Zero;
        private UInt128 guid = UInt128.Zero;
        private bool mLocationRead;
        private WowPoint mLocation;

        internal WoWCorpse(IntPtr address)
        {
            Address = address;
        }

        public string OwnerName
        {
            get
            {
                string temp;
                if (!WowPlayer.Names.TryGetValue(OwnerGUID, out temp))
                {
                    try
                    {
                        ushort serverID = (ushort)((OwnerGUID.Low >> 42) & 0x1FFF);
                        temp = GameFunctions.Lua_GetFunctionReturn("select(6, GetPlayerInfoByGUID(\"Player-" + serverID + "-" + OwnerGUID.High.ToString("X") + "\"))");
                        if (!string.IsNullOrWhiteSpace(temp))
                        {
                            WowPlayer.Names.Add(OwnerGUID, temp);
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

        public UInt128 OwnerGUID
        {
            get
            {
                if (ownerGUID == UInt128.Zero)
                {
                    // todo
                    ownerGUID = WoWManager.WoWProcess.Memory.Read<UInt128>(Address + 0x728);
                }
                return ownerGUID;
            }
        }

        public UInt128 GUID
        {
            get
            {
                if (guid == UInt128.Zero)
                {
                    guid = WoWManager.WoWProcess.Memory.Read<UInt128>(Address + WowBuildInfoX64.ObjectGUID);
                }
                return guid;
            }
        }

        public WowPoint Location
        {
            get
            {
                if (!mLocationRead)
                {
                    // todo
                    mLocation = WoWManager.WoWProcess.Memory.Read<WowPoint>(Address + 0x248);
                    mLocationRead = true;
                }
                return mLocation;
            }
        }

        public byte DisplayID
        {
            get
            {
                return WoWManager.WoWProcess.Memory.Read<byte>(Address + 0x728 + 16);
            }
        }

    }
}
