using System;

namespace WoWPatternUpdater
{
    public interface IModifier
    {
        IntPtr Apply(MemoryManager bm, IntPtr address);
    }
}
