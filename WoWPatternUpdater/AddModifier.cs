using System;

namespace WoWPatternUpdater
{
    public class AddModifier : IModifier
    {
        public uint Offset { get; private set; }

        public AddModifier(uint val)
        {
            Offset = val;
        }

        public IntPtr Apply(MemoryManager bm, IntPtr addr)
        {
            return addr + (int)Offset;
        }
    }
}
