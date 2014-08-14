using AxTools.Classes;
using AxTools.Components;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.Services;
using MetroFramework;
using MetroFramework.Forms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class AppSettings : BorderedMetroForm
    {
        private readonly Settings settings = Settings.Instance;
        private readonly bool isSettingsLoaded;

        internal AppSettings()
        {
            InitializeComponent();
            CheckBoxStartAxToolsWithWindows.CheckedChanged += CheckBox9CheckedChanged;
            checkBoxNotifyIfBigLogFile.CheckedChanged += CheckBox5CheckedChanged;
            CheckBox7.CheckedChanged += CheckBox7CheckedChanged;
            CheckBox6.CheckedChanged += CheckBox6CheckedChanged;
            TextBox7.TextChanged += TextBox7TextChanged;
            TextBox6.TextChanged += TextBox6TextChanged;
            TextBox5.TextChanged += TextBox5TextChanged;
            TextBox4.TextChanged += TextBox4TextChanged;
            CheckBox3.CheckedChanged += CheckBox3CheckedChanged;

            foreach (Keys i in new[] {
                Keys.None, Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11,
                Keys.F12, Keys.Home, Keys.End, Keys.Insert, Keys.Delete, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5,
                Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
            })
            {
                comboBoxClickerHotkey.Items.Add(i.ToString());
                comboBoxWExecLuaTimer.Items.Add(i.ToString());
                comboBoxWExecModule.Items.Add(i.ToString());
            }

            for (int i = 0; i <= 30; i++)
            {
                comboBoxBadNetworkStatusProcent.Items.Add(i + "%");
            }
            comboBoxBadNetworkStatusProcent.SelectedIndex = settings.PingerBadPacketLoss;
            for (int i = 0; i <= 50; i++)
            {
                comboBoxVeryBadNetworkStatusProcent.Items.Add(i + "%");
            }
            comboBoxVeryBadNetworkStatusProcent.SelectedIndex = settings.PingerVeryBadPacketLoss;
            int counter = 25;
            while (counter <= 750)
            {
                comboBoxBadNetworkStatusPing.Items.Add(counter + "ms");
                comboBoxVeryBadNetworkStatusPing.Items.Add(counter + "ms");
                counter += 25;
            }
            comboBoxBadNetworkStatusPing.SelectedIndex = settings.PingerBadPing/25 - 1;
            comboBoxVeryBadNetworkStatusPing.SelectedIndex = settings.PingerVeryBadPing / 25 - 1;

            ComboBox_server_ip.Items.Clear();
            ComboBox_server_ip.Items.AddRange(Globals.GameServers.Select(k => k.Description).Cast<object>().ToArray());
            ComboBox_server_ip.SelectedIndex = settings.PingerServerID;
            comboBoxClickerHotkey.Text = settings.ClickerHotkey.ToString();
            comboBoxWExecLuaTimer.Text = settings.LuaTimerHotkey.ToString();
            comboBoxWExecModule.Text = settings.WoWPluginHotkey.ToString();
            checkBoxAddonsBackup.Checked = settings.WoWAddonsBackupIsActive;
            numericUpDownBackupCopiesToKeep.Value = settings.WoWAddonsBackupNumberOfArchives;
            numericUpDownBackupTimer.Value = settings.WoWAddonsBackupMinimumTimeBetweenBackup;
            metroComboBoxBackupCompressionLevel.SelectedIndex = settings.WoWAddonsBackupCompressionLevel;
            textBoxBackupPath.Text = settings.WoWAddonsBackupPath;
            textBoxMumblePath.Text = settings.MumbleDirectory;
            textBoxRaidcallPath.Text = settings.RaidcallDirectory;
            textBoxTeamspeak3Path.Text = settings.TS3Directory;
            textBoxVentriloPath.Text = settings.VentriloDirectory;
            textBoxWowPath.Text = settings.WoWDirectory;
            metroStyleManager1.Style = settings.StyleColor;
            metroComboBoxStyle.SelectedIndex = (int)settings.StyleColor == 0 ? 0 : (int)settings.StyleColor - 1;
            metroTabControl1.SelectedIndex = 0;
            checkBoxMinimizeToTray.Checked = settings.MinimizeToTray;

            Icon = Resources.AppIcon;
            checkBox_AntiAFK.Checked = settings.WoWAntiKick;
            checkBoxNotifyIfBigLogFile.Checked = settings.WoWNotifyIfBigLogs;
            textBoxNotifyIfBigLogFile.Text = settings.WoWNotifyIfBigLogsSize.ToString();
            CheckBox7.Checked = settings.WoWCustomWindowNoBorder;
            CheckBox6.Checked = settings.WoWCustomizeWindow;
            foreach (Control i in new Control[] {CheckBox7, GroupBox1, GroupBox2})
            {
                i.Enabled = CheckBox6.Checked;
            }
            CheckBox3.Checked = settings.WoWWipeCreatureCache;
            TextBox7.Text = settings.WoWCustomWindowSize.X.ToString();
            TextBox6.Text = settings.WoWCustomWindowSize.Y.ToString();
            TextBox5.Text = settings.WoWCustomWindowLocation.X.ToString();
            TextBox4.Text = settings.WoWCustomWindowLocation.Y.ToString();
            //CheckBox9
            try
            {
                Microsoft.Win32.RegistryKey regVersion = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run");
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
            metroToolTip1.SetToolTip(checkBoxNotifyIfBigLogFile, "Moves WoW log files to temporary folder\r\non AxTools' startup\r\nand deletes it on AxTools' shutdown");
            metroToolTip1.SetToolTip(checkBox_AntiAFK, "Enables anti kick function for WoW.\r\nIt will prevent your character\r\nfrom /afk status");
            isSettingsLoaded = true;
        }

        internal AppSettings(int tabPage) : this()
        {
            metroTabControl1.SelectedIndex = tabPage;
        }

        private void CheckBox9CheckedChanged(Object sender, EventArgs e)
        {
            if (isSettingsLoaded)
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
        }

        private void CheckBox5CheckedChanged(Object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.WoWNotifyIfBigLogs = checkBoxNotifyIfBigLogFile.Checked;
            }
        }

        private void CheckBox7CheckedChanged(Object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.WoWCustomWindowNoBorder = CheckBox7.Checked;
            }
        }

        private void CheckBox6CheckedChanged(Object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.WoWCustomizeWindow = CheckBox6.Checked;
                foreach (var i in new Control[] {CheckBox7, GroupBox1, GroupBox2})
                {
                    i.Enabled = CheckBox6.Checked;
                }
            }
        }

        private void TextBox7TextChanged(Object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                if (TextBox7.Text != string.Empty && Convert.ToUInt16(TextBox7.Text) >= 720)
                {
                    errorProvider.Clear();
                    settings.WoWCustomWindowSize.X = Convert.ToUInt16(TextBox7.Text);
                }
                else
                {
                    errorProvider.SetError(TextBox7, "Incorrect value! It must be bigger than 720px");
                }
            }
        }

        private void TextBox6TextChanged(Object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                if (TextBox6.Text != string.Empty && Convert.ToUInt16(TextBox6.Text) >= 576)
                {
                    errorProvider.Clear();
                    settings.WoWCustomWindowSize.Y = Convert.ToUInt16(TextBox6.Text);
                }
                else
                {
                    errorProvider.SetError(TextBox6, "Incorrect value! It must be bigger than 576px");
                }
            }
        }

        private void TextBox5TextChanged(Object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                if (TextBox5.Text != string.Empty && Convert.ToInt16(TextBox5.Text) >= 0)
                {
                    errorProvider.Clear();
                    settings.WoWCustomWindowLocation.X = Convert.ToInt32(TextBox5.Text);
                }
                else
                {
                    errorProvider.SetError(TextBox5, "Incorrect value! It must be bigger than zero");
                }
            }
        }

        private void TextBox4TextChanged(Object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                if (TextBox4.Text != string.Empty && Convert.ToInt32(TextBox4.Text) >= 0)
                {
                    errorProvider.Clear();
                    settings.WoWCustomWindowLocation.Y = Convert.ToInt32(TextBox4.Text);
                }
                else
                {
                    errorProvider.SetError(TextBox4, "Incorrect value! It must be bigger than zero");
                }
            }
        }

        private void CheckBox3CheckedChanged(Object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.WoWWipeCreatureCache = CheckBox3.Checked;
            }
        }

        private void Button9Click(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                using (FolderBrowserDialog p = new FolderBrowserDialog {ShowNewFolderButton = false, SelectedPath = string.Empty})
                {
                    p.Description = "Select Ventrilo directory:";
                    if (p.ShowDialog(this) == DialogResult.OK)
                    {
                        textBoxVentriloPath.Text = p.SelectedPath;
                        settings.VentriloDirectory = p.SelectedPath;
                    }
                }
            }
        }

        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.WoWAntiKick = checkBox_AntiAFK.Checked;
            }
        }

        private void ComboBoxClickerHotkeySelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                Keys key;
                if (Enum.TryParse(comboBoxClickerHotkey.Text, true, out key))
                {
                    settings.ClickerHotkey = key;
                }
            }
        }

        private void ComboBoxWExecLuaTimerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                Keys key;
                if (Enum.TryParse(comboBoxWExecLuaTimer.Text, true, out key))
                {
                    settings.LuaTimerHotkey = key;
                }
            }
        }

        private void ComboBoxWExecModuleSelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                Keys key;
                if (Enum.TryParse(comboBoxWExecModule.Text, true, out key))
                {
                    settings.WoWPluginHotkey = key;
                }
            }
        }

        private void CheckBoxAddonsBackupCheckedChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.WoWAddonsBackupIsActive = checkBoxAddonsBackup.Checked;
            }
        }

        private void NumericUpDownBackupCopiesToKeepValueChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.WoWAddonsBackupNumberOfArchives = (int) numericUpDownBackupCopiesToKeep.Value;
            }
        }

        private void NumericUpDownBackupTimerValueChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.WoWAddonsBackupMinimumTimeBetweenBackup = (int) numericUpDownBackupTimer.Value;
            }
        }

        private void ButtonRaidcallPathClick(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                using (FolderBrowserDialog p = new FolderBrowserDialog {ShowNewFolderButton = false, SelectedPath = string.Empty})
                {
                    p.Description = "Select RaidCall directory:";
                    if (p.ShowDialog(this) == DialogResult.OK)
                    {
                        textBoxRaidcallPath.Text = p.SelectedPath;
                        settings.RaidcallDirectory = p.SelectedPath;
                    }
                }
            }
        }

        private void ButtonMumblePathClick(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                using (FolderBrowserDialog p = new FolderBrowserDialog {ShowNewFolderButton = false, SelectedPath = string.Empty})
                {
                    p.Description = "Select Mumble directory:";
                    if (p.ShowDialog(this) == DialogResult.OK)
                    {
                        textBoxMumblePath.Text = p.SelectedPath;
                        settings.MumbleDirectory = p.SelectedPath;
                    }
                }
            }
        }

        private void ButtonTeamspeak3PathClick(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                using (FolderBrowserDialog p = new FolderBrowserDialog {ShowNewFolderButton = false, SelectedPath = string.Empty})
                {
                    p.Description = "Select Teamspeak directory:";
                    if (p.ShowDialog(this) == DialogResult.OK)
                    {
                        textBoxTeamspeak3Path.Text = p.SelectedPath;
                        settings.TS3Directory = p.SelectedPath;
                    }
                }
            }
        }

        private void ButtonBackupPathClick(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                using (FolderBrowserDialog p = new FolderBrowserDialog {ShowNewFolderButton = false, SelectedPath = string.Empty})
                {
                    p.Description = "Select addons backup directory:";
                    if (p.ShowDialog(this) == DialogResult.OK)
                    {
                        textBoxBackupPath.Text = p.SelectedPath;
                        settings.WoWAddonsBackupPath = p.SelectedPath;
                    }
                }
            }
        }

        private void MetroComboBoxBackupCompressionLevelSelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.WoWAddonsBackupCompressionLevel = metroComboBoxBackupCompressionLevel.SelectedIndex;
            }
        }

        private void buttonWowPath_Click(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                using (FolderBrowserDialog p = new FolderBrowserDialog {ShowNewFolderButton = false, SelectedPath = string.Empty})
                {
                    p.Description = "Select WoW directory:";
                    if (p.ShowDialog(this) == DialogResult.OK)
                    {
                        textBoxWowPath.Text = p.SelectedPath;
                        settings.WoWDirectory = p.SelectedPath;
                    }
                }
            }
        }

        private void metroComboBoxStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                int style = metroComboBoxStyle.SelectedIndex == 0 ? 0 : metroComboBoxStyle.SelectedIndex + 1;
                settings.StyleColor = (MetroColorStyle) style;
                foreach (object i in Application.OpenForms)
                {
                    if (i.GetType().ParentTypes().Any(l => l == typeof (MetroForm)))
                    {
                        if (((MetroForm) i).StyleManager != null)
                        {
                            ((MetroForm) i).StyleManager.Style = (MetroColorStyle) style;
                        }
                    }
                }
            }
        }

        private void linkShowLog_Click(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                if (File.Exists(Globals.LogFileName))
                {
                    try
                    {
                        Process.Start(Globals.LogFileName);
                    }
                    catch (Exception ex)
                    {
                        this.ShowTaskDialog("Cannot open log file", ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
                    }
                }
                else
                {
                    this.ShowTaskDialog("Cannot open log file", "It doesn't exist", TaskDialogButton.OK, TaskDialogIcon.Stop);
                }
            }
        }

        private void linkSendLogToDev_Click(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                try
                {
                    string subject = InputBox.Input("Any comment? (optional)");
                    WaitingOverlay waitingOverlay = new WaitingOverlay(this);
                    waitingOverlay.Show();
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            Log.SendViaEmail(subject);
                        }
                        catch (Exception ex)
                        {
                            Log.Print("Can't send log: " + ex.Message, true);
                            this.ShowTaskDialog("Can't send log", ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
                        }
                        finally
                        {
                            Invoke(new Action(waitingOverlay.Close));
                        }
                    });
                }
                catch (Exception ex)
                {
                    this.ShowTaskDialog("Log file sending error", ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
                }
            }
        }

        private void ComboBox_server_ip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded && ComboBox_server_ip.SelectedIndex != -1)
            {
                settings.PingerServerID = ComboBox_server_ip.SelectedIndex;
                if (ComboBox_server_ip.SelectedIndex == 0)
                {
                    if (Pinger.Enabled)
                    {
                        Pinger.Stop();
                    }
                }
                else
                {
                    if (!Pinger.Enabled)
                    {
                        Pinger.Start();
                    }
                }
            }
        }

        private void comboBoxBadNetworkStatusProcent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.PingerBadPacketLoss = comboBoxBadNetworkStatusProcent.SelectedIndex;
            }
        }

        private void comboBoxBadNetworkStatusPing_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.PingerBadPing = (comboBoxBadNetworkStatusPing.SelectedIndex + 1)*25;
            }
        }

        private void comboBoxVeryBadNetworkStatusProcent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.PingerVeryBadPacketLoss = comboBoxVeryBadNetworkStatusProcent.SelectedIndex;
            }
        }

        private void comboBoxVeryBadNetworkStatusPing_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.PingerVeryBadPing = (comboBoxVeryBadNetworkStatusPing.SelectedIndex + 1)*25;
            }
        }

        private void checkBoxMinimizeToTray_CheckedChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                settings.MinimizeToTray = checkBoxMinimizeToTray.Checked;
            }
        }

        private void textBoxNotifyIfBigLogFile_TextChanged(object sender, EventArgs e)
        {
            if (isSettingsLoaded)
            {
                int parsedValue;
                if (int.TryParse(textBoxNotifyIfBigLogFile.Text, out parsedValue) && parsedValue > 0)
                {
                    ErrorProviderExt.ClearError(textBoxNotifyIfBigLogFile);
                    settings.WoWNotifyIfBigLogsSize = parsedValue;
                }
                else
                {
                    ErrorProviderExt.SetError(textBoxNotifyIfBigLogFile, "Value must be a number bigger than 0", Color.Red);
                }
            }
        }
    
    }
}
