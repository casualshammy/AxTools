using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AxTools.Helpers;
using AxTools.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AxTools.WoW;
using AxTools.WoW.Helpers;
using AxTools.WoW.PluginSystem.API;
using Components.Forms;
using Settings2 = AxTools.Helpers.Settings2;

namespace AxTools.Forms
{
    internal partial class BlackMarket : BorderedMetroForm
    {
        private readonly WowProcess wowProcess;
        private readonly GameInterface game;
        private DateTime lastRefresh = DateTime.UtcNow;
        private readonly unsafe int sizeofBmItem = sizeof(BlackMarketItem);
        private readonly object imageListLocker = new object();

        public int ProcessID { get; set; }

        public BlackMarket(GameInterface gameInterface)
        {
            InitializeComponent();
            StyleManager.Style = Settings2.Instance.StyleColor;
            game = gameInterface;
            wowProcess = wow;
            ProcessID = wow.ProcessID;
            game = new GameInterface(wow);
            log = new Log2($"BlackMarket - {wow.ProcessID}");
            Icon = Resources.AppIcon;
            timerUpdateList.Enabled = true;
            log.Info("Opened");
        }
        
        private void BlackMarketFormClosing(object sender, FormClosingEventArgs e)
        {
            timerUpdateList.Enabled = false;
            log.Info("Closed");
        }

        private void MetroLinkRefreshClick(object sender, EventArgs e)
        {
            if (game.IsInGame)
            {
                int startTime = Environment.TickCount;
                lastRefresh = DateTime.UtcNow;
                listView1.Items.Clear();
                uint numItems = wowProcess.Memory.Read<uint>(wowProcess.Memory.ImageBase + WowBuildInfoX64.BlackMarketNumItems);
                if (numItems != 0)
                {
                    WaitingOverlay waitingOverlay = new WaitingOverlay(this, "Please wait...").Show();
                    IntPtr baseAddr = wowProcess.Memory.Read<IntPtr>(wowProcess.Memory.ImageBase + WowBuildInfoX64.BlackMarketItems);
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            for (uint i = 0; i < numItems; ++i)
                            {
                                int finalAddr = (int) (baseAddr + (int) (i*sizeofBmItem));
                                BlackMarketItem item = wowProcess.Memory.Read<BlackMarketItem>(new IntPtr(finalAddr));
                                uint gold = (uint) (item.currBid > 0 ? (uint) (item.currBid/10000) : item.NextBid/10000);
                                AddItemToListView(item, gold);
                            }
                            SetItemNamesAndImages();
                        }
                        catch (Exception ex)
                        {
                            log.Error(string.Format("Refresh error: {0}", ex.Message));
                            this.TaskDialog("BM refresh error", ex.Message, NotifyUserType.Error);
                        }
                        finally
                        {
                            Invoke(new Action(waitingOverlay.Close));
                            log.Info(string.Format("Refresh time: {0}", Environment.TickCount - startTime));
                        }
                    });
                }
                else
                {
                    log.Info("Nothing to scan!");
                    this.TaskDialog("BM Tracker: Item count is null", "Are you sure the black market window is open?", NotifyUserType.Error);
                }
            }
            else
            {
                this.TaskDialog("Player isn't logged in", "If you sure it is, close this window and open BMTracker again", NotifyUserType.Warn);
            }
        }

        private void AddItemToListView(BlackMarketItem bmItem, uint gold)
        {
            ListViewItem lvi = new ListViewItem(new[]
            {
                //game.Lua_GetFunctionReturn("GetItemInfo(" + item.Entry + ")"),
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
                    PostInvoke(() => { l.SubItems[0].Text = name; });
                    if (!ImageListContainsKeySafe(bmItem.Entry.ToString()))
                    {
                        ImageListAddSafe(bmItem.Entry.ToString(), Wowhead.GetItemInfo(bmItem.Entry).Image);
                    }
                    PostInvoke(() => { l.ImageKey = bmItem.Entry.ToString(); });
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("SetItemNamesAndImages() error: {0}", ex.Message));
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

        private void TimerUpdateList_Tick(object sender, EventArgs e)
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
