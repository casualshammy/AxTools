using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.Classes;
using AxTools.Components;
using AxTools.Properties;
using Ionic.Zip;
using Ionic.Zlib;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class AddonsBackup : BorderedMetroForm
    {
        public AddonsBackup()
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            metroStyleManager1.Style = Settings.NewStyleColor;
            Width = MainForm.Instance.Width;
            BeginInvoke((MethodInvoker) delegate
            {
                int maxHeight = Screen.PrimaryScreen.WorkingArea.Height;
                MainForm mainForm = MainForm.Instance;
                Location = maxHeight - mainForm.Location.Y - mainForm.Size.Height - 20 > Size.Height
                    ? new Point(mainForm.Location.X, mainForm.Location.Y + mainForm.Size.Height + 20)
                    : new Point(mainForm.Location.X, mainForm.Location.Y - Size.Height - 20);
                OnActivated(EventArgs.Empty);
            });
        }

        internal void Start(bool trayMode)
        {
            if (!Directory.Exists(Settings.WowExe + "\\WTF"))
            {
                Log.Print("Backup error: WTF directory isn't found");
                if (trayMode)
                {
                    MainForm.Instance.ShowNotifyIconMessage("Backup error", "\"WTF\" folder isn't found", ToolTipIcon.Error);
                }
                else
                {
                    this.ShowTaskDialog("Backup error", "\"WTF\" folder isn't found", TaskDialogButton.OK, TaskDialogIcon.Stop);
                }
                return;
            }
            if (Utils.CalcDirectorySize(Settings.WowExe + "\\WTF") > 1000 * 1024 * 1024)
            {
                Log.Print("Backup error: WTF directory is too large", true);
                if (trayMode)
                {
                    MainForm.Instance.ShowNotifyIconMessage("Backup error", "WTF directory is too large", ToolTipIcon.Error);
                }
                else
                {
                    this.ShowTaskDialog("Backup error", "WTF directory is too large", TaskDialogButton.OK, TaskDialogIcon.Stop);
                }
                return;
            }
            Log.Print("BackupAddons :: Starting...");
            DirectoryInfo backupDirectory = new DirectoryInfo(Settings.AddonsBackupPath);
            if (!backupDirectory.Exists)
            {
                backupDirectory.Create();
                Log.Print("BackupAddons :: Backup directory created");
            }
            List<FileInfo> backupFiles = backupDirectory.GetFileSystemInfos().Where(i => i.Name.Contains("AddonsBackup_") && i is FileInfo).Cast<FileInfo>().ToList();
            Log.Print("BackupAddons :: Total backup files: " + backupFiles.Count);
            if (backupFiles.Count >= Settings.AddonsBackupNum)
            {
                // I place newest file to the end of list
                backupFiles.Sort((first, second) =>
                {
                    if (first.CreationTimeUtc > second.CreationTimeUtc)
                    {
                        return 1;
                    }
                    return -1;
                });
                for (int i = 0; i < backupFiles.Count - Settings.AddonsBackupNum + 1; i++)
                {
                    backupFiles[i].Delete();
                    Log.Print("BackupAddons :: Old backup file is deleted: " + backupFiles[i].Name);
                }
            }
            if (trayMode)
            {
                MainForm.Instance.ShowNotifyIconMessage("Performing backup operation", "Please don't close AxTools until the operation is completed", ToolTipIcon.Info);
            }
            string zipPath = String.Format("{0}\\AddonsBackup_{1:yyyyMMdd_HHmmss}.zip", Settings.AddonsBackupPath, DateTime.UtcNow);
            Log.Print("BackupAddons :: Zipping to file: " + zipPath);
            try
            {
                using (ZipFile zip = new ZipFile(zipPath, Encoding.UTF8))
                {
                    zip.CompressionLevel = (CompressionLevel)Settings.BackupCompressionLevel;
                    zip.AddDirectory(Settings.WowExe + "\\WTF", "\\WTF");
                    zip.AddDirectory(Settings.WowExe + "\\Interface", "\\Interface");
                    zip.SaveProgress += AddonsBackup_SaveProgress;
                    zip.Save();
                    zip.SaveProgress -= AddonsBackup_SaveProgress;
                }
                Log.Print("BackupAddons :: Backup successfully created: " + zipPath);
                if (trayMode)
                {
                    MainForm.Instance.ShowNotifyIconMessage("AddOns backup operation was successfully completed", "Backup file was stored in " + Settings.AddonsBackupPath, ToolTipIcon.Info);
                }
                else
                {
                    this.ShowTaskDialog("Backup successful", "Backup file was stored in " + Settings.AddonsBackupPath, TaskDialogButton.OK, TaskDialogIcon.Information);
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                Log.Print("BackupAddons :: Backup error: Zipping failed: " + ex.Message, true);
                this.ShowTaskDialog("Backup error", "Zipping failed\r\n" + ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private void AddonsBackup_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (e.EntriesSaved != 0 && e.EntriesTotal != 0 && e.EntriesTotal >= e.EntriesSaved)
            {
                BeginInvoke((MethodInvoker) delegate
                {
                    metroProgressBar1.Maximum = e.EntriesTotal;
                    metroProgressBar1.Value = e.EntriesSaved;
                    metroLabel1.Text = e.EntriesSaved + "/" + e.EntriesTotal;
                });
            }
        }

    }
}
