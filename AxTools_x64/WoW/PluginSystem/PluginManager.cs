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
    internal static class PluginManager
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
                            Log.Info(String.Format("{0}:{1} :: [{2}] Plugin is started", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, plugin.Name));
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
                                ? string.Format("{0}:{1} :: [{2}] Plugin is stopped", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, plugin.Name)
                                : string.Format("UNKNOWN:null :: [{0}] Plugin is stopped", plugin.Name));
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("Can't shutdown plugin [{0}]: {1}", plugin.Name, ex.Message));
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
                                            Log.Info(string.Format("Can't load plugin [{0}]: [Name] is empty", temp.Name));
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
