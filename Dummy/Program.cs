using System;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

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
            

            Console.WriteLine("Main() is finishing...");
            Console.ReadLine();
        }

        private static void Test()
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    //webClient.Credentials = new NetworkCredential("Axio-5GDMJHD20R", "3BFCE06892A8AAE50818625702B0C4CA93F57CF7AEC02146F416EE278D31F478");
                    webClient.ForceBasicAuth("Axio-5GDMJHD20R_", "3BFCE06892A8AAE50818625702B0C4CA93F57CF7AEC02146F416EE278D31F478");
                    //webClient.DownloadFile("https://axtools.axio.name:5000/get_update_info", "files\\" + l.ToString() + ".json");
                    Console.WriteLine(webClient.DownloadString("https://axtools.axio.name/upd/_update1.json"));
                }
                catch (WebException webEx)
                {
                    if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response is HttpWebResponse && ((HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("INCORRECT LOGIN OR PASSWORD");
                    }
                    else if (webEx.Status == WebExceptionStatus.TrustFailure || webEx.Status == WebExceptionStatus.SecureChannelFailure)
                    {
                        Console.WriteLine("SSL ERROR");
                    }
                    else
                    {
                        Console.WriteLine(webEx.GetType() + ": " + webEx.Status + ":\r\n" + webEx.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetType() + ":\r\n" + ex.Message);
                }
            }
        }

        public static void ForceBasicAuth(this WebClient webClient, string username, string password)
        {
            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            webClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
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
    
}
