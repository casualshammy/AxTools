using System;
using System.Windows.Forms;

namespace LibSMS
{
    public partial class SettingsForm : Form
    {
        //private static LibSMS plugin;
        private const int CP_NOCLOSE_BUTTON = 0x200;

        public SettingsForm()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        internal static void Open(LibSMS inst, Settings settingsInstance)
        {
            if (inst != null)
            {
                SettingsForm fishingConfig = new SettingsForm
                {
                    textBoxSMSAPI = {Text = settingsInstance.SMSAPI},
                    textBoxPushbulletAPIKey = {Text = settingsInstance.PushbulletAPIKey},
                    textBoxPushbulletRecipient = {Text = settingsInstance.PushbulletRecipient}
                };
                fishingConfig.ShowDialog();
                settingsInstance.SMSAPI = fishingConfig.textBoxSMSAPI.Text;
                settingsInstance.PushbulletAPIKey = fishingConfig.textBoxPushbulletAPIKey.Text;
                settingsInstance.PushbulletRecipient = fishingConfig.textBoxPushbulletRecipient.Text;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Close();
        }
    
    }
}
