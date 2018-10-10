namespace LibSMS
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
            this.textBoxSMSAPI = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.labelSMSURL = new System.Windows.Forms.Label();
            this.labelPushbulletAPIKey = new System.Windows.Forms.Label();
            this.textBoxPushbulletAPIKey = new System.Windows.Forms.TextBox();
            this.labelPushbulletRecipient = new System.Windows.Forms.Label();
            this.textBoxPushbulletRecipient = new System.Windows.Forms.TextBox();
            this.buttonPBAuth = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxSMSAPI
            // 
            this.textBoxSMSAPI.Location = new System.Drawing.Point(12, 25);
            this.textBoxSMSAPI.Name = "textBoxSMSAPI";
            this.textBoxSMSAPI.Size = new System.Drawing.Size(501, 20);
            this.textBoxSMSAPI.TabIndex = 0;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(384, 129);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(129, 23);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "Save && close";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // labelSMSURL
            // 
            this.labelSMSURL.AutoSize = true;
            this.labelSMSURL.Location = new System.Drawing.Point(12, 9);
            this.labelSMSURL.Name = "labelSMSURL";
            this.labelSMSURL.Size = new System.Drawing.Size(58, 13);
            this.labelSMSURL.TabIndex = 2;
            this.labelSMSURL.Text = "SMS URL:";
            // 
            // labelPushbulletAPIKey
            // 
            this.labelPushbulletAPIKey.AutoSize = true;
            this.labelPushbulletAPIKey.Location = new System.Drawing.Point(12, 48);
            this.labelPushbulletAPIKey.Name = "labelPushbulletAPIKey";
            this.labelPushbulletAPIKey.Size = new System.Drawing.Size(99, 13);
            this.labelPushbulletAPIKey.TabIndex = 4;
            this.labelPushbulletAPIKey.Text = "Pushbullet API key:";
            // 
            // textBoxPushbulletAPIKey
            // 
            this.textBoxPushbulletAPIKey.Location = new System.Drawing.Point(12, 64);
            this.textBoxPushbulletAPIKey.Name = "textBoxPushbulletAPIKey";
            this.textBoxPushbulletAPIKey.Size = new System.Drawing.Size(420, 20);
            this.textBoxPushbulletAPIKey.TabIndex = 3;
            // 
            // labelPushbulletRecipient
            // 
            this.labelPushbulletRecipient.AutoSize = true;
            this.labelPushbulletRecipient.Location = new System.Drawing.Point(12, 87);
            this.labelPushbulletRecipient.Name = "labelPushbulletRecipient";
            this.labelPushbulletRecipient.Size = new System.Drawing.Size(138, 13);
            this.labelPushbulletRecipient.TabIndex = 6;
            this.labelPushbulletRecipient.Text = "Pushbullet recipient (e-mail):";
            // 
            // textBoxPushbulletRecipient
            // 
            this.textBoxPushbulletRecipient.Location = new System.Drawing.Point(12, 103);
            this.textBoxPushbulletRecipient.Name = "textBoxPushbulletRecipient";
            this.textBoxPushbulletRecipient.Size = new System.Drawing.Size(501, 20);
            this.textBoxPushbulletRecipient.TabIndex = 5;
            // 
            // buttonPBAuth
            // 
            this.buttonPBAuth.Location = new System.Drawing.Point(438, 62);
            this.buttonPBAuth.Name = "buttonPBAuth";
            this.buttonPBAuth.Size = new System.Drawing.Size(75, 23);
            this.buttonPBAuth.TabIndex = 7;
            this.buttonPBAuth.Text = "Auth";
            this.buttonPBAuth.UseVisualStyleBackColor = true;
            this.buttonPBAuth.Click += new System.EventHandler(this.buttonPBAuth_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 165);
            this.Controls.Add(this.buttonPBAuth);
            this.Controls.Add(this.labelPushbulletRecipient);
            this.Controls.Add(this.textBoxPushbulletRecipient);
            this.Controls.Add(this.labelPushbulletAPIKey);
            this.Controls.Add(this.textBoxPushbulletAPIKey);
            this.Controls.Add(this.labelSMSURL);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxSMSAPI);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSMSAPI;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label labelSMSURL;
        private System.Windows.Forms.Label labelPushbulletAPIKey;
        private System.Windows.Forms.TextBox textBoxPushbulletAPIKey;
        private System.Windows.Forms.Label labelPushbulletRecipient;
        private System.Windows.Forms.TextBox textBoxPushbulletRecipient;
        private System.Windows.Forms.Button buttonPBAuth;
    }
}