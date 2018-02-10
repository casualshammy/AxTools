using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WoW.PluginSystem;
using Components.Forms;
using KeyboardWatcher;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AxTools.Forms
{
    internal partial class MainForm_PluginHotkeys : BorderedMetroForm
    {

        private const int sizeBetweenRows = 26;

        internal MainForm_PluginHotkeys(IPlugin2[] plugins)
        {
            InitializeComponent();
            StyleManager.Style = AxTools.Helpers.Settings2.Instance.StyleColor;
            Icon = Resources.AppIcon;
            for (int i = 0; i < plugins.Length; i++)
            {
                PictureBox pictureBox = new PictureBox
                {
                    Parent = panel1,
                    Image = plugins[i].TrayIcon,
                    Location = new Point(23, 3 + sizeBetweenRows * i),
                    Size = new Size(19, 19),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                panel1.Controls.Add(pictureBox);
                MetroLabel label = new MetroLabel
                {
                    Parent = panel1,
                    AutoSize = false,
                    CustomBackground = false,
                    CustomForeColor = false,
                    FontSize = MetroFramework.MetroLabelSize.Small,
                    FontWeight = MetroFramework.MetroLabelWeight.Regular,
                    LabelMode = MetroLabelMode.Default,
                    Location = new Point(pictureBox.Location.X + pictureBox.Size.Width + 3, 5 + sizeBetweenRows * i),
                    Size = new Size(174, 19),
                    Text = plugins[i].Name,
                    UseStyleColors = true
                };
                panel1.Controls.Add(label);
                MetroTextBox textBox = new MetroTextBox
                {
                    Parent = panel1,
                    CustomBackground = false,
                    CustomForeColor = false,
                    FontSize = MetroFramework.MetroTextBoxSize.Small,
                    FontWeight = MetroFramework.MetroTextBoxWeight.Regular,
                    Location = new Point(label.Location.X + label.Size.Width + 3, 1 + sizeBetweenRows * i),
                    Multiline = false,
                    Text = AxTools.Helpers.Settings2.Instance.PluginHotkeys.ContainsKey(plugins[i].Name) ? AxTools.Helpers.Settings2.Instance.PluginHotkeys[plugins[i].Name].ToString() : "",
                    Size = new Size(105, 23),
                    UseStyleColors = true,
                    Tag = plugins[i]
                };
                textBox.KeyDown += TextBox_KeyDown;
                panel1.Controls.Add(textBox);
                MetroButton btn = new MetroButton
                {
                    Parent = panel1,
                    Highlight = true,
                    Location = new Point(textBox.Location.X + textBox.Size.Width + 3, 1 + sizeBetweenRows * i),
                    Size = new Size(75, 23),
                    Text = "Clear"
                };
                btn.Click += delegate { TextBox_KeyDown(textBox, new KeyEventArgs(Keys.None)); };
                panel1.Controls.Add(btn);
                Invalidate(true);
            }

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            MetroTextBox textBox = (MetroTextBox)sender;
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                KeyExt key = new KeyExt(e.KeyCode, e.Alt, e.Shift, e.Control);
                textBox.Text = key.ToString();
                string name = "Plugin_" + ((IPlugin2)textBox.Tag).Name;
                HotkeyManager.RemoveKeys(name);
                if (e.KeyCode == Keys.None)
                {
                    AxTools.Helpers.Settings2.Instance.PluginHotkeys.Remove(((IPlugin2)textBox.Tag).Name);
                }
                else
                {
                    AxTools.Helpers.Settings2.Instance.PluginHotkeys[((IPlugin2)textBox.Tag).Name] = key;
                    Task.Run(() => {
                        Thread.Sleep(1000);
                        HotkeyManager.AddKeys(name, key);
                    });
                }
                AxTools.Helpers.Settings2.Instance.InvokePluginHotkeysChanged();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
