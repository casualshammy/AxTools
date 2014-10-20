using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace WoWHashCalc
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
            {
                using (FileStream fileStream = File.Open(Application.StartupPath + "\\Wow.exe", FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = provider.ComputeHash(fileStream);
                    textBoxSHA256.Text = "0x" + BitConverter.ToString(hash).Replace("-", ", 0x");
                }
            }
            labelSHA256.Text = stopwatch.ElapsedMilliseconds + "ms";
            stopwatch.Restart();
            using (SHA384CryptoServiceProvider provider = new SHA384CryptoServiceProvider())
            {
                using (FileStream fileStream = File.Open(Application.StartupPath + "\\Wow.exe", FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = provider.ComputeHash(fileStream);
                    textBoxSHA384.Text = "0x" + BitConverter.ToString(hash).Replace("-", ", 0x");
                }
            }
            labelSHA384.Text = stopwatch.ElapsedMilliseconds + "ms";
            stopwatch.Restart();
            using (SHA512CryptoServiceProvider provider = new SHA512CryptoServiceProvider())
            {
                using (FileStream fileStream = File.Open(Application.StartupPath + "\\Wow.exe", FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = provider.ComputeHash(fileStream);
                    textBoxSHA512.Text = "0x" + BitConverter.ToString(hash).Replace("-", ", 0x");
                }
            }
            labelSHA512.Text = stopwatch.ElapsedMilliseconds + "ms";
            stopwatch.Stop();
        }
    }
}
