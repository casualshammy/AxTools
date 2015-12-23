using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WoW.Management;
using Newtonsoft.Json;

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
        ///     Shows notify message in tray
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="icon"></param>
        public static void ShowNotifyMessage(string title, string text, ToolTipIcon icon)
        {
            AppSpecUtils.NotifyUser(title, text, (NotifyUserType) icon, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">Any text you want</param>
        /// <param name="wowIcon">Example: "Interface\\\\Icons\\\\achievement_bg_winwsg"</param>
        /// <param name="color">Text color</param>
        public static void ShowIngameNotify(string text, string wowIcon, Color color)
        {
            if (Settings.Instance.WoWPluginShowIngameNotifications)
            {
                WoWDXInject.ShowOverlayText(text, wowIcon, color);
            }
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
        ///     Creates new <see cref="SingleThreadTimer"/> timer.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static SingleThreadTimer CreateTimer(this IPlugin plugin, int interval, Action action)
        {
            return new SingleThreadTimer(interval, action) { PluginName = plugin.Name };
        }

        public static void SaveSettingsJSON<T>(this IPlugin plugin, T data, string path = null) where T : class
        {
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    JsonSerializer js = new JsonSerializer();
                    js.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 4;
                    js.Serialize(jsonWriter, data);
                }
            }
            AppSpecUtils.CheckCreateDir();
            string mySettingsDir = Globals.PluginsSettingsPath + "\\" + plugin.Name;
            if (!Directory.Exists(mySettingsDir))
            {
                Directory.CreateDirectory(mySettingsDir);
            }
            File.WriteAllText(path ?? (mySettingsDir + "\\settings.json"), sb.ToString(), Encoding.UTF8);
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

        public static void TestFunc(out byte r0, out byte r1, out byte r2, out byte r3, out bool b)
        {
            r0 = WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.GameState - 2);
            r1 = WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.GameState - 1);
            r2 = WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.GameState);
            r3 = WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.GameState + 1);
            b = WoWManager.WoWProcess.IsInGame;
        }

    }
}
