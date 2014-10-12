using System.Drawing;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Interfaces;

namespace AxTools.Components
{
    public partial class Line : UserControl, IMetroControl
    {
        public MetroColorStyle Style { get; set; }
        public MetroThemeStyle Theme { get; set; }
        public MetroStyleManager StyleManager { get; set; }
        private readonly Pen pen = new Pen(Color.Black, 2f);
        
        public Line()
        {
            InitializeComponent();
            Paint += LineSeparator_Paint;
            base.MaximumSize = new Size(2000, 2);
            base.MinimumSize = new Size(0, 2);
            Width = 350;
        }

        private void LineSeparator_Paint(object sender, PaintEventArgs e)
        {
            pen.Color = MetroPaint.GetStyleColor(Style);
            e.Graphics.DrawLine(pen, new Point(-1, 0), new Point(Width, 1));
        }

    }
}
