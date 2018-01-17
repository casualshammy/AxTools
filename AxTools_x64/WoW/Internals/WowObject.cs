﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW.PluginSystem.API;

namespace AxTools.WoW.Internals
{
    /// <summary>
    ///     World of Warcraft game object
    /// </summary>
    public class WowObject : WoWObjectBase
    {
        internal WowObject(IntPtr pAddress)
        {
            Address = pAddress;
            WoWObjectsInfo info = WoWManager.WoWProcess.Memory.Read<WoWObjectsInfo>(Address);
            GUID = info.GUID;
            Bobbing = info.Bobbing == 1;
            Location = info.Location;
        }

        private static int _maxNameLength = 80;

        internal static readonly Dictionary<WoWGUID, string> Names = new Dictionary<WoWGUID, string>();

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
                    IntPtr tempOwner = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfoX64.GameObjectOwnerGUIDBase);
                    mOwnerGUID = WoWManager.WoWProcess.Memory.Read<WoWGUID>(tempOwner + WowBuildInfoX64.GameObjectOwnerGUIDOffset);
                }
                return mOwnerGUID;
            }
        }

        public override string Name
        {
            get
            {
                string temp;
                if (!Names.TryGetValue(GUID, out temp))
                {
                    try
                    {
                        IntPtr nameBase = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfoX64.GameObjectNameBase);
                        IntPtr nameAddress = WoWManager.WoWProcess.Memory.Read<IntPtr>(nameBase + WowBuildInfoX64.GameObjectNameOffset);
                        byte[] nameBytes = WoWManager.WoWProcess.Memory.ReadBytes(nameAddress, _maxNameLength);
                        while (!nameBytes.Contains((byte)0))
                        {
                            _maxNameLength += 1;
                            Log.Info("Max length for object names is increased to " + _maxNameLength);
                            nameBytes = WoWManager.WoWProcess.Memory.ReadBytes(nameAddress, _maxNameLength);
                        }
                        temp = Encoding.UTF8.GetString(nameBytes.TakeWhile(l => l != 0).ToArray());
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

        private uint mEntryID;

        public uint EntryID
        {
            get
            {
                if (mEntryID == 0)
                {
                    IntPtr descriptors = WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfoX64.GameObjectOwnerGUIDBase);
                    mEntryID = WoWManager.WoWProcess.Memory.Read<uint>(descriptors + WowBuildInfoX64.GameObjectEntryID);
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