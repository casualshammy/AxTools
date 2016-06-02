using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WoW.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AxTools.WoW.PluginSystem.API
{
    public static class Utilities
    {
        /// <summary>
        /// Makes a record to the log. WoW process name, process id and plugin name included
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="text"></param>
        public static void LogPrint(this IPlugin plugin, object text)
        {
            Log.Info(string.Format("{0} [Plugin: {1}] {2}", WoWManager.WoWProcess, plugin.Name, text));
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

        public static Task RequestPluginsStopAsync()
        {
            return Task.Factory.StartNew(PluginManagerEx.StopPlugins);
        }

        /// <summary>
        ///     Creates new <see cref="SafeTimer"/> timer.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static SafeTimer CreateTimer(this IPlugin plugin, int interval, Action action)
        {
            return new SafeTimer(interval, action) { PluginName = plugin.Name };
        }

        public static void SaveSettingsJSON<T>(this IPlugin plugin, T data, string path = null) where T : class
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
            AppFolders.CreatePluginsSettingsDir();
            string mySettingsDir = Globals.PluginsSettingsPath + "\\" + plugin.Name;
            if (!Directory.Exists(mySettingsDir))
            {
                Directory.CreateDirectory(mySettingsDir);
            }
            File.WriteAllText(path ?? mySettingsDir + "\\settings.json", sb.ToString(), Encoding.UTF8);
        }

        public static T LoadSettingsJSON<T>(this IPlugin plugin, string path = null) where T : class, new()
        {
            string mySettingsFile = path ?? string.Format("{0}\\{1}\\settings.json", Globals.PluginsSettingsPath, plugin.Name);
            if (File.Exists(mySettingsFile))
            {
                string rawText = File.ReadAllText(mySettingsFile, Encoding.UTF8);
                return JsonConvert.DeserializeObject<T>(rawText);
            }
            return new T();
        }

        public static string GetRandomString(int size)
        {
            return Utils.GetRandomString(size);
        }

        public static void RequestStartPlugin(string name)
        {
            IPlugin plugin = PluginManagerEx.LoadedPlugins.FirstOrDefault(l => l.Name == name);
            if (plugin != null)
            {
                IPlugin activePlugin = PluginManagerEx.RunningPlugins.FirstOrDefault(l => l.Name == name);
                if (activePlugin == null)
                {
                    PluginManagerEx.AddPluginToRunning(plugin);
                }
            }
        }

        public static void RequestStopPlugin(string name)
        {
            IPlugin plugin = PluginManagerEx.RunningPlugins.FirstOrDefault(l => l.Name == name);
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
        public static void ShowNotify(this IPlugin plugin, string text, bool warning, bool sound, int timeout = 7)
        {
            Notify.TrayPopup("[" + plugin.Name + "]", text, warning ? NotifyUserType.Warn : NotifyUserType.Info, sound, plugin.TrayIcon);
        }
    
    }
}
