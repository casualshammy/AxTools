using System;
using System.Windows.Forms;

namespace WoWPlugin_Notifier
{
    public partial class SettingsForm : Form
    {
        private readonly Settings settings;

        public SettingsForm(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;
            checkBoxPM.Checked = settings.OnWhisper;
            checkBoxBNetPM.Checked = settings.OnBNetWhisper;
            checkBoxStaticPopup.Checked = settings.OnStaticPopup;
        }

        private void checkBoxPM_CheckedChanged(object sender, EventArgs e)
        {
            settings.OnWhisper = checkBoxPM.Checked;
        }

        private void checkBoxBNetPM_CheckedChanged(object sender, EventArgs e)
        {
            settings.OnBNetWhisper = checkBoxBNetPM.Checked;
        }

        private void checkBoxStaticPopup_CheckedChanged(object sender, EventArgs e)
        {
            settings.OnStaticPopup = checkBoxStaticPopup.Checked;
        }

    }
}
