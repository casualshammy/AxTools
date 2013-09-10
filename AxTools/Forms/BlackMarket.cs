using AxTools.Classes;
using AxTools.Classes.WoW;
using AxTools.Properties;
using MetroFramework.Drawing;
using System;
using System.Drawing;
using System.Threading.Tasks;
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
            Task.Factory.StartNew(UpdateCountownThread, TaskCreationOptions.LongRunning);
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

        private void BlackMarketFormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Closed", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
        }

        private DateTime mLastRefresh = DateTime.Now;
        private readonly object lockListView = new object();

        private void UpdateCountownThread()
        {
            while (!IsDisposed)
            {
                if (!IsHandleCreated || !WoW.Hooked || !WoW.WProc.IsInGame)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }
                Invoke(new Action(() =>
                    {
                        foreach (ListViewItem itm in listView1.Items)
                        {
                            BlackMarketItem bmi = (BlackMarketItem) itm.Tag;
                            TimeSpan diff = DateTime.Now - mLastRefresh;
                            TimeSpan ts = TimeSpan.FromSeconds(bmi.TimeLeft) - diff;
                            itm.SubItems[1].Text = ts.TotalSeconds <= 0 ? "Finished" : ts.ToString("hh\\:mm\\:ss");
                        }
                    }));
                System.Threading.Thread.Sleep(1000);
            }
            Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Update thread was finished", WoW.WProc.ProcessName, WoW.WProc.ProcessID), false);
        }

        private unsafe void MetroLinkRefreshClick(object sender, EventArgs e)
        {
            if (!WoW.Hooked || !WoW.WProc.IsInGame)
            {
                new TaskDialog("Error!", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            int startTime = Environment.TickCount;
            mLastRefresh = DateTime.Now;
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
                    float gold = item.NumBids > 0 ? (uint) (item.currBid/10000) : (uint) (item.minBid/10000);
                    //WoW.LuaDoString("AxTools_BMData = GetItemInfo(" + item.Entry.ToString() + ")");
                    ListViewItem lvi = new ListViewItem(
                        new[] {
                            //WoW.GetLocalizedText("AxTools_BMData"),
                            WoW.GetFunctionReturn("GetItemInfo(" + item.Entry.ToString() + ")"),
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
            Log.Print(
                string.Format("{0}:{1} :: [BlackMarket tracker] Refresh time: {2}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Environment.TickCount - startTime),
                false);
        }

    }
}
