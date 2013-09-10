using AxTools.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;
using AxTools.Classes.WinAPI;

namespace AxTools.Forms
{
    internal partial class PingWidget : Form
    {
        public PingWidget()
        {
            InitializeComponent();
            AccessibleName = "PingWidget";
            Location = Settings.WidgetLocation;
            Load += PingWidgetLoad;
            MouseDown += PictureBox1MouseDown;
            MouseMove += PictureBox1MouseMove;
            MouseUp += PictureBox1MouseUp;
            FormClosing += PingWidgetFormClosing;
            MainForm.PingStatisticsChanged += Tick;
        }

        Point tmpPoint = Point.Empty;
        Point oldPoint = Point.Empty;
        bool isDragging;

        private void PingWidgetLoad(Object sender, EventArgs e)
        {
            if (Settings.PingWidgetTransparent)
            {
                int cStype = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
                cStype = cStype ^ NativeMethods.WS_EX_LAYERED ^ NativeMethods.WS_EX_TRANSPARENT;
                NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, cStype);
                NativeMethods.SetLayeredWindowAttributes(Handle, 0, 180, NativeMethods.LWA_ALPHA);
            }
            else
            {
                Opacity = 0.7;
            }
            Size = new Size(111, 37);
        }

        private void PictureBox1MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    isDragging = true;
                    oldPoint = e.Location;
                    break;
                case MouseButtons.Right:
                    Close();
                    break;
            }
        }

        private void PictureBox1MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;
            tmpPoint.X += e.X - oldPoint.X;
            tmpPoint.Y += e.Y - oldPoint.Y;
            Location = tmpPoint;
        }

        private void PictureBox1MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            Settings.WidgetLocation = Location;
        }

        private void Tick(int ping, int packetLoss)
        {
            if (ping >= 0 & ping <= 99)
            {
                Label1.ForeColor = Color.GreenYellow;
            }
            else if (ping >= 100 & ping < 500)
            {
                Label1.ForeColor = Color.Violet;
            }
            else
            {
                Label1.ForeColor = Color.Red;
            }
            Label1.Text = "Game ping: " + (ping == -1 ? "n/a" : ping.ToString());
            if (packetLoss < 5)
            {
                Label101.ForeColor = Color.GreenYellow;
            }
            else if (packetLoss < 10)
            {
                Label101.ForeColor = Color.Violet;
            }
            else
            {
                Label101.ForeColor = Color.Red;
            }
            Label101.Text = string.Concat("Packet loss: ", packetLoss, "%");
        }

        private void PingWidgetFormClosing(Object sender, FormClosingEventArgs e)
        {
            MainForm.PingStatisticsChanged -= Tick;
        }
    }
}
