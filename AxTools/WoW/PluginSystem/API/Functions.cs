﻿using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;

namespace AxTools.WoW.PluginSystem.API
{
    public static class Functions
    {
        public static void Interact(this WowObject wowObject)
        {
            WoWDXInject.Interact(wowObject.GUID);
        }

        public static void Interact(this WowNpc wowNpc)
        {
            WoWDXInject.Interact(wowNpc.GUID);
        }

        public static void Interact(ulong guid)
        {
            WoWDXInject.Interact(guid);
        }

        public static void MoveTo(WowPoint point)
        {
            WoWDXInject.MoveTo(point);
        }

        public static void TerrainClick(TerrainClickStruct clickStruct)
        {
            WoWDXInject.TerrainClick(clickStruct);
        }
    }
}