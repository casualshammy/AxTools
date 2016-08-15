using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace LibSMS
{
    public partial class SettingsForm : Form
    {
        //private static LibSMS plugin;
        private const int CP_NOCLOSE_BUTTON = 0x200;
        private Settings settings;

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
                    textBoxPushbulletRecipient = {Text = settingsInstance.PushbulletRecipient},
                    settings = settingsInstance
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

        private void buttonPBAuth_Click(object sender, EventArgs e)
        {
            textBoxPushbulletAPIKey.Text = "";
            string installkey = @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
            string entryLabel = "AxTools.exe";
            int editFlag = 11000; // IE 11
            RegistryKey existingSubKey = Registry.LocalMachine.OpenSubKey(installkey, false);
            if (existingSubKey != null)
            {
                if (existingSubKey.GetValue(entryLabel) == null)
                {
                    existingSubKey = Registry.LocalMachine.OpenSubKey(installkey, true); // writable key
                    if (existingSubKey != null)
                    {
                        existingSubKey.SetValue(entryLabel, editFlag, RegistryValueKind.DWord);
                    }
                }
            }
            new PushBulletAuth(settings).ShowDialog(this);
            textBoxPushbulletAPIKey.Text = settings.PushbulletAPIKey;
        }
    
    }
}
