using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VPNWatcher.Properties;

namespace VPNWatcher
{
    public partial class NainForm : NainForm
    {
        private readonly NotifyIcon notifyIcon;

        public NainForm()
        {
            InitializeComponent();
            Icon = Resources.vpn;
            notifyIcon = new NotifyIcon {Icon = Resources.vpn, Visible = true, Text = "VPNWatcher"};
        }
    }
}
