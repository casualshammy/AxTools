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
            this.textBoxWaitWhileLua = new System.Windows.Forms.TextBox();
            this.buttonWaitWhile = new System.Windows.Forms.Button();
            this.textBoxWaitWhileLag = new System.Windows.Forms.TextBox();
            this.btnEnableLoopPath = new System.Windows.Forms.Button();
            this.btnStartFromNearestPoint = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericPrecision2D)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCreateWaypoint
            // 
            this.buttonCreateWaypoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateWaypoint.Location = new System.Drawing.Point(335, 78);
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
            this.labelLocation.Location = new System.Drawing.Point(12, 83);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(43, 13);
            this.labelLocation.TabIndex = 1;
            this.labelLocation.Text = "[0, 0, 0]";
            // 
            // textBoxInteract
            // 
            this.textBoxInteract.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxInteract.Location = new System.Drawing.Point(15, 108);
            this.textBoxInteract.Name = "textBoxInteract";
            this.textBoxInteract.Size = new System.Drawing.Size(314, 20);
            this.textBoxInteract.TabIndex = 2;
            // 
            // buttonInteract
            // 
            this.buttonInteract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInteract.Location = new System.Drawing.Point(335, 106);
            this.buttonInteract.Name = "buttonInteract";
            this.buttonInteract.Size = new System.Drawing.Size(96, 22);
            this.buttonInteract.TabIndex = 3;
            this.buttonInteract.Text = "Interact";
            this.buttonInteract.UseVisualStyleBackColor = true;
            this.buttonInteract.Click += new System.EventHandler(this.buttonInteract_Click);
            // 
            // buttonDialogOption
            // 
            this.buttonDialogOption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDialogOption.Location = new System.Drawing.Point(335, 134);
            this.buttonDialogOption.Name = "buttonDialogOption";
            this.buttonDialogOption.Size = new System.Drawing.Size(96, 22);
            this.buttonDialogOption.TabIndex = 5;
            this.buttonDialogOption.Text = "Dialog option";
            this.buttonDialogOption.UseVisualStyleBackColor = true;
            this.buttonDialogOption.Click += new System.EventHandler(this.buttonDialogOption_Click);
            // 
            // textBoxDialogOption
            // 
            this.textBoxDialogOption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxDialogOption.Location = new System.Drawing.Point(15, 136);
            this.textBoxDialogOption.Name = "textBoxDialogOption";
            this.textBoxDialogOption.Size = new System.Drawing.Size(314, 20);
            this.textBoxDialogOption.TabIndex = 4;
            // 
            // buttonSendChat
            // 
            this.buttonSendChat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSendChat.Location = new System.Drawing.Point(335, 162);
            this.buttonSendChat.Name = "buttonSendChat";
            this.buttonSendChat.Size = new System.Drawing.Size(96, 22);
            this.buttonSendChat.TabIndex = 7;
            this.buttonSendChat.Text = "Send to chat";
            this.buttonSendChat.UseVisualStyleBackColor = true;
            this.buttonSendChat.Click += new System.EventHandler(this.buttonSendChat_Click);
            // 
            // textBoxSendChat
            // 
            this.textBoxSendChat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSendChat.Location = new System.Drawing.Point(15, 164);
            this.textBoxSendChat.Name = "textBoxSendChat";
            this.textBoxSendChat.Size = new System.Drawing.Size(314, 20);
            this.textBoxSendChat.TabIndex = 6;
            // 
            // buttonStopProfile
            // 
            this.buttonStopProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStopProfile.Location = new System.Drawing.Point(335, 344);
            this.buttonStopProfile.Name = "buttonStopProfile";
            this.buttonStopProfile.Size = new System.Drawing.Size(96, 22);
            this.buttonStopProfile.TabIndex = 9;
            this.buttonStopProfile.Text = "Stop profile";
            this.buttonStopProfile.UseVisualStyleBackColor = true;
            this.buttonStopProfile.Click += new System.EventHandler(this.buttonStopProfile_Click);
            // 
            // buttonWait
            // 
            this.buttonWait.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWait.Location = new System.Drawing.Point(335, 192);
            this.buttonWait.Name = "buttonWait";
            this.buttonWait.Size = new System.Drawing.Size(96, 22);
            this.buttonWait.TabIndex = 11;
            this.buttonWait.Text = "Wait (ms)";
            this.buttonWait.UseVisualStyleBackColor = true;
            this.buttonWait.Click += new System.EventHandler(this.buttonWait_Click);
            // 
            // textBoxWait
            // 
            this.textBoxWait.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxWait.Location = new System.Drawing.Point(15, 192);
            this.textBoxWait.Name = "textBoxWait";
            this.textBoxWait.Size = new System.Drawing.Size(314, 20);
            this.textBoxWait.TabIndex = 10;
            // 
            // buttonPrecision2D
            // 
            this.buttonPrecision2D.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPrecision2D.Location = new System.Drawing.Point(335, 220);
            this.buttonPrecision2D.Name = "buttonPrecision2D";
            this.buttonPrecision2D.Size = new System.Drawing.Size(96, 22);
            this.buttonPrecision2D.TabIndex = 12;
            this.buttonPrecision2D.Text = "Set 2D precision";
            this.buttonPrecision2D.UseVisualStyleBackColor = true;
            this.buttonPrecision2D.Click += new System.EventHandler(this.buttonPrecision2D_Click);
            // 
            // numericPrecision2D
            // 
            this.numericPrecision2D.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericPrecision2D.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericPrecision2D.Location = new System.Drawing.Point(15, 220);
            this.numericPrecision2D.Name = "numericPrecision2D";
            this.numericPrecision2D.Size = new System.Drawing.Size(314, 20);
            this.numericPrecision2D.TabIndex = 13;
            this.numericPrecision2D.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericPrecision2D.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // textBoxWaitWhileLua
            // 
            this.textBoxWaitWhileLua.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxWaitWhileLua.Location = new System.Drawing.Point(15, 246);
            this.textBoxWaitWhileLua.Name = "textBoxWaitWhileLua";
            this.textBoxWaitWhileLua.Size = new System.Drawing.Size(314, 20);
            this.textBoxWaitWhileLua.TabIndex = 14;
            // 
            // buttonWaitWhile
            // 
            this.buttonWaitWhile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWaitWhile.Location = new System.Drawing.Point(335, 246);
            this.buttonWaitWhile.Name = "buttonWaitWhile";
            this.buttonWaitWhile.Size = new System.Drawing.Size(96, 57);
            this.buttonWaitWhile.TabIndex = 15;
            this.buttonWaitWhile.Text = "WaitWhile\r\nLua expr\r\nInterval in msec";
            this.buttonWaitWhile.UseVisualStyleBackColor = true;
            this.buttonWaitWhile.Click += new System.EventHandler(this.buttonWaitWhile_Click);
            // 
            // textBoxWaitWhileLag
            // 
            this.textBoxWaitWhileLag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxWaitWhileLag.Location = new System.Drawing.Point(15, 283);
            this.textBoxWaitWhileLag.Name = "textBoxWaitWhileLag";
            this.textBoxWaitWhileLag.Size = new System.Drawing.Size(314, 20);
            this.textBoxWaitWhileLag.TabIndex = 16;
            // 
            // btnEnableLoopPath
            // 
            this.btnEnableLoopPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnableLoopPath.Location = new System.Drawing.Point(15, 12);
            this.btnEnableLoopPath.Name = "btnEnableLoopPath";
            this.btnEnableLoopPath.Size = new System.Drawing.Size(207, 22);
            this.btnEnableLoopPath.TabIndex = 17;
            this.btnEnableLoopPath.Text = "Loop path";
            this.btnEnableLoopPath.UseVisualStyleBackColor = true;
            this.btnEnableLoopPath.Click += new System.EventHandler(this.btnEnableLoopPath_Click);
            // 
            // btnStartFromNearestPoint
            // 
            this.btnStartFromNearestPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartFromNearestPoint.Location = new System.Drawing.Point(228, 12);
            this.btnStartFromNearestPoint.Name = "btnStartFromNearestPoint";
            this.btnStartFromNearestPoint.Size = new System.Drawing.Size(203, 22);
            this.btnStartFromNearestPoint.TabIndex = 18;
            this.btnStartFromNearestPoint.Text = "Start from nearest point";
            this.btnStartFromNearestPoint.UseVisualStyleBackColor = true;
            this.btnStartFromNearestPoint.Click += new System.EventHandler(this.btnStartFromNearestPoint_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 378);
            this.Controls.Add(this.btnStartFromNearestPoint);
            this.Controls.Add(this.btnEnableLoopPath);
            this.Controls.Add(this.textBoxWaitWhileLag);
            this.Controls.Add(this.buttonWaitWhile);
            this.Controls.Add(this.textBoxWaitWhileLua);
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
        private System.Windows.Forms.TextBox textBoxWaitWhileLua;
        private System.Windows.Forms.Button buttonWaitWhile;
        private System.Windows.Forms.TextBox textBoxWaitWhileLag;
        private System.Windows.Forms.Button btnEnableLoopPath;
        private System.Windows.Forms.Button btnStartFromNearestPoint;
    }
}