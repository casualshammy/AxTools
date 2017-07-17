using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WoWPatternUpdater
{
    public static class WoWPatternUpdater
    {

        private static Pattern[] patterns =
        {
            // x64
            Pattern.FromTextstyle("GlueState", "45 33 C0 0F 57 DB 48 8D 15 ?? ?? ?? ?? 41 8D 48 05 C6 05 ?? ?? ?? ?? 01 E8 ?? ?? ?? ?? 0F 57 C0 48 83 C4 28 E9", new AddModifier(19), new LeaModifier(LeaType.Cmp)),
            Pattern.FromTextstyle("GameState", "48 83 EC 28 80 3D ?? ?? ?? ?? 00 74 ?? E8 ?? ?? ?? ?? 84 C0 74 ?? 0F B6 0D ?? ?? ?? ?? 0F B6 C1 80 E1 FE", new AddModifier(6), new LeaModifier(LeaType.Cmp)),
            Pattern.FromTextstyle("BlackMarketNumItems", "8B 15 ?? ?? ?? ?? 33 C0 F2 4C 0F 2C C0 85 D2 74 ?? 4C 8B 0D ?? ?? ?? ?? 49 8B C9 44 39 01 74", new AddModifier(2), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("BlackMarketItems", "8B 15 ?? ?? ?? ?? 33 C0 F2 4C 0F 2C C0 85 D2 74 ?? 4C 8B 0D ?? ?? ?? ?? 49 8B C9 44 39 01 74", new AddModifier(20), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("LastHardwareAction", "48 83 EC 28 2B 0D ?? ?? ?? ?? 8D 81 20 6C FB FF 85 C0 78 ?? 8D 81 C0 88 E4 FF 85 C0 78 ?? E8", new AddModifier(6), new LeaModifier(LeaType.E8)),
            Pattern.FromTextstyle("TickCount", "0F 57 C0 8B C1 89 0D ?? ?? ?? ?? C7 05 ?? ?? ?? ?? 00 00 00 00 F3 48 0F 2A C0 F3 0F 59 05 ?? ?? ?? ??", new AddModifier(7), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("ObjectManager", "48 83 EC 28 44 0F B6 C1 48 8B 0D ?? ?? ?? ?? 48 85 C9 74 15 BA 00 02 00 00 E8 ?? ?? ?? ?? 84 C0 74 07 B0 01 48 83 C4 28 C3 32 C0 48 83 C4 28 C3", new AddModifier(11), new LeaModifier(LeaType.E8)),
            Pattern.FromTextstyle("PlayerName", "E8 ?? ?? ?? ?? 85 C0 75 6C 48 83 C7 21 E8 ?? ?? ?? ?? 41 B8 FF FF FF 7F 48 8B C8 48 8B D7 E8", new AddModifier(14), new LeaModifier(LeaType.E8), new AddModifier(5), new LeaModifier(LeaType.E8)),
            Pattern.FromTextstyle("NotLoadingScreen", "48 83 EC 38 80 3D ?? ?? ?? ?? 00 0F 84 ?? ?? ?? ?? 83 3D ?? ?? ?? ?? 00 48 89 5C 24 48 74 ?? E8", new AddModifier(6), new LeaModifier(LeaType.Cmp)),
            Pattern.FromTextstyle("MouseoverGUID", "45 33 C0 48 89 7C 24 70 41 8D 50 01 E8 ?? ?? ?? ?? 48 8D 15 ?? ?? ?? ?? 48 8B C8 48 8B F8 E8 ?? ?? ?? ?? 85 C0 75 ?? 0F 10 05 ?? ?? ?? ?? EB", new AddModifier(42), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("ChatIsOpened", "83 3D ?? ?? ?? ?? 00 48 8B CB 7E ?? 48 8B D0 EB ?? 33 D2 E8 ?? ?? ?? ?? E8", new AddModifier(2), new LeaModifier(LeaType.Cmp)),
            Pattern.FromTextstyle("KnownSpellsCount", "8B D8 85 C0 0F 84 ?? ?? ?? ?? 44 8B 05 ?? ?? ?? ?? 41 8B D4 45 85 C0 74 ?? 48 8B 05 ?? ?? ?? ?? 48 8B 08", new AddModifier(13), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("KnownSpells", "8B D8 85 C0 0F 84 ?? ?? ?? ?? 44 8B 05 ?? ?? ?? ?? 41 8B D4 45 85 C0 74 ?? 48 8B 05 ?? ?? ?? ?? 48 8B 08", new AddModifier(28), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("UIFrameBase", "48 8B 05 ?? ?? ?? ?? F3 0F 10 05 ?? ?? ?? ?? 0F 29 7C 24 20 48 8B D9 0F 28 CA F3 0F 59 05 ?? ?? ?? ?? F3 0F 10 3D", new AddModifier(3), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("PlayerPtr", "C7 83 E8 01 00 00 FF FF FF FF 48 89 8B xx xx xx xx 48 89 8B xx xx xx xx 48 89 0D ?? ?? ?? ?? 48 8B C3 48 83 C4 20 5B C3", new AddModifier(27), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("FocusedWidget", "E8 xx xx xx xx 8B C3 48 3B 05 ?? ?? ?? ?? 48 8B CF 0F 94 C3 8B D3 E8", new AddModifier(10), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("ChatBuffer", "E8 xx xx xx xx 44 8B 3D xx xx xx xx 41 83 CE FF 8B FB 4C 8D 2D ?? ?? ?? ?? F2 48 0F 2C C0", new AddModifier(21), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("PlayerZoneID", "0F B7 47 xx 66 85 C0 74 xx 0F B7 C8 3B 0D ?? ?? ?? ?? 74 xx 3B 0D xx xx xx xx 0F 85", new AddModifier(14), new LeaModifier(LeaType.CmpMinusOne)),
            Pattern.FromTextstyle("PlayerIsLooting", "B9 95 00 00 00 E8 xx xx xx xx 48 89 3D xx xx xx xx 40 88 3D ?? ?? ?? ?? B0 01", new AddModifier(20), new LeaModifier(LeaType.CmpMinusOne)),
        };

        /// <summary>
        /// Throws exception if pettern is invlid or not found
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static Dictionary<string, long> GetAddresses(int processId)
        {
            Process wowProcess = Process.GetProcessById(processId);
            if (wowProcess != null)
            {
                using (MemoryManager epr = new MemoryManager(wowProcess))
                {
                    object locker = new object();
                    Dictionary<string, long> dic = new Dictionary<string, long>();
                    Parallel.ForEach(patterns, pattern =>
                    {
                        try
                        {
                            IntPtr[] addresses = pattern.Find(epr).ToArray();
                            if (addresses.Length == 1)
                            {
                                lock (locker)
                                {
                                    dic.Add(pattern.Name, addresses[0].ToInt64());
                                }
                            }
                            else
                            {
                                throw new Exception("One or more patterns are invalid");
                            }
                        }
                        catch
                        {
                            throw new Exception("One or more patterns are not found");
                        }
                    });
                    return dic;
                }
            }
            else
            {
                return null;
            }
        }


        public static string CalcucateHash(string path)
        {
            using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
            {
                using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = provider.ComputeHash(fileStream);
                    return "0x" + BitConverter.ToString(hash).Replace("-", ", 0x");
                }
            }
        }
    }
}
