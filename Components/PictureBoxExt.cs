using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Components
{
    public partial class PictureBoxExt : PictureBox
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

        protected override void OnMouseEnter(EventArgs e)
        {
            Location = new Point(Location.X - 1, Location.Y - 1);
            Size = new Size(Size.Width + 2, Size.Height + 2);
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Location = new Point(Location.X + 1, Location.Y + 1);
            Size = new Size(Size.Width - 2, Size.Height - 2);
            base.OnMouseLeave(e);
            Invalidate();
        }
        
    }
}