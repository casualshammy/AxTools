using System.ComponentModel;
using System.Windows.Forms;
using AxTools.Components;
using ICSharpCode.TextEditor;

namespace AxTools.Forms
{
    internal partial class LuaConsole
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

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBoxLuaCode = new ICSharpCode.TextEditor.TextEditorControl();
            this.buttonDump = new System.Windows.Forms.Button();
            this.metroCheckBoxIgnoreGameState = new MetroFramework.Controls.MetroCheckBox();
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager();
            this.metroCheckBoxRandomize = new MetroFramework.Controls.MetroCheckBox();
            this.metroPanelTimerOptions = new MetroFramework.Controls.MetroPanel();
            this.metroCheckBoxShowIngameNotifications = new MetroFramework.Controls.MetroCheckBox();
            this.metroTextBoxTimerInterval = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.pictureBoxOpenLuaFile = new AxTools.Components.PictureBoxExt(this.components);
            this.pictureBoxSaveLuaFile = new AxTools.Components.PictureBoxExt(this.components);
            this.metroToolTip1 = new MetroFramework.Components.MetroToolTip();
            this.pictureBoxRunOnce = new AxTools.Components.PictureBoxExt(this.components);
            this.pictureBoxRunLoop = new AxTools.Components.PictureBoxExt(this.components);
            this.pictureBoxStop = new AxTools.Components.PictureBoxExt(this.components);
            this.pictureBoxSettings = new AxTools.Components.PictureBoxExt(this.components);
            this.labelRequestTime = new MetroFramework.Controls.MetroLabel();
            this.metroPanelTimerOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOpenLuaFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSaveLuaFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRunOnce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRunLoop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxLuaCode
            // 
            this.textBoxLuaCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLuaCode.AutoScroll = true;
            this.textBoxLuaCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxLuaCode.IsReadOnly = false;
            this.textBoxLuaCode.Location = new System.Drawing.Point(20, 44);
            this.textBoxLuaCode.Name = "textBoxLuaCode";
            this.textBoxLuaCode.ShowVRuler = false;
            this.textBoxLuaCode.Size = new System.Drawing.Size(610, 290);
            this.textBoxLuaCode.TabIndex = 4;
            // 
            // buttonDump
            // 
            this.buttonDump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDump.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonDump.FlatAppearance.BorderSize = 0;
            this.buttonDump.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDump.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonDump.Location = new System.Drawing.Point(23, 340);
            this.buttonDump.Name = "buttonDump";
            this.buttonDump.Size = new System.Drawing.Size(10, 10);
            this.buttonDump.TabIndex = 39;
            this.buttonDump.UseVisualStyleBackColor = false;
            this.buttonDump.Click += new System.EventHandler(this.ButtonDumpClick);
            // 
            // metroCheckBoxIgnoreGameState
            // 
            this.metroCheckBoxIgnoreGameState.AutoSize = true;
            this.metroCheckBoxIgnoreGameState.CustomBackground = false;
            this.metroCheckBoxIgnoreGameState.CustomForeColor = false;
            this.metroCheckBoxIgnoreGameState.FontSize = MetroFramework.MetroLinkSize.Small;
            this.metroCheckBoxIgnoreGameState.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.metroCheckBoxIgnoreGameState.Location = new System.Drawing.Point(3, 24);
            this.metroCheckBoxIgnoreGameState.Name = "metroCheckBoxIgnoreGameState";
            this.metroCheckBoxIgnoreGameState.Size = new System.Drawing.Size(209, 15);
            this.metroCheckBoxIgnoreGameState.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroCheckBoxIgnoreGameState.StyleManager = this.metroStyleManager1;
            this.metroCheckBoxIgnoreGameState.TabIndex = 49;
            this.metroCheckBoxIgnoreGameState.Text = "Ignore game state (offline, loading)";
            this.metroCheckBoxIgnoreGameState.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroCheckBoxIgnoreGameState.UseStyleColors = true;
            this.metroCheckBoxIgnoreGameState.UseVisualStyleBackColor = true;
            this.metroCheckBoxIgnoreGameState.CheckedChanged += new System.EventHandler(this.metroCheckBoxIgnoreGameState_CheckedChanged);
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.OwnerForm = this;
            this.metroStyleManager1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // metroCheckBoxRandomize
            // 
            this.metroCheckBoxRandomize.AutoSize = true;
            this.metroCheckBoxRandomize.CustomBackground = false;
            this.metroCheckBoxRandomize.CustomForeColor = false;
            this.metroCheckBoxRandomize.FontSize = MetroFramework.MetroLinkSize.Small;
            this.metroCheckBoxRandomize.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.metroCheckBoxRandomize.Location = new System.Drawing.Point(3, 3);
            this.metroCheckBoxRandomize.Name = "metroCheckBoxRandomize";
            this.metroCheckBoxRandomize.Size = new System.Drawing.Size(165, 15);
            this.metroCheckBoxRandomize.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroCheckBoxRandomize.StyleManager = this.metroStyleManager1;
            this.metroCheckBoxRandomize.TabIndex = 52;
            this.metroCheckBoxRandomize.Text = "Randomize interval (±20%)";
            this.metroCheckBoxRandomize.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroCheckBoxRandomize.UseStyleColors = true;
            this.metroCheckBoxRandomize.UseVisualStyleBackColor = true;
            this.metroCheckBoxRandomize.CheckedChanged += new System.EventHandler(this.metroCheckBoxRandomize_CheckedChanged);
            // 
            // metroPanelTimerOptions
            // 
            this.metroPanelTimerOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.metroPanelTimerOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.metroPanelTimerOptions.Controls.Add(this.metroCheckBoxShowIngameNotifications);
            this.metroPanelTimerOptions.Controls.Add(this.metroTextBoxTimerInterval);
            this.metroPanelTimerOptions.Controls.Add(this.metroLabel1);
            this.metroPanelTimerOptions.Controls.Add(this.metroCheckBoxRandomize);
            this.metroPanelTimerOptions.Controls.Add(this.metroCheckBoxIgnoreGameState);
            this.metroPanelTimerOptions.CustomBackground = false;
            this.metroPanelTimerOptions.HorizontalScrollbar = false;
            this.metroPanelTimerOptions.HorizontalScrollbarBarColor = true;
            this.metroPanelTimerOptions.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanelTimerOptions.HorizontalScrollbarSize = 10;
            this.metroPanelTimerOptions.Location = new System.Drawing.Point(389, 44);
            this.metroPanelTimerOptions.Name = "metroPanelTimerOptions";
            this.metroPanelTimerOptions.Size = new System.Drawing.Size(220, 96);
            this.metroPanelTimerOptions.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroPanelTimerOptions.StyleManager = this.metroStyleManager1;
            this.metroPanelTimerOptions.TabIndex = 53;
            this.metroPanelTimerOptions.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroPanelTimerOptions.VerticalScrollbar = false;
            this.metroPanelTimerOptions.VerticalScrollbarBarColor = true;
            this.metroPanelTimerOptions.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanelTimerOptions.VerticalScrollbarSize = 10;
            // 
            // metroCheckBoxShowIngameNotifications
            // 
            this.metroCheckBoxShowIngameNotifications.AutoSize = true;
            this.metroCheckBoxShowIngameNotifications.CustomBackground = false;
            this.metroCheckBoxShowIngameNotifications.CustomForeColor = false;
            this.metroCheckBoxShowIngameNotifications.FontSize = MetroFramework.MetroLinkSize.Small;
            this.metroCheckBoxShowIngameNotifications.FontWeight = MetroFramework.MetroLinkWeight.Regular;
            this.metroCheckBoxShowIngameNotifications.Location = new System.Drawing.Point(3, 45);
            this.metroCheckBoxShowIngameNotifications.Name = "metroCheckBoxShowIngameNotifications";
            this.metroCheckBoxShowIngameNotifications.Size = new System.Drawing.Size(164, 15);
            this.metroCheckBoxShowIngameNotifications.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroCheckBoxShowIngameNotifications.StyleManager = this.metroStyleManager1;
            this.metroCheckBoxShowIngameNotifications.TabIndex = 55;
            this.metroCheckBoxShowIngameNotifications.Text = "Show ingame notifications";
            this.metroCheckBoxShowIngameNotifications.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroToolTip1.SetToolTip(this.metroCheckBoxShowIngameNotifications, "Show various plugins notifications in UIError channel");
            this.metroCheckBoxShowIngameNotifications.UseStyleColors = true;
            this.metroCheckBoxShowIngameNotifications.UseVisualStyleBackColor = true;
            this.metroCheckBoxShowIngameNotifications.CheckedChanged += new System.EventHandler(this.metroCheckBoxShowIngameNotifications_CheckedChanged);
            // 
            // metroTextBoxTimerInterval
            // 
            this.metroTextBoxTimerInterval.CustomBackground = false;
            this.metroTextBoxTimerInterval.CustomForeColor = false;
            this.metroTextBoxTimerInterval.FontSize = MetroFramework.MetroTextBoxSize.Small;
            this.metroTextBoxTimerInterval.FontWeight = MetroFramework.MetroTextBoxWeight.Regular;
            this.metroTextBoxTimerInterval.Location = new System.Drawing.Point(55, 68);
            this.metroTextBoxTimerInterval.Multiline = false;
            this.metroTextBoxTimerInterval.Name = "metroTextBoxTimerInterval";
            this.metroTextBoxTimerInterval.SelectedText = "";
            this.metroTextBoxTimerInterval.Size = new System.Drawing.Size(89, 23);
            this.metroTextBoxTimerInterval.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBoxTimerInterval.StyleManager = this.metroStyleManager1;
            this.metroTextBoxTimerInterval.TabIndex = 54;
            this.metroTextBoxTimerInterval.Text = "1000";
            this.metroTextBoxTimerInterval.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBoxTimerInterval.UseStyleColors = true;
            this.metroTextBoxTimerInterval.TextChanged += new System.EventHandler(this.metroTextBoxTimerInterval_TextChanged);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.CustomBackground = false;
            this.metroLabel1.CustomForeColor = false;
            this.metroLabel1.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel1.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel1.Location = new System.Drawing.Point(3, 72);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(46, 15);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel1.StyleManager = this.metroStyleManager1;
            this.metroLabel1.TabIndex = 53;
            this.metroLabel1.Text = "Interval:";
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel1.UseStyleColors = true;
            // 
            // pictureBoxOpenLuaFile
            // 
            this.pictureBoxOpenLuaFile.Image = global::AxTools.Properties.Resources.document_open;
            this.pictureBoxOpenLuaFile.ImageOnHover = global::AxTools.Properties.Resources.DocumentOpenLight;
            this.pictureBoxOpenLuaFile.Location = new System.Drawing.Point(20, 16);
            this.pictureBoxOpenLuaFile.Name = "pictureBoxOpenLuaFile";
            this.pictureBoxOpenLuaFile.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxOpenLuaFile.TabIndex = 55;
            this.pictureBoxOpenLuaFile.TabStop = false;
            this.metroToolTip1.SetToolTip(this.pictureBoxOpenLuaFile, "Open .lua file from disk");
            this.pictureBoxOpenLuaFile.Click += new System.EventHandler(this.PictureBoxOpenLuaFileClick);
            // 
            // pictureBoxSaveLuaFile
            // 
            this.pictureBoxSaveLuaFile.Image = global::AxTools.Properties.Resources.document_save;
            this.pictureBoxSaveLuaFile.ImageOnHover = global::AxTools.Properties.Resources.DocumentSaveLight;
            this.pictureBoxSaveLuaFile.Location = new System.Drawing.Point(48, 16);
            this.pictureBoxSaveLuaFile.Name = "pictureBoxSaveLuaFile";
            this.pictureBoxSaveLuaFile.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxSaveLuaFile.TabIndex = 56;
            this.pictureBoxSaveLuaFile.TabStop = false;
            this.metroToolTip1.SetToolTip(this.pictureBoxSaveLuaFile, "Save .lua file to disk");
            this.pictureBoxSaveLuaFile.Click += new System.EventHandler(this.PictureBoxSaveLuaFileClick);
            // 
            // metroToolTip1
            // 
            this.metroToolTip1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroToolTip1.StyleManager = null;
            this.metroToolTip1.Theme = MetroFramework.MetroThemeStyle.Light;
            // 
            // pictureBoxRunOnce
            // 
            this.pictureBoxRunOnce.Image = global::AxTools.Properties.Resources.yellow_play;
            this.pictureBoxRunOnce.ImageOnHover = global::AxTools.Properties.Resources.yellow_play_light;
            this.pictureBoxRunOnce.Location = new System.Drawing.Point(108, 16);
            this.pictureBoxRunOnce.Name = "pictureBoxRunOnce";
            this.pictureBoxRunOnce.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxRunOnce.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxRunOnce.TabIndex = 57;
            this.pictureBoxRunOnce.TabStop = false;
            this.metroToolTip1.SetToolTip(this.pictureBoxRunOnce, "Run script once");
            this.pictureBoxRunOnce.Click += new System.EventHandler(this.pictureBoxRunOnce_Click);
            // 
            // pictureBoxRunLoop
            // 
            this.pictureBoxRunLoop.Image = global::AxTools.Properties.Resources.yellow_forward;
            this.pictureBoxRunLoop.ImageOnHover = global::AxTools.Properties.Resources.yellow_forward_light;
            this.pictureBoxRunLoop.Location = new System.Drawing.Point(136, 16);
            this.pictureBoxRunLoop.Name = "pictureBoxRunLoop";
            this.pictureBoxRunLoop.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxRunLoop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxRunLoop.TabIndex = 58;
            this.pictureBoxRunLoop.TabStop = false;
            this.metroToolTip1.SetToolTip(this.pictureBoxRunLoop, "Run script periodically");
            this.pictureBoxRunLoop.Click += new System.EventHandler(this.pictureBoxRunLoop_Click);
            // 
            // pictureBoxStop
            // 
            this.pictureBoxStop.Enabled = false;
            this.pictureBoxStop.Image = global::AxTools.Properties.Resources.yellow_stop_grey;
            this.pictureBoxStop.ImageOnHover = global::AxTools.Properties.Resources.yellow_stop_light;
            this.pictureBoxStop.Location = new System.Drawing.Point(164, 16);
            this.pictureBoxStop.Name = "pictureBoxStop";
            this.pictureBoxStop.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxStop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxStop.TabIndex = 59;
            this.pictureBoxStop.TabStop = false;
            this.metroToolTip1.SetToolTip(this.pictureBoxStop, "Disable periodic execution");
            this.pictureBoxStop.Click += new System.EventHandler(this.pictureBoxStop_Click);
            // 
            // pictureBoxSettings
            // 
            this.pictureBoxSettings.Image = global::AxTools.Properties.Resources.yellow_record;
            this.pictureBoxSettings.ImageOnHover = global::AxTools.Properties.Resources.yellow_record_light;
            this.pictureBoxSettings.Location = new System.Drawing.Point(192, 16);
            this.pictureBoxSettings.Name = "pictureBoxSettings";
            this.pictureBoxSettings.Size = new System.Drawing.Size(22, 22);
            this.pictureBoxSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxSettings.TabIndex = 60;
            this.pictureBoxSettings.TabStop = false;
            this.metroToolTip1.SetToolTip(this.pictureBoxSettings, "Show/hide timer options");
            this.pictureBoxSettings.Click += new System.EventHandler(this.pictureBoxSettings_Click);
            // 
            // labelRequestTime
            // 
            this.labelRequestTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRequestTime.CustomBackground = false;
            this.labelRequestTime.CustomForeColor = false;
            this.labelRequestTime.FontSize = MetroFramework.MetroLabelSize.Small;
            this.labelRequestTime.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.labelRequestTime.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.labelRequestTime.Location = new System.Drawing.Point(389, 26);
            this.labelRequestTime.Name = "labelRequestTime";
            this.labelRequestTime.Size = new System.Drawing.Size(241, 15);
            this.labelRequestTime.Style = MetroFramework.MetroColorStyle.Blue;
            this.labelRequestTime.StyleManager = this.metroStyleManager1;
            this.labelRequestTime.TabIndex = 61;
            this.labelRequestTime.Text = "Script has taken {0}ms to complete";
            this.labelRequestTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelRequestTime.Theme = MetroFramework.MetroThemeStyle.Light;
            this.labelRequestTime.UseStyleColors = true;
            this.labelRequestTime.Visible = false;
            // 
            // LuaConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 354);
            this.Controls.Add(this.labelRequestTime);
            this.Controls.Add(this.pictureBoxSettings);
            this.Controls.Add(this.pictureBoxStop);
            this.Controls.Add(this.pictureBoxRunLoop);
            this.Controls.Add(this.pictureBoxRunOnce);
            this.Controls.Add(this.pictureBoxSaveLuaFile);
            this.Controls.Add(this.pictureBoxOpenLuaFile);
            this.Controls.Add(this.metroPanelTimerOptions);
            this.Controls.Add(this.buttonDump);
            this.Controls.Add(this.textBoxLuaCode);
            this.DisplayHeader = false;
            this.Location = new System.Drawing.Point(0, 0);
            this.MinimumSize = new System.Drawing.Size(650, 354);
            this.Name = "LuaConsole";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.StyleManager = this.metroStyleManager1;
            this.Text = "Lua console";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WowModulesFormClosing);
            this.Resize += new System.EventHandler(this.LuaConsole_Resize);
            this.metroPanelTimerOptions.ResumeLayout(false);
            this.metroPanelTimerOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOpenLuaFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSaveLuaFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRunOnce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRunLoop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSettings)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TextEditorControl textBoxLuaCode;
        private Button buttonDump;
        private MetroFramework.Controls.MetroCheckBox metroCheckBoxIgnoreGameState;
        private MetroFramework.Controls.MetroCheckBox metroCheckBoxRandomize;
        private MetroFramework.Controls.MetroPanel metroPanelTimerOptions;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroTextBox metroTextBoxTimerInterval;
        private MetroFramework.Components.MetroStyleManager metroStyleManager1;
        private PictureBoxExt pictureBoxOpenLuaFile;
        private PictureBoxExt pictureBoxSaveLuaFile;
        private MetroFramework.Controls.MetroCheckBox metroCheckBoxShowIngameNotifications;
        private MetroFramework.Components.MetroToolTip metroToolTip1;
        private PictureBoxExt pictureBoxStop;
        private PictureBoxExt pictureBoxRunLoop;
        private PictureBoxExt pictureBoxRunOnce;
        private PictureBoxExt pictureBoxSettings;
        private MetroFramework.Controls.MetroLabel labelRequestTime;
    }
}

