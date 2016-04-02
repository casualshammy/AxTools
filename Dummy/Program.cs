using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using SQLite;

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
            Console.WriteLine(default(uint));
        }

        public static void ForceBasicAuth(this WebClient webClient, string username, string password)
        {
            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            webClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
        }

        private static void SendEmail(string email)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("axtoolslogsender@gmail.com", "abrakadabra!pushpush");
                using (MailMessage mailMessage = new MailMessage("axtoolslogsender@gmail.com", email))
                {
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.Subject = "Зафиксировано движение!";
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.Body = "Время: " + DateTime.Now;
                    smtpClient.Send(mailMessage);
                }
            }
        }

    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class DoAction
    {
        [JsonProperty(PropertyName = "String0", Required = Required.Always)]
        internal string String0;

        [JsonProperty(PropertyName = "String1", Required = Required.Always)]
        internal string String1;

        [JsonProperty(PropertyName = "Integer", Required = Required.Always)]
        internal int Integer;
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
