using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using Components.Forms;
using KeyboardWatcher;
using NLua.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace LuaConsole
{
    internal partial class LuaConsole : BorderedMetroForm, IPlugin3
    {
        private LuaConsole actualWindow;
        private readonly NLua.Lua luaEngine;
        private readonly GameInterface info;
        private readonly LuaConsoleSettings luaConsoleSettings;
        private readonly object luaExecutionLock = new object();
        private SafeTimer safeTimer;
        private string helpInfo = "";
        private bool LuaCancellationRequested;
        private bool ClosingProcessStarted;

        #region IPlugin3 info

        public new string Name => nameof(LuaConsole);

        public bool ConfigAvailable => false;

        public string[] Dependencies => new[] { "LibNavigator" };

        public string Description => "Easy-to-use script platform for World of Warcraft";

        public bool DontCloseOnWowShutdown => true;

        public Image TrayIcon => Resources.PluginImage;

        public Version Version => new Version(1, 0);

        #endregion IPlugin3 info

        #region Consts

        private const string MetroLinkEnableCyclicExecutionTextEnable = "<Enable cyclic execution>";
        private const string MetroLinkEnableCyclicExecutionTextDisable = "<Disable cyclic execution>";
        private const string MetroLinkRunScriptOnceRun = "<Run script once>";
        private const string MetroLinkRunScriptOnceStop = "<Stop script>";
        private const string LUA_CANCELLATION_MSG = "LUA_CANCELLATION_MSG";

        #endregion Consts

        public LuaConsole() { }

        public LuaConsole(GameInterface info)
        {
            InitializeComponent();
            Icon = Resources.PluginIcon;
            luaConsoleSettings = this.LoadSettingsJSON<LuaConsoleSettings>();
            pictureBoxOpenLuaFile.Image = Resources.OpenFile;
            pictureBoxSaveLuaFile.Image = Resources.SaveFile;
            StyleManager.Style = Utilities.MetroColorStyle;
            this.info = info;
            luaEngine = new NLua.Lua();
            SetLuaEnv();
            textBoxLuaCode.SetHighlighting("Lua");
            textBoxLuaCode.Font = Utils.FontIsInstalled("Consolas") ? new Font("Consolas", 8) : new Font("Courier New", 8);
            textBoxLuaCode.Visible = true;
            textBoxLuaCode.Encoding = Encoding.Unicode;
            textBoxTimerHotkey.Text = luaConsoleSettings.TimerHotkey.ToString();
            textBoxTimerHotkey.KeyDown += TextBoxTimerHotkey_KeyDown;
            metroToolTip1.SetToolTip(textBoxTimerHotkey, "Press 'Esc' to clear");
            metroPanelTimerOptions.Visible = false;
            Size = luaConsoleSettings.WindowSize;
            metroLinkEnableCyclicExecution.Text = MetroLinkEnableCyclicExecutionTextEnable;
            metroCheckBoxRandomize.Checked = luaConsoleSettings.TimerRnd;
            metroCheckBoxIgnoreGameState.Checked = luaConsoleSettings.IgnoreGameState;
            metroTextBoxTimerInterval.Text = luaConsoleSettings.TimerInterval.ToString();
            metroToolTip1.SetToolTip(metroLinkRunScriptOnce, "Execute script once");
            metroToolTip1.SetToolTip(metroLinkSettings, "Open settings");
            textBoxLuaCode.Text = string.Join("\r\n", luaConsoleSettings.Code);
            luaConsoleSettings.TimerHotkeyChanged += LuaTimerHotkeyChanged;
            LuaTimerHotkeyChanged(luaConsoleSettings.TimerHotkey);
            HotkeyManager.AddKeys(typeof(LuaConsole).ToString(), luaConsoleSettings.TimerHotkey);
            HotkeyManager.KeyPressed += KeyboardListener2_KeyPressed;
            textBoxLuaCode.TextChanged += TextBoxLuaCode_TextChanged;
            this.LogPrint("Loaded");
        }

        private void SetLuaEnv()
        {
            try
            {
                luaEngine.LoadCLRPackage();
                helpInfo += "CLR package is loaded, using:\r\nimport ('System.Web');\r\nlocal client = WebClient()\r\n\r\n";
                // Keys
                luaEngine.DoString("Keys={};");
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    luaEngine.DoString($"Keys[\"{key.ToString()}\"]={(int)key};");
                }
                helpInfo += "Keys[] is a table containing .Net Windows.Forms.Keys enum values\r\n\r\n";
                // Moving
                luaEngine.RegisterFunction(nameof(Move2D), this, GetType().GetMethod(nameof(Move2D)));
                helpInfo += "void Move2D(table points) - something like { {[\"X\"] = 5673.50244, [\"Y\"] = 4510.01953, [\"Z\"] = 125.027237}, {[\"X\"] = 5680.97363, [\"Y\"] = 4487.585,   [\"Z\"] = 130.122177} }\r\n";
                // game
                luaEngine.RegisterFunction("UseItemByID", info, typeof(GameInterface).GetMethod("UseItemByID"));
                helpInfo += "void UseItemByID(uint id)\r\n";
                luaEngine.RegisterFunction("UseItem", info, typeof(GameInterface).GetMethod("UseItem"));
                helpInfo += "void UseItem(int bagID, int slotID)\r\n";
                luaEngine.RegisterFunction("CastSpellByName", info, typeof(GameInterface).GetMethod("CastSpellByName"));
                helpInfo += "void CastSpellByName(string spellName)\r\n";
                luaEngine.RegisterFunction("SelectDialogOption", info, typeof(GameInterface).GetMethod("SelectDialogOption"));
                helpInfo += "void SelectDialogOption(string gossipText)\r\n";
                luaEngine.RegisterFunction("BuyMerchantItem", info, typeof(GameInterface).GetMethod("BuyMerchantItem"));
                helpInfo += "void BuyMerchantItem(uint itemID, int count)\r\n";
                luaEngine.RegisterFunction("SendToChat", info, typeof(GameInterface).GetMethod("SendToChat"));
                helpInfo += "void SendToChat(string command)\r\n";
                // Info
                luaEngine.RegisterFunction(nameof(IsInGame), this, GetType().GetMethod(nameof(IsInGame)));
                helpInfo += "bool IsInGame()\r\n";
                luaEngine.RegisterFunction(nameof(IsLoadingScreenVisible), this, GetType().GetMethod(nameof(IsLoadingScreenVisible)));
                helpInfo += "bool IsLoadingScreenVisible()\r\n";
                luaEngine.RegisterFunction(nameof(MouseoverGUID), this, GetType().GetMethod(nameof(MouseoverGUID)));
                helpInfo += "string MouseoverGUID()\r\n";
                luaEngine.RegisterFunction(nameof(IsSpellKnown), this, GetType().GetMethod(nameof(IsSpellKnown)));
                helpInfo += "bool IsSpellKnown(double spellID)\r\n";
                luaEngine.RegisterFunction(nameof(IsLooting), this, GetType().GetMethod(nameof(IsLooting)));
                helpInfo += "bool IsLooting()\r\n";
                luaEngine.RegisterFunction(nameof(ZoneID), this, GetType().GetMethod(nameof(ZoneID)));
                helpInfo += "double ZoneID()\r\n";
                luaEngine.RegisterFunction(nameof(ZoneText), this, GetType().GetMethod(nameof(ZoneText)));
                helpInfo += "string ZoneText()\r\n";
                luaEngine.RegisterFunction(nameof(Lua_GetValue), this, GetType().GetMethod(nameof(Lua_GetValue)));
                helpInfo += "string Lua_GetValue(string func)\r\n";
                luaEngine.RegisterFunction(nameof(Lua_IsTrue), this, GetType().GetMethod(nameof(Lua_IsTrue)));
                helpInfo += "bool Lua_IsTrue(string condition)\r\n";
                // Objects
                luaEngine.RegisterFunction(nameof(GetLocalPlayer), this, GetType().GetMethod(nameof(GetLocalPlayer)));
                helpInfo += "uservalue WoWPlayerMe GetLocalPlayer()\r\n";
                luaEngine.RegisterFunction(nameof(GetNpcs), this, GetType().GetMethod(nameof(GetNpcs)));
                helpInfo += "double, uservalue List<WowNpc> GetNpcs()\r\n";
                luaEngine.RegisterFunction(nameof(GetPlayers), this, GetType().GetMethod(nameof(GetPlayers)));
                helpInfo += "double, uservalue List<WowPlayer> GetPlayers()\r\n";
                luaEngine.RegisterFunction(nameof(GetObjects), this, GetType().GetMethod(nameof(GetObjects)));
                helpInfo += "double, uservalue List<WowObject> GetObjects()\r\n";
                luaEngine.RegisterFunction(nameof(GetTargetObject), this, GetType().GetMethod(nameof(GetTargetObject)));
                helpInfo += "uservalue dynamic GetTargetObject()\r\n";
                // utilities
                luaEngine.RegisterFunction(nameof(MsgBox), this, GetType().GetMethod(nameof(MsgBox)));
                helpInfo += "void MsgBox(object text)\r\n";
                luaEngine.RegisterFunction("PressKey", info, info.GetType().GetMethod("PressKey"));
                helpInfo += "void PressKey(int key)\r\n";
                luaEngine.RegisterFunction(nameof(Log), this, GetType().GetMethod(nameof(Log)));
                helpInfo += "void Log(object text)\r\n";
                luaEngine.RegisterFunction(nameof(Wait), this, GetType().GetMethod(nameof(Wait)));
                helpInfo += "void Wait(double ms) - this method contains call to StopExecutionIfCancellationRequested()\r\n";
                luaEngine.RegisterFunction(nameof(NotifyUser), this, GetType().GetMethod(nameof(NotifyUser)));
                helpInfo += "void NotifyUser(object text)\r\n";
                luaEngine.RegisterFunction(nameof(StopExecutionIfCancellationRequested), this, GetType().GetMethod(nameof(StopExecutionIfCancellationRequested)));
                helpInfo += "bool StopExecutionIfCancellationRequested()\r\n";
                // lua lib
                luaEngine.DoString("format=string.format;");
                luaEngine.DoString("if (not table.count) then table.count = function(tbl) local count = 0; for index in pairs(tbl) do count = count+1; end return count; end end");
                luaEngine.DoString("if (not table.first) then table.first = function(tbl) for _, value in pairs(tbl) do return value; end end end");
                helpInfo += "real table.count(tbl)\r\nvalue table.first(tbl)\r\n";
                luaEngine.DoString("if (not GetNearestObjectByName) then GetNearestObjectByName = function(listCount, list, name) local t = { }; for i = 0,  listCount-1 do local object = list[i]; if (object.Name == name) then t[#t+1] = object; end end if (table.count(t) > 0) then local lp = GetLocalPlayer(); table.sort(t, function(a,b) return a.Location:Distance(lp.Location) < b.Location:Distance(lp.Location); end); return table.first(t); end return nil; end end");
                helpInfo += "uservalue WowNpc/WowObject/WowPlayer GetNearestObjectByName(listLength, list, name)\r\n";
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        #region Wrappers

        public void Move2D(NLua.LuaTable points)
        {
            var libNavigator = Utilities.GetReferenceOfPlugin("LibNavigator");
            // 'Convert.ToSingle' - okay, cast to 'float' - not okay
            libNavigator.GoPath(points.Values.Cast<NLua.LuaTable>().Select(l => new WowPoint(Convert.ToSingle(l["X"]), Convert.ToSingle(l["Y"]), Convert.ToSingle(l["Z"]))).ToArray(),
                3f, info);
        }

        public bool IsInGame()
        {
            return info.IsInGame;
        }

        public bool IsLoadingScreenVisible()
        {
            return info.IsLoadingScreen;
        }

        public string MouseoverGUID()
        {
            return info.MouseoverGUID.ToString();
        }

        public bool IsSpellKnown(double spellID)
        {
            return info.IsSpellKnown((uint)spellID);
        }

        public bool IsLooting()
        {
            return info.IsLooting;
        }

        public double ZoneID()
        {
            return info.ZoneID;
        }

        public string ZoneText()
        {
            return info.ZoneText;
        }

        public string Lua_GetValue(string func)
        {
            StopExecutionIfCancellationRequested();
            return info.LuaGetValue(func);
        }

        public bool Lua_IsTrue(string condition)
        {
            StopExecutionIfCancellationRequested();
            return info.LuaIsTrue(condition);
        }

        #endregion Wrappers

        #region Wrappers - Objects

        public WoWPlayerMe GetLocalPlayer()
        {
            return info.GetGameObjects();
        }

        public double GetNpcs(out List<WowNpc> list)
        {
            list = new List<WowNpc>();
            info.GetGameObjects(null, null, list);
            return list.Count;
        }

        public double GetPlayers(out List<WowPlayer> list)
        {
            list = new List<WowPlayer>();
            info.GetGameObjects(null, list);
            return list.Count;
        }

        public double GetObjects(out List<WowObject> list)
        {
            list = new List<WowObject>();
            info.GetGameObjects(list);
            return list.Count;
        }

        public dynamic GetTargetObject()
        {
            var npcs = new List<WowNpc>();
            var players = new List<WowPlayer>();
            var me = info.GetGameObjects(null, players, npcs);
            if (me != null)
            {
                var npc = npcs.FirstOrDefault(l => l.GUID == me.TargetGUID);
                if (npc != null)
                {
                    return npc;
                }
                else
                {
                    var player = players.FirstOrDefault(l => l.GUID == me.TargetGUID);
                    if (player != null)
                    {
                        return player;
                    }
                }
            }
            return null;
        }

        #endregion Wrappers - Objects

        #region Wrappers - Utilities

        public void MsgBox(object text)
        {
            MessageBox.Show(this, text.ToString());
        }

        public void Log(object text)
        {
            this.LogPrint(text.ToString());
        }

        public void Wait(double ms)
        {
            var stopwatch = Stopwatch.StartNew();
            var time = (int)ms;
            while (time > 0)
            {
                Thread.Sleep(Math.Min(time, 100));
                time -= (int)stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();
                StopExecutionIfCancellationRequested();
            }
        }

        public void NotifyUser(object text, bool warning, bool sound)
        {
            this.ShowNotify(text.ToString(), warning, sound);
        }

        public bool StopExecutionIfCancellationRequested()
        {
            if (LuaCancellationRequested)
                throw new OperationCanceledException(LUA_CANCELLATION_MSG);
            return true;
        }

        #endregion Wrappers - Utilities

        private void SwitchTimer()
        {
            InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty);
        }

        private void TimerLuaElapsed()
        {
            if (!info.IsInGame)
            {
                if (!luaConsoleSettings.IgnoreGameState)
                {
                    this.LogPrint("Lua console's timer is stopped: the player isn't active or not in the game");
                    this.ShowNotify("Lua console's timer is stopped:\r\nThe player isn't active or not in the game", true, false);
                    Invoke(new Action(() => InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty)));
                    return;
                }
                return;
            }
            if (luaConsoleSettings.TimerRnd)
            {
                safeTimer.ChangeInterval(luaConsoleSettings.TimerInterval + Utilities.Rnd.Next(-(luaConsoleSettings.TimerInterval / 5), luaConsoleSettings.TimerInterval / 5));
            }
            try
            {
                // We should send commands as byte array because encoding is broken
                lock (luaExecutionLock)
                    luaEngine.DoString(Encoding.UTF8.GetBytes(textBoxLuaCode.Text));
            }
            catch (LuaException ex)
            {
                if (ex.InnerException.Message != LUA_CANCELLATION_MSG)
                {
                    this.ShowNotify($"Lua exception is thrown:\r\n{ex.Message}\r\n{ex.InnerException.Message}", true, true);
                    this.LogPrint($"LuaException in Timer.Elapsed: ({ex.Message}) ({ex.InnerException.Message})");
                    Thread.Sleep(1000);
                }
            }
        }

        private void TextBoxLuaCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                luaEngine.LoadString(Encoding.UTF8.GetBytes(textBoxLuaCode.Text), "textbox");
                labelLuaError.Text = "";
            }
            catch (LuaScriptException ex)
            {
                labelLuaError.Text = ex.Message;
            }
        }

        private void WowModulesFormClosing(object sender, FormClosingEventArgs e)
        {
            var alreadyClosing = ClosingProcessStarted;
            ClosingProcessStarted = true;
            if (safeTimer != null && safeTimer.IsRunning)
            {
                InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty);
            }
            luaConsoleSettings.Code = textBoxLuaCode.Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            luaConsoleSettings.TimerHotkeyChanged -= LuaTimerHotkeyChanged;
            HotkeyManager.RemoveKeys(typeof(LuaConsole).ToString());
            HotkeyManager.KeyPressed -= KeyboardListener2_KeyPressed;
            this.SaveSettingsJSON(luaConsoleSettings);
            this.LogPrint("Closed");
            if (!alreadyClosing)
            {
                Utilities.RemovePluginFromRunning(Name);
            }
        }

        private void PictureBoxOpenLuaFileClick(object sender, EventArgs e)
        {
            Select();
            using (OpenFileDialog p = new OpenFileDialog { Filter = @"Lua file|*.lua", InitialDirectory = this.GetPluginSettingsDir() })
            {
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    var text = File.ReadAllText(p.FileName, Encoding.UTF8);
                    ParseLuaScript(text);
                    textBoxLuaCode.Invalidate(true);
                }
            }
        }

        private void PictureBoxSaveLuaFileClick(object sender, EventArgs e)
        {
            Select();
            using (SaveFileDialog p = new SaveFileDialog { Filter = @"Lua file|*.lua", InitialDirectory = this.GetPluginSettingsDir() })
            {
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllText(p.FileName, $"----- Interval={luaConsoleSettings.TimerInterval}\r\n{textBoxLuaCode.Text}", Encoding.UTF8);
                }
            }
        }

        private void ParseLuaScript(string text)
        {
            var regex = new Regex("----- Interval=(\\d+)\r\n(.+)$", RegexOptions.Singleline);
            var match = regex.Match(text);
            if (match.Success)
            {
                luaConsoleSettings.TimerInterval = Convert.ToInt32(match.Groups[1].Value);
                metroTextBoxTimerInterval.Text = luaConsoleSettings.TimerInterval.ToString();
                textBoxLuaCode.Text = match.Groups[2].Value;
            }
            else
            {
                textBoxLuaCode.Text = text;
            }
        }

        private void LuaConsole_Resize(object sender, EventArgs e)
        {
            luaConsoleSettings.WindowSize = Size;
        }

        private void MetroCheckBoxRandomize_CheckedChanged(object sender, EventArgs e)
        {
            luaConsoleSettings.TimerRnd = metroCheckBoxRandomize.Checked;
        }

        private void MetroCheckBoxIgnoreGameState_CheckedChanged(object sender, EventArgs e)
        {
            luaConsoleSettings.IgnoreGameState = metroCheckBoxIgnoreGameState.Checked;
        }

        private void MetroTextBoxTimerInterval_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(metroTextBoxTimerInterval.Text, out luaConsoleSettings.TimerInterval);
        }

        private void LuaTimerHotkeyChanged(KeyExt key)
        {
            metroToolTip1.SetToolTip(metroLinkEnableCyclicExecution, $"Enable/disable cyclic script execution\r\nHotkey: [{key}]");
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

        private void MetroLinkSettings_Click(object sender, EventArgs e)
        {
            metroPanelTimerOptions.Visible = !metroPanelTimerOptions.Visible;
        }

        private void MetroLinkRunScriptOnce_Click(object sender, EventArgs e)
        {
            if (metroLinkRunScriptOnce.Text == MetroLinkRunScriptOnceStop)
            {
                metroLinkRunScriptOnce.Enabled = false;
                LuaCancellationRequested = true;
            }
            else
            {
                if (textBoxLuaCode.Text.Trim().Length != 0)
                {
                    if (!info.IsInGame)
                    {
                        new TaskDialog("Error!", nameof(AxTools), "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                        return;
                    }
                    var stopwatch = Stopwatch.StartNew();
                    metroLinkRunScriptOnce.Text = MetroLinkRunScriptOnceStop;
                    Task.Run(() =>
                    {
                        try
                        {
                            LuaCancellationRequested = false;
                            // We should send commands as byte array because encoding is broken
                            lock (luaExecutionLock)
                                luaEngine.DoString(Encoding.UTF8.GetBytes(textBoxLuaCode.Text));
                        }
                        catch (LuaException ex)
                        {
                            if (ex.InnerException.Message != LUA_CANCELLATION_MSG)
                                MsgBox($"{ex.Message}\r\n{ex.InnerException.Message}");
                        }
                    }).ContinueWith(l =>
                    {
                        PostInvoke(delegate
                        {
                            labelRequestTime.Text = $"{stopwatch.ElapsedMilliseconds}ms";
                            labelRequestTime.Visible = true;
                            metroLinkRunScriptOnce.Text = MetroLinkRunScriptOnceRun;
                            metroLinkRunScriptOnce.Enabled = true;
                        });
                    });
                }
            }
        }

        private void MetroLinkEnableCyclicExecution_Click(object sender, EventArgs e)
        {
            if (safeTimer != null && safeTimer.IsRunning)
            {
                LuaCancellationRequested = true;
                safeTimer?.Dispose();
                this.LogPrint("Timer disabled");
                NotifyUser("Timer is stopped", false, false);
                SetupTimerControls(false);
            }
            else
            {
                labelRequestTime.Visible = false;
                if (!info.IsInGame)
                {
                    new TaskDialog("Error!", nameof(AxTools), "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                    return;
                }
                if (!int.TryParse(metroTextBoxTimerInterval.Text, out luaConsoleSettings.TimerInterval) || luaConsoleSettings.TimerInterval < 50)
                {
                    TaskDialog.Show("Incorrect input!", nameof(AxTools), "Interval must be a number more or equal 50", TaskDialogButton.OK, TaskDialogIcon.Warning);
                    return;
                }
                if (textBoxLuaCode.Text.Trim().Length == 0)
                {
                    new TaskDialog("Error!", nameof(AxTools), "Script is empty", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                    return;
                }
                SetupTimerControls(true);
                LuaCancellationRequested = false;
                safeTimer = this.CreateTimer(luaConsoleSettings.TimerInterval, info, TimerLuaElapsed);
                safeTimer.Start();
                NotifyUser("Timer is started", false, false);
                this.LogPrint("Timer enabled");
            }
        }

        private void KeyboardListener2_KeyPressed(KeyExt obj)
        {
            if (obj == luaConsoleSettings.TimerHotkey)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    if (Utilities.WoWWindowHandle(info) == NativeMethods.GetForegroundWindow())
                    {
                        SwitchTimer();
                    }
                });
            }
        }

        private void TextBoxTimerHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    TextBoxTimerHotkey_KeyDown(sender, new KeyEventArgs(Keys.None));
                }
                else
                {
                    var key = new KeyExt(e.KeyCode, e.Alt, e.Shift, e.Control);
                    textBoxTimerHotkey.Text = key.ToString();
                    HotkeyManager.RemoveKeys(typeof(LuaConsole).ToString());
                    luaConsoleSettings.TimerHotkey = key;
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                        HotkeyManager.AddKeys(typeof(LuaConsole).ToString(), key);
                    });
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void LinkInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show(helpInfo);
        }

        public void OnConfig()
        {
            throw new InvalidOperationException();
        }

        public void OnStart(GameInterface game)
        {
            Utilities.InvokeInGUIThread(delegate
            {
                actualWindow = new LuaConsole(game);
                actualWindow.Show();
            });
        }

        public void OnStop()
        {
            Utilities.InvokeInGUIThread(delegate
            {
                if (actualWindow != null)
                {
                    var alreadyClosing = actualWindow.ClosingProcessStarted;
                    actualWindow.ClosingProcessStarted = true;
                    if (!alreadyClosing)
                    {
                        actualWindow.Close();
                    }
                }
            });
        }
    }
}