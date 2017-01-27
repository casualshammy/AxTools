using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using Components.WinAPI;
using MetroFramework;

namespace Components.Forms
{
    public partial class PopupNotification : BorderedMetroForm
    {
        private readonly System.Timers.Timer timer;
        private int startPosX;
        private int startPosY;
        private DateTime loadTime;
        private IntPtr prevForegroundWindow;

        public PopupNotification(string title, string message, Image image, MetroColorStyle metroColorStyle)
        {
            InitializeComponent();
            StyleManager.Style = metroColorStyle;
            TopMost = true;
            Title = title;
            Message = message;
            Icon = image;
            timer = new System.Timers.Timer(50);
            timer.Elapsed += timer_Tick;
            BeginInvoke((MethodInvoker) delegate
            {
                SetLocation();
                loadTime = DateTime.UtcNow;
                timer.Start();
                Timeout = Timeout == 0 ? 30 : Timeout;
                MouseClick += ALL_MouseClick;
                foreach (Control control in Controls)
                {
                    control.MouseEnter += ALL_MouseEnter;
                    control.MouseClick += ALL_MouseClick;
                }
            });
        }

        public string Title
        {
            get { return metroLabel1.Text; }
            set { metroLabel1.Text = value; }
        }

        public string Message
        {
            get { return metroLabel2.Text; }
            set
            {
                metroLabel2.Text = WordWrap(value);
                Size = new Size(Width, Math.Max(68, metroLabel2.Location.Y + metroLabel2.Size.Height + 10)); // 68 - standart heigth
            }
        }

        public new Image Icon
        {
            get { return pictureBox1.Image; }
            set { pictureBox1.Image = value; }
        }

        public int Timeout { get; private set; }

        public void Show(int timeout)
        {
            prevForegroundWindow = NativeMethods.GetForegroundWindow();
            Timeout = timeout;
            Shown += (sender, args) => NativeMethods.SetForegroundWindow(prevForegroundWindow);
            Show();
        }

        public new event EventHandler Click
        {
            add
            {
                base.Click += value;
                foreach (Control control in Controls)
                {
                    control.Click += value;
                }
            }
            remove
            {
                base.Click -= value;
                foreach (Control control in Controls)
                {
                    control.Click -= value;
                }
            }
        }

        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            if (Opacity > 0.01f)
            {
                if (DateTime.UtcNow - loadTime > TimeSpan.FromSeconds(Timeout))
                {
                    PostInvoke(() => { Opacity = Opacity - 0.01f; });
                }
            }
            else
            {
                PostInvoke(Close);
            }
        }

        private void ALL_MouseEnter(object sender, EventArgs e)
        {
            loadTime = DateTime.UtcNow;
            Opacity = 1.0d;
        }

        private void ALL_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Close();
            }
        }

        private void SetLocation()
        {
            IntPtr taskbarHandle = NativeMethods.FindWindow("Shell_TrayWnd", null);
            APPBARDATA data = new APPBARDATA
            {
                cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA)),
                hWnd = taskbarHandle
            };
            IntPtr result = NativeMethods.SHAppBarMessage((uint)APPBARMESSAGE.GetTaskbarPos, ref data);
            if (result == IntPtr.Zero)
            {
                throw new InvalidOperationException();
            }
            List<PopupNotification> popups = FindForms<PopupNotification>().ToList();
            popups.Remove(this);
            if (data.uEdge == ABE.Top)
            {
                startPosX = Screen.PrimaryScreen.WorkingArea.Width - Width;
                startPosY = 0;
                if (popups.Any())
                {
                    List<int> yTopLeft = popups.Select(l => l.DesktopLocation.Y).Concat(new[] { Screen.PrimaryScreen.WorkingArea.Height }).ToList();
                    yTopLeft.Sort();
                    List<int> yBottomLeft = new[] { -10 }.Concat(popups.Select(l => l.DesktopLocation.Y + l.Height)).ToList();
                    yBottomLeft.Sort();
                    for (int i = 0; i < yTopLeft.Count; i++)
                    {
                        int availHeight = yTopLeft[i] - yBottomLeft[i];
                        if (availHeight >= Height + 10 + 10)
                        {
                            startPosY = yBottomLeft[i] + 10;
                            break;
                        }
                    }
                }
            }
            else
            {
                startPosX = Screen.PrimaryScreen.WorkingArea.Width - Width;
                startPosY = Screen.PrimaryScreen.WorkingArea.Height - Height;
                if (popups.Any())
                {
                    List<int> yTopLeft = new[] { Screen.PrimaryScreen.WorkingArea.Height + 10 }.Concat(popups.Select(l => l.DesktopLocation.Y)).ToList();
                    yTopLeft.Sort();
                    yTopLeft.Reverse();
                    List<int> yBottomLeft = popups.Select(l => l.DesktopLocation.Y + l.Height).Concat(new[] { 0 }).ToList();
                    yBottomLeft.Sort();
                    yBottomLeft.Reverse();
                    for (int i = 0; i < yTopLeft.Count; i++)
                    {
                        int availHeight = yTopLeft[i] - yBottomLeft[i];
                        if (availHeight >= Height + 10 + 10)
                        {
                            startPosY = yTopLeft[i] - 10 - Height;
                            break;
                        }
                    }
                }
            }
            SetDesktopLocation(startPosX, startPosY);
        }

        private string WordWrap(string text)
        {
            using (Font font = MetroFonts.Label(metroLabel2.FontSize, metroLabel2.FontWeight))
            {
                List<string> words = text.Split(new[] {" ", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();
                string result = "";
                int sizeOfSpace = TextRenderer.MeasureText(" ", font).Width;
                while (words.Any())
                {
                    string buffer = "";
                    int sizePixels = 0;
                    while (words.Any() && sizePixels + sizeOfSpace + TextRenderer.MeasureText(words.First(), font).Width <= 300) // 300 - max length of <metroLabel2>
                    {
                        buffer += " " + words.First();
                        sizePixels += sizeOfSpace + TextRenderer.MeasureText(words.First(), font).Width;
                        words.RemoveAt(0);
                    }
                    result += buffer + "\r\n";
                }
                return result.TrimEnd('\n').TrimEnd('\r');
            }
        }

        private T[] FindForms<T>() where T : Form
        {
            return (from object i in Application.OpenForms where i.GetType() == typeof(T) select i as T).ToArray();
        }

    }
}
