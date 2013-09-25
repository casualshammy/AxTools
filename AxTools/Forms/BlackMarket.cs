using AxTools.Classes;
using AxTools.Classes.WoW;
using AxTools.Properties;
using MetroFramework.Drawing;
using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class BlackMarket : MetroFramework.Forms.MetroForm
    {
        internal BlackMarket()
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            metroStyleManager1.Style = Settings.NewStyleColor;
            timerUpdateList.Enabled = true;
            Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Opened", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (SolidBrush styleBrush = MetroPaint.GetStyleBrush(Style))
            {
                Rectangle rectRight = new Rectangle(Width - 1, 0, 1, Height);
                e.Graphics.FillRectangle(styleBrush, rectRight);
                Rectangle rectLeft = new Rectangle(0, 0, 1, Height);
                e.Graphics.FillRectangle(styleBrush, rectLeft);
                Rectangle rectBottom = new Rectangle(0, Height - 1, Width, 1);
                e.Graphics.FillRectangle(styleBrush, rectBottom);
            }
        }

        private DateTime lastRefresh = DateTime.UtcNow;
        private readonly object lockListView = new object();

        private void BlackMarketFormClosing(object sender, FormClosingEventArgs e)
        {
            timerUpdateList.Enabled = false;
            Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Closed", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
        }

        private unsafe void MetroLinkRefreshClick(object sender, EventArgs e)
        {
            if (!WoW.Hooked || !WoW.WProc.IsInGame)
            {
                new TaskDialog("Error!", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            int startTime = Environment.TickCount;
            lastRefresh = DateTime.UtcNow;
            listView1.Items.Clear();
            uint numItems = WoW.WProc.Memory.Read<uint>(WoW.WProc.Memory.ImageBase + WowBuildInfo.BlackMarketNumItems);
            if (numItems != 0)
            {
                WaitingOverlay waitingOverlay = new WaitingOverlay(this);
                waitingOverlay.Show();
                int structSize = sizeof (BlackMarketItem);
                int baseAddr = WoW.WProc.Memory.Read<int>(WoW.WProc.Memory.ImageBase + WowBuildInfo.BlackMarketItems);
                for (uint i = 0; i < numItems; ++i)
                {
                    int finalAddr = (int)(baseAddr + i * structSize);
                    BlackMarketItem item = WoW.WProc.Memory.Read<BlackMarketItem>(new IntPtr(finalAddr));
                    uint gold = item.NumBids > 0 ? (uint) (item.currBid/10000) : (uint) (item.minBid/10000);
                    ListViewItem lvi = new ListViewItem(
                        new[]
                        {
                            WoW.GetFunctionReturn("GetItemInfo(" + item.Entry + ")"),
                            TimeSpan.FromSeconds(item.TimeLeft).ToString("hh\\:mm\\:ss"),
                            gold + " g",
                            item.NumBids.ToString()
                        }) {Tag = item};
                    lock (lockListView)
                    {
                        listView1.Items.Add(lvi);
                    }  
                }
                waitingOverlay.Close();
            }
            Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Refresh time: {2}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Environment.TickCount - startTime), false);
        }

        private void timerUpdateList_Tick(object sender, EventArgs e)
        {
            foreach (ListViewItem i in listView1.Items)
            {
                BlackMarketItem item = (BlackMarketItem) i.Tag;
                TimeSpan diff = DateTime.UtcNow - lastRefresh;
                TimeSpan ts = TimeSpan.FromSeconds(item.TimeLeft) - diff;
                i.SubItems[1].Text = ts.TotalSeconds <= 0 ? "Finished" : ts.ToString("hh\\:mm\\:ss");
            }
        }

    }
}
