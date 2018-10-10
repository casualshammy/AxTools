namespace AxTools.WoW.Internals
{
    public enum WoWChatMsgType : byte
    {
        Addon = 0,
        Say = 1,
        Party = 2,
        Raid = 3,
        Guild = 4,
        Officer = 5,
        Yell = 6,
        Whisper = 7,
        WhisperMob = 8,
        WhisperInform = 9,
        Emote = 10,
        TextEmote = 11,
        MonsterSay = 12,
        MonsterParty = 13,
        MonsterYell = 14,
        MonsterWhisper = 15,
        MonsterEmote = 16,
        Channel = 17,
        ChannelJoin = 18,
        ChannelLeave = 19,
        ChannelList = 20,
        ChannelNotice = 21,
        ChannelNoticeUser = 22,
        Afk = 23,
        Dnd = 24,
        Ignored = 25,
        Skill = 26,
        Loot = 27,

        BgEventNeutral = 35,
        BgEventAlliance = 36,
        BgEventHorde = 37,
        CombatFactionChange = 38,
        RaidLeader = 39,
        RaidWarning = 40,
        RaidWarningWidescreen = 41,

        Filtered = 43,
        Battleground = 44,
        BattlegroundLeader = 45,
        Restricted = 46,

        PartyLeader = 49,

        BNetWisper = 51,
        BNetWhisperInform = 52,

        RealId = 53,

        Instance = 63,
        InstanceLeader = 64
    }
}