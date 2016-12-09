using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.Services;
using AxTools.Services.PingerHelpers;
using AxTools.WinAPI;
using AxTools.WoW.PluginSystem;
using Components.Forms;
using MetroFramework;
using MetroFramework.Forms;
using Settings = AxTools.Helpers.Settings;

namespace AxTools.Forms
{
    internal partial class AppSettings : BorderedMetroForm
    {
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
            textBoxClickerHotkey.Text = new KeysConverter().ConvertToInvariantString(settings.ClickerHotkey);
            textBoxPluginsHotkey.Text = new KeysConverter().ConvertToInvariantString(settings.WoWPluginHotkey);
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
                ScheduledTaskInfo info = WinAPI.TaskScheduler.GetTaskInfo(Globals.TaskSchedulerTaskname);
                CheckBoxStartAxToolsWithWindows.Checked = info != null && info.TaskToRun == Application.ExecutablePath;
            }
            catch (Exception ex)
            {
                CheckBoxStartAxToolsWithWindows.Checked = false;
                Log.Error("Error occured then trying to open task [" + Globals.TaskSchedulerTaskname + "]: " + ex.Message);
            }
            toolTip.SetToolTip(checkBox_AntiAFK, "Enables anti kick function for WoW.\r\nIt will prevent your character\r\nfrom /afk status");
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
            textBoxVeryBadNetworkStatusProcent.KeyUp += textBoxVeryBadNetworkStatusProcent_KeyUp;
            textBoxBadNetworkStatusProcent.KeyUp += textBoxBadNetworkStatusProcent_KeyUp;
            textBoxBadNetworkStatusPing.KeyUp += textBoxBadNetworkStatusPing_KeyUp;
            textBoxVeryBadNetworkStatusPing.KeyUp += textBoxVeryBadNetworkStatusPing_KeyUp;
            buttonBackupPath.Click += ButtonBackupPathClick;
            metroComboBoxBackupCompressionLevel.SelectedIndexChanged += MetroComboBoxBackupCompressionLevelSelectedIndexChanged;
            checkBoxPluginsShowIngameNotifications.CheckedChanged += metroCheckBox1_CheckedChanged;
            buttonTeamspeak3Path.Click += ButtonTeamspeak3PathClick;
            buttonMumblePath.Click += ButtonMumblePathClick;
            buttonRaidcallPath.Click += ButtonRaidcallPathClick;
            buttonWowPath.Click += buttonWowPath_Click;
            metroComboBoxStyle.SelectedIndexChanged += metroComboBoxStyle_SelectedIndexChanged;
            linkShowLog.Click += linkShowLog_Click;
            linkSendLogToDev.Click += linkSendLogToDev_Click;
            checkBoxMinimizeToTray.CheckedChanged += checkBoxMinimizeToTray_CheckedChanged;
            checkBoxAddonsBackup.CheckedChanged += CheckBoxAddonsBackupCheckedChanged;
            numericUpDownBackupCopiesToKeep.ValueChanged += NumericUpDownBackupCopiesToKeepValueChanged;
            numericUpDownBackupTimer.ValueChanged += NumericUpDownBackupTimerValueChanged;
            buttonVentriloPath.Click += Button9Click;
            checkBox_AntiAFK.CheckedChanged += CheckBox1CheckedChanged;
            textBoxClickerHotkey.KeyDown += textBoxClickerHotkey_KeyDown;
            textBoxPluginsHotkey.KeyDown += textBoxPluginsHotkey_KeyDown;
            buttonClickerHotkey.Click += buttonClickerHotkey_Click;
            buttonPluginsHotkey.Click += buttonPluginsHotkey_Click;
            buttonIngameKeyBinds.Click += buttonIngameKeyBinds_Click;
        }

        private void buttonIngameKeyBinds_Click(object sender, EventArgs e)
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

        private void textBoxVeryBadNetworkStatusPing_KeyUp(object sender, KeyEventArgs e)
        {
            int value;
            if (int.TryParse(textBoxVeryBadNetworkStatusPing.Text, out value))
            {
                ErrorProviderExt.ClearError(textBoxVeryBadNetworkStatusPing);
                settings.PingerVeryBadPing = value;
            }
            else
            {
                ErrorProviderExt.SetError(textBoxVeryBadNetworkStatusPing, "Value must be a number", Color.Red);
            }
        }

        private void textBoxBadNetworkStatusPing_KeyUp(object sender, KeyEventArgs e)
        {
            int value;
            if (int.TryParse(textBoxBadNetworkStatusPing.Text, out value))
            {
                ErrorProviderExt.ClearError(textBoxBadNetworkStatusPing);
                settings.PingerBadPing = value;
            }
            else
            {
                ErrorProviderExt.SetError(textBoxBadNetworkStatusPing, "Value must be a number", Color.Red);
            }
        }

        private void textBoxBadNetworkStatusProcent_KeyUp(object sender, KeyEventArgs e)
        {
            int value;
            if (int.TryParse(textBoxBadNetworkStatusProcent.Text, out value))
            {
                ErrorProviderExt.ClearError(textBoxBadNetworkStatusProcent);
                settings.PingerBadPacketLoss = value;
            }
            else
            {
                ErrorProviderExt.SetError(textBoxBadNetworkStatusProcent, "Value must be a number", Color.Red);
            }
        }

        private void textBoxVeryBadNetworkStatusProcent_KeyUp(object sender, KeyEventArgs e)
        {
            int value;
            if (int.TryParse(textBoxVeryBadNetworkStatusProcent.Text, out value))
            {
                ErrorProviderExt.ClearError(textBoxVeryBadNetworkStatusProcent);
                settings.PingerVeryBadPacketLoss = value;
            }
            else
            {
                ErrorProviderExt.SetError(textBoxVeryBadNetworkStatusProcent, "Value must be a number", Color.Red);
            }
        }

        private void buttonPluginsHotkey_Click(object sender, EventArgs e)
        {
            textBoxPluginsHotkey_KeyDown(null, new KeyEventArgs(Keys.None));
        }

        private void buttonClickerHotkey_Click(object sender, EventArgs e)
        {
            textBoxClickerHotkey_KeyDown(null, new KeyEventArgs(Keys.None));
        }

        private void CheckBox9CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxStartAxToolsWithWindows.Checked)
            {
                try
                {
                    string xml = Encoding.Unicode.GetString(Resources.axtools_start).Replace("<Command></Command>", string.Format("<Command>{0}</Command>", Application.ExecutablePath));
                    string xmlPath = string.Format("{0}\\{1}.xml", AppFolders.TempDir, Globals.TaskSchedulerTaskname);
                    File.WriteAllText(xmlPath, xml, Encoding.Unicode);
                    WinAPI.TaskScheduler.CreateTask(Globals.TaskSchedulerTaskname, xmlPath);
                }
                catch (Exception ex)
                {
                    Log.Error("app_sett.CheckBox9.CheckedChanged_1: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    WinAPI.TaskScheduler.DeleteTask(Globals.TaskSchedulerTaskname);
                }
                catch (Exception ex)
                {
                    Log.Error("app_sett.CheckBox9.CheckedChanged_2: " + ex.Message);
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

        private void buttonWowPath_Click(object sender, EventArgs e)
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

        private void metroComboBoxStyle_SelectedIndexChanged(object sender, EventArgs e)
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

        private void linkShowLog_Click(object sender, EventArgs e)
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

        private void linkSendLogToDev_Click(object sender, EventArgs e)
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
                            Log.UploadLog(subject);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Can't send log: " + ex.Message);
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
                Log.Info("Can't send log file: " + ex.Message);
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

        private void checkBoxMinimizeToTray_CheckedChanged(object sender, EventArgs e)
        {
            settings.MinimizeToTray = checkBoxMinimizeToTray.Checked;
        }

        private void metroCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWPluginShowIngameNotifications = checkBoxPluginsShowIngameNotifications.Checked;
        }

        private void textBoxClickerHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                Keys keys = LetOnlyOneKeyModifier(e);
                textBoxClickerHotkey.Text = new KeysConverter().ConvertToInvariantString(keys);
                HotkeyManager.RemoveKeys(typeof (Clicker).ToString());
                HotkeyManager.AddKeys(typeof (Clicker).ToString(), keys);
                settings.ClickerHotkey = keys;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxPluginsHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                Keys keys = LetOnlyOneKeyModifier(e);
                textBoxPluginsHotkey.Text = new KeysConverter().ConvertToInvariantString(keys);
                HotkeyManager.RemoveKeys(typeof(PluginManagerEx).ToString());
                HotkeyManager.AddKeys(typeof(PluginManagerEx).ToString(), keys);
                settings.WoWPluginHotkey = keys;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private Keys LetOnlyOneKeyModifier(KeyEventArgs args)
        {
            switch (args.Modifiers)
            {
                case Keys.Shift | Keys.Control | Keys.Alt:
                    return args.KeyData & ~Keys.Control & ~Keys.Alt;
                case Keys.Shift | Keys.Control:
                    return args.KeyData & ~Keys.Control;
                case Keys.Shift | Keys.Alt:
                    return args.KeyData & ~Keys.Alt;
                case Keys.Control | Keys.Alt:
                    return args.KeyData & ~Keys.Alt;
                default:
                    return args.KeyData;
            }
        }
    
    }
}
