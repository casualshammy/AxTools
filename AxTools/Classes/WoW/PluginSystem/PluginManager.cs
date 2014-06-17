using WindowsFormsAero.TaskDialog;
using AxTools.Classes.WoW.Management;
using AxTools.Classes.WoW.PluginSystem.Plugins;
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

        internal static void StartPlugin(IPlugin plugin)
        {
            ActivePlugin = plugin;
            _intervalBetweenPulses = plugin.Interval;
            StartPluginInternal();
        }

        internal static void StopPlugin(bool async = false, bool reportToMainWindow = false)
        {
            if (!async)
            {
                StopPluginInternal();
                ActivePlugin = null;
                if (reportToMainWindow)
                {
                    MainForm.Instance.WowPluginHotkeyChanged();
                }
            }
            else
            {
                Task.Factory.StartNew(() => StopPlugin(false, reportToMainWindow));
            }
        }

        internal static IPlugin ActivePlugin { private set; get; }

        private static Thread _pluginThread;

        private static void StartPluginInternal()
        {
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
            if (Settings.WowPluginsShowIngameNotifications)
            {
                WoWDXInject.ShowOverlayText("Plugin <" + ActivePlugin.Name + "> is started", ActivePlugin.WowIcon, Color.FromArgb(255, 102, 0));
            }
            _pluginThread.Start();
        }

        private static void StopPluginInternal()
        {
            _shouldPulse = false;
            if (!_pluginThread.Join(5000))
            {
                throw new Exception("Can't stop plugin!");
            }
            try
            {
                ActivePlugin.OnStop();
                Log.Print(WoWManager.WoWProcess != null ? String.Format("{0}:{1} :: [{2}] Plugin is stopped", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ActivePlugin.Name) : string.Format("UNKNOWN:null :: [{0}] Plugin is stopped", ActivePlugin.Name));
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("Can't shutdown plugin [{0}]: {1}", ActivePlugin.Name, ex.Message), true);
            }
            if (Settings.WowPluginsShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsInGame)
            {
                WoWDXInject.ShowOverlayText("Plugin <" + ActivePlugin.Name + "> is stopped", ActivePlugin.WowIcon, Color.FromArgb(255, 0, 0));
            }
            _pluginThread = null;
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
                    StopPlugin(true, true);
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
