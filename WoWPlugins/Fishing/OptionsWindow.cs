using AxTools.Forms;
using System;
using System.Windows.Forms;

namespace Fishing
{
    internal partial class OptionsWindow : Form
    {
        private readonly Settings thisSettings;

        public OptionsWindow(Settings fishingSettings)
        {
            InitializeComponent();
            thisSettings = fishingSettings;
            checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.Checked = thisSettings.UseAnySpecialBaitIfPreferredIsNotAvailable;
            checkBoxGetSpecialBaitFromNatPagle.Checked = thisSettings.GetSpecialBaitFromNatPagle;
            checkBoxUseArcaneLure.Checked = thisSettings.UseArcaneLure;
            checkBoxDalaran.Checked = thisSettings.DalaranAchievement;
            checkBoxLegionUseSpecialLure.Checked = thisSettings.LegionUseSpecialLure;
            checkBoxLegionMargoss.Checked = thisSettings.LegionMargossSupport;
            checkBoxBreaks.Checked = thisSettings.EnableBreaks;
            checkBoxUseWaterWalking.Checked = thisSettings.UseWaterWalking;
            checkBoxUseBestBait.Checked = thisSettings.UseBestBait;
            checkBoxUseSpecialBait.Checked = fishingSettings.UseSpecialBait;
            comboBoxSpecialBait.Text = fishingSettings.SpecialBait;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
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

        private void CheckBoxDalaran_CheckedChanged(object sender, EventArgs e)
        {
            thisSettings.DalaranAchievement = checkBoxDalaran.Checked;
        }

        private void CheckBoxLegionUseSpecialLure_CheckedChanged(object sender, EventArgs e)
        {
            thisSettings.LegionUseSpecialLure = checkBoxLegionUseSpecialLure.Checked;
        }

        private void CheckBoxLegionMargoss_CheckedChanged(object sender, EventArgs e)
        {
            thisSettings.LegionMargossSupport = checkBoxLegionMargoss.Checked;
        }

        private void CheckBoxBreaks_CheckedChanged(object sender, EventArgs e)
        {
            thisSettings.EnableBreaks = checkBoxBreaks.Checked;
        }



    }
}