namespace AxTools.WoW.PluginSystem.API
{
    public class ChatMsg
    {
        /// <summary>
        /// Addon = 0,
        /// Afk = 0x17,
        /// Battleground = 0x2c,
        /// BattlegroundLeader = 0x2d,
        /// BgEventAlliance = 0x24,
        /// BgEventHorde = 0x25,
        /// BgEventNeutral = 0x23,
        /// Channel = 0x11,
        /// ChannelJoin = 0x12,
        /// ChannelLeave = 0x13,
        /// ChannelList = 20,
        /// ChannelNotice = 0x15,
        /// ChannelNoticeUser = 0x16,
        /// CombatFactionChange = 0x26,
        /// Dnd = 0x18,
        /// Emote = 10,
        /// Filtered = 0x2b,
        /// Guild = 4,
        /// Ignored = 0x19,
        /// Loot = 0x1b,
        /// MonsterEmote = 0x10,
        /// MonsterParty = 13,
        /// MonsterSay = 12,
        /// MonsterWhisper = 15,
        /// MonsterYell = 14,
        /// Officer = 5,
        /// Party = 2,
        /// Raid = 3,
        /// RaidLeader = 0x27,
        /// RaidWarning = 40,
        /// RaidWarningWidescreen = 0x29,
        /// RealId = 0x35,
        /// Restricted = 0x2e,
        /// Say = 1,
        /// Skill = 0x1a,
        /// TextEmote = 11,
        /// Whisper = 7,
        /// WhisperInform = 9,
        /// WhisperMob = 8,
        /// Yell = 6
        /// BNetWisper = 51,
        /// </summary>
        public int Type;
        public string Channel;
        public string Sender;
        public string SenderGUID;
        public string Text;
    }
}