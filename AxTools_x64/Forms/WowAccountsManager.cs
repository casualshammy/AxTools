using AxTools.WoW;
using Components.Forms;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Settings2 = AxTools.Helpers.Settings2;

namespace AxTools.Forms
{
    internal partial class WowAccountsManager : BorderedMetroForm
    {
        internal WowAccountsManager()
        {
            InitializeComponent();
            StyleManager.Style = Settings2.Instance.StyleColor;
            Icon = AxTools.Helpers.Resources.ApplicationIcon;
            UpdateControls();
        }

        private void MetroButtonWowAccountSaveUpdate_Click(object sender, EventArgs e)
        {
            WoWAccount2 wowAccount = WoWAccount2.AllAccounts.FirstOrDefault(i => i.GetLogin() == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                var index = WoWAccount2.AllAccounts.IndexOf(wowAccount);
                WoWAccount2.AllAccounts[index] = new WoWAccount2() { EncryptedLogin = WoWAccount2.GetEncryptedArray(textBoxWowAccountLogin.Text), EncryptedPassword = WoWAccount2.GetEncryptedArray(textBoxWowAccountPassword.Text) };
            }
            else
            {
                WoWAccount2.AllAccounts.Add(new WoWAccount2() { EncryptedLogin = WoWAccount2.GetEncryptedArray(textBoxWowAccountLogin.Text), EncryptedPassword = WoWAccount2.GetEncryptedArray(textBoxWowAccountPassword.Text) });
            }
            UpdateControls();
        }

        private void MetroButtonWowAccountDelete_Click(object sender, EventArgs e)
        {
            WoWAccount2 wowAccount = WoWAccount2.AllAccounts.FirstOrDefault(i => i.GetLogin() == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                WoWAccount2.AllAccounts.Remove(wowAccount);
            }
            UpdateControls();
        }

        private void TextBoxWowAccountLogin_TextChanged(object sender, EventArgs e)
        {
            if (WoWAccount2.AllAccounts.Any(i => i.GetLogin() == textBoxWowAccountLogin.Text))
            {
                metroButtonWowAccountSaveUpdate.Text = "Update";
                metroButtonWowAccountDelete.Enabled = true;
            }
            else
            {
                metroButtonWowAccountSaveUpdate.Text = "Add";
                metroButtonWowAccountDelete.Enabled = false;
            }
            metroButtonWowAccountSaveUpdate.Enabled = textBoxWowAccountPassword.Text.Trim().Length != 0 && Regex.IsMatch(textBoxWowAccountLogin.Text, "\\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,}\\b", RegexOptions.IgnoreCase);
        }

        private void ComboBoxWowAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxWowAccounts.SelectedIndex != -1)
            {
                textBoxWowAccountLogin.Text = WoWAccount2.AllAccounts[comboBoxWowAccounts.SelectedIndex].GetLogin();
                textBoxWowAccountPassword.Text = "********";
            }
        }

        private void UpdateControls()
        {
            comboBoxWowAccounts.Items.Clear();
            foreach (WoWAccount2 i in WoWAccount2.AllAccounts)
            {
                comboBoxWowAccounts.Items.Add(i.GetLogin());
            }
            textBoxWowAccountLogin.Text = "Login";
            textBoxWowAccountPassword.Text = "Password";
        }
    }
}