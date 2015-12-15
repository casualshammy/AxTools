using System;
using System.Windows.Forms;

namespace AxTools.WoW.PluginSystem.Plugins
{
    public partial class GoodsDestroyerConfig : Form
    {
        public GoodsDestroyerConfig()
        {
            InitializeComponent();
        }

        internal static void Open(GoodsDestroyerSettings settingsInstance)
        {
            GoodsDestroyerConfig goodsDestroyerConfig = new GoodsDestroyerConfig
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
