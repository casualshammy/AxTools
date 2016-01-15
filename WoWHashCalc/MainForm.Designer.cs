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
            this.textBoxX86 = new System.Windows.Forms.TextBox();
            this.labelX86 = new System.Windows.Forms.Label();
            this.labelX64 = new System.Windows.Forms.Label();
            this.textBoxX64 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxX86
            // 
            this.textBoxX86.Location = new System.Drawing.Point(12, 25);
            this.textBoxX86.Name = "textBoxX86";
            this.textBoxX86.ReadOnly = true;
            this.textBoxX86.Size = new System.Drawing.Size(1110, 20);
            this.textBoxX86.TabIndex = 0;
            this.textBoxX86.Text = "Please wait...";
            // 
            // labelX86
            // 
            this.labelX86.AutoSize = true;
            this.labelX86.Location = new System.Drawing.Point(12, 9);
            this.labelX86.Name = "labelX86";
            this.labelX86.Size = new System.Drawing.Size(26, 13);
            this.labelX86.TabIndex = 4;
            this.labelX86.Text = "X86";
            // 
            // labelX64
            // 
            this.labelX64.AutoSize = true;
            this.labelX64.Location = new System.Drawing.Point(12, 66);
            this.labelX64.Name = "labelX64";
            this.labelX64.Size = new System.Drawing.Size(26, 13);
            this.labelX64.TabIndex = 5;
            this.labelX64.Text = "X64";
            // 
            // textBoxX64
            // 
            this.textBoxX64.Location = new System.Drawing.Point(12, 82);
            this.textBoxX64.Name = "textBoxX64";
            this.textBoxX64.ReadOnly = true;
            this.textBoxX64.Size = new System.Drawing.Size(1110, 20);
            this.textBoxX64.TabIndex = 6;
            this.textBoxX64.Text = "Please wait...";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 115);
            this.Controls.Add(this.textBoxX64);
            this.Controls.Add(this.labelX64);
            this.Controls.Add(this.labelX86);
            this.Controls.Add(this.textBoxX86);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxX86;
        private System.Windows.Forms.Label labelX86;
        private System.Windows.Forms.Label labelX64;
        private System.Windows.Forms.TextBox textBoxX64;
    }
}

