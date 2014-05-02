using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfRadar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                RenderOptions.SetEdgeMode(MainImage, EdgeMode.Aliased);
                writeableBitmap = BitmapFactory.New((int)MainImage.ActualWidth-10, (int)MainImage.ActualHeight-10);//new WriteableBitmap((int) MainImage.ActualWidth, (int) MainImage.ActualHeight, 96, 96, PixelFormats.Bgr32, null);
                //MainImage.Source = writeableBitmap;
                //writeableBitmap.GetBitmapContext();
                //writeableBitmap.Clear(Colors.Black);


                //MainImage.MouseMove += MainImage_MouseMove;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        readonly WriteableBitmap writeableBitmap;

        void MainImage_MouseMove(object sender, MouseEventArgs e)
        {
            writeableBitmap.Lock();
            int posX = (int) e.GetPosition(MainImage).X;
            int posY = (int) e.GetPosition(MainImage).Y;
            writeableBitmap.DrawLine(posX, posY, posX - 50, posY - 50, Colors.BlueViolet);
            writeableBitmap.Unlock();
        }

    }
}
