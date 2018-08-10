using AxTools.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AxTools.Forms.Helpers
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class WoWRadarSettings
    {
        private static readonly Log2 logger = new Log2("WoWRadarSettings");
        private static readonly object _lock = new object();
        private static string settingsFile = AppFolders.ConfigDir + "\\wow-radar.json";

        private static WoWRadarSettings _instance;

        internal static WoWRadarSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            if (File.Exists(settingsFile))
                            {
                                try
                                {
                                    string rawText = File.ReadAllText(settingsFile, Encoding.UTF8);
                                    _instance = JsonConvert.DeserializeObject<WoWRadarSettings>(rawText);
                                    logger.Info("Settings2 file is loaded");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Cannot load settings file: " + ex.Message);
                                    return null;
                                }
                            }
                            else
                            {
                                _instance = new WoWRadarSettings();
                                logger.Info("Settings2 file is not found!");
                            }
                            Program.Exit += () => _instance.SaveJSON();
                        }
                    }
                }
                return _instance;
            }
        }

        private WoWRadarSettings()
        {
        }

        private void SaveJSON()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    JsonSerializer js = new JsonSerializer();
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 4;
                    js.Serialize(jsonWriter, _instance);
                }
            }
            string json = sb.ToString();
            File.WriteAllText(settingsFile, json, Encoding.UTF8);
            logger.Info("Settings2 file has been updated, time: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        [JsonProperty(Order = 0, PropertyName = "Location")]
        internal Point Location = new Point(100, 100);

        [JsonProperty(Order = 1, PropertyName = "ShowPlayersClasses")]
        internal bool ShowPlayersClasses = true;

        [JsonProperty(Order = 2, PropertyName = "ShowNPCsNames")]
        internal bool ShowNPCsNames = true;

        [JsonProperty(Order = 3, PropertyName = "ShowObjectsNames")]
        internal bool ShowObjectsNames = true;

        [JsonProperty(Order = 4, PropertyName = "DisplayCorpses")]
        internal bool DisplayCorpses = true;

        [JsonProperty(Order = 5, PropertyName = "DisplayEnemies")]
        internal bool DisplayEnemies = true;

        [JsonProperty(Order = 6, PropertyName = "DisplayFriends")]
        internal bool DisplayFriends = true;

        [JsonProperty(Order = 7, PropertyName = "DisplayNpcs")]
        internal bool DisplayNpcs = true;

        [JsonProperty(Order = 8, PropertyName = "DisplayObjects")]
        internal bool DisplayObjects = true;

        [JsonProperty(Order = 9, PropertyName = "Zoom")]
        internal float Zoom = 0.5f;

        [JsonProperty(Order = 10, PropertyName = "FriendColor")]
        internal Color FriendColor = Color.Green;

        [JsonProperty(Order = 11, PropertyName = "EnemyColor")]
        internal Color EnemyColor = Color.Red;

        [JsonProperty(Order = 12, PropertyName = "NPCColor")]
        internal Color NPCColor = Color.GreenYellow;

        [JsonProperty(Order = 13, PropertyName = "ObjectColor")]
        internal Color ObjectColor = Color.Gold;

        [JsonProperty(Order = 14, PropertyName = "AlarmSoundFile")]
        internal string AlarmSoundFile = AppFolders.ResourcesDir + "\\alarm.wav";

        [JsonProperty(Order = 15, PropertyName = "ShowLocalPlayerRotationArrowOnTop")]
        internal bool ShowLocalPlayerRotationArrowOnTop = false;

        private readonly ObservableCollection<RadarObject> list = new ObservableCollection<RadarObject>();

        [JsonProperty(Order = 50, PropertyName = "WoWRadarList")]
        internal ObservableCollection<RadarObject> List
        {
            get
            {
                return list;
            }
            set
            {
                foreach (RadarObject o in value)
                {
                    list.Add(o);
                }
            }
        }
    }
}