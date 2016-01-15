using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using AxTools.Components;
using AxTools.Properties;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Controls;
using Settings = AxTools.Helpers.Settings;

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
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager();
            this.linkEditWowAccounts = new MetroFramework.Controls.MetroLink();
            this.linkClickerSettings = new MetroFramework.Controls.MetroLink();
            this.linkBackup = new MetroFramework.Controls.MetroLink();
            this.cmbboxAccSelect = new AxTools.Components.MetroComboboxExt(this.components);
            this.tabPageModules = new MetroFramework.Controls.MetroTabPage();
            this.tileBMTracker = new AxTools.Components.MetroTileExt(this.components);
            this.tileRadar = new AxTools.Components.MetroTileExt(this.components);
            this.tileLuaConsole = new AxTools.Components.MetroTileExt(this.components);
            this.metroTabPage3 = new MetroFramework.Controls.MetroTabPage();
            this.olvPlugins = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.buttonPluginSettings = new System.Windows.Forms.Button();
            this.buttonStartStopPlugin = new MetroFramework.Controls.MetroButton();
            this.labelPluginDesc = new MetroFramework.Controls.MetroLabel();
            this.metroTabPage2 = new MetroFramework.Controls.MetroTabPage();
            this.checkBoxStartTeamspeak3WithWow = new MetroFramework.Controls.MetroCheckBox();
            this.checkBoxStartMumbleWithWow = new MetroFramework.Controls.MetroCheckBox();
            this.checkBoxStartRaidcallWithWow = new MetroFramework.Controls.MetroCheckBox();
            this.checkBoxStartVenriloWithWow = new MetroFramework.Controls.MetroCheckBox();
            this.tileRaidcall = new AxTools.Components.MetroTileExt(this.components);
            this.tileTeamspeak3 = new AxTools.Components.MetroTileExt(this.components);
            this.tileMumble = new AxTools.Components.MetroTileExt(this.components);
            this.tileVentrilo = new AxTools.Components.MetroTileExt(this.components);
            this.metroToolTip1 = new MetroFramework.Components.MetroToolTip();
            this.linkPing = new MetroFramework.Controls.MetroLink();
            this.linkSettings = new MetroFramework.Controls.MetroLink();
            this.contextMenuStripBackupAndClean = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemBackupWoWAddOns = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemOpenBackupFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemOpenWoWLogsFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkTitle = new MetroFramework.Controls.MetroLink();
            this.contextMenuStripMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.metroTabPage1.SuspendLayout();
            this.tabPageModules.SuspendLayout();
            this.metroTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvPlugins)).BeginInit();
            this.metroTabPage2.SuspendLayout();
            this.contextMenuStripBackupAndClean.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.tabControl.Controls.Add(this.tabPageModules);
            this.tabControl.Controls.Add(this.metroTabPage3);
            this.tabControl.Controls.Add(this.metroTabPage2);
            this.tabControl.CustomBackground = false;
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.FontSize = MetroFramework.MetroTabControlSize.Medium;
            this.tabControl.FontWeight = MetroFramework.MetroTabControlWeight.Bold;
            this.tabControl.HotTrack = true;
            this.tabControl.ItemSize = new System.Drawing.Size(148, 31);
            this.tabControl.Location = new System.Drawing.Point(20, 30);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 1;
            this.tabControl.Size = new System.Drawing.Size(429, 199);
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
            this.metroTabPage1.Controls.Add(this.linkClickerSettings);
            this.metroTabPage1.Controls.Add(this.linkBackup);
            this.metroTabPage1.Controls.Add(this.cmbboxAccSelect);
            this.metroTabPage1.CustomBackground = false;
            this.metroTabPage1.HorizontalScrollbar = false;
            this.metroTabPage1.HorizontalScrollbarBarColor = true;
            this.metroTabPage1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.HorizontalScrollbarSize = 10;
            this.metroTabPage1.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(421, 160);
            this.metroTabPage1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPage1.StyleManager = this.metroStyleManager1;
            this.metroTabPage1.TabIndex = 0;
            this.metroTabPage1.Text = "         Home       ";
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
            this.progressBarAddonsBackup.StyleManager = this.metroStyleManager1;
            this.progressBarAddonsBackup.TabIndex = 64;
            this.progressBarAddonsBackup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.progressBarAddonsBackup.Theme = MetroFramework.MetroThemeStyle.Light;
            this.progressBarAddonsBackup.Value = 100;
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.OwnerForm = this;
            this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // linkEditWowAccounts
            // 
            this.linkEditWowAccounts.CustomBackground = false;
            this.linkEditWowAccounts.CustomForeColor = false;
            this.linkEditWowAccounts.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkEditWowAccounts.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.linkEditWowAccounts.Location = new System.Drawing.Point(151, 69);
            this.linkEditWowAccounts.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkEditWowAccounts.Name = "linkEditWowAccounts";
            this.linkEditWowAccounts.Size = new System.Drawing.Size(116, 23);
            this.linkEditWowAccounts.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkEditWowAccounts.StyleManager = this.metroStyleManager1;
            this.linkEditWowAccounts.TabIndex = 59;
            this.linkEditWowAccounts.Text = "Edit WoW accounts";
            this.linkEditWowAccounts.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkEditWowAccounts.UseStyleColors = true;
            this.linkEditWowAccounts.Click += new System.EventHandler(this.linkEditWowAccounts_Click);
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
            this.linkClickerSettings.StyleManager = this.metroStyleManager1;
            this.linkClickerSettings.TabIndex = 50;
            this.linkClickerSettings.Text = "Clicker settings";
            this.linkClickerSettings.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkClickerSettings.UseStyleColors = true;
            this.linkClickerSettings.Click += new System.EventHandler(this.linkClickerSettings_Click);
            // 
            // linkBackup
            // 
            this.linkBackup.AutoSize = true;
            this.linkBackup.CustomBackground = false;
            this.linkBackup.CustomForeColor = false;
            this.linkBackup.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkBackup.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.linkBackup.Location = new System.Drawing.Point(3, 133);
            this.linkBackup.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkBackup.Name = "linkBackup";
            this.linkBackup.Size = new System.Drawing.Size(59, 23);
            this.linkBackup.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkBackup.StyleManager = this.metroStyleManager1;
            this.linkBackup.TabIndex = 49;
            this.linkBackup.Text = "Backups";
            this.linkBackup.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkBackup.UseStyleColors = true;
            this.linkBackup.Click += new System.EventHandler(this.linkBackupAddons_Click);
            // 
            // cmbboxAccSelect
            // 
            this.cmbboxAccSelect.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbboxAccSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbboxAccSelect.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.cmbboxAccSelect.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.cmbboxAccSelect.FormattingEnabled = true;
            this.cmbboxAccSelect.ItemHeight = 23;
            this.cmbboxAccSelect.Location = new System.Drawing.Point(76, 37);
            this.cmbboxAccSelect.Name = "cmbboxAccSelect";
            this.cmbboxAccSelect.OverlayText = "Click to launch WoW using autopass...";
            this.cmbboxAccSelect.Size = new System.Drawing.Size(290, 29);
            this.cmbboxAccSelect.Style = MetroFramework.MetroColorStyle.Blue;
            this.cmbboxAccSelect.StyleManager = this.metroStyleManager1;
            this.cmbboxAccSelect.TabIndex = 5;
            this.cmbboxAccSelect.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cmbboxAccSelect.SelectedIndexChanged += new System.EventHandler(this.CmbboxAccSelectSelectedIndexChanged);
            // 
            // tabPageModules
            // 
            this.tabPageModules.Controls.Add(this.tileBMTracker);
            this.tabPageModules.Controls.Add(this.tileRadar);
            this.tabPageModules.Controls.Add(this.tileLuaConsole);
            this.tabPageModules.CustomBackground = false;
            this.tabPageModules.HorizontalScrollbar = false;
            this.tabPageModules.HorizontalScrollbarBarColor = true;
            this.tabPageModules.HorizontalScrollbarHighlightOnWheel = false;
            this.tabPageModules.HorizontalScrollbarSize = 10;
            this.tabPageModules.Location = new System.Drawing.Point(4, 35);
            this.tabPageModules.Name = "tabPageModules";
            this.tabPageModules.Size = new System.Drawing.Size(421, 160);
            this.tabPageModules.Style = MetroFramework.MetroColorStyle.Blue;
            this.tabPageModules.StyleManager = null;
            this.tabPageModules.TabIndex = 3;
            this.tabPageModules.Text = "      Modules    ";
            this.tabPageModules.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tabPageModules.VerticalScrollbar = false;
            this.tabPageModules.VerticalScrollbarBarColor = true;
            this.tabPageModules.VerticalScrollbarHighlightOnWheel = false;
            this.tabPageModules.VerticalScrollbarSize = 10;
            // 
            // tileBMTracker
            // 
            this.tileBMTracker.ActiveControl = null;
            this.tileBMTracker.CenterText = "Black market tracker";
            this.tileBMTracker.CustomBackground = false;
            this.tileBMTracker.CustomForeColor = false;
            this.tileBMTracker.Location = new System.Drawing.Point(3, 101);
            this.tileBMTracker.Name = "tileBMTracker";
            this.tileBMTracker.PaintTileCount = true;
            this.tileBMTracker.Size = new System.Drawing.Size(415, 52);
            this.tileBMTracker.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileBMTracker.StyleManager = this.metroStyleManager1;
            this.tileBMTracker.TabIndex = 8;
            this.tileBMTracker.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileBMTracker.TileCount = 0;
            this.tileBMTracker.Click += new System.EventHandler(this.tileBMTracker_Click);
            // 
            // tileRadar
            // 
            this.tileRadar.ActiveControl = null;
            this.tileRadar.CenterText = "Radar";
            this.tileRadar.CustomBackground = false;
            this.tileRadar.CustomForeColor = false;
            this.tileRadar.Location = new System.Drawing.Point(3, 15);
            this.tileRadar.Name = "tileRadar";
            this.tileRadar.PaintTileCount = true;
            this.tileRadar.Size = new System.Drawing.Size(205, 80);
            this.tileRadar.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileRadar.StyleManager = this.metroStyleManager1;
            this.tileRadar.TabIndex = 7;
            this.tileRadar.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileRadar.TileCount = 0;
            this.tileRadar.Click += new System.EventHandler(this.tileRadar_Click);
            // 
            // tileLuaConsole
            // 
            this.tileLuaConsole.ActiveControl = null;
            this.tileLuaConsole.CenterText = "Lua console";
            this.tileLuaConsole.CustomBackground = false;
            this.tileLuaConsole.CustomForeColor = false;
            this.tileLuaConsole.Location = new System.Drawing.Point(214, 15);
            this.tileLuaConsole.Name = "tileLuaConsole";
            this.tileLuaConsole.PaintTileCount = true;
            this.tileLuaConsole.Size = new System.Drawing.Size(204, 80);
            this.tileLuaConsole.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileLuaConsole.StyleManager = this.metroStyleManager1;
            this.tileLuaConsole.TabIndex = 6;
            this.tileLuaConsole.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileLuaConsole.TileCount = 0;
            this.tileLuaConsole.Click += new System.EventHandler(this.tileLuaConsole_Click);
            // 
            // metroTabPage3
            // 
            this.metroTabPage3.Controls.Add(this.olvPlugins);
            this.metroTabPage3.Controls.Add(this.buttonPluginSettings);
            this.metroTabPage3.Controls.Add(this.buttonStartStopPlugin);
            this.metroTabPage3.Controls.Add(this.labelPluginDesc);
            this.metroTabPage3.CustomBackground = false;
            this.metroTabPage3.HorizontalScrollbar = false;
            this.metroTabPage3.HorizontalScrollbarBarColor = true;
            this.metroTabPage3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage3.HorizontalScrollbarSize = 10;
            this.metroTabPage3.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage3.Name = "metroTabPage3";
            this.metroTabPage3.Size = new System.Drawing.Size(421, 160);
            this.metroTabPage3.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPage3.StyleManager = this.metroStyleManager1;
            this.metroTabPage3.TabIndex = 2;
            this.metroTabPage3.Text = "      Plug-ins    ";
            this.metroTabPage3.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTabPage3.VerticalScrollbar = false;
            this.metroTabPage3.VerticalScrollbarBarColor = true;
            this.metroTabPage3.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage3.VerticalScrollbarSize = 10;
            // 
            // olvPlugins
            // 
            this.olvPlugins.AllColumns.Add(this.olvColumn1);
            this.olvPlugins.AllColumns.Add(this.olvColumn2);
            this.olvPlugins.AllowColumnReorder = true;
            this.olvPlugins.CheckBoxes = true;
            this.olvPlugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2});
            this.olvPlugins.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvPlugins.FullRowSelect = true;
            this.olvPlugins.HeaderWordWrap = true;
            this.olvPlugins.HideSelection = false;
            this.olvPlugins.IncludeColumnHeadersInCopy = true;
            this.olvPlugins.Location = new System.Drawing.Point(3, 15);
            this.olvPlugins.Name = "olvPlugins";
            this.olvPlugins.ShowGroups = false;
            this.olvPlugins.Size = new System.Drawing.Size(294, 142);
            this.olvPlugins.TabIndex = 81;
            this.olvPlugins.UseAlternatingBackColors = true;
            this.olvPlugins.UseCompatibleStateImageBehavior = false;
            this.olvPlugins.UseFilterIndicator = true;
            this.olvPlugins.UseFiltering = true;
            this.olvPlugins.UseHotItem = true;
            this.olvPlugins.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.HeaderCheckBox = true;
            this.olvColumn1.MaximumWidth = 217;
            this.olvColumn1.MinimumWidth = 217;
            this.olvColumn1.Text = "Name";
            this.olvColumn1.Width = 217;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "ConfigAvailable";
            this.olvColumn2.MaximumWidth = 55;
            this.olvColumn2.MinimumWidth = 55;
            this.olvColumn2.Text = "Settings";
            this.olvColumn2.Width = 55;
            // 
            // buttonPluginSettings
            // 
            this.buttonPluginSettings.BackgroundImage = global::AxTools.Properties.Resources.pluginSettings;
            this.buttonPluginSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonPluginSettings.Enabled = false;
            this.buttonPluginSettings.Location = new System.Drawing.Point(303, 15);
            this.buttonPluginSettings.Name = "buttonPluginSettings";
            this.buttonPluginSettings.Size = new System.Drawing.Size(29, 29);
            this.buttonPluginSettings.TabIndex = 80;
            this.buttonPluginSettings.UseVisualStyleBackColor = true;
            this.buttonPluginSettings.Click += new System.EventHandler(this.buttonPluginSettings_Click);
            // 
            // buttonStartStopPlugin
            // 
            this.buttonStartStopPlugin.Highlight = true;
            this.buttonStartStopPlugin.Location = new System.Drawing.Point(338, 15);
            this.buttonStartStopPlugin.Name = "buttonStartStopPlugin";
            this.buttonStartStopPlugin.Size = new System.Drawing.Size(80, 29);
            this.buttonStartStopPlugin.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonStartStopPlugin.StyleManager = this.metroStyleManager1;
            this.buttonStartStopPlugin.TabIndex = 74;
            this.buttonStartStopPlugin.Text = "Start [Insert]";
            this.buttonStartStopPlugin.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.buttonStartStopPlugin, "Start/stop plugin");
            this.buttonStartStopPlugin.Click += new System.EventHandler(this.buttonStartStopPlugin_Click);
            // 
            // labelPluginDesc
            // 
            this.labelPluginDesc.AutoSize = true;
            this.labelPluginDesc.CustomBackground = false;
            this.labelPluginDesc.CustomForeColor = false;
            this.labelPluginDesc.FontSize = MetroFramework.MetroLabelSize.Small;
            this.labelPluginDesc.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.labelPluginDesc.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.labelPluginDesc.Location = new System.Drawing.Point(303, 47);
            this.labelPluginDesc.Name = "labelPluginDesc";
            this.labelPluginDesc.Size = new System.Drawing.Size(117, 105);
            this.labelPluginDesc.Style = MetroFramework.MetroColorStyle.Blue;
            this.labelPluginDesc.StyleManager = null;
            this.labelPluginDesc.TabIndex = 82;
            this.labelPluginDesc.Text = "Check plugins you\r\nwant to enable and\r\nthen click \"Start\"\r\nbutton to launch.\r\nDou" +
    "ble click on\r\na row to open\r\nsettings dialog";
            this.labelPluginDesc.Theme = MetroFramework.MetroThemeStyle.Light;
            this.labelPluginDesc.UseStyleColors = true;
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
            this.metroTabPage2.Size = new System.Drawing.Size(421, 160);
            this.metroTabPage2.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPage2.StyleManager = this.metroStyleManager1;
            this.metroTabPage2.TabIndex = 1;
            this.metroTabPage2.Text = "        VoIP     ";
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
            this.checkBoxStartMumbleWithWow.StyleManager = this.metroStyleManager1;
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
            this.checkBoxStartRaidcallWithWow.StyleManager = this.metroStyleManager1;
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
            this.checkBoxStartVenriloWithWow.StyleManager = this.metroStyleManager1;
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
            // metroToolTip1
            // 
            this.metroToolTip1.AutoPopDelay = 10000;
            this.metroToolTip1.InitialDelay = 500;
            this.metroToolTip1.ReshowDelay = 100;
            this.metroToolTip1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroToolTip1.StyleManager = null;
            this.metroToolTip1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // linkPing
            // 
            this.linkPing.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.linkPing.CustomBackground = false;
            this.linkPing.CustomForeColor = false;
            this.linkPing.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkPing.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.linkPing.Location = new System.Drawing.Point(253, 5);
            this.linkPing.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkPing.Name = "linkPing";
            this.linkPing.Size = new System.Drawing.Size(100, 20);
            this.linkPing.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkPing.StyleManager = this.metroStyleManager1;
            this.linkPing.TabIndex = 68;
            this.linkPing.Text = "[999ms]::[100%]  |";
            this.linkPing.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkPing.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.linkPing, "This is ingame connection info. It\'s formatted as\r\n  [worst ping of the last 10]:" +
        ":[packet loss in the last 200 seconds]  \r\nLeft-click to clear statistics\r\nRight-" +
        "click to open pinger settings");
            this.linkPing.UseStyleColors = true;
            this.linkPing.MouseDown += new System.Windows.Forms.MouseEventHandler(this.linkPing_MouseDown);
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
            this.metroToolTip1.SetToolTip(this.linkSettings, "Click to open settings dialog");
            this.linkSettings.UseStyleColors = true;
            this.linkSettings.Click += new System.EventHandler(this.linkSettings_Click);
            // 
            // contextMenuStripBackupAndClean
            // 
            this.contextMenuStripBackupAndClean.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemBackupWoWAddOns,
            this.toolStripMenuItemOpenBackupFolder,
            this.toolStripSeparator3,
            this.toolStripMenuItemOpenWoWLogsFolder});
            this.contextMenuStripBackupAndClean.Name = "contextMenuStripMain";
            this.contextMenuStripBackupAndClean.Size = new System.Drawing.Size(195, 76);
            // 
            // toolStripMenuItemBackupWoWAddOns
            // 
            this.toolStripMenuItemBackupWoWAddOns.Image = global::AxTools.Properties.Resources.data_backup;
            this.toolStripMenuItemBackupWoWAddOns.Name = "toolStripMenuItemBackupWoWAddOns";
            this.toolStripMenuItemBackupWoWAddOns.Size = new System.Drawing.Size(194, 22);
            this.toolStripMenuItemBackupWoWAddOns.Text = "Backup WoW AddOns";
            this.toolStripMenuItemBackupWoWAddOns.Click += new System.EventHandler(this.toolStripMenuItemBackupWoWAddOns_Click);
            // 
            // toolStripMenuItemOpenBackupFolder
            // 
            this.toolStripMenuItemOpenBackupFolder.Name = "toolStripMenuItemOpenBackupFolder";
            this.toolStripMenuItemOpenBackupFolder.Size = new System.Drawing.Size(194, 22);
            this.toolStripMenuItemOpenBackupFolder.Text = "Open backup folder";
            this.toolStripMenuItemOpenBackupFolder.Click += new System.EventHandler(this.toolStripMenuItemOpenBackupFolder_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(191, 6);
            // 
            // toolStripMenuItemOpenWoWLogsFolder
            // 
            this.toolStripMenuItemOpenWoWLogsFolder.Name = "toolStripMenuItemOpenWoWLogsFolder";
            this.toolStripMenuItemOpenWoWLogsFolder.Size = new System.Drawing.Size(194, 22);
            this.toolStripMenuItemOpenWoWLogsFolder.Text = "Open WoW logs folder";
            this.toolStripMenuItemOpenWoWLogsFolder.Click += new System.EventHandler(this.toolStripMenuItemOpenWoWLogsFolder_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::AxTools.Properties.Resources.AppIcon1;
            this.pictureBox1.Location = new System.Drawing.Point(13, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(20, 20);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 66;
            this.pictureBox1.TabStop = false;
            // 
            // linkTitle
            // 
            this.linkTitle.AutoSize = true;
            this.linkTitle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.linkTitle.CustomBackground = false;
            this.linkTitle.CustomForeColor = false;
            this.linkTitle.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkTitle.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.linkTitle.Location = new System.Drawing.Point(33, 5);
            this.linkTitle.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkTitle.Name = "linkTitle";
            this.linkTitle.Size = new System.Drawing.Size(55, 23);
            this.linkTitle.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkTitle.StyleManager = this.metroStyleManager1;
            this.linkTitle.TabIndex = 69;
            this.linkTitle.Text = "AxTools";
            this.linkTitle.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkTitle.UseStyleColors = true;
            this.linkTitle.Click += new System.EventHandler(this.linkTitle_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(469, 234);
            this.Controls.Add(this.linkTitle);
            this.Controls.Add(this.linkPing);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.linkSettings);
            this.Controls.Add(this.tabControl);
            this.DisplayHeader = false;
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 5);
            this.Resizable = false;
            this.StyleManager = this.metroStyleManager1;
            this.Text = "AxTools";
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.contextMenuStripMain.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.metroTabPage1.ResumeLayout(false);
            this.metroTabPage1.PerformLayout();
            this.tabPageModules.ResumeLayout(false);
            this.metroTabPage3.ResumeLayout(false);
            this.metroTabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvPlugins)).EndInit();
            this.metroTabPage2.ResumeLayout(false);
            this.metroTabPage2.PerformLayout();
            this.contextMenuStripBackupAndClean.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        internal NotifyIcon notifyIconMain;
        private MetroTabControl tabControl;
        private MetroTabPage metroTabPage1;
        private MetroTabPage metroTabPage2;
        private MetroTileExt tileRaidcall;
        private MetroTileExt tileTeamspeak3;
        private MetroTileExt tileMumble;
        private MetroTileExt tileVentrilo;
        private MetroTabPage metroTabPage3;
        private MetroComboboxExt cmbboxAccSelect;
        private MetroToolTip metroToolTip1;
        private MetroStyleManager metroStyleManager1;
        private ContextMenuStrip contextMenuStripMain;
        private ToolStripMenuItem woWRadarToolStripMenuItem;
        private ToolStripMenuItem woWPluginsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem launchWoWToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem luaConsoleToolStripMenuItem;
        private MetroCheckBox checkBoxStartVenriloWithWow;
        private MetroCheckBox checkBoxStartRaidcallWithWow;
        private MetroCheckBox checkBoxStartTeamspeak3WithWow;
        private MetroCheckBox checkBoxStartMumbleWithWow;
        private ToolStripMenuItem stopActivePluginorPresshotkeyToolStripMenuItem;
        private ToolStripMenuItem blackMarketTrackerToolStripMenuItem;
        private MetroLink linkBackup;
        private MetroLink linkClickerSettings;
        private MetroButton buttonStartStopPlugin;
        private MetroLink linkEditWowAccounts;
        private MetroProgressBar progressBarAddonsBackup;
        private MetroLink linkSettings;
        private ContextMenuStrip contextMenuStripBackupAndClean;
        private ToolStripMenuItem toolStripMenuItemBackupWoWAddOns;
        private ToolStripMenuItem toolStripMenuItemOpenBackupFolder;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItemOpenWoWLogsFolder;
        private PictureBox pictureBox1;
        private MetroLink linkPing;
        private MetroLink linkTitle;
        private Button buttonPluginSettings;
        private BrightIdeasSoftware.ObjectListView olvPlugins;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private MetroTabPage tabPageModules;
        private MetroTileExt tileBMTracker;
        private MetroTileExt tileRadar;
        private MetroTileExt tileLuaConsole;
        private MetroLabel labelPluginDesc;
    }
}

