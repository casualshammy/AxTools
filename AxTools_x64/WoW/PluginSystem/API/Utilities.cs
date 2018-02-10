using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxTools.Forms;
using AxTools.Helpers;
using AxTools.Services;
using AxTools.WoW.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AxTools.WoW.PluginSystem.API
{
    public static class Utilities
    {
        private static readonly Log2 log = new Log2("WoWAPI.Utilities");

        static Utilities()
        {
            WoWAntiKick.ActionEmulated += ptr =>
            {
                AntiAfkActionEmulated?.Invoke(ptr);
            };
        }
        
        /// <summary>
        /// Makes a record to the log. WoW process name, process id and plugin name are included
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="text"></param>
        public static void LogPrint(this IPlugin2 plugin, object text)
        {
            log.Info($"[Plugin: {plugin.Name}] {text}");
        }

        /// <summary>
        /// 
        /// </summary>
        public static IntPtr MainWindowHandle
        {
            get
            {
                return MainForm.Instance.Handle;
            }
        }
        
        /// <summary>
        ///     Creates new <see cref="SafeTimer"/> timer.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static SafeTimer CreateTimer(this IPlugin2 plugin, int interval, GameInterface game, Action action)
        {
            return new SafeTimer(interval, game, action) { PluginName = plugin.Name };
        }

        public static void SaveSettingsJSON<T>(this IPlugin2 plugin, T data, string path = null) where T : class
        {
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    JsonSerializer js = new JsonSerializer();
                    js.Converters.Add(new StringEnumConverter());
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 4;
                    js.Serialize(jsonWriter, data);
                }
            }
            string mySettingsDir = AppFolders.PluginsSettingsDir + "\\" + plugin.Name;
            if (!Directory.Exists(mySettingsDir))
            {
                Directory.CreateDirectory(mySettingsDir);
            }
            File.WriteAllText(path ?? mySettingsDir + "\\settings.json", sb.ToString(), Encoding.UTF8);
        }

        public static T LoadSettingsJSON<T>(this IPlugin2 plugin, string path = null) where T : class, new()
        {
            string mySettingsFile = path ?? string.Format("{0}\\{1}\\settings.json", AppFolders.PluginsSettingsDir, plugin.Name);
            if (File.Exists(mySettingsFile))
            {
                string rawText = File.ReadAllText(mySettingsFile, Encoding.UTF8);
                return JsonConvert.DeserializeObject<T>(rawText);
            }
            return new T();
        }

        public static T LoadJSON<T>(this IPlugin2 plugin, string data) where T : class
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string GetJSON<T>(this IPlugin2 plugin, T data) where T : class
        {
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    JsonSerializer js = new JsonSerializer();
                    js.Converters.Add(new StringEnumConverter());
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 4;
                    js.Serialize(jsonWriter, data);
                }
            }
            return sb.ToString();
        }

        public static string GetRandomString(int size, bool onlyLetters)
        {
            return Utils.GetRandomString(size, onlyLetters);
        }

        /// <summary>
        /// DO NOT USE INSIDE <see cref="IPlugin2.OnStop"/> or <see cref="IPlugin2.OnStart"/> METHODS!
        /// </summary>
        /// <param name="name"></param>
        public static void AddPluginToRunning(this IPlugin2 callerPlugin, string name)
        {
            IPlugin2 plugin = PluginManagerEx.LoadedPlugins.FirstOrDefault(l => l.Name == name);
            if (plugin != null)
            {
                IPlugin2 activePlugin = PluginManagerEx.RunningPlugins.FirstOrDefault(l => l.Name == name);
                if (activePlugin == null)
                {
                    WowProcess wow = WoWProcessManager.List.First(l => l.ProcessID == PluginManagerEx.PluginWoW[callerPlugin.Name]);
                    PluginManagerEx.AddPluginToRunning(plugin, wow);
                }
            }
        }

        /// <summary>
        /// DO NOT USE INSIDE <see cref="IPlugin2.OnStop"/> or <see cref="IPlugin2.OnStart"/> METHODS!
        /// </summary>
        /// <param name="name"></param>
        public static void RemovePluginFromRunning(string name)
        {
            IPlugin2 plugin = PluginManagerEx.RunningPlugins.FirstOrDefault(l => l.Name == name);
            if (plugin != null)
            {
                PluginManagerEx.RemovePluginFromRunning(plugin);
            }
        }

        public static dynamic GetReferenceOfPlugin(string pluginName)
        {
            return PluginManagerEx.LoadedPlugins.FirstOrDefault(l => l.Name == pluginName);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="text">Any text you want</param>
        /// <param name="warning">Is it warning or info</param>
        /// <param name="sound">Play sound</param>
        /// <param name="timeout">Time in seconds before popup disappears</param>
        public static void ShowNotify(this IPlugin2 plugin, string text, bool warning, bool sound, int timeout = 10, EventHandler onClick = null)
        {
            Notify.TrayPopup("[" + plugin.Name + "]", text, warning ? NotifyUserType.Warn : NotifyUserType.Info, sound, plugin.TrayIcon, timeout, onClick);
        }
        
        public static string GetPluginSettingsDir(this IPlugin2 plugin)
        {
            return $"{AppFolders.PluginsSettingsDir}\\{plugin.Name}";
        }

        public static IntPtr WoWWindowHandle(WowProcess wow)
        {
            return wow.MainWindowHandle;
        }

        public static event Action<IntPtr> AntiAfkActionEmulated;

    }
}
