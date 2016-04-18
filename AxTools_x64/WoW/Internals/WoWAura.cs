﻿using System.Runtime.InteropServices;
using AxTools.WoW.Helpers;

namespace AxTools.WoW.Internals
{
    [StructLayout(LayoutKind.Explicit, Size = 0x68)]
    public struct WoWAura
    {
        [FieldOffset(0x40)] internal WoWGUID OwnerGUID;
        [FieldOffset(0x50)] internal int SpellId;
        [FieldOffset(0x59)] internal byte Stack;
        [FieldOffset(0x60)] internal uint TimeLeftInMs;

        internal WoWAura(WoWGUID ownerGUID, int spellID, byte stack, uint timeLeft)
        {
            OwnerGUID = ownerGUID;
            SpellId = spellID;
            Stack = stack;
            TimeLeftInMs = timeLeft;
        }

        public string Name
        {
            get { return Wowhead.GetSpellInfo(SpellId).Name; }
        }
    }
}