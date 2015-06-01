using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fasm;
using Newtonsoft.Json;

namespace FASMHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            string filepath = args[0].Trim('"');
            string content = File.ReadAllText(filepath);
            List<string> asm = JsonConvert.DeserializeObject<List<string>>(content);
            ManagedFasm fasm = new ManagedFasm();
            fasm.Clear();
            foreach (string str in asm)
            {
                fasm.AddLine(str);
            }
            File.WriteAllBytes(filepath.Replace(".json", ".asmbyte"), fasm.Assemble());
        }
    }
}
