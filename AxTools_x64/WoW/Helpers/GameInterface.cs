using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using FMemory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AxTools.WoW.Helpers
{
    public class GameInterface
    {
        internal readonly WowProcess wowProcess;
        private readonly Log2 log;
        private readonly string gossipVariableName = Utils.GetRandomString(5, true);
        private readonly string LuaReturnFrameName = Utilities.GetRandomString(5, true);
        private readonly string LuaReturnTokenName = Utilities.GetRandomString(5, true);
        private readonly string LuaReturnVarName = Utilities.GetRandomString(5, true);
        private readonly List<ChatMsg> ChatMessages = new List<ChatMsg>(Enumerable.Repeat(ChatMsg.Empty, 60));
        private static readonly Dictionary<int, object> chatLocks = new Dictionary<int, object>();
        private static readonly Dictionary<int, object> readChatLocks = new Dictionary<int, object>();
        private static readonly Dictionary<int, object> luaLocks = new Dictionary<int, object>();

        internal GameInterface(WowProcess wow)
        {
            wowProcess = wow ?? throw new ArgumentNullException(nameof(wow));
            Memory = wow.Memory ?? throw new ArgumentNullException(nameof(wow), "Memory is null");
            log = new Log2($"GameInterface - {wow.ProcessID}");
            if (!chatLocks.ContainsKey(wowProcess.ProcessID)) chatLocks[wowProcess.ProcessID] = new object();
            if (!readChatLocks.ContainsKey(wowProcess.ProcessID)) readChatLocks[wowProcess.ProcessID] = new object();
            if (!luaLocks.ContainsKey(wowProcess.ProcessID)) luaLocks[wowProcess.ProcessID] = new object();
        }

        #region game

        public bool ChatIsOpened => wowProcess.Memory.Read<uint>(wowProcess.Memory.ImageBase + WowBuildInfoX64.ChatIsOpened) == 1;

        public void UseItemByID(uint id)
        {
            ChatboxSendText("/use item:" + id);
        }

        public void UseItem(int bagID, int slotID)
        {
            ChatboxSendText($"/use {bagID} {slotID}");
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
            SendToChat($"/run for i=1,GetMerchantNumItems() do if(GetMerchantItemInfo(i)==\"{itemName}\")then BuyMerchantItem(i,{count});return;end;end");
        }

        public void SendToChat(string command)
        {
            ChatboxSendText(command);
        }

        public object GetChatLock()
        {
            return chatLocks[wowProcess.ProcessID];
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
                            var vkLControl = (IntPtr)0xA2;
                            var vkA = (IntPtr)0x41;
                            var vkDelete = (IntPtr)0x2E;
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
                            var counter = 1000;
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
                            log.Info($"ChatboxSendText: recursive call, attempts: {attempts}");
                            ChatboxSendText(text, attempts);
                        }
                        else
                        {
                            log.Error($"ChatboxSendText: text and editboxText are not equal; text: {text}; editboxText: {editboxText}");
                            Notify.TrayPopup("Can't send command via chat", "Please don't type while this bot is working", NotifyUserType.Warn, true);
                        }
                    }
                }
            }
            else
            {
                log.Error($"ChatboxSendText: string is too long (length={text.Length}): {text}");
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
        
        #endregion Internal methods

        #endregion game

        #region Chat

        public IEnumerable<ChatMsg> ReadChat()
        {
            lock (readChatLocks[wowProcess.ProcessID])
            {
                if (IsInGame && !IsLoadingScreen)
                {
                    var chatStart = Memory.ImageBase + WowBuildInfoX64.ChatBuffer;
                    for (int i = 0; i < 60; i++)
                    {
                        var baseMsg = chatStart + i * WowBuildInfoX64.ChatNextMessage;
                        var message = new ChatMsg(
                            sender: Encoding.UTF8.GetString(Memory.ReadBytes(baseMsg + WowBuildInfoX64.ChatSenderName, 0x100).TakeWhile(l => l != 0).ToArray()),
                            channel: Memory.Read<byte>(baseMsg + WowBuildInfoX64.ChatChannelNum),
                            senderGUID: Memory.Read<WoWGUID>(baseMsg + WowBuildInfoX64.ChatSenderGuid),
                            text: Encoding.UTF8.GetString(Memory.ReadBytes(baseMsg + WowBuildInfoX64.ChatFullMessageOffset, 0x200).TakeWhile(l => l != 0).ToArray()),
                            timestamp: Memory.Read<int>(baseMsg + WowBuildInfoX64.ChatTimeStamp),
                            type: Memory.Read<WoWChatMsgType>(baseMsg + WowBuildInfoX64.ChatType)
                        );
                        if (ChatMessages[i] != message)
                        {
                            ChatMessages[i] = message;
                            yield return message;
                        }
                    }
                }
            }
        }

        #endregion Chat

        #region Info

        public string ZoneText => Wowhead.GetZoneText(ZoneID);

        public uint ZoneID => Memory.Read<uint>(Memory.ImageBase + WowBuildInfoX64.PlayerZoneID);

        public bool IsLooting
        {
            get
            {
                var lootNum = Memory.Read<uint>(Memory.ImageBase + WowBuildInfoX64.PlayerIsLooting + WowBuildInfoX64.PlayerIsLootingOffset0);
                if (lootNum == 0xFFFFFFF)
                {
                    lootNum = Memory.Read<uint>(Memory.ImageBase + WowBuildInfoX64.PlayerIsLooting + WowBuildInfoX64.PlayerIsLootingOffset1);
                }
                return lootNum != 0;
            }
        }

        public bool IsInGame
        {
            get
            {
                if (Memory == null)
                {
                    log.Error("memoryMgr is null");
                    return false;
                }
                try
                {
                    return Memory.Read<byte>(Memory.ImageBase + WowBuildInfoX64.GameState) == 4;
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
                    return Memory.Read<byte>(Memory.ImageBase + WowBuildInfoX64.NotLoadingScreen) == 0;
                }
                catch (Exception ex)
                {
                    StackTrace stackTrace = new StackTrace();
                    StackFrame[] stackFrames = stackTrace.GetFrames();
                    string stack = stackFrames != null ? string.Join(" -->> ", stackFrames.Select(l => $"{l.GetFileName()}::{l.GetMethod().Name}").Reverse()) : "Stack is null";
                    log.Error($"IsLoadingScreen: stack trace: {stack}; error message: {ex.Message}");
                    return false;
                }
            }
        }

        public bool IsAfk
        {
            get => Memory.Read<uint>(Memory.ImageBase + WowBuildInfoX64.IsChatAFK) != 0;
            set => SendToChat("/afk");
        }

        public bool IsSpellKnown(uint spellID)
        {
            var totalKnownSpells = Memory.Read<uint>(Memory.ImageBase + WowBuildInfoX64.KnownSpellsCount);
            var knownSpells = Memory.Read<IntPtr>(Memory.ImageBase + WowBuildInfoX64.KnownSpells);
            for (int i = 0; i < totalKnownSpells; i++)
            {
                var spellAddress = Memory.Read<IntPtr>(knownSpells + 8 * i);
                var pSpellID = Memory.Read<uint>(spellAddress + 4);
                var pSpellSuccess = Memory.Read<uint>(spellAddress);
                if (pSpellSuccess == 1 && pSpellID == spellID)
                {
                    return true;
                }
            }
            return false;
        }

        public WoWGUID MouseoverGUID => Memory.Read<WoWGUID>(Memory.ImageBase + WowBuildInfoX64.MouseoverGUID);

        #endregion Info

        #region Lua

        public string LuaGetValue(string function)
        {
            lock (luaLocks[wowProcess.ProcessID])
            {
                var myEditbox = WoWUIFrame.GetFrameByName(this, LuaReturnFrameName);
                if (myEditbox == null)
                {
                    SendToChat($"/run if(not {LuaReturnFrameName})then CreateFrame(\"EditBox\", \"{LuaReturnFrameName}\", UIParent);{LuaReturnFrameName}:SetAutoFocus(false);{LuaReturnFrameName}:ClearFocus(); end");
                }
                SendToChat($"/run {LuaReturnVarName}={function};{LuaReturnFrameName}:SetText(\"{LuaReturnTokenName}=\"..{LuaReturnVarName});");
                var counter = 2000;
                while (counter > 0)
                {
                    var frame = WoWUIFrame.GetFrameByName(this, LuaReturnFrameName);
                    if (frame != null)
                    {
                        var editboxText = frame.EditboxText;
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

        #endregion Lua

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
                var zDiff = Math.Abs(me.Location.Z - point.Z);
                var xyDiff = (float)me.Location.Distance2D(point);
                var spacePressed = false;
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
                            log.Info($"[Move3D] Okay, we're moving XY; current position: [{me.Location}]; distance2D to dest: [{me.Location.Distance2D(point)}]"); // todo: remove
                        }
                        else if (!me.IsMoving)
                        {
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                            log.Info($"[Move3D] W is released: {point}"); // todo: remove
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                            log.Info($"[Move3D] W is pressed: {point}"); // todo: remove
                        }
                    }
                    else
                    {
                        if (!continueMovingIfSuccessful)
                        {
                            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                            log.Info($"[Move3D] W is released3: {point}"); // todo: remove
                        }
                    }
                    if (!spacePressed && zDiff > precisionZ && point.Z > me.Location.Z && me.IsMounted && !me.IsFlying)
                    {
                        log.Info("[Move3D] Space is pressed");
                        NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.Space, IntPtr.Zero);
                        NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.Space, IntPtr.Zero);
                        spacePressed = true;
                    }
                    if (me.IsFlying && me.IsMounted)
                    {
                        if (zDiff > precisionZ)
                        {
                            var needPitch = MoveHelper.AngleVertical(me, point);
                            if (Math.Abs(needPitch - me.Pitch) > 0.1)
                            {
                                me.Pitch = needPitch;
                            }
                            if (me.Location.Distance(oldPos) < 1f && !me.IsMoving)
                            {
                                NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.W, IntPtr.Zero);
                                log.Info($"[Move3D] W is released2: {point}"); // todo: remove
                                NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.W, IntPtr.Zero);
                                log.Info($"[Move3D] W is pressed2: {point}"); // todo: remove
                            }
                        }
                        else
                        {
                            me.Pitch = (float)(Utils.Rnd.NextDouble() / 10);
                        }
                    }
                    oldPos = me.Location;
                }
                log.Info($"[game.Move3D] Return, timeout: {timeoutInMs}, diffXY: {me.Location.Distance2D(point)}, diffZ: {zDiff}"); // todo: remove
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
                var moving = me.IsMoving;
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
                var moving = me.IsMoving;
                if (face > 1)
                {
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Keys.S, IntPtr.Zero);
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Keys.S, IntPtr.Zero);
                    moving = false;
                }
                MoveHelper.FaceHorizontalWithTimer(wowProcess, face, Keys.D, moving);
            }
        }

        #endregion MoveMgr

        #region ObjMgr

        public WoWPlayerMe GetGameObjects(List<WowObject> objects = null, List<WowPlayer> players = null, List<WowNpc> npcs = null)
        {
            if (IsInGame)
            {
                return ObjectMgr.Pulse(wowProcess, objects, players, npcs);
            }
            objects?.Clear();
            players?.Clear();
            npcs?.Clear();
            return null;
        }

        #endregion ObjMgr

        #region Utils

        public MemoryManager Memory { get; }

        public void PressKey(int key)
        {
            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
            NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)key, IntPtr.Zero);
        }

        public WoWUIFrame GetUIFrameByName(string name)
        {
            return WoWUIFrame.GetFrameByName(this, name);
        }

        #endregion Utils

    }
}