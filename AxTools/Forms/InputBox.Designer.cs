namespace AxTools.Forms
{
    partial class InputBox
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
            this.button1 = new MetroFramework.Controls.MetroButton();
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager();
            this.button2 = new MetroFramework.Controls.MetroButton();
            this.textBox1 = new MetroFramework.Controls.MetroTextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Highlight = true;
            this.button1.Location = new System.Drawing.Point(153, 84);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.Style = MetroFramework.MetroColorStyle.Blue;
            this.button1.StyleManager = this.metroStyleManager1;
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.OwnerForm = this;
            this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // button2
            // 
            this.button2.Highlight = true;
            this.button2.Location = new System.Drawing.Point(234, 84);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.Style = MetroFramework.MetroColorStyle.Blue;
            this.button2.StyleManager = this.metroStyleManager1;
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // textBox1
            // 
            this.textBox1.CustomBackground = false;
            this.textBox1.CustomForeColor = false;
            this.textBox1.FontSize = MetroFramework.MetroTextBoxSize.Small;
            this.textBox1.FontWeight = MetroFramework.MetroTextBoxWeight.Regular;
            this.textBox1.Location = new System.Drawing.Point(12, 58);
            this.textBox1.Multiline = false;
            this.textBox1.Name = "textBox1";
            this.textBox1.SelectedText = "";
            this.textBox1.Size = new System.Drawing.Size(297, 20);
            this.textBox1.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBox1.StyleManager = this.metroStyleManager1;
            this.textBox1.TabIndex = 3;
            this.textBox1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBox1.UseStyleColors = true;
            // 
            // InputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 119);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "InputBox";
            this.Resizable = false;
            this.StyleManager = this.metroStyleManager1;
            this.Text = "InputBox";
            this.Load += new System.EventHandler(this.InputBoxLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroButton button1;
        private MetroFramework.Controls.MetroButton button2;
        private MetroFramework.Controls.MetroTextBox textBox1;
        private MetroFramework.Components.MetroStyleManager metroStyleManager1;
    }
}