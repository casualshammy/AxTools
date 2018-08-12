using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WoW.Helpers;
using MetroFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

        public static event Action<IntPtr> AntiAfkActionEmulated;

        public static MetroColorStyle MetroColorStyle => Settings2.Instance.StyleColor;
        public static Random Rnd = new Random(Environment.TickCount + 17);

        /// <summary>
        /// Makes a record to the log. WoW process name, process id and plugin name are included
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="text"></param>
        public static void LogPrint(this IPlugin3 plugin, object text)
        {
            log.Info($"[Plugin: {plugin.Name}] {text}");
        }

        public static void LogError(this IPlugin3 plugin, object text)
        {
            log.Error($"[Plugin: {plugin.Name}] {text}");
        }

        /// <summary>
        ///
        /// </summary>
        public static IntPtr MainWindowHandle => MainForm.Instance.Handle;

        /// <summary>
        ///     Creates new <see cref="SafeTimer"/> timer.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="interval"></param>
        /// <param name="game"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static SafeTimer CreateTimer(this IPlugin3 plugin, int interval, GameInterface game, Action action)
        {
            return new SafeTimer(interval, game, action) { PluginName = plugin.Name };
        }

        public static void SaveSettingsJSON<T>(this IPlugin3 plugin, T data, string path = null) where T : class
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

        public static T LoadSettingsJSON<T>(this IPlugin3 plugin, string path = null) where T : class, new()
        {
            string mySettingsFile = path ?? $"{AppFolders.PluginsSettingsDir}\\{plugin.Name}\\settings.json";
            if (File.Exists(mySettingsFile))
            {
                string rawText = File.ReadAllText(mySettingsFile, Encoding.UTF8);
                return JsonConvert.DeserializeObject<T>(rawText);
            }
            return new T();
        }

        public static T LoadJSON<T>(this IPlugin3 plugin, string data) where T : class
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string GetJSON<T>(this IPlugin3 plugin, T data) where T : class
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
        /// DO NOT USE INSIDE <see cref="IPlugin3.OnStop"/> or <see cref="IPlugin3.OnStart"/> METHODS!
        /// </summary>
        /// <param name="callerPlugin"></param>
        /// <param name="name"></param>
        public static void AddPluginToRunning(this IPlugin3 callerPlugin, string name)
        {
            IPlugin3 plugin = PluginManagerEx.LoadedPlugins.FirstOrDefault(l => l.Name == name);
            if (plugin != null)
            {
                IPlugin3 activePlugin = PluginManagerEx.RunningPlugins.FirstOrDefault(l => l.Name == name);
                if (activePlugin == null && WoWProcessManager.Processes.TryGetValue(PluginManagerEx.PluginWoW[callerPlugin.Name], out WowProcess wow))
                {
                    PluginManagerEx.AddPluginToRunning(plugin, wow);
                }
            }
        }

        /// <summary>
        /// DO NOT USE INSIDE <see cref="IPlugin3.OnStop"/> or <see cref="IPlugin3.OnStart"/> METHODS!
        /// </summary>
        /// <param name="name"></param>
        public static void RemovePluginFromRunning(string name)
        {
            IPlugin3 plugin = PluginManagerEx.RunningPlugins.FirstOrDefault(l => l.Name == name);
            if (plugin != null)
            {
                PluginManagerEx.RemovePluginFromRunning(plugin);
            }
        }

        public static dynamic GetReferenceOfPlugin(string pluginName)
        {
            return PluginManagerEx.LoadedPlugins.FirstOrDefault(l => l.Name == pluginName);
        }

        ///  <summary>
        /// 
        ///  </summary>
        ///  <param name="plugin"></param>
        ///  <param name="text">Any text you want</param>
        ///  <param name="warning">Is it warning or info</param>
        ///  <param name="sound">Play sound</param>
        ///  <param name="timeout">Time in seconds before popup disappears</param>
        /// <param name="onClick"></param>
        public static void ShowNotify(this IPlugin3 plugin, string text, bool warning, bool sound, int timeout = 10, EventHandler onClick = null)
        {
            Notify.TrayPopup("[" + plugin.Name + "]", text, warning ? NotifyUserType.Warn : NotifyUserType.Info, sound, plugin.TrayIcon, timeout, onClick);
        }

        public static string GetPluginSettingsDir(this IPlugin3 plugin)
        {
            return $"{AppFolders.PluginsSettingsDir}\\{plugin.Name}";
        }

        public static IntPtr WoWWindowHandle(GameInterface info)
        {
            return info.wowProcess.MainWindowHandle;
        }

        public static void InvokeInGUIThread(Action action)
        {
            MainForm.Instance.PostInvoke(action);
        }

        public static void TaskDialog(this Form form, string title, string message, bool warningOrError)
        {
            form.TaskDialog(title, message, warningOrError ? NotifyUserType.Warn : NotifyUserType.Info);
        }
    }
}