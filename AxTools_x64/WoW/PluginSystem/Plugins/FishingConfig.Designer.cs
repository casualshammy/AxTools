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
            this.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable = new System.Windows.Forms.CheckBox();
            this.checkBoxGetSpecialBaitFromNatPagle = new System.Windows.Forms.CheckBox();
            this.groupBoxWOD = new System.Windows.Forms.GroupBox();
            this.groupBoxLegion = new System.Windows.Forms.GroupBox();
            this.checkBoxDalaran = new System.Windows.Forms.CheckBox();
            this.checkBoxUseArcaneLure = new System.Windows.Forms.CheckBox();
            this.groupBoxWOD.SuspendLayout();
            this.groupBoxLegion.SuspendLayout();
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
            this.comboBoxSpecialBait.Location = new System.Drawing.Point(134, 17);
            this.comboBoxSpecialBait.Name = "comboBoxSpecialBait";
            this.comboBoxSpecialBait.Size = new System.Drawing.Size(229, 21);
            this.comboBoxSpecialBait.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(12, 242);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(377, 23);
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
            this.checkBoxUseSpecialBait.Location = new System.Drawing.Point(6, 19);
            this.checkBoxUseSpecialBait.Name = "checkBoxUseSpecialBait";
            this.checkBoxUseSpecialBait.Size = new System.Drawing.Size(122, 17);
            this.checkBoxUseSpecialBait.TabIndex = 6;
            this.checkBoxUseSpecialBait.Text = "Use the special bait:";
            this.checkBoxUseSpecialBait.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable
            // 
            this.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.AutoSize = true;
            this.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.Location = new System.Drawing.Point(6, 65);
            this.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.Name = "checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable";
            this.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.Size = new System.Drawing.Size(247, 17);
            this.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.TabIndex = 7;
            this.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.Text = "Use any special bait if preferred is not available";
            this.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.UseVisualStyleBackColor = true;
            // 
            // checkBoxGetSpecialBaitFromNatPagle
            // 
            this.checkBoxGetSpecialBaitFromNatPagle.AutoSize = true;
            this.checkBoxGetSpecialBaitFromNatPagle.Location = new System.Drawing.Point(6, 42);
            this.checkBoxGetSpecialBaitFromNatPagle.Name = "checkBoxGetSpecialBaitFromNatPagle";
            this.checkBoxGetSpecialBaitFromNatPagle.Size = new System.Drawing.Size(172, 17);
            this.checkBoxGetSpecialBaitFromNatPagle.TabIndex = 8;
            this.checkBoxGetSpecialBaitFromNatPagle.Text = "Get special bait from Nat Pagle";
            this.checkBoxGetSpecialBaitFromNatPagle.UseVisualStyleBackColor = true;
            // 
            // groupBoxWOD
            // 
            this.groupBoxWOD.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxWOD.Controls.Add(this.checkBoxUseSpecialBait);
            this.groupBoxWOD.Controls.Add(this.checkBoxGetSpecialBaitFromNatPagle);
            this.groupBoxWOD.Controls.Add(this.comboBoxSpecialBait);
            this.groupBoxWOD.Controls.Add(this.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable);
            this.groupBoxWOD.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBoxWOD.Location = new System.Drawing.Point(12, 35);
            this.groupBoxWOD.Name = "groupBoxWOD";
            this.groupBoxWOD.Size = new System.Drawing.Size(377, 92);
            this.groupBoxWOD.TabIndex = 9;
            this.groupBoxWOD.TabStop = false;
            this.groupBoxWOD.Text = "Warlords of Draenor";
            // 
            // groupBoxLegion
            // 
            this.groupBoxLegion.Controls.Add(this.checkBoxDalaran);
            this.groupBoxLegion.Controls.Add(this.checkBoxUseArcaneLure);
            this.groupBoxLegion.Location = new System.Drawing.Point(12, 133);
            this.groupBoxLegion.Name = "groupBoxLegion";
            this.groupBoxLegion.Size = new System.Drawing.Size(377, 100);
            this.groupBoxLegion.TabIndex = 10;
            this.groupBoxLegion.TabStop = false;
            this.groupBoxLegion.Text = "Legion";
            // 
            // checkBoxDalaran
            // 
            this.checkBoxDalaran.AutoSize = true;
            this.checkBoxDalaran.Location = new System.Drawing.Point(6, 42);
            this.checkBoxDalaran.Name = "checkBoxDalaran";
            this.checkBoxDalaran.Size = new System.Drawing.Size(271, 17);
            this.checkBoxDalaran.TabIndex = 1;
            this.checkBoxDalaran.Text = "Do Dalaran achievement (stand near Marcia Chase)";
            this.checkBoxDalaran.UseVisualStyleBackColor = true;
            this.checkBoxDalaran.CheckedChanged += new System.EventHandler(this.checkBoxDalaran_CheckedChanged);
            // 
            // checkBoxUseArcaneLure
            // 
            this.checkBoxUseArcaneLure.AutoSize = true;
            this.checkBoxUseArcaneLure.Location = new System.Drawing.Point(6, 19);
            this.checkBoxUseArcaneLure.Name = "checkBoxUseArcaneLure";
            this.checkBoxUseArcaneLure.Size = new System.Drawing.Size(101, 17);
            this.checkBoxUseArcaneLure.TabIndex = 0;
            this.checkBoxUseArcaneLure.Text = "Use arcane lure";
            this.checkBoxUseArcaneLure.UseVisualStyleBackColor = true;
            // 
            // FishingConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 277);
            this.Controls.Add(this.groupBoxLegion);
            this.Controls.Add(this.groupBoxWOD);
            this.Controls.Add(this.checkBoxUseBestBait);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FishingConfig";
            this.Text = "FishingSettings";
            this.groupBoxWOD.ResumeLayout(false);
            this.groupBoxWOD.PerformLayout();
            this.groupBoxLegion.ResumeLayout(false);
            this.groupBoxLegion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxSpecialBait;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxUseBestBait;
        private System.Windows.Forms.CheckBox checkBoxUseSpecialBait;
        private System.Windows.Forms.CheckBox checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable;
        private System.Windows.Forms.CheckBox checkBoxGetSpecialBaitFromNatPagle;
        private System.Windows.Forms.GroupBox groupBoxWOD;
        private System.Windows.Forms.GroupBox groupBoxLegion;
        private System.Windows.Forms.CheckBox checkBoxUseArcaneLure;
        private System.Windows.Forms.CheckBox checkBoxDalaran;
    }
}