namespace AxTools.Forms
{
    partial class ClickerSettings
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
            this.num_clicker_interval = new System.Windows.Forms.NumericUpDown();
            this.comboBoxClickerKey = new MetroFramework.Controls.MetroComboBox();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel8 = new MetroFramework.Controls.MetroLabel();
            this.buttonClickerHotkey = new MetroFramework.Controls.MetroButton();
            this.textBoxClickerHotkey = new MetroFramework.Controls.MetroTextBox();
            this.labelClickerHotkey = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.num_clicker_interval)).BeginInit();
            this.SuspendLayout();
            // 
            // num_clicker_interval
            // 
            this.num_clicker_interval.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.num_clicker_interval.Location = new System.Drawing.Point(113, 59);
            this.num_clicker_interval.Maximum = new decimal(new int[] {
            36000000,
            0,
            0,
            0});
            this.num_clicker_interval.Name = "num_clicker_interval";
            this.num_clicker_interval.Size = new System.Drawing.Size(152, 20);
            this.num_clicker_interval.TabIndex = 59;
            this.num_clicker_interval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.num_clicker_interval.ThousandsSeparator = true;
            this.num_clicker_interval.Value = new decimal(new int[] {
            36000000,
            0,
            0,
            0});
            this.num_clicker_interval.ValueChanged += new System.EventHandler(this.Num_clicker_interval_ValueChanged);
            // 
            // comboBoxClickerKey
            // 
            this.comboBoxClickerKey.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxClickerKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxClickerKey.FontSize = MetroFramework.MetroLinkSize.Small;
            this.comboBoxClickerKey.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.comboBoxClickerKey.FormattingEnabled = true;
            this.comboBoxClickerKey.ItemHeight = 19;
            this.comboBoxClickerKey.Location = new System.Drawing.Point(113, 28);
            this.comboBoxClickerKey.Name = "comboBoxClickerKey";
            this.comboBoxClickerKey.Size = new System.Drawing.Size(152, 25);
            this.comboBoxClickerKey.Style = MetroFramework.MetroColorStyle.Blue;
            this.comboBoxClickerKey.TabIndex = 58;
            this.comboBoxClickerKey.Theme = MetroFramework.MetroThemeStyle.Light;
            this.comboBoxClickerKey.SelectedIndexChanged += new System.EventHandler(this.ComboBoxClickerKey_SelectedIndexChanged);
            // 
            // metroLabel7
            // 
            this.metroLabel7.AutoSize = true;
            this.metroLabel7.CustomBackground = false;
            this.metroLabel7.CustomForeColor = false;
            this.metroLabel7.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabel7.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel7.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel7.Location = new System.Drawing.Point(23, 58);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(83, 19);
            this.metroLabel7.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel7.TabIndex = 57;
            this.metroLabel7.Text = "Interval (ms):";
            this.metroLabel7.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel7.UseStyleColors = true;
            // 
            // metroLabel8
            // 
            this.metroLabel8.AutoSize = true;
            this.metroLabel8.CustomBackground = false;
            this.metroLabel8.CustomForeColor = false;
            this.metroLabel8.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabel8.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel8.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel8.Location = new System.Drawing.Point(23, 30);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(84, 19);
            this.metroLabel8.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel8.TabIndex = 56;
            this.metroLabel8.Text = "Key to spam:";
            this.metroLabel8.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel8.UseStyleColors = true;
            // 
            // buttonClickerHotkey
            // 
            this.buttonClickerHotkey.Highlight = true;
            this.buttonClickerHotkey.Location = new System.Drawing.Point(216, 85);
            this.buttonClickerHotkey.Name = "buttonClickerHotkey";
            this.buttonClickerHotkey.Size = new System.Drawing.Size(49, 23);
            this.buttonClickerHotkey.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonClickerHotkey.TabIndex = 62;
            this.buttonClickerHotkey.Text = "Clear";
            this.buttonClickerHotkey.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // textBoxClickerHotkey
            // 
            this.textBoxClickerHotkey.CustomBackground = false;
            this.textBoxClickerHotkey.CustomForeColor = false;
            this.textBoxClickerHotkey.FontSize = MetroFramework.MetroTextBoxSize.Small;
            this.textBoxClickerHotkey.FontWeight = MetroFramework.MetroTextBoxWeight.Regular;
            this.textBoxClickerHotkey.Location = new System.Drawing.Point(113, 85);
            this.textBoxClickerHotkey.Multiline = false;
            this.textBoxClickerHotkey.Name = "textBoxClickerHotkey";
            this.textBoxClickerHotkey.SelectedText = "";
            this.textBoxClickerHotkey.Size = new System.Drawing.Size(97, 23);
            this.textBoxClickerHotkey.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxClickerHotkey.TabIndex = 61;
            this.textBoxClickerHotkey.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxClickerHotkey.UseStyleColors = true;
            // 
            // labelClickerHotkey
            // 
            this.labelClickerHotkey.AutoSize = true;
            this.labelClickerHotkey.CustomBackground = false;
            this.labelClickerHotkey.CustomForeColor = false;
            this.labelClickerHotkey.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.labelClickerHotkey.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.labelClickerHotkey.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.labelClickerHotkey.Location = new System.Drawing.Point(23, 87);
            this.labelClickerHotkey.Name = "labelClickerHotkey";
            this.labelClickerHotkey.Size = new System.Drawing.Size(52, 19);
            this.labelClickerHotkey.Style = MetroFramework.MetroColorStyle.Blue;
            this.labelClickerHotkey.TabIndex = 60;
            this.labelClickerHotkey.Text = "Hotkey:";
            this.labelClickerHotkey.Theme = MetroFramework.MetroThemeStyle.Light;
            this.labelClickerHotkey.UseStyleColors = true;
            // 
            // ClickerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 124);
            this.Controls.Add(this.buttonClickerHotkey);
            this.Controls.Add(this.textBoxClickerHotkey);
            this.Controls.Add(this.labelClickerHotkey);
            this.Controls.Add(this.num_clicker_interval);
            this.Controls.Add(this.comboBoxClickerKey);
            this.Controls.Add(this.metroLabel7);
            this.Controls.Add(this.metroLabel8);
            this.DisplayHeader = false;
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClickerSettings";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.Text = "Clicker";
            ((System.ComponentModel.ISupportInitialize)(this.num_clicker_interval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroComboBox comboBoxClickerKey;
        private MetroFramework.Controls.MetroLabel metroLabel7;
        private MetroFramework.Controls.MetroLabel metroLabel8;
        private System.Windows.Forms.NumericUpDown num_clicker_interval;
        private MetroFramework.Controls.MetroButton buttonClickerHotkey;
        private MetroFramework.Controls.MetroTextBox textBoxClickerHotkey;
        private MetroFramework.Controls.MetroLabel labelClickerHotkey;
    }
}