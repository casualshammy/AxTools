using System.Windows.Forms;
using AxTools.Components;

namespace AxTools.Forms
{
    internal partial class WowRadar
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxMain = new System.Windows.Forms.PictureBox();
            this.checkBoxFriends = new AxTools.Components.CheckBoxExt(this.components);
            this.checkBoxEnemies = new AxTools.Components.CheckBoxExt(this.components);
            this.checkBoxObjects = new AxTools.Components.CheckBoxExt(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.checkBoxNpcs = new AxTools.Components.CheckBoxExt(this.components);
            this.pictureBox_ZoomOut = new AxTools.Components.PictureBoxExt(this.components);
            this.pictureBox_ZoomIn = new AxTools.Components.PictureBoxExt(this.components);
            this.pictureBoxRadarSettings = new AxTools.Components.PictureBoxExt(this.components);
            this.toolTip1 = new MetroFramework.Components.MetroToolTip();
            this.checkBoxCorpses = new System.Windows.Forms.CheckBox();
            this.textBoxDetailedInfo = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ZoomOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ZoomIn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRadarSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
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
            // pictureBox2
            // 
            this.pictureBox2.Image = global::AxTools.Properties.Resources.close_41741;
            this.pictureBox2.Location = new System.Drawing.Point(205, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(20, 20);
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.PictureBox2Click);
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
            // pictureBox_ZoomOut
            // 
            this.pictureBox_ZoomOut.Image = global::AxTools.Properties.Resources.plus;
            this.pictureBox_ZoomOut.ImageOnHover = global::AxTools.Properties.Resources.PlusLight;
            this.pictureBox_ZoomOut.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_ZoomOut.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox_ZoomOut.Name = "pictureBox_ZoomOut";
            this.pictureBox_ZoomOut.Size = new System.Drawing.Size(16, 16);
            this.pictureBox_ZoomOut.TabIndex = 35;
            this.pictureBox_ZoomOut.TabStop = false;
            this.pictureBox_ZoomOut.Click += new System.EventHandler(this.PictureBoxZoomOutClick);
            // 
            // pictureBox_ZoomIn
            // 
            this.pictureBox_ZoomIn.Image = global::AxTools.Properties.Resources.minus;
            this.pictureBox_ZoomIn.ImageOnHover = global::AxTools.Properties.Resources.MinusLight;
            this.pictureBox_ZoomIn.Location = new System.Drawing.Point(16, 0);
            this.pictureBox_ZoomIn.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox_ZoomIn.Name = "pictureBox_ZoomIn";
            this.pictureBox_ZoomIn.Size = new System.Drawing.Size(16, 16);
            this.pictureBox_ZoomIn.TabIndex = 36;
            this.pictureBox_ZoomIn.TabStop = false;
            this.pictureBox_ZoomIn.Click += new System.EventHandler(this.PictureBoxZoomInClick);
            // 
            // pictureBoxRadarSettings
            // 
            this.pictureBoxRadarSettings.Image = global::AxTools.Properties.Resources.Settings20px;
            this.pictureBoxRadarSettings.ImageOnHover = global::AxTools.Properties.Resources.SettingsLight20px;
            this.pictureBoxRadarSettings.Location = new System.Drawing.Point(35, 0);
            this.pictureBoxRadarSettings.Name = "pictureBoxRadarSettings";
            this.pictureBoxRadarSettings.Size = new System.Drawing.Size(16, 16);
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
            // WowRadar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(225, 274);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ZoomOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ZoomIn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRadarSettings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private PictureBox pictureBoxMain;
        private CheckBoxExt checkBoxFriends;
        private CheckBoxExt checkBoxEnemies;
        private CheckBoxExt checkBoxObjects;
        private System.Windows.Forms.PictureBox pictureBox2;
        private CheckBoxExt checkBoxNpcs;
        private PictureBoxExt pictureBox_ZoomOut;
        private PictureBoxExt pictureBox_ZoomIn;
        private PictureBoxExt pictureBoxRadarSettings;
        private MetroFramework.Components.MetroToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBoxCorpses;
        private System.Windows.Forms.TextBox textBoxDetailedInfo;
    }
}