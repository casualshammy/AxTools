using System;
using System.Windows.Forms;

namespace PathPlayer
{
    public partial class SettingsForm : Form
    {

        public SettingsForm()
        {
            InitializeComponent();
        }

        internal static void Open(Settings settingsInstance)
        {
            SettingsForm fishingConfig = new SettingsForm
            {
                textBoxPath = { Text = settingsInstance.Path },
                checkBoxLoopPath = { Checked = settingsInstance.LoopPath },
                checkBoxStartFromNearestPoint = { Checked = settingsInstance.StartFromNearestPoint },
                checkBoxRandomJumps = { Checked = settingsInstance.RandomJumps }
            };
            fishingConfig.ShowDialog();
            settingsInstance.Path = fishingConfig.textBoxPath.Text;
            settingsInstance.LoopPath = fishingConfig.checkBoxLoopPath.Checked;
            settingsInstance.StartFromNearestPoint = fishingConfig.checkBoxStartFromNearestPoint.Checked;
            settingsInstance.RandomJumps = fishingConfig.checkBoxRandomJumps.Checked;
        }

        // ReSharper disable once InconsistentNaming
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog p = new OpenFileDialog { Filter = @"Text files (*.txt, *.json)|*.txt;*.json", InitialDirectory = Application.StartupPath + "\\pluginsSettings\\PathPlayer" })
            {
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    textBoxPath.Text = p.FileName;
                }
            }
        }
    }
}
