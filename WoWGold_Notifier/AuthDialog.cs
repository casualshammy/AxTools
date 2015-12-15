using System;
using System.Net;
using System.Windows.Forms;

namespace WoWGold_Notifier
{
    public partial class AuthDialog : Form
    {
        public AuthDialog(Uri site)
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
            WebRequest.DefaultWebProxy = null;
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            BeginInvoke((MethodInvoker) delegate
            {
                webBrowser1.Navigate(site);
            });
        }

    }
}
