using System;
using System.Windows.Forms;

namespace Follower
{
    public partial class SettingsForm : Form
    {
        private readonly Settings _settings;

        internal SettingsForm(Settings settings)
        {
            InitializeComponent();
            _settings = settings;
            checkBoxTrain.Checked = _settings.TrainMode;
        }

        private void CheckBoxTrain_CheckedChanged(object sender, EventArgs e)
        {
            _settings.TrainMode = checkBoxTrain.Checked;
        }
    }
}
