using System.Windows.Forms;

namespace InkCrafter
{
    public partial class InkCrafterConfig : Form
    {
        public InkCrafterConfig()
        {
            InitializeComponent();
        }

        internal static void Open(InkCrafterSettings settingsInstance)
        {
            InkCrafterConfig goodsDestroyerConfig = new InkCrafterConfig
            {
                textBoxModernInk = {Text = settingsInstance.WarbindersInkCount.ToString()}
            };
            goodsDestroyerConfig.ShowDialog();
            int temp;
            if (int.TryParse(goodsDestroyerConfig.textBoxModernInk.Text, out temp))
            {
                settingsInstance.WarbindersInkCount = temp;
            }
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

        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            Close();
        }

    }
}
