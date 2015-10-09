using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MyMemory;
using MyMemory.Hooks;
using Newtonsoft.Json;

namespace Dummy
{
    class Program
    {
        private unsafe static void Main(string[] args)
        {
            Color color = Color.FromArgb(111, 123, 222);
            Console.WriteLine(JsonConvert.SerializeObject(color));
            Console.ReadLine();
        }

        
    }
}
