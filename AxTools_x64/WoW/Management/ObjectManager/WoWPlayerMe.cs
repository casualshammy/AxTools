using System;

// ReSharper disable InconsistentNaming
namespace AxTools.WoW.Management.ObjectManager
{
    public sealed class WoWPlayerMe : WowPlayer
    {
        internal WoWPlayerMe(IntPtr address, UInt128 guid) : base(address)
        {
            MGUID = guid;
        }

        internal WoWPlayerMe(IntPtr address) : base(address)
        {

        }

        private uint castingSpellID;
        public uint CastingSpellID
        {
            get
            {
                if (castingSpellID == 0)
                {
                    castingSpellID = WoWManager.WoWProcess.Memory.Read<uint>(Address + WowBuildInfoX64.UnitCastingID);
                }
                return castingSpellID;
            }
        }

        private uint channelSpellID;
        public uint ChannelSpellID
        {
            get
            {
                if (channelSpellID == 0)
                {
                    channelSpellID = WoWManager.WoWProcess.Memory.Read<uint>(Address + WowBuildInfoX64.UnitChannelingID);
                }
                return channelSpellID;
            }
        }

        private bool rotationRead;
        private float rotation;
        public float Rotation
        {
            get
            {
                if (!rotationRead)
                {
                    rotation = WoWManager.WoWProcess.Memory.Read<float>(Address + WowBuildInfoX64.UnitRotation);
                    rotationRead = true;
                }
                return rotation;
            }
        }

        public bool Is_Looting
        {
            get { return IsLooting; }
        }

        public static bool IsLooting
        {
            get
            {
                return WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.PlayerIsLooting) != 0;
            }
        }

        //public new string Name
        //{
        //    get { return Name; }
        //}
        //public static new string Name
        //{
        //    get
        //    {
        //        string name = Encoding.UTF8.GetString(WoWManager.WoWProcess.Memory.ReadBytes(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.PlayerName, 24));
        //        int index = name.IndexOf('\0');
        //        if (index != -1)
        //        {
        //            name.Remove(index);
        //        }
        //        return name;
        //    }
        //}

        public static uint Zone_ID
        {
            get { return ZoneID; }
        }

        public static uint ZoneID
        {
            get
            {
                return WoWManager.WoWProcess.Memory.Read<uint>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.PlayerZoneID);
            }
        }
    
    }
}
// ReSharper restore InconsistentNaming