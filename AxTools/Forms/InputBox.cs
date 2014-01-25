using System;
using System.Drawing;
using System.Windows.Forms;
using AxTools.Classes;
using AxTools.Components;
using MetroFramework.Drawing;

namespace AxTools.Forms
{
    internal partial class InputBox : BorderedMetroForm
    {
        public InputBox()
        {
            InitializeComponent();
            button1.Click += Button1Click;
            button2.Click += Button2Click;
            metroStyleManager1.Style = Settings.NewStyleColor;
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
