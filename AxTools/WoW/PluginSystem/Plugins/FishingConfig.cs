using System;
using System.Linq;
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

        internal static void Open()
        {
            Fishing fishing = PluginManager.Plugins.FirstOrDefault(i => i.GetType() == typeof(Fishing)) as Fishing;
            if (fishing != null)
            {
                fishing.LoadSettings();
                FishingConfig fishingConfig = new FishingConfig {comboBoxBait = {Text = fishing.Baits.Bait}, comboBoxSpecialBait = {Text = fishing.Baits.SpecialBait}};
                fishingConfig.ShowDialog(MainForm.Instance);
                fishing.Baits.Bait = fishingConfig.comboBoxBait.Text;
                fishing.Baits.SpecialBait = fishingConfig.comboBoxSpecialBait.Text;
                fishing.SaveSettings();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
