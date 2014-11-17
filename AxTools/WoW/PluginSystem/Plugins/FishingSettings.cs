using System;
using System.Linq;
using System.Windows.Forms;
using AxTools.Forms;

namespace AxTools.WoW.PluginSystem.Plugins
{
    internal partial class FishingSettings : Form
    {
        public FishingSettings()
        {
            InitializeComponent();
        }

        internal static void InitializeBait(out string bait, out string specialBait)
        {
            MainForm.Instance.Activate();
            FishingSettings fishingSettings = new FishingSettings();
            fishingSettings.ShowDialog(MainForm.Instance);
            bait = fishingSettings.comboBoxBait.Text;
            specialBait = fishingSettings.comboBoxSpecialBait.Text;
        }

        private void FishingSettings_Load(object sender, EventArgs e)
        {
            Fishing fishing = PluginManager.Plugins.FirstOrDefault(i => i.GetType() == typeof (Fishing)) as Fishing;
            if (fishing != null)
            {
                comboBoxBait.Text = fishing.Bait;
                comboBoxSpecialBait.Text = fishing.SpecialBait;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
