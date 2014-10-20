namespace WoWHashCalc
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
            this.textBoxSHA256 = new System.Windows.Forms.TextBox();
            this.labelSHA256 = new System.Windows.Forms.Label();
            this.textBoxSHA512 = new System.Windows.Forms.TextBox();
            this.textBoxSHA384 = new System.Windows.Forms.TextBox();
            this.labelSHA384 = new System.Windows.Forms.Label();
            this.labelSHA512 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxSHA256
            // 
            this.textBoxSHA256.Location = new System.Drawing.Point(12, 12);
            this.textBoxSHA256.Name = "textBoxSHA256";
            this.textBoxSHA256.ReadOnly = true;
            this.textBoxSHA256.Size = new System.Drawing.Size(1110, 20);
            this.textBoxSHA256.TabIndex = 0;
            // 
            // labelSHA256
            // 
            this.labelSHA256.AutoSize = true;
            this.labelSHA256.Location = new System.Drawing.Point(12, 35);
            this.labelSHA256.Name = "labelSHA256";
            this.labelSHA256.Size = new System.Drawing.Size(69, 13);
            this.labelSHA256.TabIndex = 1;
            this.labelSHA256.Text = "labelSHA256";
            // 
            // textBoxSHA512
            // 
            this.textBoxSHA512.Location = new System.Drawing.Point(12, 90);
            this.textBoxSHA512.Name = "textBoxSHA512";
            this.textBoxSHA512.ReadOnly = true;
            this.textBoxSHA512.Size = new System.Drawing.Size(1110, 20);
            this.textBoxSHA512.TabIndex = 2;
            // 
            // textBoxSHA384
            // 
            this.textBoxSHA384.Location = new System.Drawing.Point(12, 51);
            this.textBoxSHA384.Name = "textBoxSHA384";
            this.textBoxSHA384.ReadOnly = true;
            this.textBoxSHA384.Size = new System.Drawing.Size(1110, 20);
            this.textBoxSHA384.TabIndex = 3;
            // 
            // labelSHA384
            // 
            this.labelSHA384.AutoSize = true;
            this.labelSHA384.Location = new System.Drawing.Point(12, 74);
            this.labelSHA384.Name = "labelSHA384";
            this.labelSHA384.Size = new System.Drawing.Size(35, 13);
            this.labelSHA384.TabIndex = 4;
            this.labelSHA384.Text = "label1";
            // 
            // labelSHA512
            // 
            this.labelSHA512.AutoSize = true;
            this.labelSHA512.Location = new System.Drawing.Point(12, 113);
            this.labelSHA512.Name = "labelSHA512";
            this.labelSHA512.Size = new System.Drawing.Size(35, 13);
            this.labelSHA512.TabIndex = 5;
            this.labelSHA512.Text = "label1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 135);
            this.Controls.Add(this.labelSHA512);
            this.Controls.Add(this.labelSHA384);
            this.Controls.Add(this.textBoxSHA384);
            this.Controls.Add(this.textBoxSHA512);
            this.Controls.Add(this.labelSHA256);
            this.Controls.Add(this.textBoxSHA256);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSHA256;
        private System.Windows.Forms.Label labelSHA256;
        private System.Windows.Forms.TextBox textBoxSHA512;
        private System.Windows.Forms.TextBox textBoxSHA384;
        private System.Windows.Forms.Label labelSHA384;
        private System.Windows.Forms.Label labelSHA512;
    }
}

