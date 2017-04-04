using System;
using System.Runtime.InteropServices;
using AxTools.Helpers;
using AxTools.WoW.Helpers;

namespace AxTools.WoW.Internals
{
    [StructLayout(LayoutKind.Explicit, Size = 0x90)]
    public struct WoWAura
    {
        [FieldOffset(0x50)] public WoWGUID OwnerGUID;
        [FieldOffset(0x60)] public int SpellId;
        [FieldOffset(0x69)] public byte Stack;
        [FieldOffset(0x70)] public uint TimeLeftInMs;

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
                try
                {
                    return Wowhead.GetSpellInfo(SpellId).Name;
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("[Wowhead] Can't get aura name, id: {0}, error: {1}", SpellId, ex.Message));
                    return "";
                }
            }
        }
    }
}
