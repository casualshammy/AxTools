using Components;
using MetroFramework.Controls;

namespace AxTools.Forms
{
    partial class WowAccountsManager
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
            this.components = new System.ComponentModel.Container();
            this.metroButtonWowAccountDelete = new MetroFramework.Controls.MetroButton();
            this.metroButtonWowAccountSaveUpdate = new MetroFramework.Controls.MetroButton();
            this.textBoxWowAccountPassword = new MetroFramework.Controls.MetroTextBox();
            this.textBoxWowAccountLogin = new MetroFramework.Controls.MetroTextBox();
            this.comboBoxWowAccounts = new MetroFramework.Controls.MetroComboBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // metroButtonWowAccountDelete
            // 
            this.metroButtonWowAccountDelete.Enabled = false;
            this.metroButtonWowAccountDelete.Highlight = true;
            this.metroButtonWowAccountDelete.Location = new System.Drawing.Point(332, 133);
            this.metroButtonWowAccountDelete.Name = "metroButtonWowAccountDelete";
            this.metroButtonWowAccountDelete.Size = new System.Drawing.Size(106, 23);
            this.metroButtonWowAccountDelete.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroButtonWowAccountDelete.TabIndex = 43;
            this.metroButtonWowAccountDelete.Text = "Delete";
            this.metroButtonWowAccountDelete.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroButtonWowAccountDelete.Click += new System.EventHandler(this.metroButtonWowAccountDelete_Click);
            // 
            // metroButtonWowAccountSaveUpdate
            // 
            this.metroButtonWowAccountSaveUpdate.Enabled = false;
            this.metroButtonWowAccountSaveUpdate.Highlight = true;
            this.metroButtonWowAccountSaveUpdate.Location = new System.Drawing.Point(332, 104);
            this.metroButtonWowAccountSaveUpdate.Name = "metroButtonWowAccountSaveUpdate";
            this.metroButtonWowAccountSaveUpdate.Size = new System.Drawing.Size(106, 23);
            this.metroButtonWowAccountSaveUpdate.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroButtonWowAccountSaveUpdate.TabIndex = 42;
            this.metroButtonWowAccountSaveUpdate.Text = "Add";
            this.metroButtonWowAccountSaveUpdate.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroButtonWowAccountSaveUpdate.Click += new System.EventHandler(this.metroButtonWowAccountSaveUpdate_Click);
            // 
            // textBoxWowAccountPassword
            // 
            this.textBoxWowAccountPassword.CustomBackground = false;
            this.textBoxWowAccountPassword.CustomForeColor = false;
            this.textBoxWowAccountPassword.FontSize = MetroFramework.MetroTextBoxSize.Small;
            this.textBoxWowAccountPassword.FontWeight = MetroFramework.MetroTextBoxWeight.Regular;
            this.textBoxWowAccountPassword.Location = new System.Drawing.Point(23, 133);
            this.textBoxWowAccountPassword.Multiline = false;
            this.textBoxWowAccountPassword.Name = "textBoxWowAccountPassword";
            this.textBoxWowAccountPassword.SelectedText = "";
            this.textBoxWowAccountPassword.Size = new System.Drawing.Size(303, 23);
            this.textBoxWowAccountPassword.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxWowAccountPassword.TabIndex = 41;
            this.textBoxWowAccountPassword.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxWowAccountPassword.UseStyleColors = true;
            // 
            // textBoxWowAccountLogin
            // 
            this.textBoxWowAccountLogin.CustomBackground = false;
            this.textBoxWowAccountLogin.CustomForeColor = false;
            this.textBoxWowAccountLogin.FontSize = MetroFramework.MetroTextBoxSize.Small;
            this.textBoxWowAccountLogin.FontWeight = MetroFramework.MetroTextBoxWeight.Regular;
            this.textBoxWowAccountLogin.Location = new System.Drawing.Point(23, 104);
            this.textBoxWowAccountLogin.Multiline = false;
            this.textBoxWowAccountLogin.Name = "textBoxWowAccountLogin";
            this.textBoxWowAccountLogin.SelectedText = "";
            this.textBoxWowAccountLogin.Size = new System.Drawing.Size(303, 23);
            this.textBoxWowAccountLogin.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxWowAccountLogin.TabIndex = 40;
            this.textBoxWowAccountLogin.Text = "Login";
            this.textBoxWowAccountLogin.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxWowAccountLogin.UseStyleColors = true;
            this.textBoxWowAccountLogin.TextChanged += new System.EventHandler(this.textBoxWowAccountLogin_TextChanged);
            // 
            // comboBoxWowAccounts
            // 
            this.comboBoxWowAccounts.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxWowAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWowAccounts.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.comboBoxWowAccounts.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.comboBoxWowAccounts.FormattingEnabled = true;
            this.comboBoxWowAccounts.ItemHeight = 23;
            this.comboBoxWowAccounts.Location = new System.Drawing.Point(23, 52);
            this.comboBoxWowAccounts.Name = "comboBoxWowAccounts";
            this.comboBoxWowAccounts.Size = new System.Drawing.Size(415, 29);
            this.comboBoxWowAccounts.Style = MetroFramework.MetroColorStyle.Blue;
            this.comboBoxWowAccounts.TabIndex = 39;
            this.comboBoxWowAccounts.Theme = MetroFramework.MetroThemeStyle.Light;
            this.comboBoxWowAccounts.SelectedIndexChanged += new System.EventHandler(this.comboBoxWowAccounts_SelectedIndexChanged);
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.CustomBackground = false;
            this.metroLabel3.CustomForeColor = false;
            this.metroLabel3.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabel3.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel3.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel3.Location = new System.Drawing.Point(23, 84);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(185, 19);
            this.metroLabel3.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel3.TabIndex = 38;
            this.metroLabel3.Text = "...or just enter a new one here:";
            this.metroLabel3.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel3.UseStyleColors = true;
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.CustomBackground = false;
            this.metroLabel4.CustomForeColor = false;
            this.metroLabel4.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabel4.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel4.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel4.Location = new System.Drawing.Point(23, 30);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(161, 19);
            this.metroLabel4.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel4.TabIndex = 37;
            this.metroLabel4.Text = "Select an account to edit...";
            this.metroLabel4.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel4.UseStyleColors = true;
            // 
            // WowAccountsManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 175);
            this.Controls.Add(this.metroButtonWowAccountDelete);
            this.Controls.Add(this.metroButtonWowAccountSaveUpdate);
            this.Controls.Add(this.textBoxWowAccountPassword);
            this.Controls.Add(this.textBoxWowAccountLogin);
            this.Controls.Add(this.comboBoxWowAccounts);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.metroLabel4);
            this.DisplayHeader = false;
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.Name = "WowAccountsManager";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.Text = "WowAccounts";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroButton metroButtonWowAccountDelete;
        private MetroFramework.Controls.MetroButton metroButtonWowAccountSaveUpdate;
        private MetroTextBox textBoxWowAccountPassword;
        private MetroFramework.Controls.MetroTextBox textBoxWowAccountLogin;
        private MetroFramework.Controls.MetroComboBox comboBoxWowAccounts;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel metroLabel4;
    }
}