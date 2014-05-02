using System;
using System.Windows.Forms;
using AxTools.Classes;
using AxTools.Components;

namespace AxTools.Forms
{
    internal partial class InputBox : BorderedMetroForm
    {
        internal InputBox()
        {
            InitializeComponent();
            button1.Click += Button1Click;
            button2.Click += Button2Click;
            metroStyleManager1.Style = Settings.NewStyleColor;
            BeginInvoke((MethodInvoker) delegate
            {
                Location = MousePosition;
                OnActivated(EventArgs.Empty);
            });
        }

        string temp;

        internal static string Input(string prompt)
        {
            InputBox bform = new InputBox {metroLabel1 = {Text = prompt}};
            bform.ShowDialog();
            return bform.temp;
        }
 
        private void Button1Click(object sender, EventArgs e)
        {
            temp = textBox1.Text;
            Close();
        }
 
        private void Button2Click(object sender, EventArgs e)
        {
            Close();
        }

        private void InputBoxLoad(object sender, EventArgs e)
        {
            
        }
    }
}
