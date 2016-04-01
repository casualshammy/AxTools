namespace AxTools.Forms
{
    partial class AppSettingsWoWBinds
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
            this.buttonTarget = new MetroFramework.Controls.MetroButton();
            this.textBoxTarget = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel16 = new MetroFramework.Controls.MetroLabel();
            this.buttonInteract = new MetroFramework.Controls.MetroButton();
            this.textBoxInteract = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel15 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // buttonTarget
            // 
            this.buttonTarget.Highlight = true;
            this.buttonTarget.Location = new System.Drawing.Point(120, 100);
            this.buttonTarget.Name = "buttonTarget";
            this.buttonTarget.Size = new System.Drawing.Size(54, 23);
            this.buttonTarget.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonTarget.TabIndex = 67;
            this.buttonTarget.Text = "Clear";
            this.buttonTarget.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // textBoxTarget
            // 
            this.textBoxTarget.CustomBackground = false;
            this.textBoxTarget.CustomForeColor = false;
            this.textBoxTarget.FontSize = MetroFramework.MetroTextBoxSize.Small;
            this.textBoxTarget.FontWeight = MetroFramework.MetroTextBoxWeight.Regular;
            this.textBoxTarget.Location = new System.Drawing.Point(23, 100);
            this.textBoxTarget.Multiline = false;
            this.textBoxTarget.Name = "textBoxTarget";
            this.textBoxTarget.SelectedText = "";
            this.textBoxTarget.Size = new System.Drawing.Size(91, 23);
            this.textBoxTarget.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxTarget.TabIndex = 66;
            this.textBoxTarget.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxTarget.UseStyleColors = true;
            // 
            // metroLabel16
            // 
            this.metroLabel16.AutoSize = true;
            this.metroLabel16.CustomBackground = false;
            this.metroLabel16.CustomForeColor = false;
            this.metroLabel16.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabel16.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel16.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel16.Location = new System.Drawing.Point(23, 78);
            this.metroLabel16.Name = "metroLabel16";
            this.metroLabel16.Size = new System.Drawing.Size(119, 19);
            this.metroLabel16.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel16.TabIndex = 65;
            this.metroLabel16.Text = "Target mouseover:";
            this.metroLabel16.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel16.UseStyleColors = true;
            // 
            // buttonInteract
            // 
            this.buttonInteract.Highlight = true;
            this.buttonInteract.Location = new System.Drawing.Point(120, 52);
            this.buttonInteract.Name = "buttonInteract";
            this.buttonInteract.Size = new System.Drawing.Size(54, 23);
            this.buttonInteract.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonInteract.TabIndex = 64;
            this.buttonInteract.Text = "Clear";
            this.buttonInteract.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // textBoxInteract
            // 
            this.textBoxInteract.CustomBackground = false;
            this.textBoxInteract.CustomForeColor = false;
            this.textBoxInteract.FontSize = MetroFramework.MetroTextBoxSize.Small;
            this.textBoxInteract.FontWeight = MetroFramework.MetroTextBoxWeight.Regular;
            this.textBoxInteract.Location = new System.Drawing.Point(23, 52);
            this.textBoxInteract.Multiline = false;
            this.textBoxInteract.Name = "textBoxInteract";
            this.textBoxInteract.SelectedText = "";
            this.textBoxInteract.Size = new System.Drawing.Size(91, 23);
            this.textBoxInteract.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxInteract.TabIndex = 63;
            this.textBoxInteract.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxInteract.UseStyleColors = true;
            // 
            // metroLabel15
            // 
            this.metroLabel15.AutoSize = true;
            this.metroLabel15.CustomBackground = false;
            this.metroLabel15.CustomForeColor = false;
            this.metroLabel15.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabel15.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel15.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel15.Location = new System.Drawing.Point(23, 30);
            this.metroLabel15.Name = "metroLabel15";
            this.metroLabel15.Size = new System.Drawing.Size(151, 19);
            this.metroLabel15.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel15.TabIndex = 62;
            this.metroLabel15.Text = "Interact with mouseover:";
            this.metroLabel15.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel15.UseStyleColors = true;
            // 
            // AppSettingsWoWBinds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(199, 148);
            this.Controls.Add(this.buttonTarget);
            this.Controls.Add(this.textBoxTarget);
            this.Controls.Add(this.metroLabel16);
            this.Controls.Add(this.buttonInteract);
            this.Controls.Add(this.textBoxInteract);
            this.Controls.Add(this.metroLabel15);
            this.DisplayHeader = false;
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppSettingsWoWBinds";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.Text = "AppSettingsWoWBinds";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroButton buttonTarget;
        private MetroFramework.Controls.MetroTextBox textBoxTarget;
        private MetroFramework.Controls.MetroLabel metroLabel16;
        private MetroFramework.Controls.MetroButton buttonInteract;
        private MetroFramework.Controls.MetroTextBox textBoxInteract;
        private MetroFramework.Controls.MetroLabel metroLabel15;
    }
}