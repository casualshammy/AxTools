namespace AxTools.WoW
{
    internal static class WowBuildInfo
    {

        #region Build info

        // BUILD: 19116
        internal static readonly byte[] WoWHash =
        {
            0xEF, 0xB1, 0x83, 0x77, 0x36, 0x3F, 0xA5, 0x47, 0x08, 0x92, 0xD4, 0x7A, 0xE2, 0x82, 0x52, 0xD3, 0x20, 0x03, 0x48, 0x7C, 0x92, 0x85, 0xD4, 0x3E, 0xE4, 0x1E, 0x46, 0xC0, 0xF3, 0x94, 0xF1, 0x51
        };
        
        #endregion
        
        #region Static infos

        internal static readonly int LastHardwareAction = 0xC32744; // CGGameUI::UpdatePlayerAFK / WRITE
        internal static readonly int IsInGame = 0xD936DE; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound)
        internal static readonly int IsBattlegroundFinished = 0xDF25C8; // Script_GetBattlefieldWinner (2)
        internal static readonly int PlayerName = 0xED5CD0; // ClientServices::GetCharacterName (or Script_UnitName/GetPlayerName)
        internal static readonly int PlayerRealm = 0xED5E7E; // Гордунни = D0 93 D0 BE D1 80 D0 B4 D1 83 D0 BD D0 BD D0 B8 // Черный Шрам = D0 A7 D0 B5 D1 80 D0 BD D1 8B D0 B9 20 D0 A8 D1 80 D0 B0 D0 BC
        internal static readonly int PlayerZoneID = 0xD93728; // CGGameUI::NewZoneFeedback (16) (or Script_GetRaidRosterInfo (101))
        internal static readonly int PlayerIsLooting = 0xE04550; // CGPlayer_C::IsLooting (17) (or Script_SetLootPortrait (32) or Script_GetContainerPurchaseInfo)
        internal static readonly int BlackMarketNumItems = 0xE10758;
        internal static readonly int BlackMarketItems = 0xE1075C;
        internal static readonly int UnitNameCachePointer = 0xC8D914;// + 0x8;
        internal static readonly int ObjectManager = 0xCB9068; // ClntObjMgrPush (7)
        internal static readonly int PlayerPtr = 0xD263F8; // ClntObjMgrGetActivePlayerObj
        internal static readonly int GlueState = 0xC986E0;
        internal static readonly int FocusedWidget = 0xC323F8;

        #endregion

        #region Injected methods

        internal static readonly int ClntObjMgrGetActivePlayerObj = 0x3A23; // ClntObjMgrGetActivePlayerObj
        internal static readonly int LuaDoStringAddress = 0x242E9; // FrameScript_ExecuteBuffer
        internal static readonly int LuaGetLocalizedTextAddress = 0x2D8A8D; // FrameScript_GetLocalizedText
        internal static readonly int TargetUnit = 0x905323; // CGGameUI::Target
        internal static readonly int Interact = 0x907577; // CGGameUI::Interact
        internal static readonly int HandleTerrainClick = 0x247284; // Spell_C__HandleTerrainClick
        internal static readonly int ClickToMove = 0x2E74A5; // CGUnit_C::InitializeTrackingState

        #endregion

        #region Object manager

        internal static readonly int ObjectManagerFirstObject = 0x0C;
        internal static readonly int ObjectManagerNextObject = 0x3C;
        internal static readonly int ObjectType = 0xC;
        //internal static readonly int LocalGUID = 0xE8;
        internal const int ObjectGUID = 0x28; // declared as const because it used in WoWObjectsInfo

        #endregion

        #region Game object

        internal static readonly int GameObjectOwnerGUIDBase = 0x4;
        internal static readonly int GameObjectOwnerGUIDOffset = 0x30;
        //internal static readonly int GameObjectAnimation = 0x104;
        internal static readonly int GameObjectEntryID = 0x24;
        internal static readonly int GameObjectNameBase = 0x26C;
        internal static readonly int GameObjectNameOffset = 0xB4;
        //internal static readonly int GameObjectLocation = 0x138;

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