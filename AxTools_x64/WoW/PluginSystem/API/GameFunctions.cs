using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;

namespace AxTools.WoW.PluginSystem.API
{
    public static class GameFunctions
    {

        #region Interact

        public static void Interact(this WowObject wowObject)
        {
            Interact(wowObject.GUID);
        }

        public static void Interact(this WowNpc wowNpc)
        {
            Interact(wowNpc.GUID);
        }

        public static void Interact(WoWGUID guid)
        {
            WaitWhileWoWIsMinimized();
            if (IsInGame)
            {
                if (Settings.Instance.WoWInteractMouseover != Keys.None)
                {
                    WoWManager.WoWProcess.Memory.Write(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.MouseoverGUID, guid);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Settings.Instance.WoWInteractMouseover, IntPtr.Zero);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Settings.Instance.WoWInteractMouseover, IntPtr.Zero);
                }
                else
                {
                    Notify.TrayPopup("Attention!", "Please set up WoW internal keybinds in <Settings -> World of Warcraft -> Ingame key binds>", NotifyUserType.Warn, true);
                    Thread.Sleep(3000);
                }
            }
        }

        #endregion

        #region Target

        public static void Target(this WowNpc wowNpc)
        {
            Target(wowNpc.GUID);
        }

        public static void Target(this WowPlayer wowPlayer)
        {
            Target(wowPlayer.GUID);
        }

        public static void Target(WoWGUID guid)
        {
            WaitWhileWoWIsMinimized();
            if (IsInGame)
            {
                if (Settings.Instance.WoWTargetMouseover != Keys.None)
                {
                    WoWManager.WoWProcess.Memory.Write(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.MouseoverGUID, guid);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Settings.Instance.WoWTargetMouseover, IntPtr.Zero);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Settings.Instance.WoWTargetMouseover, IntPtr.Zero);
                }
                else
                {
                    Notify.TrayPopup("Attention!", "Please set up WoW internal keybinds in <Settings -> World of Warcraft -> Ingame key binds>", NotifyUserType.Warn, true);
                    Thread.Sleep(3000);
                }
            }
        }

        #endregion

        #region Chat commands

        private static readonly string GossipVarName = Utilities.GetRandomString(5);
        private static readonly object ChatLock = new object();

        public static void UseItemByID(uint id)
        {
            ChatboxSendText("/use item:" + id);
        }

        public static void UseItem(int bagID, int slotID)
        {
            ChatboxSendText(string.Format("/use {0} {1}", bagID, slotID));
        }

        public static void CastSpellByName(string spellName)
        {
            ChatboxSendText("/cast " + spellName);
        }

        public static void SelectDialogOption(string gossipText)
        {
            ChatboxSendText(string.Format("/run _G[\"{1}\"]=0;for i=1,100 do if(select(i,GetGossipOptions())==\"{0}\")then _G[\"{1}\"]=i/2+0.5 end end", gossipText, GossipVarName));
            ChatboxSendText(string.Format("/run if(_G[\"{0}\"]>0)then SelectGossipOption(_G[\"{0}\"], nil, true) end", GossipVarName));
        }

        public static void SendToChat(string command)
        {
            ChatboxSendText(command);
        }

        private static void ChatboxSendText(string text)
        {
            if (text.Length <= 254) // 254 - max length of non-latin string in game chat box
            {
                lock (ChatLock)
                {
                    WaitWhileWoWIsMinimized();
                    if (IsInGame && !IsLoadingScreen)
                    {
                        if (ChatIsOpened)
                        {
                            IntPtr vkLControl = (IntPtr) (long) 0xA2, vkA = (IntPtr) (long) 0x41, vkDelete = (IntPtr) (long) 0x2E;
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, vkLControl, IntPtr.Zero);
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, vkA, IntPtr.Zero);
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, vkA, IntPtr.Zero);
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, vkLControl, IntPtr.Zero);
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, vkDelete, IntPtr.Zero);
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, vkDelete, IntPtr.Zero);
                            Thread.Sleep(200);
                        }
                        else
                        {
                            int counter = 1000;
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr) 13, IntPtr.Zero);
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr) 13, IntPtr.Zero);
                            while (!ChatIsOpened && counter > 0)
                            {
                                counter -= 10;
                                Thread.Sleep(10);
                            }
                            Thread.Sleep(250);
                        }
                        foreach (char ch in text)
                        {
                            NativeMethods.PostMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_CHAR, (IntPtr) ch, IntPtr.Zero);
                        }
                        Thread.Sleep(250);
                        string editboxText = GetEditboxText();
                        if (text == editboxText)
                        {
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr) 13, IntPtr.Zero);
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr) 13, IntPtr.Zero);
                        }
                        else
                        {
                            Log.Error(string.Format("ChatboxSendText: text and editboxText are not equal; text: {0}; editboxText: {1}", text, editboxText));
                            Notify.Balloon("Can't send command via chat", "Please don't type while this bot is working", NotifyUserType.Warn, true);
                        }
                    }
                }
            }
            else
            {
                Log.Error(string.Format("ChatboxSendText: too long string (length={0}): {1}", text.Length, text));
            }
        }

        private static string GetEditboxText()
        {
            WoWUIFrame frame = WoWUIFrame.GetFrameByName("ChatFrame1EditBox");
            return frame != null ? frame.EditboxText : null;
        }

        private static bool ChatIsOpened
        {
            get { return WoWManager.WoWProcess.Memory.Read<uint>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.ChatIsOpened) == 1; }
        }

        #endregion

        #region Chat

        private static readonly object _chatLock = new object();

        private static readonly List<string> ChatMessagesLast = new List<string>(Enumerable.Repeat("", 60));

        public static event Action<ChatMsg> NewChatMessage;

        public static void ReadChat()
        {
            lock (_chatLock)
            {
                if (IsInGame && !IsLoadingScreen)
                {
                    IntPtr chatStart = WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.ChatBuffer;
                    for (int i = 0; i < 60; i++)
                    {
                        IntPtr baseMsg = chatStart + i * WowBuildInfoX64.ChatNextMessage;
                        string s = Encoding.UTF8.GetString(WoWManager.WoWProcess.Memory.ReadBytes(baseMsg + WowBuildInfoX64.ChatFullMessageOffset, 0x200).TakeWhile(l => l != 0).ToArray());
                        if (ChatMessagesLast[i] != s)
                        {
                            ChatMessagesLast[i] = s;
                            if (NewChatMessage != null)
                            {
                                NewChatMessage(ParseChatMsg(s));
                            }
                        }
                    }
                }
            }
        }

        private static ChatMsg ParseChatMsg(string s)
        {
            // Type: [7], Channel: [], Player Name: [Тэлин-Гордунни], Sender GUID: [Player-1602-05E946D2], Active player: [Player-1929-0844D1FA], Text: [2]
            Regex regex = new Regex("Type: \\[(\\d+)\\], Channel: \\[(.*)\\], Player Name: \\[(.*)\\], Sender GUID: \\[(.*)\\].*, Text: \\[(.*)\\]");
            Match match = regex.Match(s);
            if (match.Success)
            {
                return new ChatMsg
                {
                    Type = int.Parse(match.Groups[1].Value),
                    Channel = match.Groups[2].Value,
                    Sender = match.Groups[3].Value,
                    SenderGUID = match.Groups[4].Value,
                    Text = match.Groups[5].Value
                };
            }
            throw new Exception("This message is incorrect: " + s);
        }

        #endregion

        #region General Info

        public static string ZoneText
        {
            get
            {
                //IntPtr zoneTextPtr = WoWManager.WoWProcess.Memory.Read<IntPtr>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.ZoneText);
                //byte[] bytes = WoWManager.WoWProcess.Memory.ReadBytes(zoneTextPtr, 100).TakeWhile(l => l != 0).ToArray();
                //return Encoding.UTF8.GetString(bytes);
                return Wowhead.GetZoneText(ZoneID);
            }
        }

        public static uint ZoneID
        {
            get
            {
                return WoWManager.WoWProcess.Memory.Read<uint>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.PlayerZoneID);
            }
        }

        public static bool IsLooting
        {
            get
            {
                return WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.PlayerIsLooting) != 0;
            }
        }

        public static bool IsInGame
        {
            get
            {
                try
                {
                    return WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.GameState) == 1;
                }
                catch (Exception ex)
                {
                    StackTrace stackTrace = new StackTrace();
                    StackFrame[] stackFrames = stackTrace.GetFrames();
                    Log.Error("IsInGame: " + string.Join(" -->> ", stackFrames != null ? stackFrames.Select(l => l.GetMethod().Name).Reverse() : new[] { "Stack is null" }) + ex.Message);
                    return false;
                }
            }
        }

        internal static bool IsInGame_(WowProcess process)
        {
            if (process.Memory == null) return false;
            return process.Memory.Read<byte>(process.Memory.ImageBase + WowBuildInfoX64.GameState) == 1;
        }

        public static bool IsLoadingScreen
        {
            get
            {
                try
                {
                    return WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.Possible_NotLoadingScreen) == 0;
                }
                catch (Exception ex)
                {
                    StackTrace stackTrace = new StackTrace();
                    StackFrame[] stackFrames = stackTrace.GetFrames();
                    Log.Error("IsLoadingScreen: " + string.Join(" -->> ", stackFrames != null ? stackFrames.Select(l => l.GetMethod().Name).Reverse() : new[] { "Stack is null" }) + ex.Message);
                    return false;
                }
            }
        }

        public static bool IsSpellKnown(uint spellID)
        {
            uint totalKnownSpells = WoWManager.WoWProcess.Memory.Read<uint>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.KnownSpellsCount);
            IntPtr knownSpells = WoWManager.WoWProcess.Memory.Read<IntPtr>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.KnownSpells);
            for (int i = 0; i < totalKnownSpells; i++)
            {
                IntPtr spellAddress = WoWManager.WoWProcess.Memory.Read<IntPtr>(knownSpells + 8 * i);
                uint pSpellID = WoWManager.WoWProcess.Memory.Read<uint>(spellAddress + 4);
                uint pSpellSuccess = WoWManager.WoWProcess.Memory.Read<uint>(spellAddress);
                if (pSpellSuccess == 1 && pSpellID == spellID)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Moving

        public static void Move2D(WowPoint point, float precision, int timeoutInMs, bool continueMovingIfFailed, bool continueMovingIfSuccessful)
        {
            WaitWhileWoWIsMinimized();
            if (IsInGame && !IsLoadingScreen)
            {
                WoWPlayerMe me = ObjMgr.Pulse();
                WowPoint oldPos = me.Location;
                while (IsInGame && !IsLoadingScreen && timeoutInMs > 0 && me.Location.Distance2D(point) > precision)
                {
                    Thread.Sleep(100);
                    timeoutInMs -= 100;
                    me = ObjMgr.Pulse();
                    point.Face();
                    if (me.Location.Distance2D(oldPos) > 1f)
                    {
                        oldPos = me.Location;
                        Log.Info(string.Format("{0} [GameFunctions.MoveTo] Okay, we're moving; current position: [{1}]; distance to dest: [{2}]", WoWManager.WoWProcess, me.Location, me.Location.Distance2D(point))); // todo: remove
                    }
                    else if (!me.IsMoving)
                    {
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                        Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is released: {1}", WoWManager.WoWProcess, point)); // todo: remove
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                        Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is pressed: {1}", WoWManager.WoWProcess, point)); // todo: remove
                    }
                    //else
                    //{
                    //    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr) Keys.W, IntPtr.Zero);
                    //    Log.Info(string.Format("{0} [GameFunctions.MoveTo] Pressing W: {1}", WoWManager.WoWProcess, point));
                    //}
                }
                if ((!continueMovingIfFailed || timeoutInMs > 0) && (!continueMovingIfSuccessful || timeoutInMs <= 0))
                {
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr) Keys.W, IntPtr.Zero);
                    Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is released2: {1}", WoWManager.WoWProcess, point)); // todo: remove
                    //NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.S, IntPtr.Zero);
                    //NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.S, IntPtr.Zero);
                }
                Log.Info(string.Format("{0} [GameFunctions.MoveTo] return; distance to dest: [{1}]", WoWManager.WoWProcess, me.Location.Distance2D(point))); // todo: remove
            }
        }

        public static void Move3D(WowPoint point, float precision2D, float precisionZ, int timeoutInMs, bool continueMoving)
        {
            WaitWhileWoWIsMinimized();
            if (IsInGame && !IsLoadingScreen)
            {
                WoWPlayerMe me = ObjMgr.Pulse();
                WowPoint oldPos = me.Location;
                float zDiff = Math.Abs(me.Location.Z - point.Z);
                while (IsInGame && !IsLoadingScreen && timeoutInMs > 0 && (me.Location.Distance2D(point) > precision2D || zDiff > precisionZ))
                {
                    Thread.Sleep(100);
                    timeoutInMs -= 100;
                    me = ObjMgr.Pulse();
                    float oldZDiff = zDiff;
                    zDiff = Math.Abs(me.Location.Z - point.Z);
                    if (me.Location.Distance2D(point) > precision2D)
                    {
                        point.Face();
                        if (me.Location.Distance2D(oldPos) > 1f)
                        {
                            oldPos = me.Location;
                            Log.Info(string.Format("{0} [GameFunctions.MoveTo] Okay, we're moving XY; current position: [{1}]; distance2D to dest: [{2}]", WoWManager.WoWProcess, me.Location, me.Location.Distance2D(point))); // todo: remove
                        }
                        else if (!me.IsMoving)
                        {
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                            Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is released: {1}", WoWManager.WoWProcess, point)); // todo: remove
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                            Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is pressed: {1}", WoWManager.WoWProcess, point)); // todo: remove
                        }
                    }
                    else
                    {
                        if (zDiff > precisionZ || !continueMoving)
                        {
                            NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                            Log.Info(string.Format("{0} [GameFunctions.MoveTo] W is released3: {1}", WoWManager.WoWProcess, point)); // todo: remove
                        }
                    }
                    if (me.IsFlying && me.IsMounted && zDiff > precisionZ)
                    {
                        if (zDiff < oldZDiff)
                        {
                            Log.Info(string.Format("{0} [GameFunctions.MoveTo] Okay, we're moving Z; current position: [{1}]; distance to dest: [{2}]; zDiff: {3}; oldZDiff: {4}", WoWManager.WoWProcess, me.Location,
                                me.Location.Distance(point), zDiff, oldZDiff)); // todo: remove
                        }
                        else
                        {
                            if (me.Location.Z - point.Z > precisionZ)
                            {
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.X, IntPtr.Zero);
                                Log.Info(string.Format("{0} [GameFunctions.MoveTo] X is pressed: {1}", WoWManager.WoWProcess, point)); // todo: remove
                            }
                            else if (point.Z - me.Location.Z > precisionZ)
                            {
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.X, IntPtr.Zero);
                                NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
                                Log.Info(string.Format("{0} [GameFunctions.MoveTo] Space is pressed: {1}", WoWManager.WoWProcess, point)); // todo: remove
                            }
                        }
                    }
                    else
                    {
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.X, IntPtr.Zero);
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
                        Log.Info(string.Format("{0} [GameFunctions.MoveTo] X is released: {1}", WoWManager.WoWProcess, point)); // todo: remove
                        Log.Info(string.Format("{0} [GameFunctions.MoveTo] Space is released: {1}", WoWManager.WoWProcess, point)); // todo: remove
                    }
                }
                Log.Info(string.Format("{0} [GameFunctions.Move3D] Return, timeout: {1}, diffXY: {2}, diffZ: {3}", WoWManager.WoWProcess, timeoutInMs, me.Location.Distance2D(point), zDiff)); // todo: remove
            }
        }

        #endregion

        private static void WaitWhileWoWIsMinimized()
        {
            if (WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsMinimized)
            {
                Notify.TrayPopup("Attention!", "AxTools is stuck because it can't interact with minimized WoW client. Click to activate WoW window", NotifyUserType.Warn, true, null, 10,
                    (sender, args) => NativeMethods.ShowWindow(WoWManager.WoWProcess.MainWindowHandle, 9));
                Utils.LogIfCalledFromUIThread();
                while (WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsMinimized)
                {
                    Thread.Sleep(100);
                }
            }
        }
    
    }
}
