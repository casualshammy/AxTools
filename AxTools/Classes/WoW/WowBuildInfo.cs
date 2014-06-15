namespace AxTools.Classes.WoW
{
    internal static class WowBuildInfo
    {

        #region Build info

        internal static readonly int WowBuild = 18291;
        internal static readonly int WowBuildAddress = 0xB94E74;

        #endregion
        
        #region Static infos

        internal static readonly int LastHardwareAction = 0xBB2C74; // CGGameUI::UpdatePlayerAFK / WRITE
        internal static readonly int IsInGame = 0xD65B16; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound)
        internal static readonly int IsBattlegroundFinished = 0xDC3050; // Script_GetBattlefieldWinner (2)
        internal static readonly int PlayerName = 0xEC4668; // ClientServices::GetCharacterName (or Script_UnitName/GetPlayerName)
        internal static readonly int PlayerRealm = 0xEC480E; // Гордунни = D0 93 D0 BE D1 80 D0 B4 D1 83 D0 BD D0 BD D0 B8 // Черный Шрам = D0 A7 D0 B5 D1 80 D0 BD D1 8B D0 B9 20 D0 A8 D1 80 D0 B0 D0 BC
        internal static readonly int PlayerZoneID = 0xD65BB4; // CGGameUI::NewZoneFeedback (16) (or Script_GetRaidRosterInfo (101))
        internal static readonly int PlayerIsLooting = 0xDD3270; // CGPlayer_C::IsLooting (17) (or Script_SetLootPortrait (32) or Script_GetContainerPurchaseInfo)
        internal static readonly int BlackMarketNumItems = 0xE10758;
        internal static readonly int BlackMarketItems = 0xE1075C;
        internal static readonly int UnitNameCachePointer = 0xC86838 + 0x8;
        internal static readonly int ObjectManager = 0xCB47C4; // ClntObjMgrPush (7)
        internal static readonly int PlayerPtr = 0xCFF49C; // ClntObjMgrGetActivePlayerObj

        #endregion

        #region Injected methods

        internal static readonly int ClntObjMgrGetActivePlayerObj = 0x4F84; // ClntObjMgrGetActivePlayerObj
        internal static readonly int LuaDoStringAddress = 0x4FD26; // FrameScript_ExecuteBuffer
        internal static readonly int LuaGetLocalizedTextAddress = 0x4141AE; // FrameScript_GetLocalizedText
        internal static readonly int SelectTarget = 0x8CE494; // CGGameUI::Target
        internal static readonly int Interact = 0x8D01EE; // CGGameUI::Interact
        //internal static readonly int HandleTerrainClick = 0x38D7AA; // Spell_C__HandleTerrainClick
        internal static readonly int ClickToMove = 0x41FA94; // CGUnit_C::InitializeTrackingState

        #endregion

        #region Object manager

        internal static readonly int ObjectManagerFirstObject = 0xCC;
        internal static readonly int ObjectManagerNextObject = 0x34;
        internal static readonly int ObjectType = 0xC;
        internal static readonly int LocalGUID = 0xE8;
        internal static readonly int ObjectGUID = 0x28;

        #endregion

        #region Game object

        internal static readonly int GameObjectOwnerGUIDBase = 0x4;
        internal static readonly int GameObjectOwnerGUIDOffset = 0x20;
        internal static readonly int GameObjectAnimation = 0xCC;
        internal static readonly int GameObjectEntryID = 0x28;
        internal static readonly int GameObjectNameBase = 0x1C0;
        internal static readonly int GameObjectNameOffset = 0xB0;
        internal static readonly int GameObjectLocation = 0x1F4;

        #endregion

        #region Player unit

        internal static readonly int UnitDescriptors = 0x4;
        internal static readonly int UnitHealth = 0x84;
        internal static readonly int UnitHealthMax = 0x9C;
        internal static readonly int UnitTargetGUID = 0x58;
        internal static readonly int UnitCastingID = 3256; // Script_UnitCastingInfo //
        internal static readonly int UnitChannelingID = 3280; // Script_UnitChannelInfo //
        internal static readonly int UnitLocation = 0x838;
        internal static readonly int UnitRotation = UnitLocation + 0x10;
        internal static readonly int UnitNameMaskOffset = 0x024;
        internal static readonly int UnitNameBaseOffset = 0x18;
        internal static readonly int UnitNameStringOffset = 0x21;

        #endregion

        #region NPC

        internal static readonly int NpcNameBase = 0x9B4;
        internal static readonly int NpcNameOffset = 0x6C;

        #endregion
    
    }
}