using System.Reflection;

namespace AxTools.WoW
{
	[Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
	internal static class WowBuildInfoX64
	{

		#region Comments

		//internal const int PlayerZoneID = 0x1735880; // use CheatEngine
		//internal const int ChatBuffer = 0x1736250;
		//internal const int PlayerPtr = 0x1694B10; // Script_GetCurrentTitle
		//internal const int NameCacheBase = 0x0;
		//internal const int GameState = 0x1734705;
		//internal const int GlueState = 0x155862C; // Search for string "Copy request %d finished"
		//internal const int LastHardwareAction = 0x14BD468;
		//internal const int FocusedWidget = 0x14B1010; // Script_GetCurrentKeyBoardFocus; Frame__HasFocus
		//internal const int BlackMarketNumItems = 0x17E6FC0; // Script_C_BlackMarket.GetItemInfoByIndex
		//internal const int Possible_NotLoadingScreen = 0x11FC734; // go to upper functions, look for strings there
		//internal const int KnownSpellsCount = 0x17937A0; // Script_CastSpellByID
		//internal const int BlackMarketItems = 0x17E6FC8; // Script_C_BlackMarket.GetItemInfoByIndex
		//internal const int KnownSpells = 0x17937A8; // Script_CastSpellByID
		//internal const int MouseoverGUID = 0x17359F0; // Search for string "Current Object Track"
		//internal const int TickCount = 0x14B038C; // Uper function: Script_GetSessionTime
		//internal const int PlayerIsLooting = 0x17AA2CD;
		//internal const int ChatIsOpened = 0x14CA470; // CheatEngine :(
		//internal const int ObjectManager = 0x156EC50;
		//internal const int PlayerName = 0x181F0B0;
		//internal const int UIFrameBase = 0x14BD460; // Script_EnumerateFrames

		/*
		internal static readonly int LastHardwareAction = 0x1446208; // [int] CGGameUI::UpdatePlayerAFK / WRITE
		internal static readonly int TickCount = 0x1439160; // [int]
		internal static readonly int GameState = 0x16A179E; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound)
		internal static readonly int Possible_NotLoadingScreen = 0x135474C;
		//internal static readonly int PlayerName = 0x17EF7E0; // ClientServices::GetCharacterName (or Script_UnitName/GetPlayerName)
		//internal static readonly int PlayerRealm = 0x17EF996; // Гордунни = D0 93 D0 BE D1 80 D0 B4 D1 83 D0 BD D0 BD D0 B8 // Черный Шрам = D0 A7 D0 B5 D1 80 D0 BD D1 8B D0 B9 20 D0 A8 D1 80 D0 B0 D0 BC
		internal static readonly int PlayerZoneID = 0x16A1820; // CGGameUI::NewZoneFeedback (16) (or Script_GetRaidRosterInfo (101))
		internal static readonly int PlayerIsLooting = 0x1715264; // [byte] CGPlayer_C::IsLooting (17) (or Script_SetLootPortrait (32) or Script_GetContainerPurchaseInfo)
		internal static readonly int ObjectManager = 0x14E4DC0; // ClntObjMgrPush (7)
		internal static readonly int PlayerPtr = 0x1606320; // [IntPtr] ClntObjMgrGetActivePlayerObj
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
            0x99, 0x2B, 0xDA, 0x72, 0x54, 0xA9, 0x55, 0x28, 0xA5, 0xE5, 0xA0, 0x6F, 0x75, 0x53, 0xA5, 0x1E, 0x2F, 0x6C, 0xFF, 0x53, 0xB3, 0x3B, 0x4F, 0xA1, 0x30, 0xA3, 0x9D, 0x0B, 0x3F, 0x95, 0xB1, 0x07
        };

        #endregion

        #region Static infos

        internal const int KnownSpells = 0x1922398;
        internal const int NotLoadingScreen = 0x1352674;
        internal const int GlueState = 0x169C3C4;
        internal const int LastHardwareAction = 0x160D9F8;
        internal const int GameState = 0x18C329C;
        internal const int ChatBuffer = 0x18C4D70;
        internal const int UIFrameBase = 0x160D9F0;
        internal const int MouseoverGUID = 0x18C44B0;
        internal const int KnownSpellsCount = 0x1922390;
        internal const int PlayerPtr = 0x1820A40;
        internal const int TickCount = 0x16007DC;
        internal const int ChatIsOpened = 0x1622024;
        internal const int BlackMarketNumItems = 0x19771E0;
        internal const int ObjectManager = 0x16B5C40;
        internal const int PlayerZoneID = 0x1933480;
        internal const int FocusedWidget = 0x1601490;
        internal const int BlackMarketItems = 0x19771E8;
        internal const int PlayerName = 0x19B1550;
        internal const int PlayerIsLooting = 0x193E22D;
        internal const int NameCacheBase = 0x166A918;

		#endregion

		#region UIFrame

		internal const int UIFirstFrame = 0xCB0;
		internal const int UINextFrame = 0xCA0;
		internal const int UIFrameVisible = 0xC8;
		internal const int UIFrameVisible1 = 0x13;
		internal const int UIFrameVisible2 = 1;
		internal const int UIFrameName = 0x20;
		internal const int UIEditBoxText = 0x238;

		#endregion

		#region Object manager

		internal static readonly int ObjectManagerFirstObject = 0x18;
		internal static readonly int ObjectManagerNextObject = 0x70;
		internal static readonly int ObjectType = 0x20;
		internal const int ObjectGUID = 0x58; // 0x50?

		#endregion

		#region Game object

		internal static readonly int GameObjectOwnerGUIDBase = 0x10;
		internal static readonly int GameObjectOwnerGUIDOffset = 0x30;
		internal static readonly int GameObjectEntryID = 0x24;
		internal static readonly int GameObjectNameBase = 0x478;
		internal static readonly int GameObjectNameOffset = 0xD8;
		internal const int GameObjectIsBobbing = 0x1C4;
		internal const int GameObjectLocation = 0x228;

		#endregion

		#region Player unit

		internal static readonly int UnitDescriptors = 0x10;
		internal static readonly int UnitCastingID = 0x1C6C;
		internal static readonly int UnitChannelingID = 0x1CA0;
		internal static readonly int UnitLocation = 0x1560;
		internal static readonly int UnitRotation = UnitLocation + 0x10;

		internal const int UnitTargetGUID = 0xA0;
		internal const int UnitClass = 0xD5;
		internal const int UnitHealth = 0xE0;
		internal const int UnitPower = 0xE8;
		internal const int UnitHealthMax = 0x100;
		internal const int UnitPowerMax = 0x108;
		internal const int UnitLevel = 0x150;
		internal const int UnitRace = 0x164;
		internal const int UnitFlags = 0x180;
		internal const int UnitMountDisplayID = 0x1AC;

		internal const int AuraCount1 = 0x26C0;
		internal const int AuraCount2 = 0x1DC0;
		internal const int AuraTable1 = 0x1DC4;
		internal const int AuraTable2 = 0x1DC8;

		internal const int NameCacheNext = 0x00;
		internal const int NameCacheGuid = 0x20;
		internal const int NameCacheName = 0x31;

		internal const int PlayerInvSlots = 0x10F4;

		internal const int PlayerSpeedBase = 0x210;
		internal const int PlayerSpeedOffset = 0xA4;

		#endregion

		#region NPC

		internal static readonly int NpcNameBase = 0x1718; // OK
		internal static readonly int NpcNameOffset = 0xA0; // OK

		internal const int NpcDynamicFlags = 0x28;

		#endregion

		#region WoWItem

		internal const int WoWItemContainedIn = 0x40;
		internal const int WoWItemStackCount = 0x70;
		internal const int WoWItemEnchantment = 0x90;

		internal const int WoWContainerItems = 0x154;

		#endregion

		#region GameChat

		internal const int ChatNextMessage = 0x17F0; // OK
		internal const int ChatFullMessageOffset = 0x65; // OK

		#endregion

	}
}