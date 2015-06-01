using System;
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
            using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
            {
                using (FileStream fileStream = File.Open(Application.StartupPath + "\\Wow.exe", FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = provider.ComputeHash(fileStream);
                    textBoxX86.Text = "0x" + BitConverter.ToString(hash).Replace("-", ", 0x");
                }
            }
            using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
            {
                using (FileStream fileStream = File.Open(Application.StartupPath + "\\Wow-64.exe", FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = provider.ComputeHash(fileStream);
                    textBoxX64.Text = "0x" + BitConverter.ToString(hash).Replace("-", ", 0x");
                }
            }
        }
    }
}
