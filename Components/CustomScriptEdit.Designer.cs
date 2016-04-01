namespace Components
{
    partial class CustomScriptEdit
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
            this.customScriptTextBox = new MetroFramework.Controls.MetroTextBox();
            this.closeButton = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // customScriptTextBox
            // 
            this.customScriptTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customScriptTextBox.CustomBackground = false;
            this.customScriptTextBox.CustomForeColor = false;
            this.customScriptTextBox.FontSize = MetroFramework.MetroTextBoxSize.Small;
            this.customScriptTextBox.FontWeight = MetroFramework.MetroTextBoxWeight.Regular;
            this.customScriptTextBox.Location = new System.Drawing.Point(23, 33);
            this.customScriptTextBox.Multiline = true;
            this.customScriptTextBox.Name = "customScriptTextBox";
            this.customScriptTextBox.SelectedText = "";
            this.customScriptTextBox.Size = new System.Drawing.Size(437, 153);
            this.customScriptTextBox.Style = MetroFramework.MetroColorStyle.Blue;
            this.customScriptTextBox.StyleManager = null;
            this.customScriptTextBox.TabIndex = 0;
            this.customScriptTextBox.Theme = MetroFramework.MetroThemeStyle.Light;
            this.customScriptTextBox.UseStyleColors = true;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.closeButton.Highlight = true;
            this.closeButton.Location = new System.Drawing.Point(168, 192);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(148, 23);
            this.closeButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.closeButton.StyleManager = null;
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Save && Close";
            this.closeButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // CustomScriptEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 233);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.customScriptTextBox);
            this.DisplayHeader = false;
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(483, 233);
            this.Name = "CustomScriptEdit";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Text = "CustomScriptEdit";
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox customScriptTextBox;
        private MetroFramework.Controls.MetroButton closeButton;
    }
}