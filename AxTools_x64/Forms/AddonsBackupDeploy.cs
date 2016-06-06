using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxTools.Helpers;
using Components;
using AxTools.Services;
using Components.Forms;

namespace AxTools.Forms
{
    internal partial class AddonsBackupDeploy : BorderedMetroForm
    {
        private readonly string[] pathsToArchives; 

        public AddonsBackupDeploy()
        {
            InitializeComponent();
            StyleManager.Style = Settings.Instance.StyleColor;
            pathsToArchives = AddonsBackup.GetArchives();
            foreach (string archive in pathsToArchives)
            {
                string fileName = Path.GetFileNameWithoutExtension(archive);
                // ReSharper disable once PossibleNullReferenceException
                string s = fileName.Replace("AddonsBackup_", "");
                DateTime dateTime = DateTime.ParseExact(s, "yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
                comboBoxArchives.Items.Add(dateTime.ToString());
            }
            progressBarExtract.Visible = false;
            AddonsBackup.IsRunningChanged += AddonsBackup_IsRunningChanged;
            AddonsBackup.ProgressPercentageChanged += AddonsBackup_ProgressPercentageChanged;
        }

        private void AddonsBackupDeploy_FormClosing(object sender, FormClosingEventArgs e)
        {
            AddonsBackup.IsRunningChanged -= AddonsBackup_IsRunningChanged;
            AddonsBackup.ProgressPercentageChanged -= AddonsBackup_ProgressPercentageChanged;
        }

        private void AddonsBackup_IsRunningChanged(bool obj)
        {
            PostInvoke(() => { progressBarExtract.Visible = obj; });
        }

        private void AddonsBackup_ProgressPercentageChanged(int obj)
        {
            PostInvoke(() => { progressBarExtract.Value = obj; });
        }

        private void buttonBeginDeployment_Click(object sender, EventArgs e)
        {
            buttonBeginDeployment.Enabled = false;
            Task.Run(() => { AddonsBackup.DeployArchive(pathsToArchives[comboBoxArchives.SelectedIndex]); }).ContinueWith(task => { buttonBeginDeployment.Enabled = true; });
        }

    }
}
