using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using Components.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace BlackMarketTracker
{
    internal partial class BlackMarketTracker : BorderedMetroForm, IPlugin3
    {
        #region IPlugin3 info

        public new string Name => "BlackMarketTracker";

        public bool ConfigAvailable => false;

        public string[] Dependencies => null;

        public string Description => "Shows exact remaining time of black market auctions";

        public bool DontCloseOnWowShutdown => true;

        public Image TrayIcon => null;

        public Version Version => new Version(1, 0);

        #endregion IPlugin3 info

        private BlackMarketTracker actualWindow;
        private readonly GameInterface game;
        private DateTime lastRefresh = DateTime.UtcNow;
        private readonly object imageListLocker = new object();
        private readonly List<BlackMarketItem> bmItems = new List<BlackMarketItem>();

        public BlackMarketTracker()
        {
        }

        public BlackMarketTracker(GameInterface gameInterface)
        {
            InitializeComponent();
            StyleManager.Style = Utilities.MetroColorStyle;
            game = gameInterface;
            this.LogPrint("Opened");
        }

        private void BlackMarketFormClosing(object sender, FormClosingEventArgs e)
        {
            timerUpdateList.Enabled = false;
            this.LogPrint("Closed");
        }

        private void MetroLinkRefreshClick(object sender, EventArgs e)
        {
            if (game.IsInGame)
            {
                lastRefresh = DateTime.UtcNow;
                listView1.Items.Clear();
                bmItems.Clear();
                timerUpdateList.Enabled = false;
                WaitingOverlay waitingOverlay = new WaitingOverlay(this, "Please wait...", Utilities.MetroColorStyle).Show();
                Task.Run(delegate
                {
                    foreach (BlackMarketItem item in BlackMarketMgr.GetAllItems(game))
                    {
                        bmItems.Add(item);
                        PostInvoke(delegate
                        {
                            if (!ImageListContainsKeySafe(item.Name))
                                ImageListAddSafe(item.Name, item.Image);
                            listView1.Items.Add(new ListViewItem(new[]
                            {
                                item.Name,
                                item.TimeLeft.ToString("hh\\:mm\\:ss"),
                                item.LastBidGold + " g",
                                item.NumBids.ToString()
                            })
                            { ImageKey = item.Name });
                        });
                    }
                }).ContinueWith(l =>
                {
                    PostInvoke(delegate
                    {
                        timerUpdateList.Enabled = true;
                        waitingOverlay.Close();
                    });
                });
            }
            else
            {
                TaskDialog.Show("Player isn't logged in", "AxTools", "If you sure it is, close this window and open BMTracker again", 0, TaskDialogIcon.Warning);
            }
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
            listView1.Items.Clear();
            foreach (BlackMarketItem item in bmItems)
            {
                TimeSpan ts = item.TimeLeft - (DateTime.UtcNow - lastRefresh);
                listView1.Items.Add(new ListViewItem(new[]
                {
                    item.Name,
                    ts.TotalSeconds <= 0 ? "Finished" : string.Format("{0} ({1})", ts.ToString("hh\\:mm\\:ss"), (DateTime.Now + ts).ToString("T")),
                    item.LastBidGold + " g",
                    item.NumBids.ToString()
                })
                { ImageKey = item.Name });
            }
        }

        #region IPlugin3 methods

        public void OnConfig()
        {
            throw new InvalidOperationException();
        }

        public void OnStart(GameInterface game)
        {
            Utilities.InvokeInGUIThread(delegate {
                (actualWindow = new BlackMarketTracker(game)).Show();
            });
        }

        public void OnStop()
        {
            Utilities.InvokeInGUIThread(delegate {
                actualWindow?.Close();
            });
        }

        #endregion IPlugin3 methods
    }
}