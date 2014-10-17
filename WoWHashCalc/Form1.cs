using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace WoWHashCalc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
            {
                using (FileStream fileStream = File.Open(Application.StartupPath + "\\Wow.exe", FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = provider.ComputeHash(fileStream);
                    textBox1.Text = "0x" + BitConverter.ToString(hash).Replace("-", ", 0x");
                }
            }
            label1.Text = stopwatch.ElapsedMilliseconds + "ms";
        }
    }
}
