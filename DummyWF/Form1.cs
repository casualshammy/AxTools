using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DummyWF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;
            Bitmap bitmap = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int argb = random.Next(int.MinValue + 1, int.MaxValue - 1);
                    bitmap.SetPixel(x, y, argb >= 0 ? Color.Black : Color.White);
                }
            }
            pictureBox1.Image = bitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (CSPRNG csprng = new CSPRNG())
            {
                int width = pictureBox1.Width;
                int height = pictureBox1.Height;
                Bitmap bitmap = new Bitmap(width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int argb = csprng.Next();
                        bitmap.SetPixel(x, y, argb >= 0 ? Color.Black : Color.White);
                    }
                }
                pictureBox1.Image = bitmap;
            }
        }
    }
}
