namespace AxTools.Classes.WoW
{
    internal static class WowBuildInfo
    {
        #region Build info

        internal static readonly int WowBuild = 17688;
        internal static readonly int WowBuildAddress = 0xB8FEAC;

        #endregion
        
        #region Static infos

        internal static readonly int LastHardwareAction = 0xBADCAC; // CGGameUI::UpdatePlayerAFK / WRITE
        internal static readonly int IsInGame = 0xD60B0E; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound)
        internal static readonly int IsBattlegroundFinished = 0xDBE038; // Script_GetBattlefieldWinner (2)
        internal static readonly int PlayerName = 0xEBF648; // ClientServices::GetCharacterName (or Script_UnitName)
        internal static readonly int PlayerRealm = 0xEBF7DE;
        internal static readonly int PlayerZoneID = 0xD60BAC; // CGGameUI::NewZoneFeedback (16) (or Script_GetRaidRosterInfo (101))
        internal static readonly int PlayerIsLooting = 0xDCE0B8; // CGPlayer_C::IsLooting (17) (or Script_SetLootPortrait (32) or Script_GetContainerPurchaseInfo)
        internal static readonly int BlackMarketNumItems = 0xE0B5A0;
        internal static readonly int BlackMarketItems = 0xE0B5A4;

        internal static readonly int UnitNameCachePointer = 0xC81878 + 0x8;
        internal static readonly int ObjectManager = 0xCAF7CC; // ClntObjMgrPush (7)

        #endregion

        #region Injected methods

        internal static readonly int ClntObjMgrGetActivePlayerObjAddress = 0x4E3B; // ClntObjMgrGetActivePlayerObj
        internal static readonly int LuaDoStringAddress = 0x5039C; // FrameScript_ExecuteBuffer
        internal static readonly int LuaGetLocalizedTextAddress = 0x41345F; // FrameScript_GetLocalizedText
        //internal static readonly int ClickToMove = 0x3E240B; // CGUnit_C::InitializeTrackingState // INCORRECT
        internal static readonly int SelectTarget = 0x8CBE19; // CGGameUI::Target
        internal static readonly int Interact = 0x8CDB78; // CGGameUI::Interact
        //internal static readonly int HandleTerrainClick = 0x38476E; // Spell_C__HandleTerrainClick

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