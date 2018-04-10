using AxTools.Forms.Helpers;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WinAPI;
using AxTools.WoW;
using AxTools.WoW.Internals;
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
using Settings2 = AxTools.Helpers.Settings2;

namespace AxTools.Forms
{
    internal partial class LuaConsole : BorderedMetroForm, IWoWModule
    {

        private NLua.Lua luaEngine;
        private readonly WowProcess process;
        private readonly GameInterface info;
        private readonly LuaConsoleSettings luaConsoleSettings = LuaConsoleSettings.Instance;
        private readonly Log2 log;
        private readonly object luaExecutionLock = new object();
        private WoW.Helpers.SafeTimer safeTimer;
        private string helpInfo = "";
        private readonly Settings2 settings = Settings2.Instance;
        public int ProcessID { get => process.ProcessID; }
        
        private const string MetroLinkEnableCyclicExecutionTextEnable = "<Enable cyclic execution>";
        private const string MetroLinkEnableCyclicExecutionTextDisable = "<Disable cyclic execution>";

        #region Images

        private const string OpenFile = "iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAABVUlEQVRoQ+2Yzw7BQBDGW44IXgKP4OwuIt6FK0fv4iDO4swbiIcgEm7+fZto0q5N2+1M26yMZE52vp3fzLdtur7n+M93vH5PAMqeoExAJkD" +
            "swN9aqIvGLBEDRMOySQusn1vmZF5umkAHagdEK7Oq5xUGYQJYofAJofggtRAIE8AZFbQZAJRE7hAmgAc2rjIBcMjcIbJDTBFHXdAE8MSiCsfOzBpX6PV1CJcAVD82iFG4Ma4B3PTHehqAsl92b82KkXoEgPmgmuSsJ/CCSngyzllIAJhtJRaSM0" +
            "C0lLWFYhOIxWRJFwDnzoBYKIvRY3LkDMgZIFpKLCQWKtpC6sO5Rtw0r/RUH/Xq6mKYVwVE3TXyx0nXKj0s2COaxM240y/fi61TEoD6X0EE1+t17kos9ZRttogZIlK80in7CWPJ8rtcAMgtJArIBIgNJKfLBMgtJAo4P4EPa2dGMSGVYO4AAAAASUVORK5CYII=";

        private const string SaveFile = "iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAB60lEQVRoQ+2ZPUsEMRCG7xo/ULHXP2ChIqhgbacgYq+1WBwiFrbaaSNiIf4A7UUQG3+AH2ih2NiKtSAiWuk74MIRdrOTTJJLIAcvd7c3k7z" +
            "PTDYcm2Yj8Vczcf+NDNDpDuYOxN6BERjchWahAc9mdzD+tukcuiVE5q+hQdNBBfHGEDqAMxhZFJixTTWC0AF8BFg2VZBsCB3ArzK66x1LHV+FYUHEDEBAtRCxA9RCpACghUgFgCBKvWYA280eeXW7kDp07oCg2E5SxR1w4kIwSAYQFM9Jau6Akz" +
            "IKBskdEBTPSWqwDnzD7hF0Cj3/Wx/F+zK0BnVb4gQBeIW5uTbjqtcxXLiEhi0gvANQ5ac05gvP4/hwB3UZQngH2IehTaapA8StM2OLMO8Ak5jpgWmKOkVdMHl5B+iBmx+mo17EfTFjg3UgeQBaFvfMqk4j7pYZG6wDdGNuME0dIq7FjA0GQOufK" +
            "vtUY2wCv99A0W2j5PsNmoceKyDI/AU0ZFh9Cve+CxWeqBPH0AnU/ldiBd9XLSpvvYQ+kdlnUSkfKeSl9HxC91zoHEkLPtxYjEmP+pfK8mI74Cjz+I6LM9CLKQDF0ynNHkRHTP0WlZOk0LK5graqzFfe2ZJZQ+e6PrQI7T+f1AevuDphXkKdbsEfAZhbMfflGqAAAAAASUVORK5CYII=";


        #endregion

        public LuaConsole(WowProcess proc)
        {
            InitializeComponent();
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(OpenFile)))
            {
                pictureBoxOpenLuaFile.Image = Image.FromStream(memoryStream);
            }
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(SaveFile)))
            {
                pictureBoxSaveLuaFile.Image = Image.FromStream(memoryStream);
            }
            log = new Log2($"LuaConsole - {proc.ProcessID}");
            StyleManager.Style = Settings2.Instance.StyleColor;
            Icon = Resources.AppIcon;
            process = proc;
            info = new GameInterface(proc);
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
            log.Info("Loaded");
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
                luaEngine.RegisterFunction("Move2D", this, GetType().GetMethod("Move2D"));
                helpInfo += "void Move2D(double x, double y, double z)\r\n";
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
                luaEngine.RegisterFunction("IsInGame", this, GetType().GetMethod("IsInGame"));
                helpInfo += "bool IsInGame()\r\n";
                luaEngine.RegisterFunction("IsLoadingScreenVisible", this, GetType().GetMethod("IsLoadingScreenVisible"));
                helpInfo += "bool IsLoadingScreenVisible()\r\n";
                luaEngine.RegisterFunction("MouseoverGUID", this, GetType().GetMethod("MouseoverGUID"));
                helpInfo += "string MouseoverGUID()\r\n";
                luaEngine.RegisterFunction("IsSpellKnown", this, GetType().GetMethod("IsSpellKnown"));
                helpInfo += "bool IsSpellKnown(double spellID)\r\n";
                luaEngine.RegisterFunction("IsLooting", this, GetType().GetMethod("IsLooting"));
                helpInfo += "bool IsLooting()\r\n";
                luaEngine.RegisterFunction("ZoneID", this, GetType().GetMethod("ZoneID"));
                helpInfo += "double ZoneID()\r\n";
                luaEngine.RegisterFunction("ZoneText", this, GetType().GetMethod("ZoneText"));
                helpInfo += "string ZoneText()\r\n";
                luaEngine.RegisterFunction("Lua_GetValue", this, GetType().GetMethod("Lua_GetValue"));
                helpInfo += "string Lua_GetValue(string func)\r\n";
                luaEngine.RegisterFunction("Lua_IsTrue", this, GetType().GetMethod("Lua_IsTrue"));
                helpInfo += "bool Lua_IsTrue(string condition)\r\n";
                // Objects
                luaEngine.RegisterFunction("GetLocalPlayer", this, GetType().GetMethod("GetLocalPlayer"));
                helpInfo += "uservalue WoWPlayerMe GetLocalPlayer()\r\n";
                luaEngine.RegisterFunction("GetNpcs", this, GetType().GetMethod("GetNpcs"));
                helpInfo += "double, uservalue List<WowNpc> GetNpcs()\r\n";
                luaEngine.RegisterFunction("GetPlayers", this, GetType().GetMethod("GetPlayers"));
                helpInfo += "double, uservalue List<WowPlayer> GetPlayers()\r\n";
                luaEngine.RegisterFunction("GetObjects", this, GetType().GetMethod("GetObjects"));
                helpInfo += "double, uservalue List<WowObject> GetObjects()\r\n";
                luaEngine.RegisterFunction("GetTargetObject", this, GetType().GetMethod("GetTargetObject"));
                helpInfo += "uservalue dynamic GetTargetObject()\r\n";
                // utils
                luaEngine.RegisterFunction("MsgBox", this, GetType().GetMethod("MsgBox"));
                helpInfo += "viod MsgBox(object text)\r\n";
                luaEngine.RegisterFunction("PressKey", this, GetType().GetMethod("PressKey"));
                helpInfo += "void PressKey(int key)\r\n";
                luaEngine.RegisterFunction("Log", this, GetType().GetMethod("Log"));
                helpInfo += "void Log(object text)\r\n";
                // lua lib
                luaEngine.DoString("format=string.format;");
                luaEngine.DoString("if (not table.count) then table.count = function(tbl) local count = 0; for index in pairs(tbl) do count = count+1; end return count; end end");
                luaEngine.DoString("if (not table.first) then table.first = function(tbl) for _, value in pairs(tbl) do return value; end end end");
                helpInfo += "real table.count(tbl)\r\nvalue table.first(tbl)\r\n";
            }
            catch (Exception ex)
            {
                MsgBox(ex.Message);
            }
            
        }

        #region Wrappers

        public void Move2D(double x, double y, double z)
        {
            info.Move2D(new WowPoint((float)x, (float)y, (float)z), 3f, 1000, true, false);
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
            return info.LuaGetValue(func);
        }

        public bool Lua_IsTrue(string condition)
        {
            return info.LuaIsTrue(condition);
        }
        
        #endregion

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
            List<WowNpc> npcs = new List<WowNpc>();
            List<WowPlayer> players = new List<WowPlayer>();
            WoWPlayerMe me = info.GetGameObjects(null, players, npcs);
            if (me != null)
            {
                WowNpc npc = npcs.FirstOrDefault(l => l.GUID == me.TargetGUID);
                if (npc != null)
                {
                    return npc;
                }
                else
                {
                    WowPlayer player = players.FirstOrDefault(l => l.GUID == me.TargetGUID);
                    if (player != null)
                    {
                        return player;
                    }
                }
            }
            return null;
        }

        #endregion

        #region Wrappers - Utils
        
        public void PressKey(int key)
        {
            NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
            NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)key, IntPtr.Zero);
        }

        public void MsgBox(object text)
        {
            MessageBox.Show(this, text.ToString());
        }

        public void Log(object text)
        {
            log.Info(text.ToString());
        }

        #endregion

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
                    log.Info("Lua console's timer is stopped: the player isn't active or not in the game");
                    Notify.SmartNotify("Lua console's timer is stopped", "The player isn't active or not in the game", NotifyUserType.Warn, false);
                    Invoke(new Action(() => InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty)));
                    return;
                }
                return;
            }
            if (luaConsoleSettings.TimerRnd)
            {
                safeTimer.ChangeInterval(luaConsoleSettings.TimerInterval + Utils.Rnd.Next(-(luaConsoleSettings.TimerInterval / 5), luaConsoleSettings.TimerInterval / 5));
            }
            try
            {
                // We should send commands as byte array because encoding is broken
                lock (luaExecutionLock)
                    luaEngine.DoString(Encoding.UTF8.GetBytes(textBoxLuaCode.Text));
            }
            catch (LuaException ex)
            {
                Notify.TrayPopup("Lua exception is thrown", ex.Message, NotifyUserType.Error, true);
                log.Info("LuaException in Timer.Elapsed: " + ex.Message);
                Thread.Sleep(1000);
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
            if (safeTimer != null && safeTimer.IsRunning)
            {
                InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty);
            }
            luaConsoleSettings.Code = textBoxLuaCode.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            luaConsoleSettings.TimerHotkeyChanged -= LuaTimerHotkeyChanged;
            HotkeyManager.RemoveKeys(typeof(LuaConsole).ToString());
            HotkeyManager.KeyPressed -= KeyboardListener2_KeyPressed;
            luaConsoleSettings.SaveJSON();
            log.Info("Closed");
        }

        private void PictureBoxOpenLuaFileClick(object sender, EventArgs e)
        {
            Select();
            using (OpenFileDialog p = new OpenFileDialog { Filter = @"Lua file|*.lua", InitialDirectory = AppFolders.UserfilesDir })
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
            using (SaveFileDialog p = new SaveFileDialog { Filter = @"Lua file|*.lua", InitialDirectory = AppFolders.UserfilesDir })
            {
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllText(p.FileName, string.Format("----- Interval={0}\r\n{1}", luaConsoleSettings.TimerInterval, textBoxLuaCode.Text), Encoding.UTF8);
                }
            }
        }

        private void ParseLuaScript(string text)
        {
            Regex regex = new Regex("----- Interval=(\\d+)\r\n(.+)$", RegexOptions.Singleline);
            Match match = regex.Match(text);
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

        private void metroCheckBoxRandomize_CheckedChanged(object sender, EventArgs e)
        {
            luaConsoleSettings.TimerRnd = metroCheckBoxRandomize.Checked;
        }

        private void metroCheckBoxIgnoreGameState_CheckedChanged(object sender, EventArgs e)
        {
            luaConsoleSettings.IgnoreGameState = metroCheckBoxIgnoreGameState.Checked;
        }
        
        private void metroTextBoxTimerInterval_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(metroTextBoxTimerInterval.Text, out luaConsoleSettings.TimerInterval);
        }

        private void LuaTimerHotkeyChanged(KeyExt key)
        {
            metroToolTip1.SetToolTip(metroLinkEnableCyclicExecution, string.Format("Enable/disable cyclic script execution\r\nHotkey: [{0}]", key.ToString()));
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
                if (!info.IsInGame)
                {
                    new TaskDialog("Error!", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                    return;
                }
                Stopwatch stopwatch = Stopwatch.StartNew();
                Task.Run(() => {
                    try
                    {
                        // We should send commands as byte array because encoding is broken
                        lock (luaExecutionLock)
                            luaEngine.DoString(Encoding.UTF8.GetBytes(textBoxLuaCode.Text));
                    }
                    catch (NLua.Exceptions.LuaException ex)
                    {
                        MsgBox(ex.Message);
                    }
                    finally
                    {
                        PostInvoke(() => {
                            labelRequestTime.Text = string.Format("{0}ms", stopwatch.ElapsedMilliseconds);
                            labelRequestTime.Visible = true;
                        });
                    }
                });
            }
        }

        private void metroLinkEnableCyclicExecution_Click(object sender, EventArgs e)
        {
            if (safeTimer != null && safeTimer.IsRunning)
            {
                safeTimer?.Dispose();
                log.Info("Timer disabled");
                Notify.SmartNotify("LuaConsole", "Timer is stopped", NotifyUserType.Info, false);
                SetupTimerControls(false);
            }
            else
            {
                labelRequestTime.Visible = false;
                if (!info.IsInGame)
                {
                    new TaskDialog("Error!", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                    return;
                }
                if (!int.TryParse(metroTextBoxTimerInterval.Text, out luaConsoleSettings.TimerInterval) || luaConsoleSettings.TimerInterval < 50)
                {
                    TaskDialog.Show("Incorrect input!", "AxTools", "Interval must be a number more or equal 50", TaskDialogButton.OK, TaskDialogIcon.Warning);
                    return;
                }
                if (textBoxLuaCode.Text.Trim().Length == 0)
                {
                    new TaskDialog("Error!", "AxTools", "Script is empty", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                    return;
                }
                SetupTimerControls(true);
                (safeTimer = new WoW.Helpers.SafeTimer(luaConsoleSettings.TimerInterval, info, TimerLuaElapsed)).Start();
                Notify.SmartNotify("LuaConsole", "Timer is started", NotifyUserType.Info, false);
                log.Info("Timer enabled");
            }
        }

        private void KeyboardListener2_KeyPressed(KeyExt obj)
        {
            if (obj == luaConsoleSettings.TimerHotkey)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                   if (WoWProcessManager.Processes.Values.Any(i => i.MainWindowHandle == NativeMethods.GetForegroundWindow()))
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
                    KeyExt key = new KeyExt(e.KeyCode, e.Alt, e.Shift, e.Control);
                    textBoxTimerHotkey.Text = key.ToString();
                    HotkeyManager.RemoveKeys(typeof(LuaConsole).ToString());
                    luaConsoleSettings.TimerHotkey = key;
                    Task.Run(() => {
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

    }
}