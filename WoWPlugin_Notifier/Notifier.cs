using System;
using System.Drawing;
using System.Threading;
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

        public string WowIcon { get { return ""; } }

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
        }

        private void TimerStaticPopup_OnElapsed()
        {
            WoWUIFrame frame = WoWUIFrame.GetFrameByName("StaticPopup1");
            if (frame != null && frame.IsVisible)
            {
                this.LogPrint("Static popup is visible!");
                dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                libSMS.SendSMS("Static popup is visible!");
                for (int i = 0; i < 60; i++)
                {
                    if (timerStaticPopup.IsRunning)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private void TimerChat_OnElapsed()
        {
            GameFunctions.ReadChat();
        }

        private void GameFunctionsOnNewChatMessage(ChatMsg obj)
        {
            if ((settingsInstance.OnWhisper && obj.Type == 7) || (settingsInstance.OnBNetWhisper && obj.Type == 51))
            {
                this.LogPrint(string.Format("New PM from {0}: {1}", obj.Sender, obj.Text));
                dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                libSMS.SendSMS(string.Format("New PM from {0}: {1}", obj.Sender, obj.Text));
            }
        }

        

        #endregion

        private Settings settingsInstance;
        private SafeTimer timerChat;
        private SafeTimer timerStaticPopup;

    }
}
