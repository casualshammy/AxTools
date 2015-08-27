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
using System.Windows.Forms;
using WoWGold_Notifier.Properties;
using WoWGold_Notifier.WinAPI;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WoWGold_Notifier
{
    public partial class MainForm : Form
    {
        private readonly List<string> orders = new List<string>();
        private readonly NotifyIcon notifyIcon;
        private readonly Thread timerThread;
        private bool _lock = true;
        private bool shouldInformUser;
        private readonly string logFilePath = Application.StartupPath + "\\log.txt";
        private readonly Stopwatch stopwatch;
        private CookieContainer cookies;
        private static int _timerDefaultInterval = 100;
        private const int TIMER_INTERVAL = 100;
        private static readonly Uri SITE = new Uri("http://supply.elfmoney.ru");
        private const string ButtonToClickText = "Выполнить";
        private const string ServerName = "Гордунни";
        private const int MaxValue = 40000;

        public MainForm()
        {
            InitializeComponent();
            stopwatch = new Stopwatch();
            WebRequest.DefaultWebProxy = null;
            webBrowser1.DocumentCompleted += WebBrowserOnDocumentCompleted;
            webBrowser1.ScriptErrorsSuppressed = true;
            cookies = GetUriCookieContainer(SITE);
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            notifyIcon = new NotifyIcon {Icon = Icon.FromHandle(Resources.coins.GetHicon()), Visible = true, Text = "WoWGold Notifier"};
            timerThread = new Thread(TimerThreadFunc);
            timerThread.Start();
        }

        private void WebBrowserOnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            cookies = GetUriCookieContainer(SITE);
        }

        private void On503Error()
        {
            labelResponse.Text = "Response: 503 Error";
            _timerDefaultInterval += 5;
        }

        private void ProcessPage()
        {
            _timerDefaultInterval = Math.Max(_timerDefaultInterval - 1, TIMER_INTERVAL);
            try
            {
                string source = HttpGet(SITE);
                if (source.Contains("503 Service Temporarily Unavailable"))
                {
                    On503Error();
                }
                else if (!source.Contains(ButtonToClickText))
                {
                    labelResponse.Text = "Response: Auth error?";
                }
                else
                {
                    labelResponse.Text = "Response: 200 OK";
                }
                HtmlDocument p = new HtmlDocument {OptionDefaultStreamEncoding = Encoding.UTF8};
                p.LoadHtml(source);
                foreach (HtmlNode node in p.DocumentNode.Descendants("tr").Where(i => i.Attributes.Contains("data-id")).Reverse())
                {
                    if (!orders.Contains(node.Attributes["data-id"].Value))
                    {
                        HtmlNode btn = node.Descendants("td").ToArray()[7];
                        if (btn.InnerText.Contains(ButtonToClickText))
                        {
                            string server = node.Descendants("td").ToArray()[2].InnerText;
                            if (server.Contains(ServerName) && shouldInformUser)
                            {
                                int amount = int.Parse(node.Descendants("td").ToArray()[4].InnerText.Split(',')[0]);
                                if (amount > 0 && amount <= MaxValue)
                                {
                                    try
                                    {
                                        if (btn.InnerText.Contains(ButtonToClickText))
                                        {
                                            string href = btn.Descendants("a").ToArray()[0].Attributes["href"].Value;
                                            HttpGet(new Uri(SITE + href));
                                            Log("Trying to bind: " + SITE + href);
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
                                        cbSize = (uint) Marshal.SizeOf(typeof (FLASHWINFO)),
                                        hwnd = Handle,
                                        dwFlags = FlashWindowFlags.FLASHW_TRAY | FlashWindowFlags.FLASHW_TIMERNOFG
                                    };
                                    NativeMethods.FlashWindowEx(ref flashwinfo);
                                }
                            }
                            orders.Add(node.Attributes["data-id"].Value);
                        }
                    }
                }
                shouldInformUser = true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("(503)"))
                {
                    On503Error();
                }
                Log("ProcessPage error: " + ex.Message);
            }
        }

        private string HttpGet(Uri uri)
        {
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(uri);
            req.Method = "GET";
            req.UserAgent = "Mozilla/5.0 (Windows; U; MSIE 9.0; WIndows NT 9.0; en-US))";
            req.CookieContainer = cookies;
            using (WebResponse response = req.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    return string.Empty;
                }
            }
        }

        private void TimerThreadFunc()
        {
            while (_lock)
            {
                stopwatch.Restart();
                ProcessPage();
                long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                BeginInvoke((Action) (() =>
                {
                    label1.Text = string.Format("Last updated: {0:HH:mm:ss.fff}", DateTime.Now);
                    labelPerformance.Text = "Performance: " + elapsedMilliseconds + "ms";
                    labelThreads.Text = "Interval: " + _timerDefaultInterval;
                }));
                int counter = (int) (_timerDefaultInterval - stopwatch.ElapsedMilliseconds);
                if (counter > 0)
                {
                    Thread.Sleep(counter);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _lock = false;
            timerThread.Join(5000);
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }

        private void buttonGoToSite_Click(object sender, EventArgs e)
        {
            Process.Start(SITE.ToString());
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

        private void buttonLog_Click(object sender, EventArgs e)
        {
            Process.Start(logFilePath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(SITE);
        }


        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(string url, string cookieName, StringBuilder cookieData, ref int size, Int32 dwFlags, IntPtr lpReserved);

        private const Int32 InternetCookieHttponly = 0x2000;

        /// <summary>
        /// Gets the URI cookie container.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static CookieContainer GetUriCookieContainer(Uri uri)
        {
            CookieContainer cookies = null;
            // Determine the size of the cookie
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
                    return null;
            }
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }
        
    }
}
