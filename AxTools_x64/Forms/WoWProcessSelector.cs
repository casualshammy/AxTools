using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW;
using AxTools.WoW.PluginSystem.API;
using Components.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AxTools.Forms
{
    internal partial class WoWProcessSelector : BorderedMetroForm
    {
        private WowProcess process;
        private readonly Dictionary<int, WowProcess> processPerComboboxIndex = new Dictionary<int, WowProcess>();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static WowProcess GetWoWProcess()
        {
            if (WoWProcessManager.Processes.Count == 0)
            {
                return null;
            }
            if (WoWProcessManager.Processes.Count == 1)
            {
                return WoWProcessManager.Processes.First().Value;
            }
            WoWProcessSelector bform = new WoWProcessSelector();
            bform.ShowDialog();
            return bform.process;
        }

        private WoWProcessSelector()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            StyleManager.Style = Settings2.Instance.StyleColor;
            foreach (var i in WoWProcessManager.Processes)
            {
                var index = comboBox1.Items.Add(i.Value.IsValidBuild && new GameInterface(i.Value).IsInGame ? $"pID: {i.Key}"
                    : $"pID: {i.Key} (ERROR DETECTED)");
                processPerComboboxIndex.Add(index, i.Value);
            }
            button1.Enabled = false;
            button2.Enabled = false;
            BeginInvoke((MethodInvoker)delegate
           {
               MainWindow main = MainWindow.Instance;
               Location = new Point(main.Location.X + main.Size.Width / 2 - Size.Width / 2, main.Location.Y + main.Size.Height / 2 - Size.Height / 2);
               OnActivated(EventArgs.Empty);
           });
        }

        private void Button2Click(object sender, EventArgs e)
        {
            FLASHWINFO fi = new FLASHWINFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(FLASHWINFO)),
                hwnd = processPerComboboxIndex[comboBox1.SelectedIndex].MainWindowHandle,
                dwFlags = FlashWindowFlags.FLASHW_TRAY | FlashWindowFlags.FLASHW_TIMERNOFG
            };
            NativeMethods.FlashWindowEx(ref fi);
        }

        private void Button1Click(object sender, EventArgs e)
        {
            process = processPerComboboxIndex[comboBox1.SelectedIndex];
            Close();
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = comboBox1.Text != string.Empty;
            button2.Enabled = comboBox1.Text != string.Empty;
        }
    }
}