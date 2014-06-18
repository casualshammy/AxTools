using System.Runtime.InteropServices;

namespace AxTools.Classes.WoW.Management.ObjectManager
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WowPlayerInfo
    {
        internal readonly ulong TargetGUID; // 0x58
        private readonly uint unk1, unk2, unk3, unk4, unk5, unk6;
        private readonly byte unkb1;
        internal readonly byte Class; // 0x72
        private readonly short unks1;
        private readonly uint unk8, unk9;
        internal readonly uint Health; // 0x84
        private readonly uint unk10, unk11, unk12, unk13, unk14;
        internal readonly uint HealthMax; // 0x9C
        private readonly uint unk21, unk22, unk23, unk24, unk25, unk26, unk27, unk28, unk29, unk30, unk31, unk32, unk33, unk34, unk35;
        internal readonly uint Level; // 0xDC
        private readonly uint unk41;
        internal readonly uint FactionTemplate; // 0xE4
    }
}
