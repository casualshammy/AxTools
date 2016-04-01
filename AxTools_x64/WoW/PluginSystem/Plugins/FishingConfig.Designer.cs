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
            this.labelCastRodKey = new System.Windows.Forms.Label();
            this.textBoxCasRodKey = new System.Windows.Forms.TextBox();
            this.buttonCastRodKey = new System.Windows.Forms.Button();
            this.buttonBaitKey = new System.Windows.Forms.Button();
            this.textBoxBaitKey = new System.Windows.Forms.TextBox();
            this.labelBaitKey = new System.Windows.Forms.Label();
            this.buttonWoDBaitKey = new System.Windows.Forms.Button();
            this.textBoxWoDBaitKey = new System.Windows.Forms.TextBox();
            this.labelWoDBaitKey = new System.Windows.Forms.Label();
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
            this.button1.Location = new System.Drawing.Point(12, 200);
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
            // labelCastRodKey
            // 
            this.labelCastRodKey.AutoSize = true;
            this.labelCastRodKey.Location = new System.Drawing.Point(12, 88);
            this.labelCastRodKey.Name = "labelCastRodKey";
            this.labelCastRodKey.Size = new System.Drawing.Size(69, 13);
            this.labelCastRodKey.TabIndex = 7;
            this.labelCastRodKey.Text = "Cast rod key:";
            // 
            // textBoxCasRodKey
            // 
            this.textBoxCasRodKey.Location = new System.Drawing.Point(129, 85);
            this.textBoxCasRodKey.Name = "textBoxCasRodKey";
            this.textBoxCasRodKey.Size = new System.Drawing.Size(100, 20);
            this.textBoxCasRodKey.TabIndex = 8;
            // 
            // buttonCastRodKey
            // 
            this.buttonCastRodKey.Location = new System.Drawing.Point(235, 85);
            this.buttonCastRodKey.Name = "buttonCastRodKey";
            this.buttonCastRodKey.Size = new System.Drawing.Size(75, 20);
            this.buttonCastRodKey.TabIndex = 9;
            this.buttonCastRodKey.Text = "Clear";
            this.buttonCastRodKey.UseVisualStyleBackColor = true;
            // 
            // buttonBaitKey
            // 
            this.buttonBaitKey.Location = new System.Drawing.Point(235, 111);
            this.buttonBaitKey.Name = "buttonBaitKey";
            this.buttonBaitKey.Size = new System.Drawing.Size(75, 20);
            this.buttonBaitKey.TabIndex = 12;
            this.buttonBaitKey.Text = "Clear";
            this.buttonBaitKey.UseVisualStyleBackColor = true;
            // 
            // textBoxBaitKey
            // 
            this.textBoxBaitKey.Location = new System.Drawing.Point(129, 111);
            this.textBoxBaitKey.Name = "textBoxBaitKey";
            this.textBoxBaitKey.Size = new System.Drawing.Size(100, 20);
            this.textBoxBaitKey.TabIndex = 11;
            // 
            // labelBaitKey
            // 
            this.labelBaitKey.AutoSize = true;
            this.labelBaitKey.Location = new System.Drawing.Point(12, 114);
            this.labelBaitKey.Name = "labelBaitKey";
            this.labelBaitKey.Size = new System.Drawing.Size(48, 13);
            this.labelBaitKey.TabIndex = 10;
            this.labelBaitKey.Text = "Bait key:";
            // 
            // buttonWoDBaitKey
            // 
            this.buttonWoDBaitKey.Location = new System.Drawing.Point(235, 137);
            this.buttonWoDBaitKey.Name = "buttonWoDBaitKey";
            this.buttonWoDBaitKey.Size = new System.Drawing.Size(75, 20);
            this.buttonWoDBaitKey.TabIndex = 15;
            this.buttonWoDBaitKey.Text = "Clear";
            this.buttonWoDBaitKey.UseVisualStyleBackColor = true;
            // 
            // textBoxWoDBaitKey
            // 
            this.textBoxWoDBaitKey.Location = new System.Drawing.Point(129, 137);
            this.textBoxWoDBaitKey.Name = "textBoxWoDBaitKey";
            this.textBoxWoDBaitKey.Size = new System.Drawing.Size(100, 20);
            this.textBoxWoDBaitKey.TabIndex = 14;
            // 
            // labelWoDBaitKey
            // 
            this.labelWoDBaitKey.AutoSize = true;
            this.labelWoDBaitKey.Location = new System.Drawing.Point(12, 140);
            this.labelWoDBaitKey.Name = "labelWoDBaitKey";
            this.labelWoDBaitKey.Size = new System.Drawing.Size(111, 13);
            this.labelWoDBaitKey.TabIndex = 13;
            this.labelWoDBaitKey.Text = "WoD special bait key:";
            // 
            // FishingConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 235);
            this.Controls.Add(this.buttonWoDBaitKey);
            this.Controls.Add(this.textBoxWoDBaitKey);
            this.Controls.Add(this.labelWoDBaitKey);
            this.Controls.Add(this.buttonBaitKey);
            this.Controls.Add(this.textBoxBaitKey);
            this.Controls.Add(this.labelBaitKey);
            this.Controls.Add(this.buttonCastRodKey);
            this.Controls.Add(this.textBoxCasRodKey);
            this.Controls.Add(this.labelCastRodKey);
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
        private System.Windows.Forms.Label labelCastRodKey;
        private System.Windows.Forms.TextBox textBoxCasRodKey;
        private System.Windows.Forms.Button buttonCastRodKey;
        private System.Windows.Forms.Button buttonBaitKey;
        private System.Windows.Forms.TextBox textBoxBaitKey;
        private System.Windows.Forms.Label labelBaitKey;
        private System.Windows.Forms.Button buttonWoDBaitKey;
        private System.Windows.Forms.TextBox textBoxWoDBaitKey;
        private System.Windows.Forms.Label labelWoDBaitKey;
    }
}