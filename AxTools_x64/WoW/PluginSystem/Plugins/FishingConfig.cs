using System;
using System.Windows.Forms;
using AxTools.Forms;

namespace AxTools.WoW.PluginSystem.Plugins
{
    internal partial class FishingConfig : Form
    {
        public FishingConfig()
        {
            InitializeComponent();
        }

        internal static void Open(Fishing fishing)
        {
            if (fishing != null)
            {
                fishing.LoadSettings();
                FishingConfig fishingConfig = new FishingConfig
                {
                    checkBoxUseBestBait = {Checked = fishing.fishingSettings.UseBestBait},
                    checkBoxUseSpecialBait = {Checked = fishing.fishingSettings.UseSpecialBait},
                    comboBoxSpecialBait = {Text = fishing.fishingSettings.SpecialBait}
                };
                fishingConfig.ShowDialog(MainForm.Instance);
                fishing.fishingSettings.UseBestBait = fishingConfig.checkBoxUseBestBait.Checked;
                fishing.fishingSettings.UseSpecialBait = fishingConfig.checkBoxUseSpecialBait.Checked;
                fishing.fishingSettings.SpecialBait = fishingConfig.comboBoxSpecialBait.Text;
                fishing.SaveSettings();
            }
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

    }
}
