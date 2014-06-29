using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using OpenPop.Pop3;

namespace IEmailNotifier
{
    public partial class Form1 : Form
    {
        readonly System.Timers.Timer t = new System.Timers.Timer(60000);

        public Form1()
        {
            InitializeComponent();
            t.Elapsed += Timer_OnElapsed;
            Timer_OnElapsed(null, null);
            t.Start();
        }

        private void Timer_OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                using (Pop3Client pop3Client = new Pop3Client())
                {
                    pop3Client.Connect("pop.zoho.com", 995, true);
                    pop3Client.Authenticate("axio@axio.name", "siemens2006zz", AuthenticationMethod.UsernameAndPassword);
                    List<string> uids = pop3Client.GetMessageUids();
                    foreach (string i in uids)
                    {
                        MessageBox.Show(i);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
