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
                Pattern.FromTextstyle("Possible_NotLoadingScreen", "48 83 EC 38 80 3D ?? ?? ?? ?? 00 0F 84 F8 00 00 00 83 3D ?? ?? ?? ?? 00 48 89 5C 24 40 74 05 E8", new AddModifier(6), new LeaModifier(LeaType.Cmp)),
                Pattern.FromTextstyle("NameCache_in[rcx]<r9", "4C 89 09 48 8B 4A 10 48 8B 42 18 48 89 41 08 48 89 5A 10 48 89 5A 18 48 89 5A 20 41 FF 4B 20 48 8B 1C 24 48 83 C4 08 C3", new AddModifier(0), new LeaModifier(LeaType.SimpleAddress)),
                Pattern.FromTextstyle("MouseoverGUID", "41 B8 09 00 00 00 E8 ?? ?? ?? ?? 85 C0 75 ?? 48 8B 05 ?? ?? ?? ?? 48 83 45 20 09 4C 8D 05", new AddModifier(18), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ChatIsOpened", "48 83 EC 28 33 C9 E8 ?? ?? ?? ?? 33 D2 48 8B C8 E8 ?? ?? ?? ?? C7 05 ?? ?? ?? ?? 00 00 00 00 48 89 05 ?? ?? ?? ?? 48 83 C4 28 C3", new AddModifier(23), new LeaModifier(LeaType.RelativePlus8)),
                Pattern.FromTextstyle("KnownSpellsCount", "40 53 48 83 EC 20 8B 15 ?? ?? ?? ?? 8D 42 FF 48 63 D8 85 C0 78 63 4C 8B 05 ?? ?? ?? ?? 0F 1F 00 49 8B 0C D8", new AddModifier(8), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("KnownSpells", "40 53 48 83 EC 20 8B 15 ?? ?? ?? ?? 8D 42 FF 48 63 D8 85 C0 78 63 4C 8B 05 ?? ?? ?? ?? 0F 1F 00 49 8B 0C D8", new AddModifier(25), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ZoneText", "48 85 C0 74 16 48 63 48 18 85 C9 74 0E 48 8D 5C 08 18 EB 07 48 8B 1D ?? ?? ?? ?? 48 8B D3 48 8B CF", new AddModifier(23), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ChatBuffer", "45 33 FF 4C 8B F1 41 8B E8 4C 8B EA 41 83 CC FF 48 8D 0D ?? ?? ?? ?? 41 8B FF 48 89 74 24 50", new AddModifier(19), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("UIFrameBase", "48 8B 05 ?? ?? ?? ?? 48 8B 98 ?? ?? ?? ?? F6 C3 01", new AddModifier(3), new LeaModifier(LeaType.CmpMinusOne)),

            };
            string reportFilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\result.txt";
            File.Delete(reportFilePath);
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine("Calculating hash...");
            File.AppendAllLines(reportFilePath, new[] {"internal static readonly byte[] WoWHash =", "{", "\t" + CalcucateHash(wowProcess[0].MainModule.FileName), "};"});
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

            for (long i = 0; i < size - patternLength; i += CacheSize - patternLength)
            {
                byte[] cache = bm.ReadBytes(start + (int)i, (int) (CacheSize > size - i ? size - i : CacheSize));
                for (uint i2 = 0; i2 < cache.Length - patternLength; i2++)
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
            Pattern ret = new Pattern { Name = name };
            if (modifiers != null)
                ret.Modifiers = modifiers.ToList();
            string[] split = pattern.Split(' ');
            int index = 0;
            ret.Bytes = new byte[split.Length];
            ret.Mask = new bool[split.Length];
            foreach (string token in split)
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
            return addr + (int)Offset;
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
