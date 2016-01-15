using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dummy.TS3
{
    internal class TS3ClientQuery : IDisposable
    {
        private readonly int port;
        private readonly string host;
        private TelnetConnection telnet;
        private readonly Dictionary<string, Action<string>> specialStrings;
        private volatile bool connected;

        internal TS3ClientQuery(string host, int port)
        {
            this.port = port;
            this.host = host;
            specialStrings = new Dictionary<string, Action<string>>
            {
                {"notifytalkstatuschange", NotifyTalkStatusChanged},
            };
        }

        internal void Connect(bool keepAlive)
        {
            telnet = new TelnetConnection(host, port);
            connected = true;
            if (keepAlive)
            {
                Task.Factory.StartNew(() =>
                {
                    while (connected)
                    {
                        WhoAmI();
                        Thread.Sleep(1000);
                    }
                });
            }
        }

        internal void Disconnect()
        {
            connected = false;
            if (telnet != null)
            {
                telnet.Disconnect();
            }
        }

        internal TS3ClientInfo WhoAmI()
        {
            ParseAndHandle();
            telnet.WriteLine("whoami");
            return TS3ClientInfo.Parse(string.Join("\r\n", ParseAndHandle()));
        }

        internal TS3ClientInfo[] ClientList()
        {
            ParseAndHandle();
            telnet.WriteLine("clientlist");
            return TS3ClientInfo.ParseMultiple(string.Join("\r\n", ParseAndHandle()));
        }

        internal void SubscribeToClientNotifyEvent(string eventName)
        {
            if (specialStrings.ContainsKey(eventName))
            {
                ParseAndHandle();
                telnet.WriteLine("clientnotifyregister schandlerid=0 event=" + eventName);
                ParseAndHandle();
                Console.WriteLine("ClientNotifyEvent is registered: " + eventName);
            }
        }

        internal List<string> ParseAndHandle()
        {
            string data = telnet.Read();
            List<string> list = new List<string>();
            if (!string.IsNullOrWhiteSpace(data))
            {
                List<string> dataLines = data.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                dataLines.RemoveAll(l => l == "error id=0 msg=ok");
                foreach (string line in dataLines)
                {
                    string firstWord = line.Split(' ')[0];
                    if (specialStrings.ContainsKey(firstWord))
                    {
                        specialStrings[firstWord](line);
                    }
                    else
                    {
                        list.Add(line);
                    }
                }
            }
            return list;
        }

        private void NotifyTalkStatusChanged(string s)
        {
            NotifyTalkStatusChange notifyTalkStatus = NotifyTalkStatusChange.Parse(s);
            TS3ClientInfo client = ClientList().FirstOrDefault(l => l.CLID == notifyTalkStatus.CLID);
            string name = client != null ? client.Name : "UNKNOWN";
            if (notifyTalkStatus.Status)
            {
                Console.WriteLine(name + " is speaking");
            }
            else
            {
                Console.WriteLine(name + " is not speaking");
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }

    internal class SpecialStrings
    {
        
    }
}
