using System.Drawing;
using System.Linq;
using AxTools.Classes;
using AxTools.Properties;
using System;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Drawing;
using MetroFramework.Forms;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class AppSettings : MetroForm
    {
        public AppSettings()
        {
            InitializeComponent();
            AccessibleName = "AppSettings";
            CheckBoxStartAxToolsWithWindows.CheckedChanged += CheckBox9CheckedChanged;
            CheckBox5.CheckedChanged += CheckBox5CheckedChanged;
            CheckBoxAxToolsAddon.CheckedChanged += CheckBox10CheckedChanged;
            CheckBox4.CheckedChanged += CheckBox4CheckedChanged;
            CheckBox7.CheckedChanged += CheckBox7CheckedChanged;
            CheckBox6.CheckedChanged += CheckBox6CheckedChanged;
            TextBox7.TextChanged += TextBox7TextChanged;
            TextBox6.TextChanged += TextBox6TextChanged;
            TextBox5.TextChanged += TextBox5TextChanged;
            TextBox4.TextChanged += TextBox4TextChanged;
            CheckBox3.CheckedChanged += CheckBox3CheckedChanged;
            CheckBoxTransparentPingWidget.CheckedChanged += CheckBox11CheckedChanged;

            foreach (Keys i in new[] {
                Keys.None, Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11,
                Keys.F12, Keys.Home, Keys.End, Keys.Insert, Keys.Delete, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5,
                Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
            })
            {
                comboBoxClickerHotkey.Items.Add(i.ToString());
                comboBoxClickerKey.Items.Add(i.ToString());
                comboBoxWowLoginHotkey.Items.Add(i.ToString());
                comboBoxWExecLuaTimer.Items.Add(i.ToString());
                comboBoxWExecModule.Items.Add(i.ToString());
            }
            comboBoxClickerKey.Text = Settings.ClickerKey.ToString();
            num_clicker_interval.Value = Settings.ClickerInterval;
            //ComboBox_server_ip
            ComboBox_server_ip.Items.Clear();
            ComboBox_server_ip.Items.AddRange(Globals.GameServers.Select(k => k.Description).Cast<object>().ToArray());
            ComboBox_server_ip.Text = Settings.GameServer.Description;
            comboBoxClickerHotkey.Text = Settings.ClickerHotkey.ToString();
            comboBoxWowLoginHotkey.Text = Settings.WowLoginHotkey.ToString();
            comboBoxWExecLuaTimer.Text = Settings.LuaTimerHotkey.ToString();
            comboBoxWExecModule.Text = Settings.PrecompiledModulesHotkey.ToString();
            checkBoxAddonsBackup.Checked = Settings.AddonsBackup;
            numericUpDownBackupCopiesToKeep.Value = Settings.AddonsBackupNum;
            numericUpDownBackupTimer.Value = Settings.AddonsBackupTimer;
            metroComboBoxBackupCompressionLevel.SelectedIndex = Settings.BackupCompressionLevel;
            textBoxBackupPath.Text = Settings.AddonsBackupPath;
            textBoxMumblePath.Text = Settings.MumbleExe;
            textBoxRaidcallPath.Text = Settings.RaidcallExe;
            textBoxTeamspeak3Path.Text = Settings.TeamspeakExe;
            textBoxVentriloPath.Text = Settings.VtExe;
            textBoxWowPath.Text = Settings.WowExe;
            metroStyleManager1.Style = Settings.NewStyleColor;
            metroComboBoxStyle.SelectedIndex = (int)Settings.NewStyleColor == 0 ? 0 : (int)Settings.NewStyleColor - 1;
            metroTabControl1.SelectedIndex = 0;

            Icon = Resources.AppIcon;
            checkBox_AntiAFK.Checked = Settings.Wasd;
            CheckBox4.Checked = Settings.AutoPingWidget;
            CheckBox5.Checked = Settings.DelWowLog;
            CheckBoxAxToolsAddon.Checked = Settings.AxToolsAddon;
            CheckBox7.Checked = Settings.Noframe;
            CheckBox6.Checked = Settings.AutoAcceptWndSetts;
            CheckBoxTransparentPingWidget.Checked = Settings.PingWidgetTransparent;
            foreach (var i in new Control[] {CheckBox7, GroupBox1, GroupBox2})
            {
                i.Enabled = CheckBox6.Checked;
            }
            CheckBox3.Checked = Settings.CreatureCache;
            TextBox7.Text = Settings.WowWindowSize.X.ToString();
            TextBox6.Text = Settings.WowWindowSize.Y.ToString();
            TextBox5.Text = Settings.WowWindowLocation.X.ToString();
            TextBox4.Text = Settings.WowWindowLocation.Y.ToString();
            //CheckBox9
            try
            {
                Microsoft.Win32.RegistryKey regVersion =
                    Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run");
                if (regVersion != null)
                {
                    if (regVersion.GetValue("AxTools") == null | regVersion.GetValue("AxTools").ToString() != Application.ExecutablePath)
                    {
                        regVersion.Close();
                        CheckBoxStartAxToolsWithWindows.Checked = false;
                    }
                    else
                    {
                        regVersion.Close();
                        CheckBoxStartAxToolsWithWindows.Checked = true;
                    }
                }
                else
                {
                    CheckBoxStartAxToolsWithWindows.Checked = false;
                }
            }
            catch
            {
                CheckBoxStartAxToolsWithWindows.Checked = false;
            }
            //tooltips
            metroToolTip1.SetToolTip(CheckBox3, "Deletes creature cache file when possible");
            metroToolTip1.SetToolTip(CheckBox5,
                                     "Moves WoW log files to temporary folder\r\non AxTools' startup\r\nand deletes it on AxTools' shutdown");
            metroToolTip1.SetToolTip(CheckBox4, "Shows a small overlay frame with ping and\r\npacket loss info on WoW client's startup");
            metroToolTip1.SetToolTip(CheckBoxTransparentPingWidget,
                                     "Makes ping widget transparent for mouse clicks.\r\nPing widget should be reopen\r\nif this option changes");
            metroToolTip1.SetToolTip(CheckBoxAxToolsAddon, "Checks ax_tools version and updates it if needed\r\non AxTools' startup");
            metroToolTip1.SetToolTip(checkBox_AntiAFK,
                                     "Enables anti kick function for WoW.\r\nIt will prevent your character\r\nfrom /afk status");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (SolidBrush styleBrush = MetroPaint.GetStyleBrush(Style))
            {
                Rectangle rectRight = new Rectangle(Width - 1, 0, 1, Height);
                e.Graphics.FillRectangle(styleBrush, rectRight);
                Rectangle rectLeft = new Rectangle(0, 0, 1, Height);
                e.Graphics.FillRectangle(styleBrush, rectLeft);
                Rectangle rectBottom = new Rectangle(0, Height - 1, Width, 1);
                e.Graphics.FillRectangle(styleBrush, rectBottom);
            }
        }

        private void CheckBox9CheckedChanged(Object sender, EventArgs e)
        {
            if (CheckBoxStartAxToolsWithWindows.Checked)
            {
                try
                {
                    Microsoft.Win32.RegistryKey regVersion =
                        Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run");
                    if (regVersion != null)
                    {
                        regVersion.SetValue("AxTools", Application.ExecutablePath);
                        regVersion.Close();
                    }
                }
                catch (Exception ex)
                {
                    Log.Print("app_sett.CheckBox9.CheckedChanged_1: " + ex.Message, true);
                }
            }
            else
            {
                try
                {
                    Microsoft.Win32.RegistryKey regVersion = 
                        Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run");
                    if (regVersion != null)
                    {
                        if (regVersion.GetValue("AxTools") != null)
                        {
                            regVersion.DeleteValue("AxTools");
                            regVersion.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Print("app_sett.CheckBox9.CheckedChanged_2: " + ex.Message, true);
                }
            }
        }

        private void CheckBox5CheckedChanged(Object sender, EventArgs e)
        {
            Settings.DelWowLog = CheckBox5.Checked;
        }

        private void CheckBox10CheckedChanged(Object sender, EventArgs e)
        {
            Settings.AxToolsAddon = CheckBoxAxToolsAddon.Checked;
        }

        private void CheckBox4CheckedChanged(Object sender, EventArgs e)
        {
            Settings.AutoPingWidget = CheckBox4.Checked;
        }

        private void CheckBox7CheckedChanged(Object sender, EventArgs e)
        {
            Settings.Noframe = CheckBox7.Checked;
        }

        private void CheckBox6CheckedChanged(Object sender, EventArgs e)
        {
            Settings.AutoAcceptWndSetts = CheckBox6.Checked;
            foreach (var i in new Control[] { CheckBox7, GroupBox1, GroupBox2 })
            {
                i.Enabled = CheckBox6.Checked;
            }
        }

        private void TextBox7TextChanged(Object sender, EventArgs e)
        {
            if (TextBox7.Text != string.Empty && Convert.ToUInt16(TextBox7.Text) >= 720)
            {
                errorProvider1.Clear();
                Settings.WowWindowSize.X = Convert.ToUInt16(TextBox7.Text);
            }
            else
            {
                errorProvider1.SetError(TextBox7, "Incorrect value! It must be bigger than 720px");
            }
        }

        private void TextBox6TextChanged(Object sender, EventArgs e)
        {
            if (TextBox6.Text != string.Empty && Convert.ToUInt16(TextBox6.Text) >= 576)
            {
                errorProvider1.Clear();
                Settings.WowWindowSize.Y = Convert.ToUInt16(TextBox6.Text);
            }
            else
            {
                errorProvider1.SetError(TextBox6, "Incorrect value! It must be bigger than 576px");
            }
        }

        private void TextBox5TextChanged(Object sender, EventArgs e)
        {
            if (TextBox5.Text != string.Empty && Convert.ToInt16(TextBox5.Text) >= 0)
            {
                errorProvider1.Clear();
                Settings.WowWindowLocation.X = Convert.ToInt32(TextBox5.Text);
            }
            else
            {
                errorProvider1.SetError(TextBox5, "Incorrect value! It must be bigger than zero");
            }
        }

        private void TextBox4TextChanged(Object sender, EventArgs e)
        {
            if ((TextBox4.Text != string.Empty && Convert.ToInt16(TextBox4.Text) >= 0))
            {
                errorProvider1.Clear();
                Settings.WowWindowLocation.Y = Convert.ToInt32(TextBox4.Text);
            }
            else
            {
                errorProvider1.SetError(TextBox4, "Incorrect value! It must be bigger than zero");
            }
        }

        private void CheckBox3CheckedChanged(Object sender, EventArgs e)
        {
            Settings.CreatureCache = CheckBox3.Checked;
        }

        private void CheckBox11CheckedChanged(Object sender, EventArgs e)
        {
            Settings.PingWidgetTransparent = CheckBoxTransparentPingWidget.Checked;
        }

        private void Button9Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog p = new FolderBrowserDialog { ShowNewFolderButton = false, SelectedPath = string.Empty })
            {
                p.Description = "Select Ventrilo directory:";
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    textBoxVentriloPath.Text = p.SelectedPath;
                    Settings.VtExe = p.SelectedPath;
                }
            }
        }

        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            Settings.Wasd = checkBox_AntiAFK.Checked;
        }

        private void ComboBoxClickerHotkeySelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(comboBoxClickerHotkey.Text, true, out Settings.ClickerHotkey);
        }

        private void ComboBoxWowLoginHotkeySelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(comboBoxWowLoginHotkey.Text, true, out Settings.WowLoginHotkey);
        }

        private void ComboBoxWExecLuaTimerSelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(comboBoxWExecLuaTimer.Text, true, out Settings.LuaTimerHotkey);
            LuaConsole luaConsole = Utils.FindForm<LuaConsole>();
            if (luaConsole != null)
            {
                luaConsole.TimerHotkeyChanged();
            }
        }

        private void ComboBoxWExecModuleSelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(comboBoxWExecModule.Text, true, out Settings.PrecompiledModulesHotkey);
            MainForm main = Utils.FindForm<MainForm>();
            if (main != null)
            {
                main.HotkeysChanged();
            }
        }

        private void ComboBoxServerIpSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBox_server_ip.SelectedIndex != -1)
            {
                Settings.GameServer = Globals.GameServers.First(i => i.Description == ComboBox_server_ip.Text);
            }
        }

        private void CheckBoxAddonsBackupCheckedChanged(object sender, EventArgs e)
        {
            Settings.AddonsBackup = checkBoxAddonsBackup.Checked;
        }

        private void NumericUpDownBackupCopiesToKeepValueChanged(object sender, EventArgs e)
        {
            Settings.AddonsBackupNum = (int) numericUpDownBackupCopiesToKeep.Value;
        }

        private void NumericUpDownBackupTimerValueChanged(object sender, EventArgs e)
        {
            Settings.AddonsBackupTimer = (int) numericUpDownBackupTimer.Value;
        }

        private void ButtonRaidcallPathClick(object sender, EventArgs e)
        {
            using (FolderBrowserDialog p = new FolderBrowserDialog {ShowNewFolderButton = false, SelectedPath = string.Empty})
            {
                p.Description = "Select RaidCall directory:";
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    textBoxRaidcallPath.Text = p.SelectedPath;
                    Settings.RaidcallExe = p.SelectedPath;
                }
            }
        }

        private void ButtonMumblePathClick(object sender, EventArgs e)
        {
            using (FolderBrowserDialog p = new FolderBrowserDialog {ShowNewFolderButton = false, SelectedPath = string.Empty})
            {
                p.Description = "Select Mumble directory:";
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    textBoxMumblePath.Text = p.SelectedPath;
                    Settings.MumbleExe = p.SelectedPath;
                }
            }
        }

        private void ButtonTeamspeak3PathClick(object sender, EventArgs e)
        {
            using (FolderBrowserDialog p = new FolderBrowserDialog {ShowNewFolderButton = false, SelectedPath = string.Empty})
            {
                p.Description = "Select Teamspeak directory:";
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    textBoxTeamspeak3Path.Text = p.SelectedPath;
                    Settings.TeamspeakExe = p.SelectedPath;
                }
            }
        }

        private void ButtonBackupPathClick(object sender, EventArgs e)
        {
            using (FolderBrowserDialog p = new FolderBrowserDialog {ShowNewFolderButton = false, SelectedPath = string.Empty})
            {
                p.Description = "Select addons backup directory:";
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    textBoxBackupPath.Text = p.SelectedPath;
                    Settings.AddonsBackupPath = p.SelectedPath;
                }
            }
        }

        private void ComboBoxClickerKeySelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(comboBoxClickerKey.Text, true, out Settings.ClickerKey);
        }

        private void NumClickerIntervalValueChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated) Settings.ClickerInterval = Convert.ToInt32(num_clicker_interval.Value);
        }

        private void MetroComboBoxBackupCompressionLevelSelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.BackupCompressionLevel = metroComboBoxBackupCompressionLevel.SelectedIndex;
        }

        private void buttonWowPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog p = new FolderBrowserDialog { ShowNewFolderButton = false, SelectedPath = string.Empty })
            {
                p.Description = "Select WoW directory:";
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    textBoxWowPath.Text = p.SelectedPath;
                    Settings.WowExe = p.SelectedPath;
                }
            }
        }

        private void metroComboBoxStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            int style = metroComboBoxStyle.SelectedIndex == 0 ? 0 : metroComboBoxStyle.SelectedIndex + 1;
            Settings.NewStyleColor = (MetroColorStyle) style;
            foreach (object i in Application.OpenForms)
            {
                if (i.GetType().BaseType == typeof (MetroForm))
                {
                    if (((MetroForm) i).StyleManager != null)
                    {
                        ((MetroForm) i).StyleManager.Style = (MetroColorStyle) style;
                    }
                }
            }
        }

    }
}
