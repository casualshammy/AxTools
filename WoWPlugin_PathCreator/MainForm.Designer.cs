namespace WoWPlugin_PathCreator
{
    partial class MainForm
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
            this.buttonCreateWaypoint = new System.Windows.Forms.Button();
            this.labelLocation = new System.Windows.Forms.Label();
            this.textBoxInteract = new System.Windows.Forms.TextBox();
            this.buttonInteract = new System.Windows.Forms.Button();
            this.buttonDialogOption = new System.Windows.Forms.Button();
            this.textBoxDialogOption = new System.Windows.Forms.TextBox();
            this.buttonSendChat = new System.Windows.Forms.Button();
            this.textBoxSendChat = new System.Windows.Forms.TextBox();
            this.buttonStopProfile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCreateWaypoint
            // 
            this.buttonCreateWaypoint.Location = new System.Drawing.Point(176, 12);
            this.buttonCreateWaypoint.Name = "buttonCreateWaypoint";
            this.buttonCreateWaypoint.Size = new System.Drawing.Size(96, 23);
            this.buttonCreateWaypoint.TabIndex = 0;
            this.buttonCreateWaypoint.Text = "Create waypoint";
            this.buttonCreateWaypoint.UseVisualStyleBackColor = true;
            this.buttonCreateWaypoint.Click += new System.EventHandler(this.buttonCreateWaypoint_Click);
            // 
            // labelLocation
            // 
            this.labelLocation.AutoSize = true;
            this.labelLocation.Location = new System.Drawing.Point(12, 17);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(43, 13);
            this.labelLocation.TabIndex = 1;
            this.labelLocation.Text = "[0, 0, 0]";
            // 
            // textBoxInteract
            // 
            this.textBoxInteract.Location = new System.Drawing.Point(15, 42);
            this.textBoxInteract.Name = "textBoxInteract";
            this.textBoxInteract.Size = new System.Drawing.Size(155, 20);
            this.textBoxInteract.TabIndex = 2;
            // 
            // buttonInteract
            // 
            this.buttonInteract.Location = new System.Drawing.Point(176, 41);
            this.buttonInteract.Name = "buttonInteract";
            this.buttonInteract.Size = new System.Drawing.Size(96, 22);
            this.buttonInteract.TabIndex = 3;
            this.buttonInteract.Text = "Interact";
            this.buttonInteract.UseVisualStyleBackColor = true;
            this.buttonInteract.Click += new System.EventHandler(this.buttonInteract_Click);
            // 
            // buttonDialogOption
            // 
            this.buttonDialogOption.Location = new System.Drawing.Point(176, 69);
            this.buttonDialogOption.Name = "buttonDialogOption";
            this.buttonDialogOption.Size = new System.Drawing.Size(96, 22);
            this.buttonDialogOption.TabIndex = 5;
            this.buttonDialogOption.Text = "Dialog option";
            this.buttonDialogOption.UseVisualStyleBackColor = true;
            this.buttonDialogOption.Click += new System.EventHandler(this.buttonDialogOption_Click);
            // 
            // textBoxDialogOption
            // 
            this.textBoxDialogOption.Location = new System.Drawing.Point(15, 70);
            this.textBoxDialogOption.Name = "textBoxDialogOption";
            this.textBoxDialogOption.Size = new System.Drawing.Size(155, 20);
            this.textBoxDialogOption.TabIndex = 4;
            // 
            // buttonSendChat
            // 
            this.buttonSendChat.Location = new System.Drawing.Point(176, 97);
            this.buttonSendChat.Name = "buttonSendChat";
            this.buttonSendChat.Size = new System.Drawing.Size(96, 22);
            this.buttonSendChat.TabIndex = 7;
            this.buttonSendChat.Text = "Send to chat";
            this.buttonSendChat.UseVisualStyleBackColor = true;
            this.buttonSendChat.Click += new System.EventHandler(this.buttonSendChat_Click);
            // 
            // textBoxSendChat
            // 
            this.textBoxSendChat.Location = new System.Drawing.Point(15, 98);
            this.textBoxSendChat.Name = "textBoxSendChat";
            this.textBoxSendChat.Size = new System.Drawing.Size(155, 20);
            this.textBoxSendChat.TabIndex = 6;
            // 
            // buttonStopProfile
            // 
            this.buttonStopProfile.Location = new System.Drawing.Point(176, 125);
            this.buttonStopProfile.Name = "buttonStopProfile";
            this.buttonStopProfile.Size = new System.Drawing.Size(96, 22);
            this.buttonStopProfile.TabIndex = 9;
            this.buttonStopProfile.Text = "Stop profile";
            this.buttonStopProfile.UseVisualStyleBackColor = true;
            this.buttonStopProfile.Click += new System.EventHandler(this.buttonStopProfile_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 225);
            this.Controls.Add(this.buttonStopProfile);
            this.Controls.Add(this.buttonSendChat);
            this.Controls.Add(this.textBoxSendChat);
            this.Controls.Add(this.buttonDialogOption);
            this.Controls.Add(this.textBoxDialogOption);
            this.Controls.Add(this.buttonInteract);
            this.Controls.Add(this.textBoxInteract);
            this.Controls.Add(this.labelLocation);
            this.Controls.Add(this.buttonCreateWaypoint);
            this.Name = "MainForm";
            this.Text = "Path Creator";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCreateWaypoint;
        internal System.Windows.Forms.Label labelLocation;
        private System.Windows.Forms.TextBox textBoxInteract;
        private System.Windows.Forms.Button buttonInteract;
        private System.Windows.Forms.Button buttonDialogOption;
        private System.Windows.Forms.TextBox textBoxDialogOption;
        private System.Windows.Forms.Button buttonSendChat;
        private System.Windows.Forms.TextBox textBoxSendChat;
        private System.Windows.Forms.Button buttonStopProfile;
    }
}