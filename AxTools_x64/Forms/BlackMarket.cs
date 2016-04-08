using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Components;
using AxTools.Helpers;
using AxTools.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.WoW;
using AxTools.WoW.Helpers;
using AxTools.WoW.PluginSystem.API;
using Settings = AxTools.Helpers.Settings;

namespace AxTools.Forms
{
    internal partial class BlackMarket : BorderedMetroForm, IWoWModule
    {
        private DateTime lastRefresh = DateTime.UtcNow;
        private readonly unsafe int sizeofBmItem = sizeof(BlackMarketItem);
        private readonly object imageListLocker = new object();

        public BlackMarket()
        {
            InitializeComponent();
           StyleManager.Style = Settings.Instance.StyleColor;
            Icon = Resources.AppIcon;
            timerUpdateList.Enabled = true;
            Log.Info(string.Format("{0} [BlackMarket tracker] Opened", WoWManager.WoWProcess));
        }
        
        private void BlackMarketFormClosing(object sender, FormClosingEventArgs e)
        {
            timerUpdateList.Enabled = false;
            Log.Info(string.Format("{0} [BlackMarket tracker] Closed", WoWManager.WoWProcess));
        }

        private void MetroLinkRefreshClick(object sender, EventArgs e)
        {
            if (WoWManager.Hooked && GameFunctions.IsInGame)
            {
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
                                int finalAddr = (int) (baseAddr + (int) (i*sizeofBmItem));
                                BlackMarketItem item = WoWManager.WoWProcess.Memory.Read<BlackMarketItem>(new IntPtr(finalAddr));
                                uint gold = (uint) (item.currBid > 0 ? (uint) (item.currBid/10000) : item.NextBid/10000);
                                AddItemToListView(item, gold);
                            }
                            SetItemNamesAndImages();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("{0} [BlackMarket tracker] Refresh error: {1}", WoWManager.WoWProcess, ex.Message));
                            AppSpecUtils.NotifyUser("BM refresh error", ex.Message, NotifyUserType.Error, false);
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
                    AppSpecUtils.NotifyUser("Item count is null", "Are you sure the black market window is open?", NotifyUserType.Error, false);
                }
            }
            else
            {
                this.ShowTaskDialog("Player isn't logged in", "If you sure it is, close this window and open BMTracker again", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private void AddItemToListView(BlackMarketItem bmItem, uint gold)
        {
            ListViewItem lvi = new ListViewItem(new[]
            {
                //GameFunctions.Lua_GetFunctionReturn("GetItemInfo(" + item.Entry + ")"),
                "ItemID: " + bmItem.Entry,
                TimeSpan.FromSeconds(bmItem.TimeLeft).ToString("hh\\:mm\\:ss"),
                gold + " g",
                bmItem.NumBids.ToString()
            }) {Tag = bmItem};
            Invoke((MethodInvoker) delegate { listView1.Items.Add(lvi); });
        }

        private void SetItemNamesAndImages()
        {
            Parallel.ForEach(listView1.Items.Cast<ListViewItem>(), l =>
            {
                try
                {
                    BlackMarketItem bmItem = (BlackMarketItem) l.Tag;
                    string name = Wowhead.GetItemInfo(bmItem.Entry).Name;
                    InvokePost(() => { l.SubItems[0].Text = name; });
                    if (!ImageListContainsKeySafe(bmItem.Entry.ToString()))
                    {
                        ImageListAddSafe(bmItem.Entry.ToString(), Wowhead.GetItemInfo(bmItem.Entry).Image);
                    }
                    InvokePost(() => { l.ImageKey = bmItem.Entry.ToString(); });
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("{0} [BlackMarket tracker] SetItemNamesAndImages() error: {1}", WoWManager.WoWProcess, ex.Message));
                }
            });
        }

        private bool ImageListContainsKeySafe(string key)
        {
            lock (imageListLocker)
            {
                return imageListWowhead.Images.ContainsKey(key);
            }
        }

        private void ImageListAddSafe(string key, Image image)
        {
            lock (imageListLocker)
            {
                imageListWowhead.Images.Add(key, image);
            }
        }

        private void timerUpdateList_Tick(object sender, EventArgs e)
        {
            foreach (ListViewItem i in listView1.Items)
            {
                BlackMarketItem item = (BlackMarketItem) i.Tag;
                TimeSpan diff = DateTime.UtcNow - lastRefresh;
                TimeSpan ts = TimeSpan.FromSeconds(item.TimeLeft) - diff;
                i.SubItems[1].Text = ts.TotalSeconds <= 0 ? "Finished" : string.Format("{0} ({1})", ts.ToString("hh\\:mm\\:ss"), (DateTime.Now + ts).ToString("T"));
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 168)]
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

        }

    }
}
