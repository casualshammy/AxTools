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
            this.buttonWait = new System.Windows.Forms.Button();
            this.textBoxWait = new System.Windows.Forms.TextBox();
            this.buttonPrecision2D = new System.Windows.Forms.Button();
            this.numericPrecision2D = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericPrecision2D)).BeginInit();
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
            this.buttonStopProfile.Location = new System.Drawing.Point(176, 191);
            this.buttonStopProfile.Name = "buttonStopProfile";
            this.buttonStopProfile.Size = new System.Drawing.Size(96, 22);
            this.buttonStopProfile.TabIndex = 9;
            this.buttonStopProfile.Text = "Stop profile";
            this.buttonStopProfile.UseVisualStyleBackColor = true;
            this.buttonStopProfile.Click += new System.EventHandler(this.buttonStopProfile_Click);
            // 
            // buttonWait
            // 
            this.buttonWait.Location = new System.Drawing.Point(176, 125);
            this.buttonWait.Name = "buttonWait";
            this.buttonWait.Size = new System.Drawing.Size(96, 22);
            this.buttonWait.TabIndex = 11;
            this.buttonWait.Text = "Wait (ms)";
            this.buttonWait.UseVisualStyleBackColor = true;
            this.buttonWait.Click += new System.EventHandler(this.buttonWait_Click);
            // 
            // textBoxWait
            // 
            this.textBoxWait.Location = new System.Drawing.Point(15, 126);
            this.textBoxWait.Name = "textBoxWait";
            this.textBoxWait.Size = new System.Drawing.Size(155, 20);
            this.textBoxWait.TabIndex = 10;
            // 
            // buttonPrecision2D
            // 
            this.buttonPrecision2D.Location = new System.Drawing.Point(176, 153);
            this.buttonPrecision2D.Name = "buttonPrecision2D";
            this.buttonPrecision2D.Size = new System.Drawing.Size(96, 22);
            this.buttonPrecision2D.TabIndex = 12;
            this.buttonPrecision2D.Text = "Set 2D precision";
            this.buttonPrecision2D.UseVisualStyleBackColor = true;
            this.buttonPrecision2D.Click += new System.EventHandler(this.buttonPrecision2D_Click);
            // 
            // numericPrecision2D
            // 
            this.numericPrecision2D.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericPrecision2D.Location = new System.Drawing.Point(15, 154);
            this.numericPrecision2D.Name = "numericPrecision2D";
            this.numericPrecision2D.Size = new System.Drawing.Size(155, 20);
            this.numericPrecision2D.TabIndex = 13;
            this.numericPrecision2D.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericPrecision2D.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 225);
            this.Controls.Add(this.numericPrecision2D);
            this.Controls.Add(this.buttonPrecision2D);
            this.Controls.Add(this.buttonWait);
            this.Controls.Add(this.textBoxWait);
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
            ((System.ComponentModel.ISupportInitialize)(this.numericPrecision2D)).EndInit();
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
        private System.Windows.Forms.Button buttonWait;
        private System.Windows.Forms.TextBox textBoxWait;
        private System.Windows.Forms.Button buttonPrecision2D;
        private System.Windows.Forms.NumericUpDown numericPrecision2D;
    }
}