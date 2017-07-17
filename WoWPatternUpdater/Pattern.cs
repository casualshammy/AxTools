using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WoWPatternUpdater
{
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

        private IEnumerable<IntPtr> FindStart(MemoryManager bm)
        {
            ProcessModule mainModule = bm.Process.MainModule;
            IntPtr start = mainModule.BaseAddress;
            long size = mainModule.ModuleMemorySize;
            long patternLength = Bytes.LongLength;

            List<IntPtr> addresses = new List<IntPtr>();

            for (long i = 0; i < size - patternLength; i += CacheSize - patternLength)
            {
                byte[] cache = bm.ReadBytes(start + (int)i, (int)(CacheSize > size - i ? size - i : CacheSize));
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

        public IEnumerable<IntPtr> Find(MemoryManager bm)
        {
            foreach (IntPtr intPtr in FindStart(bm))
            {
                IntPtr start = intPtr;
                foreach (IModifier modifier in Modifiers)
                {
                    start = modifier.Apply(bm, start);
                }
                yield return new IntPtr((long)start - (long)bm.Process.MainModule.BaseAddress);
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
}
