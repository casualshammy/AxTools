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
                Pattern.FromTextstyle("GlueState", "45 33 C0 0F 57 DB 48 8D 15 ?? ?? ?? ?? 41 8D 48 05 C6 05 ?? ?? ?? ?? 01 E8 ?? ?? ?? ?? 0F 57 C0 48 83 C4 28 E9", new AddModifier(19), new LeaModifier(LeaType.Cmp)),
                Pattern.FromTextstyle("GameState", "B8 01 00 00 00 D2 E0 84 05 ?? ?? ?? ?? 0F 95 C0 C3", new AddModifier(9), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("BlackMarketNumItems", "8B 15 ?? ?? ?? ?? 33 C0 F2 4C 0F 2C C0 85 D2 74 ?? 4C 8B 0D ?? ?? ?? ?? 49 8B C9 44 39 01 74", new AddModifier(2), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("BlackMarketItems", "8B 15 ?? ?? ?? ?? 33 C0 F2 4C 0F 2C C0 85 D2 74 ?? 4C 8B 0D ?? ?? ?? ?? 49 8B C9 44 39 01 74", new AddModifier(20), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("LastHardwareAction", "48 83 EC 28 2B 0D ?? ?? ?? ?? 8D 81 20 6C FB FF 85 C0 78 ?? 8D 81 C0 88 E4 FF 85 C0 78 ?? E8", new AddModifier(6), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("TickCount", "0F 57 C0 8B C1 89 0D ?? ?? ?? ?? C7 05 ?? ?? ?? ?? 00 00 00 00 F3 48 0F 2A C0 F3 0F 59 05 ?? ?? ?? ??", new AddModifier(7), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ObjectManager", "57 41 56 41 57 48 83 EC 30 4C 8B 05 ?? ?? ?? ?? 45 33 F6 33 ED 49 8B 88 A0 01 00 00", new AddModifier(12), new LeaModifier(LeaType.CmpMinusOne)),
                ////Pattern.FromTextstyle("PlayerName", "E8 ?? ?? ?? ?? 85 C0 75 6C 48 83 C7 21 E8 ?? ?? ?? ?? 41 B8 FF FF FF 7F 48 8B C8 48 8B D7 E8", new AddModifier(14), new LeaModifier(LeaType.E8), new AddModifier(5), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("NotLoadingScreen", "48 83 EC 38 80 3D ?? ?? ?? ?? 00 0F 84 ?? ?? ?? ?? 83 3D ?? ?? ?? ?? 00 48 89 5C 24 48 74 ?? E8", new AddModifier(6), new LeaModifier(LeaType.Cmp)),
                Pattern.FromTextstyle("MouseoverGUID", "45 33 C0 48 89 7C 24 70 41 8D 50 01 E8 ?? ?? ?? ?? 48 8D 15 ?? ?? ?? ?? 48 8B C8 48 8B F8 E8 ?? ?? ?? ?? 85 C0 75 ?? 0F 10 05 ?? ?? ?? ?? EB", new AddModifier(42), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ChatIsOpened", "83 3D ?? ?? ?? ?? 00 48 8B CB 7E ?? 48 8B D0 EB ?? 33 D2 E8 ?? ?? ?? ?? E8", new AddModifier(2), new LeaModifier(LeaType.Cmp)),
                Pattern.FromTextstyle("KnownSpellsCount", "E8 xx xx xx xx 8B D8 85 C0 0F 84 xx xx 00 00 44 8B 05 ?? ?? ?? ?? 41 8B C4 45 85 C0 74 xx 48 8B 15 xx xx xx xx", new AddModifier(18), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("KnownSpells", "E8 xx xx xx xx 8B D8 85 C0 0F 84 xx xx 00 00 44 8B 05 xx xx xx xx 41 8B C4 45 85 C0 74 xx 48 8B 15 ?? ?? ?? ??", new AddModifier(33), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("UIFrameBase", "48 8B 05 ?? ?? ?? ?? F3 0F 10 05 ?? ?? ?? ?? 0F 29 7C 24 20 48 8B D9 0F 28 CA F3 0F 59 05 ?? ?? ?? ?? F3 0F 10 3D", new AddModifier(3), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("FocusedWidget", "E8 xx xx xx xx 8B C3 48 3B 05 ?? ?? ?? ?? 48 8B CF 0F 94 C3 8B D3 E8", new AddModifier(10), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ChatBuffer", "E8 xx xx xx xx 44 8B 3D xx xx xx xx 41 83 CE FF 8B FB 4C 8D 2D ?? ?? ?? ?? F2 48 0F 2C C0", new AddModifier(21), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("PlayerZoneID", "0F B7 47 xx 66 85 C0 74 xx 0F B7 C8 3B 0D ?? ?? ?? ?? 74 xx 3B 0D xx xx xx xx 0F 85", new AddModifier(14), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("PlayerIsLooting", "41 56 48 81 EC 50 01 00 00 45 33 C0 33 D2 48 8B F9 E8 xx xx xx xx 48 8D 0D ?? ?? ?? ?? E8", new AddModifier(25), new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("PlayerGUID", "48 8D 05 ?? ?? ?? ?? 41 B8 03 00 00 00 0F 1F 00 0F 10 01", new AddModifier(3), new LeaModifier(LeaType.CmpMinusOne)),
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
                        var addresses = pattern.Find(epr).ToArray();
                        if (addresses.Length == 1)
                        {
                            consoleOutput = pattern.Name + ": 0x" + addresses[0].address.ToInt64().ToString("X") + "; address of instruction: 0x" + addresses[0].unmodifiedAddress.ToInt64().ToString("X");
                            lock (locker)
                            {
                                File.AppendAllLines(reportFilePath, new[] { "internal const int " + pattern.Name + " = 0x" + addresses[0].address.ToInt64().ToString("X") + ";" });
                            }
                        }
                        else
                        {
                            consoleOutput = "!!!  " + pattern.Name + ": 0x" + string.Join(", 0x", addresses.Select(l => l.address.ToInt64().ToString("X")));
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

        public struct Result
        {
            public IntPtr address;
            public IntPtr unmodifiedAddress;
        }

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

        public IEnumerable<Result> Find(MemoryManagement.MemoryManager bm)
        {
            foreach (IntPtr intPtr in FindStart(bm))
            {
                IntPtr start = intPtr;
                foreach (IModifier modifier in Modifiers)
                {
                    start = modifier.Apply(bm, start);
                }
                yield return new Result { address = new IntPtr((long)start - (long)bm.Process.MainModule.BaseAddress), unmodifiedAddress = new IntPtr(intPtr.ToInt64() - bm.Process.MainModule.BaseAddress.ToInt64()) };
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
                if (token.Contains("?") || token.Contains("x"))
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
