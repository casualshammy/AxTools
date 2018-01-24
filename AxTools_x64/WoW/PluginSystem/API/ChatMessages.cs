using AxTools.Helpers;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AxTools.WoW.PluginSystem.API
{
    public static class ChatMessages
    {

        private static readonly object _chatLock = new object();
        private static readonly List<string> ChatMessagesLast = new List<string>(Enumerable.Repeat("", 60));
        public static event Action<ChatMsg> NewChatMessage;
        private static readonly Log2 log = new Log2("ChatMessages");

        /// <summary>
        ///     Invokes <see cref="NewChatMessage"/> if new messages appears
        /// </summary>
        public static void ReadChat()
        {
            lock (_chatLock)
            {
                if (Info.IsInGame && !Info.IsLoadingScreen)
                {
                    IntPtr chatStart = WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.ChatBuffer;
                    for (int i = 0; i < 60; i++)
                    {
                        IntPtr baseMsg = chatStart + i * WowBuildInfoX64.ChatNextMessage;
                        string s = Encoding.UTF8.GetString(WoWManager.WoWProcess.Memory.ReadBytes(baseMsg + WowBuildInfoX64.ChatFullMessageOffset, 0x200).TakeWhile(l => l != 0).ToArray());
                        if (ChatMessagesLast[i] != s)
                        {
                            ChatMessagesLast[i] = s;
                            NewChatMessage?.Invoke(ParseChatMsg(s));
                        }
                    }
                }
            }
        }

        private static ChatMsg ParseChatMsg(string s)
        {
            // Type: [7], Channel: [], Player Name: [Тэлин-Гордунни], Sender GUID: [Player-1602-05E946D2], Active player: [Player-1929-0844D1FA], Text: [2]
            Regex regex = new Regex("Type: \\[(\\d+)\\], Channel: \\[(.*)\\], Player Name: \\[(.*)\\], Sender GUID: \\[(.*)\\], Active player: \\[.*\\], Text: \\[(.*)\\]");
            Match match = regex.Match(s);
            if (match.Success)
            {
                if (!Enum.IsDefined(typeof(WoWChatMsgType), int.Parse(match.Groups[1].Value)))
                {
                    log.Error(string.Format("Type: {0}; Channel: {1}; Player Name: {2}; Sender GUID: {3}; Text: {4}",
                        int.Parse(match.Groups[1].Value), match.Groups[2].Value, match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value));
                }
                return new ChatMsg
                {
                    Type = (WoWChatMsgType)int.Parse(match.Groups[1].Value),
                    Channel = match.Groups[2].Value,
                    Sender = match.Groups[3].Value,
                    SenderGUID = match.Groups[4].Value,
                    Text = match.Groups[5].Value
                };
            }
            log.Error("ParseChatMsg: unknown signature: " + s);
            return new ChatMsg();
        }

    }
}
