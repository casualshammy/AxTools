using System;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_Notifier
{
    public class Notifier : IPlugin
    {

        #region Info

        public string Name { get { return "Notifier"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Sends sms on various events (need LibSMS)"; } }

        private Image trayIcon;
        public Image TrayIcon { get { return trayIcon ?? (trayIcon = Image.FromFile(string.Format("{0}\\plugins\\{1}\\Mobile-Sms-icon.png", Application.StartupPath, Name))); } }

        public bool ConfigAvailable { get { return true; } }

        #endregion

        #region Methods

        public void OnConfig()
        {
            if (settingsInstance == null)
            {
                settingsInstance = this.LoadSettingsJSON<Settings>();
            }
            new SettingsForm(settingsInstance).ShowDialog();
            this.SaveSettingsJSON(settingsInstance);
        }

        public void OnStart()
        {
            settingsInstance = this.LoadSettingsJSON<Settings>();
            dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
            if (libSMS == null)
            {
                this.ShowNotify("LibSMS isn't found! Plugin will not work.", true, true);
                this.LogPrint("LibSMS isn't found! Plugin will not work.");
                return;
            }
            if (libSMS.GetType().GetMethod("SendSMS") == null)
            {
                this.ShowNotify("LibSMS.SendSMS method isn't found! Plugin will not work.", true, true);
                this.LogPrint("LibSMS.SendSMS method isn't found! Plugin will not work.");
                return;
            }
            this.LogPrint("LibSMS is OK");
            if (settingsInstance.OnWhisper || settingsInstance.OnBNetWhisper)
            {
                GameFunctions.ReadChat();
                GameFunctions.NewChatMessage += GameFunctionsOnNewChatMessage;
                (timerChat = this.CreateTimer(1000, TimerChat_OnElapsed)).Start();
                this.LogPrint("Whisper notification enabled");
            }
            if (settingsInstance.OnStaticPopup)
            {
                (timerStaticPopup = this.CreateTimer(15000, TimerStaticPopup_OnElapsed)).Start();
                this.LogPrint("StaticPopup notification enabled");
            }
            if (settingsInstance.OnDisconnect)
            {
                timerDisconnect = new System.Timers.Timer(100);
                timerDisconnect.Elapsed += TimerDisconnect_OnElapsed;
                timerDisconnect.Start();
                this.LogPrint("Disconnect notification enabled");
            }
            running = true;
            this.LogPrint("Successfully started");
        }

        public void OnStop()
        {
            if ((settingsInstance.OnWhisper || settingsInstance.OnBNetWhisper) && timerChat != null)
            {
                GameFunctions.NewChatMessage -= GameFunctionsOnNewChatMessage;
                timerChat.Dispose();
            }
            if (settingsInstance.OnStaticPopup && timerStaticPopup != null)
            {
                timerStaticPopup.Dispose();
            }
            if (settingsInstance.OnDisconnect)
            {
                TimerDisconnect_OnElapsed(null, null);
                if (timerDisconnect != null)
                {
                    timerDisconnect.Dispose();
                }
            }
            running = false;
        }

        private void TimerStaticPopup_OnElapsed()
        {
            Tuple<string, string>[] frameNames = { new Tuple<string, string>("General popup", "StaticPopup1") }; // , new Tuple<string, string>("PvP invite dialog", "PVPReadyDialogEnterBattleButton")
            foreach (Tuple<string, string> tuple in frameNames)
            {
                WoWUIFrame frame = WoWUIFrame.GetFrameByName(tuple.Item2);
                if (frame != null && frame.IsVisible)
                {
                    this.LogPrint(tuple.Item1 + " is visible!");
                    string message = tuple.Item1 + " is visible!";
                    dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                    libSMS.SendSMS(message);
                    this.ShowNotify(message, false, false);
                    this.LogPrint("Message is sent: " + message);
                    for (int i = 0; i < 60; i++)
                    {
                        if (timerStaticPopup.IsRunning)
                        {
                            Thread.Sleep(1000);
                            
                        }
                    }
                    break;
                }
            }
        }

        private void TimerChat_OnElapsed()
        {
            GameFunctions.ReadChat();
        }

        private void TimerDisconnect_OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!GameFunctions.IsInGame && (DateTime.UtcNow - lastTimeNotifiedAboutDisconnect).TotalSeconds > 60)
            {
                lastTimeNotifiedAboutDisconnect = DateTime.UtcNow;
                this.LogPrint("Player is not in game!");
                this.ShowNotify("Player is not in game!", false, false);
                dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                libSMS.SendSMS("Player is not in game!");
            }
        }

        private void GameFunctionsOnNewChatMessage(ChatMsg obj)
        {
            if ((settingsInstance.OnWhisper && obj.Type == WoWChatMsgType.Whisper) || (settingsInstance.OnBNetWhisper && obj.Type == WoWChatMsgType.BNetWisper))
            {
                this.LogPrint(string.Format("New PM from {0}: {1}", obj.Sender, obj.Text));
                dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                libSMS.SendSMS(string.Format("New PM from {0}: {1}", obj.Sender, obj.Text));
                this.ShowNotify(string.Format("New PM from {0}: {1}", obj.Sender, obj.Text), false, false);
            }
        }

        #endregion

        private Settings settingsInstance;
        private SafeTimer timerChat;
        private SafeTimer timerStaticPopup;
        private System.Timers.Timer timerDisconnect;
        private DateTime lastTimeNotifiedAboutDisconnect = DateTime.MinValue;
        private bool running = false;

    }
}
