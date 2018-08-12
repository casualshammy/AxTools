using AxTools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AxTools.WoW.Internals
{
    /// <summary>
    ///     World of Warcraft game object
    /// </summary>
    public class WowObject : WoWObjectBase
    {
        private static readonly Log2 log = new Log2("WowObject");

        internal WowObject(IntPtr address, WowProcess wow) : base(wow)
        {
            Address = address;
            WoWObjectsInfo inf = memory.Read<WoWObjectsInfo>(Address);
            GUID = inf.GUID;
            Bobbing = inf.Bobbing == 1;
            Location = inf.Location;
        }

        private static int _maxNameLength = 200;

        internal static readonly Dictionary<uint, string> Names = new Dictionary<uint, string>();

        public readonly IntPtr Address;

        public readonly WowPoint Location;

        internal readonly bool Bobbing;

        private WoWGUID mOwnerGUID;

        public WoWGUID OwnerGUID
        {
            get
            {
                if (mOwnerGUID == WoWGUID.Zero)
                {
                    IntPtr tempOwner = memory.Read<IntPtr>(Address + WowBuildInfoX64.GameObjectOwnerGUIDBase);
                    mOwnerGUID = memory.Read<WoWGUID>(tempOwner + WowBuildInfoX64.GameObjectOwnerGUIDOffset);
                }
                return mOwnerGUID;
            }
        }

        public override string Name
        {
            get
            {
                if (!Names.TryGetValue(EntryID, out var temp))
                {
                    try
                    {
                        IntPtr nameBase = memory.Read<IntPtr>(Address + WowBuildInfoX64.GameObjectNameBase);
                        IntPtr nameAddress = memory.Read<IntPtr>(nameBase + WowBuildInfoX64.GameObjectNameOffset);
                        byte[] nameBytes = memory.ReadBytes(nameAddress, _maxNameLength);
                        while (!nameBytes.Contains((byte)0))
                        {
                            _maxNameLength += 1;
                            log.Error("Max length for object names is increased to " + _maxNameLength);
                            nameBytes = memory.ReadBytes(nameAddress, _maxNameLength);
                        }
                        temp = Encoding.UTF8.GetString(nameBytes.TakeWhile(l => l != 0).ToArray());
                        Names.Add(EntryID, temp);
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
                return temp;
            }
        }

        private uint mEntryID;

        public uint EntryID
        {
            get
            {
                if (mEntryID == 0)
                {
                    IntPtr descriptors = memory.Read<IntPtr>(Address + WowBuildInfoX64.GameObjectOwnerGUIDBase);
                    mEntryID = memory.Read<uint>(descriptors + WowBuildInfoX64.GameObjectEntryID);
                }
                return mEntryID;
            }
        }

        public override void Target()
        {
            throw new InvalidOperationException("You cannot target object!");
        }
    }
}