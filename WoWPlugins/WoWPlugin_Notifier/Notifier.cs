using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_Notifier
{
    public class Notifier : IPlugin2
    {

        #region Info

        public string Name { get { return "Notifier"; } }

        public Version Version { get { return new Version(1, 0); } }

        public string Description { get { return "Sends sms on various events (need LibSMS)"; } }

        private Image trayIcon;
        public Image TrayIcon { get { return trayIcon ?? (trayIcon = Image.FromFile(string.Format("{0}\\plugins\\{1}\\Mobile-Sms-icon.png", Application.StartupPath, Name))); } }

        public bool ConfigAvailable { get { return true; } }

        public string[] Dependencies => new string[] { "LibSMS" };

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

        public void OnStart(GameInterface game)
        {
            this.game = game;
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
                // we don't want to read old messages
                game.ReadChat().ToArray();
                (timerChat = this.CreateTimer(1000, game, TimerChat_OnElapsed)).Start();
                this.LogPrint("Whisper notification enabled");
            }
            if (settingsInstance.OnStaticPopup)
            {
                (timerStaticPopup = this.CreateTimer(15000,game, TimerStaticPopup_OnElapsed)).Start();
                foreach (var i in PopupFrames)
                {
                    this.LogPrint($"{i.Item1} --> {i.Item2}");
                }
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
            foreach (Tuple<string, string> tuple in PopupFrames)
            {
                WoWUIFrame frame = WoWUIFrame.GetFrameByName(game, tuple.Item2);
                if (frame != null && frame.IsVisible)
                {
                    this.LogPrint(tuple.Item1 + " is visible!");
                    string message = tuple.Item1 + " is visible!";
                    dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                    libSMS.SendSMS(message, game);
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
            foreach (var msg in game.ReadChat())
            {
                if ((settingsInstance.OnWhisper && msg.Type == WoWChatMsgType.Whisper) || (settingsInstance.OnBNetWhisper && msg.Type == WoWChatMsgType.BNetWisper))
                {
                    this.LogPrint(string.Format("New PM from {0}: {1}", msg.Sender, msg.Text));
                    dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                    libSMS.SendSMS(string.Format("New PM from {0}: {1}", msg.Sender, msg.Text), game);
                    this.ShowNotify(string.Format("New PM from {0}: {1}", msg.Sender, msg.Text), false, false);
                }
            }
        }

        private void TimerDisconnect_OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!game.IsInGame && (DateTime.UtcNow - lastTimeNotifiedAboutDisconnect).TotalSeconds > 60)
            {
                lastTimeNotifiedAboutDisconnect = DateTime.UtcNow;
                this.LogPrint("Player is not in game!");
                this.ShowNotify("Player is not in game!", false, false);
                dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                libSMS.SendSMS("Player is not in game!", game);
            }
        }
        
        #endregion

        private Settings settingsInstance;
        private GameInterface game;
        private SafeTimer timerChat;
        private SafeTimer timerStaticPopup;
        private System.Timers.Timer timerDisconnect;
        private DateTime lastTimeNotifiedAboutDisconnect = DateTime.MinValue;
        private bool running = false;
        private static Tuple<string, string>[] PopupFrames = {
            new Tuple<string, string>("General popup", "StaticPopup1"),
            new Tuple<string, string>("PvE dungeon invite", "LFGDungeonReadyDialog"),
            //new Tuple<string, string>("PvP dungeon invite", "PVPReadyDialog")
        };

    }
}
