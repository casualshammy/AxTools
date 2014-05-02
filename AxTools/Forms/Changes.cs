using System.Drawing;
using AxTools.Components;
using AxTools.Properties;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class Changes : BorderedMetroForm
    {
        internal Changes(string imagePath)
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            using (Image objImage = Image.FromFile(imagePath))
            {
                Width = objImage.Width + 40;
                Height = objImage.Height + 50;
                pictureBox1.Size = objImage.Size;
                pictureBox1.ImageLocation = imagePath;
            }
            metroStyleManager1.Style = Settings.NewStyleColor;
        }
        
    }
}
