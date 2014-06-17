using System;
using System.Text;

namespace AxTools.Classes.WoW.Management.ObjectManager
{
    public class WoWPlayerMe : WowPlayer
    {
        internal WoWPlayerMe(IntPtr address, ulong guid = 0) : base(address)
        {
            MGUID = guid;
        }

        public new string Name
        {
            get
            {
                string temp;
                if (!Names.TryGetValue(GUID, out temp))
                {
                    temp = Encoding.UTF8.GetString(WoWManager.WoWProcess.Memory.ReadBytes(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfo.PlayerName, 24));
                    if (temp.Contains("\0"))
                    {
                        temp = temp.Split('\0')[0];
                    }
                    if (!string.IsNullOrWhiteSpace(temp))
                    {
                        Names.Add(GUID, temp);
                    }
                }
                return temp;
            }
        }

        private uint castingSpellID;
        public uint CastingSpellID
        {
            get
            {
                if (castingSpellID == 0)
                {
                    castingSpellID = WoWManager.WoWProcess.Memory.Read<uint>(Address + WowBuildInfo.UnitCastingID);
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
                    channelSpellID = WoWManager.WoWProcess.Memory.Read<uint>(Address + WowBuildInfo.UnitChannelingID);
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
                    rotation = WoWManager.WoWProcess.Memory.Read<float>(Address + WowBuildInfo.UnitRotation);
                    rotationRead = true;
                }
                return rotation;
            }
        }

    }
}
