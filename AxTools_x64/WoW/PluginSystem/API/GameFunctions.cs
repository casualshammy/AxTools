using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        private static readonly string GossipVarName = Utilities.GetRandomString(5, true);
        private static readonly object ChatLock = new object();
        private static readonly Log2 log = new Log2("API.Chat");

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

        public static void BuyMerchantItem(uint itemID, int count)
        {
            string itemName = Wowhead.GetItemInfo(itemID).Name;
            SendToChat(string.Format("/run for i=1,GetMerchantNumItems() do if(GetMerchantItemInfo(i)==\"{0}\")then BuyMerchantItem(i,{1});return;end;end", itemName, count));
        }

        public static void SendToChat(string command)
        {
            ChatboxSendText(command);
        }

        internal static void SendToChat(WowProcess process, string command)
        {
            ChatboxSendText(process, command);
        }

        internal static void SetAfkStatus(WowProcess process)
        {
            if (!Lua.IsTrue(process, "IsChatAFK()"))
            {
                SendToChat(process, "/afk");
            }
        }

        #region Internal methods
        
        internal static void ChatboxSendText(string text, int attempts = 3)
        {
            ChatboxSendText(WoWManager.WoWProcess, text, attempts);
        }

        internal static void ChatboxSendText(WowProcess process, string text, int attempts = 3)
        {
            if (text.Length <= 254) // 254 - max length of non-latin string in game chat box
            {
                lock (ChatLock)
                {
                    process.WaitWhileWoWIsMinimized();
                    if (Info.IsProcessInGame(process) && !Info.IsProcessOnLoadingScreen(process))
                    {
                        if (ChatIsOpenedInProcess(process))
                        {
                            IntPtr vkLControl = (IntPtr)0xA2, vkA = (IntPtr)0x41, vkDelete = (IntPtr)0x2E;
                            NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYDOWN, vkLControl, IntPtr.Zero);
                            NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYDOWN, vkA, IntPtr.Zero);
                            NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYUP, vkA, IntPtr.Zero);
                            NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYUP, vkLControl, IntPtr.Zero);
                            NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYDOWN, vkDelete, IntPtr.Zero);
                            NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYUP, vkDelete, IntPtr.Zero);
                            Thread.Sleep(200);
                        }
                        else
                        {
                            int counter = 1000;
                            NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)13, IntPtr.Zero);
                            NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)13, IntPtr.Zero);
                            while (!ChatIsOpenedInProcess(process) && counter > 0)
                            {
                                counter -= 10;
                                Thread.Sleep(10);
                            }
                            Thread.Sleep(250);
                        }
                        foreach (char ch in text)
                        {
                            NativeMethods.PostMessage(process.MainWindowHandle, Win32Consts.WM_CHAR, (IntPtr)ch, IntPtr.Zero);
                        }
                        string editboxText = null;
                        Thread.Sleep(100);
                        for (int i = 0; i < 10; i++)
                        {
                            if ((editboxText = GetEditboxText(process)) == text)
                            {
                                NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)13, IntPtr.Zero);
                                NativeMethods.SendMessage(process.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)13, IntPtr.Zero);
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

        private static string GetEditboxText(string frameName = null)
        {
            return GetEditboxText(WoWManager.WoWProcess, frameName);
        }

        private static string GetEditboxText(WowProcess process, string frameName = null)
        {
            WoWUIFrame frame = WoWUIFrame.GetFrameByName(process, frameName ?? "ChatFrame1EditBox");
            if (frame == null)
            {
                log.Error("GetEditboxText: " + (frameName ?? "ChatFrame1EditBox") + " is null");
            }
            return frame?.EditboxText;
        }

        private static bool ChatIsOpened
        {
            get { return ChatIsOpenedInProcess(WoWManager.WoWProcess); }
        }

        private static bool ChatIsOpenedInProcess(WowProcess process)
        {
            return process.Memory.Read<uint>(process.Memory.ImageBase + WowBuildInfoX64.ChatIsOpened) == 1;
        }
        
        #endregion

    }
}
