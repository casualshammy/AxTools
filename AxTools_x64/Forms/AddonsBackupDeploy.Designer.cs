namespace AxTools.Forms
{
    partial class AddonsBackupDeploy
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxArchives = new MetroFramework.Controls.MetroComboBox();
            this.labelSelectArchive = new MetroFramework.Controls.MetroLabel();
            this.buttonBeginDeployment = new MetroFramework.Controls.MetroButton();
            this.progressBarExtract = new MetroFramework.Controls.MetroProgressBar();
            this.SuspendLayout();
            // 
            // comboBoxArchives
            // 
            this.comboBoxArchives.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxArchives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxArchives.FontSize = MetroFramework.MetroLinkSize.Small;
            this.comboBoxArchives.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.comboBoxArchives.FormattingEnabled = true;
            this.comboBoxArchives.ItemHeight = 19;
            this.comboBoxArchives.Location = new System.Drawing.Point(23, 48);
            this.comboBoxArchives.Name = "comboBoxArchives";
            this.comboBoxArchives.Size = new System.Drawing.Size(250, 25);
            this.comboBoxArchives.Style = MetroFramework.MetroColorStyle.Blue;
            this.comboBoxArchives.TabIndex = 72;
            this.comboBoxArchives.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // labelSelectArchive
            // 
            this.labelSelectArchive.AutoSize = true;
            this.labelSelectArchive.CustomBackground = false;
            this.labelSelectArchive.CustomForeColor = false;
            this.labelSelectArchive.FontSize = MetroFramework.MetroLabelSize.Small;
            this.labelSelectArchive.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.labelSelectArchive.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.labelSelectArchive.Location = new System.Drawing.Point(23, 30);
            this.labelSelectArchive.Name = "labelSelectArchive";
            this.labelSelectArchive.Size = new System.Drawing.Size(110, 15);
            this.labelSelectArchive.Style = MetroFramework.MetroColorStyle.Blue;
            this.labelSelectArchive.TabIndex = 73;
            this.labelSelectArchive.Text = "Select archive file:";
            this.labelSelectArchive.Theme = MetroFramework.MetroThemeStyle.Light;
            this.labelSelectArchive.UseStyleColors = true;
            // 
            // buttonBeginDeployment
            // 
            this.buttonBeginDeployment.Highlight = true;
            this.buttonBeginDeployment.Location = new System.Drawing.Point(279, 48);
            this.buttonBeginDeployment.Name = "buttonBeginDeployment";
            this.buttonBeginDeployment.Size = new System.Drawing.Size(72, 25);
            this.buttonBeginDeployment.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonBeginDeployment.TabIndex = 74;
            this.buttonBeginDeployment.Text = "Deploy";
            this.buttonBeginDeployment.Theme = MetroFramework.MetroThemeStyle.Light;
            this.buttonBeginDeployment.Click += new System.EventHandler(this.buttonBeginDeployment_Click);
            // 
            // progressBarExtract
            // 
            this.progressBarExtract.FontSize = MetroFramework.MetroProgressBarSize.Medium;
            this.progressBarExtract.FontWeight = MetroFramework.MetroProgressBarWeight.Light;
            this.progressBarExtract.HideProgressText = false;
            this.progressBarExtract.Location = new System.Drawing.Point(23, 79);
            this.progressBarExtract.Name = "progressBarExtract";
            this.progressBarExtract.ProgressBarStyle = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarExtract.Size = new System.Drawing.Size(328, 23);
            this.progressBarExtract.Style = MetroFramework.MetroColorStyle.Blue;
            this.progressBarExtract.TabIndex = 75;
            this.progressBarExtract.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.progressBarExtract.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // AddonsBackupDeploy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 107);
            this.Controls.Add(this.progressBarExtract);
            this.Controls.Add(this.buttonBeginDeployment);
            this.Controls.Add(this.labelSelectArchive);
            this.Controls.Add(this.comboBoxArchives);
            this.DisplayHeader = false;
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.Name = "AddonsBackupDeploy";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddonsBackupDeploy_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroComboBox comboBoxArchives;
        private MetroFramework.Controls.MetroLabel labelSelectArchive;
        private MetroFramework.Controls.MetroButton buttonBeginDeployment;
        private MetroFramework.Controls.MetroProgressBar progressBarExtract;

    }
}