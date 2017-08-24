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
            this.checkBoxTrain = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBoxTrain
            // 
            this.checkBoxTrain.AutoSize = true;
            this.checkBoxTrain.Location = new System.Drawing.Point(12, 12);
            this.checkBoxTrain.Name = "checkBoxTrain";
            this.checkBoxTrain.Size = new System.Drawing.Size(79, 17);
            this.checkBoxTrain.TabIndex = 0;
            this.checkBoxTrain.Text = "Train mode";
            this.checkBoxTrain.UseVisualStyleBackColor = true;
            this.checkBoxTrain.CheckedChanged += new System.EventHandler(this.CheckBoxTrain_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(116, 39);
            this.Controls.Add(this.checkBoxTrain);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxTrain;
    }
}