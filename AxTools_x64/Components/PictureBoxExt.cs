using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AxTools.Components
{
    internal partial class PictureBoxExt : PictureBox
    {
        public PictureBoxExt()
        {
            InitializeComponent();
        }

        public PictureBoxExt(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        public Image ImageOnHover { get; set; }

        private Image previousImage;

        protected override void OnMouseEnter(EventArgs e)
        {
            if (ImageOnHover != null && Enabled)
            {
                previousImage = Image;
                Image = ImageOnHover;
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (previousImage != null && Enabled)
            {
                Image = previousImage;
            }
            base.OnMouseLeave(e);
        }
    }
}
