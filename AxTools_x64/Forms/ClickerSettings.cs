﻿using System;
using System.Drawing;
using System.Windows.Forms;
using AxTools.Helpers;
using AxTools.Properties;
using Components.Forms;
using Settings2 = AxTools.Helpers.Settings2;

namespace AxTools.Forms
{
    internal partial class ClickerSettings : BorderedMetroForm
    {
        private readonly Settings2 settings = Settings2.Instance;

        internal ClickerSettings()
        {
            InitializeComponent();
           StyleManager.Style = Settings2.Instance.StyleColor;
            Icon = Resources.AppIcon;
            Keys[] keys =
            {
                Keys.None, Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11,
                Keys.F12, Keys.Home, Keys.End, Keys.Insert, Keys.Delete, Keys.Space, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5,
                Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
            };
            foreach (Keys i in keys)
            {
                comboBoxClickerKey.Items.Add(i.ToString());
            }
            comboBoxClickerKey.Text = settings.ClickerKey.ToString();
            num_clicker_interval.Value = settings.ClickerInterval;
            num_clicker_interval.TextChanged += num_clicker_interval_TextChanged;
            BeginInvoke((MethodInvoker) delegate
            {
                MainForm mainForm = MainForm.Instance;
                Location = new Point(mainForm.Location.X + mainForm.Size.Width - Size.Width, mainForm.Location.Y + mainForm.Size.Height - Size.Height);
                OnActivated(EventArgs.Empty);
            });
        }

        private void comboBoxClickerKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(comboBoxClickerKey.Text, true, out settings.ClickerKey);
        }

        private void num_clicker_interval_ValueChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                int interval = Convert.ToInt32(num_clicker_interval.Value);
                if (interval >= 50)
                {
                    ErrorProviderExt.ClearError(num_clicker_interval);
                    settings.ClickerInterval = interval;
                }
                else
                {
                    ErrorProviderExt.SetError(num_clicker_interval, "Interval can't be less than 50ms", Color.Red);
                }
            }
        }

        private void num_clicker_interval_TextChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                int interval;
                if (!int.TryParse(num_clicker_interval.Text, out interval) || interval < 50)
                {
                    ErrorProviderExt.SetError(num_clicker_interval, "Interval can't be less than 50ms", Color.Red);
                }
                else
                {
                    ErrorProviderExt.ClearError(num_clicker_interval);
                }
            }
        }

    }
}
