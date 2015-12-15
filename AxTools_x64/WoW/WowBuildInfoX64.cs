using System.Reflection;

namespace AxTools.WoW
{
    [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
    internal static class WowBuildInfoX64
    {

        #region Build info

        internal static readonly byte[] WoWHash =
        {
            0x4F, 0xA1, 0xBE, 0x44, 0xD3, 0xED, 0x08, 0x8F, 0xE8, 0x56, 0x68, 0x67, 0xFC, 0xBF, 0x83, 0xB1, 0xD4, 0xC9, 0x56, 0x36, 0xCA, 0x90, 0xB8, 0x2F, 0xC9, 0x48, 0x18, 0xF3, 0x4E, 0x52, 0xC5, 0x85
        };
        
        #endregion
        
        #region Static infos

        internal static readonly int LastHardwareAction = 0x1446208; // [int] CGGameUI::UpdatePlayerAFK / WRITE
        internal static readonly int TickCount = 0x1439160; // [int]
        internal static readonly int GameState = 0x16A179E; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound)
        //internal static readonly int PlayerName = 0x17EF7E0; // ClientServices::GetCharacterName (or Script_UnitName/GetPlayerName)
        //internal static readonly int PlayerRealm = 0x17EF996; // Гордунни = D0 93 D0 BE D1 80 D0 B4 D1 83 D0 BD D0 BD D0 B8 // Черный Шрам = D0 A7 D0 B5 D1 80 D0 BD D1 8B D0 B9 20 D0 A8 D1 80 D0 B0 D0 BC
        internal static readonly int PlayerZoneID = 0x16A1820; // CGGameUI::NewZoneFeedback (16) (or Script_GetRaidRosterInfo (101))
        internal static readonly int PlayerIsLooting = 0x1715264; // [byte] CGPlayer_C::IsLooting (17) (or Script_SetLootPortrait (32) or Script_GetContainerPurchaseInfo)
        internal static readonly int BlackMarketNumItems = 0x1756528; // [uint]
        internal static readonly int BlackMarketItems = 0x1756530; // [IntPtr]
        internal static readonly int ObjectManager = 0x14E4DC0; // ClntObjMgrPush (7)
        internal static readonly int PlayerPtr = 0x1606320; // [IntPtr] ClntObjMgrGetActivePlayerObj
        internal static readonly int GlueState = 0x14CE2E4; // dword
        internal static readonly int FocusedWidget = 0x1445DB8; // qword
        
        #endregion

        #region Injected methods

        internal static readonly int FrameScript_ExecuteBuffer = 0x3CD20;
        internal static readonly int FrameScript_GetLocalizedText = 0x556920;
        internal static readonly int CGGameUI_Target = 0x6ED970;
        internal static readonly int CGGameUI_Interact = 0x6F0640;
        internal static readonly int CGUnit_C_InitializeTrackingState = 0x5736F0;
        internal const int CGWorldFrame_Render = 0x701280; // Wow-64.exe+701280
        internal const int HookLength = 12;
        internal static readonly byte[] HookPattern = {0x48, 0x89, 0x7C, 0x24, 0x20, 0x55, 0x48, 0x8D, 0x6C, 0x24, 0xA0, 0x48};

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
        internal static readonly int UnitCastingID = 0x1B98; // Script_UnitCastingInfo //
        internal static readonly int UnitChannelingID = 0x1BB8; // Script_UnitChannelInfo //
        internal static readonly int UnitLocation = 0x1548;
        internal static readonly int UnitRotation = UnitLocation + 0x10;

        internal const int UnitTargetGUID = 0xA0;
        internal const int UnitClass = 0xE5;
        internal const int UnitHealth = 0xF0;
        internal const int UnitPower = 0xF4;
        internal const int UnitHealthMax = 0x10C;
        internal const int UnitPowerMax = 0x110;
        internal const int UnitLevel = 0x158;
        internal const int UnitRace = 0x160;

        #endregion

        #region NPC

        internal static readonly int NpcNameBase = 0x16F0;
        internal static readonly int NpcNameOffset = 0xA0;
        
        internal const int NpcDynamicFlags = 0x28;

        #endregion

    }
}