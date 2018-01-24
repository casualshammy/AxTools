using System;
using System.Runtime.InteropServices;
using AxTools.Helpers;
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
    }
}
