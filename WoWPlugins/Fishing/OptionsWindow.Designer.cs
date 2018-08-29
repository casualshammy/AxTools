namespace Fishing
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
            this.comboBoxSpecialBait = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBoxUseBestBait = new System.Windows.Forms.CheckBox();
            this.checkBoxUseSpecialBait = new System.Windows.Forms.CheckBox();
            this.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable = new System.Windows.Forms.CheckBox();
            this.checkBoxGetSpecialBaitFromNatPagle = new System.Windows.Forms.CheckBox();
            this.groupBoxWOD = new System.Windows.Forms.GroupBox();
            this.groupBoxLegion = new System.Windows.Forms.GroupBox();
            this.checkBoxLegionMargoss = new System.Windows.Forms.CheckBox();
            this.checkBoxLegionUseSpecialLure = new System.Windows.Forms.CheckBox();
            this.checkBoxDalaran = new System.Windows.Forms.CheckBox();
            this.checkBoxUseArcaneLure = new System.Windows.Forms.CheckBox();
            this.checkBoxBreaks = new System.Windows.Forms.CheckBox();
            this.checkBoxUseWaterWalking = new System.Windows.Forms.CheckBox();
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
            this.button1.Location = new System.Drawing.Point(12, 342);
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
            this.groupBoxWOD.Location = new System.Drawing.Point(12, 114);
            this.groupBoxWOD.Name = "groupBoxWOD";
            this.groupBoxWOD.Size = new System.Drawing.Size(377, 92);
            this.groupBoxWOD.TabIndex = 9;
            this.groupBoxWOD.TabStop = false;
            this.groupBoxWOD.Text = "Warlords of Draenor";
            // 
            // groupBoxLegion
            // 
            this.groupBoxLegion.Controls.Add(this.checkBoxLegionMargoss);
            this.groupBoxLegion.Controls.Add(this.checkBoxLegionUseSpecialLure);
            this.groupBoxLegion.Controls.Add(this.checkBoxDalaran);
            this.groupBoxLegion.Controls.Add(this.checkBoxUseArcaneLure);
            this.groupBoxLegion.Location = new System.Drawing.Point(12, 212);
            this.groupBoxLegion.Name = "groupBoxLegion";
            this.groupBoxLegion.Size = new System.Drawing.Size(377, 124);
            this.groupBoxLegion.TabIndex = 10;
            this.groupBoxLegion.TabStop = false;
            this.groupBoxLegion.Text = "Legion";
            // 
            // checkBoxLegionMargoss
            // 
            this.checkBoxLegionMargoss.AutoSize = true;
            this.checkBoxLegionMargoss.Location = new System.Drawing.Point(6, 88);
            this.checkBoxLegionMargoss.Name = "checkBoxLegionMargoss";
            this.checkBoxLegionMargoss.Size = new System.Drawing.Size(102, 17);
            this.checkBoxLegionMargoss.TabIndex = 3;
            this.checkBoxLegionMargoss.Text = "Farm Legion rep";
            this.checkBoxLegionMargoss.UseVisualStyleBackColor = true;
            this.checkBoxLegionMargoss.CheckedChanged += new System.EventHandler(this.CheckBoxLegionMargoss_CheckedChanged);
            // 
            // checkBoxLegionUseSpecialLure
            // 
            this.checkBoxLegionUseSpecialLure.AutoSize = true;
            this.checkBoxLegionUseSpecialLure.Location = new System.Drawing.Point(6, 42);
            this.checkBoxLegionUseSpecialLure.Name = "checkBoxLegionUseSpecialLure";
            this.checkBoxLegionUseSpecialLure.Size = new System.Drawing.Size(101, 17);
            this.checkBoxLegionUseSpecialLure.TabIndex = 2;
            this.checkBoxLegionUseSpecialLure.Text = "Use special lure";
            this.checkBoxLegionUseSpecialLure.UseVisualStyleBackColor = true;
            this.checkBoxLegionUseSpecialLure.CheckedChanged += new System.EventHandler(this.CheckBoxLegionUseSpecialLure_CheckedChanged);
            // 
            // checkBoxDalaran
            // 
            this.checkBoxDalaran.AutoSize = true;
            this.checkBoxDalaran.Location = new System.Drawing.Point(6, 65);
            this.checkBoxDalaran.Name = "checkBoxDalaran";
            this.checkBoxDalaran.Size = new System.Drawing.Size(271, 17);
            this.checkBoxDalaran.TabIndex = 1;
            this.checkBoxDalaran.Text = "Do Dalaran achievement (stand near Marcia Chase)";
            this.checkBoxDalaran.UseVisualStyleBackColor = true;
            this.checkBoxDalaran.CheckedChanged += new System.EventHandler(this.CheckBoxDalaran_CheckedChanged);
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
            // checkBoxBreaks
            // 
            this.checkBoxBreaks.AutoSize = true;
            this.checkBoxBreaks.Location = new System.Drawing.Point(12, 35);
            this.checkBoxBreaks.Name = "checkBoxBreaks";
            this.checkBoxBreaks.Size = new System.Drawing.Size(126, 17);
            this.checkBoxBreaks.TabIndex = 11;
            this.checkBoxBreaks.Text = "Make random breaks";
            this.checkBoxBreaks.UseVisualStyleBackColor = true;
            this.checkBoxBreaks.CheckedChanged += new System.EventHandler(this.CheckBoxBreaks_CheckedChanged);
            // 
            // checkBoxUseWaterWalking
            // 
            this.checkBoxUseWaterWalking.AutoSize = true;
            this.checkBoxUseWaterWalking.Location = new System.Drawing.Point(12, 58);
            this.checkBoxUseWaterWalking.Name = "checkBoxUseWaterWalking";
            this.checkBoxUseWaterWalking.Size = new System.Drawing.Size(113, 17);
            this.checkBoxUseWaterWalking.TabIndex = 12;
            this.checkBoxUseWaterWalking.Text = "Use water walking";
            this.checkBoxUseWaterWalking.UseVisualStyleBackColor = true;
            // 
            // FishingConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 377);
            this.Controls.Add(this.checkBoxUseWaterWalking);
            this.Controls.Add(this.checkBoxBreaks);
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
        private System.Windows.Forms.CheckBox checkBoxLegionUseSpecialLure;
        private System.Windows.Forms.CheckBox checkBoxLegionMargoss;
        private System.Windows.Forms.CheckBox checkBoxBreaks;
        private System.Windows.Forms.CheckBox checkBoxUseWaterWalking;
    }
}