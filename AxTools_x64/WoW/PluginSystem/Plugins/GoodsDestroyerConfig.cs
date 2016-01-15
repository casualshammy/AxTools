﻿using System;
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
                checkBox1 = {Checked = settingsInstance.LaunchInkCrafter}
            };
            goodsDestroyerConfig.ShowDialog();
            settingsInstance.LaunchInkCrafter = goodsDestroyerConfig.checkBox1.Checked;
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

    }
}
