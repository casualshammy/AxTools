using System;
using System.Drawing;

namespace Components.Forms
{
    internal partial class CustomScriptEdit : BorderedMetroForm
    {
        private CustomScriptEdit()
        {
            InitializeComponent();
            closeButton.Location = new Point(Size.Width/2 - closeButton.Size.Width/2, closeButton.Location.Y);
        }

        internal static string OpenDialog(string values)
        {
            CustomScriptEdit cse = new CustomScriptEdit {customScriptTextBox = {Text = values}};
            cse.ShowDialog();
            return cse.customScriptTextBox.Text.Trim(' ', '\r', '\n');
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
