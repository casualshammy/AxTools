﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMemory;
using AxTools.WoW.Helpers;

namespace AxTools.WoW.Internals
{
    [StructLayout(LayoutKind.Explicit, Size = 0xA0)]
    public struct WoWAura
    {
        [FieldOffset(0x50)] public WoWGUID OwnerGUID;
        [FieldOffset(0x70)] public int SpellId;
        [FieldOffset(0x79)] public byte Stack;
        [FieldOffset(0x80)] public uint TimeLeftInMs;
        
        internal WoWAura(WoWGUID ownerGUID, int spellID, byte stack, uint timeLeft)
        {
            OwnerGUID = ownerGUID;
            SpellId = spellID;
            Stack = stack;
            TimeLeftInMs = timeLeft;
        }

        public string Name
        {
            get
            {
                return Wowhead.GetSpellInfo(SpellId).Name;
            }
        }

        private static unsafe int auraStructSize = sizeof(WoWAura);

        internal static List<WoWAura> GetAurasForMemoryAddress(MemoryManager memory, IntPtr address)
        {
            List<WoWAura> auras = new List<WoWAura>();
            IntPtr table = address + WowBuildInfoX64.AuraTable1;
            int auraCount = memory.Read<int>(address + WowBuildInfoX64.AuraCount1);
            if (auraCount == -1)
            {
                table = memory.Read<IntPtr>(address + WowBuildInfoX64.AuraTable2);
                auraCount = memory.Read<int>(address + WowBuildInfoX64.AuraCount2);
            }
            for (int i = 0; i < auraCount; i++)
            {
                WoWAura rawAura = memory.Read<WoWAura>(table + i * auraStructSize);
                if (rawAura.SpellId != 0)
                {
                    WoWAura aura = new WoWAura(rawAura.OwnerGUID, rawAura.SpellId, rawAura.Stack, (uint)(rawAura.TimeLeftInMs - Environment.TickCount));
                    auras.Add(aura);
                }
            }
            return auras;
        }

    }
}
