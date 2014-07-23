using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using WoWGold_Notifier.Properties;
using WoWGold_Notifier.WinAPI;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Timer = System.Timers.Timer;

namespace WoWGold_Notifier
{
    public partial class MainForm : Form
    {
        private readonly List<string> orders = new List<string>();
        private readonly NotifyIcon notifyIcon;
        private readonly Timer timer = new Timer(1000);
        private readonly object _lock = new object();
        private bool shouldInformUser;
        private readonly Action navigate;
        private readonly Action printTime;
        private readonly AutoResetEvent are;
        private readonly string logFilePath = Application.StartupPath + "\\log.txt";
        private readonly string site = "https://supply.elfmoney.ru/";
        // ReSharper disable InconsistentNaming
        private static readonly int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        private static readonly int SET_FEATURE_ON_PROCESS = 0x00000002;
        // ReSharper restore InconsistentNaming

        public MainForm()
        {
            InitializeComponent();
            DisableIEClickSounds();
            WebRequest.DefaultWebProxy = null;
            webBrowser1.DocumentCompleted += WebBrowserOnDocumentCompleted;
            are = new AutoResetEvent(true);
            navigate = () => webBrowser1.Navigate(site);
            printTime = () => { label1.Text = String.Format("Last updated: {0:HH:mm:ss}", DateTime.Now); };
            notifyIcon = new NotifyIcon {Icon = Icon.FromHandle(Resources.coins.GetHicon()), Visible = true, Text = "WoWGold Notifier"};
            timer.Elapsed += TimerOnElapsed;
            timer.Start();
        }

        private void WebBrowserOnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                HtmlDocument p = new HtmlDocument { OptionDefaultStreamEncoding = Encoding.UTF8 };
                p.Load(webBrowser1.DocumentStream);
                foreach (HtmlNode node in p.DocumentNode.Descendants("tr").Where(i => i.Attributes.Contains("data-id")).Reverse())
                {
                    if (!orders.Contains(node.Attributes["data-id"].Value))
                    {
                        HtmlNode btn = node.Descendants("td").ToArray()[7];
                        if (btn.InnerText.Contains("Продать"))
                        {
                            string server = node.Descendants("td").ToArray()[2].InnerText;
                            if (server.Contains("Гордунни") && shouldInformUser)
                            {
                                string amount = node.Descendants("td").ToArray()[4].InnerText;
                                try
                                {
                                    if (btn.InnerText.Contains("Продать"))
                                    {
                                        string href = btn.Descendants("a").ToArray()[0].Attributes["href"].Value;
                                        webBrowser1.Navigate(site + href);
                                        SendSMS(server + " - " + amount);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log("Click error: " + ex.Message);
                                }
                                SystemSounds.Exclamation.Play();
                                notifyIcon.ShowBalloonTip(30000, "WoWGold - New Order!", server + " - " + amount, ToolTipIcon.Warning);
                                FLASHWINFO flashwinfo = new FLASHWINFO
                                {
                                    cbSize = (uint)Marshal.SizeOf(typeof(FLASHWINFO)),
                                    hwnd = Handle,
                                    dwFlags = FlashWindowFlags.FLASHW_TRAY | FlashWindowFlags.FLASHW_TIMERNOFG
                                };
                                NativeMethods.FlashWindowEx(ref flashwinfo);
                            }
                            orders.Add(node.Attributes["data-id"].Value);
                        }
                    }
                }
                shouldInformUser = true;
                Invoke(printTime);
            }
            catch (Exception ex)
            {
                Log("WebBrowserOnDocumentCompleted error: " + ex.Message);
            }
            are.Set();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            int threadCount = Process.GetCurrentProcess().Threads.Count;
            BeginInvoke((Action) (() => { labelThreads.Text = "Threads: " + threadCount; }));
            if (threadCount > 50)
            {
                Log("WoWGold.Ru: Something went wrong! (>50 threads)");
                SendSMS("WoWGold.Ru: Something went wrong! (>50 threads)");
            }
            lock (_lock)
            {
                if (!are.WaitOne(10000))
                {
                    Log("WoWGold.Ru: Something went wrong!");
                    //SendSMS("WoWGold.Ru: Something went wrong!");
                }
                webBrowser1.Invoke(navigate);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }

        private void buttonGoToSite_Click(object sender, EventArgs e)
        {
            Process.Start(site);
        }

        private void SendSMS(string text)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadString("http://sms.ru/sms/send?api_id=891233b0-11a1-0ce4-dd8d-4ac68fa5b0ac&to=79052051023&text=" + text.Replace(' ', '+'));
            }
        }

        private void Log(string text)
        {
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath);
            }
            File.AppendAllLines(logFilePath, new[] {DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss.fff   ") + text}, Encoding.UTF8);
        }

        private void DisableIEClickSounds()
        {
            NativeMethods.CoInternetSetFeatureEnabled(FEATURE_DISABLE_NAVIGATION_SOUNDS, SET_FEATURE_ON_PROCESS, true);
        }

        private void buttonLog_Click(object sender, EventArgs e)
        {
            Process.Start(logFilePath);
        }
    
    }
}
