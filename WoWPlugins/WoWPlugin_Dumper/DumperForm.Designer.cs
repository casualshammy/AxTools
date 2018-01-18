namespace WoWPlugin_Dumper
{
    partial class DumperForm
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
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.buttonDumpobjects = new System.Windows.Forms.Button();
            this.buttonUIFrames = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonDumpInventory = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonDumpPlayer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(12, 45);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(225, 31);
            this.buttonRefresh.TabIndex = 1;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 12);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(225, 27);
            this.progressBar1.TabIndex = 2;
            // 
            // buttonDumpobjects
            // 
            this.buttonDumpobjects.Location = new System.Drawing.Point(12, 82);
            this.buttonDumpobjects.Name = "buttonDumpobjects";
            this.buttonDumpobjects.Size = new System.Drawing.Size(225, 31);
            this.buttonDumpobjects.TabIndex = 3;
            this.buttonDumpobjects.Text = "Dump objects, NPC, players";
            this.buttonDumpobjects.UseVisualStyleBackColor = true;
            this.buttonDumpobjects.Click += new System.EventHandler(this.buttonDumpobjects_Click);
            // 
            // buttonUIFrames
            // 
            this.buttonUIFrames.Location = new System.Drawing.Point(12, 119);
            this.buttonUIFrames.Name = "buttonUIFrames";
            this.buttonUIFrames.Size = new System.Drawing.Size(225, 31);
            this.buttonUIFrames.TabIndex = 4;
            this.buttonUIFrames.Text = "Dump UIFrames";
            this.buttonUIFrames.UseVisualStyleBackColor = true;
            this.buttonUIFrames.Click += new System.EventHandler(this.buttonUIFrames_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 156);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(225, 31);
            this.button1.TabIndex = 5;
            this.button1.Text = "Dump ChatMessages";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // buttonDumpInventory
            // 
            this.buttonDumpInventory.Location = new System.Drawing.Point(12, 193);
            this.buttonDumpInventory.Name = "buttonDumpInventory";
            this.buttonDumpInventory.Size = new System.Drawing.Size(225, 31);
            this.buttonDumpInventory.TabIndex = 6;
            this.buttonDumpInventory.Text = "Dump inventory";
            this.buttonDumpInventory.UseVisualStyleBackColor = true;
            this.buttonDumpInventory.Click += new System.EventHandler(this.ButtonDumpInventory_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(243, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(858, 262);
            this.textBox1.TabIndex = 7;
            // 
            // buttonDumpPlayer
            // 
            this.buttonDumpPlayer.Location = new System.Drawing.Point(12, 230);
            this.buttonDumpPlayer.Name = "buttonDumpPlayer";
            this.buttonDumpPlayer.Size = new System.Drawing.Size(225, 31);
            this.buttonDumpPlayer.TabIndex = 8;
            this.buttonDumpPlayer.Text = "Dump player";
            this.buttonDumpPlayer.UseVisualStyleBackColor = true;
            this.buttonDumpPlayer.Click += new System.EventHandler(this.buttonDumpPlayer_Click);
            // 
            // DumperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1113, 286);
            this.Controls.Add(this.buttonDumpPlayer);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonDumpInventory);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonUIFrames);
            this.Controls.Add(this.buttonDumpobjects);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonRefresh);
            this.Name = "DumperForm";
            this.Text = "DumperForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DumperForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonDumpobjects;
        private System.Windows.Forms.Button buttonUIFrames;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonDumpInventory;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonDumpPlayer;
    }
}