using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using AxTools.Components;
using AxTools.Helpers;
using AxTools.Properties;
using Settings = AxTools.Helpers.Settings;

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
            metroStyleManager1.Style = Settings.Instance.StyleColor;
        }

        /// <summary>
        ///     Remark: should be run from UI thread
        /// </summary>
        internal static void ShowChangesIfNeeded()
        {
            if (Globals.AppVersion.Major != Settings.Instance.LastUsedVersion.Major || Globals.AppVersion.Minor != Settings.Instance.LastUsedVersion.Minor)
            {
                Task.Factory.StartNew(() =>
                {
                    AppSpecUtils.CheckCreateDir();
                    using (WebClient pWebClient = new WebClient())
                    {
                        pWebClient.DownloadFile(Globals.UpdateServerPath + "/changes.jpg", Globals.TempPath + "\\changes.jpg");
                    }
                }).ContinueWith(l =>
                {
                    if (l.Exception == null)
                    {
                        new Changes(Globals.TempPath + "\\changes.jpg").ShowDialog();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            Settings.Instance.LastUsedVersion = Globals.AppVersion;
        }

    }
}
