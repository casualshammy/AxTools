using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AxTools.Components;
using AxTools.Properties;
using MetroFramework;
using MetroFramework.Components;

namespace AxTools.Forms
{
    internal partial class WowRadar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WowRadar));
            this.pictureBoxMain = new System.Windows.Forms.PictureBox();
            this.pictureBoxClose = new AxTools.Components.PictureBoxExt(this.components);
            this.pictureBoxRadarSettings = new AxTools.Components.PictureBoxExt(this.components);
            this.toolTip1 = new MetroFramework.Components.MetroToolTip();
            this.checkBoxFriends = new AxTools.Components.CheckBoxExt(this.components);
            this.checkBoxEnemies = new AxTools.Components.CheckBoxExt(this.components);
            this.checkBoxCorpses = new System.Windows.Forms.CheckBox();
            this.textBoxDetailedInfo = new System.Windows.Forms.TextBox();
            this.checkBoxObjects = new AxTools.Components.CheckBoxExt(this.components);
            this.checkBoxNpcs = new AxTools.Components.CheckBoxExt(this.components);
            this.labelHint = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRadarSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxMain
            // 
            this.pictureBoxMain.BackColor = System.Drawing.Color.Black;
            this.pictureBoxMain.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxMain.Name = "pictureBoxMain";
            this.pictureBoxMain.Size = new System.Drawing.Size(225, 225);
            this.pictureBoxMain.TabIndex = 2;
            this.pictureBoxMain.TabStop = false;
            this.pictureBoxMain.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox1Paint);
            this.pictureBoxMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBoxMainMouseClick);
            this.pictureBoxMain.MouseLeave += new System.EventHandler(this.PictureBoxMainMouseLeave);
            // 
            // pictureBoxClose
            // 
            this.pictureBoxClose.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxClose.Image")));
            this.pictureBoxClose.ImageOnHover = ((System.Drawing.Image)(resources.GetObject("pictureBoxClose.ImageOnHover")));
            this.pictureBoxClose.Location = new System.Drawing.Point(205, 0);
            this.pictureBoxClose.Margin = new System.Windows.Forms.Padding(1);
            this.pictureBoxClose.Name = "pictureBoxClose";
            this.pictureBoxClose.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxClose.TabIndex = 6;
            this.pictureBoxClose.TabStop = false;
            this.pictureBoxClose.Click += new System.EventHandler(this.PictureBox2Click);
            // 
            // pictureBoxRadarSettings
            // 
            this.pictureBoxRadarSettings.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxRadarSettings.Image")));
            this.pictureBoxRadarSettings.ImageOnHover = ((System.Drawing.Image)(resources.GetObject("pictureBoxRadarSettings.ImageOnHover")));
            this.pictureBoxRadarSettings.Location = new System.Drawing.Point(183, 0);
            this.pictureBoxRadarSettings.Margin = new System.Windows.Forms.Padding(1);
            this.pictureBoxRadarSettings.Name = "pictureBoxRadarSettings";
            this.pictureBoxRadarSettings.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxRadarSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxRadarSettings.TabIndex = 38;
            this.pictureBoxRadarSettings.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxRadarSettings, "Click to open radar settings");
            this.pictureBoxRadarSettings.Click += new System.EventHandler(this.PictureBoxRadarSettingsClick);
            // 
            // toolTip1
            // 
            this.toolTip1.Style = MetroFramework.MetroColorStyle.Blue;
            this.toolTip1.StyleManager = null;
            this.toolTip1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // checkBoxFriends
            // 
            this.checkBoxFriends.AutoSize = true;
            this.checkBoxFriends.ForeColor = System.Drawing.Color.Green;
            this.checkBoxFriends.Location = new System.Drawing.Point(12, 231);
            this.checkBoxFriends.Name = "checkBoxFriends";
            this.checkBoxFriends.Size = new System.Drawing.Size(79, 17);
            this.checkBoxFriends.TabIndex = 3;
            this.checkBoxFriends.Text = "F: 999/999";
            this.toolTip1.SetToolTip(this.checkBoxFriends, "Show friendly players");
            this.checkBoxFriends.UseVisualStyleBackColor = true;
            this.checkBoxFriends.MouseClickExtended += new AxTools.Components.CheckBoxExt.MouseClickExt(this.CheckBoxFriendsMouseClickExtended);
            // 
            // checkBoxEnemies
            // 
            this.checkBoxEnemies.AutoSize = true;
            this.checkBoxEnemies.ForeColor = System.Drawing.Color.Red;
            this.checkBoxEnemies.Location = new System.Drawing.Point(93, 231);
            this.checkBoxEnemies.Name = "checkBoxEnemies";
            this.checkBoxEnemies.Size = new System.Drawing.Size(80, 17);
            this.checkBoxEnemies.TabIndex = 4;
            this.checkBoxEnemies.Text = "E: 999/999";
            this.toolTip1.SetToolTip(this.checkBoxEnemies, "Show hostile players");
            this.checkBoxEnemies.UseVisualStyleBackColor = true;
            this.checkBoxEnemies.MouseClickExtended += new AxTools.Components.CheckBoxExt.MouseClickExt(this.CheckBoxEnemiesMouseClickExtended);
            // 
            // checkBoxCorpses
            // 
            this.checkBoxCorpses.AutoSize = true;
            this.checkBoxCorpses.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.checkBoxCorpses.ForeColor = System.Drawing.Color.Gray;
            this.checkBoxCorpses.Location = new System.Drawing.Point(177, 236);
            this.checkBoxCorpses.Name = "checkBoxCorpses";
            this.checkBoxCorpses.Size = new System.Drawing.Size(49, 31);
            this.checkBoxCorpses.TabIndex = 39;
            this.checkBoxCorpses.Text = "Corpses";
            this.checkBoxCorpses.UseVisualStyleBackColor = true;
            // 
            // textBoxDetailedInfo
            // 
            this.textBoxDetailedInfo.BackColor = System.Drawing.Color.Black;
            this.textBoxDetailedInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxDetailedInfo.ForeColor = System.Drawing.Color.Black;
            this.textBoxDetailedInfo.Location = new System.Drawing.Point(54, 184);
            this.textBoxDetailedInfo.Multiline = true;
            this.textBoxDetailedInfo.Name = "textBoxDetailedInfo";
            this.textBoxDetailedInfo.Size = new System.Drawing.Size(125, 26);
            this.textBoxDetailedInfo.TabIndex = 40;
            this.textBoxDetailedInfo.Text = "ММММММММММММ\r\n(War90) 100%";
            this.textBoxDetailedInfo.Visible = false;
            // 
            // checkBoxObjects
            // 
            this.checkBoxObjects.AutoSize = true;
            this.checkBoxObjects.ForeColor = System.Drawing.Color.Gold;
            this.checkBoxObjects.Location = new System.Drawing.Point(93, 254);
            this.checkBoxObjects.Name = "checkBoxObjects";
            this.checkBoxObjects.Size = new System.Drawing.Size(86, 17);
            this.checkBoxObjects.TabIndex = 5;
            this.checkBoxObjects.Text = "Objects: 999";
            this.checkBoxObjects.UseVisualStyleBackColor = true;
            this.checkBoxObjects.MouseClickExtended += new AxTools.Components.CheckBoxExt.MouseClickExt(this.CheckBoxObjectsMouseClickExtended);
            // 
            // checkBoxNpcs
            // 
            this.checkBoxNpcs.AutoSize = true;
            this.checkBoxNpcs.ForeColor = System.Drawing.Color.GreenYellow;
            this.checkBoxNpcs.Location = new System.Drawing.Point(12, 254);
            this.checkBoxNpcs.Name = "checkBoxNpcs";
            this.checkBoxNpcs.Size = new System.Drawing.Size(81, 17);
            this.checkBoxNpcs.TabIndex = 7;
            this.checkBoxNpcs.Text = "N: 999/999";
            this.checkBoxNpcs.UseVisualStyleBackColor = true;
            this.checkBoxNpcs.MouseClickExtended += new AxTools.Components.CheckBoxExt.MouseClickExt(this.CheckBoxNpcsMouseClickExtended);
            // 
            // labelHint
            // 
            this.labelHint.AutoSize = true;
            this.labelHint.ForeColor = System.Drawing.SystemColors.Control;
            this.labelHint.Location = new System.Drawing.Point(3, 3);
            this.labelHint.Name = "labelHint";
            this.labelHint.Size = new System.Drawing.Size(123, 39);
            this.labelHint.TabIndex = 41;
            this.labelHint.Text = "Left click: target/interact\r\nRight click: move to\r\nMouse wheel: zoom";
            // 
            // WowRadar
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(225, 274);
            this.Controls.Add(this.labelHint);
            this.Controls.Add(this.textBoxDetailedInfo);
            this.Controls.Add(this.checkBoxCorpses);
            this.Controls.Add(this.pictureBoxRadarSettings);
            this.Controls.Add(this.checkBoxNpcs);
            this.Controls.Add(this.pictureBoxClose);
            this.Controls.Add(this.checkBoxObjects);
            this.Controls.Add(this.checkBoxEnemies);
            this.Controls.Add(this.checkBoxFriends);
            this.Controls.Add(this.pictureBoxMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WowRadar";
            this.Opacity = 0.7D;
            this.Text = "Radar";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RadarFormClosing);
            this.Load += new System.EventHandler(this.RadarLoad);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RadarMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RadarMouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RadarMouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRadarSettings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pictureBoxMain;
        private CheckBoxExt checkBoxFriends;
        private CheckBoxExt checkBoxEnemies;
        private CheckBoxExt checkBoxObjects;
        private CheckBoxExt checkBoxNpcs;
        private PictureBoxExt pictureBoxClose;
        private PictureBoxExt pictureBoxRadarSettings;
        private MetroToolTip toolTip1;
        private CheckBox checkBoxCorpses;
        private TextBox textBoxDetailedInfo;
        private Label labelHint;
    }
}