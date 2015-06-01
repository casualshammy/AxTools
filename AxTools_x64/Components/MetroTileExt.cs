using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Drawing;

namespace AxTools.Components
{
    public partial class MetroTileExt : MetroTile
    {
        public MetroTileExt()
        {
            InitializeComponent();
        }

        public MetroTileExt(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        private string centerText;

        public string CenterText
        {
            get
            {
                return centerText;
            }
            set
            {
                centerText = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!string.IsNullOrEmpty(centerText))
            {
                Font font = MetroFonts.Link(MetroLinkSize.Small, MetroLinkWeight.Bold);
                Size size = TextRenderer.MeasureText(centerText, font);
                Point point = new Point(Width/2 - size.Width/2, Height/2 - size.Height/2);
                TextRenderer.DrawText(e.Graphics, centerText, font, point, MetroPaint.ForeColor.Tile.Normal(Theme));
            }
        }
    }
}
