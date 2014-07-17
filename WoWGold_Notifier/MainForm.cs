using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using HtmlAgilityPack;
using WoWGold_Notifier.WinAPI;

namespace WoWGold_Notifier
{
    public partial class MainForm : Form
    {
        private readonly List<string> orders = new List<string>();
        private readonly NotifyIcon notifyIcon;
        private readonly System.Timers.Timer timer = new System.Timers.Timer(5000);
        private readonly object _lock = new object();

        public MainForm()
        {
            InitializeComponent();
            notifyIcon = new NotifyIcon {Icon = Icon.FromHandle(Properties.Resources.coins.GetHicon()), Visible = true};
            timer.Elapsed += TimerOnElapsed;
            timer.Start();
            webBrowser1.DocumentCompleted += WebBrowserOnDocumentCompleted;
        }

        private void WebBrowserOnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            lock (_lock)
            {
                HtmlAgilityPack.HtmlDocument p = new HtmlAgilityPack.HtmlDocument { OptionDefaultStreamEncoding = Encoding.UTF8 };
                p.Load(webBrowser1.DocumentStream);
                foreach (HtmlNode node in p.DocumentNode.Descendants("tr").Where(i => i.Attributes.Contains("data-id")).Reverse())
                {
                    if (!orders.Contains(node.Attributes["data-id"].Value))
                    {
                        string server = node.Descendants("td").ToArray()[2].InnerText;
                        string amount = node.Descendants("td").ToArray()[4].InnerText;
                        if (server.Contains("Гордунни"))
                        {
                            try
                            {
                                HtmlNode btn = node.Descendants("td").ToArray()[7];
                                if (btn.InnerText.Contains("Продать"))
                                {
                                    if (webBrowser1.Document != null)
                                    {
                                        foreach (HtmlElement link in webBrowser1.Document.GetElementsByTagName("td"))
                                        {
                                            if (!string.IsNullOrWhiteSpace(link.InnerHtml) && !string.IsNullOrWhiteSpace(btn.InnerHtml))
                                            {
                                                try
                                                {
                                                    string href = btn.Descendants("a").ToArray()[0].Attributes["href"].Value;
                                                    if (link.InnerHtml.Contains(href))
                                                    {
                                                        webBrowser1.Navigate("http://supply.wowgold.ru/" + href);
                                                        using (WebClient webClient = new WebClient())
                                                        {
                                                            webClient.DownloadString("http://sms.ru/sms/send?api_id=891233b0-11a1-0ce4-dd8d-4ac68fa5b0ac&to=79052051023&text=" + (server + " - " + amount).Replace(' ', '_'));
                                                        }
                                                    }
                                                }
                                                // ReSharper disable once EmptyGeneralCatchClause
                                                catch (Exception)
                                                {
                                                    
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // ReSharper disable once EmptyGeneralCatchClause
                            catch (Exception)
                            {
                                
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
                        orders.Add(node.Attributes["data-id"].Value);
                    }
                }
            }
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (_lock)
            {
                webBrowser1.Navigate("http://supply.wowgold.ru/");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }

        private void buttonGoToSite_Click(object sender, EventArgs e)
        {
            Process.Start("http://supply.wowgold.ru/");
        }
    }
}
