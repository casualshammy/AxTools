namespace AxTools.WoW
{
    internal static class WowBuildInfoX64
    {

        #region Build info

        internal static readonly byte[] WoWHash =
        {
            0x49, 0xD0, 0xDB, 0x46, 0x43, 0xBF, 0x98, 0xA4, 0xB2, 0x5E, 0xB8, 0x83, 0xBD, 0x3D, 0x74, 0x55, 0x19, 0x97, 0xC5, 0xFF, 0x29, 0xB0, 0x46, 0x4B, 0xC5, 0x8F, 0xDF, 0xE4, 0xD4, 0x27, 0x07, 0x4A
        };
        
        #endregion
        
        #region Static infos

        internal static readonly int LastHardwareAction = 0x138B0F8; // [int] CGGameUI::UpdatePlayerAFK / WRITE
        internal static readonly int TickCount = 0x137E050; // [int]
        internal static readonly int GameState = 0x15C1596; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound)
        internal static readonly int PlayerName = 0x16BB4C0; // ClientServices::GetCharacterName (or Script_UnitName/GetPlayerName)
        internal static readonly int PlayerRealm = 0x16BB676; // Гордунни = D0 93 D0 BE D1 80 D0 B4 D1 83 D0 BD D0 BD D0 B8 // Черный Шрам = D0 A7 D0 B5 D1 80 D0 BD D1 8B D0 B9 20 D0 A8 D1 80 D0 B0 D0 BC
        internal static readonly int PlayerZoneID = 0x15C1618; // CGGameUI::NewZoneFeedback (16) (or Script_GetRaidRosterInfo (101))
        internal static readonly int PlayerIsLooting = 0x15E2A14; // [byte] CGPlayer_C::IsLooting (17) (or Script_SetLootPortrait (32) or Script_GetContainerPurchaseInfo)
        internal static readonly int BlackMarketNumItems = 0x1623CD8;
        internal static readonly int BlackMarketItems = 0x1623CE0;
        internal static readonly int ObjectManager = 0x141DC70; // ClntObjMgrPush (7)
        internal static readonly int PlayerPtr = 0x1528450; // ClntObjMgrGetActivePlayerObj
        internal static readonly int GlueState = 0x1408A84; // dword
        internal static readonly int FocusedWidget = 0x138ACA8; // qword
        
        #endregion

        #region Injected methods

        internal static readonly int FrameScript_ExecuteBuffer = 0x38FF0;
        internal static readonly int FrameScript_GetLocalizedText = 0x4E2BE0;
        internal static readonly int CGGameUI_Target = 0x6709D0;
        internal static readonly int CGGameUI_Interact = 0x674170;
        internal static readonly int CGUnit_C_InitializeTrackingState = 0x4FF410;
        internal const int CGWorldFrame_Render = 0x682180;
        internal const int HookLength = 12;
        internal static readonly byte[] HookPattern = { 0x48, 0x89, 0x5C, 0x24, 0x08, 0x48, 0x89, 0x74, 0x24, 0x10, 0x57, 0x48 };

        #endregion

        #region Object manager

        internal static readonly int ObjectManagerFirstObject = 0x18;
        internal static readonly int ObjectManagerNextObject = 0x68;
        internal static readonly int ObjectType = 0x18;
        internal const int ObjectGUID = 0x50; // declared as const because it used in WoWObjectsInfo

        #endregion

        #region Game object

        internal static readonly int GameObjectOwnerGUIDBase = 0x8;
        internal static readonly int GameObjectOwnerGUIDOffset = 0x30;
        internal static readonly int GameObjectEntryID = 0x24;
        internal static readonly int GameObjectNameBase = 0x498;
        internal static readonly int GameObjectNameOffset = 0xD8;
        internal const int GameObjectIsBobbing = 0x1E0;
        internal const int GameObjectLocation = 0x248;

        #endregion

        #region Player unit

        internal static readonly int UnitDescriptors = 0x8;
        internal static readonly int UnitCastingID = 0x1B30; // Script_UnitCastingInfo //
        internal static readonly int UnitChannelingID = 0x1B50; // Script_UnitChannelInfo //
        internal static readonly int UnitLocation = 0x14E8;
        internal static readonly int UnitRotation = UnitLocation + 0x10;
        //internal static readonly int UnitNameCacheGUIDOffset = 0x10;
        //internal static readonly int UnitNameCacheNameOffset = 0x21;

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

        internal static readonly int NpcNameBase = 0x1690;
        internal static readonly int NpcNameOffset = 0xA0;
        
        internal const int NpcDynamicFlags = 0x28;

        #endregion

    }
}