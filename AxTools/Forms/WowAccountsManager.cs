using System;
using System.Linq;
using AxTools.Components;
using AxTools.Properties;
using AxTools.WoW;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class WowAccountsManager : BorderedMetroForm
    {
        internal WowAccountsManager()
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            metroStyleManager1.Style = Settings.NewStyleColor;
            comboBoxWowAccounts.Items.Clear();
            foreach (WowAccount i in WowAccount.GetAccounts())
            {
                comboBoxWowAccounts.Items.Add(i.Login);
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }

        private void metroButtonWowAccountSaveUpdate_Click(object sender, EventArgs e)
        {
            WowAccount wowAccount = WowAccount.GetAccounts().FirstOrDefault(i => i.Login == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                wowAccount.Password = textBoxWowAccountPassword.Text;
            }
            else
            {
                WowAccount.GetAccounts().Add(new WowAccount(textBoxWowAccountLogin.Text, textBoxWowAccountPassword.Text));
            }
            comboBoxWowAccounts.Items.Clear();
            foreach (WowAccount i in WowAccount.GetAccounts())
            {
                comboBoxWowAccounts.Items.Add(i.Login);
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }

        private void metroButtonWowAccountDelete_Click(object sender, EventArgs e)
        {
            WowAccount wowAccount = WowAccount.GetAccounts().FirstOrDefault(i => i.Login == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                WowAccount.GetAccounts().Remove(wowAccount);
            }
            comboBoxWowAccounts.Items.Clear();
            foreach (WowAccount i in WowAccount.GetAccounts())
            {
                comboBoxWowAccounts.Items.Add(i.Login);
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }

        private void textBoxWowAccountLogin_TextChanged(object sender, EventArgs e)
        {
            if (WowAccount.GetAccounts().Any(i => i.Login == textBoxWowAccountLogin.Text))
            {
                metroButtonWowAccountSaveUpdate.Text = "Update";
                metroButtonWowAccountDelete.Enabled = true;
            }
            else
            {
                metroButtonWowAccountSaveUpdate.Text = "Add";
                metroButtonWowAccountDelete.Enabled = false;
            }
            metroButtonWowAccountSaveUpdate.Enabled = textBoxWowAccountLogin.Text.Contains('@') && textBoxWowAccountLogin.Text.Contains('.') && textBoxWowAccountPassword.Text.Trim().Length != 0;
        }

        private void comboBoxWowAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxWowAccounts.SelectedIndex != -1)
            {
                textBoxWowAccountLogin.Text = WowAccount.GetAccounts()[comboBoxWowAccounts.SelectedIndex].Login;
                textBoxWowAccountPassword.Text = new String('*', WowAccount.GetAccounts()[comboBoxWowAccounts.SelectedIndex].Password.Length);
            }
            
        }

    }
}
