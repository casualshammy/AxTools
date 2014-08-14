using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using AxTools.Classes;
using AxTools.Components;
using AxTools.Properties;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Controls;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        //Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
        //Для ее изменения используйте конструктор форм Windows Form.  
        //Не изменяйте ее в редакторе исходного кода.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.woWRadarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.luaConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blackMarketTrackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.stopActivePluginorPresshotkeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.launchWoWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.woWPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.progressBarAddonsBackup = new MetroFramework.Controls.MetroProgressBar();
            this.linkEditWowAccounts = new MetroFramework.Controls.MetroLink();
            this.buttonLaunchWowWithoutAutopass = new MetroFramework.Controls.MetroButton();
            this.linkClickerSettings = new MetroFramework.Controls.MetroLink();
            this.linkBackup = new MetroFramework.Controls.MetroLink();
            this.linkOpenBackupFolder = new MetroFramework.Controls.MetroLink();
            this.cmbboxAccSelect = new AxTools.Components.MetroComboboxExt(this.components);
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager();
            this.metroTabPage3 = new MetroFramework.Controls.MetroTabPage();
            this.buttonStartStopPlugin = new MetroFramework.Controls.MetroButton();
            this.checkBoxEnableCustomPlugins = new MetroFramework.Controls.MetroCheckBox();
            this.metroCheckBoxPluginShowIngameNotification = new MetroFramework.Controls.MetroCheckBox();
            this.metroButtonLuaConsole = new MetroFramework.Controls.MetroButton();
            this.metroButtonRadar = new MetroFramework.Controls.MetroButton();
            this.metroButtonBlackMarketTracker = new MetroFramework.Controls.MetroButton();
            this.buttonUnloadInjector = new MetroFramework.Controls.MetroButton();
            this.comboBoxWowPlugins = new AxTools.Components.MetroComboboxExt(this.components);
            this.metroTabPage2 = new MetroFramework.Controls.MetroTabPage();
            this.checkBoxStartTeamspeak3WithWow = new MetroFramework.Controls.MetroCheckBox();
            this.checkBoxStartMumbleWithWow = new MetroFramework.Controls.MetroCheckBox();
            this.checkBoxStartRaidcallWithWow = new MetroFramework.Controls.MetroCheckBox();
            this.checkBoxStartVenriloWithWow = new MetroFramework.Controls.MetroCheckBox();
            this.tileRaidcall = new AxTools.Components.MetroTileExt(this.components);
            this.tileTeamspeak3 = new AxTools.Components.MetroTileExt(this.components);
            this.tileMumble = new AxTools.Components.MetroTileExt(this.components);
            this.tileVentrilo = new AxTools.Components.MetroTileExt(this.components);
            this.labelPingNum = new MetroFramework.Controls.MetroLabel();
            this.metroToolTip1 = new MetroFramework.Components.MetroToolTip();
            this.linkSettings = new MetroFramework.Controls.MetroLink();
            this.contextMenuStripMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.metroTabPage1.SuspendLayout();
            this.metroTabPage3.SuspendLayout();
            this.metroTabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIconMain
            // 
            this.notifyIconMain.ContextMenuStrip = this.contextMenuStripMain;
            this.notifyIconMain.Text = "AxTools";
            this.notifyIconMain.Visible = true;
            this.notifyIconMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIconMainMouseClick);
            // 
            // contextMenuStripMain
            // 
            this.contextMenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.woWRadarToolStripMenuItem,
            this.luaConsoleToolStripMenuItem,
            this.blackMarketTrackerToolStripMenuItem,
            this.toolStripSeparator2,
            this.stopActivePluginorPresshotkeyToolStripMenuItem,
            this.toolStripSeparator1,
            this.launchWoWToolStripMenuItem});
            this.contextMenuStripMain.Name = "contextMenuStripMain";
            this.contextMenuStripMain.Size = new System.Drawing.Size(182, 126);
            // 
            // woWRadarToolStripMenuItem
            // 
            this.woWRadarToolStripMenuItem.Image = global::AxTools.Properties.Resources.radar;
            this.woWRadarToolStripMenuItem.Name = "woWRadarToolStripMenuItem";
            this.woWRadarToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.woWRadarToolStripMenuItem.Text = "WoW Radar";
            this.woWRadarToolStripMenuItem.Click += new System.EventHandler(this.WoWRadarToolStripMenuItemClick);
            // 
            // luaConsoleToolStripMenuItem
            // 
            this.luaConsoleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("luaConsoleToolStripMenuItem.Image")));
            this.luaConsoleToolStripMenuItem.Name = "luaConsoleToolStripMenuItem";
            this.luaConsoleToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.luaConsoleToolStripMenuItem.Text = "Lua console";
            this.luaConsoleToolStripMenuItem.Click += new System.EventHandler(this.LuaConsoleToolStripMenuItemClick);
            // 
            // blackMarketTrackerToolStripMenuItem
            // 
            this.blackMarketTrackerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("blackMarketTrackerToolStripMenuItem.Image")));
            this.blackMarketTrackerToolStripMenuItem.Name = "blackMarketTrackerToolStripMenuItem";
            this.blackMarketTrackerToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.blackMarketTrackerToolStripMenuItem.Text = "Black Market tracker";
            this.blackMarketTrackerToolStripMenuItem.Click += new System.EventHandler(this.blackMarketTrackerToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
            // 
            // stopActivePluginorPresshotkeyToolStripMenuItem
            // 
            this.stopActivePluginorPresshotkeyToolStripMenuItem.Enabled = false;
            this.stopActivePluginorPresshotkeyToolStripMenuItem.Name = "stopActivePluginorPresshotkeyToolStripMenuItem";
            this.stopActivePluginorPresshotkeyToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.stopActivePluginorPresshotkeyToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.stopActivePluginorPresshotkeyToolStripMenuItem.Text = "Stop active plug-in";
            this.stopActivePluginorPresshotkeyToolStripMenuItem.Click += new System.EventHandler(this.stopActivePluginorPresshotkeyToolStripMenuItem_Click_1);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // launchWoWToolStripMenuItem
            // 
            this.launchWoWToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("launchWoWToolStripMenuItem.Image")));
            this.launchWoWToolStripMenuItem.Name = "launchWoWToolStripMenuItem";
            this.launchWoWToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.launchWoWToolStripMenuItem.Text = "Exit";
            this.launchWoWToolStripMenuItem.Click += new System.EventHandler(this.ExitAxToolsToolStripMenuItem_Click);
            // 
            // woWPluginsToolStripMenuItem
            // 
            this.woWPluginsToolStripMenuItem.Name = "woWPluginsToolStripMenuItem";
            this.woWPluginsToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.metroTabPage1);
            this.tabControl.Controls.Add(this.metroTabPage3);
            this.tabControl.Controls.Add(this.metroTabPage2);
            this.tabControl.CustomBackground = false;
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.FontSize = MetroFramework.MetroTabControlSize.Medium;
            this.tabControl.FontWeight = MetroFramework.MetroTabControlWeight.Bold;
            this.tabControl.HotTrack = true;
            this.tabControl.ItemSize = new System.Drawing.Size(148, 31);
            this.tabControl.Location = new System.Drawing.Point(20, 60);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(429, 195);
            this.tabControl.Style = MetroFramework.MetroColorStyle.Blue;
            this.tabControl.StyleManager = this.metroStyleManager1;
            this.tabControl.TabIndex = 59;
            this.tabControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tabControl.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tabControl.UseStyleColors = true;
            // 
            // metroTabPage1
            // 
            this.metroTabPage1.Controls.Add(this.progressBarAddonsBackup);
            this.metroTabPage1.Controls.Add(this.linkEditWowAccounts);
            this.metroTabPage1.Controls.Add(this.buttonLaunchWowWithoutAutopass);
            this.metroTabPage1.Controls.Add(this.linkClickerSettings);
            this.metroTabPage1.Controls.Add(this.linkBackup);
            this.metroTabPage1.Controls.Add(this.linkOpenBackupFolder);
            this.metroTabPage1.Controls.Add(this.cmbboxAccSelect);
            this.metroTabPage1.CustomBackground = false;
            this.metroTabPage1.HorizontalScrollbar = false;
            this.metroTabPage1.HorizontalScrollbarBarColor = true;
            this.metroTabPage1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.HorizontalScrollbarSize = 10;
            this.metroTabPage1.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(421, 156);
            this.metroTabPage1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPage1.StyleManager = this.metroStyleManager1;
            this.metroTabPage1.TabIndex = 0;
            this.metroTabPage1.Text = "             Main           ";
            this.metroTabPage1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTabPage1.VerticalScrollbar = false;
            this.metroTabPage1.VerticalScrollbarBarColor = true;
            this.metroTabPage1.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.VerticalScrollbarSize = 10;
            // 
            // progressBarAddonsBackup
            // 
            this.progressBarAddonsBackup.FontSize = MetroFramework.MetroProgressBarSize.Medium;
            this.progressBarAddonsBackup.FontWeight = MetroFramework.MetroProgressBarWeight.Light;
            this.progressBarAddonsBackup.HideProgressText = false;
            this.progressBarAddonsBackup.Location = new System.Drawing.Point(3, 107);
            this.progressBarAddonsBackup.Name = "progressBarAddonsBackup";
            this.progressBarAddonsBackup.ProgressBarStyle = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarAddonsBackup.Size = new System.Drawing.Size(92, 23);
            this.progressBarAddonsBackup.Style = MetroFramework.MetroColorStyle.Blue;
            this.progressBarAddonsBackup.StyleManager = null;
            this.progressBarAddonsBackup.TabIndex = 64;
            this.progressBarAddonsBackup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.progressBarAddonsBackup.Theme = MetroFramework.MetroThemeStyle.Light;
            this.progressBarAddonsBackup.Value = 100;
            // 
            // linkEditWowAccounts
            // 
            this.linkEditWowAccounts.CustomBackground = false;
            this.linkEditWowAccounts.CustomForeColor = false;
            this.linkEditWowAccounts.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkEditWowAccounts.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.linkEditWowAccounts.Location = new System.Drawing.Point(151, 47);
            this.linkEditWowAccounts.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkEditWowAccounts.Name = "linkEditWowAccounts";
            this.linkEditWowAccounts.Size = new System.Drawing.Size(116, 23);
            this.linkEditWowAccounts.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkEditWowAccounts.StyleManager = null;
            this.linkEditWowAccounts.TabIndex = 59;
            this.linkEditWowAccounts.Text = "Edit WoW accounts";
            this.linkEditWowAccounts.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkEditWowAccounts.UseStyleColors = true;
            this.linkEditWowAccounts.Click += new System.EventHandler(this.linkEditWowAccounts_Click);
            // 
            // buttonLaunchWowWithoutAutopass
            // 
            this.buttonLaunchWowWithoutAutopass.Highlight = true;
            this.buttonLaunchWowWithoutAutopass.Location = new System.Drawing.Point(3, 73);
            this.buttonLaunchWowWithoutAutopass.Name = "buttonLaunchWowWithoutAutopass";
            this.buttonLaunchWowWithoutAutopass.Size = new System.Drawing.Size(415, 23);
            this.buttonLaunchWowWithoutAutopass.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonLaunchWowWithoutAutopass.StyleManager = null;
            this.buttonLaunchWowWithoutAutopass.TabIndex = 57;
            this.buttonLaunchWowWithoutAutopass.Text = "Launch WoW w/o autopass";
            this.buttonLaunchWowWithoutAutopass.Theme = MetroFramework.MetroThemeStyle.Light;
            this.buttonLaunchWowWithoutAutopass.Click += new System.EventHandler(this.buttonLaunchWowWithoutAutopass_Click);
            // 
            // linkClickerSettings
            // 
            this.linkClickerSettings.AutoSize = true;
            this.linkClickerSettings.CustomBackground = false;
            this.linkClickerSettings.CustomForeColor = false;
            this.linkClickerSettings.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkClickerSettings.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.linkClickerSettings.Location = new System.Drawing.Point(325, 133);
            this.linkClickerSettings.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkClickerSettings.Name = "linkClickerSettings";
            this.linkClickerSettings.Size = new System.Drawing.Size(93, 23);
            this.linkClickerSettings.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkClickerSettings.StyleManager = null;
            this.linkClickerSettings.TabIndex = 50;
            this.linkClickerSettings.Text = "Clicker settings";
            this.linkClickerSettings.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkClickerSettings.UseStyleColors = true;
            this.linkClickerSettings.Click += new System.EventHandler(this.linkClickerSettings_Click);
            // 
            // linkBackup
            // 
            this.linkBackup.AutoSize = true;
            this.linkBackup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.linkBackup.CustomBackground = false;
            this.linkBackup.CustomForeColor = false;
            this.linkBackup.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkBackup.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.linkBackup.Location = new System.Drawing.Point(3, 133);
            this.linkBackup.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkBackup.Name = "linkBackup";
            this.linkBackup.Size = new System.Drawing.Size(54, 23);
            this.linkBackup.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkBackup.StyleManager = null;
            this.linkBackup.TabIndex = 49;
            this.linkBackup.Text = "Backup";
            this.linkBackup.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkBackup.UseStyleColors = true;
            this.linkBackup.Click += new System.EventHandler(this.linkBackupAddons_Click);
            // 
            // linkOpenBackupFolder
            // 
            this.linkOpenBackupFolder.CustomBackground = false;
            this.linkOpenBackupFolder.CustomForeColor = false;
            this.linkOpenBackupFolder.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkOpenBackupFolder.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.linkOpenBackupFolder.Location = new System.Drawing.Point(149, 133);
            this.linkOpenBackupFolder.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkOpenBackupFolder.Name = "linkOpenBackupFolder";
            this.linkOpenBackupFolder.Size = new System.Drawing.Size(118, 23);
            this.linkOpenBackupFolder.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkOpenBackupFolder.StyleManager = null;
            this.linkOpenBackupFolder.TabIndex = 48;
            this.linkOpenBackupFolder.Text = "Open backup folder";
            this.linkOpenBackupFolder.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkOpenBackupFolder.UseStyleColors = true;
            this.linkOpenBackupFolder.Click += new System.EventHandler(this.linkOpenBackupFolder_Click);
            // 
            // cmbboxAccSelect
            // 
            this.cmbboxAccSelect.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbboxAccSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbboxAccSelect.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.cmbboxAccSelect.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.cmbboxAccSelect.FormattingEnabled = true;
            this.cmbboxAccSelect.ItemHeight = 23;
            this.cmbboxAccSelect.Location = new System.Drawing.Point(76, 15);
            this.cmbboxAccSelect.Name = "cmbboxAccSelect";
            this.cmbboxAccSelect.OverlayText = "Click to launch WoW using autopass...";
            this.cmbboxAccSelect.Size = new System.Drawing.Size(290, 29);
            this.cmbboxAccSelect.Style = MetroFramework.MetroColorStyle.Blue;
            this.cmbboxAccSelect.StyleManager = this.metroStyleManager1;
            this.cmbboxAccSelect.TabIndex = 5;
            this.cmbboxAccSelect.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cmbboxAccSelect.SelectedIndexChanged += new System.EventHandler(this.CmbboxAccSelectSelectedIndexChanged);
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.OwnerForm = this;
            this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // metroTabPage3
            // 
            this.metroTabPage3.Controls.Add(this.buttonStartStopPlugin);
            this.metroTabPage3.Controls.Add(this.checkBoxEnableCustomPlugins);
            this.metroTabPage3.Controls.Add(this.metroCheckBoxPluginShowIngameNotification);
            this.metroTabPage3.Controls.Add(this.metroButtonLuaConsole);
            this.metroTabPage3.Controls.Add(this.metroButtonRadar);
            this.metroTabPage3.Controls.Add(this.metroButtonBlackMarketTracker);
            this.metroTabPage3.Controls.Add(this.buttonUnloadInjector);
            this.metroTabPage3.Controls.Add(this.comboBoxWowPlugins);
            this.metroTabPage3.CustomBackground = false;
            this.metroTabPage3.HorizontalScrollbar = false;
            this.metroTabPage3.HorizontalScrollbarBarColor = true;
            this.metroTabPage3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage3.HorizontalScrollbarSize = 10;
            this.metroTabPage3.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage3.Name = "metroTabPage3";
            this.metroTabPage3.Size = new System.Drawing.Size(421, 156);
            this.metroTabPage3.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPage3.StyleManager = this.metroStyleManager1;
            this.metroTabPage3.TabIndex = 2;
            this.metroTabPage3.Text = "      WoW plug-ins    ";
            this.metroTabPage3.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTabPage3.VerticalScrollbar = false;
            this.metroTabPage3.VerticalScrollbarBarColor = true;
            this.metroTabPage3.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage3.VerticalScrollbarSize = 10;
            // 
            // buttonStartStopPlugin
            // 
            this.buttonStartStopPlugin.Highlight = true;
            this.buttonStartStopPlugin.Location = new System.Drawing.Point(338, 15);
            this.buttonStartStopPlugin.Name = "buttonStartStopPlugin";
            this.buttonStartStopPlugin.Size = new System.Drawing.Size(80, 50);
            this.buttonStartStopPlugin.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonStartStopPlugin.StyleManager = this.metroStyleManager1;
            this.buttonStartStopPlugin.TabIndex = 74;
            this.buttonStartStopPlugin.Text = "Start [Insert]";
            this.buttonStartStopPlugin.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.buttonStartStopPlugin, "Start/stop plugin");
            this.buttonStartStopPlugin.Click += new System.EventHandler(this.buttonStartStopPlugin_Click);
            // 
            // checkBoxEnableCustomPlugins
            // 
            this.checkBoxEnableCustomPlugins.AutoSize = true;
            this.checkBoxEnableCustomPlugins.CustomBackground = false;
            this.checkBoxEnableCustomPlugins.CustomForeColor = false;
            this.checkBoxEnableCustomPlugins.FontSize = MetroFramework.MetroLinkSize.Small;
            this.checkBoxEnableCustomPlugins.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.checkBoxEnableCustomPlugins.Location = new System.Drawing.Point(189, 50);
            this.checkBoxEnableCustomPlugins.Name = "checkBoxEnableCustomPlugins";
            this.checkBoxEnableCustomPlugins.Size = new System.Drawing.Size(143, 15);
            this.checkBoxEnableCustomPlugins.Style = MetroFramework.MetroColorStyle.Blue;
            this.checkBoxEnableCustomPlugins.StyleManager = null;
            this.checkBoxEnableCustomPlugins.TabIndex = 73;
            this.checkBoxEnableCustomPlugins.Text = "Enable custom plugins";
            this.checkBoxEnableCustomPlugins.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.checkBoxEnableCustomPlugins, "Enable plugins from \"plugins\" folder");
            this.checkBoxEnableCustomPlugins.UseStyleColors = true;
            this.checkBoxEnableCustomPlugins.UseVisualStyleBackColor = true;
            this.checkBoxEnableCustomPlugins.CheckedChanged += new System.EventHandler(this.checkBoxEnableCustomPlugins_CheckedChanged);
            // 
            // metroCheckBoxPluginShowIngameNotification
            // 
            this.metroCheckBoxPluginShowIngameNotification.AutoSize = true;
            this.metroCheckBoxPluginShowIngameNotification.CustomBackground = false;
            this.metroCheckBoxPluginShowIngameNotification.CustomForeColor = false;
            this.metroCheckBoxPluginShowIngameNotification.FontSize = MetroFramework.MetroLinkSize.Small;
            this.metroCheckBoxPluginShowIngameNotification.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.metroCheckBoxPluginShowIngameNotification.Location = new System.Drawing.Point(3, 50);
            this.metroCheckBoxPluginShowIngameNotification.Name = "metroCheckBoxPluginShowIngameNotification";
            this.metroCheckBoxPluginShowIngameNotification.Size = new System.Drawing.Size(159, 15);
            this.metroCheckBoxPluginShowIngameNotification.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroCheckBoxPluginShowIngameNotification.StyleManager = null;
            this.metroCheckBoxPluginShowIngameNotification.TabIndex = 72;
            this.metroCheckBoxPluginShowIngameNotification.Text = "Show ingame notification";
            this.metroCheckBoxPluginShowIngameNotification.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.metroCheckBoxPluginShowIngameNotification, "Show various plugins notifications in UIError channel");
            this.metroCheckBoxPluginShowIngameNotification.UseStyleColors = true;
            this.metroCheckBoxPluginShowIngameNotification.UseVisualStyleBackColor = true;
            this.metroCheckBoxPluginShowIngameNotification.CheckedChanged += new System.EventHandler(this.metroCheckBoxPluginShowIngameNotification_CheckedChanged);
            // 
            // metroButtonLuaConsole
            // 
            this.metroButtonLuaConsole.Highlight = true;
            this.metroButtonLuaConsole.Location = new System.Drawing.Point(199, 130);
            this.metroButtonLuaConsole.Name = "metroButtonLuaConsole";
            this.metroButtonLuaConsole.Size = new System.Drawing.Size(88, 23);
            this.metroButtonLuaConsole.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroButtonLuaConsole.StyleManager = this.metroStyleManager1;
            this.metroButtonLuaConsole.TabIndex = 71;
            this.metroButtonLuaConsole.Text = "Lua console";
            this.metroButtonLuaConsole.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroButtonLuaConsole.Click += new System.EventHandler(this.MetroButtonLuaConsoleClick);
            // 
            // metroButtonRadar
            // 
            this.metroButtonRadar.Highlight = true;
            this.metroButtonRadar.Location = new System.Drawing.Point(132, 130);
            this.metroButtonRadar.Name = "metroButtonRadar";
            this.metroButtonRadar.Size = new System.Drawing.Size(61, 23);
            this.metroButtonRadar.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroButtonRadar.StyleManager = this.metroStyleManager1;
            this.metroButtonRadar.TabIndex = 70;
            this.metroButtonRadar.Text = "Radar";
            this.metroButtonRadar.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroButtonRadar.Click += new System.EventHandler(this.MetroButtonRadarClick);
            // 
            // metroButtonBlackMarketTracker
            // 
            this.metroButtonBlackMarketTracker.Highlight = true;
            this.metroButtonBlackMarketTracker.Location = new System.Drawing.Point(3, 130);
            this.metroButtonBlackMarketTracker.Name = "metroButtonBlackMarketTracker";
            this.metroButtonBlackMarketTracker.Size = new System.Drawing.Size(123, 23);
            this.metroButtonBlackMarketTracker.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroButtonBlackMarketTracker.StyleManager = this.metroStyleManager1;
            this.metroButtonBlackMarketTracker.TabIndex = 16;
            this.metroButtonBlackMarketTracker.Text = "Black Market tracker";
            this.metroButtonBlackMarketTracker.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroButtonBlackMarketTracker.Click += new System.EventHandler(this.MetroButtonBlackMarketTrackerClick);
            // 
            // buttonUnloadInjector
            // 
            this.buttonUnloadInjector.Highlight = true;
            this.buttonUnloadInjector.Location = new System.Drawing.Point(293, 130);
            this.buttonUnloadInjector.Name = "buttonUnloadInjector";
            this.buttonUnloadInjector.Size = new System.Drawing.Size(125, 23);
            this.buttonUnloadInjector.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonUnloadInjector.StyleManager = this.metroStyleManager1;
            this.buttonUnloadInjector.TabIndex = 15;
            this.buttonUnloadInjector.Text = "Unload injector";
            this.buttonUnloadInjector.Theme = MetroFramework.MetroThemeStyle.Light;
            this.buttonUnloadInjector.Click += new System.EventHandler(this.ButtonUnloadInjectorClick);
            // 
            // comboBoxWowPlugins
            // 
            this.comboBoxWowPlugins.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxWowPlugins.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWowPlugins.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.comboBoxWowPlugins.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.comboBoxWowPlugins.FormattingEnabled = true;
            this.comboBoxWowPlugins.ItemHeight = 23;
            this.comboBoxWowPlugins.Items.AddRange(new object[] {
            "Fishing",
            "Capture flags/orbs on the battlefields",
            "Milling/disenchanting/prospecting"});
            this.comboBoxWowPlugins.Location = new System.Drawing.Point(3, 15);
            this.comboBoxWowPlugins.Name = "comboBoxWowPlugins";
            this.comboBoxWowPlugins.OverlayText = "Click to select plugin...";
            this.comboBoxWowPlugins.Size = new System.Drawing.Size(329, 29);
            this.comboBoxWowPlugins.Style = MetroFramework.MetroColorStyle.Blue;
            this.comboBoxWowPlugins.StyleManager = this.metroStyleManager1;
            this.comboBoxWowPlugins.TabIndex = 6;
            this.comboBoxWowPlugins.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.comboBoxWowPlugins, "Click to select plugin");
            this.comboBoxWowPlugins.SelectedIndexChanged += new System.EventHandler(this.ComboBoxWowPluginsSelectedIndexChanged);
            // 
            // metroTabPage2
            // 
            this.metroTabPage2.Controls.Add(this.checkBoxStartTeamspeak3WithWow);
            this.metroTabPage2.Controls.Add(this.checkBoxStartMumbleWithWow);
            this.metroTabPage2.Controls.Add(this.checkBoxStartRaidcallWithWow);
            this.metroTabPage2.Controls.Add(this.checkBoxStartVenriloWithWow);
            this.metroTabPage2.Controls.Add(this.tileRaidcall);
            this.metroTabPage2.Controls.Add(this.tileTeamspeak3);
            this.metroTabPage2.Controls.Add(this.tileMumble);
            this.metroTabPage2.Controls.Add(this.tileVentrilo);
            this.metroTabPage2.CustomBackground = false;
            this.metroTabPage2.HorizontalScrollbar = true;
            this.metroTabPage2.HorizontalScrollbarBarColor = true;
            this.metroTabPage2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.HorizontalScrollbarSize = 10;
            this.metroTabPage2.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage2.Name = "metroTabPage2";
            this.metroTabPage2.Size = new System.Drawing.Size(421, 156);
            this.metroTabPage2.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPage2.StyleManager = this.metroStyleManager1;
            this.metroTabPage2.TabIndex = 1;
            this.metroTabPage2.Text = "             VoIP           ";
            this.metroTabPage2.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTabPage2.VerticalScrollbar = true;
            this.metroTabPage2.VerticalScrollbarBarColor = true;
            this.metroTabPage2.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.VerticalScrollbarSize = 10;
            // 
            // checkBoxStartTeamspeak3WithWow
            // 
            this.checkBoxStartTeamspeak3WithWow.AutoSize = true;
            this.checkBoxStartTeamspeak3WithWow.CustomBackground = false;
            this.checkBoxStartTeamspeak3WithWow.CustomForeColor = false;
            this.checkBoxStartTeamspeak3WithWow.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.checkBoxStartTeamspeak3WithWow.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.checkBoxStartTeamspeak3WithWow.Location = new System.Drawing.Point(241, 112);
            this.checkBoxStartTeamspeak3WithWow.Name = "checkBoxStartTeamspeak3WithWow";
            this.checkBoxStartTeamspeak3WithWow.Size = new System.Drawing.Size(148, 19);
            this.checkBoxStartTeamspeak3WithWow.Style = MetroFramework.MetroColorStyle.Blue;
            this.checkBoxStartTeamspeak3WithWow.StyleManager = this.metroStyleManager1;
            this.checkBoxStartTeamspeak3WithWow.TabIndex = 46;
            this.checkBoxStartTeamspeak3WithWow.Text = "Start TS3 with WoW";
            this.checkBoxStartTeamspeak3WithWow.Theme = MetroFramework.MetroThemeStyle.Light;
            this.checkBoxStartTeamspeak3WithWow.UseStyleColors = true;
            this.checkBoxStartTeamspeak3WithWow.UseVisualStyleBackColor = true;
            this.checkBoxStartTeamspeak3WithWow.CheckedChanged += new System.EventHandler(this.checkBoxStartTeamspeak3WithWow_CheckedChanged);
            // 
            // checkBoxStartMumbleWithWow
            // 
            this.checkBoxStartMumbleWithWow.AutoSize = true;
            this.checkBoxStartMumbleWithWow.CustomBackground = false;
            this.checkBoxStartMumbleWithWow.CustomForeColor = false;
            this.checkBoxStartMumbleWithWow.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.checkBoxStartMumbleWithWow.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.checkBoxStartMumbleWithWow.Location = new System.Drawing.Point(241, 87);
            this.checkBoxStartMumbleWithWow.Name = "checkBoxStartMumbleWithWow";
            this.checkBoxStartMumbleWithWow.Size = new System.Drawing.Size(177, 19);
            this.checkBoxStartMumbleWithWow.Style = MetroFramework.MetroColorStyle.Blue;
            this.checkBoxStartMumbleWithWow.StyleManager = null;
            this.checkBoxStartMumbleWithWow.TabIndex = 47;
            this.checkBoxStartMumbleWithWow.Text = "Start Mumble with WoW";
            this.checkBoxStartMumbleWithWow.Theme = MetroFramework.MetroThemeStyle.Light;
            this.checkBoxStartMumbleWithWow.UseStyleColors = true;
            this.checkBoxStartMumbleWithWow.UseVisualStyleBackColor = true;
            this.checkBoxStartMumbleWithWow.CheckedChanged += new System.EventHandler(this.checkBoxStartMumbleWithWow_CheckedChanged);
            // 
            // checkBoxStartRaidcallWithWow
            // 
            this.checkBoxStartRaidcallWithWow.AutoSize = true;
            this.checkBoxStartRaidcallWithWow.CustomBackground = false;
            this.checkBoxStartRaidcallWithWow.CustomForeColor = false;
            this.checkBoxStartRaidcallWithWow.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.checkBoxStartRaidcallWithWow.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.checkBoxStartRaidcallWithWow.Location = new System.Drawing.Point(241, 62);
            this.checkBoxStartRaidcallWithWow.Name = "checkBoxStartRaidcallWithWow";
            this.checkBoxStartRaidcallWithWow.Size = new System.Drawing.Size(171, 19);
            this.checkBoxStartRaidcallWithWow.Style = MetroFramework.MetroColorStyle.Blue;
            this.checkBoxStartRaidcallWithWow.StyleManager = null;
            this.checkBoxStartRaidcallWithWow.TabIndex = 45;
            this.checkBoxStartRaidcallWithWow.Text = "Start Raidcall with WoW";
            this.checkBoxStartRaidcallWithWow.Theme = MetroFramework.MetroThemeStyle.Light;
            this.checkBoxStartRaidcallWithWow.UseStyleColors = true;
            this.checkBoxStartRaidcallWithWow.UseVisualStyleBackColor = true;
            this.checkBoxStartRaidcallWithWow.CheckedChanged += new System.EventHandler(this.checkBoxStartRaidcallWithWow_CheckedChanged);
            // 
            // checkBoxStartVenriloWithWow
            // 
            this.checkBoxStartVenriloWithWow.AutoSize = true;
            this.checkBoxStartVenriloWithWow.CustomBackground = false;
            this.checkBoxStartVenriloWithWow.CustomForeColor = false;
            this.checkBoxStartVenriloWithWow.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.checkBoxStartVenriloWithWow.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.checkBoxStartVenriloWithWow.Location = new System.Drawing.Point(241, 37);
            this.checkBoxStartVenriloWithWow.Name = "checkBoxStartVenriloWithWow";
            this.checkBoxStartVenriloWithWow.Size = new System.Drawing.Size(174, 19);
            this.checkBoxStartVenriloWithWow.Style = MetroFramework.MetroColorStyle.Blue;
            this.checkBoxStartVenriloWithWow.StyleManager = null;
            this.checkBoxStartVenriloWithWow.TabIndex = 44;
            this.checkBoxStartVenriloWithWow.Text = "Start Ventrilo with WoW";
            this.checkBoxStartVenriloWithWow.Theme = MetroFramework.MetroThemeStyle.Light;
            this.checkBoxStartVenriloWithWow.UseStyleColors = true;
            this.checkBoxStartVenriloWithWow.UseVisualStyleBackColor = true;
            this.checkBoxStartVenriloWithWow.CheckedChanged += new System.EventHandler(this.checkBoxStartVenriloWithWow_CheckedChanged);
            // 
            // tileRaidcall
            // 
            this.tileRaidcall.ActiveControl = null;
            this.tileRaidcall.CenterText = "Raidcall";
            this.tileRaidcall.CustomBackground = false;
            this.tileRaidcall.CustomForeColor = false;
            this.tileRaidcall.Location = new System.Drawing.Point(125, 15);
            this.tileRaidcall.Name = "tileRaidcall";
            this.tileRaidcall.PaintTileCount = true;
            this.tileRaidcall.Size = new System.Drawing.Size(110, 66);
            this.tileRaidcall.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileRaidcall.StyleManager = this.metroStyleManager1;
            this.tileRaidcall.TabIndex = 8;
            this.tileRaidcall.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileRaidcall.TileCount = 0;
            this.tileRaidcall.Click += new System.EventHandler(this.TileRaidcallClick);
            // 
            // tileTeamspeak3
            // 
            this.tileTeamspeak3.ActiveControl = null;
            this.tileTeamspeak3.CenterText = "Teamspeak 3";
            this.tileTeamspeak3.CustomBackground = false;
            this.tileTeamspeak3.CustomForeColor = false;
            this.tileTeamspeak3.Location = new System.Drawing.Point(3, 87);
            this.tileTeamspeak3.Name = "tileTeamspeak3";
            this.tileTeamspeak3.PaintTileCount = true;
            this.tileTeamspeak3.Size = new System.Drawing.Size(116, 66);
            this.tileTeamspeak3.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileTeamspeak3.StyleManager = this.metroStyleManager1;
            this.tileTeamspeak3.TabIndex = 7;
            this.tileTeamspeak3.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileTeamspeak3.TileCount = 0;
            this.tileTeamspeak3.Click += new System.EventHandler(this.TileTeamspeak3Click);
            // 
            // tileMumble
            // 
            this.tileMumble.ActiveControl = null;
            this.tileMumble.CenterText = "Mumble";
            this.tileMumble.CustomBackground = false;
            this.tileMumble.CustomForeColor = false;
            this.tileMumble.Location = new System.Drawing.Point(125, 87);
            this.tileMumble.Name = "tileMumble";
            this.tileMumble.PaintTileCount = true;
            this.tileMumble.Size = new System.Drawing.Size(110, 66);
            this.tileMumble.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileMumble.StyleManager = this.metroStyleManager1;
            this.tileMumble.TabIndex = 6;
            this.tileMumble.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileMumble.TileCount = 0;
            this.tileMumble.Click += new System.EventHandler(this.TileMumbleClick);
            // 
            // tileVentrilo
            // 
            this.tileVentrilo.ActiveControl = null;
            this.tileVentrilo.CenterText = "Ventrilo";
            this.tileVentrilo.CustomBackground = false;
            this.tileVentrilo.CustomForeColor = false;
            this.tileVentrilo.Location = new System.Drawing.Point(3, 15);
            this.tileVentrilo.Name = "tileVentrilo";
            this.tileVentrilo.PaintTileCount = true;
            this.tileVentrilo.Size = new System.Drawing.Size(116, 66);
            this.tileVentrilo.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileVentrilo.StyleManager = this.metroStyleManager1;
            this.tileVentrilo.TabIndex = 5;
            this.tileVentrilo.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileVentrilo.TileCount = 0;
            this.tileVentrilo.Click += new System.EventHandler(this.TileVentriloClick);
            // 
            // labelPingNum
            // 
            this.labelPingNum.AutoSize = true;
            this.labelPingNum.CustomBackground = false;
            this.labelPingNum.CustomForeColor = false;
            this.labelPingNum.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.labelPingNum.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.labelPingNum.ForeColor = System.Drawing.Color.BlueViolet;
            this.labelPingNum.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.labelPingNum.Location = new System.Drawing.Point(184, 6);
            this.labelPingNum.Name = "labelPingNum";
            this.labelPingNum.Size = new System.Drawing.Size(108, 19);
            this.labelPingNum.Style = MetroFramework.MetroColorStyle.Blue;
            this.labelPingNum.StyleManager = this.metroStyleManager1;
            this.labelPingNum.TabIndex = 63;
            this.labelPingNum.Text = "[999ms]::[100%]";
            this.labelPingNum.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.labelPingNum, "Click to show connectivity info widget");
            this.labelPingNum.UseStyleColors = true;
            this.labelPingNum.Visible = false;
            this.labelPingNum.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LabelPingNumMouseClick);
            // 
            // metroToolTip1
            // 
            this.metroToolTip1.AutoPopDelay = 10000;
            this.metroToolTip1.InitialDelay = 500;
            this.metroToolTip1.ReshowDelay = 100;
            this.metroToolTip1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroToolTip1.StyleManager = null;
            this.metroToolTip1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // linkSettings
            // 
            this.linkSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.linkSettings.CustomBackground = false;
            this.linkSettings.CustomForeColor = false;
            this.linkSettings.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkSettings.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.linkSettings.Location = new System.Drawing.Point(349, 5);
            this.linkSettings.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkSettings.Name = "linkSettings";
            this.linkSettings.Size = new System.Drawing.Size(53, 20);
            this.linkSettings.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkSettings.StyleManager = this.metroStyleManager1;
            this.linkSettings.TabIndex = 65;
            this.linkSettings.Text = "settings";
            this.linkSettings.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkSettings.UseStyleColors = true;
            this.linkSettings.Click += new System.EventHandler(this.linkSettings_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(469, 265);
            this.Controls.Add(this.linkSettings);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.labelPingNum);
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(20, 60, 20, 10);
            this.Resizable = false;
            this.StyleManager = this.metroStyleManager1;
            this.Text = "AxTools";
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.contextMenuStripMain.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.metroTabPage1.ResumeLayout(false);
            this.metroTabPage1.PerformLayout();
            this.metroTabPage3.ResumeLayout(false);
            this.metroTabPage3.PerformLayout();
            this.metroTabPage2.ResumeLayout(false);
            this.metroTabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private NotifyIcon notifyIconMain;
        private MetroTabControl tabControl;
        private MetroTabPage metroTabPage1;
        private MetroTabPage metroTabPage2;
        private MetroTileExt tileRaidcall;
        private MetroTileExt tileTeamspeak3;
        private MetroTileExt tileMumble;
        private MetroTileExt tileVentrilo;
        private MetroTabPage metroTabPage3;
        private MetroComboboxExt comboBoxWowPlugins;
        private MetroComboboxExt cmbboxAccSelect;
        private MetroButton buttonUnloadInjector;
        private MetroLabel labelPingNum;
        private MetroToolTip metroToolTip1;
        private MetroStyleManager metroStyleManager1;
        private ContextMenuStrip contextMenuStripMain;
        private ToolStripMenuItem woWRadarToolStripMenuItem;
        private ToolStripMenuItem woWPluginsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem launchWoWToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem luaConsoleToolStripMenuItem;
        private MetroButton metroButtonBlackMarketTracker;
        private MetroButton metroButtonLuaConsole;
        private MetroButton metroButtonRadar;
        private MetroCheckBox checkBoxStartVenriloWithWow;
        private MetroCheckBox checkBoxStartRaidcallWithWow;
        private MetroCheckBox checkBoxStartTeamspeak3WithWow;
        private MetroCheckBox checkBoxStartMumbleWithWow;
        private ToolStripMenuItem stopActivePluginorPresshotkeyToolStripMenuItem;
        private ToolStripMenuItem blackMarketTrackerToolStripMenuItem;
        private MetroCheckBox metroCheckBoxPluginShowIngameNotification;
        private MetroLink linkOpenBackupFolder;
        private MetroLink linkBackup;
        private MetroLink linkClickerSettings;
        private MetroCheckBox checkBoxEnableCustomPlugins;
        private MetroButton buttonStartStopPlugin;
        private MetroButton buttonLaunchWowWithoutAutopass;
        private MetroLink linkEditWowAccounts;
        private MetroProgressBar progressBarAddonsBackup;
        private MetroLink linkSettings;
    }
}

