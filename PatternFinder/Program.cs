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
            Process[] wowProcess = Process.GetProcessesByName("Wow");
            if (wowProcess.Length != 1)
            {
                Console.WriteLine("!!! No WoW process found!");
                Console.ReadLine();
                return;
            }
            Pattern[] patterns =
            {
                // x64
                Pattern.FromTextstyle("BlackMarketNumItems", "8B 15 ?? ?? ?? ?? 33 C0 F2 4C 0F 2C C0 85 D2 74 xx 4C 8B 0D xx xx xx xx 49 8B C9 44 39 01 74",  new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("BlackMarketItems", "8B 15 xx xx xx xx 33 C0 F2 4C 0F 2C C0 85 D2 74 xx 4C 8B 0D ?? ?? ?? ?? 49 8B C9 44 39 01 74",  new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("LastHardwareAction", "48 83 EC 28 2B 0D ?? ?? ?? ?? 8D 81 20 6C FB FF 85 C0 78 xx 8D 81 C0 88 E4 FF 85 C0 78 xx E8",  new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("TickCount", "0F 57 C0 8B C1 89 0D ?? ?? ?? ?? C7 05 xx xx xx xx 00 00 00 00 F3 48 0F 2A C0 F3 0F 59 05 xx xx xx xx",  new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("MouseoverGUID", "0F 10 07 0F 11 05 ?? ?? ?? ?? E8 xx xx xx xx 0F BE D0 4C 8D 05", new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ChatIsOpened", "83 3D ?? ?? ?? ?? 00 48 8B CB 7E xx 48 8B D0 EB xx 33 D2 E8 xx xx xx xx E8",  new LeaModifier(LeaType.Cmp)),
                Pattern.FromTextstyle("FocusedWidget", "E8 xx xx xx xx 8B C3 48 3B 05 ?? ?? ?? ?? 48 8B CF 0F 94 C3 8B D3 E8",  new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("ObjectManager", "48 83 EC 30 4C 8B 05 ?? ?? ?? ?? 45 33 F6 33 ED 45 33 FF 33 DB 33 FF 33 F6 49 8B 80 E8 01 00 00 A8 01",  new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("GlueState", "80 3D xx xx xx xx 00 75 19 80 3D ?? ?? ?? ?? 00 74 10 80 7B 20 00 74 0A 48 83 C4 20 5B", new LeaModifier(LeaType.Cmp)),
                Pattern.FromTextstyle("GameState", "48 83 EC 58 0F B6 05 ?? ?? ?? ?? A8 10 74 44 0F B6 C8 0F BA F1 04", new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("KnownSpellsCount", "8B C2 C3 44 8B 0D ?? ?? ?? ?? 33 D2 45 85 C9 74 23 4C 8B 15 xx xx xx xx", new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("KnownSpells", "8B C2 C3 44 8B 0D xx xx xx xx 33 D2 45 85 C9 74 23 4C 8B 15 ?? ?? ?? ??", new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("UIFrameBase", "48 8B 05 ?? ?? ?? ?? 48 8B 88 D0 0C 00 00 F6 C1 01", new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("PlayerZoneID", "40 55 48 83 EC 70 8B 2D ?? ?? ?? ??", new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("PlayerIsLooting", "8B D7 48 8D 0D ?? ?? ?? ?? E8 xx xx xx xx 80 38 00 74", new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("PlayerGUID", "48 8D 05 ?? ?? ?? ?? 41 B8 03 00 00 00 0F 1F 00", new LeaModifier(LeaType.CmpMinusOne)),
                Pattern.FromTextstyle("NotLoadingScreen", "48 83 EC 28 80 3D ?? ?? ?? ?? 00 0F 84 xx xx xx xx 83 3D xx xx xx xx 00 48", new LeaModifier(LeaType.Cmp)),
                Pattern.FromTextstyle("IsChatAFK", "8B 0C 81 C1 E9 07 F6 C1 01 74 xx 39 1D ?? ?? ?? ?? 75", new LeaModifier(LeaType.CmpMinusOne)),
                
                
                
                
                
                
                
                
                
                //Pattern.FromTextstyle("ChatBuffer", "E8 xx xx xx xx 44 8B 3D xx xx xx xx 41 83 CE FF 8B FB 4C 8D 2D ?? ?? ?? ?? F2 48 0F 2C C0", new LeaModifier(LeaType.CmpMinusOne)),
            };
            string reportFilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\result.txt";
            File.Delete(reportFilePath);
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine("Calculating hash...");
            File.AppendAllLines(reportFilePath, new[] {"internal static readonly byte[] WoWHash =", "{", "\t" + CalcucateHash(wowProcess[0].MainModule.FileName), "};"});
            var versionInfo = FileVersionInfo.GetVersionInfo(wowProcess[0].MainModule.FileName);
            File.AppendAllLines(reportFilePath, new[] { $"internal const int WoWRevision = {versionInfo.FilePrivatePart}" });
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
                File.AppendAllLines(reportFilePath, new[] { "", "", $"Base address: 0x{epr.ImageBase.ToInt64().ToString("X")}", $"Size of Wow.exe: 0x{epr.Process.MainModule.ModuleMemorySize.ToString("X")}" });
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
            HashSet<IntPtr> foundAddresses = new HashSet<IntPtr>();
            foreach (IntPtr intPtr in FindStart(bm))
            {
                IntPtr start = intPtr;
                foreach (IModifier modifier in Modifiers)
                {
                    start = modifier.Apply(bm, start);
                }
                IntPtr address = new IntPtr((long)start - (long)bm.Process.MainModule.BaseAddress);
                if (!foundAddresses.Contains(address))
                {
                    foundAddresses.Add(address);
                    yield return new Result { address = address, unmodifiedAddress = new IntPtr(intPtr.ToInt64() - bm.Process.MainModule.BaseAddress.ToInt64()) };
                }
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
            uint addModifierIndex = 0;
            bool addModifierInitialised = false;
            foreach (string token in split)
            {
                if (token.Length > 2)
                    throw new InvalidDataException("Invalid token: " + token);
                if (token.Contains("x"))
                {
                    ret.Mask[index++] = false;
                }
                else if (token.Contains("?"))
                {
                    ret.Mask[index++] = false;
                    if (!addModifierInitialised)
                    {
                        ret.Modifiers.Insert(0, new AddModifier(addModifierIndex)); // index matters
                        addModifierInitialised = true;
                    }
                }
                else
                {
                    byte data = byte.Parse(token, NumberStyles.HexNumber);
                    ret.Bytes[index] = data;
                    ret.Mask[index] = true;
                    index++;
                }
                addModifierIndex++;
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
