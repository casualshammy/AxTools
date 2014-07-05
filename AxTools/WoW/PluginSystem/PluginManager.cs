using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.Classes;
using AxTools.Forms;
using AxTools.WoW.Management;
using AxTools.WoW.PluginSystem.Plugins;

namespace AxTools.WoW.PluginSystem
{
    internal static class PluginManager
    {
        internal static List<IPlugin> Plugins = new List<IPlugin>();

        internal static IPlugin ActivePlugin { private set; get; }

        private static volatile bool _shouldPulse;

        private static int _intervalBetweenPulses;

        private static Stopwatch _balancingStopwatch;

        private static readonly object _lock = new object();

        private static Thread _pluginThread;

        internal static void StartPlugin(IPlugin plugin)
        {
            lock (_lock)
            {
                if (ActivePlugin == null)
                {
                    ActivePlugin = plugin;
                    _intervalBetweenPulses = plugin.Interval;
                    _shouldPulse = true;
                    _pluginThread = new Thread(PluginPulse) {IsBackground = true};
                    try
                    {
                        ActivePlugin.OnStart();
                        Log.Print(String.Format("{0}:{1} :: [{2}] Plugin is started", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ActivePlugin.Name));
                    }
                    catch (Exception ex)
                    {
                        Log.Print(string.Format("Plugin OnStart error [{0}]: {1}", ActivePlugin.Name, ex.Message), true);
                    }
                    _balancingStopwatch = new Stopwatch();
                    if (Settings.Instance.WoWPluginShowIngameNotifications)
                    {
                        WoWDXInject.ShowOverlayText("Plugin <" + ActivePlugin.Name + "> is started", ActivePlugin.WowIcon, Color.FromArgb(255, 102, 0));
                    }
                    _pluginThread.Start();
                }
                else
                {
                    throw new Exception("ActivePlugin isn't null!");
                }
            }
        }

        internal static void StopPlugin(bool reportToMainWindow = false)
        {
            lock (_lock)
            {
                if (ActivePlugin != null)
                {
                    _shouldPulse = false;
                    if (!_pluginThread.Join(5000))
                    {
                        throw new Exception("Can't stop plugin!");
                    }
                    try
                    {
                        ActivePlugin.OnStop();
                        Log.Print(WoWManager.WoWProcess != null
                            ? string.Format("{0}:{1} :: [{2}] Plugin is stopped", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ActivePlugin.Name)
                            : string.Format("UNKNOWN:null :: [{0}] Plugin is stopped", ActivePlugin.Name));
                    }
                    catch (Exception ex)
                    {
                        Log.Print(string.Format("Can't shutdown plugin [{0}]: {1}", ActivePlugin.Name, ex.Message), true);
                    }
                    if (Settings.Instance.WoWPluginShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsInGame)
                    {
                        WoWDXInject.ShowOverlayText("Plugin <" + ActivePlugin.Name + "> is stopped", ActivePlugin.WowIcon, Color.FromArgb(255, 0, 0));
                    }
                    _pluginThread = null;
                    ActivePlugin = null;
                    if (reportToMainWindow)
                    {
                        MainForm.Instance.WowPluginHotkeyChanged();
                    }
                }
                else
                {
                    throw new Exception("ActivePlugin is null!");
                }
            }
        }

        private static void PluginPulse()
        {
            while (_shouldPulse)
            {
                _balancingStopwatch.Restart();
                if (!WoWManager.Hooked || !WoWManager.WoWProcess.IsInGame)
                {
                    Log.Print(String.Format("{0}:{1} :: [{2}] Plugin is stopped: the player isn't active or not in the game", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ActivePlugin.Name));
                    MainForm.Instance.ShowNotifyIconMessage("[" + ActivePlugin.Name + "] Plugin is stopped", "The player isn't active or not in the game", ToolTipIcon.Error);
                    Task.Factory.StartNew(() => StopPlugin(true));
                    return;
                }
                try
                {
                    ActivePlugin.OnPulse();
                }
                catch (Exception ex)
                {
                    Log.Print(String.Format("{0}:{1} :: [{2}] OnPulse error: {3}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ActivePlugin.Name, ex.Message), true);
                }
                int shouldWait = (int) (_intervalBetweenPulses - _balancingStopwatch.ElapsedMilliseconds);
                while (shouldWait > 0 && _shouldPulse)
                {
                    int t = Math.Min(shouldWait, 100);
                    shouldWait -= t;
                    Thread.Sleep(t);
                }
            }
        }

        internal static void LoadPlugins()
        {
            Plugins.Add(new Fishing());
            Log.Print(string.Format("Plugin loaded: {0} {1}", Plugins.Last().Name, Plugins.Last().Version));
            Plugins.Add(new FlagReturner());
            Log.Print(string.Format("Plugin loaded: {0} {1}", Plugins.Last().Name, Plugins.Last().Version));
            Plugins.Add(new GoodsDestroyer());
            Log.Print(string.Format("Plugin loaded: {0} {1}", Plugins.Last().Name, Plugins.Last().Version));
        }

        internal static void LoadPluginsFromDisk()
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
                                Log.Print("Compiler Error: " + error);
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
                                            Log.Print(string.Format("Plugin loaded: {0} {1}", temp.Name, temp.Version));
                                        }
                                        else
                                        {
                                            Log.Print(string.Format("Can't load plugin [{0}]: [Name] is empty", temp.Name));
                                            haveError = true;
                                        }
                                    }
                                    else
                                    {
                                        Log.Print(string.Format("Can't load plugin [{0}]: already loaded", temp.Name));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Print(ex.Message, true);
                }
            }
            if (haveError)
            {
                MainForm.Instance.ShowTaskDialog("Plugin error", "Error has occured during compiling plugins\r\nSome plugins could not be loaded and are disabled", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

    }
}
