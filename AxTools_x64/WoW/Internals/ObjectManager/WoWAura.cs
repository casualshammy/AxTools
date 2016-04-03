using System.Runtime.InteropServices;

namespace AxTools.WoW.Management.ObjectManager
{
    [StructLayout(LayoutKind.Explicit, Size = 0x68)]
    public struct WoWAura
    {
        [FieldOffset(0x40)] internal WoWGUID OwnerGUID;
        [FieldOffset(0x50)] internal uint SpellId;
        [FieldOffset(0x59)] internal byte Stack;
        [FieldOffset(0x60)] internal int TimeLeftInMs;

        internal WoWAura(WoWGUID ownerGUID, uint spellID, byte stack, int timeLeft)
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
