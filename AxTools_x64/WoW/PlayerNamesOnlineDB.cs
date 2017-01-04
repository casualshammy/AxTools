using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxTools.WoW.Internals;

namespace AxTools.WoW
{
    internal static class PlayerNamesOnlineDB
    {

        internal static string GetPlayerName(WoWGUID playerGUID)
        {
            
        }

        internal static bool IsPlayerNameKnown(WoWGUID playerGUID)
        {
            
        }

        internal static void SendPlayerName(WoWGUID playerGUID, string name)
        {
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(name));

        }

    }
}
