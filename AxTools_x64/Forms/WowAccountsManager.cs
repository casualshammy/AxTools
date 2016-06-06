using System;
using System.Linq;
using System.Text.RegularExpressions;
using AxTools.Properties;
using AxTools.WoW;
using Components.Forms;
using Settings = AxTools.Helpers.Settings;

namespace AxTools.Forms
{
    internal partial class WowAccountsManager : BorderedMetroForm
    {
        internal WowAccountsManager()
        {
            InitializeComponent();
           StyleManager.Style = Settings.Instance.StyleColor;
            Icon = Resources.AppIcon;
            UpdateControls();
        }

        private void metroButtonWowAccountSaveUpdate_Click(object sender, EventArgs e)
        {
            WoWAccount wowAccount = WoWAccount.AllAccounts.FirstOrDefault(i => i.Login == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                int index = WoWAccount.AllAccounts.IndexOf(wowAccount);
                WoWAccount.AllAccounts[index] = new WoWAccount(textBoxWowAccountLogin.Text, textBoxWowAccountPassword.Text);
            }
            else
            {
                WoWAccount.AllAccounts.Add(new WoWAccount(textBoxWowAccountLogin.Text, textBoxWowAccountPassword.Text));
            }
            UpdateControls();
        }

        private void metroButtonWowAccountDelete_Click(object sender, EventArgs e)
        {
            WoWAccount wowAccount = WoWAccount.AllAccounts.FirstOrDefault(i => i.Login == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                WoWAccount.AllAccounts.Remove(wowAccount);
            }
            UpdateControls();
        }

        private void textBoxWowAccountLogin_TextChanged(object sender, EventArgs e)
        {
            if (WoWAccount.AllAccounts.Any(i => i.Login == textBoxWowAccountLogin.Text))
            {
                metroButtonWowAccountSaveUpdate.Text = "Update";
                metroButtonWowAccountDelete.Enabled = true;
            }
            else
            {
                metroButtonWowAccountSaveUpdate.Text = "Add";
                metroButtonWowAccountDelete.Enabled = false;
            }
            metroButtonWowAccountSaveUpdate.Enabled = textBoxWowAccountPassword.Text.Trim().Length != 0 && Regex.IsMatch(textBoxWowAccountLogin.Text, "\\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,}\\b");
        }

        private void comboBoxWowAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxWowAccounts.SelectedIndex != -1)
            {
                textBoxWowAccountLogin.Text = WoWAccount.AllAccounts[comboBoxWowAccounts.SelectedIndex].Login;
                textBoxWowAccountPassword.Text = new string('*', WoWAccount.AllAccounts[comboBoxWowAccounts.SelectedIndex].Password.Length);
            }
            
        }

        private void UpdateControls()
        {
            comboBoxWowAccounts.Items.Clear();
            foreach (WoWAccount i in WoWAccount.AllAccounts)
            {
                comboBoxWowAccounts.Items.Add(i.Login);
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }

    }
}
