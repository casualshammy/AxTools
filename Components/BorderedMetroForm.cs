using System;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Forms;

namespace Components
{
    public class BorderedMetroForm : MetroForm
    {
        private readonly MetroStyleManager styleManagerInst;

        public BorderedMetroForm()
        {
            ShadowType = ShadowType.None;
            styleManagerInst = new MetroStyleManager(this) {Style = MetroColorStyle.Blue, Theme = MetroThemeStyle.Light};
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (styleManagerInst != null)
            {
                styleManagerInst.Dispose();
            }
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

        public void PostInvoke(Action action)
        {
            BeginInvoke(new MethodInvoker(action));
        }
        
    }
}
