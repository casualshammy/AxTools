using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AxTools.WoW.PluginSystem.Plugins
{
    internal class Fishing : IPlugin
    {

        #region Info

        public string Name
        {
            get { return "Fishing"; }
        }

        public Version Version
        {
            get { return new Version(1, 0); }
        }

        public string Author
        {
            get { return "CasualShammy"; }
        }

        public string Description
        {
            get { return "This is a very simple fish bot. It supports Nat's Hat and Weather-Beaten Fishing Hat if equipped"; }
        }

        public Image TrayIcon { get { return Resources.trade_fishing; } }

        public string WowIcon
        {
            get { return "Interface\\\\Icons\\\\trade_fishing"; }
        }

        public bool ConfigAvailable
        {
            get { return true; }
        }

        #endregion

        #region Events

        public void OnConfig()
        {
            FishingConfig.Open();
        }

        public void OnStart()
        {
            LoadSettings();
            state = 0;
            bobber = null;
            iterationStartTime = Environment.TickCount;
            lureSpecialBait = "if (not UnitBuff(\"player\", \"" + fishingSettings.SpecialBait + "\")) then " +
                              "if (GetItemCount(\"" + fishingSettings.SpecialBait + "\") > 0) then UseItemByName(\"" + fishingSettings.SpecialBait + "\") end end";
            timer = this.CreateTimer(100, OnPulse);
            timer.Start();
        }

        private void OnPulse()
        {
            if (Environment.TickCount - iterationStartTime > 25000)
            {
                state = 0;
                this.LogPrint("Timeout has expired");
            }
            switch (state)
            {
                case 0:
                    if (!WoWManager.WoWProcess.PlayerIsLooting)
                    {
                        this.LogPrint("Cast fishing...");
                        WoWDXInject.LuaDoString(castFishing);
                        Thread.Sleep(1500);
                        state = 1;
                        iterationStartTime = Environment.TickCount;
                    }
                    break;
                case 1:
                    WoWPlayerMe localPlayer;
                    try
                    {
                        localPlayer = ObjectMgr.Pulse(wowObjects);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("{0}:{1} :: [Fishing] Pulse error: {2}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ex.Message));
                        break;
                    }
                    if (localPlayer.ChannelSpellID == 0)
                    {
                        this.LogPrint("Player isn't fishing, recast...");
                        state = 0;
                        break;
                    }
                    bobber = wowObjects.FirstOrDefault(i => i.OwnerGUID == localPlayer.GUID);
                    if (bobber != null && bobber.Bobbing)
                    {
                        this.LogPrint("Got bit!");
                        Thread.Sleep(Utils.Rnd.Next(500, 1000));
                        state = 2;
                    }
                    break;
                case 2:
                    if (bobber != null)
                    {
                        this.LogPrint("Interacting...");
                        WoWDXInject.Interact(bobber.GUID);
                        bobber = null;
                        state = 3;
                    }
                    else
                    {
                        this.LogPrint("Bobber isn't found, recast...");
                        state = 0;
                    }
                    break;
                case 3:
                    if (WoWManager.WoWProcess.PlayerIsLooting)
                    {
                        state = 4;
                        this.LogPrint("Looting...");
                    }
                    break;
                case 4:
                    if (!WoWManager.WoWProcess.PlayerIsLooting)
                    {
                        // more random
                        if (Utils.Rnd.Next(0, 25) == 0)
                        {
                            int breakTime = Utils.Rnd.Next(15, 45);
                            this.LogPrint(string.Format("I'm human! Let's have a break ({0} sec)", breakTime));
                            Thread.Sleep(breakTime * 1000);
                        }
                        //
                        this.LogPrint("Looted, applying lure...");
                        if (fishingSettings.UseSpecialBait)
                        {
                            WoWDXInject.LuaDoString(lureSpecialBait);
                        }
                        if (fishingSettings.UseBestBait)
                        {
                            WoWDXInject.LuaDoString(lureBait);
                        }
                        Thread.Sleep(500);
                        state = 0;
                    }
                    break;
            }
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        #endregion

        #region Variables
        
        private readonly string lureBait = "if (not GetWeaponEnchantInfo()) then " +
                                           "if (GetItemCount(118391) > 0) then local name = GetItemInfo(118391); UseItemByName(name) " +                    // Королевский червяк
                                           "elseif (IsEquippedItem(88710) and GetInventoryItemCooldown(\"player\", 1) == 0) then UseInventoryItem(1) " +    // Шляпа Ната
                                           "elseif (GetItemCount(68049) > 0) then local name = GetItemInfo(68049); UseItemByName(name) " +                  // Термостойкая вращающаяся наживка
                                           "elseif (GetItemCount(34861) > 0) then local name = GetItemInfo(34861); UseItemByName(name) " +                  // Заостренный рыболовный крючок
                                           "elseif (IsEquippedItem(33820) and GetInventoryItemCooldown(\"player\", 1) == 0) then UseInventoryItem(1) " +    // Видавшая виды рыболовная шапка
                                           "elseif (GetItemCount(6529) > 0) then local name = GetItemInfo(6529); UseItemByName(name) " +                    // Блесна
                                           " end" +
                                           " end";

        private readonly string castFishing = "if (not UnitAffectingCombat(\"player\")) then CastSpellByName(\"Рыбная ловля\") end";
        private WowObject bobber;
        private readonly List<WowObject> wowObjects = new List<WowObject>();
        private int iterationStartTime;
        // ReSharper disable once InconsistentNaming
        internal FishingSettings fishingSettings;
        private string lureSpecialBait;
        private int state;
        private SingleThreadTimer timer;

        #endregion

        #region Settings

        [JsonObject(MemberSerialization.OptIn)]
        internal struct FishingSettings
        {
            [JsonProperty(PropertyName = "UseBestBait")]
            internal bool UseBestBait;

            [JsonProperty(PropertyName = "UseSpecialBait")]
            internal bool UseSpecialBait;

            [JsonProperty(PropertyName = "SpecialBait")]
            internal string SpecialBait;
        }

        internal void SaveSettings()
        {
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    JsonSerializer js = new JsonSerializer();
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 4;
                    js.Serialize(jsonWriter, fishingSettings);
                }
            }
            string json = sb.ToString();
            AppSpecUtils.CheckCreateDir();
            string mySettingsDir = Globals.PluginsSettingsPath + "\\Fishing";
            if (!Directory.Exists(mySettingsDir))
            {
                Directory.CreateDirectory(mySettingsDir);
            }
            File.WriteAllText(mySettingsDir + "\\FishingSettings.json", json, Encoding.UTF8);
        }

        internal void LoadSettings()
        {
            string mySettingsDir = Globals.PluginsSettingsPath + "\\Fishing";
            string mySettingsFile = mySettingsDir + "\\FishingSettings.json";
            if (File.Exists(mySettingsFile))
            {
                string rawText = File.ReadAllText(mySettingsFile, Encoding.UTF8);
                fishingSettings = JsonConvert.DeserializeObject<FishingSettings>(rawText);
            }
        }

        #endregion

    }
}
