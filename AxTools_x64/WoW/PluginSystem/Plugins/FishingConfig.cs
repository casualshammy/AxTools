using System;
using System.Windows.Forms;
using AxTools.Forms;

namespace AxTools.WoW.PluginSystem.Plugins
{
    internal partial class FishingConfig : Form
    {
        private readonly FishingSettings thisSettings;

        public FishingConfig(FishingSettings fishingSettings)
        {
            InitializeComponent();
            thisSettings = fishingSettings;
            checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.Checked = thisSettings.UseAnySpecialBaitIfPreferredIsNotAvailable;
            checkBoxGetSpecialBaitFromNatPagle.Checked = thisSettings.GetSpecialBaitFromNatPagle;
        }

        internal static void Open(FishingSettings fishingSettings)
        {
            FishingConfig fishingConfig = new FishingConfig(fishingSettings)
            {
                checkBoxUseBestBait = {Checked = fishingSettings.UseBestBait},
                checkBoxUseSpecialBait = {Checked = fishingSettings.UseSpecialBait},
                comboBoxSpecialBait = {Text = fishingSettings.SpecialBait}
            };
            fishingConfig.ShowDialog(MainForm.Instance);
            fishingSettings.UseBestBait = fishingConfig.checkBoxUseBestBait.Checked;
            fishingSettings.UseSpecialBait = fishingConfig.checkBoxUseSpecialBait.Checked;
            fishingSettings.SpecialBait = fishingConfig.comboBoxSpecialBait.Text;
            fishingSettings.UseAnySpecialBaitIfPreferredIsNotAvailable = fishingConfig.checkBoxUseAnySpecialBaitIfPreferredIsNotAvailable.Checked;
            fishingSettings.GetSpecialBaitFromNatPagle = fishingConfig.checkBoxGetSpecialBaitFromNatPagle.Checked;
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

        private Keys LetOnlyOneKeyModifier(KeyEventArgs args)
        {
            switch (args.Modifiers)
            {
                case Keys.Shift | Keys.Control | Keys.Alt:
                    return args.KeyData & ~Keys.Control & ~Keys.Alt;
                case Keys.Shift | Keys.Control:
                    return args.KeyData & ~Keys.Control;
                case Keys.Shift | Keys.Alt:
                    return args.KeyData & ~Keys.Alt;
                case Keys.Control | Keys.Alt:
                    return args.KeyData & ~Keys.Alt;
                default:
                    return args.KeyData;
            }
        }

    }
}
