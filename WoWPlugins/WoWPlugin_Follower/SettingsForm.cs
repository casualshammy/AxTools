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
            numericMaxDistance.Value = _settings.MaxDistance;
            numericPrecision.Value = _settings.Precision;
        }

        private void NumericPrecision_ValueChanged(object sender, EventArgs e)
        {
            _settings.Precision = (int)numericPrecision.Value;
        }

        private void NumericMaxDistance_ValueChanged(object sender, EventArgs e)
        {
            _settings.MaxDistance = (int)numericMaxDistance.Value;
        }

    }
}
