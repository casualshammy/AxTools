using System.Threading.Tasks;
using AxTools.Classes;
using AxTools.Classes.WoW;
using AxTools.Components;
using AxTools.Properties;
using MetroFramework.Drawing;
using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class BlackMarket : BorderedMetroForm
    {
        internal BlackMarket()
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            metroStyleManager1.Style = Settings.NewStyleColor;
            timerUpdateList.Enabled = true;
            Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Opened", WoW.WProc.ProcessName, WoW.WProc.ProcessID));
        }
        
        private DateTime lastRefresh = DateTime.UtcNow;
        private readonly unsafe int bmStructSize = sizeof(BlackMarketItem);

        private void BlackMarketFormClosing(object sender, FormClosingEventArgs e)
        {
            timerUpdateList.Enabled = false;
            Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Closed", WoW.WProc.ProcessName, WoW.WProc.ProcessID));
        }

        private void MetroLinkRefreshClick(object sender, EventArgs e)
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
                int baseAddr = WoW.WProc.Memory.Read<int>(WoW.WProc.Memory.ImageBase + WowBuildInfo.BlackMarketItems);
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        for (uint i = 0; i < numItems; ++i)
                        {
                            int finalAddr = (int) (baseAddr + i*bmStructSize);
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
                            Invoke(new Action(() => listView1.Items.Add(lvi)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Refresh error: {2}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, ex.Message), true);
                    }
                    finally
                    {
                        Invoke(new Action(waitingOverlay.Close));
                        Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Refresh time: {2}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, Environment.TickCount - startTime));
                    }
                });
            }
            else
            {
                Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Nothing to scan!", WoW.WProc.ProcessName, WoW.WProc.ProcessID));
                this.ShowTaskDialog("Items are zero", "Are you sure the black market window open?", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
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
