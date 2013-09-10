using AxTools.Classes;
using AxTools.Classes.WinAPI;
using AxTools.Classes.WoW;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MetroFramework.Drawing;

namespace AxTools.Forms
{
    internal partial class ProcessSelection : MetroFramework.Forms.MetroForm
    {
        internal ProcessSelection()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            metroStyleManager1.Style = Settings.NewStyleColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (SolidBrush styleBrush = MetroPaint.GetStyleBrush(Style))
            {
                Rectangle rectRight = new Rectangle(Width - 1, 0, 1, Height);
                e.Graphics.FillRectangle(styleBrush, rectRight);
                Rectangle rectLeft = new Rectangle(0, 0, 1, Height);
                e.Graphics.FillRectangle(styleBrush, rectLeft);
                Rectangle rectBottom = new Rectangle(0, Height - 1, Width, 1);
                e.Graphics.FillRectangle(styleBrush, rectBottom);
            }
        }

        internal static bool SelectProcess(List<WowProcess> listProcess, out int index)
        {
            ProcessSelection bform = new ProcessSelection {wListProcess = listProcess};
            bform.ShowDialog();
            index = bform.wIndex;
            return bform.ret;
        }

        private int wIndex = -1;
        private bool ret;
        private List<WowProcess> wListProcess;

        private void ProcessSelectionLoad(object sender, EventArgs e)
        {
            foreach (var i in wListProcess)
            {
                comboBox1.Items.Add(i.IsValidBuild && i.IsInGame ?
                    string.Format("{0} - {1} (pID: {2})", i.PlayerName, i.PlayerRealm, i.ProcessID) :
                    string.Format("UNKNOWN (pID: {0})", i.ProcessID));
            }
            button1.Enabled = false;
            button2.Enabled = false;
            MainForm main = Utils.FindForm<MainForm>();
            if (main != null)
            {
                Location = new Point(main.Location.X + main.Size.Width / 2 - Size.Width / 2, main.Location.Y + main.Size.Height / 2 - Size.Height / 2);
            }
        }

        private void Button2Click(object sender, EventArgs e)
        {
            var fi = new FLASHWINFO {
                cbSize = (uint) Marshal.SizeOf(typeof (FLASHWINFO)),
                hwnd = wListProcess[comboBox1.SelectedIndex].MainWindowHandle,
                dwFlags = FlashWindowFlags.FLASHW_TRAY | FlashWindowFlags.FLASHW_TIMERNOFG
            };
            NativeMethods.FlashWindowEx(ref fi);
        }

        private void Button1Click(object sender, EventArgs e)
        {
            ret = true;
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
