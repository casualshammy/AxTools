using Components;

namespace BlackMarketTracker
{
    internal partial class BlackMarketTracker
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
            this.listView1 = new Components.ListViewDoubleBuffered();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.metroLinkRefresh = new MetroFramework.Controls.MetroLink();
            this.timerUpdateList = new System.Windows.Forms.Timer(this.components);
            this.imageListWowhead = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(20, 60);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(519, 243);
            this.listView1.SmallImageList = this.imageListWowhead;
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Item";
            this.columnHeader1.Width = 272;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Time Remaining";
            this.columnHeader2.Width = 105;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Current bid";
            this.columnHeader3.Width = 103;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Bids";
            this.columnHeader4.Width = 35;
            // 
            // metroLinkRefresh
            // 
            this.metroLinkRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.metroLinkRefresh.CustomBackground = false;
            this.metroLinkRefresh.CustomForeColor = false;
            this.metroLinkRefresh.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.metroLinkRefresh.FontWeight = MetroFramework.MetroLinkWeight.Bold;
            this.metroLinkRefresh.Location = new System.Drawing.Point(342, 31);
            this.metroLinkRefresh.Name = "metroLinkRefresh";
            this.metroLinkRefresh.Size = new System.Drawing.Size(115, 23);
            this.metroLinkRefresh.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLinkRefresh.TabIndex = 2;
            this.metroLinkRefresh.Text = ">> Refresh <<";
            this.metroLinkRefresh.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLinkRefresh.UseStyleColors = true;
            this.metroLinkRefresh.Click += new System.EventHandler(this.MetroLinkRefreshClick);
            // 
            // timerUpdateList
            // 
            this.timerUpdateList.Interval = 500;
            this.timerUpdateList.Tick += new System.EventHandler(this.TimerUpdateList_Tick);
            // 
            // imageListWowhead
            // 
            this.imageListWowhead.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListWowhead.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListWowhead.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // BlackMarket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 323);
            this.Controls.Add(this.metroLinkRefresh);
            this.Controls.Add(this.listView1);
            this.Location = new System.Drawing.Point(0, 0);
            this.Text = "BlackMarket tracker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BlackMarketFormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private ListViewDoubleBuffered listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private MetroFramework.Controls.MetroLink metroLinkRefresh;
        private System.Windows.Forms.Timer timerUpdateList;
        private System.Windows.Forms.ImageList imageListWowhead;
    }
}