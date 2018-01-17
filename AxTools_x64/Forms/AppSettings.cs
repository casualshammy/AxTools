using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.Services;
using AxTools.Services.PingerHelpers;
using AxTools.WoW.PluginSystem;
using Components.Forms;
using KeyboardWatcher;
using MetroFramework;
using MetroFramework.Forms;
using Microsoft.Win32;
using Settings = AxTools.Helpers.Settings;

namespace AxTools.Forms
{
    internal partial class AppSettings : BorderedMetroForm
    {
        private static readonly Log2 log = new Log2("AppSettings");
        private readonly Settings settings = Settings.Instance;

        internal AppSettings()
        {
            InitializeComponent();
            StyleManager.Style = Settings.Instance.StyleColor;
            Icon = Resources.AppIcon;
            tabControl.SelectedIndex = 0;
            SetupData();
            SetupEvents();
        }

        internal AppSettings(int tabPage) : this()
        {
            tabControl.SelectedIndex = tabPage;
        }

        private void SetupData()
        {
            textBoxClickerHotkey.Text = settings.ClickerHotkey.ToString();
            textBoxPluginsHotkey.Text = settings.WoWPluginHotkey.ToString();
            textBoxBadNetworkStatusProcent.Text = settings.PingerBadPacketLoss.ToString();
            textBoxVeryBadNetworkStatusProcent.Text = settings.PingerVeryBadPacketLoss.ToString();
            textBoxBadNetworkStatusPing.Text = settings.PingerBadPing.ToString();
            textBoxVeryBadNetworkStatusPing.Text = settings.PingerVeryBadPing.ToString();
            ComboBox_server_ip.Items.Clear();
            ComboBox_server_ip.Items.AddRange(GameServers.Entries.Select(k => k.Description).Cast<object>().ToArray());
            ComboBox_server_ip.SelectedIndex = settings.PingerServerID;
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
            metroComboBoxStyle.SelectedIndex = (int)settings.StyleColor == 0 ? 0 : (int)settings.StyleColor - 1;
            checkBoxMinimizeToTray.Checked = settings.MinimizeToTray;
            checkBoxPluginsShowIngameNotifications.Checked = settings.WoWPluginShowIngameNotifications;
            checkBox_AntiAFK.Checked = settings.WoWAntiKick;
            CheckBox7.Checked = settings.WoWCustomWindowNoBorder;
            CheckBox6.Checked = settings.WoWCustomizeWindow;
            foreach (Control i in new Control[] { CheckBox7, GroupBox1, GroupBox2 })
            {
                i.Enabled = CheckBox6.Checked;
            }
            TextBox7.Text = settings.WoWCustomWindowRectangle.Width.ToString();
            TextBox6.Text = settings.WoWCustomWindowRectangle.Height.ToString();
            TextBox5.Text = settings.WoWCustomWindowRectangle.X.ToString();
            TextBox4.Text = settings.WoWCustomWindowRectangle.Y.ToString();
            try
            {
                using (RegistryKey regVersion = Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run"))
                {
                    CheckBoxStartAxToolsWithWindows.Checked = regVersion != null && regVersion.GetValue("AxTools") != null && regVersion.GetValue("AxTools").ToString() == Application.ExecutablePath;
                }
            }
            catch (Exception ex)
            {
                CheckBoxStartAxToolsWithWindows.Checked = false;
                log.Error("Error occured then trying to open registry key [SOFTWARE\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run]: " + ex.Message);
            }
            toolTip.SetToolTip(checkBox_AntiAFK, "Enables anti kick function for WoW.\r\nIt will prevent your character\r\nfrom /afk status");
            checkBoxMakeBackupNotWhilePlaying.Checked = settings.WoWAddonsBackup_DoNotCreateBackupWhileWoWClientIsRunning;
            toolTip.SetToolTip(checkBoxMakeBackupNotWhilePlaying, "Backup creation is CPU-intensive operation and can cause lag.\r\nThis option will prevent AxTools from making backups while WoW client is running.");
            checkBoxSetAfkStatus.Checked = settings.WoW_AntiKick_SetAfkState;
            checkBoxSetAfkStatus.Enabled = checkBox_AntiAFK.Checked;
            toolTip.SetToolTip(checkBoxSetAfkStatus, "This feature will not work if WoW client is minimized");
        }

        private void SetupEvents()
        {
            CheckBoxStartAxToolsWithWindows.CheckedChanged += CheckBox9CheckedChanged;
            CheckBox7.CheckedChanged += CheckBox7CheckedChanged;
            CheckBox6.CheckedChanged += CheckBox6CheckedChanged;
            TextBox7.TextChanged += TextBox7TextChanged;
            TextBox6.TextChanged += TextBox6TextChanged;
            TextBox5.TextChanged += TextBox5TextChanged;
            TextBox4.TextChanged += TextBox4TextChanged;
            ComboBox_server_ip.SelectedIndexChanged += ComboBox_server_ip_SelectedIndexChanged;
            textBoxVeryBadNetworkStatusProcent.KeyUp += TextBoxVeryBadNetworkStatusProcent_KeyUp;
            textBoxBadNetworkStatusProcent.KeyUp += TextBoxBadNetworkStatusProcent_KeyUp;
            textBoxBadNetworkStatusPing.KeyUp += TextBoxBadNetworkStatusPing_KeyUp;
            textBoxVeryBadNetworkStatusPing.KeyUp += TextBoxVeryBadNetworkStatusPing_KeyUp;
            buttonBackupPath.Click += ButtonBackupPathClick;
            metroComboBoxBackupCompressionLevel.SelectedIndexChanged += MetroComboBoxBackupCompressionLevelSelectedIndexChanged;
            checkBoxPluginsShowIngameNotifications.CheckedChanged += MetroCheckBox1_CheckedChanged;
            buttonTeamspeak3Path.Click += ButtonTeamspeak3PathClick;
            buttonMumblePath.Click += ButtonMumblePathClick;
            buttonRaidcallPath.Click += ButtonRaidcallPathClick;
            buttonWowPath.Click += ButtonWowPath_Click;
            metroComboBoxStyle.SelectedIndexChanged += MetroComboBoxStyle_SelectedIndexChanged;
            linkShowLog.Click += LinkShowLog_Click;
            linkSendLogToDev.Click += LinkSendLogToDev_Click;
            checkBoxMinimizeToTray.CheckedChanged += CheckBoxMinimizeToTray_CheckedChanged;
            checkBoxAddonsBackup.CheckedChanged += CheckBoxAddonsBackupCheckedChanged;
            numericUpDownBackupCopiesToKeep.ValueChanged += NumericUpDownBackupCopiesToKeepValueChanged;
            numericUpDownBackupTimer.ValueChanged += NumericUpDownBackupTimerValueChanged;
            buttonVentriloPath.Click += Button9Click;
            checkBox_AntiAFK.CheckedChanged += CheckBox1CheckedChanged;
            textBoxClickerHotkey.KeyDown += TextBoxClickerHotkey_KeyDown;
            textBoxPluginsHotkey.KeyDown += TextBoxPluginsHotkey_KeyDown;
            buttonClickerHotkey.Click += ButtonClickerHotkey_Click;
            buttonPluginsHotkey.Click += ButtonPluginsHotkey_Click;
            buttonIngameKeyBinds.Click += ButtonIngameKeyBinds_Click;
            checkBoxMakeBackupNotWhilePlaying.CheckedChanged += CheckBoxMakeBackupNotWhilePlaying_CheckedChanged;
            checkBoxSetAfkStatus.CheckedChanged += CheckBoxSetAfkStatus_CheckedChanged;
        }

        private void CheckBoxSetAfkStatus_CheckedChanged(object sender, EventArgs e)
        {
            settings.WoW_AntiKick_SetAfkState = checkBoxSetAfkStatus.Checked;
        }

        private void CheckBoxMakeBackupNotWhilePlaying_CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWAddonsBackup_DoNotCreateBackupWhileWoWClientIsRunning = checkBoxMakeBackupNotWhilePlaying.Checked;
        }

        private void ButtonIngameKeyBinds_Click(object sender, EventArgs e)
        {
            AppSettingsWoWBinds form = Utils.FindForm<AppSettingsWoWBinds>();
            if (form != null)
            {
                form.Show();
                form.Activate();
            }
            else
            {
                new AppSettingsWoWBinds().ShowDialog(this);
            }
        }

        private void TextBoxVeryBadNetworkStatusPing_KeyUp(object sender, KeyEventArgs e)
        {
            if (int.TryParse(textBoxVeryBadNetworkStatusPing.Text, out int value))
            {
                ErrorProviderExt.ClearError(textBoxVeryBadNetworkStatusPing);
                settings.PingerVeryBadPing = value;
            }
            else
            {
                ErrorProviderExt.SetError(textBoxVeryBadNetworkStatusPing, "Value must be a number", Color.Red);
            }
        }

        private void TextBoxBadNetworkStatusPing_KeyUp(object sender, KeyEventArgs e)
        {
            if (int.TryParse(textBoxBadNetworkStatusPing.Text, out int value))
            {
                ErrorProviderExt.ClearError(textBoxBadNetworkStatusPing);
                settings.PingerBadPing = value;
            }
            else
            {
                ErrorProviderExt.SetError(textBoxBadNetworkStatusPing, "Value must be a number", Color.Red);
            }
        }

        private void TextBoxBadNetworkStatusProcent_KeyUp(object sender, KeyEventArgs e)
        {
            if (int.TryParse(textBoxBadNetworkStatusProcent.Text, out int value))
            {
                ErrorProviderExt.ClearError(textBoxBadNetworkStatusProcent);
                settings.PingerBadPacketLoss = value;
            }
            else
            {
                ErrorProviderExt.SetError(textBoxBadNetworkStatusProcent, "Value must be a number", Color.Red);
            }
        }

        private void TextBoxVeryBadNetworkStatusProcent_KeyUp(object sender, KeyEventArgs e)
        {
            if (int.TryParse(textBoxVeryBadNetworkStatusProcent.Text, out int value))
            {
                ErrorProviderExt.ClearError(textBoxVeryBadNetworkStatusProcent);
                settings.PingerVeryBadPacketLoss = value;
            }
            else
            {
                ErrorProviderExt.SetError(textBoxVeryBadNetworkStatusProcent, "Value must be a number", Color.Red);
            }
        }

        private void ButtonPluginsHotkey_Click(object sender, EventArgs e)
        {
            TextBoxPluginsHotkey_KeyDown(null, new KeyEventArgs(Keys.None));
        }

        private void ButtonClickerHotkey_Click(object sender, EventArgs e)
        {
            TextBoxClickerHotkey_KeyDown(null, new KeyEventArgs(Keys.None));
        }

        private void CheckBox9CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxStartAxToolsWithWindows.Checked)
            {
                try
                {
                    RegistryKey regVersion =
                        Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run");
                    if (regVersion != null)
                    {
                        regVersion.SetValue("AxTools", Application.ExecutablePath);
                        regVersion.Close();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("app_sett.CheckBox9.CheckedChanged_1: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    RegistryKey regVersion =
                        Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run");
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
                    log.Error("app_sett.CheckBox9.CheckedChanged_2: " + ex.Message);
                }
            }
        }

        private void CheckBox7CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWCustomWindowNoBorder = CheckBox7.Checked;
        }

        private void CheckBox6CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWCustomizeWindow = CheckBox6.Checked;
            foreach (Control i in new Control[] {CheckBox7, GroupBox1, GroupBox2})
            {
                i.Enabled = CheckBox6.Checked;
            }
        }

        private void TextBox7TextChanged(object sender, EventArgs e)
        {
            if (TextBox7.Text != string.Empty && Convert.ToUInt16(TextBox7.Text) >= 720)
            {
                ErrorProviderExt.ClearError(TextBox7);
                settings.WoWCustomWindowRectangle.Width = Convert.ToUInt16(TextBox7.Text);
            }
            else
            {
                ErrorProviderExt.SetError(TextBox7, "Incorrect value! It must be bigger than 720px", Color.Red);
            }
        }

        private void TextBox6TextChanged(object sender, EventArgs e)
        {
            if (TextBox6.Text != string.Empty && Convert.ToUInt16(TextBox6.Text) >= 576)
            {
                ErrorProviderExt.ClearError(TextBox6);
                settings.WoWCustomWindowRectangle.Height = Convert.ToUInt16(TextBox6.Text);
            }
            else
            {
                ErrorProviderExt.SetError(TextBox6, "Incorrect value! It must be bigger than 576px", Color.Red);
            }
        }

        private void TextBox5TextChanged(object sender, EventArgs e)
        {
            if (TextBox5.Text != string.Empty && Convert.ToInt16(TextBox5.Text) >= 0)
            {
                ErrorProviderExt.ClearError(TextBox5);
                settings.WoWCustomWindowRectangle.X = Convert.ToInt32(TextBox5.Text);
            }
            else
            {
                ErrorProviderExt.SetError(TextBox5, "Incorrect value! It must be bigger than zero", Color.Red);
            }
        }

        private void TextBox4TextChanged(object sender, EventArgs e)
        {
            if (TextBox4.Text != string.Empty && Convert.ToInt32(TextBox4.Text) >= 0)
            {
                ErrorProviderExt.ClearError(TextBox4);
                settings.WoWCustomWindowRectangle.Y = Convert.ToInt32(TextBox4.Text);
            }
            else
            {
                ErrorProviderExt.SetError(TextBox4, "Incorrect value! It must be bigger than zero", Color.Red);
            }
        }

        private void Button9Click(object sender, EventArgs e)
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

        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWAntiKick = checkBox_AntiAFK.Checked;
            checkBoxSetAfkStatus.Enabled = checkBox_AntiAFK.Checked;
        }

        private void CheckBoxAddonsBackupCheckedChanged(object sender, EventArgs e)
        {
            settings.WoWAddonsBackupIsActive = checkBoxAddonsBackup.Checked;
        }

        private void NumericUpDownBackupCopiesToKeepValueChanged(object sender, EventArgs e)
        {
            settings.WoWAddonsBackupNumberOfArchives = (int) numericUpDownBackupCopiesToKeep.Value;
        }

        private void NumericUpDownBackupTimerValueChanged(object sender, EventArgs e)
        {
            settings.WoWAddonsBackupMinimumTimeBetweenBackup = (int) numericUpDownBackupTimer.Value;
        }

        private void ButtonRaidcallPathClick(object sender, EventArgs e)
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

        private void ButtonMumblePathClick(object sender, EventArgs e)
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

        private void ButtonTeamspeak3PathClick(object sender, EventArgs e)
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

        private void ButtonBackupPathClick(object sender, EventArgs e)
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

        private void MetroComboBoxBackupCompressionLevelSelectedIndexChanged(object sender, EventArgs e)
        {
            settings.WoWAddonsBackupCompressionLevel = metroComboBoxBackupCompressionLevel.SelectedIndex;
        }

        private void ButtonWowPath_Click(object sender, EventArgs e)
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

        private void MetroComboBoxStyle_SelectedIndexChanged(object sender, EventArgs e)
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

        private void LinkShowLog_Click(object sender, EventArgs e)
        {
            if (File.Exists(Globals.LogFileName))
            {
                try
                {
                    Process.Start(Globals.LogFileName);
                }
                catch (Exception ex)
                {
                    this.TaskDialog("Cannot open log file", ex.Message, NotifyUserType.Error);
                }
            }
            else
            {
                this.TaskDialog("Cannot open log file", "It doesn't exist", NotifyUserType.Error);
            }
        }

        private void LinkSendLogToDev_Click(object sender, EventArgs e)
        {
            try
            {
                string subject = InputBox.Input("Any comment? (optional)", settings.StyleColor);
                if (subject != null)
                {
                    WaitingOverlay waitingOverlay = WaitingOverlay.Show(this);
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            Log2.UploadLog(subject);
                        }
                        catch (Exception ex)
                        {
                            log.Error("Can't send log: " + ex.Message);
                            this.TaskDialog("Can't send log", ex.Message, NotifyUserType.Error);
                        }
                        finally
                        {
                            Invoke(new Action(waitingOverlay.Close));
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                log.Info("Can't send log file: " + ex.Message);
                this.TaskDialog("Log file sending error", ex.Message, NotifyUserType.Error);
            }
        }

        private void ComboBox_server_ip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBox_server_ip.SelectedIndex != -1)
            {
                settings.PingerServerID = ComboBox_server_ip.SelectedIndex;
                if (ComboBox_server_ip.SelectedIndex == 0)
                {
                    if (Pinger.Enabled)
                    {
                        Pinger.Enabled = false;
                    }
                }
                else
                {
                    if (!Pinger.Enabled)
                    {
                        Pinger.Enabled = true;
                    }
                    else
                    {
                        Pinger.Enabled = false;
                        Pinger.Enabled = true;
                    }
                }
                
            }
        }

        private void CheckBoxMinimizeToTray_CheckedChanged(object sender, EventArgs e)
        {
            settings.MinimizeToTray = checkBoxMinimizeToTray.Checked;
        }

        private void MetroCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWPluginShowIngameNotifications = checkBoxPluginsShowIngameNotifications.Checked;
        }

        private void TextBoxClickerHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                KeyExt key = new KeyExt(e.KeyCode, e.Alt, e.Shift, e.Control);
                textBoxClickerHotkey.Text = key.ToString();
                HotkeyManager.RemoveKeys(typeof (Clicker).ToString());
                HotkeyManager.AddKeys(typeof(Clicker).ToString(), key);
                settings.ClickerHotkey = key;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void TextBoxPluginsHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                KeyExt key = new KeyExt(e.KeyCode, e.Alt, e.Shift, e.Control);
                textBoxPluginsHotkey.Text = key.ToString();
                HotkeyManager.RemoveKeys(typeof(PluginManagerEx).ToString());
                HotkeyManager.AddKeys(typeof(PluginManagerEx).ToString(), key);
                settings.WoWPluginHotkey = key;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        //private Keys LetOnlyOneKeyModifier(KeyEventArgs args)
        //{
        //    switch (args.Modifiers)
        //    {
        //        case Keys.Shift | Keys.Control | Keys.Alt:
        //            return args.KeyData & ~Keys.Control & ~Keys.Alt;
        //        case Keys.Shift | Keys.Control:
        //            return args.KeyData & ~Keys.Control;
        //        case Keys.Shift | Keys.Alt:
        //            return args.KeyData & ~Keys.Alt;
        //        case Keys.Control | Keys.Alt:
        //            return args.KeyData & ~Keys.Alt;
        //        default:
        //            return args.KeyData;
        //    }
        //}
    
    }
}
