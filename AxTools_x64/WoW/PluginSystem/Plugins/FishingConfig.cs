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
            textBoxCasRodKey.Text = new KeysConverter().ConvertToInvariantString(thisSettings.KeyCastRod);
            textBoxBaitKey.Text = new KeysConverter().ConvertToInvariantString(thisSettings.KeyBait);
            textBoxWoDBaitKey.Text = new KeysConverter().ConvertToInvariantString(thisSettings.KeySpecialBait);
            textBoxCasRodKey.KeyDown += textBoxCasRodKey_KeyDown;
            textBoxBaitKey.KeyDown += textBoxBaitKey_KeyDown;
            textBoxWoDBaitKey.KeyDown += textBoxWoDBaitKey_KeyDown;
        }

        private void textBoxWoDBaitKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                Keys keys = LetOnlyOneKeyModifier(e);
                textBoxWoDBaitKey.Text = new KeysConverter().ConvertToInvariantString(keys);
                thisSettings.KeySpecialBait = keys;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxBaitKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                Keys keys = LetOnlyOneKeyModifier(e);
                textBoxBaitKey.Text = new KeysConverter().ConvertToInvariantString(keys);
                thisSettings.KeyBait = keys;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxCasRodKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                Keys keys = LetOnlyOneKeyModifier(e);
                textBoxCasRodKey.Text = new KeysConverter().ConvertToInvariantString(keys);
                thisSettings.KeyCastRod = keys;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
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
