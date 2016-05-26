using Components;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WoW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.Services;
using AxTools.WinAPI;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem.API;
using MyMemory;
using MyMemory.Hooks;
using Settings = AxTools.Helpers.Settings;
using Timer = System.Timers.Timer;

namespace AxTools.Forms
{
    internal partial class LuaConsole : BorderedMetroForm, IWoWModule
    {
        private readonly Settings settings = Settings.Instance;
        internal static bool TimerEnabled { get; private set; }
        private readonly Timer timerLua = new Timer(1000);
        private const string MetroLinkEnableCyclicExecutionTextEnable = "<Enable cyclic execution>";
        private const string MetroLinkEnableCyclicExecutionTextDisable = "<Disable cyclic execution>";
        private static WowProcess _wowProcess;
        private static RemoteProcess _remoteProcess;
        private static HookJmp _hookJmp;
        private static readonly object HookLock = new object();

        public LuaConsole()
        {
            InitializeComponent();
            StyleManager.Style = Settings.Instance.StyleColor;
            AccessibleName = "Lua";
            timerLua.Elapsed += TimerLuaElapsed;
            Icon = Resources.AppIcon;
            textBoxLuaCode.SetHighlighting("Lua");
            textBoxLuaCode.Font = Utils.FontIsInstalled("Consolas") ? new Font("Consolas", 8) : new Font("Courier New", 8);
            textBoxLuaCode.Visible = true;
            metroPanelTimerOptions.Visible = false;
            Size = settings.WoWLuaConsoleWindowSize;
            metroLinkEnableCyclicExecution.Text = MetroLinkEnableCyclicExecutionTextEnable;
            metroCheckBoxRandomize.Checked = settings.WoWLuaConsoleTimerRnd;
            metroCheckBoxIgnoreGameState.Checked = settings.WoWLuaConsoleIgnoreGameState;
            metroTextBoxTimerInterval.Text = settings.WoWLuaConsoleTimerInterval.ToString();
            metroCheckBoxShowIngameNotifications.Checked = settings.WoWLuaConsoleShowIngameNotifications;
            metroToolTip1.SetToolTip(metroLinkRunScriptOnce, "Execute script once");
            metroToolTip1.SetToolTip(metroLinkSettings, "Open settings");
            textBoxLuaCode.Text = settings.WoWLuaConsoleLastText;
            settings.LuaTimerHotkeyChanged += LuaTimerHotkeyChanged;
            LuaTimerHotkeyChanged(settings.LuaTimerHotkey);
            HotkeyManager.AddKeys(typeof(LuaConsole).ToString(), settings.LuaTimerHotkey);
            HotkeyManager.KeyPressed += KeyboardListener2_KeyPressed;
            if (!InitializeHook())
            {
                PostInvoke(() =>
                {
                    Notify.TrayPopup("Hooking is failed", "Lua console will be closed", NotifyUserType.Error, true);
                    Close();
                });
            }
            Log.Info(string.Format("{0}:{1} :: [Lua console] Loaded", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
        }

        private bool InitializeHook()
        {
            _wowProcess = WoWManager.WoWProcess;
            byte[] hookOriginalBytes = _wowProcess.Memory.ReadBytes(_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, WowBuildInfoX64.HookPattern.Length);
            if (hookOriginalBytes.SequenceEqual(WowBuildInfoX64.HookPattern))
            {
                lock (HookLock)
                {
                    _remoteProcess = new RemoteProcess((uint)_wowProcess.ProcessID);
                    _hookJmp = _remoteProcess.HooksManager.CreateJmpHook(_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, WowBuildInfoX64.HookPattern.Length);
                }
                Log.Info(string.Format("{0} [WoW hook] Signature is valid, address: 0x{1:X}", _wowProcess, (_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render).ToInt64()));
                return true;
            }
            Log.Error(string.Format("{0} [WoW hook] Hook point has invalid signature, bytes: {1}", _wowProcess, BitConverter.ToString(hookOriginalBytes)));
            return false;
        }

        private void RemoveHook()
        {
            if (!_wowProcess.Memory.Process.HasExited)
            {
                lock (HookLock)
                {
                    if (_remoteProcess != null)
                    {
                        _remoteProcess.Dispose();
                    }
                }
            }
        }

        private void Lua_DoString(string command)
        {
            byte[] commandBytes = Encoding.UTF8.GetBytes(command);
            IntPtr cmdAddr = _remoteProcess.MemoryManager.AllocateRawMemory((uint)(commandBytes.Length + 1));
            _remoteProcess.MemoryManager.WriteBytes(cmdAddr, commandBytes);
            string[] code =
            {
                "sub rsp, 0x20",
                "mov rcx, 0x" + cmdAddr.ToInt64().ToString("X"),
                "mov rdx, 0x" + cmdAddr.ToInt64().ToString("X"),
                "mov r8, 0x0",
                "mov rax, 0x" + (_wowProcess.Memory.ImageBase + WowBuildInfoX64.FrameScript_ExecuteBuffer).ToInt64().ToString("X"),
                "call rax",
                "add rsp, 0x20",
                "retn"
            };
            HookApplyExecuteRemove(code);
            _remoteProcess.MemoryManager.FreeRawMemory(cmdAddr);
        }

        private void HookApplyExecuteRemove(string[] asm)
        {
            lock (HookLock)
            {
                if (_wowProcess.IsMinimized)
                {
                    Notify.Balloon("Attention!", "AxTools is stuck because it can't interact with minimized WoW client. Please activate WoW window!", NotifyUserType.Warn, true);
                }
                _hookJmp.Apply();
                _hookJmp.Executor.Execute<long>(asm, true);
                _hookJmp.Remove();
            }
        }

        private void SwitchTimer()
        {
            InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty);
        }

        private void TimerLuaElapsed(object sender, ElapsedEventArgs e)
        {
            if (!WoWManager.Hooked || !GameFunctions.IsInGame)
            {
                if (!settings.WoWLuaConsoleIgnoreGameState)
                {
                    Log.Info(string.Format("{0}:{1} :: Lua console's timer is stopped: the player isn't active or not in the game", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
                    Notify.Balloon("Lua console's timer is stopped", "The player isn't active or not in the game", NotifyUserType.Warn, false);
                    Invoke(new Action(() => InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty)));
                    return;
                }
                return;
            }
            if (settings.WoWLuaConsoleTimerRnd)
            {
                timerLua.Interval = settings.WoWLuaConsoleTimerInterval + Utils.Rnd.Next(-(settings.WoWLuaConsoleTimerInterval / 5), settings.WoWLuaConsoleTimerInterval / 5);
            }
            Lua_DoString(textBoxLuaCode.Text);
        }

        private void ButtonDumpClick(object sender, EventArgs e)
        {
            //Timer __timer = new Timer(50);
            //__timer.Elapsed += delegate
            //{
            //    //IntPtr playerPtr = WoWManager.WoWProcess.Memory.Read<IntPtr>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.PlayerPtr);
            //    textBoxLuaCode.Text = GameFunctions.IsInGame ? GameFunctions.Lua_GetFunctionReturn("tostring(GetTime())") : "NotInGame";
            //};
            //__timer.Start();

            Log.Info("Dump START");
            List<WowPlayer> wowUnits = new List<WowPlayer>();
            List<WowObject> wowObjects = new List<WowObject>();
            List<WowNpc> wowNpcs = new List<WowNpc>();
            WoWPlayerMe localPlayer;
            try
            {
                localPlayer = ObjectMgr.Pulse(wowObjects, wowUnits, wowNpcs);
                Log.Info("Dump OK");
            }
            catch (Exception ex)
            {
                Log.Error("Dump error: " + ex.Message);
                return;
            }
            Log.Info("Local player---------------------------------------");
            Log.Info(string.Format("GUID: 0x{0}; Address: 0x{1:X}; Location: {2}; ZoneID: {3}; ZoneName: {4}; Realm: {5}; GUID bytes: {6}; IsLooting: {7}; Name: {8}",
                            localPlayer.GUID, localPlayer.Address.ToInt64(), localPlayer.Location, GameFunctions.ZoneID,
                            GameFunctions.ZoneText, "N/A", BitConverter.ToString(WoWManager.WoWProcess.Memory.ReadBytes(localPlayer.Address + WowBuildInfoX64.ObjectGUID, 16)), GameFunctions.IsLooting, localPlayer.Name));
            Log.Info("----Local player buffs----");
            foreach (string info in localPlayer.Auras.AsParallel().Select(l => string.Format("ID: {0}; Name: {1}; Stack: {2}; TimeLeft: {3}; OwnerGUID: {4}", l.SpellId, Wowhead.GetSpellInfo(l.SpellId).Name, l.Stack, l.TimeLeftInMs, l.OwnerGUID)))
            {
                Log.Info("\t" + info);
            }
            Log.Info("----Mouseover----");
            Log.Info(string.Format("\tGUID: {0}", WoWManager.WoWProcess.Memory.Read<WoWGUID>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.MouseoverGUID)));
            Log.Info("----Inventory slots----");
            foreach (WoWItem item in localPlayer.Inventory.AsParallel())
            {
                Log.Info(string.Format("\tID: {0}; Name: {1}; StackCount: {2}; Contained in: {3}; Enchant: {4}", item.EntryID, item.Name, item.StackSize, item.ContainedIn, item.Enchant));
            }
            Log.Info("----Items in bags----");
            foreach (WoWItem item in localPlayer.ItemsInBags)
            {
                Log.Info(string.Format("\tID: {0}; GUID: {7}; Name: {1}; StackCount: {2}; Contained in: {3}; Enchant: {4}; BagID, SlotID: {5} {6}", item.EntryID, item.Name, item.StackSize, item.ContainedIn, item.Enchant,
                    item.BagID, item.SlotID, item.GUID));
            }
            Log.Info("Objects-----------------------------------------");
            foreach (WowObject i in wowObjects)
            {
                Log.Info(string.Format("{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; OwnerGUID: 0x{4}; Address: 0x{5:X}; EntryID: {6}", i.Name, i.GUID, i.Location, (int) i.Location.Distance(localPlayer.Location), i.OwnerGUID,
                    i.Address.ToInt64(), i.EntryID));
            }
            Log.Info("Npcs-----------------------------------------");
            foreach (WowNpc i in wowNpcs)
            {
                Log.Info(string.Format("{0}; Location: {1}; Distance: {2}; HP:{3}; MaxHP:{4}; Address:0x{5:X}; GUID:0x{6}; EntryID: {7}", i.Name, i.Location,
                    (int) i.Location.Distance(localPlayer.Location), i.Health, i.HealthMax, i.Address.ToInt64(), i.GUID, i.EntryID));
            }
            Log.Info("Players-----------------------------------------");
            foreach (WowPlayer i in wowUnits)
            {
                Log.Info(string.Format(
                    "{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; Address:{4:X}; Class:{5}; Level:{6}; HP:{7}; MaxHP:{8}; TargetGUID: 0x{9}; IsAlliance:{10}; Auras: {{ {11} }}",
                    i.Name, i.GUID, i.Location, (int)i.Location.Distance(localPlayer.Location), i.Address.ToInt64(), i.Class, i.Level, i.Health, i.HealthMax,
                    i.TargetGUID, i.Faction, string.Join(",", i.Auras.Select(l => l.Name + "::" + l.Stack + "::" + l.TimeLeftInMs + "::" + l.OwnerGUID.ToString()))));
            }
        }
        
        private void WowModulesFormClosing(object sender, FormClosingEventArgs e)
        {
            if (timerLua.Enabled)
            {
                InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty);
            }
            settings.WoWLuaConsoleLastText = textBoxLuaCode.Text;
            settings.LuaTimerHotkeyChanged -= LuaTimerHotkeyChanged;
            HotkeyManager.RemoveKeys(typeof(LuaConsole).ToString());
            HotkeyManager.KeyPressed -= KeyboardListener2_KeyPressed;
            RemoveHook();
            Log.Info(string.Format("{0}:{1} :: [Lua console] Closed", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
        }

        private void PictureBoxOpenLuaFileClick(object sender, EventArgs e)
        {
            Select();
            AppFolders.CreateUserfilesDir();
            using (OpenFileDialog p = new OpenFileDialog { Filter = @"Lua file|*.lua", InitialDirectory = Globals.UserfilesPath })
            {
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    string text = File.ReadAllText(p.FileName, Encoding.UTF8);
                    ParseLuaScript(text);
                    textBoxLuaCode.Invalidate(true);
                }
            }
        }

        private void PictureBoxSaveLuaFileClick(object sender, EventArgs e)
        {
            Select();
            AppFolders.CreateUserfilesDir();
            using (SaveFileDialog p = new SaveFileDialog { Filter = @"Lua file|*.lua", InitialDirectory = Globals.UserfilesPath })
            {
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllText(p.FileName, string.Format("----- Interval={0}\r\n{1}", settings.WoWLuaConsoleTimerInterval, textBoxLuaCode.Text), Encoding.UTF8);
                }
            }
        }

        private void ParseLuaScript(string text)
        {
            if (text.Contains("### Interval=") || text.Contains("----- Interval="))
            {
                using (StringReader cStringReader = new StringReader(text))
                {
                    string readLine = cStringReader.ReadLine();
                    if (readLine != null)
                    {
                        settings.WoWLuaConsoleTimerInterval = Convert.ToInt32(readLine.Split('=')[1]);
                        metroTextBoxTimerInterval.Text = settings.WoWLuaConsoleTimerInterval.ToString();
                    }
                    textBoxLuaCode.Text = cStringReader.ReadToEnd();
                }
            }
            else
            {
                textBoxLuaCode.Text = text;
            }
        }

        private void LuaConsole_Resize(object sender, EventArgs e)
        {
            settings.WoWLuaConsoleWindowSize = Size;
        }

        private void metroCheckBoxRandomize_CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWLuaConsoleTimerRnd = metroCheckBoxRandomize.Checked;
        }

        private void metroCheckBoxIgnoreGameState_CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWLuaConsoleIgnoreGameState = metroCheckBoxIgnoreGameState.Checked;
        }

        private void metroCheckBoxShowIngameNotifications_CheckedChanged(object sender, EventArgs e)
        {
            settings.WoWLuaConsoleShowIngameNotifications = metroCheckBoxShowIngameNotifications.Checked;
        }

        private void metroTextBoxTimerInterval_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(metroTextBoxTimerInterval.Text, out settings.WoWLuaConsoleTimerInterval);
        }

        private void LuaTimerHotkeyChanged(Keys key)
        {
            metroToolTip1.SetToolTip(metroLinkEnableCyclicExecution, string.Format("Enable/disable cyclic script execution\r\nHotkey: [{0}]", key));
        }

        private void SetupTimerControls(bool timerActive)
        {
            if (timerActive)
            {
                textBoxLuaCode.IsReadOnly = true;
                metroLinkEnableCyclicExecution.Text = MetroLinkEnableCyclicExecutionTextDisable;
                metroLinkSettings.Enabled = false;
                metroPanelTimerOptions.Visible = false;
            }
            else
            {
                textBoxLuaCode.IsReadOnly = false;
                metroLinkEnableCyclicExecution.Text = MetroLinkEnableCyclicExecutionTextEnable;
                metroLinkSettings.Enabled = true;
            }
        }

        private void metroLinkSettings_Click(object sender, EventArgs e)
        {
            metroPanelTimerOptions.Visible = !metroPanelTimerOptions.Visible;
        }

        private void metroLinkRunScriptOnce_Click(object sender, EventArgs e)
        {
            if (textBoxLuaCode.Text.Trim().Length != 0)
            {
                if (!WoWManager.Hooked || !GameFunctions.IsInGame)
                {
                    new TaskDialog("Error!", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                    return;
                }
                Stopwatch stopwatch = Stopwatch.StartNew();
                Lua_DoString(textBoxLuaCode.Text);
                labelRequestTime.Text = string.Format("{0}ms", stopwatch.ElapsedMilliseconds);
                labelRequestTime.Visible = true;
            }
        }

        private void metroLinkEnableCyclicExecution_Click(object sender, EventArgs e)
        {
            if (TimerEnabled)
            {
                timerLua.Enabled = false;
                Log.Info(WoWManager.WoWProcess != null
                              ? string.Format("{0}:{1} :: [Lua console] Lua timer disabled", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID)
                              : "UNKNOWN:null :: Lua timer disabled");
                if (settings.WoWLuaConsoleShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && GameFunctions.IsInGame)
                {
                    Notify.Balloon("LuaConsole", "Timer is stopped", NotifyUserType.Info, false);
                }
                TimerEnabled = false;
                SetupTimerControls(false);
            }
            else
            {
                labelRequestTime.Visible = false;
                if (!WoWManager.Hooked || !GameFunctions.IsInGame)
                {
                    new TaskDialog("Error!", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                    return;
                }
                if (!int.TryParse(metroTextBoxTimerInterval.Text, out settings.WoWLuaConsoleTimerInterval) || settings.WoWLuaConsoleTimerInterval < 50)
                {
                    TaskDialog.Show("Incorrect input!", "AxTools", "Interval must be a number more or equal 50", TaskDialogButton.OK, TaskDialogIcon.Warning);
                    return;
                }
                if (textBoxLuaCode.Text.Trim().Length == 0)
                {
                    new TaskDialog("Error!", "AxTools", "Script is empty", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                    return;
                }
                timerLua.Interval = settings.WoWLuaConsoleTimerInterval;
                TimerEnabled = true;
                SetupTimerControls(true);
                timerLua.Enabled = true;
                if (settings.WoWLuaConsoleShowIngameNotifications)
                {
                    Notify.Balloon("LuaConsole", "Timer is started", NotifyUserType.Info, false);
                }
                Log.Info(string.Format("{0}:{1} :: [Lua console] Lua timer enabled", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
            }
        }

        private void KeyboardListener2_KeyPressed(Keys obj)
        {
            if (obj == settings.LuaTimerHotkey)
            {
                BeginInvoke((MethodInvoker) delegate
                {
                    if (WoWProcessManager.List.Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
                    {
                        SwitchTimer();
                    }
                });
            }
        }

    }
}