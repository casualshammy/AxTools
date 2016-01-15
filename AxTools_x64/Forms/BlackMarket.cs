using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AxTools.Components;
using AxTools.Helpers;
using AxTools.Properties;
using System;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.WoW;
using AxTools.WoW.Management;
using AxTools.WoW.PluginSystem.API;
using Settings = AxTools.Helpers.Settings;

namespace AxTools.Forms
{
    internal partial class BlackMarket : BorderedMetroForm, IWoWModule
    {
        public BlackMarket()
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            metroStyleManager1.Style = Settings.Instance.StyleColor;
            timerUpdateList.Enabled = true;
            Log.Info(string.Format("{0} [BlackMarket tracker] Opened", WoWManager.WoWProcess));
        }
        
        private DateTime lastRefresh = DateTime.UtcNow;

        private void BlackMarketFormClosing(object sender, FormClosingEventArgs e)
        {
            timerUpdateList.Enabled = false;
            Log.Info(string.Format("{0} [BlackMarket tracker] Closed", WoWManager.WoWProcess));
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
            uint numItems = WoWManager.WoWProcess.Memory.Read<uint>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.BlackMarketNumItems);
            if (numItems != 0)
            {
                WaitingOverlay waitingOverlay = WaitingOverlay.Show(this);
                IntPtr baseAddr = WoWManager.WoWProcess.Memory.Read<IntPtr>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.BlackMarketItems);
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        for (uint i = 0; i < numItems; ++i)
                        {
                            unsafe
                            {
                                int finalAddr = (int)(baseAddr + (int)(i * sizeof(BlackMarketItem)));
                                BlackMarketItem item = WoWManager.WoWProcess.Memory.Read<BlackMarketItem>(new IntPtr(finalAddr));
                                uint gold = (uint)(item.currBid > 0 ? (uint)(item.currBid / 10000) : item.NextBid / 10000);
                                ListViewItem lvi = new ListViewItem(new[]
                                {
                                    GameFunctions.Lua_GetFunctionReturn("GetItemInfo(" + item.Entry + ")"),
                                    TimeSpan.FromSeconds(item.TimeLeft).ToString("hh\\:mm\\:ss"),
                                    gold + " g",
                                    item.NumBids.ToString()
                                }) { Tag = item };
                                Invoke((MethodInvoker)(() => listView1.Items.Add(lvi)));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("{0} [BlackMarket tracker] Refresh error: {1}", WoWManager.WoWProcess, ex.Message));
                    }
                    finally
                    {
                        Invoke(new Action(waitingOverlay.Close));
                        Log.Info(string.Format("{0} [BlackMarket tracker] Refresh time: {1}", WoWManager.WoWProcess, Environment.TickCount - startTime));
                    }
                });
            }
            else
            {
                Log.Info(string.Format("{0} [BlackMarket tracker] Nothing to scan!", WoWManager.WoWProcess));
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

        [StructLayout(LayoutKind.Explicit)]
        private struct BlackMarketItem
        {
            [FieldOffset(0x8)]
            public readonly uint Entry;

            [FieldOffset(0x80)]
            public readonly ulong NextBid;

            [FieldOffset(0x90)]
            public readonly ulong currBid;

            [FieldOffset(0x98)]
            public readonly uint TimeLeft;

            [MarshalAs(UnmanagedType.Bool)]
            [FieldOffset(0x9C)]
            private readonly bool YouHaveHighBid;

            [FieldOffset(0xA0)]
            public readonly uint NumBids;

            [FieldOffset(164)]
            private readonly uint Unknown_DoNotRemove;
        }

        //private class BlackMarketItemEx
        //{
        //    internal string Name;
        //    internal DateTime FinishTime;
        //    internal int Gold;
        //    internal int BidsNum;
        //}

    }
}
