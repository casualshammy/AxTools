using Components.WinAPI;
using MetroFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace Components.Forms
{
    public partial class PopupNotification : BorderedMetroForm
    {
        private readonly System.Timers.Timer timer;
        private static readonly System.Timers.Timer arrangementTimer = new System.Timers.Timer(250);
        private DateTime loadTime;
        private const float FadeOutStep = 1f / 5000f * 33.3f;

        static PopupNotification()
        {
            arrangementTimer.Elapsed += ArrangementTimer_Elapsed;
            arrangementTimer.Start();
        }

        public PopupNotification(string title, string message, Image image, MetroColorStyle metroColorStyle)
        {
            InitializeComponent();
            StyleManager.Style = metroColorStyle;
            Title = title;
            Message = message;
            Icon = image;
            timer = new System.Timers.Timer(33.3); // 30fps
            timer.Elapsed += Timer_Tick;
            FormClosing += PopupNotification_FormClosing;
            BeginInvoke((MethodInvoker)delegate
            {
                ArrangementTimer_Elapsed(null, null);
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
            get => metroLabel1.Text;
            set => metroLabel1.Text = value;
        }

        public string Message
        {
            get => metroLabel2.Text;
            set
            {
                metroLabel2.Text = WordWrap(value);
                Size = new Size(Width, Math.Max(68, metroLabel2.Location.Y + metroLabel2.Size.Height + 10)); // 68 - standard height
            }
        }

        public new Image Icon
        {
            get => pictureBox1.Image;
            set => pictureBox1.Image = value;
        }

        public int Timeout { get; private set; }

        public bool IsClosed { get; private set; }

        public void Show(int timeout)
        {
            var prevForegroundWindow = NativeMethods.GetForegroundWindow();
            TopMost = true;
            Timeout = timeout;
            base.Show();
            NativeMethods.SetForegroundWindow(prevForegroundWindow);
        }

        public new void Show()
        {
            Show(7);
        }

        public void RefreshTimeout()
        {
            loadTime = DateTime.UtcNow;
            Opacity = 1.0d;
        }

        public new event MouseEventHandler Click
        {
            add
            {
                base.MouseClick += value;
                foreach (Control control in Controls)
                {
                    control.MouseClick += value;
                }
            }
            remove
            {
                base.MouseClick -= value;
                foreach (Control control in Controls)
                {
                    control.MouseClick -= value;
                }
            }
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            if (Opacity > FadeOutStep)
            {
                if (DateTime.UtcNow - loadTime > TimeSpan.FromSeconds(Timeout))
                {
                    PostInvoke(() => { Opacity = Opacity - FadeOutStep; });
                }
            }
            else
            {
                PostInvoke(Close);
            }
        }

        private static void ArrangementTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var appBarData = GetAppBarData();
            var popups = FindForms<PopupNotification>().ToList();
            popups.Sort((first, second) =>
            {
                return appBarData.uEdge == ABE.Top ? first.DesktopLocation.Y.CompareTo(second.DesktopLocation.Y) : -first.DesktopLocation.Y.CompareTo(second.DesktopLocation.Y);
            });
            if (appBarData.uEdge == ABE.Top)
            {
                int x;
                var y = 0;
                foreach (PopupNotification popup in popups)
                {
                    x = Screen.PrimaryScreen.WorkingArea.Width - popup.Width;
                    popup.PostInvoke(() => popup.SetDesktopLocation(x, y));
                    y += popup.Height + 10;
                }
            }
            else
            {
                int x;
                var y = Screen.PrimaryScreen.WorkingArea.Height - (popups.FirstOrDefault() != null ? popups.FirstOrDefault().Height : 0);
                foreach (PopupNotification popup in popups)
                {
                    x = Screen.PrimaryScreen.WorkingArea.Width - popup.Width;
                    popup.PostInvoke(() => popup.SetDesktopLocation(x, y));
                    y -= popup.Height - 10;
                }
            }
        }

        private void ALL_MouseEnter(object sender, EventArgs e)
        {
            RefreshTimeout();
        }

        private void ALL_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Close();
                ArrangementTimer_Elapsed(null, null);
            }
        }

        private void PopupNotification_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsClosed = true;
        }

        private string WordWrap(string text)
        {
            using (Font font = MetroFonts.Label(metroLabel2.FontSize, metroLabel2.FontWeight))
            {
                var words = text.Split(new[] { " ", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var result = new StringBuilder("");
                var sizeOfSpace = TextRenderer.MeasureText(" ", font).Width;
                while (words.Any())
                {
                    result.Append(" " + words.First());
                    var sizePixels = sizeOfSpace + TextRenderer.MeasureText(words.First(), font).Width;
                    words.RemoveAt(0);
                    while (words.Any() && sizePixels + sizeOfSpace + TextRenderer.MeasureText(words.First(), font).Width <= 300*1.4) // 300 - max length of <metroLabel2>, plus fix
                    {
                        result.Append(" " + words.First());
                        sizePixels += sizeOfSpace + TextRenderer.MeasureText(words.First(), font).Width;
                        words.RemoveAt(0);
                    }
                    result.Append("\r\n");
                }
                return result.ToString().TrimEnd('\n').TrimEnd('\r');
            }
        }

        private static IEnumerable<T> FindForms<T>() where T : Form
        {
            return Application.OpenForms.OfType<T>();
        }

        private static APPBARDATA GetAppBarData()
        {
            var taskbarHandle = NativeMethods.FindWindow("Shell_TrayWnd", null);
            var data = new APPBARDATA
            {
                cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA)),
                hWnd = taskbarHandle
            };
            var result = NativeMethods.SHAppBarMessage((uint)APPBARMESSAGE.GetTaskbarPos, ref data);
            if (result == IntPtr.Zero)
            {
                throw new InvalidOperationException();
            }
            return data;
        }
    }
}