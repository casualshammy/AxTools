using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Runtime.Serialization.Json;

namespace JSONTester
{
    class Program
    {
        static void Main()
        {
            using (WebClient webClient = new WebClient())
            {
                Console.WriteLine("Enter channel name:");
                string channel = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Please wait...");
                string json;
                try
                {
                    json = webClient.DownloadString("http://api.justin.tv/api/stream/list.json?channel=" + channel);
                }
                catch (Exception ex)
                {
                    Console.Write("webClient.DownloadString\r\n" + ex);
                    Console.ReadLine();
                    return;
                }
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    List<TwitchStreamInfo> list;
                    try
                    {
                        list = (List<TwitchStreamInfo>)new DataContractJsonSerializer(typeof(List<TwitchStreamInfo>)).ReadObject(memoryStream);
                    }
                    catch (Exception ex)
                    {
                        Console.Write("new DataContractJsonSerializer\r\n" + ex);
                        Console.ReadLine();
                        return;
                    }
                    try
                    {
                        if (list.Count > 0)
                        {
                            TwitchStreamInfo twitchStreamInfo = list[0];
                            Console.Clear();
                            Console.WriteLine("Channel: " + channel);
                            Console.WriteLine("Game: " + twitchStreamInfo.MetaGame);
                            Console.WriteLine("Title: " + twitchStreamInfo.Title);
                            Console.WriteLine("Video size: " + twitchStreamInfo.VideoWidth + "x" + twitchStreamInfo.VideoHeight);
                            Console.WriteLine("Video bitrate: " + twitchStreamInfo.VideoBitrate);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Stream offline");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write("Console.Write(list[0].VideoBitrate);\r\n" + ex);
                        Console.ReadLine();
                        return;
                    }
                    Console.ReadLine();
                }
            }
        }

        [Serializable]
        [DataContract(Name = "TwitchStreamInfo")]
        private class TwitchStreamInfo
        {
            [DataMember(Name = "video_bitrate")]
            internal float VideoBitrate;

            [DataMember(Name = "meta_game")]
            internal string MetaGame;

            [DataMember(Name = "title")]
            internal string Title;

            [DataMember(Name = "video_height")]
            internal int VideoHeight;

            [DataMember(Name = "video_width")]
            internal int VideoWidth;
        }
    }
}
