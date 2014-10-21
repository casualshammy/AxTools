namespace AxTools.WoW
{
    internal static class WowBuildInfo
    {

        #region Build info

        //internal static readonly int WowBuild = 19033;
        //internal static readonly int WowBuildAddress = 0x2543BB0;
        internal static readonly byte[] WoWHash =
        {
            0x4F, 0xAA, 0x11, 0xAA, 0x50, 0x70, 0x36, 0x41, 0x76, 0x7C, 0x6B, 0x56, 0xF5, 0x3A, 0x78, 0x6F, 0xF2, 0xFB, 0xE8, 0xCA, 0x12, 0x76, 0xAB, 0xCD, 0xE9, 0xD0, 0x32, 0x33, 0x2A, 0x31, 0x74, 0x14
        };

        #endregion
        
        #region Static infos

        internal static readonly int LastHardwareAction = 0xC3061C; // CGGameUI::UpdatePlayerAFK / WRITE
        internal static readonly int IsInGame = 0xD9133E; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound)
        internal static readonly int IsBattlegroundFinished = 0xDF0228; // Script_GetBattlefieldWinner (2)
        internal static readonly int PlayerName = 0xED3928; // ClientServices::GetCharacterName (or Script_UnitName/GetPlayerName)
        internal static readonly int PlayerRealm = 0xED3AD6; // Гордунни = D0 93 D0 BE D1 80 D0 B4 D1 83 D0 BD D0 BD D0 B8 // Черный Шрам = D0 A7 D0 B5 D1 80 D0 BD D1 8B D0 B9 20 D0 A8 D1 80 D0 B0 D0 BC
        internal static readonly int PlayerZoneID = 0xD91388; // CGGameUI::NewZoneFeedback (16) (or Script_GetRaidRosterInfo (101))
        internal static readonly int PlayerIsLooting = 0xE021B0; // CGPlayer_C::IsLooting (17) (or Script_SetLootPortrait (32) or Script_GetContainerPurchaseInfo)
        internal static readonly int BlackMarketNumItems = 0xE10758;
        internal static readonly int BlackMarketItems = 0xE1075C;
        internal static readonly int UnitNameCachePointer = 0xC8B574;// + 0x8;
        internal static readonly int ObjectManager = 0xCB6CC8; // ClntObjMgrPush (7)
        internal static readonly int PlayerPtr = 0xD24058; // ClntObjMgrGetActivePlayerObj
        internal static readonly int GlueState = 0xC96340;
        internal static readonly int FocusedWidget = 0xC302D0;
        //internal static readonly int PlayerGUID = 0xD34208;

        #endregion

        #region Injected methods

        internal static readonly int ClntObjMgrGetActivePlayerObj = 0x39F7; // ClntObjMgrGetActivePlayerObj
        internal static readonly int LuaDoStringAddress = 0x240C9; // FrameScript_ExecuteBuffer
        internal static readonly int LuaGetLocalizedTextAddress = 0x2D7060; // FrameScript_GetLocalizedText
        internal static readonly int TargetUnit = 0x903527; // CGGameUI::Target
        internal static readonly int Interact = 0x90577C; // CGGameUI::Interact
        internal static readonly int HandleTerrainClick = 0x245853; // Spell_C__HandleTerrainClick
        internal static readonly int ClickToMove = 0x2E57A9; // CGUnit_C::InitializeTrackingState

        #endregion

        #region Object manager

        internal static readonly int ObjectManagerFirstObject = 0x0C;
        internal static readonly int ObjectManagerNextObject = 0x3C;
        internal static readonly int ObjectType = 0xC;
        //internal static readonly int LocalGUID = 0xE8;
        internal static readonly int ObjectGUID = 0x28;

        #endregion

        #region Game object

        internal static readonly int GameObjectOwnerGUIDBase = 0x4;
        internal static readonly int GameObjectOwnerGUIDOffset = 0x30;
        internal static readonly int GameObjectAnimation = 0x104;
        internal static readonly int GameObjectEntryID = 0x24;
        internal static readonly int GameObjectNameBase = 0x26C;
        internal static readonly int GameObjectNameOffset = 0xB4;
        internal static readonly int GameObjectLocation = 0x138;

        #endregion

        #region Player unit

        internal static readonly int UnitDescriptors = 0x4;
        internal static readonly int UnitHealth = 0xEC;
        internal static readonly int UnitHealthMax = 0x108;
        internal static readonly int UnitTargetGUID = 0xA0;
        internal static readonly int UnitCastingID = 0xF38; // Script_UnitCastingInfo //
        internal static readonly int UnitChannelingID = 0xF58; // Script_UnitChannelInfo //
        internal static readonly int UnitLocation = 0xA50;
        internal static readonly int UnitRotation = UnitLocation + 0x10;
        internal static readonly int UnitNameCacheGUIDOffset = 0x10;
        internal static readonly int UnitNameCacheNameOffset = 0x21;

        #endregion

        #region NPC

        internal static readonly int NpcNameBase = 0xBC4;
        internal static readonly int NpcNameOffset = 0x7C;

        #endregion
    
    }
}