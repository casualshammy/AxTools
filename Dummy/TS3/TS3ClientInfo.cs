using System;
using System.Collections.Generic;
using System.Linq;

namespace Dummy.TS3
{
    internal class TS3ClientInfo
    {
        internal int CLID;
        internal int CID;
        internal string Name;

        internal TS3ClientInfo(int clid, int cid, string name)
        {
            CLID = clid;
            CID = cid;
            Name = name;
        }

        internal static TS3ClientInfo Parse(string data)
        {
            string[] strings = data.Split(new[] {" ", "\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            int clid = 0;
            int cid = 0;
            string name = null;
            foreach (string s in strings)
            {
                if (s.Contains("clid"))
                {
                    clid = int.Parse(s.Split('=')[1]);
                }
                else if (s.Contains("cid"))
                {
                    cid = int.Parse(s.Split('=')[1]);
                }
                else if (s.Contains("client_nickname"))
                {
                    name = s.Split('=')[1];
                }
            }
            return new TS3ClientInfo(clid, cid, name);
        }

        internal static TS3ClientInfo[] ParseMultiple(string data)
        {
            string[] clientsRawData = data.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            return clientsRawData.Select(Parse).ToArray();
        }
    }
}
