using System;

namespace Dummy.TS3
{
    internal class NotifyTalkStatusChange
    {
        internal bool Status;
        internal int CLID;

        internal NotifyTalkStatusChange(int clid, bool status)
        {
            Status = status;
            CLID = clid;
        }

        internal static NotifyTalkStatusChange Parse(string data)
        {
            string[] strings = data.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            int clid = 0;
            bool status = false;
            foreach (string s in strings)
            {
                if (s.Contains("clid="))
                {
                    clid = int.Parse(s.Split('=')[1]);
                }
                else if (s.Contains("status="))
                {
                    status = s.Split('=')[1] == "1";
                }
            }
            return new NotifyTalkStatusChange(clid, status);
        }
    }
}
