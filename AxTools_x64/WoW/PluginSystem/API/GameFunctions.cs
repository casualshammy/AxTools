using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
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
                    AppSpecUtils.NotifyUser("Attention!", "Please set up WoW internal keybinds in <Settings -> World of Warcraft -> Ingame key binds>", NotifyUserType.Warn, true);
                    Thread.Sleep(1000);
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
                    AppSpecUtils.NotifyUser("Attention!", "Please set up WoW internal keybinds in <Settings -> World of Warcraft -> Ingame key binds>", NotifyUserType.Warn, true);
                    Thread.Sleep(1000);
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
            lock (ChatLock)
            {
                WaitWhileWoWIsMinimized();
                if (IsInGame && !IsLoadingScreen)
                {
                    if (ChatIsOpened)
                    {
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)(long)MicrosoftVirtualKeys.VK_LCONTROL, IntPtr.Zero);
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)(long)MicrosoftVirtualKeys.A, IntPtr.Zero);
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)(long)MicrosoftVirtualKeys.A, IntPtr.Zero);
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)(long)MicrosoftVirtualKeys.VK_LCONTROL, IntPtr.Zero);
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)(long)MicrosoftVirtualKeys.Delete, IntPtr.Zero);
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)(long)MicrosoftVirtualKeys.Delete, IntPtr.Zero);
                        Thread.Sleep(200);
                    }
                    else
                    {
                        int counter = 1000;
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)13, IntPtr.Zero);
                        NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)13, IntPtr.Zero);
                        while (!ChatIsOpened && counter > 0)
                        {
                            counter -= 10;
                            Thread.Sleep(10);
                        }
                        Thread.Sleep(250);
                    }
                    foreach (char ch in text)
                    {
                        NativeMethods.PostMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_CHAR, (IntPtr)ch, IntPtr.Zero);
                    }
                    Thread.Sleep(250);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)13, IntPtr.Zero);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)13, IntPtr.Zero);
                }
            }
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
                return WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.GameState) == 1;
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
                return WoWManager.WoWProcess.Memory.Read<byte>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.Possible_NotLoadingScreen) == 0;
            }
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

        internal static void ShowNotify(string text)
        {
            AppSpecUtils.NotifyUser("AxTools", text, NotifyUserType.Info, false, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="text">Any text you want</param>
        /// <param name="warning">Is it warning or info</param>
        /// <param name="sound">Play sound</param>
        public static void ShowNotify(this IPlugin plugin, string text, bool warning, bool sound)
        {
            AppSpecUtils.NotifyUser("[" + plugin.Name + "]", text, warning ? NotifyUserType.Warn : NotifyUserType.Info, sound, true);
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

        private static void WaitWhileWoWIsMinimized()
        {
            if (WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsMinimized)
            {
                AppSpecUtils.NotifyUser("Attention!", "AxTools is stuck because it can't interact with minimized WoW client. Please activate WoW window!", NotifyUserType.Warn, true);
                while (WoWManager.WoWProcess != null && WoWManager.WoWProcess.IsMinimized)
                {
                    Thread.Sleep(100);
                }
            }
        }

    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal enum MicrosoftVirtualKeys
    {
        /// <summary>
        /// </summary>
        VK_LCONTROL = 0xA2,
        /// <summary>
        /// </summary>
        Alt = 0xA5,
        /// <summary>
        /// </summary>
        Indifferent = 0,
        /// <summary>
        /// </summary>
        key0 = 0x30,
        /// <summary>
        /// </summary>
        key1 = 0x31,
        /// <summary>
        /// </summary>
        key2 = 0x32,
        /// <summary>
        /// </summary>
        key3 = 0x33,
        /// <summary>
        /// </summary>
        key4 = 0x34,
        /// <summary>
        /// </summary>
        key5 = 0x35,
        /// <summary>
        /// </summary>
        key6 = 0x36,
        /// <summary>
        /// </summary>
        key7 = 0x37,
        /// <summary>
        /// </summary>
        key8 = 0x38,
        /// <summary>
        /// </summary>
        key9 = 0x39,
        /// <summary>
        /// </summary>
        LeftButton = 0x01,
        /// <summary>
        /// </summary>
        RightButton = 0x02,
        /// <summary>
        /// </summary>
        Cancel = 0x03,
        /// <summary>
        /// </summary>
        MiddleButton = 0x04,
        /// <summary>
        /// </summary>
        ExtraButton1 = 0x05,
        /// <summary>
        /// </summary>
        ExtraButton2 = 0x06,
        /// <summary>
        /// </summary>
        Back = 0x08,
        /// <summary>
        /// </summary>
        Tab = 0x09,
        /// <summary>
        /// </summary>
        Clear = 0x0C,
        /// <summary>
        /// </summary>
        Return = 0x0D,
        /// <summary>
        /// </summary>
        VK_SHIFT = 0x10,
        /// <summary>
        /// </summary>
        Control = 0x11,
        /// <summary>
        /// </summary>
        Menu = 0x12,
        /// <summary>
        /// </summary>
        Pause = 0x13,
        /// <summary>
        /// </summary>
        Kana = 0x15,
        /// <summary>
        /// </summary>
        Hangeul = 0x15,
        /// <summary>
        /// </summary>
        Hangul = 0x15,
        /// <summary>
        /// </summary>
        Junja = 0x17,
        /// <summary>
        /// </summary>
        Final = 0x18,
        /// <summary>
        /// </summary>
        Hanja = 0x19,
        /// <summary>
        /// </summary>
        Kanji = 0x19,
        /// <summary>
        /// </summary>
        Escape = 0x1B,
        /// <summary>
        /// </summary>
        Convert = 0x1C,
        /// <summary>
        /// </summary>
        NonConvert = 0x1D,
        /// <summary>
        /// </summary>
        Accept = 0x1E,
        /// <summary>
        /// </summary>
        ModeChange = 0x1F,
        /// <summary>
        /// </summary>
        Space = 0x20,
        /// <summary>
        /// </summary>
        Prior = 0x21,
        /// <summary>
        /// </summary>
        Next = 0x22,
        /// <summary>
        /// </summary>
        End = 0x23,
        /// <summary>
        /// </summary>
        Home = 0x24,
        /// <summary>
        /// </summary>
        Left = 0x25,
        /// <summary>
        /// </summary>
        Up = 0x26,
        /// <summary>
        /// </summary>
        Right = 0x27,
        /// <summary>
        /// </summary>
        Down = 0x28,
        /// <summary>
        /// </summary>
        Select = 0x29,
        /// <summary>
        /// </summary>
        Print = 0x2A,
        /// <summary>
        /// </summary>
        Execute = 0x2B,
        /// <summary>
        /// </summary>
        Snapshot = 0x2C,
        /// <summary>
        /// </summary>
        Insert = 0x2D,
        /// <summary>
        /// </summary>
        Delete = 0x2E,
        /// <summary>
        /// </summary>
        Help = 0x2F,
        /// <summary>
        /// </summary>
        N0 = 0x30,
        /// <summary>
        /// </summary>
        N1 = 0x31,
        /// <summary>
        /// </summary>
        N2 = 0x32,
        /// <summary>
        /// </summary>
        N3 = 0x33,
        /// <summary>
        /// </summary>
        N4 = 0x34,
        /// <summary>
        /// </summary>
        N5 = 0x35,
        /// <summary>
        /// </summary>
        N6 = 0x36,
        /// <summary>
        /// </summary>
        N7 = 0x37,
        /// <summary>
        /// </summary>
        N8 = 0x38,
        /// <summary>
        /// </summary>
        N9 = 0x39,
        /// <summary>
        /// </summary>
        A = 0x41,
        /// <summary>
        /// </summary>
        B = 0x42,
        /// <summary>
        /// </summary>
        C = 0x43,
        /// <summary>
        /// </summary>
        D = 0x44,
        /// <summary>
        /// </summary>
        E = 0x45,
        /// <summary>
        /// </summary>
        F = 0x46,
        /// <summary>
        /// </summary>
        G = 0x47,
        /// <summary>
        /// </summary>
        H = 0x48,
        /// <summary>
        /// </summary>
        I = 0x49,
        /// <summary>
        /// </summary>
        J = 0x4A,
        /// <summary>
        /// </summary>
        K = 0x4B,
        /// <summary>
        /// </summary>
        L = 0x4C,
        /// <summary>
        /// </summary>
        M = 0x4D,
        /// <summary>
        /// </summary>
        N = 0x4E,
        /// <summary>
        /// </summary>
        O = 0x4F,
        /// <summary>
        /// </summary>
        P = 0x50,
        /// <summary>
        /// </summary>
        Q = 0x51,
        /// <summary>
        /// </summary>
        R = 0x52,
        /// <summary>
        /// </summary>
        S = 0x53,
        /// <summary>
        /// </summary>
        T = 0x54,
        /// <summary>
        /// </summary>
        U = 0x55,
        /// <summary>
        /// </summary>
        V = 0x56,
        /// <summary>
        /// </summary>
        W = 0x57,
        /// <summary>
        /// </summary>
        X = 0x58,
        /// <summary>
        /// </summary>
        Y = 0x59,
        /// <summary>
        /// </summary>
        Z = 0x5A,
        /// <summary>
        /// </summary>
        LeftWindows = 0x5B,
        /// <summary>
        /// </summary>
        RightWindows = 0x5C,
        /// <summary>
        /// </summary>
        Application = 0x5D,
        /// <summary>
        /// </summary>
        Sleep = 0x5F,
        /// <summary>
        /// </summary>
        Numpad0 = 0x60,
        /// <summary>
        /// </summary>
        Numpad1 = 0x61,
        /// <summary>
        /// </summary>
        Numpad2 = 0x62,
        /// <summary>
        /// </summary>
        Numpad3 = 0x63,
        /// <summary>
        /// </summary>
        Numpad4 = 0x64,
        /// <summary>
        /// </summary>
        Numpad5 = 0x65,
        /// <summary>
        /// </summary>
        Numpad6 = 0x66,
        /// <summary>
        /// </summary>
        Numpad7 = 0x67,
        /// <summary>
        /// </summary>
        Numpad8 = 0x68,
        /// <summary>
        /// </summary>
        Numpad9 = 0x69,
        /// <summary>
        /// </summary>
        Multiply = 0x6A,
        /// <summary>
        /// </summary>
        Add = 0x6B,
        /// <summary>
        /// </summary>
        Separator = 0x6C,
        /// <summary>
        /// </summary>
        Subtract = 0x6D,
        /// <summary>
        /// </summary>
        Decimal = 0x6E,
        /// <summary>
        /// </summary>
        Divide = 0x6F,
        /// <summary>
        /// </summary>
        F1 = 0x70,
        /// <summary>
        /// </summary>
        F2 = 0x71,
        /// <summary>
        /// </summary>
        F3 = 0x72,
        /// <summary>
        /// </summary>
        F4 = 0x73,
        /// <summary>
        /// </summary>
        F5 = 0x74,
        /// <summary>
        /// </summary>
        F6 = 0x75,
        /// <summary>
        /// </summary>
        F7 = 0x76,
        /// <summary>
        /// </summary>
        F8 = 0x77,
        /// <summary>
        /// </summary>
        F9 = 0x78,
        /// <summary>
        /// </summary>
        F10 = 0x79,
        /// <summary>
        /// </summary>
        F11 = 0x7A,
        /// <summary>
        /// </summary>
        F12 = 0x7B,
        /// <summary>
        /// </summary>
        F13 = 0x7C,
        /// <summary>
        /// </summary>
        F14 = 0x7D,
        /// <summary>
        /// </summary>
        F15 = 0x7E,
        /// <summary>
        /// </summary>
        F16 = 0x7F,
        /// <summary>
        /// </summary>
        F17 = 0x80,
        /// <summary>
        /// </summary>
        F18 = 0x81,
        /// <summary>
        /// </summary>
        F19 = 0x82,
        /// <summary>
        /// </summary>
        F20 = 0x83,
        /// <summary>
        /// </summary>
        F21 = 0x84,
        /// <summary>
        /// </summary>
        F22 = 0x85,
        /// <summary>
        /// </summary>
        F23 = 0x86,
        /// <summary>
        /// </summary>
        F24 = 0x87,
        /// <summary>
        /// </summary>
        NumLock = 0x90,
        /// <summary>
        /// </summary>
        ScrollLock = 0x91,
        /// <summary>
        /// </summary>
        NEC_Equal = 0x92,
        /// <summary>
        /// </summary>
        Fujitsu_Jisho = 0x92,
        /// <summary>
        /// </summary>
        Fujitsu_Masshou = 0x93,
        /// <summary>
        /// </summary>
        Fujitsu_Touroku = 0x94,
        /// <summary>
        /// </summary>
        Fujitsu_Loya = 0x95,
        /// <summary>
        /// </summary>
        Fujitsu_Roya = 0x96,
        /// <summary>
        /// </summary>
        LeftShift = 0xA0,
        /// <summary>
        /// </summary>
        RightShift = 0xA1,
        /// <summary>
        /// </summary>
        LeftControl = 0xA2,
        /// <summary>
        /// </summary>
        RightControl = 0xA3,
        /// <summary>
        /// </summary>
        LeftMenu = 0xA4,
        /// <summary>
        /// </summary>
        RightMenu = 0xA5,
        /// <summary>
        /// </summary>
        BrowserBack = 0xA6,
        /// <summary>
        /// </summary>
        BrowserForward = 0xA7,
        /// <summary>
        /// </summary>
        BrowserRefresh = 0xA8,
        /// <summary>
        /// </summary>
        BrowserStop = 0xA9,
        /// <summary>
        /// </summary>
        BrowserSearch = 0xAA,
        /// <summary>
        /// </summary>
        BrowserFavorites = 0xAB,
        /// <summary>
        /// </summary>
        BrowserHome = 0xAC,
        /// <summary>
        /// </summary>
        VolumeMute = 0xAD,
        /// <summary>
        /// </summary>
        VolumeDown = 0xAE,
        /// <summary>
        /// </summary>
        VolumeUp = 0xAF,
        /// <summary>
        /// </summary>
        MediaNextTrack = 0xB0,
        /// <summary>
        /// </summary>
        MediaPrevTrack = 0xB1,
        /// <summary>
        /// </summary>
        MediaStop = 0xB2,
        /// <summary>
        /// </summary>
        MediaPlayPause = 0xB3,
        /// <summary>
        /// </summary>
        LaunchMail = 0xB4,
        /// <summary>
        /// </summary>
        LaunchMediaSelect = 0xB5,
        /// <summary>
        /// </summary>
        LaunchApplication1 = 0xB6,
        /// <summary>
        /// </summary>
        LaunchApplication2 = 0xB7,
        /// <summary>
        /// </summary>
        OEM1 = 0xBA,
        /// <summary>
        /// </summary>
        OEMPlus = 0xBB,
        /// <summary>
        /// </summary>
        OEMComma = 0xBC,
        /// <summary>
        /// </summary>
        OEMMinus = 0xBD,
        /// <summary>
        /// </summary>
        OEMPeriod = 0xBE,
        /// <summary>
        /// </summary>
        OEM2 = 0xBF,
        /// <summary>
        /// </summary>
        OEM3 = 0xC0,
        /// <summary>
        /// </summary>
        OEM4 = 0xDB,
        /// <summary>
        /// </summary>
        OEM5 = 0xDC,
        /// <summary>
        /// </summary>
        OEM6 = 0xDD,
        /// <summary>
        /// </summary>
        OEM7 = 0xDE,
        /// <summary>
        /// </summary>
        OEM8 = 0xDF,
        /// <summary>
        /// </summary>
        OEMAX = 0xE1,
        /// <summary>
        /// </summary>
        OEM102 = 0xE2,
        /// <summary>
        /// </summary>
        ICOHelp = 0xE3,
        /// <summary>
        /// </summary>
        ICO00 = 0xE4,
        /// <summary>
        /// </summary>
        ProcessKey = 0xE5,
        /// <summary>
        /// </summary>
        ICOClear = 0xE6,
        /// <summary>
        /// </summary>
        Packet = 0xE7,
        /// <summary>
        /// </summary>
        OEMReset = 0xE9,
        /// <summary>
        /// </summary>
        OEMJump = 0xEA,
        /// <summary>
        /// </summary>
        OEMPA1 = 0xEB,
        /// <summary>
        /// </summary>
        OEMPA2 = 0xEC,
        /// <summary>
        /// </summary>
        OEMPA3 = 0xED,
        /// <summary>
        /// </summary>
        OEMWSCtrl = 0xEE,
        /// <summary>
        /// </summary>
        OEMCUSel = 0xEF,
        /// <summary>
        /// </summary>
        OEMATTN = 0xF0,
        /// <summary>
        /// </summary>
        OEMFinish = 0xF1,
        /// <summary>
        /// </summary>
        OEMCopy = 0xF2,
        /// <summary>
        /// </summary>
        OEMAuto = 0xF3,
        /// <summary>
        /// </summary>
        OEMENLW = 0xF4,
        /// <summary>
        /// </summary>
        OEMBackTab = 0xF5,
        /// <summary>
        /// </summary>
        ATTN = 0xF6,
        /// <summary>
        /// </summary>
        CRSel = 0xF7,
        /// <summary>
        /// </summary>
        EXSel = 0xF8,
        /// <summary>
        /// </summary>
        EREOF = 0xF9,
        /// <summary>
        /// </summary>
        Play = 0xFA,
        /// <summary>
        /// </summary>
        Zoom = 0xFB,
        /// <summary>
        /// </summary>
        Noname = 0xFC,
        /// <summary>
        /// </summary>
        PA1 = 0xFD,
        /// <summary>
        /// </summary>
        OEMClear = 0xFE,
        VK_OEM_MINUS = 0xBD,
        VK_OEM_PLUS = 0xBB
    }

}
