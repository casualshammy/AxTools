namespace AxTools.WoW
{
    internal static class WowBuildInfo
    {

        #region Build info

        internal static readonly byte[] WoWHash =
        {
            0x0C, 0xEC, 0x9B, 0xCC, 0xB2, 0xBF, 0x49, 0x23, 0x71, 0x2A, 0xB8, 0x73, 0x3D, 0x2D, 0x15, 0xB7, 0x6E, 0xF1, 0x10, 0x18, 0x2C, 0xFF, 0x2A, 0x2F, 0x81, 0xE6, 0xED, 0x7C, 0x1B, 0x41, 0x0C, 0x1A
        };
        
        #endregion
        
        #region Static infos

        internal static readonly int LastHardwareAction = 0xCB63F0; // CGGameUI::UpdatePlayerAFK / WRITE
        internal static readonly int GameState = 0xE35B2E; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound)
        internal static readonly int PlayerName = 0xF24D30; // ClientServices::GetCharacterName (or Script_UnitName/GetPlayerName)
        internal static readonly int PlayerRealm = 0xF24EDE; // Гордунни = D0 93 D0 BE D1 80 D0 B4 D1 83 D0 BD D0 BD D0 B8 // Черный Шрам = D0 A7 D0 B5 D1 80 D0 BD D1 8B D0 B9 20 D0 A8 D1 80 D0 B0 D0 BC
        internal static readonly int PlayerZoneID = 0xE35B78; // CGGameUI::NewZoneFeedback (16) (or Script_GetRaidRosterInfo (101))
        internal static readonly int PlayerIsLooting = 0xE54970; // CGPlayer_C::IsLooting (17) (or Script_SetLootPortrait (32) or Script_GetContainerPurchaseInfo)
        internal static readonly int BlackMarketNumItems = 0xE922B0;
        internal static readonly int BlackMarketItems = 0xE922B4;
        internal static readonly int UnitNameCachePointer = 0xD12734;// + 0x8;
        internal static readonly int ObjectManager = 0xD3B358; // ClntObjMgrPush (7)
        internal static readonly int PlayerPtr = 0xDC0BC8; // ClntObjMgrGetActivePlayerObj
        internal static readonly int GlueState = 0xD1D878;
        internal static readonly int FocusedWidget = 0xCB60B8;
        
        #endregion

        #region Injected methods

        internal static readonly int ClntObjMgrGetActivePlayerObj = 0x3B0E; // ClntObjMgrGetActivePlayerObj
        internal static readonly int LuaDoStringAddress = 0x2513D; // FrameScript_ExecuteBuffer
        internal static readonly int LuaGetLocalizedTextAddress = 0x2EA052; // FrameScript_GetLocalizedText
        internal static readonly int TargetUnit = 0x947048; // CGGameUI::Target
        internal static readonly int Interact = 0x94929D; // CGGameUI::Interact
        internal static readonly int HandleTerrainClick = 0x256A23; // Spell_C__HandleTerrainClick
        internal static readonly int ClickToMove = 0x2F9D1C; // CGUnit_C::InitializeTrackingState

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
        internal static readonly int GameObjectNameBase = 0x274;
        internal static readonly int GameObjectNameOffset = 0xB4;
        //internal static readonly int GameObjectLocation = 0x138;
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