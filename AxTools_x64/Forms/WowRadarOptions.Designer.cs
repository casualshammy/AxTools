using System.Windows.Forms;
using Components;
using MetroFramework.Components;
using MetroFramework.Controls;

namespace AxTools.Forms
{
    internal partial class WowRadarOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new MetroFramework.Components.MetroToolTip();
            this.dataGridViewObjects = new Components.DataGridViewExt();
            this.ObjectToFindEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ObjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Interact = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SoundAlarm = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.buttonAddNPC = new MetroFramework.Controls.MetroButton();
            this.metroCheckBoxShowPlayersClasses = new MetroFramework.Controls.MetroCheckBox();
            this.metroCheckBoxShowNpcsNames = new MetroFramework.Controls.MetroCheckBox();
            this.metroCheckBoxShowObjectsNames = new MetroFramework.Controls.MetroCheckBox();
            this.metroTextBoxAddNew = new MetroFramework.Controls.MetroTextBox();
            this.buttonAddUnknown = new MetroFramework.Controls.MetroButton();
            this.comboboxNPCs = new Components.MetroComboboxExt(this.components);
            this.comboboxObjects = new Components.MetroComboboxExt(this.components);
            this.buttonAddObject = new MetroFramework.Controls.MetroButton();
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.tabPageTrackingUnits = new MetroFramework.Controls.MetroTabPage();
            this.oListView = new BrightIdeasSoftware.ObjectListView();
            this.oColumnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.oColumnInteract = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.oColumnSoundAlarm = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.buttonSaveFile = new MetroFramework.Controls.MetroButton();
            this.buttonOpenFile = new MetroFramework.Controls.MetroButton();
            this.tabPageAppearance = new MetroFramework.Controls.MetroTabPage();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewObjects)).BeginInit();
            this.metroTabControl1.SuspendLayout();
            this.tabPageTrackingUnits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.oListView)).BeginInit();
            this.tabPageAppearance.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.Style = MetroFramework.MetroColorStyle.Blue;
            this.toolTip1.StyleManager = null;
            this.toolTip1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // dataGridViewObjects
            // 
            this.dataGridViewObjects.AllowUserToAddRows = false;
            this.dataGridViewObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewObjects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ObjectToFindEnabled,
            this.ObjectName,
            this.Interact,
            this.SoundAlarm});
            this.dataGridViewObjects.Location = new System.Drawing.Point(3, 39);
            this.dataGridViewObjects.Name = "dataGridViewObjects";
            this.dataGridViewObjects.RowHeadersVisible = false;
            this.dataGridViewObjects.Size = new System.Drawing.Size(388, 198);
            this.dataGridViewObjects.TabIndex = 53;
            // 
            // ObjectToFindEnabled
            // 
            this.ObjectToFindEnabled.FillWeight = 20F;
            this.ObjectToFindEnabled.HeaderText = " ";
            this.ObjectToFindEnabled.Name = "ObjectToFindEnabled";
            this.ObjectToFindEnabled.Width = 20;
            // 
            // ObjectName
            // 
            this.ObjectName.FillWeight = 265F;
            this.ObjectName.HeaderText = "Name";
            this.ObjectName.Name = "ObjectName";
            this.ObjectName.Width = 265;
            // 
            // Interact
            // 
            this.Interact.FillWeight = 50F;
            this.Interact.HeaderText = "Interact";
            this.Interact.Name = "Interact";
            this.Interact.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Interact.Width = 50;
            // 
            // SoundAlarm
            // 
            this.SoundAlarm.FillWeight = 50F;
            this.SoundAlarm.HeaderText = "Sound    alarm";
            this.SoundAlarm.Name = "SoundAlarm";
            this.SoundAlarm.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SoundAlarm.Width = 50;
            // 
            // buttonAddNPC
            // 
            this.buttonAddNPC.Highlight = true;
            this.buttonAddNPC.Location = new System.Drawing.Point(320, 272);
            this.buttonAddNPC.Name = "buttonAddNPC";
            this.buttonAddNPC.Size = new System.Drawing.Size(71, 25);
            this.buttonAddNPC.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonAddNPC.StyleManager = null;
            this.buttonAddNPC.TabIndex = 38;
            this.buttonAddNPC.Text = "Add";
            this.buttonAddNPC.Theme = MetroFramework.MetroThemeStyle.Light;
            this.buttonAddNPC.Click += new System.EventHandler(this.buttonAddNPC_Click);
            // 
            // metroCheckBoxShowPlayersClasses
            // 
            this.metroCheckBoxShowPlayersClasses.AutoSize = true;
            this.metroCheckBoxShowPlayersClasses.CustomBackground = false;
            this.metroCheckBoxShowPlayersClasses.CustomForeColor = false;
            this.metroCheckBoxShowPlayersClasses.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.metroCheckBoxShowPlayersClasses.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.metroCheckBoxShowPlayersClasses.Location = new System.Drawing.Point(3, 35);
            this.metroCheckBoxShowPlayersClasses.Name = "metroCheckBoxShowPlayersClasses";
            this.metroCheckBoxShowPlayersClasses.Size = new System.Drawing.Size(131, 19);
            this.metroCheckBoxShowPlayersClasses.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroCheckBoxShowPlayersClasses.StyleManager = null;
            this.metroCheckBoxShowPlayersClasses.TabIndex = 47;
            this.metroCheckBoxShowPlayersClasses.Text = "Show player class";
            this.metroCheckBoxShowPlayersClasses.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroCheckBoxShowPlayersClasses.UseStyleColors = true;
            this.metroCheckBoxShowPlayersClasses.UseVisualStyleBackColor = true;
            this.metroCheckBoxShowPlayersClasses.CheckedChanged += new System.EventHandler(this.MetroCheckBoxShowPlayersClassesCheckedChanged);
            // 
            // metroCheckBoxShowNpcsNames
            // 
            this.metroCheckBoxShowNpcsNames.AutoSize = true;
            this.metroCheckBoxShowNpcsNames.CustomBackground = false;
            this.metroCheckBoxShowNpcsNames.CustomForeColor = false;
            this.metroCheckBoxShowNpcsNames.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.metroCheckBoxShowNpcsNames.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.metroCheckBoxShowNpcsNames.Location = new System.Drawing.Point(3, 10);
            this.metroCheckBoxShowNpcsNames.Name = "metroCheckBoxShowNpcsNames";
            this.metroCheckBoxShowNpcsNames.Size = new System.Drawing.Size(127, 19);
            this.metroCheckBoxShowNpcsNames.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroCheckBoxShowNpcsNames.StyleManager = null;
            this.metroCheckBoxShowNpcsNames.TabIndex = 48;
            this.metroCheckBoxShowNpcsNames.Text = "Show NPC name";
            this.metroCheckBoxShowNpcsNames.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroCheckBoxShowNpcsNames.UseStyleColors = true;
            this.metroCheckBoxShowNpcsNames.UseVisualStyleBackColor = true;
            this.metroCheckBoxShowNpcsNames.CheckedChanged += new System.EventHandler(this.MetroCheckBoxShowNpcsNamesCheckedChanged);
            // 
            // metroCheckBoxShowObjectsNames
            // 
            this.metroCheckBoxShowObjectsNames.AutoSize = true;
            this.metroCheckBoxShowObjectsNames.CustomBackground = false;
            this.metroCheckBoxShowObjectsNames.CustomForeColor = false;
            this.metroCheckBoxShowObjectsNames.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.metroCheckBoxShowObjectsNames.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.metroCheckBoxShowObjectsNames.Location = new System.Drawing.Point(140, 10);
            this.metroCheckBoxShowObjectsNames.Name = "metroCheckBoxShowObjectsNames";
            this.metroCheckBoxShowObjectsNames.Size = new System.Drawing.Size(137, 19);
            this.metroCheckBoxShowObjectsNames.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroCheckBoxShowObjectsNames.StyleManager = null;
            this.metroCheckBoxShowObjectsNames.TabIndex = 49;
            this.metroCheckBoxShowObjectsNames.Text = "Show object name";
            this.metroCheckBoxShowObjectsNames.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroCheckBoxShowObjectsNames.UseStyleColors = true;
            this.metroCheckBoxShowObjectsNames.UseVisualStyleBackColor = true;
            this.metroCheckBoxShowObjectsNames.CheckedChanged += new System.EventHandler(this.MetroCheckBoxShowObjectsNamesCheckedChanged);
            // 
            // metroTextBoxAddNew
            // 
            this.metroTextBoxAddNew.CustomBackground = false;
            this.metroTextBoxAddNew.CustomForeColor = false;
            this.metroTextBoxAddNew.FontSize = MetroFramework.MetroTextBoxSize.Small;
            this.metroTextBoxAddNew.FontWeight = MetroFramework.MetroTextBoxWeight.Regular;
            this.metroTextBoxAddNew.Location = new System.Drawing.Point(3, 243);
            this.metroTextBoxAddNew.Multiline = false;
            this.metroTextBoxAddNew.Name = "metroTextBoxAddNew";
            this.metroTextBoxAddNew.SelectedText = "";
            this.metroTextBoxAddNew.Size = new System.Drawing.Size(311, 23);
            this.metroTextBoxAddNew.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBoxAddNew.StyleManager = null;
            this.metroTextBoxAddNew.TabIndex = 54;
            this.metroTextBoxAddNew.Text = "Enter object/npc name here...";
            this.metroTextBoxAddNew.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBoxAddNew.UseStyleColors = true;
            // 
            // buttonAddUnknown
            // 
            this.buttonAddUnknown.Highlight = true;
            this.buttonAddUnknown.Location = new System.Drawing.Point(320, 243);
            this.buttonAddUnknown.Name = "buttonAddUnknown";
            this.buttonAddUnknown.Size = new System.Drawing.Size(71, 23);
            this.buttonAddUnknown.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonAddUnknown.StyleManager = null;
            this.buttonAddUnknown.TabIndex = 55;
            this.buttonAddUnknown.Text = "Add";
            this.buttonAddUnknown.Theme = MetroFramework.MetroThemeStyle.Light;
            this.buttonAddUnknown.Click += new System.EventHandler(this.buttonAddUnknown_Click);
            // 
            // comboboxNPCs
            // 
            this.comboboxNPCs.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboboxNPCs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxNPCs.FontSize = MetroFramework.MetroLinkSize.Small;
            this.comboboxNPCs.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.comboboxNPCs.FormattingEnabled = true;
            this.comboboxNPCs.ItemHeight = 19;
            this.comboboxNPCs.Location = new System.Drawing.Point(3, 272);
            this.comboboxNPCs.Name = "comboboxNPCs";
            this.comboboxNPCs.OverlayText = "List of visible NPCs...";
            this.comboboxNPCs.Size = new System.Drawing.Size(311, 25);
            this.comboboxNPCs.Style = MetroFramework.MetroColorStyle.Blue;
            this.comboboxNPCs.StyleManager = null;
            this.comboboxNPCs.TabIndex = 59;
            this.comboboxNPCs.Theme = MetroFramework.MetroThemeStyle.Light;
            this.comboboxNPCs.Click += new System.EventHandler(this.comboboxNPCs_Click);
            // 
            // comboboxObjects
            // 
            this.comboboxObjects.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboboxObjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxObjects.FontSize = MetroFramework.MetroLinkSize.Small;
            this.comboboxObjects.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.comboboxObjects.FormattingEnabled = true;
            this.comboboxObjects.ItemHeight = 19;
            this.comboboxObjects.Location = new System.Drawing.Point(3, 303);
            this.comboboxObjects.Name = "comboboxObjects";
            this.comboboxObjects.OverlayText = "List of visible objects...";
            this.comboboxObjects.Size = new System.Drawing.Size(311, 25);
            this.comboboxObjects.Style = MetroFramework.MetroColorStyle.Blue;
            this.comboboxObjects.StyleManager = null;
            this.comboboxObjects.TabIndex = 60;
            this.comboboxObjects.Theme = MetroFramework.MetroThemeStyle.Light;
            this.comboboxObjects.Click += new System.EventHandler(this.comboboxObjects_Click);
            // 
            // buttonAddObject
            // 
            this.buttonAddObject.Highlight = true;
            this.buttonAddObject.Location = new System.Drawing.Point(320, 303);
            this.buttonAddObject.Name = "buttonAddObject";
            this.buttonAddObject.Size = new System.Drawing.Size(71, 25);
            this.buttonAddObject.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonAddObject.StyleManager = null;
            this.buttonAddObject.TabIndex = 61;
            this.buttonAddObject.Text = "Add";
            this.buttonAddObject.Theme = MetroFramework.MetroThemeStyle.Light;
            this.buttonAddObject.Click += new System.EventHandler(this.buttonAddObject_Click);
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.tabPageTrackingUnits);
            this.metroTabControl1.Controls.Add(this.tabPageAppearance);
            this.metroTabControl1.CustomBackground = false;
            this.metroTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroTabControl1.FontSize = MetroFramework.MetroTabControlSize.Medium;
            this.metroTabControl1.FontWeight = MetroFramework.MetroTabControlWeight.Bold;
            this.metroTabControl1.Location = new System.Drawing.Point(15, 30);
            this.metroTabControl1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(991, 368);
            this.metroTabControl1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabControl1.TabIndex = 62;
            this.metroTabControl1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.metroTabControl1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTabControl1.UseStyleColors = true;
            // 
            // tabPageTrackingUnits
            // 
            this.tabPageTrackingUnits.Controls.Add(this.oListView);
            this.tabPageTrackingUnits.Controls.Add(this.buttonSaveFile);
            this.tabPageTrackingUnits.Controls.Add(this.buttonOpenFile);
            this.tabPageTrackingUnits.Controls.Add(this.buttonAddObject);
            this.tabPageTrackingUnits.Controls.Add(this.comboboxObjects);
            this.tabPageTrackingUnits.Controls.Add(this.dataGridViewObjects);
            this.tabPageTrackingUnits.Controls.Add(this.comboboxNPCs);
            this.tabPageTrackingUnits.Controls.Add(this.buttonAddUnknown);
            this.tabPageTrackingUnits.Controls.Add(this.metroTextBoxAddNew);
            this.tabPageTrackingUnits.Controls.Add(this.buttonAddNPC);
            this.tabPageTrackingUnits.CustomBackground = false;
            this.tabPageTrackingUnits.HorizontalScrollbar = false;
            this.tabPageTrackingUnits.HorizontalScrollbarBarColor = true;
            this.tabPageTrackingUnits.HorizontalScrollbarHighlightOnWheel = false;
            this.tabPageTrackingUnits.HorizontalScrollbarSize = 10;
            this.tabPageTrackingUnits.Location = new System.Drawing.Point(4, 35);
            this.tabPageTrackingUnits.Name = "tabPageTrackingUnits";
            this.tabPageTrackingUnits.Size = new System.Drawing.Size(983, 329);
            this.tabPageTrackingUnits.Style = MetroFramework.MetroColorStyle.Blue;
            this.tabPageTrackingUnits.StyleManager = null;
            this.tabPageTrackingUnits.TabIndex = 0;
            this.tabPageTrackingUnits.Text = "             Tracking units           ";
            this.tabPageTrackingUnits.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tabPageTrackingUnits.VerticalScrollbar = false;
            this.tabPageTrackingUnits.VerticalScrollbarBarColor = true;
            this.tabPageTrackingUnits.VerticalScrollbarHighlightOnWheel = false;
            this.tabPageTrackingUnits.VerticalScrollbarSize = 10;
            // 
            // oListView
            // 
            this.oListView.AllColumns.Add(this.oColumnName);
            this.oListView.AllColumns.Add(this.oColumnInteract);
            this.oListView.AllColumns.Add(this.oColumnSoundAlarm);
            this.oListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.oListView.CheckBoxes = true;
            this.oListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.oColumnName,
            this.oColumnInteract,
            this.oColumnSoundAlarm});
            this.oListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.oListView.FullRowSelect = true;
            this.oListView.HeaderWordWrap = true;
            this.oListView.HideSelection = false;
            this.oListView.IncludeColumnHeadersInCopy = true;
            this.oListView.Location = new System.Drawing.Point(397, 39);
            this.oListView.Name = "oListView";
            this.oListView.OwnerDraw = true;
            this.oListView.ShowGroups = false;
            this.oListView.ShowImagesOnSubItems = true;
            this.oListView.Size = new System.Drawing.Size(386, 198);
            this.oListView.TabIndex = 63;
            this.oListView.UseAlternatingBackColors = true;
            this.oListView.UseCompatibleStateImageBehavior = false;
            this.oListView.UseHotItem = true;
            this.oListView.UseSubItemCheckBoxes = true;
            this.oListView.View = System.Windows.Forms.View.Details;
            // 
            // oColumnName
            // 
            this.oColumnName.AspectName = "Name";
            this.oColumnName.MaximumWidth = 265;
            this.oColumnName.MinimumWidth = 265;
            this.oColumnName.Text = "Name";
            this.oColumnName.Width = 265;
            // 
            // oColumnInteract
            // 
            this.oColumnInteract.AspectName = "Interact";
            this.oColumnInteract.CheckBoxes = true;
            this.oColumnInteract.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.oColumnInteract.MaximumWidth = 50;
            this.oColumnInteract.MinimumWidth = 50;
            this.oColumnInteract.Text = "Interact";
            this.oColumnInteract.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.oColumnInteract.Width = 50;
            // 
            // oColumnSoundAlarm
            // 
            this.oColumnSoundAlarm.AspectName = "SoundAlarm";
            this.oColumnSoundAlarm.CheckBoxes = true;
            this.oColumnSoundAlarm.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.oColumnSoundAlarm.MaximumWidth = 50;
            this.oColumnSoundAlarm.MinimumWidth = 50;
            this.oColumnSoundAlarm.Text = "Sound alarm";
            this.oColumnSoundAlarm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.oColumnSoundAlarm.Width = 50;
            // 
            // buttonSaveFile
            // 
            this.buttonSaveFile.Highlight = true;
            this.buttonSaveFile.Location = new System.Drawing.Point(200, 10);
            this.buttonSaveFile.Name = "buttonSaveFile";
            this.buttonSaveFile.Size = new System.Drawing.Size(191, 23);
            this.buttonSaveFile.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonSaveFile.StyleManager = null;
            this.buttonSaveFile.TabIndex = 64;
            this.buttonSaveFile.Text = "Save list to file";
            this.buttonSaveFile.Theme = MetroFramework.MetroThemeStyle.Light;
            this.buttonSaveFile.Click += new System.EventHandler(this.buttonSaveFile_Click);
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Highlight = true;
            this.buttonOpenFile.Location = new System.Drawing.Point(3, 10);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(191, 23);
            this.buttonOpenFile.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonOpenFile.StyleManager = null;
            this.buttonOpenFile.TabIndex = 63;
            this.buttonOpenFile.Text = "Load list from file";
            this.buttonOpenFile.Theme = MetroFramework.MetroThemeStyle.Light;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // tabPageAppearance
            // 
            this.tabPageAppearance.Controls.Add(this.metroCheckBoxShowPlayersClasses);
            this.tabPageAppearance.Controls.Add(this.metroCheckBoxShowNpcsNames);
            this.tabPageAppearance.Controls.Add(this.metroCheckBoxShowObjectsNames);
            this.tabPageAppearance.CustomBackground = false;
            this.tabPageAppearance.HorizontalScrollbar = false;
            this.tabPageAppearance.HorizontalScrollbarBarColor = true;
            this.tabPageAppearance.HorizontalScrollbarHighlightOnWheel = false;
            this.tabPageAppearance.HorizontalScrollbarSize = 10;
            this.tabPageAppearance.Location = new System.Drawing.Point(4, 35);
            this.tabPageAppearance.Name = "tabPageAppearance";
            this.tabPageAppearance.Size = new System.Drawing.Size(983, 329);
            this.tabPageAppearance.Style = MetroFramework.MetroColorStyle.Blue;
            this.tabPageAppearance.StyleManager = null;
            this.tabPageAppearance.TabIndex = 1;
            this.tabPageAppearance.Text = "               Appearance            ";
            this.tabPageAppearance.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tabPageAppearance.VerticalScrollbar = false;
            this.tabPageAppearance.VerticalScrollbarBarColor = true;
            this.tabPageAppearance.VerticalScrollbarHighlightOnWheel = false;
            this.tabPageAppearance.VerticalScrollbarSize = 10;
            // 
            // WowRadarOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1021, 408);
            this.Controls.Add(this.metroTabControl1);
            this.DisplayHeader = false;
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.Name = "WowRadarOptions";
            this.Padding = new System.Windows.Forms.Padding(15, 30, 15, 10);
            this.Resizable = false;
            this.ShowInTaskbar = false;
            this.Text = "Radar settings";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewObjects)).EndInit();
            this.metroTabControl1.ResumeLayout(false);
            this.tabPageTrackingUnits.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.oListView)).EndInit();
            this.tabPageAppearance.ResumeLayout(false);
            this.tabPageAppearance.PerformLayout();
            this.ResumeLayout(false);

        }

        

        #endregion

        private MetroFramework.Components.MetroToolTip toolTip1;
        private MetroFramework.Controls.MetroButton buttonAddNPC;
        private MetroCheckBox metroCheckBoxShowObjectsNames;
        private MetroCheckBox metroCheckBoxShowNpcsNames;
        private MetroCheckBox metroCheckBoxShowPlayersClasses;
        private DataGridViewExt dataGridViewObjects;
        private MetroButton buttonAddUnknown;
        private MetroTextBox metroTextBoxAddNew;
        private MetroButton buttonAddObject;
        private MetroComboboxExt comboboxObjects;
        private MetroComboboxExt comboboxNPCs;
        private MetroTabControl metroTabControl1;
        private MetroTabPage tabPageTrackingUnits;
        private MetroTabPage tabPageAppearance;
        private DataGridViewCheckBoxColumn ObjectToFindEnabled;
        private DataGridViewTextBoxColumn ObjectName;
        private DataGridViewCheckBoxColumn Interact;
        private DataGridViewCheckBoxColumn SoundAlarm;
        private MetroButton buttonSaveFile;
        private MetroButton buttonOpenFile;
        private BrightIdeasSoftware.ObjectListView oListView;
        private BrightIdeasSoftware.OLVColumn oColumnName;
        private BrightIdeasSoftware.OLVColumn oColumnInteract;
        private BrightIdeasSoftware.OLVColumn oColumnSoundAlarm;
    }
}