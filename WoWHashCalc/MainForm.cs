using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoWHashCalc
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
            DoWork();
        }

        private async void DoWork()
        {
            textBoxX86.Text = await CalcucateHashAsync(Application.StartupPath + "\\Wow.exe");
            textBoxX64.Text = await CalcucateHashAsync(Application.StartupPath + "\\Wow-64.exe");
        }

        private async Task<string> CalcucateHashAsync(string path)
        {
            Task<string> task = Task.Run(() =>
            {
                using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
                {
                    using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        byte[] hash = provider.ComputeHash(fileStream);
                        return "0x" + BitConverter.ToString(hash).Replace("-", ", 0x");
                    }
                }
            });
            await task;
            return task.Result;
        }
    }
}
