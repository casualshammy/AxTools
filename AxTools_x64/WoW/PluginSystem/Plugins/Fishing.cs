using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AxTools.Classes;
using AxTools.Properties;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;
using Newtonsoft.Json;

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

        public int Interval
        {
            get { return 100; }
        }

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
        }

        public void OnPulse()
        {
            if (Environment.TickCount - iterationStartTime > 25000)
            {
                state = 0;
                Log.Info(String.Format("{0}:{1} :: [{2}] Timeout has expired", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name));
            }
            switch (state)
            {
                case 0:
                    Log.Info(String.Format("{0}:{1} :: [{2}] Cast fishing...", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name));
                    WoWDXInject.LuaDoString(castFishing);
                    Thread.Sleep(1500);
                    state = 1;
                    iterationStartTime = Environment.TickCount;
                    break;
                case 1:
                    WoWPlayerMe localPlayer;
                    try
                    {
                        localPlayer = ObjectMgr.Pulse(wowObjects);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(String.Format("{0}:{1} :: [Fishing] Pulse error: {2}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ex.Message));
                        break;
                    }
                    if (localPlayer.ChannelSpellID == 0)
                    {
                        Log.Info(String.Format("{0}:{1} :: [{2}] Player isn't fishing, recast...", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name));
                        state = 0;
                        break;
                    }
                    bobber = wowObjects.FirstOrDefault(i => i.OwnerGUID == localPlayer.GUID);
                    if (bobber != null && bobber.Bobbing) //4456449
                    {
                        Log.Info(String.Format("{0}:{1} :: [{2}] Got bit!", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name));
                        Thread.Sleep(Utils.Rnd.Next(500, 1000));
                        state = 2;
                    }
                    break;
                case 2:
                    if (bobber != null)
                    {
                        Log.Info(String.Format("{0}:{1} :: [{2}] Interacting...", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name));
                        WoWDXInject.Interact(bobber.GUID);
                        bobber = null;
                        state = 3;
                    }
                    else
                    {
                        Log.Info(String.Format("{0}:{1} :: [{2}] Bobber isn't found, recast...", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name));
                        state = 0;
                    }
                    break;
                case 3:
                    if (WoWManager.WoWProcess.PlayerIsLooting)
                    {
                        state = 4;
                        Log.Info(String.Format("{0}:{1} :: [{2}] Looting...", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name));
                    }
                    break;
                case 4:
                    if (!WoWManager.WoWProcess.PlayerIsLooting)
                    {
                        // more random
                        if (Utils.Rnd.Next(0, 25) == 0)
                        {
                            int breakTime = Utils.Rnd.Next(15, 45);
                            Log.Info(string.Format("{0}:{1} :: [{2}] I'm human! Let's have a break ({3} sec)", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name, breakTime));
                            Thread.Sleep(breakTime * 1000);
                        }
                        //
                        Log.Info(String.Format("{0}:{1} :: [{2}] Looted, applying lure...", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, Name));
                        if (fishingSettings.UseBestBait)
                        {
                            WoWDXInject.LuaDoString(lureBait);
                        }
                        if (fishingSettings.UseSpecialBait)
                        {
                            WoWDXInject.LuaDoString(lureSpecialBait);
                        }
                        Thread.Sleep(500);
                        state = 0;
                    }
                    break;
            }
        }

        public void OnStop()
        {
            
        }

        #endregion

        #region Variables
        
        private int state;

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
            Utils.CheckCreateDir();
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
