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
        private static readonly Log2 log = new Log2("PluginManagerEx");
        private static readonly SynchronizedCollection<PluginContainer> _pluginContainers = new SynchronizedCollection<PluginContainer>();
        private static readonly object AddRemoveLock = new object();
        internal static readonly Dictionary<string, int> PluginWoW = new Dictionary<string, int>();
        internal static bool UpdateIsActive;
        
        internal static event Action PluginStateChanged;
        internal static event Action PluginsLoaded;
        internal static event Action<IPlugin3> PluginLoaded;
        internal static event Action<IPlugin3> PluginUnloaded;

        internal static Dictionary<string, List<DateTime>> _pluginsUsageStats;

        private static Dictionary<string, List<DateTime>> PluginsUsageStats
        {
            get
            {
                if (_pluginsUsageStats == null)
                {
                    string settingsFile = AppFolders.ConfigDir + "\\plugins-usage-stats.json";
                    if (File.Exists(settingsFile))
                    {
                        try
                        {
                            string rawText = File.ReadAllText(settingsFile, Encoding.UTF8);
                            _pluginsUsageStats = JsonConvert.DeserializeObject<Dictionary<string, List<DateTime>>>(rawText);
                        }
                        catch
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
            string settingsFile = AppFolders.ConfigDir + "\\plugins-usage-stats.json";
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                {
                    JsonSerializer js = new JsonSerializer();
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 4;
                    js.Serialize(jsonWriter, PluginsUsageStats);
                }
            }
            string json = sb.ToString();
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
                        log.Info($"{process} [{plugin.Name}] Plugin is started");
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Plugin OnStart error [{plugin.Name}]: {ex.Message}");
                    }
                    if (Settings2.Instance.WoWPluginShowIngameNotifications)
                    {
                        Notify.TrayPopup("AxTools", "Plugin <" + plugin.Name + "> is started", NotifyUserType.Info, false, plugin.TrayIcon);
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
                        log.Info($"[{plugin.Name}] Plugin is stopped");
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Can't shutdown plugin [{plugin.Name}]: {ex.Message}");
                    }
                    if (Settings2.Instance.WoWPluginShowIngameNotifications)
                    {
                        Notify.TrayPopup("AxTools", "Plugin <" + plugin.Name + "> is stopped", NotifyUserType.Info, false, plugin.TrayIcon);
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
            List<IPlugin3> list = LoadedPlugins.ToList();
            list.Sort((first, second) =>
            {
                // delete old timestamps
                if (!PluginsUsageStats.ContainsKey(first.Name)) PluginsUsageStats.Add(first.Name, new List<DateTime>());
                if (!PluginsUsageStats.ContainsKey(second.Name)) PluginsUsageStats.Add(second.Name, new List<DateTime>());
                PluginsUsageStats[first.Name].RemoveAll(l => (DateTime.UtcNow - l).TotalDays > 30);
                PluginsUsageStats[second.Name].RemoveAll(l => (DateTime.UtcNow - l).TotalDays > 30);
                int cmp = PluginsUsageStats[first.Name].Count.CompareTo(PluginsUsageStats[second.Name].Count);
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
            Guid @lock = Program.ShutdownLock.GetLock();
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
                            string fileName = Path.Combine(AppFolders.TempDir, $"plugins_update\\{pluginName}.zip");
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
                            log.Info($"Call to UpdatePluginsFromWeb caused non-critical error while processing '{pluginName}' plugin: {ex.Message}");
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
            log.Info($"Plugin loaded: {_pluginContainers.Last().Plugin.Name} {_pluginContainers.Last().Plugin.Version}");
            PluginLoaded?.Invoke(fishing);
            IPlugin3 flagReturner = new FlagReturner();
            _pluginContainers.Add(new PluginContainer(flagReturner));
            log.Info($"Plugin loaded: {_pluginContainers.Last().Plugin.Name} {_pluginContainers.Last().Plugin.Version}");
            PluginLoaded?.Invoke(flagReturner);
            IPlugin3 goodsDestroyer = new GoodsDestroyer();
            _pluginContainers.Add(new PluginContainer(goodsDestroyer));
            log.Info($"Plugin loaded: {_pluginContainers.Last().Plugin.Name} {_pluginContainers.Last().Plugin.Version}");
            PluginLoaded?.Invoke(goodsDestroyer);
        }

        private static void LoadPluginsFromDisk()
        {
            Assembly.LoadFile(Path.Combine(Application.StartupPath, "KeraLua.dll"));
            Assembly.LoadFile(Path.Combine(Application.StartupPath, "NLua.dll"));
            Assembly.LoadFile(Path.Combine(Application.StartupPath, "ICSharpCode.TextEditor.dll"));
            string[] directories = Directory.GetDirectories(AppFolders.PluginsDir);
            foreach (string directory in directories)
            {
                try
                {
                    log.Info($"Processing directory <{directory}>");
                    string md5ForFolder = Utils.CreateMd5ForFolder(directory);
                    string dllPath = $"{AppFolders.PluginsBinariesDir}\\{md5ForFolder}.dll";
                    Assembly dll;
                    if (!File.Exists(dllPath))
                    {
                        dll = CompilePlugin(directory);
                        log.Info($"Plugin from directory <{directory}> is compiled");
                    }
                    else
                    {
                        dll = Assembly.LoadFile(dllPath);
                        log.Info($"Plugin from directory <{directory}> with hash {md5ForFolder} is already compiled");
                    }
                    if (dll != null)
                    {
                        Type type = dll.GetTypes().FirstOrDefault(k => k.IsClass && typeof(IPlugin3).IsAssignableFrom(k));
                        if (type != default(Type))
                        {
                            IPlugin3 temp = (IPlugin3)Activator.CreateInstance(type);
                            if (_pluginContainers.Select(l => l.Plugin).All(l => l.Name != temp.Name))
                            {
                                if (!string.IsNullOrWhiteSpace(temp.Name))
                                {
                                    _pluginContainers.Add(new PluginContainer(temp));
                                    log.Info($"Plugin loaded: {temp.Name} {temp.Version}");
                                    PluginLoaded?.Invoke(temp);
                                }
                                else
                                {
                                    throw new Exception("<IPlugin2.Name> is empty");
                                }
                            }
                            else
                            {
                                log.Info($"Can't load plugin [{temp.Name}]: already loaded");
                            }
                        }
                        else
                        {
                            throw new Exception("Can't find IPlugin2 interface in plugin image!");
                        }
                    }
                    else
                    {
                        throw new Exception("Plugin image is null!");
                    }
                }
                catch (Exception ex)
                {
                    log.Info($"Can't load plugin <{directory}>: {ex.Message}");
                    Notify.TrayPopup("[PluginManager] Plugin error", $"Plugin from folder '{directory}' cannot be loaded.\r\nClick here to open the website with updated versions of plugins",
                        NotifyUserType.Warn, true, null, 10, (sender, args) => Process.Start(Globals.PluginsURL));
                }
            }
        }

        private static void CheckDependencies()
        {
            HashSet<int> indexesOfPluginsWithUnresolvedDeps = new HashSet<int>();
            for (int i = 0; i < _pluginContainers.Count; i++)
            {
                if (_pluginContainers[i].Plugin is IPlugin3 plugin2)
                {
                    if (plugin2.Dependencies != null)
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
            }
            foreach (int indexOfPluginWithUnresolvedDeps in indexesOfPluginsWithUnresolvedDeps)
            {
                IPlugin3 plugin2 = _pluginContainers[indexOfPluginWithUnresolvedDeps].Plugin;
                _pluginContainers.RemoveAt(indexOfPluginWithUnresolvedDeps);
                PluginUnloaded?.Invoke(plugin2);
                Notify.SmartNotify("Plugin error", $"Plugin {plugin2.Name} requires {string.Join(", ", plugin2.Dependencies)}", NotifyUserType.Error, true);
            }
        }

        private static void ClearOldAssemblies()
        {
            string[] assemblies = Directory.GetFiles(AppFolders.PluginsBinariesDir);
            foreach (string assembly in assemblies)
            {
                try
                {
                    File.Delete(assembly);
                    log.Info("Old assembly is deleted: " + assembly);
                }
                catch { /* don't care why */ }
            }
        }

        private static Assembly CompilePlugin(string directory)
        {
            CodeCompiler cc = new CodeCompiler(directory);
            CompilerResults results = cc.Compile();
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