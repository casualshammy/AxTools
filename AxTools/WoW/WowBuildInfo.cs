namespace AxTools.WoW
{
    internal static class WowBuildInfo
    {

        #region Build info

        internal static readonly byte[] WoWHash =
        {
            0x69, 0x4F, 0x5D, 0x3A, 0x10, 0xAF, 0x54, 0xC6, 0xBC, 0xAA, 0xF8, 0x37, 0x89, 0x0D, 0xDA, 0x36, 0xA2, 0x17, 0xB8, 0x29, 0x7B, 0x9B, 0x2E, 0x75, 0x79, 0xD2, 0xA7, 0xCA, 0x6F, 0x1B, 0xBE, 0x83
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
        internal static readonly int BlackMarketNumItems = 0xE42DE8;
        internal static readonly int BlackMarketItems = 0xE42DEC;
        internal static readonly int UnitNameCachePointer = 0xC8D914;// + 0x8;
        internal static readonly int ObjectManager = 0xCB9068; // ClntObjMgrPush (7)
        internal static readonly int PlayerPtr = 0xD263F8; // ClntObjMgrGetActivePlayerObj
        internal static readonly int GlueState = 0xC986E0;
        internal static readonly int FocusedWidget = 0xC323F8;

        #endregion

        #region Injected methods

        internal static readonly int ClntObjMgrGetActivePlayerObj = 0x3A35; // ClntObjMgrGetActivePlayerObj
        internal static readonly int LuaDoStringAddress = 0x2451D; // FrameScript_ExecuteBuffer
        internal static readonly int LuaGetLocalizedTextAddress = 0x2D8E88; // FrameScript_GetLocalizedText
        internal static readonly int TargetUnit = 0x905584; // CGGameUI::Target
        internal static readonly int Interact = 0x9077D8; // CGGameUI::Interact
        internal static readonly int HandleTerrainClick = 0x2477E3; // Spell_C__HandleTerrainClick
        internal static readonly int ClickToMove = 0x2E786A; // CGUnit_C::InitializeTrackingState

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