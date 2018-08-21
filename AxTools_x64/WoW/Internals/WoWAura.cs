using AxTools.WoW.Helpers;
using FMemory;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AxTools.WoW.Internals
{
    [StructLayout(LayoutKind.Explicit, Size = 0xA8)]
    public struct WoWAura
    {
        [FieldOffset(0x68)] public WoWGUID OwnerGUID;
        [FieldOffset(0x88)] public int SpellId;
        [FieldOffset(0x91)] public byte Stack;
        [FieldOffset(0x98)] public uint TimeLeftInMs;

        internal WoWAura(WoWGUID ownerGUID, int spellID, byte stack, uint timeLeft)
        {
            OwnerGUID = ownerGUID;
            SpellId = spellID;
            Stack = stack;
            TimeLeftInMs = timeLeft;
        }

        public string Name => Wowhead.GetSpellInfo(SpellId).Name;

        private static readonly unsafe int auraStructSize = sizeof(WoWAura);

        internal static List<WoWAura> GetAurasForMemoryAddress(MemoryManager memory, IntPtr address)
        {
            List<WoWAura> auras = new List<WoWAura>();
            var table = address + WowBuildInfoX64.AuraTable1;
            var auraCount = memory.Read<int>(address + WowBuildInfoX64.AuraCount1);
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