namespace PathPlayer
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
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.checkBoxLoopPath = new System.Windows.Forms.CheckBox();
            this.checkBoxStartFromNearestPoint = new System.Windows.Forms.CheckBox();
            this.checkBoxRandomJumps = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(12, 12);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(434, 20);
            this.textBoxPath.TabIndex = 0;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(452, 11);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(533, 12);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // checkBoxLoopPath
            // 
            this.checkBoxLoopPath.AutoSize = true;
            this.checkBoxLoopPath.Location = new System.Drawing.Point(12, 40);
            this.checkBoxLoopPath.Name = "checkBoxLoopPath";
            this.checkBoxLoopPath.Size = new System.Drawing.Size(74, 17);
            this.checkBoxLoopPath.TabIndex = 3;
            this.checkBoxLoopPath.Text = "Loop path";
            this.checkBoxLoopPath.UseVisualStyleBackColor = true;
            // 
            // checkBoxStartFromNearestPoint
            // 
            this.checkBoxStartFromNearestPoint.AutoSize = true;
            this.checkBoxStartFromNearestPoint.Location = new System.Drawing.Point(92, 40);
            this.checkBoxStartFromNearestPoint.Name = "checkBoxStartFromNearestPoint";
            this.checkBoxStartFromNearestPoint.Size = new System.Drawing.Size(135, 17);
            this.checkBoxStartFromNearestPoint.TabIndex = 4;
            this.checkBoxStartFromNearestPoint.Text = "Start from nearest point";
            this.checkBoxStartFromNearestPoint.UseVisualStyleBackColor = true;
            // 
            // checkBoxRandomJumps
            // 
            this.checkBoxRandomJumps.AutoSize = true;
            this.checkBoxRandomJumps.Location = new System.Drawing.Point(233, 40);
            this.checkBoxRandomJumps.Name = "checkBoxRandomJumps";
            this.checkBoxRandomJumps.Size = new System.Drawing.Size(212, 17);
            this.checkBoxRandomJumps.TabIndex = 5;
            this.checkBoxRandomJumps.Text = "Random jumps (NOT IMPLEMENTED!)";
            this.checkBoxRandomJumps.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 69);
            this.Controls.Add(this.checkBoxRandomJumps);
            this.Controls.Add(this.checkBoxStartFromNearestPoint);
            this.Controls.Add(this.checkBoxLoopPath);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.CheckBox checkBoxLoopPath;
        private System.Windows.Forms.CheckBox checkBoxStartFromNearestPoint;
        private System.Windows.Forms.CheckBox checkBoxRandomJumps;
    }
}