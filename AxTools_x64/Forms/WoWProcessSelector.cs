using Components;
using AxTools.WinAPI;
using AxTools.WoW;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AxTools.Helpers;
using AxTools.WoW.PluginSystem.API;

namespace AxTools.Forms
{
    internal partial class WoWProcessSelector : BorderedMetroForm
    {
        private WoWProcessSelector()
        {
            InitializeComponent();
            ShowInTaskbar = false;
           StyleManager.Style = Settings.Instance.StyleColor;
            foreach (WowProcess i in WoWProcessManager.List)
            {
                comboBox1.Items.Add(i.IsValidBuild && GameFunctions.IsInGame_(i) ?
                    string.Format("pID: {0}", i.ProcessID) :
                    string.Format("pID: {0} (ERROR DETECTED)", i.ProcessID));
            }
            button1.Enabled = false;
            button2.Enabled = false;
            BeginInvoke((MethodInvoker) delegate
            {
                MainForm main = MainForm.Instance;
                Location = new Point(main.Location.X + main.Size.Width/2 - Size.Width/2, main.Location.Y + main.Size.Height/2 - Size.Height/2);
                OnActivated(EventArgs.Empty);
            });
        }

        /// <summary>
        ///     
        /// </summary>
        /// <returns></returns>
        internal static WowProcess GetWoWProcess()
        {
            if (WoWProcessManager.List.Count == 0)
            {
                return null;
            }
            if (WoWProcessManager.List.Count == 1)
            {
                return WoWProcessManager.List[0];
            }
            WoWProcessSelector bform = new WoWProcessSelector();
            bform.ShowDialog();
            return WoWProcessManager.List[bform.wIndex];
        }

        private int wIndex = -1;

        private void Button2Click(object sender, EventArgs e)
        {
            var fi = new FLASHWINFO {
                cbSize = (uint) Marshal.SizeOf(typeof (FLASHWINFO)),
                hwnd = WoWProcessManager.List[comboBox1.SelectedIndex].MainWindowHandle,
                dwFlags = FlashWindowFlags.FLASHW_TRAY | FlashWindowFlags.FLASHW_TIMERNOFG
            };
            NativeMethods.FlashWindowEx(ref fi);
        }

        private void Button1Click(object sender, EventArgs e)
        {
            wIndex = comboBox1.SelectedIndex;
            Close();
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = comboBox1.Text != string.Empty;
            button2.Enabled = comboBox1.Text != string.Empty;
        }
    
    }
}
