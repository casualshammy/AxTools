using System.Diagnostics;
using System.Windows.Forms;

namespace axtools_updater
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            pictureBox2.Image = Properties.Resources.close_4174;
            pictureBox2.Click += (sender, args) => Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Program.AxToolsWebsite);
        }
        
    }
}
