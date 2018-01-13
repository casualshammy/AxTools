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
            // DumperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 128);
            this.Controls.Add(this.buttonDumpobjects);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonRefresh);
            this.Name = "DumperForm";
            this.Text = "DumperForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonDumpobjects;
    }
}