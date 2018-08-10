using MetroFramework.Controls;
using MetroFramework.Drawing;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Components
{
    internal partial class MetroToggleExt : MetroToggle
    {
        internal MetroToggleExt()
        {
            InitializeComponent();
        }

        internal MetroToggleExt(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private int size = 80;

        public string ExtraText { get; set; }

        public int SizeExt
        {
            get { return size; }
            set { size = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            SizeF textSize = e.Graphics.MeasureString(ExtraText, DefaultFont);
            int textLeftBound = ClientRectangle.Width / 2 - (int)textSize.Width / 2;
            Rectangle bounds = new Rectangle(DisplayStatus ? 30 + textLeftBound : textLeftBound, 2, (int)textSize.Width + 4, ClientRectangle.Height - 4);
            TextRenderer.DrawText(e.Graphics, ExtraText, DefaultFont, bounds, Color.White,
                                  Checked ? MetroPaint.GetStyleColor(Style) : MetroPaint.BorderColor.CheckBox.Normal(Theme),
                                  MetroPaint.GetTextFormatFlags(TextAlign));
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = base.GetPreferredSize(proposedSize);
            preferredSize.Width = size;
            return preferredSize;
        }
    }
}