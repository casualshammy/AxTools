using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using GreyMagic;

namespace PatternFinder
{
    class Program
    {
        static void Main()
        {
            Process[] wowProcess = Process.GetProcessesByName("Wow");
            if (wowProcess.Length == 0)
            {
                Console.WriteLine("!!! No WoW process found!");
                Console.ReadLine();
                return;
            }
            Pattern[] patterns =
            {
                Pattern.FromTextstyle("lua_protectedparser", "55 8B EC 83 EC 14 83 7D 14 00 75 07 ?? ?? ?? ?? ?? ?? ?? FF 75 10 8D 45 EC", new AddModifier(0), new LeaModifier(LeaType.SimpleAddress)),
                Pattern.FromTextstyle("GlueState", "83 3d ?? ?? ?? ?? ?? 75 ?? e8 ?? ?? ?? ?? 8b 10 8b c8 ff 62 5c c3", new AddModifier(2), new LeaModifier()),
                Pattern.FromTextstyle("GameState", "80 3d ?? ?? ?? ?? ?? 74 ?? 50 b9 ?? ?? ?? ?? e8 ?? ?? ?? ?? 85 c0 74 ?? 8b 40 08 83 f8 02 74 ?? 83 f8 01 75 ?? b0 01 c3 32 c0 c3", new AddModifier(2), new LeaModifier()),
                Pattern.FromTextstyle("FocusedWidget", "3b 05 ?? ?? ?? ?? 0f 94 c1 51 ff 75 08 e8 ?? ?? ?? ?? 33 c0 83 c4 10 40 5d c3", new AddModifier(2), new LeaModifier()),
                Pattern.FromTextstyle("BlackMarketNumItems", "55 8b ec 8b 0d ?? ?? ?? ?? 33 c0 56 85 c9 74 15 8b 15 ?? ?? ?? ?? 8b 32 3b 75 08 74 0b 40 83 c2 70", new AddModifier(5), new LeaModifier()),
                Pattern.FromTextstyle("BlackMarketItems", "55 8b ec 8b 0d ?? ?? ?? ?? 33 c0 56 85 c9 74 15 8b 15 ?? ?? ?? ?? 8b 32 3b 75 08 74 0b 40 83 c2 70", new AddModifier(18), new LeaModifier()),
                Pattern.FromTextstyle("LastHardwareAction", "55 8b ec a1 ?? ?? ?? ?? 6a 00 50 ff 75 08 e8 ?? ?? ?? ?? 8b c8 e8 ?? ?? ?? ?? 33 c0 5d c3", new AddModifier(4), new LeaModifier()),
                Pattern.FromTextstyle("ObjectManager", "a1 ?? ?? ?? ?? c3 55 8b ec a1 ?? ?? ?? ?? 85 c0 74 17 ff 75 08 68 18 01 00 00", new AddModifier(1), new LeaModifier()),
                Pattern.FromTextstyle("PlayerIsLooting", "e8 ?? ?? ?? ?? 03 05 ?? ?? ?? ?? c6 05 ?? ?? ?? ?? 00 a3 ?? ?? ?? ?? c3", new AddModifier(13), new LeaModifier()),
                Pattern.FromTextstyle("PlayerName", "0f be 05 ?? ?? ?? ?? f7 d8 1b c0 25 ?? ?? ?? ?? c3 a0 ?? ?? ?? ?? c3", new AddModifier(3), new LeaModifier()),
                Pattern.FromTextstyle("PlayerZoneID", "8b 15 ?? ?? ?? ?? 3b 15 ?? ?? ?? ?? 75 06 a0 ?? ?? ?? ?? c3", new AddModifier(2), new LeaModifier()),
                Pattern.FromTextstyle("PlayerPtr", "55 8b ec a1 ?? ?? ?? ?? 83 ec 10 85 c0 75 26 8d 45 f0 50 e8 ?? ?? ?? ?? 68 ce 00 00 00 68", new AddModifier(4), new LeaModifier()),
                Pattern.FromTextstyle("ClntObjMgrGetActivePlayerObj", "55 8b ec 83 ec 10 56 e8 ?? ?? ?? ?? 33 f6 3b c6 0F 84 07 01 00 00 8B 88 24 01 00 00", new AddModifier(8), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("LuaDoStringAddress", "74 0c 6a 00 50 50 e8 ?? ?? ?? ?? 83 c4 0c 33 c0 5d c3", new AddModifier(7), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("LuaGetLocalizedTextAddress", "83 c4 14 6a ff 8d 45 e0 50 8b ce e8 ?? ?? ?? ?? 5e c9 c2 04 00", new AddModifier(12), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("TargetUnit", "C1 E8 1A 83 E0 3F 0F BE F0 F7 DE 1B F6 81 E6 ?? ?? ?? ?? F7 DE 1B F6 68 ?? ?? ?? ?? F7 DE E8 ?? ?? ?? ?? 56 FF 75 08 E8", new AddModifier(31), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("Interact", "8D 45 F0 a5 50 75 0A e8 ?? ?? ?? ?? e9 ?? ?? ?? ?? e8 ?? ?? ?? ?? e9 ?? ?? ?? ?? 8B 83 30 02 00 00", new AddModifier(18), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("HandleTerrainClick", "39 73 1C 0F 85 92 00 00 00 53 e8 ?? ?? ?? ?? 59 84 C0 74 07", new AddModifier(11), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("ClickToMove", "F3 0F 11 04 24 FF 75 08 8B CE 68 ?? ?? ?? ?? 6A 04 e8 ?? ?? ?? ?? 5E 5d C2 04 00", new AddModifier(18), new LeaModifier(LeaType.E8)),
                Pattern.FromTextstyle("CGWorldFrame::Render", "55 8b ec a1 ?? ?? ?? ?? 8b 80 d8 00 00 00 56 57 33 ff 47 a8 01 75 ?? 85 c0 75 ?? 33 c9", new AddModifier(0), new LeaModifier(LeaType.SimpleAddress))
            };
            string reportFilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\result.txt";
            File.Delete(reportFilePath);
            using (ExternalProcessReader epr = new ExternalProcessReader(wowProcess[0]))
            {
                int counter = 0;
                foreach (Pattern pattern in patterns)
                {
                    try
                    {
                        bool alreadyFound = false;
                        foreach (IntPtr intPtr in pattern.Find(epr))
                        {
                            if (!alreadyFound)
                            {
                                Console.WriteLine(pattern.Name + ": 0x" + intPtr.ToInt32().ToString("X"));
                            }
                            else
                            {
                                Console.WriteLine("!!!  " + pattern.Name + ": 0x" + intPtr.ToInt32().ToString("X"));
                            }
                            File.AppendAllLines(reportFilePath, new[] {"internal static readonly int " + pattern.Name + " = 0x" + intPtr.ToInt32().ToString("X") + ";"});
                            alreadyFound = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!!!  " + ex.Message);
                    }
                    counter++;
                    int cursorTop = Console.CursorTop;
                    int cursorLeft = Console.CursorLeft;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.WriteLine("---- " + counter*100/patterns.Length + "% ----");
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }

    public class Pattern
    {
        public string Name { get; private set; }
        public byte[] Bytes { get; private set; }
        public bool[] Mask { get; private set; }
        const int CacheSize = 0x500;
        public List<IModifier> Modifiers = new List<IModifier>();

        private bool DataCompare(byte[] data, uint dataOffset)
        {
            return !Mask.Where((t, i) => t && Bytes[i] != data[dataOffset + i]).Any();
        }

        private IEnumerable<IntPtr> FindStart(ExternalProcessReader bm)
        {
            var mainModule = bm.Process.MainModule;
            var start = mainModule.BaseAddress;
            var size = mainModule.ModuleMemorySize;
            var patternLength = Bytes.Length;

            List<IntPtr> addresses = new List<IntPtr>();

            for (uint i = 0; i < size - patternLength; i += (uint)(CacheSize - patternLength))
            {
                byte[] cache = bm.ReadBytes(start + (int)i, CacheSize > size - i ? size - (int)i : CacheSize);
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

        public IEnumerable<IntPtr> Find(ExternalProcessReader bm)
        {
            foreach (IntPtr intPtr in FindStart(bm))
            {
                IntPtr start = intPtr;
                foreach (IModifier modifier in Modifiers)
                {
                    start = modifier.Apply(bm, start);
                }
                yield return start - (int)bm.Process.MainModule.BaseAddress;
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
        IntPtr Apply(ExternalProcessReader bm, IntPtr address);
    }

    public class AddModifier : IModifier
    {
        public uint Offset { get; private set; }

        public AddModifier(uint val)
        {
            Offset = val;
        }

        public IntPtr Apply(ExternalProcessReader bm, IntPtr addr)
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
        SimpleAddress
    }

    public class LeaModifier : IModifier
    {
        public LeaType Type { get; private set; }

        public LeaModifier()
        {
            Type = LeaType.Dword;
        }

        public LeaModifier(LeaType type)
        {
            Type = type;
        }

        public IntPtr Apply(ExternalProcessReader bm, IntPtr address)
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
            }
            throw new InvalidDataException("Unknown LeaType");
        }
    }

}
