﻿using System;
using System.Drawing;
using System.Windows.Forms;
using AxTools.Components;
using AxTools.Properties;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class ClickerSettings : BorderedMetroForm
    {
        internal ClickerSettings()
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            metroStyleManager1.Style = Settings.NewStyleColor;
            Keys[] keys =
            {
                Keys.None, Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11,
                Keys.F12, Keys.Home, Keys.End, Keys.Insert, Keys.Delete, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5,
                Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
            };
            foreach (Keys i in keys)
            {
                comboBoxClickerKey.Items.Add(i.ToString());
            }
            comboBoxClickerKey.Text = Settings.ClickerKey.ToString();
            num_clicker_interval.Value = Settings.ClickerInterval;
            labelError.Visible = false;
            num_clicker_interval.TextChanged += num_clicker_interval_TextChanged;
            BeginInvoke((MethodInvoker)delegate
            {
                int maxWidth = Screen.PrimaryScreen.WorkingArea.Width;
                MainForm mainForm = MainForm.Instance;
                Location = maxWidth - mainForm.Location.X - mainForm.Size.Width - 20 > Size.Width
                    ? new Point(mainForm.Location.X + mainForm.Size.Width + 20, mainForm.Location.Y)
                    : new Point(mainForm.Location.X - Size.Width - 20, mainForm.Location.Y);
                OnActivated(EventArgs.Empty);
            });
        }

        private void comboBoxClickerKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(comboBoxClickerKey.Text, true, out Settings.ClickerKey);
        }

        private void num_clicker_interval_ValueChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                int interval = Convert.ToInt32(num_clicker_interval.Value);
                if (interval >= 50)
                {
                    labelError.Visible = false;
                    Settings.ClickerInterval = interval;
                }
                else
                {
                    labelError.Visible = true;
                }
            }
        }

        private void num_clicker_interval_TextChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                int interval;
                labelError.Visible = !int.TryParse(num_clicker_interval.Text, out interval) || interval < 50;
            }
        }

    }
}