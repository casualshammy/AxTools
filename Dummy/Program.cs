using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Dummy
{
    internal static class Program
    {
        private static void Main()
        {

            //Stopwatch stopwatch = Stopwatch.StartNew();
            //int counter = 0;
            //Parallel.For(0, 1000, l =>
            //{
            //    using (WebClient webClient = new WebClient())
            //    {
            //        try
            //        {
            //            //webClient.Credentials = new NetworkCredential("Axio-5GDMJHD20R", "3BFCE06892A8AAE50818625702B0C4CA93F57CF7AEC02146F416EE278D31F478");
            //            webClient.ForceBasicAuth("Axio-5GDMJHD20R", "3BFCE06892A8AAE50818625702B0C4CA93F57CF7AEC02146F416EE278D31F478");
            //            //webClient.DownloadFile("https://axtools.axio.name:5000/get_update_info", "files\\" + l.ToString() + ".json");
            //            webClient.DownloadFile("https://axtools.axio.name/upd/_update0.json", "files\\" + l.ToString() + ".json");
            //        }
            //        catch (WebException webEx)
            //        {
            //            if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response is HttpWebResponse && ((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.Unauthorized)
            //            {
            //                Console.WriteLine("INCORRECT LOGIN OR PASSWORD");
            //            }
            //            else if (webEx.Status == WebExceptionStatus.TrustFailure || webEx.Status == WebExceptionStatus.SecureChannelFailure)
            //            {
            //                Console.WriteLine("SSL ERROR");
            //            }
            //            else
            //            {
            //                Console.WriteLine(webEx.GetType() + ": " + webEx.Status + ":\r\n" + webEx.Message);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.GetType() + ":\r\n" + ex.Message);
            //        }
            //    }
            //    counter++;
            //    Console.WriteLine("Processed: " + counter);
            //});
            //Console.WriteLine("Stopwatch: " + stopwatch.ElapsedMilliseconds / 1000);

            Test();

            Console.WriteLine();
            Console.WriteLine(DateTime.UtcNow + " || Main() is finishing...");
            Console.ReadLine();
        }

        private static void Test()
        {
            string p = "";
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                p = GetRandomStringA(8);
            }
            Console.WriteLine("GetRandomStringA: " + stopwatch.ElapsedMilliseconds + "ms, " + p);
            stopwatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                p = GetRandomStringB(8);
            }
            Console.WriteLine("GetRandomStringB: " + stopwatch.ElapsedMilliseconds + "ms, " + p);
            stopwatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                p = GetRandomStringA(8);
            }
            Console.WriteLine("GetRandomStringA: " + stopwatch.ElapsedMilliseconds + "ms, " + p);
            stopwatch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                p = GetRandomStringB(8);
            }
            Console.WriteLine("GetRandomStringB: " + stopwatch.ElapsedMilliseconds + "ms, " + p);
        }

        internal static readonly Random Rnd = new Random();

        internal static string GetRandomStringA(int size)
        {
            StringBuilder builder = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Rnd.NextDouble() + 65)));
                if (Rnd.Next(10) % 2 == 0)
                {
                    ch = ch.ToString().ToLower()[0];
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        internal static string GetRandomStringB(int size)
        {
            StringBuilder builder = new StringBuilder(size);
            string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < size; i++)
            {
                char c = chars[Rnd.Next(0, chars.Length)];
                builder.Append(c);
            }
            return builder.ToString();
        }

    }

    
    public class DoAction
    {
        public DoAction()
        {
            
        }

        public string String0 { get; set; }

        public string String1 { get; set; }

        public int Id { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class WowheadItemInfo
    {
        [JsonConstructor]
        internal WowheadItemInfo()
        {

        }

        internal WowheadItemInfo(string name, uint @class, uint level, uint quality)
        {
            Name = name;
            Class = @class;
            Level = level;
            Quality = quality;
        }

        [JsonProperty(PropertyName = "Name")]
        internal string Name;

        [JsonProperty(PropertyName = "Class")]
        internal uint Class;

        [JsonProperty(PropertyName = "Level")]
        internal uint Level;

        [JsonProperty(PropertyName = "Quality")]
        internal uint Quality;

    }

}
