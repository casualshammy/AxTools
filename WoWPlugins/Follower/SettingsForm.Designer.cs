namespace Follower
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
            this.numericPrecision = new System.Windows.Forms.NumericUpDown();
            this.labelPrecision = new System.Windows.Forms.Label();
            this.labelMaxDistance = new System.Windows.Forms.Label();
            this.numericMaxDistance = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericPrecision)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxDistance)).BeginInit();
            this.SuspendLayout();
            // 
            // numericPrecision
            // 
            this.numericPrecision.Location = new System.Drawing.Point(88, 12);
            this.numericPrecision.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericPrecision.Name = "numericPrecision";
            this.numericPrecision.Size = new System.Drawing.Size(53, 20);
            this.numericPrecision.TabIndex = 1;
            this.numericPrecision.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericPrecision.ValueChanged += new System.EventHandler(this.NumericPrecision_ValueChanged);
            // 
            // labelPrecision
            // 
            this.labelPrecision.AutoSize = true;
            this.labelPrecision.Location = new System.Drawing.Point(12, 14);
            this.labelPrecision.Name = "labelPrecision";
            this.labelPrecision.Size = new System.Drawing.Size(50, 13);
            this.labelPrecision.TabIndex = 2;
            this.labelPrecision.Text = "Precision";
            // 
            // labelMaxDistance
            // 
            this.labelMaxDistance.AutoSize = true;
            this.labelMaxDistance.Location = new System.Drawing.Point(12, 40);
            this.labelMaxDistance.Name = "labelMaxDistance";
            this.labelMaxDistance.Size = new System.Drawing.Size(70, 13);
            this.labelMaxDistance.TabIndex = 4;
            this.labelMaxDistance.Text = "Max distance";
            // 
            // numericMaxDistance
            // 
            this.numericMaxDistance.Location = new System.Drawing.Point(88, 38);
            this.numericMaxDistance.Name = "numericMaxDistance";
            this.numericMaxDistance.Size = new System.Drawing.Size(53, 20);
            this.numericMaxDistance.TabIndex = 3;
            this.numericMaxDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericMaxDistance.ValueChanged += new System.EventHandler(this.NumericMaxDistance_ValueChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(159, 70);
            this.Controls.Add(this.labelMaxDistance);
            this.Controls.Add(this.numericMaxDistance);
            this.Controls.Add(this.labelPrecision);
            this.Controls.Add(this.numericPrecision);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.numericPrecision)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxDistance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericPrecision;
        private System.Windows.Forms.Label labelPrecision;
        private System.Windows.Forms.Label labelMaxDistance;
        private System.Windows.Forms.NumericUpDown numericMaxDistance;
    }
}