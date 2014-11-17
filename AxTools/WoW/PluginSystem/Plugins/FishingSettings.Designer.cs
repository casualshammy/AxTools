namespace AxTools.WoW.PluginSystem.Plugins
{
    partial class FishingSettings
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
            this.comboBoxBait = new System.Windows.Forms.ComboBox();
            this.comboBoxSpecialBait = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBoxBait
            // 
            this.comboBoxBait.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBait.FormattingEnabled = true;
            this.comboBoxBait.Items.AddRange(new object[] {
            "",
            "Термостойкая вращающаяся наживка",
            "Королевский червяк"});
            this.comboBoxBait.Location = new System.Drawing.Point(83, 12);
            this.comboBoxBait.Name = "comboBoxBait";
            this.comboBoxBait.Size = new System.Drawing.Size(167, 21);
            this.comboBoxBait.TabIndex = 0;
            // 
            // comboBoxSpecialBait
            // 
            this.comboBoxSpecialBait.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpecialBait.FormattingEnabled = true;
            this.comboBoxSpecialBait.Items.AddRange(new object[] {
            "",
            "Наживка на глубинного угря-мешкорота",
            "Наживка для толстопузика",
            "Наживка для хлыстохвоста Черноводья",
            "Наживка для безротого скрытиуса",
            "Наживка для слепого озерного осетра",
            "Наживка для огненного аммонита",
            "Наживка для морского скорпиона"});
            this.comboBoxSpecialBait.Location = new System.Drawing.Point(83, 39);
            this.comboBoxSpecialBait.Name = "comboBoxSpecialBait";
            this.comboBoxSpecialBait.Size = new System.Drawing.Size(167, 21);
            this.comboBoxSpecialBait.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Bait:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Special bait:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(238, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Launch";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FishingSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 97);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxSpecialBait);
            this.Controls.Add(this.comboBoxBait);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FishingSettings";
            this.Text = "FishingSettings";
            this.Load += new System.EventHandler(this.FishingSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxBait;
        private System.Windows.Forms.ComboBox comboBoxSpecialBait;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}