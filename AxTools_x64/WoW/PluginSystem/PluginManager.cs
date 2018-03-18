using AxTools.Helpers;
using AxTools.WoW.PluginSystem.Plugins;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AxTools.WoW.PluginSystem.API;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;
using KeyboardWatcher;

namespace AxTools.WoW.PluginSystem
{
    internal static class PluginManagerEx
    {
        private static readonly Log2 log = new Log2("PluginManagerEx");
        private static readonly SynchronizedCollection<PluginContainer> _pluginContainers = new SynchronizedCollection<PluginContainer>();
        private static readonly object AddRemoveLock = new object();
        internal static readonly Dictionary<string, int> PluginWoW = new Dictionary<string, int>();
        internal static event Action PluginStateChanged;
        internal static event Action PluginsLoaded;
        internal static event Action<IPlugin2> PluginLoaded;
        internal static event Action<IPlugin2> PluginUnloaded;

        internal static Dictionary<string, List<DateTime>> _pluginsUsageStats;
        private static Dictionary<string, List<DateTime>> pluginsUsageStats
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
                    js.Serialize(jsonWriter, pluginsUsageStats);
                }
            }
            string json = sb.ToString();
            File.WriteAllText(settingsFile, json, Encoding.UTF8);
            log.Info("PluginsUsageStats file has been updated, time: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        internal static ReadOnlyCollection<IPlugin2> RunningPlugins
        {
            get
            {
                return new ReadOnlyCollection<IPlugin2>(_pluginContainers.Where(l => l.IsRunning).Select(l => l.Plugin).ToList());
            }
        }

        internal static ReadOnlyCollection<IPlugin2> LoadedPlugins
        {
            get
            {
                return new ReadOnlyCollection<IPlugin2>(_pluginContainers.Select(l => l.Plugin).ToList());
            }
        }
        
        internal static void AddPluginToRunning(IPlugin2 plugin, WowProcess process)
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
                        pluginsUsageStats[plugin.Name].Add(DateTime.UtcNow);
                        SavePluginUsageStats();
                        log.Info(string.Format("{0} [{1}] Plugin is started", process, plugin.Name));
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Plugin OnStart error [{0}]: {1}", plugin.Name, ex.Message));
                    }
                    if (Settings2.Instance.WoWPluginShowIngameNotifications)
                    {
                        Notify.TrayPopup("AxTools", "Plugin <" + plugin.Name + "> is started", NotifyUserType.Info, false, plugin.TrayIcon);
                    }
                    PluginStateChanged?.Invoke();
                }
            }
        }

        internal static void RemovePluginFromRunning(IPlugin2 plugin)
        {
            lock (AddRemoveLock)
            {
                if (RunningPlugins.Contains(plugin))
                {
                    try
                    {
                        plugin.OnStop();
                        PluginWoW.Remove(plugin.Name);
                        log.Info(string.Format("[{0}] Plugin is stopped", plugin.Name));
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Can't shutdown plugin [{0}]: {1}", plugin.Name, ex.Message));
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
            return Task.Run(delegate {
                LoadPlugins();
                LoadPluginsFromDisk();
                CheckDependencies();
                ClearOldAssemblies();
                foreach (IPlugin2 plugin in LoadedPlugins)
                {
                    if (!pluginsUsageStats.ContainsKey(plugin.Name))
                    {
                        pluginsUsageStats[plugin.Name] = new List<DateTime>();
                        SavePluginUsageStats();
                    }
                }
                foreach (var i in Settings.Instance.PluginHotkeys.Where(l => LoadedPlugins.Select(k => k.Name).Contains(l.Key)))
                {
                    HotkeyManager.AddKeys("Plugin_" + i.Key, i.Value);
                }
                PluginsLoaded?.Invoke();
            });
        }

        internal static List<IPlugin2> GetSortedByUsageListOfPlugins()
        {
            List<IPlugin2> list = LoadedPlugins.ToList();
            list.Sort((first, second) =>
            {
                // delete old timestamps
                pluginsUsageStats[first.Name].RemoveAll(l => (DateTime.UtcNow - l).TotalDays > 30);
                pluginsUsageStats[second.Name].RemoveAll(l => (DateTime.UtcNow - l).TotalDays > 30);
                int cmp = pluginsUsageStats[first.Name].Count.CompareTo(pluginsUsageStats[second.Name].Count);
                if (cmp == 0)
                {
                    return first.Name.CompareTo(second.Name);
                }
                else
                {
                    return -cmp; // by descending
                }
            });
            SavePluginUsageStats();
            return list;
        }

        private static void LoadPlugins()
        {
            IPlugin2 fishing = new Fishing();
            _pluginContainers.Add(new PluginContainer(fishing));
            log.Info(string.Format("Plugin loaded: {0} {1}", _pluginContainers.Last().Plugin.Name, _pluginContainers.Last().Plugin.Version));
            PluginLoaded?.Invoke(fishing);
            IPlugin2 flagReturner = new FlagReturner();
            _pluginContainers.Add(new PluginContainer(flagReturner));
            log.Info(string.Format("Plugin loaded: {0} {1}", _pluginContainers.Last().Plugin.Name, _pluginContainers.Last().Plugin.Version));
            PluginLoaded?.Invoke(flagReturner);
            IPlugin2 goodsDestroyer = new GoodsDestroyer();
            _pluginContainers.Add(new PluginContainer(goodsDestroyer));
            log.Info(string.Format("Plugin loaded: {0} {1}", _pluginContainers.Last().Plugin.Name, _pluginContainers.Last().Plugin.Version));
            PluginLoaded?.Invoke(goodsDestroyer);
        }

        private static void LoadPluginsFromDisk()
        {
            string[] directories = Directory.GetDirectories(AppFolders.PluginsDir);
            foreach (string directory in directories)
            {
                try
                {
                    log.Info(string.Format("Processing directory <{0}>", directory));
                    string md5ForFolder = Utils.CreateMd5ForFolder(directory);
                    string dllPath = string.Format("{0}\\{1}.dll", AppFolders.PluginsBinariesDir, md5ForFolder);
                    Assembly dll;
                    if (!File.Exists(dllPath))
                    {
                        dll = CompilePlugin(directory);
                        log.Info(string.Format("Plugin from directory <{0}> is compiled", directory));
                    }
                    else
                    {
                        dll = Assembly.LoadFile(dllPath);
                        log.Info(string.Format("Plugin from directory <{0}> with hash {1} is already compiled", directory, md5ForFolder));
                    }
                    if (dll != null)
                    {
                        Type type = dll.GetTypes().FirstOrDefault(k => k.IsClass && typeof (IPlugin2).IsAssignableFrom(k));
                        if (type != default(Type))
                        {
                            IPlugin2 temp = (IPlugin2) Activator.CreateInstance(type);
                            if (_pluginContainers.Select(l => l.Plugin).All(l => l.Name != temp.Name))
                            {
                                if (!string.IsNullOrWhiteSpace(temp.Name))
                                {
                                    _pluginContainers.Add(new PluginContainer(temp));
                                    log.Info(string.Format("Plugin loaded: {0} {1}", temp.Name, temp.Version));
                                    PluginLoaded?.Invoke(temp);
                                }
                                else
                                {
                                    throw new Exception("<IPlugin2.Name> is empty");
                                }
                            }
                            else
                            {
                                log.Info(string.Format("Can't load plugin [{0}]: already loaded", temp.Name));
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
                    log.Info(string.Format("Can't load plugin <{0}>: {1}", directory, ex.Message));
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
                if (_pluginContainers[i].Plugin is IPlugin2 plugin2)
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
                IPlugin2 plugin2 = _pluginContainers[indexOfPluginWithUnresolvedDeps].Plugin as IPlugin2;
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
