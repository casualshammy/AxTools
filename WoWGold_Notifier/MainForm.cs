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
using WoWGold_Notifier.TaskbarProgressbar;
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
        private bool initialLoadingFinished;
        private readonly string logFilePath = Application.StartupPath + "\\log.txt";
        private readonly Stopwatch stopwatch;
        private CookieContainer cookies;
        private const int TIMER_INTERVAL = 100;
        private static readonly Uri SITE = new Uri("http://supply.elfmoney.ru");
        internal static readonly NetworkCredential Credentials = new NetworkCredential("axio@axio.name", "axio-739");
        private readonly List<string> knownBidButtons = new List<string>();
        private const string ButtonTextIsPerforming = "Выдать";
        private const string ButtonTextToClickText = "Выполнить";
        private const string TEXT_PERFORMING = "Выполняется";
        private const string TEXT_NO_ORDERS_AVAILABLE = "К сожалению, в данный момент нет заказов";
        private readonly string[] serverNames =
        {
            "Черный Шрам - Альянс"
        };
        private const int MaxValue = 40000;

        public MainForm()
        {
            InitializeComponent();
            stopwatch = new Stopwatch();
            WebRequest.DefaultWebProxy = null;
            cookies = GetUriCookieContainer(SITE);
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            notifyIcon = new NotifyIcon {Icon = Icon.FromHandle(Resources.coins.GetHicon()), Visible = true, Text = "WoWGold Notifier"};
            timerThread = new Thread(TimerThreadFunc);
            timerThread.Start();
        }

        private string HttpGetBruteForce503(Uri uri)
        {
            int counter = 0;
            while (counter < 20)
            {
                try
                {
                    return HttpGet(uri);
                }
                catch
                {
                    Thread.Sleep(100);
                    counter++;
                }
            }
            return null;
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
                if (checkBoxTimerEnabled.Checked)
                {
                    OnElapsed();
                    long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    BeginInvoke((Action) (() =>
                    {
                        label1.Text = string.Format("Last updated: {0:HH:mm:ss.fff}", DateTime.Now);
                        labelPerformance.Text = "Performance: " + elapsedMilliseconds + "ms";
                    }));
                }
                int counter = (int)(TIMER_INTERVAL - stopwatch.ElapsedMilliseconds);
                if (counter > 0)
                {
                    Thread.Sleep(counter);
                }
            }
        }

        private void OnElapsed()
        {
            HashSet<string> bidButtons = GetNewBidButtons();
            if (bidButtons.Count > 0)
            {
                foreach (string bidButton in bidButtons)
                {
                    ClickBidButton(bidButton);
                }
                if (IsNewBiddingAvailable())
                {
                    SendSMS("WOWGOLD: ORDER BID");
                    SystemSounds.Exclamation.Play();
                    notifyIcon.ShowBalloonTip(30000, "WoWGold - New Order!", "Bidded!", ToolTipIcon.Warning);
                    FLASHWINFO flashwinfo = new FLASHWINFO
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(FLASHWINFO)),
                        hwnd = Handle,
                        dwFlags = FlashWindowFlags.FLASHW_TRAY | FlashWindowFlags.FLASHW_TIMERNOFG
                    };
                    NativeMethods.FlashWindowEx(ref flashwinfo);
                }
            }
            initialLoadingFinished = true;
        }

        private HtmlDocument GetHtmlDocument()
        {
            string source = HttpGetBruteForce503(SITE);
            if (source != null)
            {
                if (source.Contains(ButtonTextToClickText) || source.Contains(TEXT_PERFORMING))
                {
                    labelResponse.Text = "Response: 200 OK";
                    TBProgressBar.SetProgressValue(Handle, 100, 100);
                    TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.NoProgress);
                }
                else if (source.Contains(TEXT_NO_ORDERS_AVAILABLE))
                {
                    labelResponse.Text = "Response: No orders";
                    TBProgressBar.SetProgressValue(Handle, 100, 100);
                    TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.Paused);
                }
                else
                {
                    labelResponse.Text = "Response: Auth error?";
                    TBProgressBar.SetProgressValue(Handle, 100, 100);
                    TBProgressBar.SetProgressState(Handle, ThumbnailProgressState.Error);
                }
                HtmlDocument p = new HtmlDocument {OptionDefaultStreamEncoding = Encoding.UTF8};
                p.LoadHtml(source);
                return p;
            }
            return null;
        }

        private HashSet<string> GetNewBidButtons()
        {
            HtmlDocument htmlDocument = GetHtmlDocument();
            HashSet<string> set = new HashSet<string>();
            if (htmlDocument != null)
            {
                foreach (HtmlNode node in htmlDocument.DocumentNode.Descendants("tr").Where(i => i.Attributes.Contains("data-id")).Reverse())
                {
                    if (!orders.Contains(node.Attributes["data-id"].Value))
                    {
                        HtmlNode btn = node.Descendants("td").ToArray()[7];
                        if (btn.InnerText.Contains(ButtonTextToClickText))
                        {
                            string server = node.Descendants("td").ToArray()[2].InnerText;
                            if (initialLoadingFinished && serverNames.Any(l => server.Contains(l)))
                            {
                                int amount = int.Parse(node.Descendants("td").ToArray()[4].InnerText.Split(',')[0]);
                                if (amount > 0 && amount <= MaxValue)
                                {
                                    if (btn.InnerText.Contains(ButtonTextToClickText))
                                    {
                                        string href = btn.Descendants("a").ToArray()[0].Attributes["href"].Value;
                                        set.Add(href);
                                    }
                                }
                            }
                            orders.Add(node.Attributes["data-id"].Value);
                        }
                    }
                }
            }
            return set;
        }

        private void ClickBidButton(string url)
        {
            int counter = 0;
            while (counter < 20)
            {
                try
                {
                    HttpGet(new Uri(SITE + url));
                    Log("Trying to bind: " + SITE + url);
                    return;
                }
                catch
                {
                    Thread.Sleep(100);
                    counter++;
                }
            }
        }

        private bool IsNewBiddingAvailable()
        {
            HtmlDocument htmlDocument = GetHtmlDocument();
            if (htmlDocument != null)
            {
                bool found = false;
                foreach (HtmlNode node in htmlDocument.DocumentNode.Descendants("tr").Where(i => i.Attributes.Contains("class") && i.Attributes["class"].Value == "active").Reverse())
                {
                    HtmlNode nodeWithID = node.Descendants("td").First().Descendants("a").FirstOrDefault(l => l.Attributes.Contains("class") && l.Attributes["class"].Value == "order-info");
                    if (nodeWithID != null)
                    {
                        string dataID = nodeWithID.Attributes["data-id"].Value;
                        if (!knownBidButtons.Contains(dataID))
                        {
                            HtmlNode btn = node.Descendants("td").ToArray()[7];
                            if (btn.InnerText.Contains(ButtonTextIsPerforming))
                            {
                                knownBidButtons.Add(dataID);
                                found = true;
                            }
                        }
                    }
                }
                return found;
            }
            return false;
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

        private void button1_Click(object sender, EventArgs e)
        {
            AuthDialog authDialog = new AuthDialog(SITE);
            authDialog.ShowDialog(this);
            cookies = GetUriCookieContainer(SITE);
        }


        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(string url, string cookieName, StringBuilder cookieData, ref int size, int dwFlags, IntPtr lpReserved);

        private const int InternetCookieHttponly = 0x2000;

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
