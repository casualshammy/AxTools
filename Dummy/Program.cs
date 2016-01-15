using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

namespace Dummy
{
    internal class Program
    {
        private static void Main()
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFile("https://axtools.axio.name:5000/get_update_package%login=Axioma+pass=Ss125521", "u.zip");
                }
                catch (WebException webEx)
                {
                    if (webEx.Status == WebExceptionStatus.SecureChannelFailure || webEx.Status == WebExceptionStatus.TrustFailure)
                    {
                        Console.WriteLine("SSL TRUST ERROR");
                    }
                    Console.WriteLine(webEx.Status);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetType().ToString() + "\r\n" + ex.Message);
                }
                
            }
            

            Console.ReadLine();
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
