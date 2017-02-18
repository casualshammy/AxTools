namespace InkCrafter
{
    partial class InkCrafterConfig
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
            this.labelModernInk = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxModernInk = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelModernInk
            // 
            this.labelModernInk.AutoSize = true;
            this.labelModernInk.Location = new System.Drawing.Point(12, 9);
            this.labelModernInk.Name = "labelModernInk";
            this.labelModernInk.Size = new System.Drawing.Size(179, 13);
            this.labelModernInk.TabIndex = 0;
            this.labelModernInk.Text = "How much Warbinder\'s Ink to retain:";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(15, 32);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(262, 23);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxModernInk
            // 
            this.textBoxModernInk.Location = new System.Drawing.Point(197, 6);
            this.textBoxModernInk.Name = "textBoxModernInk";
            this.textBoxModernInk.Size = new System.Drawing.Size(80, 20);
            this.textBoxModernInk.TabIndex = 6;
            this.textBoxModernInk.Text = "20";
            this.textBoxModernInk.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GoodsDestroyerConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 68);
            this.Controls.Add(this.textBoxModernInk);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.labelModernInk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GoodsDestroyerConfig";
            this.Text = "GoodsDestroyerSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelModernInk;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxModernInk;
    }
}