using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WoW.Management;
using AxTools.WoW.PluginSystem.Plugins;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using WindowsFormsAero.TaskDialog;

namespace AxTools.WoW.PluginSystem
{
    internal static class PluginManagerB
    {
        internal static List<IPlugin> Plugins = new List<IPlugin>();

        private static List<IPlugin> _activePlugins = new List<IPlugin>();
        internal static List<IPlugin> ActivePlugins { get { return _activePlugins; } }

        private static readonly object _lock = new object();

        internal static event Action PluginStateChanged;

        internal static void StartPlugins(IEnumerable<IPlugin> plugins)
        {
            lock (_lock)
            {
                if (_activePlugins.Count == 0)
                {
                    _activePlugins = plugins.ToList();
                    foreach (IPlugin plugin in _activePlugins)
                    {
                        try
                        {
                            plugin.OnStart();
                            Log.Info(string.Format("{0} [{1}] Plugin is started", WoWManager.WoWProcess, plugin.Name));
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("Plugin OnStart error [{0}]: {1}", plugin.Name, ex.Message));
                        }
                            
                    }
                    if (Settings.Instance.WoWPluginShowIngameNotifications)
                    {
                        if (_activePlugins.Count == 1)
                        {
                            WoWDXInject.ShowOverlayText("Plugin <" + _activePlugins.First().Name + "> is started", _activePlugins.First().WowIcon, Color.FromArgb(255, 102, 0));
                        }
                        else
                        {
                            WoWDXInject.ShowOverlayText("Plugins are started", "", Color.FromArgb(255, 102, 0));
                        }
                    }
                    if (PluginStateChanged != null)
                    {
                        PluginStateChanged();
                    }
                }
                else
                {
                    throw new Exception("ActivePlugin isn't null!");
                }
            }
        }

        internal static void StopPlugins()
        {
            lock (_lock)
            {
                if (_activePlugins.Count != 0)
                {
                    foreach (IPlugin plugin in _activePlugins)
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
                    }
                    if (Settings.Instance.WoWPluginShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsInGame)
                    {
                        if (_activePlugins.Count == 1)
                        {
                            WoWDXInject.ShowOverlayText("Plugin <" + _activePlugins.First().Name + "> is stopped", _activePlugins.First().WowIcon, Color.FromArgb(255, 0, 0));
                        }
                        else
                        {
                            WoWDXInject.ShowOverlayText("Plugins are stopped", "", Color.FromArgb(255, 102, 0));
                        }
                    }
                    _activePlugins.Clear();
                    if (PluginStateChanged != null)
                    {
                        PluginStateChanged();
                    }
                }
                else
                {
                    throw new Exception("ActivePlugin is null!");
                }
            }
        }

        internal static void AddPluginToRunning(IPlugin plugin)
        {
            lock (_lock)
            {
                if (!_activePlugins.Contains(plugin))
                {
                    _activePlugins.Add(plugin);
                    try
                    {
                        plugin.OnStart();
                        Log.Info(string.Format("{0} [{1}] Plugin is started", WoWManager.WoWProcess, plugin.Name));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("Plugin OnStart error [{0}]: {1}", plugin.Name, ex.Message));
                    }
                    if (Settings.Instance.WoWPluginShowIngameNotifications)
                    {
                        WoWDXInject.ShowOverlayText("Plugin <" + plugin.Name + "> is started", plugin.WowIcon, Color.FromArgb(255, 102, 0));
                    }
                    if (PluginStateChanged != null)
                    {
                        PluginStateChanged();
                    }
                }
            }
        }

        internal static void RemovePluginFromRunning(IPlugin plugin)
        {
            lock (_lock)
            {
                if (_activePlugins.Contains(plugin))
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
                    if (Settings.Instance.WoWPluginShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsInGame)
                    {
                        WoWDXInject.ShowOverlayText("Plugin <" + plugin.Name + "> is stopped", plugin.WowIcon, Color.FromArgb(255, 0, 0));
                    }
                    _activePlugins.Remove(plugin);
                    if (PluginStateChanged != null)
                    {
                        PluginStateChanged();
                    }
                }
            }
        }

        internal static void LoadPlugins()
        {
            Plugins.Add(new Fishing());
            Log.Info(string.Format("Plugin loaded: {0} {1}", Plugins.Last().Name, Plugins.Last().Version));
            Plugins.Add(new FlagReturner());
            Log.Info(string.Format("Plugin loaded: {0} {1}", Plugins.Last().Name, Plugins.Last().Version));
            Plugins.Add(new GoodsDestroyer());
            Log.Info(string.Format("Plugin loaded: {0} {1}", Plugins.Last().Name, Plugins.Last().Version));
            LoadPluginsFromDisk();
        }

        private static void LoadPluginsFromDisk()
        {
            string[] directories = Directory.GetDirectories(Globals.PluginsPath);
            bool haveError = false;
            foreach (string directory in directories)
            {
                try
                {
                    CodeCompiler cc = new CodeCompiler(directory);
                    CompilerResults results = cc.Compile();

                    if (results != null)
                    {
                        if (results.Errors.HasErrors)
                        {
                            foreach (object error in results.Errors)
                            {
                                Log.Info("Compiler Error: " + error);
                            }
                            haveError = true;
                        }
                        else
                        {
                            Type[] types = cc.CompiledAssembly.GetTypes();
                            foreach (Type t in types)
                            {
                                if (t.IsClass && typeof(IPlugin).IsAssignableFrom(t))
                                {
                                    IPlugin temp = (IPlugin) Activator.CreateInstance(t);
                                    if (Plugins.All(i => i.Name != temp.Name))
                                    {
                                        if (!string.IsNullOrWhiteSpace(temp.Name))
                                        {
                                            Plugins.Add(temp);
                                            Log.Info(string.Format("Plugin loaded: {0} {1}", temp.Name, temp.Version));
                                        }
                                        else
                                        {
                                            Log.Info(string.Format("Can't load plugin [{0}]: [Name] is empty", temp.GetHashCode()));
                                            haveError = true;
                                        }
                                    }
                                    else
                                    {
                                        Log.Info(string.Format("Can't load plugin [{0}]: already loaded", temp.Name));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }
            if (haveError)
            {
                MainForm.Instance.ShowTaskDialog("Plugin error", "Error has occured during compiling plugins\r\nSome plugins could not be loaded and are disabled", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

    }

    internal static class PluginManagerEx
    {
        private static readonly List<PluginContainer> _pluginContainers = new List<PluginContainer>();
        private static readonly object _lock = new object();
        internal static event Action PluginStateChanged;

        internal static IEnumerable<IPlugin> EnabledPlugins
        {
            get
            {
                lock (_lock )
                {
                    return _pluginContainers.Where(l => l.EnabledByUser).Select(l => l.Plugin);
                }
            }
        }

        internal static IEnumerable<IPlugin> RunningPlugins
        {
            get
            {
                lock (_lock)
                {
                    return _pluginContainers.Where(l => l.IsRunning).Select(l => l.Plugin);
                }
            }
        }

        internal static IEnumerable<IPlugin> LoadedPlugins
        {
            get
            {
                lock (_lock )
                {
                    return _pluginContainers.Select(l => l.Plugin);
                }
            }
        }

        internal static void StartPlugins()
        {
            lock (_lock)
            {
                if (!RunningPlugins.Any())
                {
                    foreach (PluginContainer pluginContainer in _pluginContainers.Where(l => l.EnabledByUser))
                    {
                        pluginContainer.IsRunning = true;
                        try
                        {
                            pluginContainer.Plugin.OnStart();
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
                            WoWDXInject.ShowOverlayText("Plugin <" + RunningPlugins.First().Name + "> is started", RunningPlugins.First().WowIcon, Color.FromArgb(255, 102, 0));
                        }
                        else
                        {
                            WoWDXInject.ShowOverlayText("Plugins are started", "", Color.FromArgb(255, 102, 0));
                        }
                    }
                    if (PluginStateChanged != null)
                    {
                        PluginStateChanged();
                    }
                }
                else
                {
                    throw new Exception("ActivePlugin isn't null!");
                }
            }
        }

        internal static void StopPlugins()
        {
            lock (_lock)
            {
                if (RunningPlugins.Any())
                {
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
                    if (Settings.Instance.WoWPluginShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsInGame)
                    {
                        if (RunningPlugins.Count() == 1)
                        {
                            WoWDXInject.ShowOverlayText("Plugin <" + RunningPlugins.First().Name + "> is stopped", RunningPlugins.First().WowIcon, Color.FromArgb(255, 0, 0));
                        }
                        else
                        {
                            WoWDXInject.ShowOverlayText("Plugins are stopped", "", Color.FromArgb(255, 102, 0));
                        }
                    }
                    foreach (PluginContainer pluginContainer in _pluginContainers.Where(l => l.IsRunning))
                    {
                        pluginContainer.IsRunning = false;
                    }
                    if (PluginStateChanged != null)
                    {
                        PluginStateChanged();
                    }
                }
                else
                {
                    throw new Exception("ActivePlugin is null!");
                }
            }
        }

        internal static void AddPluginToRunning(IPlugin plugin)
        {
            lock (_lock)
            {
                if (!RunningPlugins.Contains(plugin))
                {
                    _pluginContainers.First(l => l.Plugin.GetType() == plugin.GetType()).IsRunning = true;
                    try
                    {
                        plugin.OnStart();
                        Log.Info(string.Format("{0} [{1}] Plugin is started", WoWManager.WoWProcess, plugin.Name));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("Plugin OnStart error [{0}]: {1}", plugin.Name, ex.Message));
                    }
                    if (Settings.Instance.WoWPluginShowIngameNotifications)
                    {
                        WoWDXInject.ShowOverlayText("Plugin <" + plugin.Name + "> is started", plugin.WowIcon, Color.FromArgb(255, 102, 0));
                    }
                    if (PluginStateChanged != null)
                    {
                        PluginStateChanged();
                    }
                }
            }
        }

        internal static void RemovePluginFromRunning(IPlugin plugin)
        {
            lock (_lock)
            {
                if (RunningPlugins.Contains(plugin))
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
                    if (Settings.Instance.WoWPluginShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsInGame)
                    {
                        WoWDXInject.ShowOverlayText("Plugin <" + plugin.Name + "> is stopped", plugin.WowIcon, Color.FromArgb(255, 0, 0));
                    }
                    _pluginContainers.First(l => l.Plugin.GetType() == plugin.GetType()).IsRunning = false;
                    if (PluginStateChanged != null)
                    {
                        PluginStateChanged();
                    }
                }
            }
        }

        internal static void EnablePlugin(IPlugin plugin)
        {
            lock (_lock)
            {
                PluginContainer container = _pluginContainers.FirstOrDefault(l => l.Plugin == plugin);
                if (container != null)
                {
                    container.EnabledByUser = true;
                }
            }
        }

        internal static void DisablePlugin(IPlugin plugin)
        {
            lock (_lock)
            {
                PluginContainer container = _pluginContainers.FirstOrDefault(l => l.Plugin == plugin);
                if (container != null)
                {
                    container.EnabledByUser = false;
                }
            }
        }

        internal static void LoadPlugins()
        {
            lock (_lock)
            {
                IPlugin fishing = new Fishing();
                _pluginContainers.Add(new PluginContainer(fishing, Settings.Instance.EnabledPluginsList.Contains(fishing.Name)));
                Log.Info(string.Format("Plugin loaded: {0} {1}", _pluginContainers.Last().Plugin.Name, _pluginContainers.Last().Plugin.Version));
                IPlugin flagReturner = new FlagReturner();
                _pluginContainers.Add(new PluginContainer(flagReturner, Settings.Instance.EnabledPluginsList.Contains(flagReturner.Name)));
                Log.Info(string.Format("Plugin loaded: {0} {1}", _pluginContainers.Last().Plugin.Name, _pluginContainers.Last().Plugin.Version));
                IPlugin goodsDestroyer = new GoodsDestroyer();
                _pluginContainers.Add(new PluginContainer(goodsDestroyer, Settings.Instance.EnabledPluginsList.Contains(goodsDestroyer.Name)));
                Log.Info(string.Format("Plugin loaded: {0} {1}", _pluginContainers.Last().Plugin.Name, _pluginContainers.Last().Plugin.Version));
                LoadPluginsFromDisk();
            }
        }

        private static void LoadPluginsFromDisk()
        {
            string[] directories = Directory.GetDirectories(Globals.PluginsPath);
            bool haveError = false;
            foreach (string directory in directories)
            {
                try
                {
                    CodeCompiler cc = new CodeCompiler(directory);
                    CompilerResults results = cc.Compile();

                    if (results != null)
                    {
                        if (results.Errors.HasErrors)
                        {
                            foreach (object error in results.Errors)
                            {
                                Log.Info("Compiler Error: " + error);
                            }
                            haveError = true;
                        }
                        else
                        {
                            Type[] types = cc.CompiledAssembly.GetTypes();
                            foreach (Type t in types)
                            {
                                if (t.IsClass && typeof(IPlugin).IsAssignableFrom(t))
                                {
                                    IPlugin temp = (IPlugin) Activator.CreateInstance(t);
                                    if (_pluginContainers.Select(l => l.Plugin).All(l => l.Name != temp.Name))
                                    {
                                        if (!string.IsNullOrWhiteSpace(temp.Name))
                                        {
                                            _pluginContainers.Add(new PluginContainer(temp, Settings.Instance.EnabledPluginsList.Contains(temp.Name)));
                                            Log.Info(string.Format("Plugin loaded: {0} {1}", temp.Name, temp.Version));
                                        }
                                        else
                                        {
                                            Log.Info(string.Format("Can't load plugin [{0}]: [Name] is empty", temp.GetHashCode()));
                                            haveError = true;
                                        }
                                    }
                                    else
                                    {
                                        Log.Info(string.Format("Can't load plugin [{0}]: already loaded", temp.Name));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }
            if (haveError)
            {
                MainForm.Instance.ShowTaskDialog("Plugin error", "Error has occured during compiling plugins\r\nSome plugins could not be loaded and are disabled", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

    }

}
