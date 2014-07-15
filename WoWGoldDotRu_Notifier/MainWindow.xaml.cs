using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using HtmlAgilityPack;

namespace WoWGoldDotRu_Notifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly WebBrowser webBrowser;
        private readonly List<string> orders = new List<string>();
        private readonly NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            notifyIcon = new NotifyIcon {Icon = System.Drawing.Icon.ExtractAssociatedIcon("coins.png"), Visible = true};
            DispatcherTimer timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(5000)};
            timer.Tick += timer_Elapsed;
            timer.Start();
            webBrowser = new WebBrowser();
            WindowsFormsHost.Child = webBrowser;
            webBrowser.DocumentCompleted += WebBrowserOnDocumentCompleted;
        }

        private void WebBrowserOnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs webBrowserDocumentCompletedEventArgs)
        {
            HtmlAgilityPack.HtmlDocument p = new HtmlAgilityPack.HtmlDocument {OptionDefaultStreamEncoding = Encoding.UTF8};
            p.Load(webBrowser.DocumentStream);
            foreach (HtmlNode node in p.DocumentNode.Descendants("tr").Where(i => i.Attributes.Contains("data-id")).Reverse())
            {
                if (!orders.Contains(node.Attributes["data-id"].Value))
                {
                    string server = node.Descendants("td").ToArray()[2].InnerText;
                    string amount = node.Descendants("td").ToArray()[4].InnerText;
                    if (server.Contains("Гордунни"))
                    {
                        SystemSounds.Exclamation.Play();
                        notifyIcon.ShowBalloonTip(30000, "WoWGold - New Order!", server + " - " + amount, ToolTipIcon.Warning);
                    }
                    orders.Add(node.Attributes["data-id"].Value);
                }
            }
        }

        private void timer_Elapsed(object sender, EventArgs eventArgs)
        {
            webBrowser.Navigate("http://supply.wowgold.ru/");
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }
    }
}
