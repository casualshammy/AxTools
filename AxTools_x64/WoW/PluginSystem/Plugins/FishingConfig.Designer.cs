namespace AxTools.WoW.PluginSystem.Plugins
{
    partial class FishingConfig
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
            this.comboBoxSpecialBait = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBoxUseBestBait = new System.Windows.Forms.CheckBox();
            this.checkBoxUseSpecialBait = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // comboBoxSpecialBait
            // 
            this.comboBoxSpecialBait.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpecialBait.FormattingEnabled = true;
            this.comboBoxSpecialBait.Items.AddRange(new object[] {
            "Наживка на глубинного угря-мешкорота",
            "Наживка для толстопузика",
            "Наживка для хлыстохвоста Черноводья",
            "Наживка для безротого скрытиуса",
            "Наживка для слепого озерного осетра",
            "Наживка для огненного аммонита",
            "Наживка для морского скорпиона"});
            this.comboBoxSpecialBait.Location = new System.Drawing.Point(140, 35);
            this.comboBoxSpecialBait.Name = "comboBoxSpecialBait";
            this.comboBoxSpecialBait.Size = new System.Drawing.Size(229, 21);
            this.comboBoxSpecialBait.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(357, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBoxUseBestBait
            // 
            this.checkBoxUseBestBait.AutoSize = true;
            this.checkBoxUseBestBait.Location = new System.Drawing.Point(12, 12);
            this.checkBoxUseBestBait.Name = "checkBoxUseBestBait";
            this.checkBoxUseBestBait.Size = new System.Drawing.Size(106, 17);
            this.checkBoxUseBestBait.TabIndex = 5;
            this.checkBoxUseBestBait.Text = "Use the best bait";
            this.checkBoxUseBestBait.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseSpecialBait
            // 
            this.checkBoxUseSpecialBait.AutoSize = true;
            this.checkBoxUseSpecialBait.Location = new System.Drawing.Point(12, 37);
            this.checkBoxUseSpecialBait.Name = "checkBoxUseSpecialBait";
            this.checkBoxUseSpecialBait.Size = new System.Drawing.Size(122, 17);
            this.checkBoxUseSpecialBait.TabIndex = 6;
            this.checkBoxUseSpecialBait.Text = "Use the special bait:";
            this.checkBoxUseSpecialBait.UseVisualStyleBackColor = true;
            // 
            // FishingConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 97);
            this.Controls.Add(this.checkBoxUseSpecialBait);
            this.Controls.Add(this.checkBoxUseBestBait);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBoxSpecialBait);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FishingConfig";
            this.Text = "FishingSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxSpecialBait;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxUseBestBait;
        private System.Windows.Forms.CheckBox checkBoxUseSpecialBait;
    }
}