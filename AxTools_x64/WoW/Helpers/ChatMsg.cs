using AxTools.WoW.Internals;

namespace AxTools.WoW.Helpers
{
    public class ChatMsg
    {
        public WoWChatMsgType Type;
        public string Channel;
        public string Sender;
        public string SenderGUID;
        public string Text;
    }
}