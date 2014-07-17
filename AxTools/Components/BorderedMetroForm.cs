using MetroFramework.Drawing;
using MetroFramework.Forms;
using System.Drawing;
using System.Windows.Forms;

namespace AxTools.Components
{
    internal class BorderedMetroForm : MetroForm
    {
        internal BorderedMetroForm()
        {
            ShadowType = ShadowType.None;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (SolidBrush styleBrush = MetroPaint.GetStyleBrush(Style))
            {
                e.Graphics.FillRectangles(styleBrush, new[]
                {
                    new Rectangle(Width - 2, 0, 2, Height), // right
                    new Rectangle(0, 0, 2, Height),         // left
                    new Rectangle(0, Height - 2, Width, 2)  // bottom
                });
            }
        }

    }
}
