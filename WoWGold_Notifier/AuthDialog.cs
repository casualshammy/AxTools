using System;
using System.Net;
using System.Windows.Forms;

namespace WoWGold_Notifier
{
    public partial class AuthDialog : Form
    {
        private readonly Uri website;

        public AuthDialog(Uri site)
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
            WebRequest.DefaultWebProxy = null;
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            website = site;
            BeginInvoke((MethodInvoker) delegate
            {
                webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
                webBrowser1.Navigate(website);
            });
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }

    }
}
