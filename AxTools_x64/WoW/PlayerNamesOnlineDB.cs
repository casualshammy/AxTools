using System;
using System.Net;
using System.Text;
using AxTools.Helpers;
using AxTools.WoW.Internals;

namespace AxTools.WoW
{
    internal static class PlayerNamesOnlineDB
    {

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
