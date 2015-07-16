using AxTools.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace AxTools.Components
{
    internal partial class PictureBoxExt : PictureBox
    {
        private Image highlightedImage;
        private Image refImage;

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
            Paint += PictureBoxExt_Paint;
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Paint -= PictureBoxExt_Paint;
            base.OnMouseLeave(e);
            Invalidate();
        }

        private void PictureBoxExt_Paint(object sender, PaintEventArgs e)
        {
            if ((highlightedImage == null || !ReferenceEquals(refImage, Image)) && Image != null)
            {
                InitializeHighlightedImage();
                Log.Info("Image initialized");
            }
            if (highlightedImage != null)
            {
                e.Graphics.DrawImage(highlightedImage, Point.Empty);
            }
        }

        private void InitializeHighlightedImage()
        {
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                float[][] colorMatrixElements =
                {
                    new[] {1.5f, 0f, 0f, 0f, 0f},
                    new[] {0f, 1.5f, 0f, 0f, 0f},
                    new[] {0f, 0f, 1.5f, 0f, 0f},
                    new[] {0f, 0f, 0f, 1f, 0f},
                    new[] {0f, 0f, 0f, 0f, 0f}
                };
                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                refImage = Image;
                highlightedImage = new Bitmap(Image, Size);
                using (Graphics g = Graphics.FromImage(highlightedImage))
                {
                    g.DrawImage(highlightedImage, new Rectangle(Point.Empty, highlightedImage.Size), 0, 0, highlightedImage.Width, highlightedImage.Height, GraphicsUnit.Pixel, imageAttributes);
                }
            }
        }

        ~PictureBoxExt()
        {
            highlightedImage.Dispose();
        }
    }
}
