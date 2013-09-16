using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AxTools.Classes.WoW;
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
            this.components = new Container();
            this.timer1 = new Timer(this.components);
            this.pictureBoxMain = new PictureBox();
            this.checkBoxFriends = new CheckBoxExt(this.components);
            this.checkBoxEnemies = new CheckBoxExt(this.components);
            this.checkBoxObjects = new CheckBoxExt(this.components);
            this.pictureBox2 = new PictureBox();
            this.checkBoxNpcs = new CheckBoxExt(this.components);
            this.pictureBox_ZoomOut = new PictureBoxExt(this.components);
            this.pictureBox_ZoomIn = new PictureBoxExt(this.components);
            this.pictureBoxRadarSettings = new PictureBoxExt(this.components);
            this.toolTip1 = new MetroToolTip();
            this.checkBoxCorpses = new CheckBox();
            this.textBoxDetailedInfo = new TextBox();
            ((ISupportInitialize)(this.pictureBoxMain)).BeginInit();
            ((ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((ISupportInitialize)(this.pictureBox_ZoomOut)).BeginInit();
            ((ISupportInitialize)(this.pictureBox_ZoomIn)).BeginInit();
            ((ISupportInitialize)(this.pictureBoxRadarSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new EventHandler(this.Timer1Tick);
            // 
            // pictureBoxMain
            // 
            this.pictureBoxMain.BackColor = Color.Black;
            this.pictureBoxMain.Location = new Point(0, 0);
            this.pictureBoxMain.Name = "pictureBoxMain";
            this.pictureBoxMain.Size = new Size(225, 225);
            this.pictureBoxMain.TabIndex = 2;
            this.pictureBoxMain.TabStop = false;
            this.pictureBoxMain.Paint += new PaintEventHandler(this.PictureBox1Paint);
            this.pictureBoxMain.MouseClick += new MouseEventHandler(this.PictureBoxMainMouseClick);
            this.pictureBoxMain.MouseLeave += new EventHandler(this.PictureBoxMainMouseLeave);
            // 
            // checkBoxFriends
            // 
            this.checkBoxFriends.AutoSize = true;
            this.checkBoxFriends.ForeColor = Color.Green;
            this.checkBoxFriends.Location = new Point(12, 231);
            this.checkBoxFriends.Name = "checkBoxFriends";
            this.checkBoxFriends.Size = new Size(79, 17);
            this.checkBoxFriends.TabIndex = 3;
            this.checkBoxFriends.Text = "F: 999/999";
            this.toolTip1.SetToolTip(this.checkBoxFriends, "Show friendly players");
            this.checkBoxFriends.UseVisualStyleBackColor = true;
            this.checkBoxFriends.MouseClickExtended += new CheckBoxExt.MouseClickExt(this.CheckBoxFriendsMouseClickExtended);
            // 
            // checkBoxEnemies
            // 
            this.checkBoxEnemies.AutoSize = true;
            this.checkBoxEnemies.ForeColor = Color.Red;
            this.checkBoxEnemies.Location = new Point(93, 231);
            this.checkBoxEnemies.Name = "checkBoxEnemies";
            this.checkBoxEnemies.Size = new Size(80, 17);
            this.checkBoxEnemies.TabIndex = 4;
            this.checkBoxEnemies.Text = "E: 999/999";
            this.toolTip1.SetToolTip(this.checkBoxEnemies, "Show hostile players");
            this.checkBoxEnemies.UseVisualStyleBackColor = true;
            this.checkBoxEnemies.MouseClickExtended += new CheckBoxExt.MouseClickExt(this.CheckBoxEnemiesMouseClickExtended);
            // 
            // checkBoxObjects
            // 
            this.checkBoxObjects.AutoSize = true;
            this.checkBoxObjects.ForeColor = Color.Gold;
            this.checkBoxObjects.Location = new Point(93, 254);
            this.checkBoxObjects.Name = "checkBoxObjects";
            this.checkBoxObjects.Size = new Size(86, 17);
            this.checkBoxObjects.TabIndex = 5;
            this.checkBoxObjects.Text = "Objects: 999";
            this.checkBoxObjects.UseVisualStyleBackColor = true;
            this.checkBoxObjects.MouseClickExtended += new CheckBoxExt.MouseClickExt(this.CheckBoxObjectsMouseClickExtended);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = Resources.close_41741;
            this.pictureBox2.Location = new Point(205, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(20, 20);
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new EventHandler(this.PictureBox2Click);
            // 
            // checkBoxNpcs
            // 
            this.checkBoxNpcs.AutoSize = true;
            this.checkBoxNpcs.ForeColor = Color.GreenYellow;
            this.checkBoxNpcs.Location = new Point(12, 254);
            this.checkBoxNpcs.Name = "checkBoxNpcs";
            this.checkBoxNpcs.Size = new Size(81, 17);
            this.checkBoxNpcs.TabIndex = 7;
            this.checkBoxNpcs.Text = "N: 999/999";
            this.checkBoxNpcs.UseVisualStyleBackColor = true;
            this.checkBoxNpcs.MouseClickExtended += new CheckBoxExt.MouseClickExt(this.CheckBoxNpcsMouseClickExtended);
            // 
            // pictureBox_ZoomOut
            // 
            this.pictureBox_ZoomOut.Image = Resources.plus;
            this.pictureBox_ZoomOut.ImageOnHover = Resources.PlusLight;
            this.pictureBox_ZoomOut.Location = new Point(0, 0);
            this.pictureBox_ZoomOut.Margin = new Padding(0);
            this.pictureBox_ZoomOut.Name = "pictureBox_ZoomOut";
            this.pictureBox_ZoomOut.Size = new Size(16, 16);
            this.pictureBox_ZoomOut.TabIndex = 35;
            this.pictureBox_ZoomOut.TabStop = false;
            this.pictureBox_ZoomOut.Click += new EventHandler(this.PictureBoxZoomOutClick);
            // 
            // pictureBox_ZoomIn
            // 
            this.pictureBox_ZoomIn.Image = Resources.minus;
            this.pictureBox_ZoomIn.ImageOnHover = Resources.MinusLight;
            this.pictureBox_ZoomIn.Location = new Point(16, 0);
            this.pictureBox_ZoomIn.Margin = new Padding(0);
            this.pictureBox_ZoomIn.Name = "pictureBox_ZoomIn";
            this.pictureBox_ZoomIn.Size = new Size(16, 16);
            this.pictureBox_ZoomIn.TabIndex = 36;
            this.pictureBox_ZoomIn.TabStop = false;
            this.pictureBox_ZoomIn.Click += new EventHandler(this.PictureBoxZoomInClick);
            // 
            // pictureBoxRadarSettings
            // 
            this.pictureBoxRadarSettings.Image = Resources.Settings20px;
            this.pictureBoxRadarSettings.ImageOnHover = Resources.SettingsLight20px;
            this.pictureBoxRadarSettings.Location = new Point(35, 0);
            this.pictureBoxRadarSettings.Name = "pictureBoxRadarSettings";
            this.pictureBoxRadarSettings.Size = new Size(16, 16);
            this.pictureBoxRadarSettings.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBoxRadarSettings.TabIndex = 38;
            this.pictureBoxRadarSettings.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxRadarSettings, "Click to open radar settings");
            this.pictureBoxRadarSettings.Click += new EventHandler(this.PictureBoxRadarSettingsClick);
            // 
            // toolTip1
            // 
            this.toolTip1.Style = MetroColorStyle.Blue;
            this.toolTip1.StyleManager = null;
            this.toolTip1.Theme = MetroThemeStyle.Light;
            // 
            // checkBoxCorpses
            // 
            this.checkBoxCorpses.AutoSize = true;
            this.checkBoxCorpses.CheckAlign = ContentAlignment.TopCenter;
            this.checkBoxCorpses.ForeColor = Color.Gray;
            this.checkBoxCorpses.Location = new Point(177, 236);
            this.checkBoxCorpses.Name = "checkBoxCorpses";
            this.checkBoxCorpses.Size = new Size(49, 31);
            this.checkBoxCorpses.TabIndex = 39;
            this.checkBoxCorpses.Text = "Corpses";
            this.checkBoxCorpses.UseVisualStyleBackColor = true;
            // 
            // textBoxDetailedInfo
            // 
            this.textBoxDetailedInfo.BackColor = Color.Black;
            this.textBoxDetailedInfo.BorderStyle = BorderStyle.None;
            this.textBoxDetailedInfo.ForeColor = Color.Black;
            this.textBoxDetailedInfo.Location = new Point(54, 184);
            this.textBoxDetailedInfo.Multiline = true;
            this.textBoxDetailedInfo.Name = "textBoxDetailedInfo";
            this.textBoxDetailedInfo.Size = new Size(125, 26);
            this.textBoxDetailedInfo.TabIndex = 40;
            this.textBoxDetailedInfo.Text = "ММММММММММММ\r\n(War90) 100%";
            this.textBoxDetailedInfo.Visible = false;
            // 
            // WowRadar
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.Black;
            this.ClientSize = new Size(225, 274);
            this.Controls.Add(this.textBoxDetailedInfo);
            this.Controls.Add(this.checkBoxCorpses);
            this.Controls.Add(this.pictureBoxRadarSettings);
            this.Controls.Add(this.pictureBox_ZoomIn);
            this.Controls.Add(this.pictureBox_ZoomOut);
            this.Controls.Add(this.checkBoxNpcs);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.checkBoxObjects);
            this.Controls.Add(this.checkBoxEnemies);
            this.Controls.Add(this.checkBoxFriends);
            this.Controls.Add(this.pictureBoxMain);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "WowRadar";
            this.Opacity = 0.7D;
            this.Text = "Radar";
            this.TopMost = true;
            this.FormClosing += new FormClosingEventHandler(this.RadarFormClosing);
            this.Load += new EventHandler(this.RadarLoad);
            this.MouseDown += new MouseEventHandler(this.RadarMouseDown);
            this.MouseMove += new MouseEventHandler(this.RadarMouseMove);
            this.MouseUp += new MouseEventHandler(this.RadarMouseUp);
            ((ISupportInitialize)(this.pictureBoxMain)).EndInit();
            ((ISupportInitialize)(this.pictureBox2)).EndInit();
            ((ISupportInitialize)(this.pictureBox_ZoomOut)).EndInit();
            ((ISupportInitialize)(this.pictureBox_ZoomIn)).EndInit();
            ((ISupportInitialize)(this.pictureBoxRadarSettings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Timer timer1;
        private PictureBox pictureBoxMain;
        private CheckBoxExt checkBoxFriends;
        private CheckBoxExt checkBoxEnemies;
        private CheckBoxExt checkBoxObjects;
        private PictureBox pictureBox2;
        private CheckBoxExt checkBoxNpcs;
        private PictureBoxExt pictureBox_ZoomOut;
        private PictureBoxExt pictureBox_ZoomIn;
        private PictureBoxExt pictureBoxRadarSettings;
        private MetroToolTip toolTip1;
        private CheckBox checkBoxCorpses;
        private TextBox textBoxDetailedInfo;
    }
}