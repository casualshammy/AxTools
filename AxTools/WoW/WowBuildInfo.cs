namespace AxTools.WoW
{
    internal static class WowBuildInfo
    {

        #region Build info

        internal static readonly byte[] WoWHash =
        {
            0x10, 0xE2, 0xD4, 0xB0, 0x3B, 0xC1, 0xE8, 0x3D, 0xAA, 0x0E, 0xB2, 0x60, 0xB0, 0x9A, 0xA1, 0x89, 0xA3, 0xC2, 0xA7, 0x9F, 0x7C, 0x11, 0x72, 0x24, 0x22, 0x72, 0x4B, 0x5A, 0x04, 0xB3, 0x82, 0x3D
        };
        
        #endregion
        
        #region Static infos

        internal static readonly int LastHardwareAction = 0xCB63F0; // CGGameUI::UpdatePlayerAFK / WRITE
        internal static readonly int GameState = 0xE35B2E; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound)
        internal static readonly int PlayerName = 0xF24D40; // ClientServices::GetCharacterName (or Script_UnitName/GetPlayerName)
        internal static readonly int PlayerRealm = 0xF24EEE; // Гордунни = D0 93 D0 BE D1 80 D0 B4 D1 83 D0 BD D0 BD D0 B8 // Черный Шрам = D0 A7 D0 B5 D1 80 D0 BD D1 8B D0 B9 20 D0 A8 D1 80 D0 B0 D0 BC
        internal static readonly int PlayerZoneID = 0xE35B78; // CGGameUI::NewZoneFeedback (16) (or Script_GetRaidRosterInfo (101))
        internal static readonly int PlayerIsLooting = 0xE54980; // CGPlayer_C::IsLooting (17) (or Script_SetLootPortrait (32) or Script_GetContainerPurchaseInfo)
        internal static readonly int BlackMarketNumItems = 0xE922C0;
        internal static readonly int BlackMarketItems = 0xE922C4;
        internal static readonly int ObjectManager = 0xD3B358; // ClntObjMgrPush (7)
        internal static readonly int PlayerPtr = 0xDC0BC8; // ClntObjMgrGetActivePlayerObj
        internal static readonly int GlueState = 0xD1D878;
        internal static readonly int FocusedWidget = 0xCB60B8;
        
        #endregion

        #region Injected methods

        internal static readonly int ClntObjMgrGetActivePlayerObj = 0x3B0E; // ClntObjMgrGetActivePlayerObj
        internal static readonly int LuaDoStringAddress = 0x250D1; // FrameScript_ExecuteBuffer
        internal static readonly int LuaGetLocalizedTextAddress = 0x2E9FC9; // FrameScript_GetLocalizedText
        internal static readonly int TargetUnit = 0x946FC0; // CGGameUI::Target
        internal static readonly int Interact = 0x949215; // CGGameUI::Interact
        internal static readonly int HandleTerrainClick = 0x256A04; // Spell_C__HandleTerrainClick
        internal static readonly int ClickToMove = 0x2F9C96; // CGUnit_C::InitializeTrackingState // Wow.exe+2F9D1C
        internal static readonly int CGWorldFrameRender = 0x267B4A;

        #endregion

        #region Object manager

        internal static readonly int ObjectManagerFirstObject = 0x0C;
        internal static readonly int ObjectManagerNextObject = 0x3C;
        internal static readonly int ObjectType = 0xC;
        internal const int ObjectGUID = 0x28; // declared as const because it used in WoWObjectsInfo

        #endregion

        #region Game object

        internal static readonly int GameObjectOwnerGUIDBase = 0x4;
        internal static readonly int GameObjectOwnerGUIDOffset = 0x30;
        internal static readonly int GameObjectEntryID = 0x24;
        internal static readonly int GameObjectNameBase = 0x274;
        internal static readonly int GameObjectNameOffset = 0xB4;
        internal const int GameObjectIsBobbing = 0x104;
        internal const int GameObjectLocation = 0x140;

        #endregion

        #region Player unit

        internal static readonly int UnitDescriptors = 0x4;
        internal static readonly int UnitCastingID = 0xF60; // Script_UnitCastingInfo //
        internal static readonly int UnitChannelingID = 0xF80; // Script_UnitChannelInfo //
        internal static readonly int UnitLocation = 0xA90;
        internal static readonly int UnitRotation = UnitLocation + 0x10;
        internal static readonly int UnitNameCacheGUIDOffset = 0x10;
        internal static readonly int UnitNameCacheNameOffset = 0x21;

        internal const int UnitTargetGUID = 0xA0;
        internal const int UnitClass = 0xE1;
        internal const int UnitHealth = 0xEC;
        internal const int UnitPower = 0xF0;
        internal const int UnitHealthMax = 0x108;
        internal const int UnitPowerMax = 0x10C;
        internal const int UnitLevel = 0x154;
        internal const int UnitRace = 0x15C;

        #endregion

        #region NPC

        internal static readonly int NpcNameBase = 0xC04;
        internal static readonly int NpcNameOffset = 0x7C;

        #endregion
    
    }
}