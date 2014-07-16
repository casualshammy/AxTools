using MetroFramework.Drawing;
using MetroFramework.Forms;
using System.Drawing;
using System.Windows.Forms;

namespace AxTools.Components
{
    internal class BorderedMetroForm : MetroForm
    {
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

        //protected override void OnResize(EventArgs e)
        //{
        //    base.OnResize(e);
        //    base.OnResizeBegin(e);
        //    base.OnResizeEnd(e);
        //    //if (!AppDomain.CurrentDomain.FriendlyName.Contains("DefaultDomain"))
        //    //{
        //    //    BeginInvoke((MethodInvoker)(() => OnActivated(EventArgs.Empty)));
        //    //    //MessageBox.Show("OnResize");
        //    //}
        //}
    }
}
