using AxTools.Helpers;
using KeyboardWatcher;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AxTools.Forms.Helpers
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class LuaConsoleSettings
    {

        private static readonly Log2 logger = new Log2("LuaConsoleSettings");
        private static readonly object _lock = new object();
        private static string settingsFile = AppFolders.ConfigDir + "\\lua-console.json";
        private static LuaConsoleSettings _instance;
        internal static LuaConsoleSettings Instance
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
                                    _instance = JsonConvert.DeserializeObject<LuaConsoleSettings>(rawText);
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
                                _instance = new LuaConsoleSettings();
                                logger.Info("Settings2 file is not found!");
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        private KeyExt timerHotkey = new KeyExt(Keys.None);
        internal Action<KeyExt> TimerHotkeyChanged;

        private LuaConsoleSettings() { }
        
        internal void SaveJSON()
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
        
        [JsonProperty(Order = 1, PropertyName = "WindowSize")]
        internal Size WindowSize = new Size(650, 354);

        [JsonProperty(Order = 2, PropertyName = "TimerRnd")]
        internal bool TimerRnd = true;

        [JsonProperty(Order = 3, PropertyName = "IgnoreGameState")]
        internal bool IgnoreGameState = false;

        [JsonProperty(Order = 4, PropertyName = "TimerInterval")]
        internal int TimerInterval = 1000;
        
        [JsonProperty(Order = 5, PropertyName = "TimerHotkey")]
        internal KeyExt TimerHotkey
        {
            get
            {
                return timerHotkey;
            }
            set
            {
                timerHotkey = value;
                TimerHotkeyChanged?.Invoke(timerHotkey);
            }
        }

        [JsonProperty(Order = 6, PropertyName = "Code")]
        internal string[] Code = new string[] { };

    }
}
