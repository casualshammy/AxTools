﻿using System.Drawing;
using System.IO;
using AxTools.Helpers;
using AxTools.Properties;
using Components.Forms;
using Settings2 = AxTools.Helpers.Settings2;

namespace AxTools.Forms
{
    internal partial class Changes : BorderedMetroForm
    {
        internal Changes(string imagePath)
        {
            InitializeComponent();
            StyleManager.Style = Settings2.Instance.StyleColor;
            Icon = Resources.AppIcon;
            using (Image objImage = Image.FromFile(imagePath))
            {
                Width = objImage.Width + 40;
                Height = objImage.Height + 50;
                pictureBox1.Size = objImage.Size;
                pictureBox1.ImageLocation = imagePath;
            }
        }

        /// <summary>
        ///     Remark: should be run from UI thread
        /// </summary>
        internal static void ShowChangesIfNeeded()
        {
            if (Globals.AppVersion.Major != Settings2.Instance.LastUsedVersion.Major || Globals.AppVersion.Minor != Settings2.Instance.LastUsedVersion.Minor)
            {
                string file = AppFolders.ResourcesDir + "\\changes.jpg";
                if (File.Exists(file))
                {
                    new Changes(file).ShowDialog();
                }
            }
            Settings2.Instance.LastUsedVersion = Globals.AppVersion;
        }

    }
}
