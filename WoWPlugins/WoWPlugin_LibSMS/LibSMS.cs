using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace LibSMS
{
    public class LibSMS : IPlugin3
    {
        #region Info

        public string Name => "LibSMS";

        public Version Version => new Version(1, 0);

        public string Description => "Provides SMS support";

        private Image trayIcon;
        public Image TrayIcon => trayIcon ?? (trayIcon = Image.FromFile($"{Application.StartupPath}\\plugins\\{Name}\\dll.jpg"));

        public string WowIcon => "";

        public bool ConfigAvailable => true;

        public string[] Dependencies => null;

        public bool DontCloseOnWowShutdown => false;

        #endregion Info

        #region Methods

        public void OnConfig()
        {
            if (settingsInstance == null)
            {
                settingsInstance = this.LoadSettingsJSON<Settings>();
            }
            SettingsForm.Open(this, settingsInstance);
            this.SaveSettingsJSON(settingsInstance);
        }

        public void OnStart(GameInterface game)
        {
            settingsInstance = this.LoadSettingsJSON<Settings>();
            this.LogPrint("Successfully started");
        }

        public void OnStop()
        {
        }

        public void SendSMS(string text, GameInterface game)
        {
            if (settingsInstance == null) settingsInstance = this.LoadSettingsJSON<Settings>();
            string smsURL = settingsInstance.SMSAPI;
            if (!string.IsNullOrWhiteSpace(smsURL))
            {
                string escapedText = UrlEncode(text);
                using (WebClient webClient = new WebClient())
                {
                    string downloadString = webClient.DownloadString(smsURL + escapedText);
                    this.LogPrint("SMS is sent, response: " + downloadString.Replace('\r', ' ').Replace('\n', ' '));
                }
            }
            string pbAPIKey = settingsInstance.PushbulletAPIKey;
            string pbRecipient = settingsInstance.PushbulletRecipient;
            if (!string.IsNullOrWhiteSpace(pbAPIKey) && !string.IsNullOrWhiteSpace(pbRecipient))
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers["Content-Type"] = "application/json";
                    webClient.Headers["Access-Token"] = pbAPIKey;
                    webClient.Encoding = Encoding.UTF8;
                    string uploadString = webClient.UploadString("https://api.pushbullet.com/v2/pushes", $"{{\"email\": \"{pbRecipient}\",\"type\": \"note\",\"title\": \"Note from AxTools\",\"body\": \"{text}\"}}");
                    this.LogPrint("Pushbullet note is sent, response: " + uploadString.Replace('\r', ' ').Replace('\n', ' '));
                }
            }
        }

        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (char @char in value)
            {
                if (ReservedCharacters.IndexOf(@char) == -1)
                {
                    sb.Append(@char);
                }
                else
                {
                    sb.AppendFormat("%{0:X2}", (int)@char);
                }
            }
            return sb.ToString();
        }

        #endregion Methods

        #region Vars

        private Settings settingsInstance;
        private static readonly string ReservedCharacters = " !*'();:@&=+$,/?%#[]";

        #endregion Vars
    }
}