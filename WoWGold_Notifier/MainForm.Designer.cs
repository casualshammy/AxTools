using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WoWGold_Notifier
{
    partial class MainForm
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.buttonGoToSite = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelThreads = new System.Windows.Forms.Label();
            this.buttonLog = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.labelPerformance = new System.Windows.Forms.Label();
            this.labelResponse = new System.Windows.Forms.Label();
            this.checkBoxTimerEnabled = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(1458, 320);
            this.webBrowser1.TabIndex = 0;
            // 
            // buttonGoToSite
            // 
            this.buttonGoToSite.Location = new System.Drawing.Point(1358, 12);
            this.buttonGoToSite.Name = "buttonGoToSite";
            this.buttonGoToSite.Size = new System.Drawing.Size(75, 23);
            this.buttonGoToSite.TabIndex = 1;
            this.buttonGoToSite.Text = "Go to site";
            this.buttonGoToSite.UseVisualStyleBackColor = true;
            this.buttonGoToSite.Click += new System.EventHandler(this.buttonGoToSite_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(993, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Last updated: 00:00:00";
            // 
            // labelThreads
            // 
            this.labelThreads.AutoSize = true;
            this.labelThreads.Location = new System.Drawing.Point(1021, 17);
            this.labelThreads.Name = "labelThreads";
            this.labelThreads.Size = new System.Drawing.Size(58, 13);
            this.labelThreads.TabIndex = 3;
            this.labelThreads.Text = "Threads: 0";
            // 
            // buttonLog
            // 
            this.buttonLog.Location = new System.Drawing.Point(1277, 12);
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Size = new System.Drawing.Size(75, 23);
            this.buttonLog.TabIndex = 4;
            this.buttonLog.Text = "Show log";
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new System.EventHandler(this.buttonLog_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1139, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Refresh page";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelPerformance
            // 
            this.labelPerformance.AutoSize = true;
            this.labelPerformance.Location = new System.Drawing.Point(993, 30);
            this.labelPerformance.Name = "labelPerformance";
            this.labelPerformance.Size = new System.Drawing.Size(110, 13);
            this.labelPerformance.TabIndex = 6;
            this.labelPerformance.Text = "Performance: 1000ms";
            // 
            // labelResponse
            // 
            this.labelResponse.AutoSize = true;
            this.labelResponse.Location = new System.Drawing.Point(813, 4);
            this.labelResponse.Name = "labelResponse";
            this.labelResponse.Size = new System.Drawing.Size(79, 13);
            this.labelResponse.TabIndex = 7;
            this.labelResponse.Text = "Response: 420";
            // 
            // checkBoxTimerEnabled
            // 
            this.checkBoxTimerEnabled.AutoSize = true;
            this.checkBoxTimerEnabled.Checked = true;
            this.checkBoxTimerEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTimerEnabled.Location = new System.Drawing.Point(816, 20);
            this.checkBoxTimerEnabled.Name = "checkBoxTimerEnabled";
            this.checkBoxTimerEnabled.Size = new System.Drawing.Size(105, 17);
            this.checkBoxTimerEnabled.TabIndex = 8;
            this.checkBoxTimerEnabled.Text = "Timer Is Enabled";
            this.checkBoxTimerEnabled.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1458, 320);
            this.Controls.Add(this.checkBoxTimerEnabled);
            this.Controls.Add(this.labelResponse);
            this.Controls.Add(this.labelPerformance);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonLog);
            this.Controls.Add(this.labelThreads);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonGoToSite);
            this.Controls.Add(this.webBrowser1);
            this.Name = "MainForm";
            this.Text = "WoWGold.Ru";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WebBrowser webBrowser1;
        private Button buttonGoToSite;
        private Label label1;
        private Label labelThreads;
        private Button buttonLog;
        private Button button1;
        private Label labelPerformance;
        private Label labelResponse;
        private CheckBox checkBoxTimerEnabled;
    }
}

