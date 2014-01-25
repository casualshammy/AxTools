﻿using System;
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
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.luaConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blackMarketTrackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fishingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.captureFlagsorbsOnTheBattlefieldsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.millingdisenchantingprospectingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopActivePluginorPresshotkeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.customizeWoTWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchWoWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.woWPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerClicker = new System.Windows.Forms.Timer(this.components);
            this.timerAntiAfk = new System.Windows.Forms.Timer(this.components);
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.linkOpenBackupFolder = new MetroFramework.Controls.MetroLink();
            this.buttonBackupAddons = new MetroFramework.Controls.MetroButton();
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.labelAccSelect = new MetroFramework.Controls.MetroLabel();
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager();
            this.cmbboxAccSelect = new MetroFramework.Controls.MetroComboBox();
            this.tileWowUpdater = new MetroFramework.Controls.MetroTile();
            this.tileWow = new MetroFramework.Controls.MetroTile();
            this.tileWowAutopass = new MetroFramework.Controls.MetroTile();
            this.metroTabPage3 = new MetroFramework.Controls.MetroTabPage();
            this.metroCheckBoxPluginShowIngameNotification = new MetroFramework.Controls.MetroCheckBox();
            this.metroButtonLuaConsole = new MetroFramework.Controls.MetroButton();
            this.metroButtonRadar = new MetroFramework.Controls.MetroButton();
            this.metroLabelSelectPlugin = new MetroFramework.Controls.MetroLabel();
            this.metroButtonBlackMarketTracker = new MetroFramework.Controls.MetroButton();
            this.buttonUnloadInjector = new MetroFramework.Controls.MetroButton();
            this.comboBoxWowPlugins = new MetroFramework.Controls.MetroComboBox();
            this.toggleWowPlugins = new AxTools.Components.MetroToggleExt(this.components);
            this.metroTabPage2 = new MetroFramework.Controls.MetroTabPage();
            this.checkBoxStartTeamspeak3WithWow = new MetroFramework.Controls.MetroCheckBox();
            this.checkBoxStartMumbleWithWow = new MetroFramework.Controls.MetroCheckBox();
            this.checkBoxStartRaidcallWithWow = new MetroFramework.Controls.MetroCheckBox();
            this.checkBoxStartVenriloWithWow = new MetroFramework.Controls.MetroCheckBox();
            this.tileRaidcall = new MetroFramework.Controls.MetroTile();
            this.tileTeamspeak3 = new MetroFramework.Controls.MetroTile();
            this.tileMumble = new MetroFramework.Controls.MetroTile();
            this.tileVentrilo = new MetroFramework.Controls.MetroTile();
            this.progressBarLoading = new MetroFramework.Controls.MetroProgressBar();
            this.labelLoading = new MetroFramework.Controls.MetroLabel();
            this.labelPingNum = new MetroFramework.Controls.MetroLabel();
            this.metroToolTip1 = new MetroFramework.Components.MetroToolTip();
            this.pictureBoxExtSettings = new AxTools.Components.PictureBoxExt(this.components);
            this.contextMenuStripMain.SuspendLayout();
            this.metroTabControl1.SuspendLayout();
            this.metroTabPage1.SuspendLayout();
            this.metroTabPage3.SuspendLayout();
            this.metroTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxExtSettings)).BeginInit();
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
            this.toolStripSeparator3,
            this.luaConsoleToolStripMenuItem,
            this.blackMarketTrackerToolStripMenuItem,
            this.toolStripSeparator2,
            this.fishingToolStripMenuItem,
            this.captureFlagsorbsOnTheBattlefieldsToolStripMenuItem,
            this.millingdisenchantingprospectingToolStripMenuItem,
            this.stopActivePluginorPresshotkeyToolStripMenuItem,
            this.toolStripSeparator1,
            this.customizeWoTWindowToolStripMenuItem,
            this.launchWoWToolStripMenuItem});
            this.contextMenuStripMain.Name = "contextMenuStripMain";
            this.contextMenuStripMain.Size = new System.Drawing.Size(282, 220);
            // 
            // woWRadarToolStripMenuItem
            // 
            this.woWRadarToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("woWRadarToolStripMenuItem.Image")));
            this.woWRadarToolStripMenuItem.Name = "woWRadarToolStripMenuItem";
            this.woWRadarToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.woWRadarToolStripMenuItem.Text = "WoW Radar";
            this.woWRadarToolStripMenuItem.Click += new System.EventHandler(this.WoWRadarToolStripMenuItemClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(278, 6);
            // 
            // luaConsoleToolStripMenuItem
            // 
            this.luaConsoleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("luaConsoleToolStripMenuItem.Image")));
            this.luaConsoleToolStripMenuItem.Name = "luaConsoleToolStripMenuItem";
            this.luaConsoleToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.luaConsoleToolStripMenuItem.Text = "Lua console";
            this.luaConsoleToolStripMenuItem.Click += new System.EventHandler(this.LuaConsoleToolStripMenuItemClick);
            // 
            // blackMarketTrackerToolStripMenuItem
            // 
            this.blackMarketTrackerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("blackMarketTrackerToolStripMenuItem.Image")));
            this.blackMarketTrackerToolStripMenuItem.Name = "blackMarketTrackerToolStripMenuItem";
            this.blackMarketTrackerToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.blackMarketTrackerToolStripMenuItem.Text = "Black Market tracker";
            this.blackMarketTrackerToolStripMenuItem.Click += new System.EventHandler(this.blackMarketTrackerToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(278, 6);
            // 
            // fishingToolStripMenuItem
            // 
            this.fishingToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fishingToolStripMenuItem.Image")));
            this.fishingToolStripMenuItem.Name = "fishingToolStripMenuItem";
            this.fishingToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.fishingToolStripMenuItem.Text = "Fishing";
            this.fishingToolStripMenuItem.Click += new System.EventHandler(this.FishingToolStripMenuItemClick);
            // 
            // captureFlagsorbsOnTheBattlefieldsToolStripMenuItem
            // 
            this.captureFlagsorbsOnTheBattlefieldsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("captureFlagsorbsOnTheBattlefieldsToolStripMenuItem.Image")));
            this.captureFlagsorbsOnTheBattlefieldsToolStripMenuItem.Name = "captureFlagsorbsOnTheBattlefieldsToolStripMenuItem";
            this.captureFlagsorbsOnTheBattlefieldsToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.captureFlagsorbsOnTheBattlefieldsToolStripMenuItem.Text = "Capture flags/orbs on the battlefields";
            this.captureFlagsorbsOnTheBattlefieldsToolStripMenuItem.Click += new System.EventHandler(this.CaptureFlagsorbsOnTheBattlefieldsToolStripMenuItemClick);
            // 
            // millingdisenchantingprospectingToolStripMenuItem
            // 
            this.millingdisenchantingprospectingToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("millingdisenchantingprospectingToolStripMenuItem.Image")));
            this.millingdisenchantingprospectingToolStripMenuItem.Name = "millingdisenchantingprospectingToolStripMenuItem";
            this.millingdisenchantingprospectingToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.millingdisenchantingprospectingToolStripMenuItem.Text = "Milling/disenchanting/prospecting";
            this.millingdisenchantingprospectingToolStripMenuItem.Click += new System.EventHandler(this.MillingdisenchantingprospectingToolStripMenuItemClick);
            // 
            // stopActivePluginorPresshotkeyToolStripMenuItem
            // 
            this.stopActivePluginorPresshotkeyToolStripMenuItem.Enabled = false;
            this.stopActivePluginorPresshotkeyToolStripMenuItem.Name = "stopActivePluginorPresshotkeyToolStripMenuItem";
            this.stopActivePluginorPresshotkeyToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.stopActivePluginorPresshotkeyToolStripMenuItem.Text = "Stop active plug-in (or press <hotkey>)";
            this.stopActivePluginorPresshotkeyToolStripMenuItem.Click += new System.EventHandler(this.stopActivePluginorPresshotkeyToolStripMenuItem_Click_1);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(278, 6);
            // 
            // customizeWoTWindowToolStripMenuItem
            // 
            this.customizeWoTWindowToolStripMenuItem.Name = "customizeWoTWindowToolStripMenuItem";
            this.customizeWoTWindowToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.customizeWoTWindowToolStripMenuItem.Text = "Customize WoT window";
            this.customizeWoTWindowToolStripMenuItem.Click += new System.EventHandler(this.customizeWoTWindowToolStripMenuItem_Click);
            // 
            // launchWoWToolStripMenuItem
            // 
            this.launchWoWToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("launchWoWToolStripMenuItem.Image")));
            this.launchWoWToolStripMenuItem.Name = "launchWoWToolStripMenuItem";
            this.launchWoWToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.launchWoWToolStripMenuItem.Text = "Exit";
            this.launchWoWToolStripMenuItem.Click += new System.EventHandler(this.LaunchWoWToolStripMenuItemClick);
            // 
            // woWPluginsToolStripMenuItem
            // 
            this.woWPluginsToolStripMenuItem.Name = "woWPluginsToolStripMenuItem";
            this.woWPluginsToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // timerClicker
            // 
            this.timerClicker.Tick += new System.EventHandler(this.TimerClickerTick);
            // 
            // timerAntiAfk
            // 
            this.timerAntiAfk.Interval = 5000;
            this.timerAntiAfk.Tick += new System.EventHandler(this.Timer2000Tick);
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.metroTabPage1);
            this.metroTabControl1.Controls.Add(this.metroTabPage3);
            this.metroTabControl1.Controls.Add(this.metroTabPage2);
            this.metroTabControl1.CustomBackground = false;
            this.metroTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroTabControl1.FontSize = MetroFramework.MetroTabControlSize.Medium;
            this.metroTabControl1.FontWeight = MetroFramework.MetroTabControlWeight.Regular;
            this.metroTabControl1.HotTrack = true;
            this.metroTabControl1.Location = new System.Drawing.Point(20, 60);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(429, 185);
            this.metroTabControl1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabControl1.StyleManager = this.metroStyleManager1;
            this.metroTabControl1.TabIndex = 59;
            this.metroTabControl1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.metroTabControl1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTabControl1.UseStyleColors = true;
            // 
            // metroTabPage1
            // 
            this.metroTabPage1.Controls.Add(this.linkOpenBackupFolder);
            this.metroTabPage1.Controls.Add(this.buttonBackupAddons);
            this.metroTabPage1.Controls.Add(this.metroButton1);
            this.metroTabPage1.Controls.Add(this.labelAccSelect);
            this.metroTabPage1.Controls.Add(this.cmbboxAccSelect);
            this.metroTabPage1.Controls.Add(this.tileWowUpdater);
            this.metroTabPage1.Controls.Add(this.tileWow);
            this.metroTabPage1.Controls.Add(this.tileWowAutopass);
            this.metroTabPage1.CustomBackground = false;
            this.metroTabPage1.HorizontalScrollbar = false;
            this.metroTabPage1.HorizontalScrollbarBarColor = true;
            this.metroTabPage1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.HorizontalScrollbarSize = 10;
            this.metroTabPage1.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(421, 146);
            this.metroTabPage1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPage1.StyleManager = this.metroStyleManager1;
            this.metroTabPage1.TabIndex = 0;
            this.metroTabPage1.Text = "       Launch WoW     ";
            this.metroTabPage1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTabPage1.VerticalScrollbar = false;
            this.metroTabPage1.VerticalScrollbarBarColor = true;
            this.metroTabPage1.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.VerticalScrollbarSize = 10;
            // 
            // linkOpenBackupFolder
            // 
            this.linkOpenBackupFolder.CustomBackground = false;
            this.linkOpenBackupFolder.CustomForeColor = false;
            this.linkOpenBackupFolder.FontSize = MetroFramework.MetroLinkSize.Small;
            this.linkOpenBackupFolder.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.linkOpenBackupFolder.Location = new System.Drawing.Point(300, 117);
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
            // buttonBackupAddons
            // 
            this.buttonBackupAddons.Highlight = true;
            this.buttonBackupAddons.Location = new System.Drawing.Point(300, 86);
            this.buttonBackupAddons.Name = "buttonBackupAddons";
            this.buttonBackupAddons.Size = new System.Drawing.Size(118, 25);
            this.buttonBackupAddons.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonBackupAddons.StyleManager = null;
            this.buttonBackupAddons.TabIndex = 8;
            this.buttonBackupAddons.Text = "Backup addons";
            this.buttonBackupAddons.Theme = MetroFramework.MetroThemeStyle.Light;
            this.buttonBackupAddons.Click += new System.EventHandler(this.buttonBackupAddons_Click);
            // 
            // metroButton1
            // 
            this.metroButton1.Highlight = true;
            this.metroButton1.Location = new System.Drawing.Point(300, 20);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(118, 25);
            this.metroButton1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroButton1.StyleManager = null;
            this.metroButton1.TabIndex = 7;
            this.metroButton1.Text = "Edit accounts";
            this.metroButton1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // labelAccSelect
            // 
            this.labelAccSelect.AutoSize = true;
            this.labelAccSelect.CustomBackground = false;
            this.labelAccSelect.CustomForeColor = false;
            this.labelAccSelect.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.labelAccSelect.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.labelAccSelect.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.labelAccSelect.Location = new System.Drawing.Point(109, -6);
            this.labelAccSelect.Name = "labelAccSelect";
            this.labelAccSelect.Size = new System.Drawing.Size(223, 19);
            this.labelAccSelect.Style = MetroFramework.MetroColorStyle.Blue;
            this.labelAccSelect.StyleManager = this.metroStyleManager1;
            this.labelAccSelect.TabIndex = 6;
            this.labelAccSelect.Text = "Click to select account (Esc to cancel)";
            this.labelAccSelect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelAccSelect.Theme = MetroFramework.MetroThemeStyle.Light;
            this.labelAccSelect.UseStyleColors = true;
            this.labelAccSelect.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelAccSelect_MouseClick);
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.OwnerForm = this;
            this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // cmbboxAccSelect
            // 
            this.cmbboxAccSelect.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbboxAccSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbboxAccSelect.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.cmbboxAccSelect.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.cmbboxAccSelect.FormattingEnabled = true;
            this.cmbboxAccSelect.ItemHeight = 23;
            this.cmbboxAccSelect.Location = new System.Drawing.Point(201, -6);
            this.cmbboxAccSelect.Name = "cmbboxAccSelect";
            this.cmbboxAccSelect.Size = new System.Drawing.Size(283, 29);
            this.cmbboxAccSelect.Style = MetroFramework.MetroColorStyle.Blue;
            this.cmbboxAccSelect.StyleManager = this.metroStyleManager1;
            this.cmbboxAccSelect.TabIndex = 5;
            this.cmbboxAccSelect.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cmbboxAccSelect.SelectedIndexChanged += new System.EventHandler(this.CmbboxAccSelectSelectedIndexChanged);
            this.cmbboxAccSelect.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbboxAccSelect_MouseClick);
            // 
            // tileWowUpdater
            // 
            this.tileWowUpdater.ActiveControl = null;
            this.tileWowUpdater.CustomBackground = false;
            this.tileWowUpdater.CustomForeColor = false;
            this.tileWowUpdater.Location = new System.Drawing.Point(184, 86);
            this.tileWowUpdater.Name = "tileWowUpdater";
            this.tileWowUpdater.PaintTileCount = true;
            this.tileWowUpdater.Size = new System.Drawing.Size(102, 60);
            this.tileWowUpdater.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileWowUpdater.StyleManager = this.metroStyleManager1;
            this.tileWowUpdater.TabIndex = 4;
            this.tileWowUpdater.Text = "Updater";
            this.tileWowUpdater.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileWowUpdater.TileCount = 0;
            this.tileWowUpdater.Click += new System.EventHandler(this.TileWowUpdaterClick);
            // 
            // tileWow
            // 
            this.tileWow.ActiveControl = null;
            this.tileWow.CustomBackground = false;
            this.tileWow.CustomForeColor = false;
            this.tileWow.Location = new System.Drawing.Point(3, 86);
            this.tileWow.Name = "tileWow";
            this.tileWow.PaintTileCount = true;
            this.tileWow.Size = new System.Drawing.Size(175, 60);
            this.tileWow.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileWow.StyleManager = this.metroStyleManager1;
            this.tileWow.TabIndex = 3;
            this.tileWow.Text = "Launch WoW";
            this.tileWow.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileWow.TileCount = 0;
            this.tileWow.Click += new System.EventHandler(this.TileWowClick);
            // 
            // tileWowAutopass
            // 
            this.tileWowAutopass.ActiveControl = null;
            this.tileWowAutopass.CustomBackground = false;
            this.tileWowAutopass.CustomForeColor = false;
            this.tileWowAutopass.Location = new System.Drawing.Point(3, 20);
            this.tileWowAutopass.Name = "tileWowAutopass";
            this.tileWowAutopass.PaintTileCount = true;
            this.tileWowAutopass.Size = new System.Drawing.Size(283, 60);
            this.tileWowAutopass.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileWowAutopass.StyleManager = this.metroStyleManager1;
            this.tileWowAutopass.TabIndex = 2;
            this.tileWowAutopass.Text = "Launch WoW with autopass";
            this.tileWowAutopass.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileWowAutopass.TileCount = 0;
            this.tileWowAutopass.Click += new System.EventHandler(this.TileWowAutopassClick);
            // 
            // metroTabPage3
            // 
            this.metroTabPage3.Controls.Add(this.metroCheckBoxPluginShowIngameNotification);
            this.metroTabPage3.Controls.Add(this.metroButtonLuaConsole);
            this.metroTabPage3.Controls.Add(this.metroButtonRadar);
            this.metroTabPage3.Controls.Add(this.metroLabelSelectPlugin);
            this.metroTabPage3.Controls.Add(this.metroButtonBlackMarketTracker);
            this.metroTabPage3.Controls.Add(this.buttonUnloadInjector);
            this.metroTabPage3.Controls.Add(this.comboBoxWowPlugins);
            this.metroTabPage3.Controls.Add(this.toggleWowPlugins);
            this.metroTabPage3.CustomBackground = false;
            this.metroTabPage3.HorizontalScrollbar = false;
            this.metroTabPage3.HorizontalScrollbarBarColor = true;
            this.metroTabPage3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage3.HorizontalScrollbarSize = 10;
            this.metroTabPage3.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage3.Name = "metroTabPage3";
            this.metroTabPage3.Size = new System.Drawing.Size(421, 146);
            this.metroTabPage3.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPage3.StyleManager = this.metroStyleManager1;
            this.metroTabPage3.TabIndex = 2;
            this.metroTabPage3.Text = "        WoW plug-ins      ";
            this.metroTabPage3.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTabPage3.VerticalScrollbar = false;
            this.metroTabPage3.VerticalScrollbarBarColor = true;
            this.metroTabPage3.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage3.VerticalScrollbarSize = 10;
            // 
            // metroCheckBoxPluginShowIngameNotification
            // 
            this.metroCheckBoxPluginShowIngameNotification.AutoSize = true;
            this.metroCheckBoxPluginShowIngameNotification.CustomBackground = false;
            this.metroCheckBoxPluginShowIngameNotification.CustomForeColor = false;
            this.metroCheckBoxPluginShowIngameNotification.FontSize = MetroFramework.MetroLinkSize.Small;
            this.metroCheckBoxPluginShowIngameNotification.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.metroCheckBoxPluginShowIngameNotification.Location = new System.Drawing.Point(3, 77);
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
            this.metroButtonLuaConsole.Location = new System.Drawing.Point(199, 121);
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
            this.metroButtonRadar.Location = new System.Drawing.Point(132, 121);
            this.metroButtonRadar.Name = "metroButtonRadar";
            this.metroButtonRadar.Size = new System.Drawing.Size(61, 23);
            this.metroButtonRadar.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroButtonRadar.StyleManager = this.metroStyleManager1;
            this.metroButtonRadar.TabIndex = 70;
            this.metroButtonRadar.Text = "Radar";
            this.metroButtonRadar.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroButtonRadar.Click += new System.EventHandler(this.MetroButtonRadarClick);
            // 
            // metroLabelSelectPlugin
            // 
            this.metroLabelSelectPlugin.AutoSize = true;
            this.metroLabelSelectPlugin.CustomBackground = false;
            this.metroLabelSelectPlugin.CustomForeColor = false;
            this.metroLabelSelectPlugin.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabelSelectPlugin.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabelSelectPlugin.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabelSelectPlugin.Location = new System.Drawing.Point(3, 20);
            this.metroLabelSelectPlugin.Name = "metroLabelSelectPlugin";
            this.metroLabelSelectPlugin.Size = new System.Drawing.Size(372, 19);
            this.metroLabelSelectPlugin.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabelSelectPlugin.StyleManager = this.metroStyleManager1;
            this.metroLabelSelectPlugin.TabIndex = 69;
            this.metroLabelSelectPlugin.Text = "Select a plugin...and hover o\'er me to see plugin description";
            this.metroLabelSelectPlugin.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabelSelectPlugin.UseStyleColors = true;
            // 
            // metroButtonBlackMarketTracker
            // 
            this.metroButtonBlackMarketTracker.Highlight = true;
            this.metroButtonBlackMarketTracker.Location = new System.Drawing.Point(3, 121);
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
            this.buttonUnloadInjector.Location = new System.Drawing.Point(293, 121);
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
            this.comboBoxWowPlugins.Location = new System.Drawing.Point(3, 42);
            this.comboBoxWowPlugins.Name = "comboBoxWowPlugins";
            this.comboBoxWowPlugins.Size = new System.Drawing.Size(329, 29);
            this.comboBoxWowPlugins.Style = MetroFramework.MetroColorStyle.Blue;
            this.comboBoxWowPlugins.StyleManager = this.metroStyleManager1;
            this.comboBoxWowPlugins.TabIndex = 6;
            this.comboBoxWowPlugins.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.comboBoxWowPlugins, "Click to select plugin");
            this.comboBoxWowPlugins.SelectedIndexChanged += new System.EventHandler(this.ComboBoxWowPluginsSelectedIndexChanged);
            // 
            // toggleWowPlugins
            // 
            this.toggleWowPlugins.AutoSize = true;
            this.toggleWowPlugins.CustomBackground = false;
            this.toggleWowPlugins.DisplayStatus = false;
            this.toggleWowPlugins.ExtraText = null;
            this.toggleWowPlugins.FontSize = MetroFramework.MetroLinkSize.Small;
            this.toggleWowPlugins.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.toggleWowPlugins.Location = new System.Drawing.Point(338, 48);
            this.toggleWowPlugins.Name = "toggleWowPlugins";
            this.toggleWowPlugins.Size = new System.Drawing.Size(80, 17);
            this.toggleWowPlugins.SizeExt = 80;
            this.toggleWowPlugins.Style = MetroFramework.MetroColorStyle.Blue;
            this.toggleWowPlugins.StyleManager = this.metroStyleManager1;
            this.toggleWowPlugins.TabIndex = 7;
            this.toggleWowPlugins.Text = "Off";
            this.toggleWowPlugins.Theme = MetroFramework.MetroThemeStyle.Light;
            this.toggleWowPlugins.UseStyleColors = true;
            this.toggleWowPlugins.UseVisualStyleBackColor = true;
            this.toggleWowPlugins.CheckedChanged += new System.EventHandler(this.ToggleWowPluginsCheckedChanged);
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
            this.metroTabPage2.HorizontalScrollbar = false;
            this.metroTabPage2.HorizontalScrollbarBarColor = true;
            this.metroTabPage2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.HorizontalScrollbarSize = 10;
            this.metroTabPage2.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage2.Name = "metroTabPage2";
            this.metroTabPage2.Size = new System.Drawing.Size(421, 146);
            this.metroTabPage2.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPage2.StyleManager = this.metroStyleManager1;
            this.metroTabPage2.TabIndex = 1;
            this.metroTabPage2.Text = "          VoIP        ";
            this.metroTabPage2.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTabPage2.VerticalScrollbar = false;
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
            this.checkBoxStartTeamspeak3WithWow.Location = new System.Drawing.Point(241, 111);
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
            this.checkBoxStartMumbleWithWow.Location = new System.Drawing.Point(241, 86);
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
            this.checkBoxStartRaidcallWithWow.Location = new System.Drawing.Point(241, 61);
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
            this.checkBoxStartVenriloWithWow.Location = new System.Drawing.Point(241, 36);
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
            this.tileRaidcall.CustomBackground = false;
            this.tileRaidcall.CustomForeColor = false;
            this.tileRaidcall.Location = new System.Drawing.Point(125, 20);
            this.tileRaidcall.Name = "tileRaidcall";
            this.tileRaidcall.PaintTileCount = true;
            this.tileRaidcall.Size = new System.Drawing.Size(110, 60);
            this.tileRaidcall.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileRaidcall.StyleManager = this.metroStyleManager1;
            this.tileRaidcall.TabIndex = 8;
            this.tileRaidcall.Text = "Raidcall";
            this.tileRaidcall.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileRaidcall.TileCount = 0;
            this.tileRaidcall.Click += new System.EventHandler(this.TileRaidcallClick);
            // 
            // tileTeamspeak3
            // 
            this.tileTeamspeak3.ActiveControl = null;
            this.tileTeamspeak3.CustomBackground = false;
            this.tileTeamspeak3.CustomForeColor = false;
            this.tileTeamspeak3.Location = new System.Drawing.Point(3, 86);
            this.tileTeamspeak3.Name = "tileTeamspeak3";
            this.tileTeamspeak3.PaintTileCount = true;
            this.tileTeamspeak3.Size = new System.Drawing.Size(116, 60);
            this.tileTeamspeak3.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileTeamspeak3.StyleManager = this.metroStyleManager1;
            this.tileTeamspeak3.TabIndex = 7;
            this.tileTeamspeak3.Text = "Teamspeak 3";
            this.tileTeamspeak3.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileTeamspeak3.TileCount = 0;
            this.tileTeamspeak3.Click += new System.EventHandler(this.TileTeamspeak3Click);
            // 
            // tileMumble
            // 
            this.tileMumble.ActiveControl = null;
            this.tileMumble.CustomBackground = false;
            this.tileMumble.CustomForeColor = false;
            this.tileMumble.Location = new System.Drawing.Point(125, 86);
            this.tileMumble.Name = "tileMumble";
            this.tileMumble.PaintTileCount = true;
            this.tileMumble.Size = new System.Drawing.Size(110, 60);
            this.tileMumble.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileMumble.StyleManager = this.metroStyleManager1;
            this.tileMumble.TabIndex = 6;
            this.tileMumble.Text = "Mumble";
            this.tileMumble.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileMumble.TileCount = 0;
            this.tileMumble.Click += new System.EventHandler(this.TileMumbleClick);
            // 
            // tileVentrilo
            // 
            this.tileVentrilo.ActiveControl = null;
            this.tileVentrilo.CustomBackground = false;
            this.tileVentrilo.CustomForeColor = false;
            this.tileVentrilo.Location = new System.Drawing.Point(3, 20);
            this.tileVentrilo.Name = "tileVentrilo";
            this.tileVentrilo.PaintTileCount = true;
            this.tileVentrilo.Size = new System.Drawing.Size(116, 60);
            this.tileVentrilo.Style = MetroFramework.MetroColorStyle.Blue;
            this.tileVentrilo.StyleManager = this.metroStyleManager1;
            this.tileVentrilo.TabIndex = 5;
            this.tileVentrilo.Text = "Ventrilo";
            this.tileVentrilo.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tileVentrilo.TileCount = 0;
            this.tileVentrilo.Click += new System.EventHandler(this.TileVentriloClick);
            // 
            // progressBarLoading
            // 
            this.progressBarLoading.FontSize = MetroFramework.MetroProgressBarSize.Medium;
            this.progressBarLoading.FontWeight = MetroFramework.MetroProgressBarWeight.Light;
            this.progressBarLoading.HideProgressText = true;
            this.progressBarLoading.Location = new System.Drawing.Point(337, 14);
            this.progressBarLoading.Name = "progressBarLoading";
            this.progressBarLoading.ProgressBarStyle = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarLoading.Size = new System.Drawing.Size(363, 23);
            this.progressBarLoading.Style = MetroFramework.MetroColorStyle.Blue;
            this.progressBarLoading.StyleManager = this.metroStyleManager1;
            this.progressBarLoading.TabIndex = 60;
            this.progressBarLoading.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.progressBarLoading.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // labelLoading
            // 
            this.labelLoading.AutoSize = true;
            this.labelLoading.CustomBackground = false;
            this.labelLoading.CustomForeColor = false;
            this.labelLoading.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.labelLoading.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.labelLoading.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.labelLoading.Location = new System.Drawing.Point(328, 45);
            this.labelLoading.Name = "labelLoading";
            this.labelLoading.Size = new System.Drawing.Size(136, 19);
            this.labelLoading.Style = MetroFramework.MetroColorStyle.Blue;
            this.labelLoading.StyleManager = this.metroStyleManager1;
            this.labelLoading.TabIndex = 61;
            this.labelLoading.Text = "Loading, please wait...";
            this.labelLoading.Theme = MetroFramework.MetroThemeStyle.Light;
            this.labelLoading.UseStyleColors = true;
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
            this.labelPingNum.Location = new System.Drawing.Point(283, 6);
            this.labelPingNum.Name = "labelPingNum";
            this.labelPingNum.Size = new System.Drawing.Size(90, 19);
            this.labelPingNum.Style = MetroFramework.MetroColorStyle.Blue;
            this.labelPingNum.StyleManager = this.metroStyleManager1;
            this.labelPingNum.TabIndex = 63;
            this.labelPingNum.Text = "[999]::[100%]";
            this.labelPingNum.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.labelPingNum, "Click to show connectivity info widget");
            this.labelPingNum.UseStyleColors = true;
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
            // pictureBoxExtSettings
            // 
            this.pictureBoxExtSettings.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxExtSettings.Image")));
            this.pictureBoxExtSettings.ImageOnHover = ((System.Drawing.Image)(resources.GetObject("pictureBoxExtSettings.ImageOnHover")));
            this.pictureBoxExtSettings.Location = new System.Drawing.Point(379, 5);
            this.pictureBoxExtSettings.Name = "pictureBoxExtSettings";
            this.pictureBoxExtSettings.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxExtSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxExtSettings.TabIndex = 68;
            this.pictureBoxExtSettings.TabStop = false;
            this.metroToolTip1.SetToolTip(this.pictureBoxExtSettings, "Application settings");
            this.pictureBoxExtSettings.Click += new System.EventHandler(this.PictureBoxExtSettingsClick);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(469, 265);
            this.Controls.Add(this.pictureBoxExtSettings);
            this.Controls.Add(this.labelLoading);
            this.Controls.Add(this.progressBarLoading);
            this.Controls.Add(this.metroTabControl1);
            this.Controls.Add(this.labelPingNum);
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Resizable = false;
            this.StyleManager = this.metroStyleManager1;
            this.Text = "AxTools 99";
            this.contextMenuStripMain.ResumeLayout(false);
            this.metroTabControl1.ResumeLayout(false);
            this.metroTabPage1.ResumeLayout(false);
            this.metroTabPage1.PerformLayout();
            this.metroTabPage3.ResumeLayout(false);
            this.metroTabPage3.PerformLayout();
            this.metroTabPage2.ResumeLayout(false);
            this.metroTabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxExtSettings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private Timer timerClicker;
        private Timer timerAntiAfk;
        private NotifyIcon notifyIconMain;
        private MetroTabControl metroTabControl1;
        private MetroTabPage metroTabPage1;
        private MetroTile tileWowUpdater;
        private MetroTile tileWow;
        private MetroTile tileWowAutopass;
        private MetroTabPage metroTabPage2;
        private MetroTile tileRaidcall;
        private MetroTile tileTeamspeak3;
        private MetroTile tileMumble;
        private MetroTile tileVentrilo;
        private MetroTabPage metroTabPage3;
        private MetroComboBox comboBoxWowPlugins;
        private MetroComboBox cmbboxAccSelect;
        private MetroLabel labelAccSelect;
        private MetroButton buttonUnloadInjector;
        private MetroProgressBar progressBarLoading;
        private MetroLabel labelLoading;
        private MetroLabel labelPingNum;
        private MetroToolTip metroToolTip1;
        private MetroStyleManager metroStyleManager1;
        private ContextMenuStrip contextMenuStripMain;
        private ToolStripMenuItem woWRadarToolStripMenuItem;
        private ToolStripMenuItem woWPluginsToolStripMenuItem;
        private ToolStripMenuItem fishingToolStripMenuItem;
        private ToolStripMenuItem captureFlagsorbsOnTheBattlefieldsToolStripMenuItem;
        private ToolStripMenuItem millingdisenchantingprospectingToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem launchWoWToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem luaConsoleToolStripMenuItem;
        private MetroButton metroButtonBlackMarketTracker;
        private MetroLabel metroLabelSelectPlugin;
        private MetroButton metroButtonLuaConsole;
        private MetroButton metroButtonRadar;
        private PictureBoxExt pictureBoxExtSettings;
        private MetroToggleExt toggleWowPlugins;
        private MetroCheckBox checkBoxStartVenriloWithWow;
        private MetroCheckBox checkBoxStartRaidcallWithWow;
        private MetroCheckBox checkBoxStartTeamspeak3WithWow;
        private MetroCheckBox checkBoxStartMumbleWithWow;
        private ToolStripMenuItem stopActivePluginorPresshotkeyToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem blackMarketTrackerToolStripMenuItem;
        private MetroCheckBox metroCheckBoxPluginShowIngameNotification;
        private ToolStripMenuItem customizeWoTWindowToolStripMenuItem;
        private MetroButton buttonBackupAddons;
        private MetroButton metroButton1;
        private MetroLink linkOpenBackupFolder;
    }
}

