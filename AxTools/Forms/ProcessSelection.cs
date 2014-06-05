using System.Windows.Forms;
using AxTools.Classes;
using AxTools.Classes.WinAPI;
using AxTools.Classes.WoW;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using AxTools.Components;

namespace AxTools.Forms
{
    internal partial class ProcessSelection : BorderedMetroForm
    {
        internal ProcessSelection()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            metroStyleManager1.Style = Settings.NewStyleColor;
            foreach (WowProcess i in WowProcess.GetAllWowProcesses())
            {
                comboBox1.Items.Add(i.IsValidBuild && i.IsInGame ?
                    string.Format("{0} - {1} (pID: {2})", i.PlayerName, i.PlayerRealm, i.ProcessID) :
                    string.Format("UNKNOWN (pID: {0})", i.ProcessID));
            }
            button1.Enabled = false;
            button2.Enabled = false;
            BeginInvoke((MethodInvoker) delegate
            {
                MainForm main = MainForm.Instance;
                Location = new Point(main.Location.X + main.Size.Width/2 - Size.Width/2, main.Location.Y + main.Size.Height/2 - Size.Height/2);
                OnActivated(EventArgs.Empty);
            });
        }

        internal static int SelectProcess()
        {
            ProcessSelection bform = new ProcessSelection();
            bform.ShowDialog();
            return bform.wIndex;
        }

        private int wIndex = -1;

        private void Button2Click(object sender, EventArgs e)
        {
            var fi = new FLASHWINFO {
                cbSize = (uint) Marshal.SizeOf(typeof (FLASHWINFO)),
                hwnd = WowProcess.GetAllWowProcesses()[comboBox1.SelectedIndex].MainWindowHandle,
                dwFlags = FlashWindowFlags.FLASHW_TRAY | FlashWindowFlags.FLASHW_TIMERNOFG
            };
            NativeMethods.FlashWindowEx(ref fi);
        }

        private void Button1Click(object sender, EventArgs e)
        {
            wIndex = comboBox1.SelectedIndex;
            Close();
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = comboBox1.Text != string.Empty;
            button2.Enabled = comboBox1.Text != string.Empty;
        }
    
    }
}
