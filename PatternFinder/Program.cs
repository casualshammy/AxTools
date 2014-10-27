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
        static void Main(string[] args)
        {
            Process[] wowProcess = Process.GetProcessesByName("Wow");
            if (wowProcess.Length > 0)
            {
                using (ExternalProcessReader epr = new ExternalProcessReader(wowProcess[0]))
                {
                    Pattern glueStatePattern = Pattern.FromTextstyle("GlueState", "83 3d ?? ?? ?? ?? ?? 75 ?? e8 ?? ?? ?? ?? 8b 10 8b c8 ff 62 5c c3", new AddModifier(2), new LeaModifier());
                    Pattern gameStatePattern = Pattern.FromTextstyle("GameState", "80 3d ?? ?? ?? ?? ?? 74 ?? 50 b9 ?? ?? ?? ?? e8 ?? ?? ?? ?? 85 c0 74 ?? 8b 40 08 83 f8 02 74 ?? 83 f8 01 75 ?? b0 01 c3 32 c0 c3", new AddModifier(2), new LeaModifier());
                    Pattern focusedWidgetPattern = Pattern.FromTextstyle("FocusedWidget", "3b 05 ?? ?? ?? ?? 0f 94 c1 51 ff 75 08 e8 ?? ?? ?? ?? 33 c0 83 c4 10 40 5d c3", new AddModifier(2), new LeaModifier());
                    Console.WriteLine("GlueState: 0x" + glueStatePattern.Find(epr).ToInt32().ToString("X"));
                    Console.WriteLine("GameState: 0x" + gameStatePattern.Find(epr).ToInt32().ToString("X"));
                    Console.WriteLine("FocusedWidget: 0x" + focusedWidgetPattern.Find(epr).ToInt32().ToString("X"));
                }
                Console.ReadLine();
            }
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

        private IntPtr FindStart(ExternalProcessReader bm)
        {
            var mainModule = bm.Process.MainModule;
            var start = mainModule.BaseAddress;
            var size = mainModule.ModuleMemorySize;
            var patternLength = Bytes.Length;

            for (uint i = 0; i < size - patternLength; i += (uint)(CacheSize - patternLength))
            {
                byte[] cache = bm.ReadBytes(start + (int)i, CacheSize > size - i ? size - (int)i : CacheSize);
                for (uint i2 = 0; i2 < (cache.Length - patternLength); i2++)
                {
                    if (DataCompare(cache, i2))
                        return start + (int)(i + i2);
                }
            }
            throw new InvalidDataException(string.Format("Pattern {0} not found", Name));
        }

        public IntPtr Find(ExternalProcessReader bm)
        {
            var start = FindStart(bm);
            start = Modifiers.Aggregate(start, (current, mod) => mod.Apply(bm, current));
            return start - (int)bm.Process.MainModule.BaseAddress;
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
        Dword
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
                    return (IntPtr)bm.Read<byte>(address);
                case LeaType.Word:
                    return (IntPtr)bm.Read<ushort>(address);
                case LeaType.Dword:
                    return (IntPtr)bm.Read<uint>(address);
            }
            throw new InvalidDataException("Unknown LeaType");
        }
    }

}
