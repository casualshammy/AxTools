using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LibSMS
{
    public partial class PushBulletAuth : Form
    {
        private readonly Settings settings;

        internal PushBulletAuth(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;
            Shown += (sender, args) => GetAuthToken();
        }

        private void GetAuthToken()
        {
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            webBrowser1.Navigate("https://www.pushbullet.com/authorize?client_id=sRSEm6MvcH7Og4SauxfetdVzSnHTVgDu&redirect_uri=https%3A%2F%2Fwww.pushbullet.com%2Flogin-success&response_type=token");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string url = e.Url.ToString();
            if (url.Contains("https://www.pushbullet.com/login-success"))
            {
                Regex regex = new Regex("access_token=(.+)");
                Match match = regex.Match(url);
                if (match.Success)
                {
                    settings.PushbulletAPIKey = match.Groups[1].Value;
                }
                Close();
            }
        }
    }
}