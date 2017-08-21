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

namespace AxTools.WoW.PluginSystem
{
    internal static class PluginManagerEx
    {
        private static readonly SynchronizedCollection<PluginContainer> _pluginContainers = new SynchronizedCollection<PluginContainer>();
        private static volatile bool _isRunning;
        private static volatile bool _managerRunning;
        private static readonly object AddRemoveLock = new object();
        internal static event Action PluginStateChanged;
        internal static event Action PluginsLoaded;
        internal static event Action<IPlugin> PluginEnabled;
        internal static event Action<IPlugin> PluginDisabled;

        internal static ReadOnlyCollection<IPlugin> EnabledPlugins
        {
            get
            {
                return new ReadOnlyCollection<IPlugin>(_pluginContainers.Where(l => l.EnabledByUser).Select(l => l.Plugin).ToList());
                //return _pluginContainers.Where(l => l.EnabledByUser).Select(l => l.Plugin);
            }
        }

        internal static ReadOnlyCollection<IPlugin> RunningPlugins
        {
            get
            {
                return new ReadOnlyCollection<IPlugin>(_pluginContainers.Where(l => l.IsRunning).Select(l => l.Plugin).ToList());
            }
        }

        internal static ReadOnlyCollection<IPlugin> LoadedPlugins
        {
            get
            {
                return new ReadOnlyCollection<IPlugin>(_pluginContainers.Select(l => l.Plugin).ToList());
            }
        }

        internal static void StartPlugins()
        {
            if (!_isRunning)
            {
                _managerRunning = true;
                foreach (PluginContainer pluginContainer in _pluginContainers.Where(l => l.EnabledByUser))
                {
                    pluginContainer.IsRunning = true;
                    try
                    {
                        pluginContainer.Plugin.OnStart();
                        Settings.Instance.PluginsUsageStat2[pluginContainer.Plugin.Name].Add(DateTime.UtcNow);
                        Log.Info(string.Format("{0} [{1}] Plugin is started", WoWManager.WoWProcess, pluginContainer.Plugin.Name));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("Plugin OnStart error [{0}]: {1}", pluginContainer.Plugin.Name, ex.Message));
                    }
                }
                if (Settings.Instance.WoWPluginShowIngameNotifications)
                {
                    if (RunningPlugins.Count() == 1)
                    {
                        Notify.TrayPopup("AxTools", "Plugin <" + RunningPlugins.First().Name + "> is started", NotifyUserType.Info, false, RunningPlugins.First().TrayIcon);
                    }
                    else
                    {
                        Notify.TrayPopup("AxTools", string.Format("Plugins are started ({0})", string.Join(", ", RunningPlugins.Select(l => l.Name))), NotifyUserType.Info, false);
                    }
                }
                PluginStateChanged?.Invoke();
                _isRunning = true;
            }
            else
            {
                throw new Exception("Plugin are already running!");
            }
        }

        internal static void StopPlugins()
        {
            if (_isRunning)
            {
                _managerRunning = false;
                foreach (PluginContainer pluginContainer in _pluginContainers.Where(l => l.IsRunning))
                {
                    try
                    {
                        pluginContainer.Plugin.OnStop();
                        Log.Info(WoWManager.WoWProcess != null
                            ? string.Format("{0} [{1}] Plugin is stopped", WoWManager.WoWProcess, pluginContainer.Plugin.Name)
                            : string.Format("[UNKNOWN] [{0}] Plugin is stopped", pluginContainer.Plugin.Name));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("{0} Can't shutdown plugin [{1}]: {2}", WoWManager.WoWProcess, pluginContainer.Plugin.Name, ex.Message));
                    }
                }
                if (Settings.Instance.WoWPluginShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && GameFunctions.IsInGame)
                {
                    if (RunningPlugins.Count() == 1)
                    {
                        Notify.TrayPopup("AxTools", "Plugin <" + RunningPlugins.First().Name + "> is stopped", NotifyUserType.Info, false, RunningPlugins.First().TrayIcon);
                    }
                    else
                    {
                        Notify.TrayPopup("AxTools", string.Format("Plugins are stopped ({0})", string.Join(", ", RunningPlugins.Select(l => l.Name))), NotifyUserType.Info, false);
                    }
                }
                foreach (PluginContainer pluginContainer in _pluginContainers.Where(l => l.IsRunning))
                {
                    pluginContainer.IsRunning = false;
                }
                PluginStateChanged?.Invoke();
                _isRunning = false;
            }
            else
            {
                throw new Exception("No plugins are running!");
            }
        }

        internal static void AddPluginToRunning(IPlugin plugin)
        {
            lock (AddRemoveLock)
            {
                if (_managerRunning && !RunningPlugins.Contains(plugin))
                {
                    _pluginContainers.First(l => l.Plugin.GetType() == plugin.GetType()).IsRunning = true;
                    try
                    {
                        plugin.OnStart();
                        Settings.Instance.PluginsUsageStat2[plugin.Name].Add(DateTime.UtcNow);
                        Log.Info(string.Format("{0} [{1}] Plugin is started", WoWManager.WoWProcess, plugin.Name));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("Plugin OnStart error [{0}]: {1}", plugin.Name, ex.Message));
                    }
                    if (Settings.Instance.WoWPluginShowIngameNotifications)
                    {
                        Notify.TrayPopup("AxTools", "Plugin <" + plugin.Name + "> is started", NotifyUserType.Info, false, plugin.TrayIcon);
                    }
                    PluginStateChanged?.Invoke();
                }
            }
        }

        internal static void RemovePluginFromRunning(IPlugin plugin)
        {
            lock (AddRemoveLock)
            {
                if (_managerRunning && RunningPlugins.Contains(plugin))
                {
                    try
                    {
                        plugin.OnStop();
                        Log.Info(WoWManager.WoWProcess != null
                            ? string.Format("{0} [{1}] Plugin is stopped", WoWManager.WoWProcess, plugin.Name)
                            : string.Format("[UNKNOWN] [{0}] Plugin is stopped", plugin.Name));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("{0} Can't shutdown plugin [{1}]: {2}", WoWManager.WoWProcess, plugin.Name, ex.Message));
                    }
                    if (Settings.Instance.WoWPluginShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && GameFunctions.IsInGame)
                    {
                        Notify.TrayPopup("AxTools", "Plugin <" + plugin.Name + "> is stopped", NotifyUserType.Info, false, plugin.TrayIcon);
                    }
                    _pluginContainers.First(l => l.Plugin.GetType() == plugin.GetType()).IsRunning = false;
                    PluginStateChanged?.Invoke();
                }
            }
        }

        internal static void SetPluginEnabled(IPlugin plugin, bool enabled)
        {
            PluginContainer container = _pluginContainers.FirstOrDefault(l => l.Plugin == plugin);
            if (container != null && container.EnabledByUser != enabled)
            {
                container.EnabledByUser = enabled;
                if (enabled)
                {
                    PluginEnabled?.Invoke(plugin);
                }
                else
                {
                    PluginDisabled?.Invoke(plugin);
                }
            }
        }

        internal static Task LoadPluginsAsync()
        {
            return Task.Run((Action) LoadPlugins);
        }

        internal static List<IPlugin> GetSortedByUsageListOfPlugins()
        {
            List<IPlugin> list = LoadedPlugins.ToList();
            list.Sort((first, second) =>
            {
                // delete old timestamps
                Settings.Instance.PluginsUsageStat2[first.Name].RemoveAll(l => (DateTime.UtcNow - l).TotalDays > 30);
                Settings.Instance.PluginsUsageStat2[second.Name].RemoveAll(l => (DateTime.UtcNow - l).TotalDays > 30);
                int cmp = Settings.Instance.PluginsUsageStat2[first.Name].Count.CompareTo(Settings.Instance.PluginsUsageStat2[second.Name].Count);
                if (cmp == 0)
                {
                    return first.Name.CompareTo(second.Name);
                }
                else
                {
                    return -cmp; // by descending
                }
            });
            return list;
        }

        private static void LoadPlugins()
        {
            IPlugin fishing = new Fishing();
            _pluginContainers.Add(new PluginContainer(fishing, Settings.Instance.EnabledPluginsList.Contains(fishing.Name)));
            Log.Info(string.Format("[PluginManager] Plugin loaded: {0} {1}", _pluginContainers.Last().Plugin.Name, _pluginContainers.Last().Plugin.Version));
            IPlugin flagReturner = new FlagReturner();
            _pluginContainers.Add(new PluginContainer(flagReturner, Settings.Instance.EnabledPluginsList.Contains(flagReturner.Name)));
            Log.Info(string.Format("[PluginManager] Plugin loaded: {0} {1}", _pluginContainers.Last().Plugin.Name, _pluginContainers.Last().Plugin.Version));
            IPlugin goodsDestroyer = new GoodsDestroyer();
            _pluginContainers.Add(new PluginContainer(goodsDestroyer, Settings.Instance.EnabledPluginsList.Contains(goodsDestroyer.Name)));
            Log.Info(string.Format("[PluginManager] Plugin loaded: {0} {1}", _pluginContainers.Last().Plugin.Name, _pluginContainers.Last().Plugin.Version));
            LoadPluginsFromDisk();
            foreach (IPlugin plugin in LoadedPlugins)
            {
                if (!Settings.Instance.PluginsUsageStat2.ContainsKey(plugin.Name))
                {
                    Settings.Instance.PluginsUsageStat2[plugin.Name] = new List<DateTime>();
                }
            }
            PluginsLoaded?.Invoke();
        }

        private static void LoadPluginsFromDisk()
        {
            string[] directories = Directory.GetDirectories(AppFolders.PluginsDir);
            foreach (string directory in directories)
            {
                try
                {
                    Log.Info(string.Format("[PluginManager] Processing directory <{0}>", directory));
                    string md5ForFolder = Utils.CreateMd5ForFolder(directory);
                    string dllPath = string.Format("{0}\\{1}.dll", AppFolders.PluginsBinariesDir, md5ForFolder);
                    Assembly dll;
                    if (!File.Exists(dllPath))
                    {
                        dll = CompilePlugin(directory);
                        Log.Info(string.Format("[PluginManager] Plugin from directory <{0}> is compiled", directory));
                    }
                    else
                    {
                        dll = Assembly.LoadFile(dllPath);
                        Log.Info(string.Format("[PluginManager] Plugin from directory <{0}> with hash {1} is already compiled", directory, md5ForFolder));
                    }
                    if (dll != null)
                    {
                        Type type = dll.GetTypes().FirstOrDefault(k => k.IsClass && typeof (IPlugin).IsAssignableFrom(k));
                        if (type != default(Type))
                        {
                            IPlugin temp = (IPlugin) Activator.CreateInstance(type);
                            if (_pluginContainers.Select(l => l.Plugin).All(l => l.Name != temp.Name))
                            {
                                if (!string.IsNullOrWhiteSpace(temp.Name))
                                {
                                    _pluginContainers.Add(new PluginContainer(temp, Settings.Instance.EnabledPluginsList.Contains(temp.Name)));
                                    Log.Info(string.Format("[PluginManager] Plugin loaded: {0} {1}", temp.Name, temp.Version));
                                }
                                else
                                {
                                    throw new Exception("<IPlugin.Name> is empty");
                                }
                            }
                            else
                            {
                                Log.Info(string.Format("[PluginManager] Can't load plugin [{0}]: already loaded", temp.Name));
                            }
                        }
                        else
                        {
                            throw new Exception("Can't find IPlugin interface in plugin image!");
                        }
                    }
                    else
                    {
                        throw new Exception("Plugin image is null!");
                    }
                }
                catch (Exception ex)
                {
                    Log.Info(string.Format("[PluginManager] Can't load plugin <{0}>: {1}", directory, ex.Message));
                    Notify.TaskDialog("[PluginManager] Plugin error",
                        string.Format("Error has occured during compiling plugins. Some plugins could not be loaded and are disabled\r\n\r\nVisit <a href=\"{0}\">this website</a> to download the latest versions of plugins",
                            Globals.PluginsURL), NotifyUserType.Warn, (sender, args) => Process.Start(Globals.PluginsURL));
                }
            }
            CheckDependencies();
            ClearOldAssemblies();
        }

        private static void CheckDependencies()
        {
            HashSet<int> indexesOfPluginsWithUnresolvedDeps = new HashSet<int>();
            for (int i = 0; i < _pluginContainers.Count; i++)
            {
                if (_pluginContainers[i].Plugin is IPlugin2 plugin2)
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
                IPlugin2 plugin2 = _pluginContainers[indexOfPluginWithUnresolvedDeps].Plugin as IPlugin2;
                _pluginContainers.RemoveAt(indexOfPluginWithUnresolvedDeps);
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
                    Log.Info("[PluginManager] Old assembly is deleted: " + assembly);
                }
                catch
                {
                    //
                }
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
                    Log.Info("[PluginManager] Compiler Error: " + error);
                }
                return null;
            }
            return cc.CompiledAssembly;
        }
    
    }

}
