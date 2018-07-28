using AxTools.Helpers;
using FMemory;
using AxTools.WinAPI;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace AxTools.WoW.PluginSystem.API
{
    public class GameInterface
    {
        internal readonly WowProcess wowProcess;
        private readonly MemoryManager memoryMgr;
        private readonly Log2 log;
        private readonly string gossipVariableName = Utils.GetRandomString(5, true);
        private readonly string LuaReturnFrameName = Utilities.GetRandomString(5, true);
        private readonly string LuaReturnTokenName = Utilities.GetRandomString(5, true);
        private readonly string LuaReturnVarName = Utilities.GetRandomString(5, true);
        private readonly List<ChatMsg> ChatMessages = new List<ChatMsg>(Enumerable.Repeat(new ChatMsg(), 60));
        private static readonly Dictionary<int, object> chatLocks = new Dictionary<int, object>();
        private static readonly Dictionary<int, object> readChatLocks = new Dictionary<int, object>();
        private static readonly Dictionary<int, object> luaLocks = new Dictionary<int, object>();
  
        internal GameInterface(WowProcess wow)
        {
            wowProcess = wow ?? throw new ArgumentNullException("wow");
            memoryMgr = wow.Memory ?? throw new ArgumentNullException("wow.Memory"); ;
            log = new Log2($"GameInterface - {wow.ProcessID}");
            if (!chatLocks.ContainsKey(wowProcess.ProcessID)) chatLocks[wowProcess.ProcessID] = new object();
            if (!readChatLocks.ContainsKey(wowProcess.ProcessID)) readChatLocks[wowProcess.ProcessID] = new object();
            if (!luaLocks.ContainsKey(wowProcess.ProcessID)) luaLocks[wowProcess.ProcessID] = new object();
        }

        #region game

        public void UseItemByID(uint id)
        {
            ChatboxSendText("/use item:" + id);
        }

        public void UseItem(int bagID, int slotID)
        {
            ChatboxSendText(string.Format("/use {0} {1}", bagID, slotID));
        }

        public void CastSpellByName(string spellName)
        {
            ChatboxSendText("/cast " + spellName);
        }

        public void SelectDialogOption(string gossipText)
        {
            ChatboxSendText(string.Format("/run _G[\"{1}\"]=0;for i=1,100 do if(select(i,GetGossipOptions())==\"{0}\")then _G[\"{1}\"]=i/2+0.5 end end", gossipText, gossipVariableName));
            ChatboxSendText(string.Format("/run if(_G[\"{0}\"]>0)then SelectGossipOption(_G[\"{0}\"], nil, true) end", gossipVariableName));
        }

        public void BuyMerchantItem(uint itemID, int count)
        {
            string itemName = Wowhead.GetItemInfo(itemID).Name;
            SendToChat(string.Format("/run for i=1,GetMerchantNumItems() do if(GetMerchantItemInfo(i)==\"{0}\")then BuyMerchantItem(i,{1});return;end;end", itemName, count));
        }

        public void SendToChat(string command)
        {
            ChatboxSendText(command);
        }
        
        #region Internal methods

        internal void ChatboxSendText(string text, int attempts = 3)
        {
            if (text.Length <= 254) // 254 - max length of non-latin string in game chat box
            {
                lock (chatLocks[wowProcess.ProcessID])
                {
                    wowProcess.WaitWhileWoWIsMinimized();
                    if (IsInGame && !IsLoadingScreen)
                    {
                        if (ChatIsOpened)
                        {
                            IntPtr vkLControl = (IntPtr)0xA2, vkA = (IntPtr)0x41, vkDelete = (IntPtr)0x2E;
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, vkLControl, IntPtr.Zero);
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, vkA, IntPtr.Zero);
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, vkA, IntPtr.Zero);
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, vkLControl, IntPtr.Zero);
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, vkDelete, IntPtr.Zero);
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, vkDelete, IntPtr.Zero);
                            Thread.Sleep(200);
                        }
                        else
                        {
                            int counter = 1000;
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)13, IntPtr.Zero);
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)13, IntPtr.Zero);
                            while (!ChatIsOpened && counter > 0)
                            {
                                counter -= 10;
                                Thread.Sleep(10);
                            }
                            Thread.Sleep(250);
                        }
                        foreach (char ch in text)
                        {
                            NativeMethods.PostMessage(wowProcess.MainWindowHandle, Win32Consts.WM_CHAR, (IntPtr)ch, IntPtr.Zero);
                        }
                        string editboxText = null;
                        Thread.Sleep(100);
                        for (int i = 0; i < 10; i++)
                        {
                            if ((editboxText = GetEditboxText()) == text)
                            {
                                NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)13, IntPtr.Zero);
                                NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)13, IntPtr.Zero);
                                return;
                            }
                            Thread.Sleep(100);
                        }
                        attempts--;
                        if (attempts > 0)
                        {
                            log.Info(string.Format("ChatboxSendText: recursive call, attempts: {0}", attempts));
                            ChatboxSendText(text, attempts);
                        }
                        else
                        {
                            log.Error(string.Format("ChatboxSendText: text and editboxText are not equal; text: {0}; editboxText: {1}", text, editboxText));
                            Notify.TrayPopup("Can't send command via chat", "Please don't type while this bot is working", NotifyUserType.Warn, true);
                        }
                    }
                }
            }
            else
            {
                log.Error(string.Format("ChatboxSendText: string is too long (length={0}): {1}", text.Length, text));
            }
        }

        private string GetEditboxText(string frameName = null)
        {
            WoWUIFrame frame = WoWUIFrame.GetFrameByName(this, frameName ?? "ChatFrame1EditBox");
            if (frame == null)
            {
                log.Error("GetEditboxText: " + (frameName ?? "ChatFrame1EditBox") + " is null");
            }
            return frame?.EditboxText;
        }

        private bool ChatIsOpened
        {
            get { return wowProcess.Memory.Read<uint>(wowProcess.Memory.ImageBase + WowBuildInfoX64.ChatIsOpened) == 1; }
        }

        #endregion

        #endregion

        #region Chat
        
        /// <summary>
        ///     Invokes <see cref="NewChatMessage"/> if new messages appears
        /// </summary>
        public IEnumerable<ChatMsg> ReadChat()
        {
            lock (readChatLocks[wowProcess.ProcessID])
            {
                if (IsInGame && !IsLoadingScreen)
                {
                    IntPtr chatStart = memoryMgr.ImageBase + WowBuildInfoX64.ChatBuffer;
                    for (int i = 0; i < 60; i++)
                    {
                        IntPtr baseMsg = chatStart + i * WowBuildInfoX64.ChatNextMessage;
                        ChatMsg s = new ChatMsg
                        {
                            Sender = Encoding.UTF8.GetString(memoryMgr.ReadBytes(baseMsg + WowBuildInfoX64.ChatSenderName, 0x100).TakeWhile(l => l != 0).ToArray()),
                            Channel = memoryMgr.Read<byte>(baseMsg + WowBuildInfoX64.ChatChannelNum),
                            SenderGUID = memoryMgr.Read<WoWGUID>(baseMsg + WowBuildInfoX64.ChatSenderGuid),
                            Text = Encoding.UTF8.GetString(memoryMgr.ReadBytes(baseMsg + WowBuildInfoX64.ChatFullMessageOffset, 0x200).TakeWhile(l => l != 0).ToArray()),
                            TimeStamp = memoryMgr.Read<int>(baseMsg + WowBuildInfoX64.ChatTimeStamp),
                            Type = memoryMgr.Read<WoWChatMsgType>(baseMsg + WowBuildInfoX64.ChatType)
                        };
                        if (ChatMessages[i] != s)
                        {
                            ChatMessages[i] = s;
                            yield return s;
                        }
                    }
                }
            }
        }

        //private ChatMsg ParseChatMsg(string s, IntPtr baseMsg)
        //{
        //    // Type: [7], Channel: [], Player Name: [Тэлин-Гордунни], Sender GUID: [Player-1602-05E946D2], Active player: [Player-1929-0844D1FA], Text: [2]
        //    Regex regex = new Regex("Type: \\[(\\d+)\\], Channel: \\[(.*)\\], Player Name: \\[(.*)\\], Sender GUID: \\[(.*)\\], Active player: \\[.*\\], Text: \\[(.*)\\]");
        //    Match match = regex.Match(s);
        //    if (match.Success)
        //    {
        //        if (!Enum.IsDefined(typeof(WoWChatMsgType), int.Parse(match.Groups[1].Value)))
        //        {
        //            log.Error(string.Format("Type: {0}; Channel: {1}; Player Name: {2}; Sender GUID: {3}; Text: {4}",
        //                int.Parse(match.Groups[1].Value), match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value));
        //        }
        //        return new ChatMsg
        //        {
        //            Type = (WoWChatMsgType)int.Parse(match.Groups[1].Value),
        //            Channel = match.Groups[2].Value,
        //            Sender = match.Groups[3].Value,
        //            SenderGUID = match.Groups[4].Value,
        //            Text = match.Groups[5].Value
        //        };
        //    }
        //    log.Error($"ParseChatMsg: unknown signature: 0x{baseMsg.ToInt64().ToString("X")} ({s})");
        //    return new ChatMsg();
        //}



        #endregion

        #region Info
        
        public string ZoneText
        {
            get
            {
                return Wowhead.GetZoneText(ZoneID);
            }
        }

        public uint ZoneID
        {
            get
            {
                return memoryMgr.Read<uint>(memoryMgr.ImageBase + WowBuildInfoX64.PlayerZoneID);
            }
        }

        public bool IsLooting
        {
            get
            {
                uint lootNum = memoryMgr.Read<uint>(memoryMgr.ImageBase + WowBuildInfoX64.PlayerIsLooting + WowBuildInfoX64.PlayerIsLootingOffset0);
                if (lootNum == 0xFFFFFFF)
                {
                    lootNum = memoryMgr.Read<uint>(memoryMgr.ImageBase + WowBuildInfoX64.PlayerIsLooting + WowBuildInfoX64.PlayerIsLootingOffset1);
                }
                return lootNum != 0;
            }
        }

        public bool IsInGame
        {
            get
            {
                if (memoryMgr == null)
                {
                    log.Error("memoryMgr is null");
                    return false;
                }
                try
                {
                    return memoryMgr.Read<byte>(memoryMgr.ImageBase + WowBuildInfoX64.GameState) == 4;
                }
                catch
                {
                    // we don't care what's happened
                    return false;
                }
            }
        }

        public bool IsLoadingScreen
        {
            get
            {
                try
                {
                    return memoryMgr.Read<byte>(memoryMgr.ImageBase + WowBuildInfoX64.NotLoadingScreen) == 0;
                }
                catch (Exception ex)
                {
                    StackTrace stackTrace = new StackTrace();
                    StackFrame[] stackFrames = stackTrace.GetFrames();
                    string stack = stackFrames != null ? string.Join(" -->> ", stackFrames.Select(l => string.Format("{0}::{1}", l.GetFileName(), l.GetMethod().Name)).Reverse()) : "Stack is null";
                    log.Error(string.Format("IsLoadingScreen: stack trace: {0}; error message: {1}", stack, ex.Message));
                    return false;
                }
            }
        }

        public bool IsAfk
        {
            get
            {
                return memoryMgr.Read<uint>(memoryMgr.ImageBase + WowBuildInfoX64.IsChatAFK) != 0;
            }
            set
            {
                SendToChat("/afk");
                SendToChat("/sit");
            }
        }

        public bool IsSpellKnown(uint spellID)
        {
            uint totalKnownSpells = memoryMgr.Read<uint>(memoryMgr.ImageBase + WowBuildInfoX64.KnownSpellsCount);
            IntPtr knownSpells = memoryMgr.Read<IntPtr>(memoryMgr.ImageBase + WowBuildInfoX64.KnownSpells);
            for (int i = 0; i < totalKnownSpells; i++)
            {
                IntPtr spellAddress = memoryMgr.Read<IntPtr>(knownSpells + 8 * i);
                uint pSpellID = memoryMgr.Read<uint>(spellAddress + 4);
                uint pSpellSuccess = memoryMgr.Read<uint>(spellAddress);
                if (pSpellSuccess == 1 && pSpellID == spellID)
                {
                    return true;
                }
            }
            return false;
        }

        public WoWGUID MouseoverGUID
        {
            get { return memoryMgr.Read<WoWGUID>(memoryMgr.ImageBase + WowBuildInfoX64.MouseoverGUID); }
        }

        #endregion

        #region Lua
        
        public string LuaGetValue(string function)
        {
            lock (luaLocks[wowProcess.ProcessID])
            {
                WoWUIFrame myEditbox = WoWUIFrame.GetFrameByName(this, LuaReturnFrameName);
                if (myEditbox == null)
                {
                    SendToChat($"/run if(not {LuaReturnFrameName})then CreateFrame(\"EditBox\", \"{LuaReturnFrameName}\", UIParent);{LuaReturnFrameName}:SetAutoFocus(false);{LuaReturnFrameName}:ClearFocus(); end");
                }
                SendToChat($"/run {LuaReturnVarName}={function};{LuaReturnFrameName}:SetText(\"{LuaReturnTokenName}=\"..{LuaReturnVarName});");
                int counter = 2000;
                while (counter > 0)
                {
                    WoWUIFrame frame = WoWUIFrame.GetFrameByName(this, LuaReturnFrameName);
                    if (frame != null)
                    {
                        string editboxText = frame.EditboxText;
                        if (editboxText != null)
                        {
                            if (editboxText.Contains(LuaReturnTokenName))
                            {
                                return editboxText.Remove(0, LuaReturnTokenName.Length + 1); // 1 - length of "="
                            }
                        }
                    }
                    else
                    {
                        log.Error($"GetValue: Can't find frame ({LuaReturnFrameName})");
                    }
                    Thread.Sleep(10);
                    counter -= 10;
                }
                return null;
            }
        }

        public bool LuaIsTrue(string condition)
        {
            return LuaGetValue($"tostring({condition})") == "true";
        }

        #endregion

        #region MoveMgr

        public void Move2D(WowPoint point, float precision, int timeoutInMs, bool continueMovingIfFailed, bool continueMovingIfSuccessful)
        {
            wowProcess.WaitWhileWoWIsMinimized();
            if (IsInGame && !IsLoadingScreen)
            {
                WoWPlayerMe me = ObjectMgr.Pulse(wowProcess);
                WowPoint oldPos = me.Location;
                while (IsInGame && !IsLoadingScreen && timeoutInMs > 0 && me.Location.Distance2D(point) > precision)
                {
                    Thread.Sleep(100);
                    timeoutInMs -= 100;
                    me = ObjectMgr.Pulse(wowProcess);
                    Face(point);
                    if (me.Location.Distance2D(oldPos) > 1f)
                    {
                        oldPos = me.Location;
                        log.Info($"[Move2D] Okay, we're moving; current position: [{me.Location}]; distance to dest: [{me.Location.Distance2D(point)}]"); // todo: remove
                    }
                    else if (!me.IsMoving)
                    {
                        NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                        log.Info($"[Move2D] W is released: {point}"); // todo: remove
                        NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                        log.Info($"[Move2D] W is pressed: {point}"); // todo: remove
                    }
                }
                if ((!continueMovingIfFailed || timeoutInMs > 0) && (!continueMovingIfSuccessful || timeoutInMs <= 0))
                {
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                    log.Info($"[Move2D] W is released2: {point}"); // todo: remove
                }
                log.Info($"[Move2D] return; distance to dest: [{me.Location.Distance2D(point)}]"); // todo: remove
            }
        }

        public void Move3D(WowPoint point, float precision2D, float precisionZ, int timeoutInMs, bool continueMovingIfFailed, bool continueMovingIfSuccessful)
        {
            wowProcess.WaitWhileWoWIsMinimized();
            if (IsInGame && !IsLoadingScreen)
            {
                WoWPlayerMe me = ObjectMgr.Pulse(wowProcess);
                WowPoint oldPos = me.Location;
                float zDiff = Math.Abs(me.Location.Z - point.Z);
                float oldZDiff = zDiff;
                float xyDiff = (float)me.Location.Distance2D(point);
                bool spacePressed = false;
                while (IsInGame && !IsLoadingScreen && timeoutInMs > 0 && (xyDiff > precision2D || zDiff > precisionZ))
                {
                    Thread.Sleep(50);
                    timeoutInMs -= 50;
                    me = ObjectMgr.Pulse(wowProcess);
                    zDiff = Math.Abs(me.Location.Z - point.Z);
                    xyDiff = (float)me.Location.Distance2D(point);
                    if (xyDiff > precision2D)
                    {
                        Face(point);
                        if (me.Location.Distance2D(oldPos) > 1f)
                        {
                            log.Info(string.Format("[Move3D] Okay, we're moving XY; current position: [{0}]; distance2D to dest: [{1}]", me.Location, me.Location.Distance2D(point))); // todo: remove
                        }
                        else if (!me.IsMoving)
                        {
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                            log.Info(string.Format("[Move3D] W is released: {0}", point)); // todo: remove
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                            log.Info(string.Format("[Move3D] W is pressed: {0}", point)); // todo: remove
                        }
                    }
                    else
                    {
                        if (!continueMovingIfSuccessful)
                        {
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                            log.Info(string.Format("[Move3D] W is released3: {0}", point)); // todo: remove
                        }
                    }
                    if (!spacePressed && zDiff > precisionZ && point.Z > me.Location.Z && me.IsMounted && !me.IsFlying)
                    {
                        log.Info($"[Move3D] Space is pressed");
                        NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
                        NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
                        spacePressed = true;
                    }
                    if (me.IsFlying && me.IsMounted)
                    {
                        if (zDiff > precisionZ)
                        {
                            float needPitch = MoveHelper.AngleVertical(me, point);
                            if (Math.Abs(needPitch - me.Pitch) > 0.1)
                            {
                                me.Pitch = needPitch;
                            }
                            if (me.Location.Distance(oldPos) < 1f && !me.IsMoving)
                            {
                                NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                                log.Info(string.Format("[Move3D] W is released2: {0}", point)); // todo: remove
                                NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                                log.Info(string.Format("[Move3D] W is pressed2: {0}", point)); // todo: remove
                            }
                        }
                        else
                        {
                            me.Pitch = (float)(Utils.Rnd.NextDouble() / 10);
                        }
                    }
                    oldPos = me.Location;
                }
                log.Info(string.Format("[game.Move3D] Return, timeout: {0}, diffXY: {1}, diffZ: {2}", timeoutInMs, me.Location.Distance2D(point), zDiff)); // todo: remove
            }
        }
        
        public void Jump()
        {
            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
        }

        public void Face(WowPoint point)
        {
            float face;
            WoWPlayerMe me = ObjectMgr.Pulse(wowProcess);
            if (MoveHelper.NegativeAngle(MoveHelper.AngleHorizontal(wowProcess, point) - me.Rotation) < Math.PI)
            {
                face = MoveHelper.NegativeAngle(MoveHelper.AngleHorizontal(wowProcess, point) - me.Rotation);
                bool moving = me.IsMoving;
                if (face > 1)
                {
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.S, IntPtr.Zero);
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.S, IntPtr.Zero);
                    moving = false;
                }
                MoveHelper.FaceHorizontalWithTimer(wowProcess, face, Keys.A, moving);
            }
            else
            {
                face = MoveHelper.NegativeAngle(me.Rotation - MoveHelper.AngleHorizontal(wowProcess, point));
                bool moving = me.IsMoving;
                if (face > 1)
                {
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.S, IntPtr.Zero);
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.S, IntPtr.Zero);
                    moving = false;
                }
                MoveHelper.FaceHorizontalWithTimer(wowProcess, face, Keys.D, moving);
            }
        }
        
        #endregion

        #region ObjMgr
        
        public WoWPlayerMe GetGameObjects(List<WowObject> wowObjects = null, List<WowPlayer> wowUnits = null, List<WowNpc> wowNpcs = null)
        {
            if (IsInGame)
            {
                return ObjectMgr.Pulse(wowProcess, wowObjects, wowUnits, wowNpcs);
            }
            wowObjects?.Clear();
            wowUnits?.Clear();
            wowNpcs?.Clear();
            return null;
        }

        #endregion
        
    }
}
