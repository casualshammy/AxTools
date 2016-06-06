namespace WoWPlugin_Notifier
{
    partial class SettingsForm
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
            this.checkBoxPM = new System.Windows.Forms.CheckBox();
            this.linkLabelNote = new System.Windows.Forms.LinkLabel();
            this.checkBoxBNetPM = new System.Windows.Forms.CheckBox();
            this.checkBoxStaticPopup = new System.Windows.Forms.CheckBox();
            this.checkBoxDisconnect = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBoxPM
            // 
            this.checkBoxPM.AutoSize = true;
            this.checkBoxPM.Location = new System.Drawing.Point(35, 25);
            this.checkBoxPM.Name = "checkBoxPM";
            this.checkBoxPM.Size = new System.Drawing.Size(62, 17);
            this.checkBoxPM.TabIndex = 0;
            this.checkBoxPM.Text = "whisper";
            this.checkBoxPM.UseVisualStyleBackColor = true;
            this.checkBoxPM.CheckedChanged += new System.EventHandler(this.checkBoxPM_CheckedChanged);
            // 
            // linkLabelNote
            // 
            this.linkLabelNote.AutoSize = true;
            this.linkLabelNote.Location = new System.Drawing.Point(12, 9);
            this.linkLabelNote.Name = "linkLabelNote";
            this.linkLabelNote.Size = new System.Drawing.Size(82, 13);
            this.linkLabelNote.TabIndex = 1;
            this.linkLabelNote.TabStop = true;
            this.linkLabelNote.Text = "Send SMS on...";
            // 
            // checkBoxBNetPM
            // 
            this.checkBoxBNetPM.AutoSize = true;
            this.checkBoxBNetPM.Location = new System.Drawing.Point(35, 48);
            this.checkBoxBNetPM.Name = "checkBoxBNetPM";
            this.checkBoxBNetPM.Size = new System.Drawing.Size(92, 17);
            this.checkBoxBNetPM.TabIndex = 2;
            this.checkBoxBNetPM.Text = "B.Net whisper";
            this.checkBoxBNetPM.UseVisualStyleBackColor = true;
            this.checkBoxBNetPM.CheckedChanged += new System.EventHandler(this.checkBoxBNetPM_CheckedChanged);
            // 
            // checkBoxStaticPopup
            // 
            this.checkBoxStaticPopup.AutoSize = true;
            this.checkBoxStaticPopup.Location = new System.Drawing.Point(35, 71);
            this.checkBoxStaticPopup.Name = "checkBoxStaticPopup";
            this.checkBoxStaticPopup.Size = new System.Drawing.Size(181, 17);
            this.checkBoxStaticPopup.TabIndex = 3;
            this.checkBoxStaticPopup.Text = "static popups visible (e.g. invites)";
            this.checkBoxStaticPopup.UseVisualStyleBackColor = true;
            this.checkBoxStaticPopup.CheckedChanged += new System.EventHandler(this.checkBoxStaticPopup_CheckedChanged);
            // 
            // checkBoxDisconnect
            // 
            this.checkBoxDisconnect.AutoSize = true;
            this.checkBoxDisconnect.Location = new System.Drawing.Point(35, 94);
            this.checkBoxDisconnect.Name = "checkBoxDisconnect";
            this.checkBoxDisconnect.Size = new System.Drawing.Size(78, 17);
            this.checkBoxDisconnect.TabIndex = 4;
            this.checkBoxDisconnect.Text = "disconnect";
            this.checkBoxDisconnect.UseVisualStyleBackColor = true;
            this.checkBoxDisconnect.CheckedChanged += new System.EventHandler(this.checkBoxDisconnect_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 123);
            this.Controls.Add(this.checkBoxDisconnect);
            this.Controls.Add(this.checkBoxStaticPopup);
            this.Controls.Add(this.checkBoxBNetPM);
            this.Controls.Add(this.linkLabelNote);
            this.Controls.Add(this.checkBoxPM);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxPM;
        private System.Windows.Forms.LinkLabel linkLabelNote;
        private System.Windows.Forms.CheckBox checkBoxBNetPM;
        private System.Windows.Forms.CheckBox checkBoxStaticPopup;
        private System.Windows.Forms.CheckBox checkBoxDisconnect;
    }
}