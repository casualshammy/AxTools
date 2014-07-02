using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AxTools.Classes;
using AxTools.Components;
using AxTools.Properties;
using System;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.WoW;
using AxTools.WoW.Management;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class BlackMarket : BorderedMetroForm, IWoWModule
    {
        public BlackMarket()
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            metroStyleManager1.Style = Settings.NewStyleColor;
            timerUpdateList.Enabled = true;
            Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Opened", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
        }
        
        private DateTime lastRefresh = DateTime.UtcNow;
        private readonly unsafe int bmStructSize = sizeof(BlackMarketItem);

        private void BlackMarketFormClosing(object sender, FormClosingEventArgs e)
        {
            timerUpdateList.Enabled = false;
            Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Closed", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
        }

        private void MetroLinkRefreshClick(object sender, EventArgs e)
        {
            if (!WoWManager.Hooked || !WoWManager.WoWProcess.IsInGame)
            {
                this.ShowTaskDialog("Player isn't logged in", "If you sure it is, close this window and open BMTracker again", TaskDialogButton.OK, TaskDialogIcon.Stop);
                return;
            }
            int startTime = Environment.TickCount;
            lastRefresh = DateTime.UtcNow;
            listView1.Items.Clear();
            uint numItems = WoWManager.WoWProcess.Memory.Read<uint>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfo.BlackMarketNumItems);
            if (numItems != 0)
            {
                WaitingOverlay waitingOverlay = new WaitingOverlay(this);
                waitingOverlay.Show();
                int baseAddr = WoWManager.WoWProcess.Memory.Read<int>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfo.BlackMarketItems);
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        for (uint i = 0; i < numItems; ++i)
                        {
                            int finalAddr = (int) (baseAddr + i*bmStructSize);
                            BlackMarketItem item = WoWManager.WoWProcess.Memory.Read<BlackMarketItem>(new IntPtr(finalAddr));
                            uint gold = item.NumBids > 0 ? (uint) (item.currBid/10000) : (uint) (item.minBid/10000);
                            ListViewItem lvi = new ListViewItem(
                                new[]
                                {
                                    WoWDXInject.GetFunctionReturn("GetItemInfo(" + item.Entry + ")"),
                                    TimeSpan.FromSeconds(item.TimeLeft).ToString("hh\\:mm\\:ss"),
                                    gold + " g",
                                    item.NumBids.ToString()
                                }) {Tag = item};
                            Invoke((MethodInvoker) (() => listView1.Items.Add(lvi)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Refresh error: {2}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ex.Message), true);
                    }
                    finally
                    {
                        Invoke(new Action(waitingOverlay.Close));
                        Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Refresh time: {2}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Environment.TickCount - startTime));
                    }
                });
            }
            else
            {
                Log.Print(string.Format("{0}:{1} :: [BlackMarket tracker] Nothing to scan!", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
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

        [StructLayout(LayoutKind.Sequential)]
        private struct BlackMarketItem
        {
            private readonly uint MarketID;
            private readonly uint unk1;
            internal readonly uint Entry;
            private readonly uint unk2;
            private readonly uint Quantity;
            private readonly uint unk3;
            internal readonly ulong minBid;
            private readonly ulong minIncrement;
            internal readonly ulong currBid;
            internal readonly uint TimeLeft;
            [MarshalAs(UnmanagedType.Bool)] private readonly bool YouHaveHighBid;
            internal readonly uint NumBids;
        }

    }
}
