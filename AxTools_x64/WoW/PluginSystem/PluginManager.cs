using AxTools.Helpers;
using AxTools.WoW.PluginSystem.API;
using AxTools.WoW.PluginSystem.Plugins;
using KeyboardWatcher;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AxTools.WoW.PluginSystem
{
    internal static class PluginManagerEx
    {
        private static readonly Log2 log = new Log2(nameof(PluginManagerEx));
        private static readonly SynchronizedCollection<PluginContainer> _pluginContainers = new SynchronizedCollection<PluginContainer>();
        private static readonly object AddRemoveLock = new object();
        internal static readonly Dictionary<string, int> PluginWoW = new Dictionary<string, int>();
        internal static bool UpdateIsActive { get; set; }

        internal static event Action PluginStateChanged;
        internal static event Action PluginsLoaded;
        internal static event Action<IPlugin3> PluginLoaded;
        internal static event Action<IPlugin3> PluginUnloaded;

        private static Dictionary<string, List<DateTime>> _pluginsUsageStats;

        internal static Dictionary<string, List<DateTime>> PluginsUsageStats
        {
            get
            {
                if (_pluginsUsageStats == null)
                {
                    var settingsFile = AppFolders.ConfigDir + "\\plugins-usage-stats.json";
                    if (File.Exists(settingsFile))
                    {
                        try
                        {
                            var rawText = File.ReadAllText(settingsFile, Encoding.UTF8);
                            _pluginsUsageStats = JsonConvert.DeserializeObject<Dictionary<string, List<DateTime>>>(rawText);
                        }
                        catch (Exception)
                        {
                            _pluginsUsageStats = new Dictionary<string, List<DateTime>>();
                        }
                    }
                    else
                    {
                        _pluginsUsageStats = new Dictionary<string, List<DateTime>>();
                    }
                }
                return _pluginsUsageStats;
            }
        }

        internal static void SavePluginUsageStats()
        {
            var settingsFile = AppFolders.ConfigDir + "\\plugins-usage-stats.json";
            var sb = new StringBuilder(1024);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    var js = new JsonSerializer();
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 4;
                    js.Serialize(jsonWriter, PluginsUsageStats);
                }
            }
            var json = sb.ToString();
            File.WriteAllText(settingsFile, json, Encoding.UTF8);
        }

        internal static IEnumerable<IPlugin3> RunningPlugins
        {
            get
            {
                return _pluginContainers.Where(l => l.IsRunning).Select(l => l.Plugin);
            }
        }

        internal static IEnumerable<IPlugin3> LoadedPlugins
        {
            get
            {
                return _pluginContainers.Select(l => l.Plugin);
            }
        }

        internal static void AddPluginToRunning(IPlugin3 plugin, WowProcess process)
        {
            lock (AddRemoveLock)
            {
                if (!RunningPlugins.Contains(plugin))
                {
                    _pluginContainers.First(l => l.Plugin.GetType() == plugin.GetType()).IsRunning = true;
                    try
                    {
                        plugin.OnStart(new GameInterface(process));
                        PluginWoW[plugin.Name] = process.ProcessID;
                        PluginsUsageStats[plugin.Name].Add(DateTime.UtcNow);
                        SavePluginUsageStats();
                        log.Info($"{process} [{plugin.Name}] Plug-in is started");
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Plug-in OnStart error [{plugin.Name}]: {ex.Message}");
                    }
                    if (Settings2.Instance.WoWPluginShowIngameNotifications)
                    {
                        Notify.TrayPopup(nameof(AxTools), "Plug-in <" + plugin.Name + "> is started", NotifyUserType.Info, false, plugin.TrayIcon);
                    }
                    PluginStateChanged?.Invoke();
                }
            }
        }

        internal static void RemovePluginFromRunning(IPlugin3 plugin)
        {
            lock (AddRemoveLock)
            {
                if (RunningPlugins.Contains(plugin))
                {
                    try
                    {
                        plugin.OnStop();
                        PluginWoW.Remove(plugin.Name);
                        log.Info($"[{plugin.Name}] Plug-in is stopped");
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Can't shutdown plug-in [{plugin.Name}]: {ex.Message}");
                    }
                    if (Settings2.Instance.WoWPluginShowIngameNotifications)
                    {
                        Notify.TrayPopup(nameof(AxTools), "Plug-in <" + plugin.Name + "> is stopped", NotifyUserType.Info, false, plugin.TrayIcon);
                    }
                    _pluginContainers.First(l => l.Plugin.GetType() == plugin.GetType()).IsRunning = false;
                    PluginStateChanged?.Invoke();
                }
            }
        }

        internal static Task LoadPluginsAsync()
        {
            return Task.Run(delegate
            {
                if (UpdateIsActive)
                {
                    UpdatePluginsFromWeb();
                    Settings2.Instance.PluginsLastTimeUpdated = DateTime.UtcNow;
                    UpdateIsActive = false;
                }
                LoadPlugins();
                LoadPluginsFromDisk();
                CheckDependencies();
                ClearOldAssemblies();
                foreach (var i in Settings2.Instance.PluginHotkeys.Where(l => LoadedPlugins.Select(k => k.Name).Contains(l.Key)))
                {
                    HotkeyManager.AddKeys("Plugin_" + i.Key, i.Value);
                }
                PluginsLoaded?.Invoke();
            });
        }

        internal static List<IPlugin3> GetSortedByUsageListOfPlugins()
        {
            var list = LoadedPlugins.ToList();
            list.Sort((first, second) =>
            {
                // delete old timestamps
                if (!PluginsUsageStats.ContainsKey(first.Name)) PluginsUsageStats.Add(first.Name, new List<DateTime>());
                if (!PluginsUsageStats.ContainsKey(second.Name)) PluginsUsageStats.Add(second.Name, new List<DateTime>());
                PluginsUsageStats[first.Name].RemoveAll(l => (DateTime.UtcNow - l).TotalDays > 30);
                PluginsUsageStats[second.Name].RemoveAll(l => (DateTime.UtcNow - l).TotalDays > 30);
                var cmp = PluginsUsageStats[first.Name].Count.CompareTo(PluginsUsageStats[second.Name].Count);
                if (cmp == 0)
                {
                    return string.Compare(first.Name, second.Name, StringComparison.Ordinal);
                }
                return -cmp; // by descending
            });
            SavePluginUsageStats();
            return list;
        }

        private static void UpdatePluginsFromWeb()
        {
            var @lock = Program.ShutdownLock.GetLock();
            try
            {
                if (!Directory.Exists(Path.Combine(AppFolders.TempDir, "plugins_update")))
                    Directory.CreateDirectory(Path.Combine(AppFolders.TempDir, "plugins_update"));
                using (WebClient webClient = new WebClient())
                {
                    foreach (string pluginName in new DirectoryInfo(AppFolders.PluginsDir).GetDirectories().Select(l => l.Name))
                    {
                        try
                        {
                            log.Info($"UpdatePluginsFromWeb: updating '{pluginName}'");
                            var fileName = Path.Combine(AppFolders.TempDir, $"plugins_update\\{pluginName}.zip");
                            webClient.DownloadFile($"https://axio.name/axtools/plugins/{pluginName}.zip", fileName);
                            Directory.Delete(Path.Combine(AppFolders.PluginsDir, pluginName), true);
                            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(fileName, Encoding.UTF8))
                            {
                                zip.ExtractAll(AppFolders.PluginsDir, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                            }
                            log.Info($"UpdatePluginsFromWeb: '{pluginName}' is updated");
                        }
                        catch (Exception ex)
                        {
                            log.Info($"Call to UpdatePluginsFromWeb caused non-critical error while processing '{pluginName}' plug-in: {ex.Message}");
                        }
                    }
                }
            }
            finally
            {
                Program.ShutdownLock.ReleaseLock(@lock);
            }
        }

        private static void LoadPlugins()
        {
            IPlugin3 fishing = new Fishing();
            _pluginContainers.Add(new PluginContainer(fishing));
            log.Info($"Plug-in loaded: {_pluginContainers.Last().Plugin.Name} {_pluginContainers.Last().Plugin.Version}");
            PluginLoaded?.Invoke(fishing);
            IPlugin3 flagReturner = new FlagReturner();
            _pluginContainers.Add(new PluginContainer(flagReturner));
            log.Info($"Plug-in loaded: {_pluginContainers.Last().Plugin.Name} {_pluginContainers.Last().Plugin.Version}");
            PluginLoaded?.Invoke(flagReturner);
            IPlugin3 goodsDestroyer = new GoodsDestroyer();
            _pluginContainers.Add(new PluginContainer(goodsDestroyer));
            log.Info($"Plug-in loaded: {_pluginContainers.Last().Plugin.Name} {_pluginContainers.Last().Plugin.Version}");
            PluginLoaded?.Invoke(goodsDestroyer);
        }

        private static void LoadPluginsFromDisk()
        {
#pragma warning disable S3885 // "Assembly.Load" should be used
            Assembly.LoadFile(Path.Combine(Application.StartupPath, "KeraLua.dll"));
            Assembly.LoadFile(Path.Combine(Application.StartupPath, "NLua.dll"));
            Assembly.LoadFile(Path.Combine(Application.StartupPath, "ICSharpCode.TextEditor.dll"));

            var directories = Directory.GetDirectories(AppFolders.PluginsDir);
            foreach (string directory in directories)
            {
                try
                {
                    log.Info($"Processing directory <{directory}>");
                    var md5ForFolder = Utils.CreateMd5ForFolder(directory);
                    var dllPath = $"{AppFolders.PluginsBinariesDir}\\{md5ForFolder}.dll";
                    Assembly dll;
                    if (!File.Exists(dllPath))
                    {
                        dll = CompilePlugin(directory);
                        log.Info($"Plug-in from directory <{directory}> is compiled");
                    }
                    else
                    {
                        dll = Assembly.LoadFile(dllPath);
                        log.Info($"Plug-in from directory <{directory}> with hash {md5ForFolder} is already compiled");
                    }
                    if (dll != null)
                    {
                        var type = dll.GetTypes().FirstOrDefault(k => k.IsClass && typeof(IPlugin3).IsAssignableFrom(k));
                        if (type != default(Type))
                        {
                            var temp = (IPlugin3)Activator.CreateInstance(type);
                            if (_pluginContainers.Select(l => l.Plugin).All(l => l.Name != temp.Name))
                            {
                                if (!string.IsNullOrWhiteSpace(temp.Name))
                                {
                                    _pluginContainers.Add(new PluginContainer(temp));
                                    log.Info($"Plug-in loaded: {temp.Name} {temp.Version}");
                                    PluginLoaded?.Invoke(temp);
                                }
                                else
                                {
                                    throw new MissingFieldException("<IPlugin2.Name> is empty");
                                }
                            }
                            else
                            {
                                log.Info($"Can't load plug-in [{temp.Name}]: already loaded");
                            }
                        }
                        else
                        {
                            throw new BadImageFormatException("Can't find IPlugin2 interface in plug-in image!");
                        }
                    }
                    else
                    {
                        throw new BadImageFormatException("Plug-in image is null!");
                    }
                }
                catch (Exception ex)
                {
                    log.Info($"Can't load plug-in <{directory}>: {ex.Message}");
                    Notify.TrayPopup("[PluginManager] Plug-in error", $"Plug-in from folder '{directory}' cannot be loaded.\r\nClick here to open the website with updated versions of plug-ins",
                        NotifyUserType.Warn, true, null, 10, (sender, args) => Process.Start(Globals.PluginsURL));
                }
            }
#pragma warning restore S3885 // "Assembly.Load" should be used
        }

        private static void CheckDependencies()
        {
            var indexesOfPluginsWithUnresolvedDeps = new HashSet<int>();
            for (int i = 0; i < _pluginContainers.Count; i++)
            {
                if (_pluginContainers[i].Plugin is IPlugin3 plugin2 && plugin2.Dependencies != null)
                {
                    foreach (string dep in plugin2.Dependencies)
                    {
                        if (!_pluginContainers.Select(l => l.Plugin.Name).Contains(dep))
                        {
                            indexesOfPluginsWithUnresolvedDeps.Add(i);
                        }
                    }
                }

            }
            foreach (int indexOfPluginWithUnresolvedDeps in indexesOfPluginsWithUnresolvedDeps)
            {
                var plugin2 = _pluginContainers[indexOfPluginWithUnresolvedDeps].Plugin;
                _pluginContainers.RemoveAt(indexOfPluginWithUnresolvedDeps);
                PluginUnloaded?.Invoke(plugin2);
                Notify.SmartNotify("Plug-in error", $"Plug-in {plugin2.Name} requires {string.Join(", ", plugin2.Dependencies)}", NotifyUserType.Error, true);
            }
        }

        private static void ClearOldAssemblies()
        {
            var assemblies = Directory.GetFiles(AppFolders.PluginsBinariesDir);
            foreach (string assembly in assemblies)
            {
                try
                {
                    File.Delete(assembly);
                    log.Info("Old assembly is deleted: " + assembly);
                }
#pragma warning disable CC0004
                catch { /* don't care why */ }
#pragma warning restore CC0004
            }
        }

        private static Assembly CompilePlugin(string directory)
        {
            var cc = new CodeCompiler(directory);
            var results = cc.Compile();
            if (results.Errors.HasErrors)
            {
                foreach (object error in results.Errors)
                {
                    log.Info("Compiler Error: " + error);
                }
                return null;
            }
            return cc.CompiledAssembly;
        }



    }
}