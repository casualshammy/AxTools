using System;
using System.Windows.Forms;

namespace Destroyer
{
    public partial class OptionsWindow : Form
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Settings settings;

        public OptionsWindow(Settings settingsInstance)
        {
            InitializeComponent();
            settings = settingsInstance;
            checkBox1.Checked = settings.LaunchInkCrafter;
            checkBoxUseFastDraenorMill.Checked = settings.UseFastDraenorMill;
            checkBoxMillFelwort.Checked = settings.MillFelwort;
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

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            settings.LaunchInkCrafter = checkBox1.Checked;
        }

        private void checkBoxUseFastDraenorMill_CheckedChanged(object sender, EventArgs e)
        {
            settings.UseFastDraenorMill = checkBoxUseFastDraenorMill.Checked;
        }

        private void checkBoxMillFelwort_CheckedChanged(object sender, EventArgs e)
        {
            settings.MillFelwort = checkBoxMillFelwort.Checked;
        }

    }
}