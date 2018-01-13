using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using KeyboardWatcher;
using Newtonsoft.Json;

namespace Dummy
{
    internal static class Program
    {
        private static void Main()
        {

            Test();

            Console.WriteLine();
            Console.WriteLine(DateTime.UtcNow + " || Main() is finishing...");
            Console.ReadLine();
        }

        private static void Test()
        {
            string rawText = File.ReadAllText(@"C:\Program Files (x86)\AxTools\cfg\test.json", Encoding.UTF8);
            KeyExt key = JsonConvert.DeserializeObject<KeyExt>(rawText);
            Console.WriteLine("0: " + key);
        }

    }

    

}
