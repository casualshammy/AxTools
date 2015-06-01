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
            metroStyleManager1.Style = Settings.Instance.StyleColor;
            UpdateControls();
        }

        private void metroButtonWowAccountSaveUpdate_Click(object sender, EventArgs e)
        {
            WoWAccount wowAccount = WoWAccount.AllAccounts.FirstOrDefault(i => i.Login == textBoxWowAccountLogin.Text);
            if (wowAccount != null)
            {
                int index = WoWAccount.AllAccounts.IndexOf(wowAccount);
                WoWAccount.AllAccounts[index] = new WoWAccount(textBoxWowAccountLogin.Text, textBoxWowAccountPassword.Text);
                // WoWAccount.AllAccounts.RemoveAt(index);                                                                             // we do so because <WoWAccount.AllAccounts>
                // WoWAccount.AllAccounts.Insert(index, new WoWAccount(textBoxWowAccountLogin.Text, textBoxWowAccountPassword.Text));  // should raise <CollectionChanged> event
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
            metroButtonWowAccountSaveUpdate.Enabled = textBoxWowAccountLogin.Text.Contains('@') && textBoxWowAccountLogin.Text.Contains('.') && textBoxWowAccountPassword.Text.Trim().Length != 0;
        }

        private void comboBoxWowAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxWowAccounts.SelectedIndex != -1)
            {
                textBoxWowAccountLogin.Text = WoWAccount.AllAccounts[comboBoxWowAccounts.SelectedIndex].Login;
                textBoxWowAccountPassword.Text = new String('*', WoWAccount.AllAccounts[comboBoxWowAccounts.SelectedIndex].Password.Length);
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
