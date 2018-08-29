namespace Destroyer
{
    partial class OptionsWindow
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
            this.buttonSave = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBoxUseFastDraenorMill = new System.Windows.Forms.CheckBox();
            this.checkBoxMillFelwort = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(12, 89);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(291, 23);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Close";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 12);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(291, 17);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "Launch InkCrafter plugin when player hasn\'t herbs to mill";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBoxUseFastDraenorMill
            // 
            this.checkBoxUseFastDraenorMill.AutoSize = true;
            this.checkBoxUseFastDraenorMill.Location = new System.Drawing.Point(12, 35);
            this.checkBoxUseFastDraenorMill.Name = "checkBoxUseFastDraenorMill";
            this.checkBoxUseFastDraenorMill.Size = new System.Drawing.Size(89, 17);
            this.checkBoxUseFastDraenorMill.TabIndex = 8;
            this.checkBoxUseFastDraenorMill.Text = "Use mass mill";
            this.checkBoxUseFastDraenorMill.UseVisualStyleBackColor = true;
            this.checkBoxUseFastDraenorMill.CheckedChanged += new System.EventHandler(this.checkBoxUseFastDraenorMill_CheckedChanged);
            // 
            // checkBoxMillFelwort
            // 
            this.checkBoxMillFelwort.AutoSize = true;
            this.checkBoxMillFelwort.Location = new System.Drawing.Point(12, 58);
            this.checkBoxMillFelwort.Name = "checkBoxMillFelwort";
            this.checkBoxMillFelwort.Size = new System.Drawing.Size(78, 17);
            this.checkBoxMillFelwort.TabIndex = 9;
            this.checkBoxMillFelwort.Text = "Mill Felwort";
            this.checkBoxMillFelwort.UseVisualStyleBackColor = true;
            this.checkBoxMillFelwort.CheckedChanged += new System.EventHandler(this.checkBoxMillFelwort_CheckedChanged);
            // 
            // OptionsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 124);
            this.Controls.Add(this.checkBoxMillFelwort);
            this.Controls.Add(this.checkBoxUseFastDraenorMill);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.buttonSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsWindow";
            this.Text = "GoodsDestroyerSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBoxUseFastDraenorMill;
        private System.Windows.Forms.CheckBox checkBoxMillFelwort;
    }
}