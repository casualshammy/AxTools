using System;
using System.Collections.Generic;
using System.Linq;
using AxTools.Classes.WoW;
using AxTools.Components;
using AxTools.Properties;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class WowAccountsManager : BorderedMetroForm
    {
        internal WowAccountsManager(List<WowAccount> listOfWowAccounts)
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            metroStyleManager1.Style = Settings.NewStyleColor;
            wowAccounts = listOfWowAccounts;
            comboBoxWowAccounts.Items.Clear();
            foreach (WowAccount i in wowAccounts)
            {
                comboBoxWowAccounts.Items.Add(i.Login);
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }

        private readonly List<WowAccount> wowAccounts;

        private void metroButtonWowAccountSaveUpdate_Click(object sender, EventArgs e)
        {
            WowAccount wowAccount = wowAccounts.FirstOrDefault(i => i.Login == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                wowAccount.Password = textBoxWowAccountPassword.Text;
            }
            else
            {
                wowAccounts.Add(new WowAccount(textBoxWowAccountLogin.Text, textBoxWowAccountPassword.Text));
            }
            comboBoxWowAccounts.Items.Clear();
            foreach (WowAccount i in wowAccounts)
            {
                comboBoxWowAccounts.Items.Add(i.Login);
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }

        private void metroButtonWowAccountDelete_Click(object sender, EventArgs e)
        {
            WowAccount wowAccount = wowAccounts.FirstOrDefault(i => i.Login == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                wowAccounts.Remove(wowAccount);
            }
            comboBoxWowAccounts.Items.Clear();
            foreach (WowAccount i in wowAccounts)
            {
                comboBoxWowAccounts.Items.Add(i.Login);
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }

        private void textBoxWowAccountLogin_TextChanged(object sender, EventArgs e)
        {
            if (wowAccounts.Any(i => i.Login == textBoxWowAccountLogin.Text))
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
                textBoxWowAccountLogin.Text = wowAccounts[comboBoxWowAccounts.SelectedIndex].Login;
                textBoxWowAccountPassword.Text = new String('*', wowAccounts[comboBoxWowAccounts.SelectedIndex].Password.Length);
            }
            
        }

    }
}
