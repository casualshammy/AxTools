﻿using System.Collections.Generic;
using System.Diagnostics;
using AxTools.Classes;
using AxTools.Components;
using AxTools.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;
using Settings = AxTools.Classes.Settings;

namespace AxTools.Forms
{
    internal partial class LuaConsole : BorderedMetroForm
    {
        internal LuaConsole()
        {
            InitializeComponent();
            AccessibleName = "Lua";
            timerLua.Elapsed += TimerLuaElapsed;
            Icon = Resources.AppIcon;
            textBoxLuaCode.SetHighlighting("Lua");
            textBoxLuaCode.Font = Utils.FontIsInstalled("Consolas") ? new Font("Consolas", 8) : new Font("Courier New", 8);
            textBoxLuaCode.Visible = true;
            metroPanelTimerOptions.Visible = false;
            Size = Settings.LuaConsoleSize;
            TimerHotkeyChanged();
            metroStyleManager1.Style = Settings.NewStyleColor;
            metroCheckBoxRandomize.Checked = Settings.LuaConsoleRandomizeTimer;
            metroCheckBoxIgnoreGameState.Checked = Settings.LuaConsoleIgnoreGameState;
            metroTextBoxTimerInterval.Text = Settings.LuaConsoleTimerInterval.ToString();
            metroCheckBoxShowIngameNotifications.Checked = Settings.LuaConsoleShowIngameNotifications;
            metroToolTip1.SetToolTip(pictureBoxRunOnce, "Execute script once");
            metroToolTip1.SetToolTip(pictureBoxRunLoop, string.Format("Enable loop script execution\r\nHotkey: [{0}]", Settings.LuaTimerHotkey));
            metroToolTip1.SetToolTip(pictureBoxStop, string.Format("Disable loop script execution\r\nHotkey: [{0}]", Settings.LuaTimerHotkey));
            metroToolTip1.SetToolTip(pictureBoxSettings, "Open settings");
            string filename = Globals.CfgPath + "\\.luaconsole3";
            try
            {
                if (File.Exists(filename))
                {
                    textBoxLuaCode.Text = File.ReadAllText(filename, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [Lua console] Can't load the latest list: {2}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ex.Message), true);
            }
            Log.Print(string.Format("{0}:{1} :: [Lua console] Loaded", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
        }

        private readonly System.Timers.Timer timerLua = new System.Timers.Timer(1000);

        private void TimerLuaElapsed(object sender, ElapsedEventArgs e)
        {
            if (!WoWManager.Hooked || !WoWManager.WoWProcess.IsInGame)
            {
                if (!Settings.LuaConsoleIgnoreGameState)
                {
                    Log.Print(string.Format("{0}:{1} :: Lua console's timer is stopped: the player isn't active or not in the game", WoWManager.WoWProcess.ProcessName,
                        WoWManager.WoWProcess.ProcessID));
                    MainForm.Instance.ShowNotifyIconMessage("Lua console's timer is stopped", "The player isn't active or not in the game", ToolTipIcon.Error);
                    Invoke(new Action(() => InvokeOnClick(pictureBoxStop, EventArgs.Empty)));
                    return;
                }
                return;
            }
            if (Settings.LuaConsoleRandomizeTimer)
            {
                timerLua.Interval = Settings.LuaConsoleTimerInterval + Utils.Rnd.Next(-(Settings.LuaConsoleTimerInterval / 5), Settings.LuaConsoleTimerInterval / 5);
            }
            WoWDXInject.LuaDoString(textBoxLuaCode.Text);
        }

        private void ButtonDumpClick(object sender, EventArgs e)
        {
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
                Log.Print("Dump error: " + ex.Message, true);
                return;
            }
            var sb = new StringBuilder("\r\nLocal player-----------------------------------------\r\n");
            sb.AppendFormat("GUID: 0x{0:X}; Address: 0x{1:X}; Location: {2}; ZoneID: {3}; ZoneName: {4}; Realm: {5}; BgExit: {6}; IsLooting: {7}; Name: {8}\r\n",
                            localPlayer.GUID, (uint)localPlayer.Address, localPlayer.Location, WoWManager.WoWProcess.PlayerZoneID,
                            "dummy", WoWManager.WoWProcess.PlayerRealm, WoWManager.WoWProcess.IsBattlegroundFinished, WoWManager.WoWProcess.PlayerIsLooting, WoWManager.WoWProcess.PlayerName);
            sb.AppendLine("Objects-----------------------------------------");
            foreach (var i in wowObjects)
            {
                sb.AppendFormat("{0} - GUID: 0x{1:X}; Location: {2}; Distance: {3}; OwnerGUID: 0x{4:X}; Address: 0x{5:X}; EntryID: {6}\r\n", i.Name, i.GUID,
                                i.Location, (int)i.Location.Distance(localPlayer.Location), i.OwnerGUID, (uint)i.Address, i.EntryID);
            }
            sb.AppendLine("Npcs-----------------------------------------");
            foreach (var i in wowNpcs)
            {
                sb.AppendFormat("{0}; Location: {1}; Distance: {2}; HP:{3}; MaxHP:{4}; Address:0x{5:X}; GUID:0x{6:X}\r\n", i.Name, i.Location,
                    (int)i.Location.Distance(localPlayer.Location), i.Health, i.HealthMax, (uint)i.Address, i.GUID);
            }
            sb.AppendLine("Players-----------------------------------------");
            foreach (var i in wowUnits)
            {
                sb.AppendFormat(
                    "{0} - GUID: 0x{1:X}; Location: {2}; Distance: {3}; Address:{4:X}; Class:{5}; Level:{6}; HP:{7}; MaxHP:{8}; TargetGUID: 0x{9:X}; IsAlliance:{10}\r\n",
                    i.Name, i.GUID, i.Location, (int)i.Location.Distance(localPlayer.Location), (uint)i.Address, i.Class, i.Level, i.Health, i.HealthMax,
                    i.TargetGUID, i.IsAlliance);
            }
            Log.Print(sb.ToString());


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



        }
        
        private void WowModulesFormClosing(object sender, FormClosingEventArgs e)
        {
            if (timerLua.Enabled)
            {
                InvokeOnClick(pictureBoxStop, EventArgs.Empty);
            }
            try
            {
                Utils.CheckCreateDir();
                File.WriteAllText(Globals.CfgPath + "\\.luaconsole3", textBoxLuaCode.Text, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [Lua console] Can't save the latest list: {2}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ex.Message), true);
            }
            Log.Print(string.Format("{0}:{1} :: [Lua console] Closed", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
        }

        private void PictureBoxOpenLuaFileClick(object sender, EventArgs e)
        {
            Select();
            Utils.CheckCreateDir();
            using (OpenFileDialog p = new OpenFileDialog { Filter = @"Lua file|*.lua", InitialDirectory = Globals.UserfilesPath })
            {
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    string text = File.ReadAllText(p.FileName, Encoding.UTF8);
                    if (text.Contains("### Interval=") || text.Contains("----- Interval="))
                    {
                        using (StringReader cStringReader = new StringReader(text))
                        {
                            string readLine = cStringReader.ReadLine();
                            if (readLine != null)
                            {
                                Settings.LuaConsoleTimerInterval = Convert.ToInt32(readLine.Split('=')[1]);
                                metroTextBoxTimerInterval.Text = Settings.LuaConsoleTimerInterval.ToString();
                            }
                            textBoxLuaCode.Text = cStringReader.ReadToEnd();
                        }
                    }
                    else
                    {
                        textBoxLuaCode.Text = text;
                    }
                    textBoxLuaCode.Invalidate(true);
                }
            }
        }

        private void PictureBoxSaveLuaFileClick(object sender, EventArgs e)
        {
            Select();
            Utils.CheckCreateDir();
            using (SaveFileDialog p = new SaveFileDialog { Filter = @"Lua file|*.lua", InitialDirectory = Globals.UserfilesPath })
            {
                if (p.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllText(p.FileName, string.Format("----- Interval={0}\r\n{1}", Settings.LuaConsoleTimerInterval, textBoxLuaCode.Text), Encoding.UTF8);
                }
            }
        }

        private void LuaConsole_Resize(object sender, EventArgs e)
        {
            Settings.LuaConsoleSize = Size;
        }

        private void metroCheckBoxRandomize_CheckedChanged(object sender, EventArgs e)
        {
            Settings.LuaConsoleRandomizeTimer = metroCheckBoxRandomize.Checked;
        }

        private void metroCheckBoxIgnoreGameState_CheckedChanged(object sender, EventArgs e)
        {
            Settings.LuaConsoleIgnoreGameState = metroCheckBoxIgnoreGameState.Checked;
        }

        private void metroCheckBoxShowIngameNotifications_CheckedChanged(object sender, EventArgs e)
        {
            Settings.LuaConsoleShowIngameNotifications = metroCheckBoxShowIngameNotifications.Checked;
        }

        private void metroTextBoxTimerInterval_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(metroTextBoxTimerInterval.Text, out Settings.LuaConsoleTimerInterval);
        }

        internal void TimerHotkeyChanged()
        {
            metroToolTip1.SetToolTip(pictureBoxRunLoop, string.Format("Enable loop script execution\r\nHotkey: [{0}]", Settings.LuaTimerHotkey));
            metroToolTip1.SetToolTip(pictureBoxStop, string.Format("Disable loop script execution\r\nHotkey: [{0}]", Settings.LuaTimerHotkey));
        }

        internal void SwitchTimer()
        {
            InvokeOnClick(timerLua.Enabled ? pictureBoxStop : pictureBoxRunLoop, EventArgs.Empty);
        }

        internal bool TimerEnabled
        {
            get { return timerLua.Enabled; }
        }

        private void pictureBoxRunOnce_Click(object sender, EventArgs e)
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
                labelRequestTime.Text = string.Format("Script has taken {0}ms to complete", stopwatch.ElapsedMilliseconds);
                labelRequestTime.Visible = true;
            }
        }

        private void pictureBoxRunLoop_Click(object sender, EventArgs e)
        {
            labelRequestTime.Visible = false;
            if (!WoWManager.Hooked || !WoWManager.WoWProcess.IsInGame)
            {
                new TaskDialog("Error!", "AxTools", "Player isn't logged in", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            if (!Int32.TryParse(metroTextBoxTimerInterval.Text, out Settings.LuaConsoleTimerInterval) || Settings.LuaConsoleTimerInterval < 50)
            {
                TaskDialog.Show("Incorrect input!", "AxTools", "Interval must be a number more or equal 50", TaskDialogButton.OK, TaskDialogIcon.Warning);
                return;
            }
            if (textBoxLuaCode.Text.Trim().Length == 0)
            {
                new TaskDialog("Error!", "AxTools", "Script is empty", TaskDialogButton.OK, TaskDialogIcon.Stop).Show(this);
                return;
            }
            timerLua.Interval = Settings.LuaConsoleTimerInterval;
            MainForm.LuaTimerEnabled = true;
            SetupTimerControls(true);
            timerLua.Enabled = true;
            if (Settings.LuaConsoleShowIngameNotifications)
            {
                WoWDXInject.ShowOverlayText("LTimer is started", "Interface\\\\Icons\\\\inv_misc_pocketwatch_01", Color.FromArgb(255, 102, 0));
                //WoWDXInject.LuaDoString("UIErrorsFrame:AddMessage(\"Lua timer is started\", 0.0, 1.0, 0.0)");
            }
            Log.Print(string.Format("{0}:{1} :: [Lua console] Lua timer enabled", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID));
        }

        private void pictureBoxStop_Click(object sender, EventArgs e)
        {
            timerLua.Enabled = false;
            Log.Print(WoWManager.WoWProcess != null
                          ? string.Format("{0}:{1} :: [Lua console] Lua timer disabled", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID)
                          : "UNKNOWN:null :: Lua timer disabled");
            TimerHotkeyChanged();
            if (Settings.LuaConsoleShowIngameNotifications && WoWManager.Hooked && WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsInGame)
            {
                WoWDXInject.ShowOverlayText("LTimer is stopped", "Interface\\\\Icons\\\\inv_misc_pocketwatch_01", Color.FromArgb(255, 0, 0));
                //WoWDXInject.LuaDoString("UIErrorsFrame:AddMessage(\"Lua timer is stopped\", 1.0, 0.4, 0.0)");
            }
            MainForm.LuaTimerEnabled = false;
            SetupTimerControls(false);
        }

        private void SetupTimerControls(bool timerActive)
        {
            if (timerActive)
            {
                textBoxLuaCode.IsReadOnly = true;
                pictureBoxStop.Enabled = true;
                pictureBoxSettings.Enabled = false;
                pictureBoxRunLoop.Enabled = false;
                pictureBoxStop.Image = Resources.yellow_stop;
                pictureBoxSettings.Image = Resources.yellow_record_grey;
                pictureBoxRunLoop.Image = Resources.yellow_forward_grey;
            }
            else
            {
                textBoxLuaCode.IsReadOnly = false;
                pictureBoxStop.Enabled = false;
                pictureBoxSettings.Enabled = true;
                pictureBoxRunLoop.Enabled = true;
                pictureBoxStop.Image = Resources.yellow_stop_grey;
                pictureBoxSettings.Image = Resources.yellow_record;
                pictureBoxRunLoop.Image = Resources.yellow_forward;
            }
        }

        private void pictureBoxSettings_Click(object sender, EventArgs e)
        {
            metroPanelTimerOptions.Visible = !metroPanelTimerOptions.Visible;
        }

    }
}