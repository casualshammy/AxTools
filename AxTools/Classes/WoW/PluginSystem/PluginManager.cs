using AxTools.Classes.WoW.Plugins;
using AxTools.Forms;
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

namespace AxTools.Classes.WoW.PluginSystem
{
    internal static class PluginManager
    {
        internal static List<IPlugin> Plugins = new List<IPlugin>();

        private static bool _shouldPulse;

        private static int _intervalBetweenPulses;

        private static Stopwatch _balancingStopwatch;

        private static IPlugin _activePlugin;
        internal static IPlugin ActivePlugin
        {
            get
            {
                return _activePlugin;
            }
            set
            {
                if (value == null)
                {
                    StopPlugin();
                    _activePlugin = null;
                }
                else
                {
                    _activePlugin = value;
                    _intervalBetweenPulses = value.Interval;
                    StartPlugin();
                }
            }
        }

        private static Thread _pluginThread;

        private static void StartPlugin()
        {
            _shouldPulse = true;
            _pluginThread = new Thread(PluginPulse) {IsBackground = true};
            try
            {
                _activePlugin.OnStart();
                Log.Print(String.Format("{0}:{1} :: [{2}] Plugin is started", WoW.WProc.ProcessName, WoW.WProc.ProcessID, _activePlugin.Name));
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("Plugin OnStart error [{0}]: {1}", _activePlugin.Name, ex.Message), true);
            }
            _balancingStopwatch = new Stopwatch();
            if (Settings.WowPluginsShowIngameNotifications)
            {
                WoW.ShowOverlayText("Plugin <" + _activePlugin.Name + "> is started", _activePlugin.WowIcon, Color.FromArgb(255, 102, 0));
            }
            _pluginThread.Start();
        }

        private static void StopPlugin()
        {
            _shouldPulse = false;
            if (!_pluginThread.Join(5000))
            {
                throw new Exception("Can't stop plugin!");
            }
            try
            {
                _activePlugin.OnStop();
                Log.Print(WoW.WProc != null ? String.Format("{0}:{1} :: [{2}] Plugin is stopped", WoW.WProc.ProcessName, WoW.WProc.ProcessID, _activePlugin.Name) : string.Format("UNKNOWN:null :: [{0}] Plugin is stopped", _activePlugin.Name));
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("Can't shutdown plugin [{0}]: {1}", _activePlugin.Name, ex.Message), true);
            }
            if (Settings.WowPluginsShowIngameNotifications && WoW.Hooked && WoW.WProc != null && WoW.WProc.IsInGame)
            {
                WoW.ShowOverlayText("Plugin <" + _activePlugin.Name + "> is stopped", _activePlugin.WowIcon, Color.FromArgb(255, 0, 0));
            }
            _pluginThread = null;
        }

        internal static void StopPluginFromPlugin()
        {
            Task.Factory.StartNew(() =>
            {
                ActivePlugin = null;
            }).ContinueWith(
                l => MainForm.Instance.WowPluginHotkeyChanged()
                );
        }

        private static void PluginPulse()
        {
            while (_shouldPulse)
            {
                _balancingStopwatch.Restart();
                if (!WoW.Hooked || !WoW.WProc.IsInGame)
                {
                    Log.Print(String.Format("{0}:{1} :: [{2}] Plugin is stopped: the player isn't active or not in the game", WoW.WProc.ProcessName, WoW.WProc.ProcessID, _activePlugin.Name));
                    MainForm.Instance.ShowNotifyIconMessage("[" + _activePlugin.Name + "] Plugin is stopped", "The player isn't active or not in the game", ToolTipIcon.Error);
                    StopPluginFromPlugin();
                    return;
                }
                try
                {
                    _activePlugin.OnPulse();
                }
                catch (Exception ex)
                {
                    Log.Print(String.Format("{0}:{1} :: [{2}] OnPulse error: {3}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, _activePlugin.Name, ex.Message), true);
                }
                int shouldWait = (int) (_intervalBetweenPulses - _balancingStopwatch.ElapsedMilliseconds);
                if (shouldWait > 0 && _shouldPulse)
                {
                    Thread.Sleep(shouldWait);
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
            foreach (string directory in directories)
            {
                try
                {
                    var cc = new CodeCompiler(directory);
                    CompilerResults results = cc.Compile();

                    if (results != null)
                    {
                        if (results.Errors.HasErrors)
                        {
                            foreach (object error in results.Errors)
                            {
                                Log.Print("Compiler Error: " + error, true);
                            }
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
                                        if (!string.IsNullOrWhiteSpace(temp.TrayDescription))
                                        {
                                            Plugins.Add(temp);
                                            Log.Print(string.Format("Plugin loaded: {0} {1}", temp.Name, temp.Version));
                                        }
                                        else
                                        {
                                            Log.Print(string.Format("Can't load plugin [{0}]: TrayDescription is empty", temp.Name));
                                        }
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
        }

    }
}
