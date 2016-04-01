using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PatternFinder
{
    internal class Program
    {
        private static void Main()
        {
            Process[] wowProcess = Process.GetProcessesByName("Wow-64");
            if (wowProcess.Length != 1)
            {
                Console.WriteLine("!!! No WoW process found!");
                Console.ReadLine();
                return;
            }
            Pattern[] patterns =
            {
                // x64
                Pattern.FromTextstyle("GlueState", "B9 E2 03 00 00 E8 ?? ?? ?? ?? 80 3D ?? ?? ?? ?? 00 75 ?? 80 3D ?? ?? ?? ?? 00 74", new AddModifier(21), new LeaModifier(LeaType.Cmp)),
                Pattern.FromTextstyle("GameState", "48 83 EC 38 80 3D ?? ?? ?? ?? 00 74 ?? 48 8B 41 08 48 C1 E8 3A 83 E0 3F 84 C0 75", new AddModifier(6), new LeaModifier(LeaType.Cmp)),
                Pattern.FromTextstyle("FocusedWidget", "48 39 3D ?? ?? ?? ?? 75 ?? 33 D2 48 8B CB E8 ?? ?? ?? ?? 48 89 BB 90 0B 00 00", new AddModifier(3), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("BlackMarketNumItems", "74 3B BA 01 00 00 00 48 8B CB E8 ?? ?? ?? ?? F2 48 0F 2C C0 FF C8 3B 05 ?? ?? ?? ?? 73 1F 8B D0 48 69 D2 A8 00 00 00 48 03 15 ?? ?? ?? ?? 74 0D", new AddModifier(24), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("BlackMarketItems", "74 3B BA 01 00 00 00 48 8B CB E8 ?? ?? ?? ?? F2 48 0F 2C C0 FF C8 3B 05 ?? ?? ?? ?? 73 1F 8B D0 48 69 D2 A8 00 00 00 48 03 15 ?? ?? ?? ?? 74 0D", new AddModifier(42), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("LastHardwareAction", "48 83 EC 28 2B 0D ?? ?? ?? ?? 8D 81 20 6C FB FF 85 C0 78 ?? 8D 81 C0 88 E4 FF 85 C0 78 ?? E8", new AddModifier(6), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("TickCount", "66 0F EF C0 8B C1 F3 48 0F 2A C0 89 0D ?? ?? ?? ?? C7 05 ?? ?? ?? ?? 00 00 00 00", new AddModifier(13), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("ObjectManager", "48 83 EC 28 44 0F B6 C1 48 8B 0D ?? ?? ?? ?? 48 85 C9 74 15 BA 00 02 00 00 E8 ?? ?? ?? ?? 84 C0 74 07 B0 01 48 83 C4 28 C3 32 C0 48 83 C4 28 C3", new AddModifier(11), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("PlayerIsLooting", "48 83 EC 38 48 83 3D ?? ?? ?? ?? 00 41 B8 01 00 00 00 48 8D 15 ?? ?? ?? ?? 74 15 44 88 05 ?? ?? ?? ?? 44 88 05 ?? ?? ?? ?? B9 91 00 00 00 EB 05", new AddModifier(30), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("PlayerName", "E8 ?? ?? ?? ?? 85 C0 75 6C 48 83 C7 21 E8 ?? ?? ?? ?? 41 B8 FF FF FF 7F 48 8B C8 48 8B D7 E8", new AddModifier(14), new LeaModifier(LeaType.E8), new AddModifier(5), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("PlayerZoneID", "41 0F B7 06 A8 0C 74 6D 40 84 C5 74 68 8B D5 EB 66 41 0F B7 56 18 EB 06 8B 15 ?? ?? ?? ?? 48 8D 0D", new AddModifier(26), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("PlayerPtr", "48 89 AC 24 E0 00 00 00 C7 05 ?? ?? ?? ?? 15 00 00 00 48 89 BC 24 E8 00 00 00 E8 ?? ?? ?? ?? 48 8B 3D ?? ?? ?? ?? 48 8B E8", new AddModifier(34), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("FrameScript_ExecuteBuffer", "E8 ?? ?? ?? ?? 48 85 C0 74 13 80 38 00 74 0E 45 33 C0 48 8B D0 48 8B C8 E8 ?? ?? ?? ?? 33 C0 48 83 C4 20 5B C3", new AddModifier(25), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("CGWorldFrame_Render", "F3 0F 10 84 08 50 1A 00 00 48 8B CB F3 0F 11 45 A0 E8 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B CB E8 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 0D", new AddModifier(31), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("Possible_NotLoadingScreen", "48 83 EC 38 80 3D ?? ?? ?? ?? 00 0F 84 F8 00 00 00 83 3D ?? ?? ?? ?? 00 48 89 5C 24 40 74 05 E8", new AddModifier(6), new LeaModifier(LeaType.Cmp)),
                Pattern.FromTextstyle("NameCache_in[rcx]<r9", "4C 89 09 48 8B 4A 10 48 8B 42 18 48 89 41 08 48 89 5A 10 48 89 5A 18 48 89 5A 20 41 FF 4B 20 48 8B 1C 24 48 83 C4 08 C3", new AddModifier(0), new LeaModifier(LeaType.SimpleAddress)),
                Pattern.FromTextstyle("MouseoverGUID", "41 B8 09 00 00 00 E8 ?? ?? ?? ?? 85 C0 75 ?? 48 8B 05 ?? ?? ?? ?? 48 83 45 20 09 4C 8D 05", new AddModifier(18), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ChatIsOpened", "48 83 EC 28 33 C9 E8 ?? ?? ?? ?? 33 D2 48 8B C8 E8 ?? ?? ?? ?? C7 05 ?? ?? ?? ?? 00 00 00 00 48 89 05 ?? ?? ?? ?? 48 83 C4 28 C3", new AddModifier(23), new LeaModifier(LeaType.RelativePlus8)),
                Pattern.FromTextstyle("KnownSpellsCount", "40 53 48 83 EC 20 8B 15 ?? ?? ?? ?? 8D 42 FF 48 63 D8 85 C0 78 63 4C 8B 05 ?? ?? ?? ?? 0F 1F 00 49 8B 0C D8", new AddModifier(8), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("KnownSpells", "40 53 48 83 EC 20 8B 15 ?? ?? ?? ?? 8D 42 FF 48 63 D8 85 C0 78 63 4C 8B 05 ?? ?? ?? ?? 0F 1F 00 49 8B 0C D8", new AddModifier(25), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ZoneText", "48 85 C0 74 16 48 63 48 18 85 C9 74 0E 48 8D 5C 08 18 EB 07 48 8B 1D ?? ?? ?? ?? 48 8B D3 48 8B CF", new AddModifier(23), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ChatBuffer", "45 33 FF 4C 8B F1 41 8B E8 4C 8B EA 41 83 CC FF 48 8D 0D ?? ?? ?? ?? 41 8B FF 48 89 74 24 50", new AddModifier(19), new LeaModifier(LeaType.CmpMinusOne)),





                //Pattern.FromTextstyle("CVarAutoInteract", "75 1D 48 8B 05 ?? ?? ?? ?? 83 78 5C 00 74 10 F6 81 DE 25 00 00 20 75 07 B0 01 48 83 C4 18 C3", new AddModifier(5), new LeaModifier(LeaType.CmpMinusOne)),
                //Pattern.FromTextstyle("CTMState", "48 83 EC 18 44 8B 05 ?? ?? ?? ?? 41 83 F8 0D 75 07 32 C0 48 83 C4 18 C3 48 8B 49 08 8B 01 89 04 24", new AddModifier(7), new LeaModifier(LeaType.CmpMinusOne)),
                //Pattern.FromTextstyle("CTMLocation", "83 F8 0D 0F 84 D0 00 00 00 83 F8 04 0F 85 A4 00 00 00 F3 0F 10 2D ?? ?? ?? ?? F3 0F 10 25 ?? ?? ?? ?? F3 0F 10 1D ?? ?? ?? ?? 0F 29 74 24 30", new AddModifier(30), new LeaModifier(LeaType.CmpMinusOne)),
                //Pattern.FromTextstyle("FrameScript_GetLocalizedText", "BA 20 00 00 00 89 44 24 20 E8 ?? ?? ?? ?? 48 8D 54 24 30 41 83 C8 FF 48 8B CF E8 ?? ?? ?? ?? 48 8B 5C 24 60 48 83 C4 50", new AddModifier(27), new LeaModifier(LeaType.E8)),
                //Pattern.FromTextstyle("CGGameUI_Target", "41 83 E0 3F 45 84 C0 48 0F 44 C3 48 85 C0 0F 95 C3 E8 ?? ?? ?? ?? 8B D3 48 8B CF E8", new AddModifier(18), new LeaModifier(LeaType.E8)),
                //Pattern.FromTextstyle("CGGameUI_Interact", "83 7B 10 01 48 8B CB 75 10 E8 ?? ?? ?? ?? B8 01 00 00 00 48 83 C4 20 5B C3 E8 ?? ?? ?? ?? B8 01 00 00 00 48 83 C4 20 5B C3", new AddModifier(26), new LeaModifier(LeaType.E8)),
                //Pattern.FromTextstyle("CGUnit_C_InitializeTrackingState", "48 8D 55 E7 48 8B CF E8 ?? ?? ?? ?? 48 8B BC 24 B0 00 00 00 B8 01 00 00 00 48 8B 9C 24 B8 00 00 00", new AddModifier(8), new LeaModifier(LeaType.E8)),



                // x86
                //Pattern.FromTextstyle("GlueState", "83 3d ?? ?? ?? ?? ?? 75 ?? e8 ?? ?? ?? ?? 8b 10 8b c8 ff 62 5c c3", new AddModifier(2), new LeaModifier()),
                //Pattern.FromTextstyle("GameState", "80 3d ?? ?? ?? ?? ?? 74 ?? 50 b9 ?? ?? ?? ?? e8 ?? ?? ?? ?? 85 c0 74 ?? 8b 40 08 83 f8 02 74 ?? 83 f8 01 75 ?? b0 01 c3 32 c0 c3", new AddModifier(2), new LeaModifier()),
                //Pattern.FromTextstyle("FocusedWidget", "3b 05 ?? ?? ?? ?? 0f 94 c1 51 ff 75 08 e8 ?? ?? ?? ?? 33 c0 83 c4 10 40 5d c3", new AddModifier(2), new LeaModifier()),
                //Pattern.FromTextstyle("BlackMarketNumItems", "55 8b ec 8b 0d ?? ?? ?? ?? 33 c0 56 85 c9 74 15 8b 15 ?? ?? ?? ?? 8b 32 3b 75 08 74 0b 40 83 c2 70", new AddModifier(5), new LeaModifier()),
                //Pattern.FromTextstyle("BlackMarketItems", "55 8b ec 8b 0d ?? ?? ?? ?? 33 c0 56 85 c9 74 15 8b 15 ?? ?? ?? ?? 8b 32 3b 75 08 74 0b 40 83 c2 70", new AddModifier(18), new LeaModifier()),
                //Pattern.FromTextstyle("LastHardwareAction", "55 8b ec a1 ?? ?? ?? ?? 6a 00 50 ff 75 08 e8 ?? ?? ?? ?? 8b c8 e8 ?? ?? ?? ?? 33 c0 5d c3", new AddModifier(4), new LeaModifier()),
                //Pattern.FromTextstyle("ObjectManager", "a1 ?? ?? ?? ?? c3 55 8b ec a1 ?? ?? ?? ?? 85 c0 74 17 ff 75 08 68 18 01 00 00", new AddModifier(1), new LeaModifier()),
                //Pattern.FromTextstyle("PlayerIsLooting", "e8 ?? ?? ?? ?? 03 05 ?? ?? ?? ?? c6 05 ?? ?? ?? ?? 00 a3 ?? ?? ?? ?? c3", new AddModifier(13), new LeaModifier()),
                //Pattern.FromTextstyle("PlayerName", "0f be 05 ?? ?? ?? ?? f7 d8 1b c0 25 ?? ?? ?? ?? c3 a0 ?? ?? ?? ?? c3", new AddModifier(3), new LeaModifier()),
                //Pattern.FromTextstyle("PlayerZoneID", "8b 15 ?? ?? ?? ?? 3b 15 ?? ?? ?? ?? 75 06 a0 ?? ?? ?? ?? c3", new AddModifier(2), new LeaModifier()),
                //Pattern.FromTextstyle("PlayerPtr", "55 8b ec a1 ?? ?? ?? ?? 83 ec 10 85 c0 75 26 8d 45 f0 50 e8 ?? ?? ?? ?? 68 ce 00 00 00 68", new AddModifier(4), new LeaModifier()),
                //Pattern.FromTextstyle("ClntObjMgrGetActivePlayerObj", "55 8b ec 83 ec 10 56 e8 ?? ?? ?? ?? 33 f6 3b c6 0F 84 07 01 00 00 8B 88 24 01 00 00", new AddModifier(8), new LeaModifier(LeaType.E8)),
                //Pattern.FromTextstyle("LuaDoStringAddress", "74 0c 6a 00 50 50 e8 ?? ?? ?? ?? 83 c4 0c 33 c0 5d c3", new AddModifier(7), new LeaModifier(LeaType.E8)),
                //Pattern.FromTextstyle("LuaGetLocalizedTextAddress", "83 c4 14 6a ff 8d 45 e0 50 8b ce e8 ?? ?? ?? ?? 5e c9 c2 04 00", new AddModifier(12), new LeaModifier(LeaType.E8)),
                //Pattern.FromTextstyle("TargetUnit", "C1 E8 1A 83 E0 3F 0F BE F0 F7 DE 1B F6 81 E6 ?? ?? ?? ?? F7 DE 1B F6 68 ?? ?? ?? ?? F7 DE E8 ?? ?? ?? ?? 56 FF 75 08 E8", new AddModifier(31), new LeaModifier(LeaType.E8)),
                //Pattern.FromTextstyle("Interact", "8D 45 F0 a5 50 75 0A e8 ?? ?? ?? ?? e9 ?? ?? ?? ?? e8 ?? ?? ?? ?? e9 ?? ?? ?? ?? 8B 83 30 02 00 00", new AddModifier(18), new LeaModifier(LeaType.E8)),
                //Pattern.FromTextstyle("HandleTerrainClick", "39 73 1C 0F 85 92 00 00 00 53 e8 ?? ?? ?? ?? 59 84 C0 74 07", new AddModifier(11), new LeaModifier(LeaType.E8)),
                //Pattern.FromTextstyle("ClickToMove", "F3 0F 11 04 24 FF 75 08 8B CE 68 ?? ?? ?? ?? 6A 04 e8 ?? ?? ?? ?? 5E 5d C2 04 00", new AddModifier(18), new LeaModifier(LeaType.E8)),
                //Pattern.FromTextstyle("CGWorldFrame::Render", "55 8b ec a1 ?? ?? ?? ?? 8b 80 d8 00 00 00 56 57 33 ff 47 a8 01 75 ?? 85 c0 75 ?? 33 c9", new AddModifier(0), new LeaModifier(LeaType.SimpleAddress))
            };
            string reportFilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\result.txt";
            File.Delete(reportFilePath);
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine("Calculating hash...");
            File.AppendAllLines(reportFilePath, new[] {"internal static readonly byte[] WoWHash =", "{", CalcucateHash(wowProcess[0].MainModule.FileName), "};"});
            Console.WriteLine("Hash is calculated");
            Console.WriteLine("------------------");
            Console.WriteLine("------------------");
            using (MemoryManagement.MemoryManager epr = new MemoryManagement.MemoryManager(wowProcess[0]))
            {
                int counter = 0;
                object locker = new object();
                Parallel.ForEach(patterns, pattern =>
                {
                    string consoleOutput;
                    try
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        IntPtr[] addresses = pattern.Find(epr).ToArray();
                        if (addresses.Length == 1)
                        {
                            consoleOutput = pattern.Name + ": 0x" + addresses[0].ToInt64().ToString("X");
                            lock (locker)
                            {
                                File.AppendAllLines(reportFilePath, new[] { "internal const int " + pattern.Name + " = 0x" + addresses[0].ToInt64().ToString("X") + ";" });
                            }
                        }
                        else
                        {
                            consoleOutput = "!!!  " + pattern.Name + ": 0x" + string.Join(", 0x", addresses.Select(l => l.ToInt64().ToString("X")));
                        }
                    }
                    catch (Exception ex)
                    {
                        consoleOutput = "!!!  " + ex.Message;
                    }
                    counter++;
                    lock (locker)
                    {
                        Console.WriteLine(consoleOutput);
                        int cursorTop = Console.CursorTop;
                        int cursorLeft = Console.CursorLeft;
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.WriteLine("---- " + counter * 100 / patterns.Length + "% ----");
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                    }
                });
                Console.WriteLine();
            }
            Console.WriteLine("Stopwatch: " + stopwatch.ElapsedMilliseconds + "ms");
            Console.ReadLine();
        }

        private static string CalcucateHash(string path)
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

    public class Pattern
    {
        public string Name { get; private set; }
        public byte[] Bytes { get; private set; }
        public bool[] Mask { get; private set; }
        const long CacheSize = 0x500;
        public List<IModifier> Modifiers = new List<IModifier>();

        private bool DataCompare(byte[] data, uint dataOffset)
        {
            return !Mask.Where((t, i) => t && Bytes[i] != data[dataOffset + i]).Any();
        }

        private IEnumerable<IntPtr> FindStart(MemoryManagement.MemoryManager bm)
        {
            ProcessModule mainModule = bm.Process.MainModule;
            IntPtr start = mainModule.BaseAddress;
            long size = mainModule.ModuleMemorySize;
            long patternLength = Bytes.LongLength;

            List<IntPtr> addresses = new List<IntPtr>();

            for (long i = 0; i < size - patternLength; i += (CacheSize - patternLength))
            {
                byte[] cache = bm.ReadBytes(start + (int)i, (int) (CacheSize > size - i ? size - i : CacheSize));
                for (uint i2 = 0; i2 < (cache.Length - patternLength); i2++)
                {
                    if (DataCompare(cache, i2))
                    {
                        addresses.Add(start + (int)(i + i2));
                    } 
                }
            }
            if (addresses.Count > 0)
            {
                return addresses.AsEnumerable();
            }
            throw new InvalidDataException(string.Format("Pattern {0} not found", Name));
        }

        public IEnumerable<IntPtr> Find(MemoryManagement.MemoryManager bm)
        {
            foreach (IntPtr intPtr in FindStart(bm))
            {
                IntPtr start = intPtr;
                foreach (IModifier modifier in Modifiers)
                {
                    start = modifier.Apply(bm, start);
                }
                yield return new IntPtr((long) start - (long) bm.Process.MainModule.BaseAddress);
            }
        }

        public static Pattern FromTextstyle(string name, string pattern, params IModifier[] modifiers)
        {
            var ret = new Pattern { Name = name };
            if (modifiers != null)
                ret.Modifiers = modifiers.ToList();
            var split = pattern.Split(' ');
            int index = 0;
            ret.Bytes = new byte[split.Length];
            ret.Mask = new bool[split.Length];
            foreach (var token in split)
            {
                if (token.Length > 2)
                    throw new InvalidDataException("Invalid token: " + token);
                if (token.Contains("?"))
                    ret.Mask[index++] = false;
                else
                {
                    byte data = byte.Parse(token, NumberStyles.HexNumber);
                    ret.Bytes[index] = data;
                    ret.Mask[index] = true;
                    index++;
                }
            }
            return ret;
        }
    }

    public interface IModifier
    {
        IntPtr Apply(MemoryManagement.MemoryManager bm, IntPtr address);
    }

    public class AddModifier : IModifier
    {
        public uint Offset { get; private set; }

        public AddModifier(uint val)
        {
            Offset = val;
        }

        public IntPtr Apply(MemoryManagement.MemoryManager bm, IntPtr addr)
        {
            return (addr + (int)Offset);
        }
    }

    public enum LeaType
    {
        Byte,
        Word,
        Dword,
        E8,
        SimpleAddress,
        Cmp,
        CmpMinusOne,
        RelativePlus8,
    }

    public class LeaModifier : IModifier
    {
        public LeaType Type { get; private set; }

        public LeaModifier(LeaType type)
        {
            Type = type;
        }

        public IntPtr Apply(MemoryManagement.MemoryManager bm, IntPtr address)
        {
            switch (Type)
            {
                case LeaType.Byte:
                    return (IntPtr) bm.Read<byte>(address);
                case LeaType.Word:
                    return (IntPtr) bm.Read<ushort>(address);
                case LeaType.Dword:
                    return (IntPtr) bm.Read<uint>(address);
                case LeaType.E8:
                    return address + 4 + bm.Read<int>(address); // 4 = <call instruction size> - <E8>
                case LeaType.SimpleAddress:
                    return address;
                case LeaType.Cmp:
                    return address + 5 + bm.Read<int>(address);
                case LeaType.CmpMinusOne:
                    return address + 4 + bm.Read<int>(address);
                case LeaType.RelativePlus8:
                    return address + 8 + bm.Read<int>(address);

            }
            throw new InvalidDataException("Unknown LeaType");
        }
    }

}
