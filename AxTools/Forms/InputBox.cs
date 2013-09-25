using System;
using System.Drawing;
using System.Windows.Forms;
using AxTools.Classes;
using MetroFramework.Drawing;

namespace AxTools.Forms
{
    internal partial class InputBox : MetroFramework.Forms.MetroForm
    {
        public InputBox()
        {
            InitializeComponent();
            button1.Click += Button1Click;
            button2.Click += Button2Click;
            metroStyleManager1.Style = Settings.NewStyleColor;
        }

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

        bool t;
        String temp = string.Empty;
 
        public static bool Input(string blabel, out string s)
        {
            InputBox bform = new InputBox {metroLabel1 = {Text = blabel}};
            bform.ShowDialog();
            s = bform.temp;
            return bform.t;
        }
 
        private void Button1Click(object sender, EventArgs e)
        {
            temp = textBox1.Text;
            t = true;
            Close();
        }
 
        private void Button2Click(object sender, EventArgs e)
        {
            t = false;
            Close();
        }

        private void InputBoxLoad(object sender, EventArgs e)
        {
            Location = MousePosition;
        }
    }
}
