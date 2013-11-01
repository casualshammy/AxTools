using System.Windows.Forms;
using AxTools.Components;
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
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager();
            this.toolTip1 = new MetroFramework.Components.MetroToolTip();
            this.dataGridViewObjects = new AxTools.Components.DataGridViewExt();
            this.ObjectToFindEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ObjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Interact = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SoundAlarm = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pictureBoxSaveFile = new AxTools.Components.PictureBoxExt(this.components);
            this.pictureBoxOpenFile = new AxTools.Components.PictureBoxExt(this.components);
            this.pictureBoxDeleteLastLine = new AxTools.Components.PictureBoxExt(this.components);
            this.comboBoxSelectObjectOrNpc = new MetroFramework.Controls.MetroComboBox();
            this.label3 = new MetroFramework.Controls.MetroLabel();
            this.buttonAddObjectOrNpcToList = new MetroFramework.Controls.MetroButton();
            this.metroCheckBoxShowPlayersClasses = new MetroFramework.Controls.MetroCheckBox();
            this.metroCheckBoxShowNpcsNames = new MetroFramework.Controls.MetroCheckBox();
            this.metroCheckBoxShowObjectsNames = new MetroFramework.Controls.MetroCheckBox();
            this.metroTextBoxAddNew = new MetroFramework.Controls.MetroTextBox();
            this.metroButtonAddNew = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewObjects)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSaveFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOpenFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDeleteLastLine)).BeginInit();
            this.SuspendLayout();
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.OwnerForm = this;
            this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
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
            this.dataGridViewObjects.Location = new System.Drawing.Point(12, 69);
            this.dataGridViewObjects.Name = "dataGridViewObjects";
            this.dataGridViewObjects.RowHeadersVisible = false;
            this.dataGridViewObjects.Size = new System.Drawing.Size(360, 198);
            this.dataGridViewObjects.TabIndex = 53;
            this.toolTip1.SetToolTip(this.dataGridViewObjects, "Right click on item to delete it");
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
            this.ObjectName.FillWeight = 237F;
            this.ObjectName.HeaderText = "Name";
            this.ObjectName.Name = "ObjectName";
            this.ObjectName.Width = 237;
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
            // pictureBoxSaveFile
            // 
            this.pictureBoxSaveFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxSaveFile.Image = global::AxTools.Properties.Resources.document_save;
            this.pictureBoxSaveFile.ImageOnHover = global::AxTools.Properties.Resources.DocumentSaveLight;
            this.pictureBoxSaveFile.Location = new System.Drawing.Point(378, 97);
            this.pictureBoxSaveFile.Name = "pictureBoxSaveFile";
            this.pictureBoxSaveFile.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxSaveFile.TabIndex = 58;
            this.pictureBoxSaveFile.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxSaveFile, "Save list to file...");
            this.pictureBoxSaveFile.Click += new System.EventHandler(this.PictureBoxSaveFileClick);
            // 
            // pictureBoxOpenFile
            // 
            this.pictureBoxOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxOpenFile.Image = global::AxTools.Properties.Resources.document_open;
            this.pictureBoxOpenFile.ImageOnHover = global::AxTools.Properties.Resources.DocumentOpenLight;
            this.pictureBoxOpenFile.Location = new System.Drawing.Point(378, 69);
            this.pictureBoxOpenFile.Name = "pictureBoxOpenFile";
            this.pictureBoxOpenFile.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxOpenFile.TabIndex = 57;
            this.pictureBoxOpenFile.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxOpenFile, "Open list from file...");
            this.pictureBoxOpenFile.Click += new System.EventHandler(this.PictureBoxOpenFileClick);
            // 
            // pictureBoxDeleteLastLine
            // 
            this.pictureBoxDeleteLastLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxDeleteLastLine.Image = global::AxTools.Properties.Resources.delete_line;
            this.pictureBoxDeleteLastLine.ImageOnHover = global::AxTools.Properties.Resources.DeleteLineLight;
            this.pictureBoxDeleteLastLine.Location = new System.Drawing.Point(378, 245);
            this.pictureBoxDeleteLastLine.Name = "pictureBoxDeleteLastLine";
            this.pictureBoxDeleteLastLine.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxDeleteLastLine.TabIndex = 59;
            this.pictureBoxDeleteLastLine.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxDeleteLastLine, "Delete last row");
            this.pictureBoxDeleteLastLine.Click += new System.EventHandler(this.PictureBoxDeleteLastLineClick);
            // 
            // comboBoxSelectObjectOrNpc
            // 
            this.comboBoxSelectObjectOrNpc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxSelectObjectOrNpc.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxSelectObjectOrNpc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectObjectOrNpc.FontSize = MetroFramework.MetroLinkSize.Small;
            this.comboBoxSelectObjectOrNpc.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.comboBoxSelectObjectOrNpc.FormattingEnabled = true;
            this.comboBoxSelectObjectOrNpc.ItemHeight = 19;
            this.comboBoxSelectObjectOrNpc.Location = new System.Drawing.Point(12, 329);
            this.comboBoxSelectObjectOrNpc.Name = "comboBoxSelectObjectOrNpc";
            this.comboBoxSelectObjectOrNpc.Size = new System.Drawing.Size(311, 25);
            this.comboBoxSelectObjectOrNpc.Style = MetroFramework.MetroColorStyle.Blue;
            this.comboBoxSelectObjectOrNpc.StyleManager = this.metroStyleManager1;
            this.comboBoxSelectObjectOrNpc.TabIndex = 36;
            this.comboBoxSelectObjectOrNpc.Theme = MetroFramework.MetroThemeStyle.Light;
            this.comboBoxSelectObjectOrNpc.Click += new System.EventHandler(this.ComboBoxSelectObjectOrNpcClick);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.CustomBackground = false;
            this.label3.CustomForeColor = false;
            this.label3.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.label3.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.label3.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.label3.Location = new System.Drawing.Point(12, 307);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(265, 19);
            this.label3.Style = MetroFramework.MetroColorStyle.Blue;
            this.label3.StyleManager = this.metroStyleManager1;
            this.label3.TabIndex = 37;
            this.label3.Text = "You can add visible object/npc from the list:";
            this.label3.Theme = MetroFramework.MetroThemeStyle.Light;
            this.label3.UseStyleColors = true;
            // 
            // buttonAddObjectOrNpcToList
            // 
            this.buttonAddObjectOrNpcToList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddObjectOrNpcToList.Highlight = true;
            this.buttonAddObjectOrNpcToList.Location = new System.Drawing.Point(329, 329);
            this.buttonAddObjectOrNpcToList.Name = "buttonAddObjectOrNpcToList";
            this.buttonAddObjectOrNpcToList.Size = new System.Drawing.Size(71, 25);
            this.buttonAddObjectOrNpcToList.Style = MetroFramework.MetroColorStyle.Blue;
            this.buttonAddObjectOrNpcToList.StyleManager = this.metroStyleManager1;
            this.buttonAddObjectOrNpcToList.TabIndex = 38;
            this.buttonAddObjectOrNpcToList.Text = "Add";
            this.buttonAddObjectOrNpcToList.Theme = MetroFramework.MetroThemeStyle.Light;
            this.buttonAddObjectOrNpcToList.Click += new System.EventHandler(this.ButtonAddObjectOrNpcToListClick);
            // 
            // metroCheckBoxShowPlayersClasses
            // 
            this.metroCheckBoxShowPlayersClasses.AutoSize = true;
            this.metroCheckBoxShowPlayersClasses.CustomBackground = false;
            this.metroCheckBoxShowPlayersClasses.CustomForeColor = false;
            this.metroCheckBoxShowPlayersClasses.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.metroCheckBoxShowPlayersClasses.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.metroCheckBoxShowPlayersClasses.Location = new System.Drawing.Point(23, 19);
            this.metroCheckBoxShowPlayersClasses.Name = "metroCheckBoxShowPlayersClasses";
            this.metroCheckBoxShowPlayersClasses.Size = new System.Drawing.Size(153, 19);
            this.metroCheckBoxShowPlayersClasses.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroCheckBoxShowPlayersClasses.StyleManager = this.metroStyleManager1;
            this.metroCheckBoxShowPlayersClasses.TabIndex = 47;
            this.metroCheckBoxShowPlayersClasses.Text = "Show players\' classes";
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
            this.metroCheckBoxShowNpcsNames.Location = new System.Drawing.Point(23, 44);
            this.metroCheckBoxShowNpcsNames.Name = "metroCheckBoxShowNpcsNames";
            this.metroCheckBoxShowNpcsNames.Size = new System.Drawing.Size(142, 19);
            this.metroCheckBoxShowNpcsNames.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroCheckBoxShowNpcsNames.StyleManager = this.metroStyleManager1;
            this.metroCheckBoxShowNpcsNames.TabIndex = 48;
            this.metroCheckBoxShowNpcsNames.Text = "Show NPCs\' names";
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
            this.metroCheckBoxShowObjectsNames.Location = new System.Drawing.Point(171, 44);
            this.metroCheckBoxShowObjectsNames.Name = "metroCheckBoxShowObjectsNames";
            this.metroCheckBoxShowObjectsNames.Size = new System.Drawing.Size(152, 19);
            this.metroCheckBoxShowObjectsNames.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroCheckBoxShowObjectsNames.StyleManager = this.metroStyleManager1;
            this.metroCheckBoxShowObjectsNames.TabIndex = 49;
            this.metroCheckBoxShowObjectsNames.Text = "Show objects\' names";
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
            this.metroTextBoxAddNew.Location = new System.Drawing.Point(12, 273);
            this.metroTextBoxAddNew.Multiline = false;
            this.metroTextBoxAddNew.Name = "metroTextBoxAddNew";
            this.metroTextBoxAddNew.SelectedText = "";
            this.metroTextBoxAddNew.Size = new System.Drawing.Size(311, 23);
            this.metroTextBoxAddNew.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBoxAddNew.StyleManager = this.metroStyleManager1;
            this.metroTextBoxAddNew.TabIndex = 54;
            this.metroTextBoxAddNew.Text = "Enter object/npc name here...";
            this.metroTextBoxAddNew.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBoxAddNew.UseStyleColors = true;
            // 
            // metroButtonAddNew
            // 
            this.metroButtonAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroButtonAddNew.Highlight = true;
            this.metroButtonAddNew.Location = new System.Drawing.Point(329, 273);
            this.metroButtonAddNew.Name = "metroButtonAddNew";
            this.metroButtonAddNew.Size = new System.Drawing.Size(71, 23);
            this.metroButtonAddNew.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroButtonAddNew.StyleManager = this.metroStyleManager1;
            this.metroButtonAddNew.TabIndex = 55;
            this.metroButtonAddNew.Text = "Add";
            this.metroButtonAddNew.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroButtonAddNew.Click += new System.EventHandler(this.MetroButtonAddNewClick);
            // 
            // WowRadarOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 366);
            this.Controls.Add(this.pictureBoxDeleteLastLine);
            this.Controls.Add(this.pictureBoxSaveFile);
            this.Controls.Add(this.pictureBoxOpenFile);
            this.Controls.Add(this.metroButtonAddNew);
            this.Controls.Add(this.metroTextBoxAddNew);
            this.Controls.Add(this.dataGridViewObjects);
            this.Controls.Add(this.metroCheckBoxShowObjectsNames);
            this.Controls.Add(this.metroCheckBoxShowNpcsNames);
            this.Controls.Add(this.metroCheckBoxShowPlayersClasses);
            this.Controls.Add(this.buttonAddObjectOrNpcToList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxSelectObjectOrNpc);
            this.DisplayHeader = false;
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.Name = "WowRadarOptions";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.ShowInTaskbar = false;
            this.StyleManager = this.metroStyleManager1;
            this.Text = "Radar settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.RadarNpcObjectSelectionLoad);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewObjects)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSaveFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOpenFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDeleteLastLine)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        

        #endregion

        private MetroFramework.Components.MetroToolTip toolTip1;
        private MetroFramework.Controls.MetroComboBox comboBoxSelectObjectOrNpc;
        private MetroFramework.Controls.MetroLabel label3;
        private MetroFramework.Controls.MetroButton buttonAddObjectOrNpcToList;
        private MetroStyleManager metroStyleManager1;
        private MetroCheckBox metroCheckBoxShowObjectsNames;
        private MetroCheckBox metroCheckBoxShowNpcsNames;
        private MetroCheckBox metroCheckBoxShowPlayersClasses;
        private DataGridViewExt dataGridViewObjects;
        private MetroButton metroButtonAddNew;
        private MetroTextBox metroTextBoxAddNew;
        private PictureBoxExt pictureBoxDeleteLastLine;
        private PictureBoxExt pictureBoxSaveFile;
        private PictureBoxExt pictureBoxOpenFile;
        private DataGridViewCheckBoxColumn ObjectToFindEnabled;
        private DataGridViewTextBoxColumn ObjectName;
        private DataGridViewCheckBoxColumn Interact;
        private DataGridViewCheckBoxColumn SoundAlarm;
    }
}