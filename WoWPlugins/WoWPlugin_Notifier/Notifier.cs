using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace WoWPlugin_Notifier
{
    public class Notifier : IPlugin3
    {
        #region Info

        public string Name => nameof(Notifier);

        public Version Version => new Version(1, 0);

        public string Description => "Sends sms on various events (need LibSMS)";

        private Image trayIcon;
        public Image TrayIcon => trayIcon ?? (trayIcon = Image.FromFile($"{Application.StartupPath}\\plugins\\{Name}\\Mobile-Sms-icon.png"));

        public bool ConfigAvailable => true;

        public string[] Dependencies => new[] { "LibSMS" };

        public bool DontCloseOnWowShutdown => false;

        #endregion Info

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
                (timerStaticPopup = this.CreateTimer(15000, game, TimerStaticPopup_OnElapsed)).Start();
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
                    this.LogPrint($"New PM from {msg.Sender}: {msg.Text}");
                    dynamic libSMS = Utilities.GetReferenceOfPlugin("LibSMS");
                    libSMS.SendSMS($"New PM from {msg.Sender}: {msg.Text}", game);
                    this.ShowNotify($"New PM from {msg.Sender}: {msg.Text}", false, false);
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

        #endregion Methods

        private Settings settingsInstance;
        private GameInterface game;
        private SafeTimer timerChat;
        private SafeTimer timerStaticPopup;
        private System.Timers.Timer timerDisconnect;
        private DateTime lastTimeNotifiedAboutDisconnect = DateTime.MinValue;
        private bool running;

        private static readonly Tuple<string, string>[] PopupFrames = {
            new Tuple<string, string>("General popup", "StaticPopup1"),
            new Tuple<string, string>("PvE dungeon invite", "LFGDungeonReadyDialog"),
            //new Tuple<string, string>("PvP dungeon invite", "PVPReadyDialog")
        };
    }
}