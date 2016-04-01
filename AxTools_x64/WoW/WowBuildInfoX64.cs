using System.Reflection;

namespace AxTools.WoW
{
    [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
    internal static class WowBuildInfoX64
    {

        #region Comments

        /*
        internal static readonly int LastHardwareAction = 0x1446208; // [int] CGGameUI::UpdatePlayerAFK / WRITE
        internal static readonly int TickCount = 0x1439160; // [int]
        internal static readonly int GameState = 0x16A179E; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound)
        internal static readonly int Possible_NotLoadingScreen = 0x135474C;
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
         * 
         * corpseOwnerGUID = 0x728;
        */

        // KnownSpells: lookfor "if ( (signed int)sub_750490(v2, v3) >= 0 )" in Script_IsSpellKnown
        // PlayerSpeed: normal speed: 100% = 7f, 110% = 7.7f

        /* NameCache
         * internal const int NameCacheBase = 0x1316E98;
         * base address = 13FEA0000


            14057CED0 - 49 89 00  - mov [r8],rax
            14057CED3 - 49 89 40 08  - mov [r8+08],rax
            14057CED7 - 49 8B 49 08  - mov rcx,[r9+08] <<
            14057CEDB - 49 89 08  - mov [r8],rcx
            14057CEDE - 48 8B 41 08  - mov rax,[rcx+08]

            RAX=000000003F90F8E0
            RBX=000000003F90F8E0
            RCX=00000000526FDBA0
            RDX=000000003F90F8E0
            RSI=000000000000001B
            RDI=00000001411B6E88
            RSP=000000000013C1E8
            RBP=0000000000000000
            RIP=000000014057CEDB
            R8=000000003F90F8F0
            R9=00000001411B6E90
            R10=000000003F90F8E0
            R11=0000000049AF9658
            R12=0000000000000000
            R13=0000000001D942F0
            R14=000000013FEA0000
            R15=0000000000000000





            14057CEE2 - 49 89 40 08  - mov [r8+08],rax
            14057CEE6 - 48 89 51 08  - mov [rcx+08],rdx
            14057CEEA - 4D 89 41 08  - mov [r9+08],r8 <<
            14057CEEE - C3 - ret 
            14057CEEF - CC - int 3 

            RAX=00000001411B6E99
            RBX=000000003F90F8E0
            RCX=00000000526FDBA0
            RDX=000000003F90F8E0
            RSI=000000000000001B
            RDI=00000001411B6E88
            RSP=000000000013C1E8
            RBP=0000000000000000
            RIP=000000014057CEEE
            R8=000000003F90F8F0
            R9=00000001411B6E90
            R10=000000003F90F8E0
            R11=0000000049AF9658
            R12=0000000000000000
            R13=0000000001D942F0
            R14=000000013FEA0000
            R15=0000000000000000




            14057CED7 - 49 8B 49 08  - mov rcx,[r9+08]
            14057CEEA - 4D 89 41 08  - mov [r9+08],r8
         * 
        */

        #endregion

        #region Build info

        internal static readonly byte[] WoWHash =
        {
            0xE1, 0x8D, 0xCE, 0xD9, 0x48, 0xEF, 0x6C, 0x8A, 0x2F, 0x14, 0xF2, 0xCA, 0xF3, 0x4F, 0xD5, 0x7D, 0x29, 0xC8, 0x27, 0xC7, 0x2B, 0xDD, 0xCD, 0xC8, 0x86, 0x02, 0xEE, 0xCF, 0x56, 0x79, 0x38, 0x40
        };

        #endregion

        #region Static infos

        internal const int PlayerZoneID = 0x1519B00;
        internal const int ZoneText = 0x1519AA0;
        internal const int NameCacheBase = 0x1316E98;
        internal const int LastHardwareAction = 0x12BF688;
        internal const int PlayerPtr = 0x147E680;
        internal const int ChatBuffer = 0x151BD20;
        internal const int GlueState = 0x1347858;
        internal const int GameState = 0x1519A7E;
        internal const int FrameScript_ExecuteBuffer = 0x3DB20;
        internal const int FocusedWidget = 0x12BF240;
        internal const int CGWorldFrame_Render = 0x701DB0;
        internal const int KnownSpellsCount = 0x1577BC0;
        internal const int BlackMarketNumItems = 0x15CE6E8;
        internal const int MouseoverGUID = 0x151A0B8;
        internal const int TickCount = 0x12B25E0;
        internal const int PlayerIsLooting = 0x158D1A4;
        internal const int Possible_NotLoadingScreen = 0x11D288C;
        internal const int KnownSpells = 0x1577BC8;
        internal const int BlackMarketItems = 0x15CE6F0;
        internal const int ChatIsOpened = 0x12CD4B0;
        internal const int ObjectManager = 0x135D120;
        internal const int PlayerName = 0x1616F10;

        internal static readonly byte[] HookPattern = {0x48, 0x89, 0x7C, 0x24, 0x20, 0x55, 0x48, 0x8D, 0x6C, 0x24, 0xA0, 0x48, 0x81, 0xEC, 0x60, 0x01, 0x00, 0x00};
        internal static readonly int HookLength = HookPattern.Length;

        //internal static readonly int LastHardwareAction = 0x1446208;
        //internal static readonly int TickCount = 0x1439160;
        //internal static readonly int GameState = 0x16A179E;
        //internal static readonly int Possible_NotLoadingScreen = 0x135474C;
        //internal static readonly int PlayerZoneID = 0x16A1820;
        //internal static readonly int PlayerIsLooting = 0x1715264;
        //internal static readonly int BlackMarketNumItems = 0x1756528;
        //internal static readonly int BlackMarketItems = 0x1756530;
        //internal static readonly int ObjectManager = 0x14E4DC0;
        //internal static readonly int PlayerPtr = 0x1606320;
        //internal static readonly int GlueState = 0x14CE2E4;
        //internal static readonly int FocusedWidget = 0x1445DB8;
        //internal const int NameCacheBase = 0x14C2818;
        //internal const int MouseoverGUID = 0x16A1DD8;
        //internal const int KnownSpells = 0x16FFC28;
        //internal const int KnownSpellsCount = 0x16FFC20;
        //internal const int ChatIsOpened = 0x1454030;
        //internal const int CVarAutoInteract = 0x16A1988;
        //internal const int ZoneText = 0x16A17C0;
        //internal const int ChatBuffer = 0x16A3B30;

        #endregion

        #region Injected methods



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
        internal const int UnitFlags = 0x17C;

        internal const int AuraCount1 = 0x2390;
        internal const int AuraCount2 = 0x1D10;
        internal const int AuraTable1 = 0x1D14;
        internal const int AuraTable2 = 0x1D18;

        internal const int NameCacheNext = 0x00;
        internal const int NameCacheGuid = 0x20;
        internal const int NameCacheName = 0x31;

        internal const int PlayerInvSlots = 0x1020;

        internal const int PlayerSpeedBase = 0x230;
        internal const int PlayerSpeedOffset = 0xA0;

        #endregion

        #region NPC

        internal static readonly int NpcNameBase = 0x16F0;
        internal static readonly int NpcNameOffset = 0xA0;

        internal const int NpcDynamicFlags = 0x28;

        #endregion

        #region WoWItem

        internal const int WoWItemContainedIn = 0x40;
        internal const int WoWItemStackCount = 0x70;
        internal const int WoWItemEnchantment = 0x90;

        internal const int WoWContainerItems = 0x148;

        #endregion

        #region Misc

        internal const int CVarAutoInteract_Enabled = 92;
        internal const int ChatNextMessage = 0x17F0;
        internal const int ChatFullMessageOffset = 0x65;

        #endregion

    }
}