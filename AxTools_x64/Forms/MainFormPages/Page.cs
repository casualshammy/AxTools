using System.Drawing;
using System.Windows.Forms;

namespace AxTools.Forms.MainFormPages
{
    internal class Page : UserControl
    {
        internal Page()
        {
            size = new Size(425, 140);
            SetBounds(Location.X, Location.Y, Width, Height, BoundsSpecified.Size);
        }

        internal new int Height
        {
            get
            {
                return Size.Height;
            }
        }

        internal new int Width
        {
            get
            {
                return Size.Width;
            }
        }

        private readonly Size size;
        public new Size Size
        {
            get
            {
                return size;
            }
        }
    }
}
