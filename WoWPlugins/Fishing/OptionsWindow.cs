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
            checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.CheckedChanged += (s, args) => thisSettings.UseAnySpecialBaitIfPreferredIsNotAvailable = checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.Checked;

            checkBoxGetSpecialBaitFromNatPagle.Checked = thisSettings.GetSpecialBaitFromNatPagle;
            checkBoxGetSpecialBaitFromNatPagle.CheckedChanged += (s, args) => thisSettings.GetSpecialBaitFromNatPagle = checkBoxGetSpecialBaitFromNatPagle.Checked;

            checkBoxUseArcaneLure.Checked = thisSettings.UseArcaneLure;
            checkBoxUseArcaneLure.CheckedChanged += (s, args) => thisSettings.UseArcaneLure = checkBoxUseArcaneLure.Checked;

            checkBoxDalaran.Checked = thisSettings.DalaranAchievement;
            checkBoxDalaran.CheckedChanged += (s, args) => thisSettings.DalaranAchievement = checkBoxDalaran.Checked;

            checkBoxLegionUseSpecialLure.Checked = thisSettings.LegionUseSpecialLure;
            checkBoxLegionUseSpecialLure.CheckedChanged += (s, args) => thisSettings.LegionUseSpecialLure = checkBoxLegionUseSpecialLure.Checked;

            checkBoxLegionMargoss.Checked = thisSettings.LegionMargossSupport;
            checkBoxLegionMargoss.CheckedChanged += (s, args) => thisSettings.LegionMargossSupport = checkBoxLegionMargoss.Checked;

            checkBoxBreaks.Checked = thisSettings.EnableBreaks;
            checkBoxBreaks.CheckedChanged += (s, args) => thisSettings.EnableBreaks = checkBoxBreaks.Checked;

            checkBoxUseWaterWalking.Checked = thisSettings.UseWaterWalking;
            checkBoxUseWaterWalking.CheckedChanged += (s, args) => thisSettings.UseWaterWalking = checkBoxUseWaterWalking.Checked;

            checkBoxUseBestBait.Checked = thisSettings.UseBestBait;
            checkBoxUseBestBait.CheckedChanged += (s, args) => thisSettings.UseBestBait = checkBoxUseBestBait.Checked;

            checkBoxUseSpecialBait.Checked = fishingSettings.UseSpecialBait;
            checkBoxUseSpecialBait.CheckedChanged += (s, args) => fishingSettings.UseSpecialBait = checkBoxUseSpecialBait.Checked;

            comboBoxSpecialBait.Text = fishingSettings.SpecialBait;
            comboBoxSpecialBait.SelectedIndexChanged += (s, args) => fishingSettings.SpecialBait = comboBoxSpecialBait.Items[comboBoxSpecialBait.SelectedIndex].ToString();
        }

    }
}