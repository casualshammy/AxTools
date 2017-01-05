using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using AxTools.Helpers;
using AxTools.WoW.Internals;

namespace AxTools.WoW
{
    internal static class PlayerNamesOnlineDB
    {

        static PlayerNamesOnlineDB()
        {
            WoWAntiKick.AntiAFKTriggered += WoWAntiKickOnAntiAFKTriggered;
        }

        private static void WoWAntiKickOnAntiAFKTriggered(WowProcess wowProcess)
        {
            if (WoWManager.WoWProcess == wowProcess && WoWManager.Hooked)
            {
                List<WowPlayer> players = new List<WowPlayer>();
                WoWPlayerMe me = PluginSystem.API.ObjMgr.Pulse(players);
                if (me != null && players.Count > 0)
                {
                    WowPlayer[] unknownPlayers = players.Where(l => string.IsNullOrWhiteSpace(GetPlayerName(l.GUID))).ToArray();
                    int startTime = Environment.TickCount;
                    int counter = 0;
                    while (counter < unknownPlayers.Length && Environment.TickCount - startTime < 10*1000)
                    {
                        WowPlayer player = unknownPlayers[counter];
                        Log.Error(SendPlayerName(player.GUID, player.Name)
                            ? string.Format("PlayerNamesOnlineDB: successfully sent name for {0} ({1})", player.GUID, player.Name)
                            : string.Format("PlayerNamesOnlineDB: can't sent info for {0} ({1}), SendPlayerName has returned FALSE!", player.GUID, player.Name));
                    }
                }
            }
        }

        internal static string GetPlayerName(WoWGUID playerGUID)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(Settings.Instance.UserID, Utils.GetComputerHID());
                webClient.Encoding = Encoding.UTF8;
                string request = "Action: get-player-name\nPlayerGUID: " + playerGUID;
                string response = webClient.UploadString("https://axio.name/axtools/player-names-db/db.php", "POST", request);
                string name = Encoding.UTF8.GetString(Convert.FromBase64String(response));
                Log.Error(string.Format("PlayerNamesOnlineDB.GetPlayerName: {0}: {1}", playerGUID, name));
                return name;
            }
        }

        internal static bool IsPlayerNameKnown(WoWGUID playerGUID)
        {
            throw new NotImplementedException();
        }

        internal static bool SendPlayerName(WoWGUID playerGUID, string name)
        {
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(name));
            using (WebClient webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(Settings.Instance.UserID, Utils.GetComputerHID());
                webClient.Encoding = Encoding.UTF8;
                string request = string.Format("Action: commit-player-name\nPlayerGUID: {0}\nPlayerName: {1}", playerGUID, base64);
                return webClient.UploadString("https://axio.name/axtools/player-names-db/db.php", "POST", request) == "OK";
            }
        }

    }
}
