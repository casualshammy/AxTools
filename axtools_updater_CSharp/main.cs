using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace axtools_updater
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Label1.Text = "Initializing...";
            pictureBox2.Image = Properties.Resources.close_4174;
            pictureBox1.Image = Properties.Resources.loader;
            pictureBox2.Click += (sender, args) => Close();
            ServicePointManager.ServerCertificateValidationCallback = (o, certificate, chain, errors) => true;
            Task.Factory.StartNew(UpdateThread);
            Label1.Text = "Continue initializing...";
        }

        private readonly string dropboxPath = "https://dl.dropboxusercontent.com/u/33646867/axtools";
        
        private void Print(string text, bool isError)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<object>(l =>
                    {
                        Label1.Text = (string) l;
                        if (isError)
                        {
                            pictureBox1.Image = Properties.Resources.loader1;
                        }
                    }), text);
                }
                else
                {
                    Label1.Text = text;
                    if (isError)
                    {
                        pictureBox1.Image = Properties.Resources.loader1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateThread()
        {
            Print("Initialized", false);
            byte num = 20;
            while (num > 0)
            {
                num -= 1;
                Thread.Sleep(1000);
                var cProcessArray = Process.GetProcessesByName("AxTools");
                if (cProcessArray.Length > 0)
                {
                    Print(string.Format("Waiting for AxTools... {0} sec", num), false);
                }
                else
                {
                    break;
                }
            }
            if (num == 0)
            {
                Print("Can't update: another instance of AxTools is running", true);
                return;
            }
            Print("Starting online update...", false);
            InternetUpdate();
            Print("Completed", false);
            Program.DeleteUnusedFiles();
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Application.StartupPath + "\\AxTools.exe",
                    WorkingDirectory = Application.StartupPath
                });
                Invoke(new Action(Close));
            }
            catch
            {
                Print("Unable to start AxTools", true);
            }
        }

        private void InternetUpdate()
        {
            Print("Checking internet connection...", false);
            try
            {
                var ping = new Ping();
                var pingReply = ping.Send("google.com", 0x1388);
                if (pingReply != null && pingReply.Status != IPStatus.Success)
                {
                    Print("Internet connection is unavailable now", true);
                    return;
                }
            }
            catch
            {
                Print("Internet connection is unavailable now", true);
                return;
            }
            Print("Fetching update info...", false);
            try
            {
                string updateString;
                using (WebClient pWebClient = new WebClient())
                {
                    updateString = pWebClient.DownloadString(dropboxPath + "/update!push");
                }
                if (String.IsNullOrWhiteSpace(updateString))
                {
                    Print("Fetching data error", true);
                    return;
                }
                Print("Processing data...", false);
                using (StringReader stringReader = new StringReader(updateString))
                {
                    while (stringReader.Peek() != -1)
                    {
                        try
                        {
                            string nextString = stringReader.ReadLine();
                            if (nextString != null)
                            {
                                string[] pair = nextString.Split(new[] {":::::"}, StringSplitOptions.None);
                                if (pair[0] == "FilesToDownload")
                                {
                                    string[] updateFilesToDownload = pair[1].Split(',');
                                    using (WebClient webClient = new WebClient())
                                    {
                                        foreach (string i in updateFilesToDownload)
                                        {
                                            File.Delete(Application.StartupPath + "\\" + i);
                                            webClient.DownloadFile(string.Format("{0}/{1}", dropboxPath, i), string.Format("{0}\\{1}", Application.StartupPath, i));
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Print("Processing data error", true);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Print("Processing data error", true);
            }
        }
        
    }
}
