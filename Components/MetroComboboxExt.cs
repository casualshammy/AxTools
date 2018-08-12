using MetroFramework.Controls;
using MetroFramework.Drawing;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Components
{
    public partial class MetroComboboxExt : MetroComboBox
    {
        public MetroComboboxExt()
        {
            InitializeComponent();
        }

        public MetroComboboxExt(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        private string overlayText;

        public string OverlayText
        {
            get => overlayText;
            set
            {
                overlayText = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!string.IsNullOrWhiteSpace(overlayText))
            {
                if (SelectedIndex == -1 && !DroppedDown)
                {
                    Font font = new Font(SystemFonts.MessageBoxFont.FontFamily, 10, FontStyle.Italic);
                    Size size = TextRenderer.MeasureText(overlayText, font);
                    Point point = new Point(2, Size.Height / 2 - size.Height / 2);
                    using (SolidBrush styleBrush = MetroPaint.GetStyleBrush(Style))
                    {
                        TextRenderer.DrawText(e.Graphics, overlayText, font, point, styleBrush.Color);
                    }
                }
            }
        }
    }
}