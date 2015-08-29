using AxTools.Components;
using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WoW;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using Settings = AxTools.Helpers.Settings;

namespace AxTools.Forms
{
    internal partial class LuaConsole : BorderedMetroForm, IWoWModule
    {
        private readonly Settings settings = Settings.Instance;
        internal static bool TimerEnabled { get; private set; }
        private readonly System.Timers.Timer timerLua = new System.Timers.Timer(1000);
        private const string MetroLinkEnableCyclicExecutionTextEnable = "<Enable cyclic execution>";
        private const string MetroLinkEnableCyclicExecutionTextDisable = "<Disable cyclic execution>";

        public LuaConsole()
        {
            InitializeComponent();
            AccessibleName = "Lua";
            timerLua.Elapsed += TimerLuaElapsed;
            Icon = Resources.AppIcon;
            textBoxLuaCode.SetHighlighting("Lua");
            textBoxLuaCode.Font = Utils.FontIsInstalled("Consolas") ? new Font("Consolas", 8) : new Font("Courier New", 8);
            textBoxLuaCode.Visible = true;
            metroPanelTimerOptions.Visible = false;
            Size = settings.WoWLuaConsoleWindowSize;
            metroStyleManager1.Style = settings.StyleColor;
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
            Log.Info(string.Format("{0}:{1} :: [Lua console] Loaded", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
        }
        

        internal void SwitchTimer()
        {
            InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty);
        }

        private void TimerLuaElapsed(object sender, ElapsedEventArgs e)
        {
            if (!WoWManager.Hooked || !WoWManager.WoWProcess.IsInGame)
            {
                if (!settings.WoWLuaConsoleIgnoreGameState)
                {
                    Log.Info(string.Format("{0}:{1} :: Lua console's timer is stopped: the player isn't active or not in the game", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
                    MainForm.Instance.ShowNotifyIconMessage("Lua console's timer is stopped", "The player isn't active or not in the game", ToolTipIcon.Error);
                    Invoke(new Action(() => InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty)));
                    return;
                }
                return;
            }
            if (settings.WoWLuaConsoleTimerRnd)
            {
                timerLua.Interval = settings.WoWLuaConsoleTimerInterval + Utils.Rnd.Next(-(settings.WoWLuaConsoleTimerInterval / 5), settings.WoWLuaConsoleTimerInterval / 5);
            }
            WoWDXInject.LuaDoString(textBoxLuaCode.Text);
        }

        private void ButtonDumpClick(object sender, EventArgs e)
        {
            //for (int k = 0; k < 3; k++)
            //{
            //    byte[] b = { };
            //    Stopwatch stopwatch = Stopwatch.StartNew();
            //    for (int i = 0; i < 10000000; i++)
            //    {
            //        b = WoWDXInject.CreatePrologue();
            //    }
            //    stopwatch.Stop();
            //    Log.Info(b.Length + " // " + stopwatch.ElapsedMilliseconds + "ms");
            //}
            //return;


            List<WowPlayer> wowUnits = new List<WowPlayer>();
            List<WowObject> wowObjects = new List<WowObject>();
            List<WowNpc> wowNpcs = new List<WowNpc>();
            WoWPlayerMe localPlayer;
            try
            {
                localPlayer = ObjectMgr.Pulse(wowObjects, wowUnits, wowNpcs);
            }
            catch (Exception ex)
            {
                Log.Error("Dump error: " + ex.Message);
                return;
            }
            var sb = new StringBuilder("\r\nLocal player-----------------------------------------\r\n");
            sb.AppendFormat("GUID: 0x{0}; Address: 0x{1:X}; Location: {2}; ZoneID: {3}; ZoneName: {4}; Realm: {5}; GUID bytes: {6}; IsLooting: {7}; Name: {8}\r\n",
                            localPlayer.GUID, (uint)localPlayer.Address, localPlayer.Location, WoWManager.WoWProcess.PlayerZoneID,
                            "dummy", WoWManager.WoWProcess.PlayerRealm, BitConverter.ToString(WoWManager.WoWProcess.Memory.ReadBytes(localPlayer.Address + WowBuildInfoX64.ObjectGUID, 16)), WoWManager.WoWProcess.PlayerIsLooting, WoWManager.WoWProcess.PlayerName);
            sb.AppendLine("Objects-----------------------------------------");
            foreach (var i in wowObjects)
            {
                sb.AppendFormat("{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; OwnerGUID: 0x{4}; Address: 0x{5:X}; EntryID: {6}\r\n", i.Name, i.GUID,
                                i.Location, (int)i.Location.Distance(localPlayer.Location), i.OwnerGUID, (uint)i.Address, i.EntryID);
            }
            sb.AppendLine("Npcs-----------------------------------------");
            foreach (var i in wowNpcs)
            {
                sb.AppendFormat("{0}; Location: {1}; Distance: {2}; HP:{3}; MaxHP:{4}; Address:0x{5:X}; GUID:0x{6}\r\n", i.Name, i.Location,
                    (int)i.Location.Distance(localPlayer.Location), i.Health, i.HealthMax, (uint)i.Address, i.GUID);
            }
            sb.AppendLine("Players-----------------------------------------");
            foreach (var i in wowUnits)
            {
                sb.AppendFormat(
                    "{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; Address:{4:X}; Class:{5}; Level:{6}; HP:{7}; MaxHP:{8}; TargetGUID: 0x{9}; IsAlliance:{10}\r\n",
                    i.Name, i.GUID, i.Location, (int)i.Location.Distance(localPlayer.Location), (uint)i.Address, i.Class, i.Level, i.Health, i.HealthMax,
                    i.TargetGUID, i.IsAlliance);
            }
            Log.Info(sb.ToString());


            //sb.AppendLine("Test-----------------------------------------");
            //WowObject nomi = wowObjects.FirstOrDefault(i => i.Name == "Хранилище гильдии");
            //if (nomi != null)
            //{
            //    Log.Print("Object is found!", false);
            //    Parallel.For((long)0, 0x200, i =>
            //        {
            //            try
            //            {
            //                IntPtr desc = WoWManager.WoWProcess.Memory.Read<IntPtr>(nomi.Address + (int)i);
            //                Parallel.For((long)0, 0x100, l =>
            //                    {
            //                        try
            //                        {
            //                            uint name2 = WoWManager.WoWProcess.Memory.Read<uint>(desc + (int)i1);
            //                            if (name2 == 188127)
            //                            {
            //                                sb.AppendLine("УДАЧА!!!: " + i1.ToString() + "/" + i.ToString());
            //                                Log.Print(sb.ToString(), false);
            //                            }
            //                        }
            //                        catch (Exception)
            //                        {
            //                        }
            //                    });
            //            }
            //            catch (Exception)
            //            {
            //            }
            //        });
            //}
            // WoWDXInject.MoveHook();
        }
        
        private void WowModulesFormClosing(object sender, FormClosingEventArgs e)
        {
            if (timerLua.Enabled)
            {
                InvokeOnClick(metroLinkEnableCyclicExecution, EventArgs.Empty);
            }
            settings.WoWLuaConsoleLastText = textBoxLuaCode.Text;
            settings.LuaTimerHotkeyChanged -= LuaTimerHotkeyChanged;
            Log.Info(string.Format("{0}:{1} :: [Lua console] Closed", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
        }

        private void PictureBoxOpenLuaFileClick(object sender, EventArgs e)
        {
            Select();
            AppSpecUtils.CheckCreateDir();
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
            AppSpecUtils.CheckCreateDir();
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
            Int32.TryParse(metroTextBoxTimerInterval.Text, out settings.WoWLuaConsoleTimerInterval);
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
                if (!WoWManager.Hooked || !WoWManager.WoWProcess.IsInGame)
                {
                    new TaskDialog("Error!", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                    return;
                }
                Stopwatch stopwatch = Stopwatch.StartNew();
                WoWDXInject.LuaDoString(textBoxLuaCode.Text);
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
                if (settings.WoWLuaConsoleShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsInGame)
                {
                    WoWDXInject.ShowOverlayText("LTimer is stopped", "Interface\\\\Icons\\\\inv_misc_pocketwatch_01", Color.FromArgb(255, 0, 0));
                }
                TimerEnabled = false;
                SetupTimerControls(false);
            }
            else
            {
                labelRequestTime.Visible = false;
                if (!WoWManager.Hooked || !WoWManager.WoWProcess.IsInGame)
                {
                    new TaskDialog("Error!", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                    return;
                }
                if (!Int32.TryParse(metroTextBoxTimerInterval.Text, out settings.WoWLuaConsoleTimerInterval) || settings.WoWLuaConsoleTimerInterval < 50)
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
                    WoWDXInject.ShowOverlayText("LTimer is started", "Interface\\\\Icons\\\\inv_misc_pocketwatch_01", Color.FromArgb(255, 102, 0));
                }
                Log.Info(string.Format("{0}:{1} :: [Lua console] Lua timer enabled", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
            }
        }

    }
}