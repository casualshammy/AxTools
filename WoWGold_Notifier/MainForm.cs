using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace WoWGold_Notifier
{
    public partial class MainForm : Form
    {
        private readonly List<string> orders = new List<string>();
        private readonly NotifyIcon notifyIcon;
        private readonly System.Timers.Timer timer = new System.Timers.Timer(5000);

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
                        Activate();
                        SystemSounds.Exclamation.Play();
                        notifyIcon.ShowBalloonTip(30000, "WoWGold - New Order!", server + " - " + amount, ToolTipIcon.Warning);
                    }
                    orders.Add(node.Attributes["data-id"].Value);
                }
            }
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            webBrowser1.Navigate("http://supply.wowgold.ru/");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }
    }
}
