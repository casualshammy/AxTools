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
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager();
            this.num_clicker_interval = new System.Windows.Forms.NumericUpDown();
            this.comboBoxClickerKey = new MetroFramework.Controls.MetroComboBox();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel8 = new MetroFramework.Controls.MetroLabel();
            this.labelError = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.num_clicker_interval)).BeginInit();
            this.SuspendLayout();
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.OwnerForm = this;
            this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
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
            this.num_clicker_interval.ValueChanged += new System.EventHandler(this.num_clicker_interval_ValueChanged);
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
            this.comboBoxClickerKey.StyleManager = this.metroStyleManager1;
            this.comboBoxClickerKey.TabIndex = 58;
            this.comboBoxClickerKey.Theme = MetroFramework.MetroThemeStyle.Light;
            this.comboBoxClickerKey.SelectedIndexChanged += new System.EventHandler(this.comboBoxClickerKey_SelectedIndexChanged);
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
            this.metroLabel7.StyleManager = this.metroStyleManager1;
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
            this.metroLabel8.StyleManager = this.metroStyleManager1;
            this.metroLabel8.TabIndex = 56;
            this.metroLabel8.Text = "Key to spam:";
            this.metroLabel8.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel8.UseStyleColors = true;
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.ForeColor = System.Drawing.Color.Red;
            this.labelError.Location = new System.Drawing.Point(110, 82);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(156, 13);
            this.labelError.TabIndex = 60;
            this.labelError.Text = "Interval can\'t be less than 50ms";
            // 
            // ClickerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 100);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.num_clicker_interval);
            this.Controls.Add(this.comboBoxClickerKey);
            this.Controls.Add(this.metroLabel7);
            this.Controls.Add(this.metroLabel8);
            this.DisplayHeader = false;
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "ClickerSettings";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.StyleManager = this.metroStyleManager1;
            this.Text = "Clicker";
            ((System.ComponentModel.ISupportInitialize)(this.num_clicker_interval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Components.MetroStyleManager metroStyleManager1;
        private MetroFramework.Controls.MetroComboBox comboBoxClickerKey;
        private MetroFramework.Controls.MetroLabel metroLabel7;
        private MetroFramework.Controls.MetroLabel metroLabel8;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.NumericUpDown num_clicker_interval;
    }
}