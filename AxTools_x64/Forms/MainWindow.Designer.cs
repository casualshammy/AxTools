using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using Components;

using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Controls;
using Settings2 = AxTools.Helpers.Settings2;

namespace AxTools.Forms
{
    partial class MainWindow
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
            this.notifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabControl = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.progressBarAddonsBackup = new MetroFramework.Controls.MetroProgressBar();
            this.linkEditWowAccounts = new MetroFramework.Controls.MetroLink();
            this.linkClickerSettings = new MetroFramework.Controls.MetroLink();
            this.linkBackup = new MetroFramework.Controls.MetroLink();
            this.cmbboxAccSelect = new Components.MetroComboboxExt(this.components);
            this.metroTabPage3 = new MetroFramework.Controls.MetroTabPage();
            this.linkUpdatePlugins = new MetroFramework.Controls.MetroLink();
            this.labelTotalPluginsEnabled = new MetroFramework.Controls.MetroLabel();
            this.linkDownloadPlugins = new MetroFramework.Controls.MetroLink();
            this.olvPlugins = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.buttonStartStopPlugin = new MetroFramework.Controls.MetroButton();
            this.metroToolTip1 = new MetroFramework.Components.MetroToolTip();
            this.linkPing = new MetroFramework.Controls.MetroLink();
            this.linkSettings = new MetroFramework.Controls.MetroLink();
            this.contextMenuStripBackupAndClean = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemNextBackupTime = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBackupWoWAddOns = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDeployArchive = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemOpenWoWLogsFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkTitle = new MetroFramework.Controls.MetroLink();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tabControl.SuspendLayout();
            this.metroTabPage1.SuspendLayout();
            this.metroTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvPlugins)).BeginInit();
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
            this.notifyIconMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIconMain_MouseDoubleClick);
            // 
            // contextMenuStripMain
            // 
            this.contextMenuStripMain.Name = "contextMenuStripMain";
            this.contextMenuStripMain.Size = new System.Drawing.Size(61, 4);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.metroTabPage1);
            this.tabControl.Controls.Add(this.metroTabPage3);
            this.tabControl.CustomBackground = false;
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.FontSize = MetroFramework.MetroTabControlSize.Medium;
            this.tabControl.FontWeight = MetroFramework.MetroTabControlWeight.Bold;
            this.tabControl.HotTrack = true;
            this.tabControl.ItemSize = new System.Drawing.Size(148, 31);
            this.tabControl.Location = new System.Drawing.Point(20, 30);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(429, 199);
            this.tabControl.Style = MetroFramework.MetroColorStyle.Blue;
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
            this.metroTabPage1.StyleManager = null;
            this.metroTabPage1.TabIndex = 0;
            this.metroTabPage1.Text = "                      Home                     ";
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
            this.linkEditWowAccounts.Location = new System.Drawing.Point(151, 69);
            this.linkEditWowAccounts.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkEditWowAccounts.Name = "linkEditWowAccounts";
            this.linkEditWowAccounts.Size = new System.Drawing.Size(116, 23);
            this.linkEditWowAccounts.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkEditWowAccounts.StyleManager = null;
            this.linkEditWowAccounts.TabIndex = 59;
            this.linkEditWowAccounts.Text = "Edit WoW accounts";
            this.linkEditWowAccounts.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkEditWowAccounts.UseStyleColors = true;
            this.linkEditWowAccounts.Click += new System.EventHandler(this.LinkEditWowAccounts_Click);
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
            this.linkClickerSettings.Click += new System.EventHandler(this.LinkClickerSettings_Click);
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
            this.linkBackup.StyleManager = null;
            this.linkBackup.TabIndex = 49;
            this.linkBackup.Text = "Backups";
            this.linkBackup.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkBackup.UseStyleColors = true;
            this.linkBackup.Click += new System.EventHandler(this.LinkBackupAddons_Click);
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
            this.cmbboxAccSelect.StyleManager = null;
            this.cmbboxAccSelect.TabIndex = 5;
            this.cmbboxAccSelect.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cmbboxAccSelect.SelectedIndexChanged += new System.EventHandler(this.CmbboxAccSelectSelectedIndexChangedAsync);
            // 
            // metroTabPage3
            // 
            this.metroTabPage3.Controls.Add(this.linkUpdatePlugins);
            this.metroTabPage3.Controls.Add(this.labelTotalPluginsEnabled);
            this.metroTabPage3.Controls.Add(this.linkDownloadPlugins);
            this.metroTabPage3.Controls.Add(this.olvPlugins);
            this.metroTabPage3.Controls.Add(this.buttonStartStopPlugin);
            this.metroTabPage3.CustomBackground = false;
            this.metroTabPage3.HorizontalScrollbar = false;
            this.metroTabPage3.HorizontalScrollbarBarColor = true;
            this.metroTabPage3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage3.HorizontalScrollbarSize = 10;
            this.metroTabPage3.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage3.Name = "metroTabPage3";
            this.metroTabPage3.Size = new System.Drawing.Size(421, 160);
            this.metroTabPage3.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPage3.StyleManager = null;
            this.metroTabPage3.TabIndex = 2;
            this.metroTabPage3.Text = "                  Plug-ins                ";
            this.metroTabPage3.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTabPage3.VerticalScrollbar = false;
            this.metroTabPage3.VerticalScrollbarBarColor = true;
            this.metroTabPage3.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage3.VerticalScrollbarSize = 10;
            // 
            // linkUpdatePlugins
            // 
            this.linkUpdatePlugins.CustomBackground = false;
            this.linkUpdatePlugins.CustomForeColor = false;
            this.linkUpdatePlugins.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkUpdatePlugins.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.linkUpdatePlugins.Location = new System.Drawing.Point(303, 111);
            this.linkUpdatePlugins.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.linkUpdatePlugins.Name = "linkUpdatePlugins";
            this.linkUpdatePlugins.Size = new System.Drawing.Size(115, 23);
            this.linkUpdatePlugins.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkUpdatePlugins.StyleManager = null;
            this.linkUpdatePlugins.TabIndex = 84;
            this.linkUpdatePlugins.Text = "Update plugins";
            this.linkUpdatePlugins.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkUpdatePlugins.UseStyleColors = true;
            this.linkUpdatePlugins.Click += new System.EventHandler(this.LinkUpdatePlugins_Click);
            // 
            // labelTotalPluginsEnabled
            // 
            this.labelTotalPluginsEnabled.CustomBackground = false;
            this.labelTotalPluginsEnabled.CustomForeColor = false;
            this.labelTotalPluginsEnabled.FontSize = MetroFramework.MetroLabelSize.Small;
            this.labelTotalPluginsEnabled.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.labelTotalPluginsEnabled.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.labelTotalPluginsEnabled.Location = new System.Drawing.Point(302, 47);
            this.labelTotalPluginsEnabled.Margin = new System.Windows.Forms.Padding(0);
            this.labelTotalPluginsEnabled.Name = "labelTotalPluginsEnabled";
            this.labelTotalPluginsEnabled.Size = new System.Drawing.Size(118, 20);
            this.labelTotalPluginsEnabled.Style = MetroFramework.MetroColorStyle.Blue;
            this.labelTotalPluginsEnabled.StyleManager = null;
            this.labelTotalPluginsEnabled.TabIndex = 83;
            this.labelTotalPluginsEnabled.Text = "Plug-ins enabled: 99";
            this.labelTotalPluginsEnabled.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelTotalPluginsEnabled.Theme = MetroFramework.MetroThemeStyle.Light;
            this.labelTotalPluginsEnabled.UseStyleColors = true;
            // 
            // linkDownloadPlugins
            // 
            this.linkDownloadPlugins.CustomBackground = false;
            this.linkDownloadPlugins.CustomForeColor = false;
            this.linkDownloadPlugins.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkDownloadPlugins.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.linkDownloadPlugins.Location = new System.Drawing.Point(303, 134);
            this.linkDownloadPlugins.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.linkDownloadPlugins.Name = "linkDownloadPlugins";
            this.linkDownloadPlugins.Size = new System.Drawing.Size(115, 23);
            this.linkDownloadPlugins.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkDownloadPlugins.StyleManager = null;
            this.linkDownloadPlugins.TabIndex = 82;
            this.linkDownloadPlugins.Text = "Download plug-ins";
            this.linkDownloadPlugins.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkDownloadPlugins.UseStyleColors = true;
            this.linkDownloadPlugins.Click += new System.EventHandler(this.LinkDownloadPlugins_Click);
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
            this.olvColumn2.Text = "Settings2";
            this.olvColumn2.Width = 55;
            // 
            // buttonStartStopPlugin
            // 
            this.buttonStartStopPlugin.Highlight = true;
            this.buttonStartStopPlugin.Location = new System.Drawing.Point(303, 15);
            this.buttonStartStopPlugin.Name = "buttonStartStopPlugin";
            this.buttonStartStopPlugin.Size = new System.Drawing.Size(115, 29);
            this.buttonStartStopPlugin.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonStartStopPlugin.StyleManager = null;
            this.buttonStartStopPlugin.TabIndex = 74;
            this.buttonStartStopPlugin.Text = "Hotkeys";
            this.buttonStartStopPlugin.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.buttonStartStopPlugin, "Start/stop plug-in");
            this.buttonStartStopPlugin.Click += new System.EventHandler(this.ButtonStartStopPlugin_Click);
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
            this.linkPing.Location = new System.Drawing.Point(233, 5);
            this.linkPing.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.linkPing.Name = "linkPing";
            this.linkPing.Size = new System.Drawing.Size(120, 20);
            this.linkPing.Style = MetroFramework.MetroColorStyle.Blue;
            this.linkPing.TabIndex = 68;
            this.linkPing.Text = "[999ms]::[very bad]  |";
            this.linkPing.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkPing.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.linkPing, "This is in-game connection info. It\'s formatted as\r\n  [ping : connection quality]" +
        "  \r\nRight-click to open pinger settings");
            this.linkPing.UseStyleColors = true;
            this.linkPing.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LinkPing_MouseDown);
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
            this.linkSettings.TabIndex = 65;
            this.linkSettings.Text = "settings";
            this.linkSettings.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.linkSettings, "Click to open settings dialog");
            this.linkSettings.UseStyleColors = true;
            this.linkSettings.Click += new System.EventHandler(this.LinkSettings_Click);
            // 
            // contextMenuStripBackupAndClean
            // 
            this.contextMenuStripBackupAndClean.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemNextBackupTime,
            this.toolStripMenuItemBackupWoWAddOns,
            this.toolStripMenuItemDeployArchive,
            this.toolStripSeparator3,
            this.toolStripMenuItem1,
            this.toolStripMenuItemOpenWoWLogsFolder});
            this.contextMenuStripBackupAndClean.Name = "contextMenuStripMain";
            this.contextMenuStripBackupAndClean.Size = new System.Drawing.Size(195, 120);
            // 
            // menuItemNextBackupTime
            // 
            this.menuItemNextBackupTime.Enabled = false;
            this.menuItemNextBackupTime.Name = "menuItemNextBackupTime";
            this.menuItemNextBackupTime.Size = new System.Drawing.Size(194, 22);
            this.menuItemNextBackupTime.Text = "Next backup:";
            // 
            // toolStripMenuItemBackupWoWAddOns
            // 
            this.toolStripMenuItemBackupWoWAddOns.Name = "toolStripMenuItemBackupWoWAddOns";
            this.toolStripMenuItemBackupWoWAddOns.Size = new System.Drawing.Size(194, 22);
            this.toolStripMenuItemBackupWoWAddOns.Text = "Backup WoW AddOns";
            this.toolStripMenuItemBackupWoWAddOns.Click += new System.EventHandler(this.ToolStripMenuItemBackupWoWAddOns_Click);
            // 
            // toolStripMenuItemDeployArchive
            // 
            this.toolStripMenuItemDeployArchive.Name = "toolStripMenuItemDeployArchive";
            this.toolStripMenuItemDeployArchive.Size = new System.Drawing.Size(194, 22);
            this.toolStripMenuItemDeployArchive.Text = "Deploy archive...";
            this.toolStripMenuItemDeployArchive.Click += new System.EventHandler(this.ToolStripMenuItemDeployArchive_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(191, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(194, 22);
            this.toolStripMenuItem1.Text = "Open backup folder";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // toolStripMenuItemOpenWoWLogsFolder
            // 
            this.toolStripMenuItemOpenWoWLogsFolder.Name = "toolStripMenuItemOpenWoWLogsFolder";
            this.toolStripMenuItemOpenWoWLogsFolder.Size = new System.Drawing.Size(194, 22);
            this.toolStripMenuItemOpenWoWLogsFolder.Text = "Open WoW logs folder";
            this.toolStripMenuItemOpenWoWLogsFolder.Click += new System.EventHandler(this.ToolStripMenuItemOpenWoWLogsFolder_Click);
            // 
            // pictureBox1
            // 
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
            this.linkTitle.TabIndex = 69;
            this.linkTitle.Text = "AxTools";
            this.linkTitle.Theme = MetroFramework.MetroThemeStyle.Light;
            this.linkTitle.UseStyleColors = true;
            this.linkTitle.Click += new System.EventHandler(this.LinkTitle_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
            // 
            // MainWindow
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
            this.Name = "MainWindow";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 5);
            this.Resizable = false;
            this.Text = "AxTools";
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tabControl.ResumeLayout(false);
            this.metroTabPage1.ResumeLayout(false);
            this.metroTabPage1.PerformLayout();
            this.metroTabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvPlugins)).EndInit();
            this.contextMenuStripBackupAndClean.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        internal NotifyIcon notifyIconMain;
        private MetroTabControl tabControl;
        private MetroTabPage metroTabPage1;
        private MetroTabPage metroTabPage3;
        private MetroComboboxExt cmbboxAccSelect;
        private MetroToolTip metroToolTip1;
        private ContextMenuStrip contextMenuStripMain;
        private MetroLink linkBackup;
        private MetroLink linkClickerSettings;
        private MetroButton buttonStartStopPlugin;
        private MetroLink linkEditWowAccounts;
        private MetroProgressBar progressBarAddonsBackup;
        private MetroLink linkSettings;
        private ContextMenuStrip contextMenuStripBackupAndClean;
        private ToolStripMenuItem toolStripMenuItemBackupWoWAddOns;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItemOpenWoWLogsFolder;
        private PictureBox pictureBox1;
        private MetroLink linkPing;
        private MetroLink linkTitle;
        private BrightIdeasSoftware.ObjectListView olvPlugins;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private ToolStripMenuItem toolStripMenuItemDeployArchive;
        private MetroLink linkDownloadPlugins;
        private MetroLabel labelTotalPluginsEnabled;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem menuItemNextBackupTime;
        private MetroLink linkUpdatePlugins;
    }
}

