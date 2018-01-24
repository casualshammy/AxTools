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
		internal static readonly int GameState = 0x16A179E; // CGGameUI::LeaveWorld (or Script_IsPlayerInWorld (2) or Script_PlaySound) (it's equal to 2 if in game, 1 if loading from login screen)
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

		// HOW TO FIND "PlayerGUID"
		// Search for "REALM_SEPARATORS" string after "FriendList.cpp" string. Call before "FriendList.cpp" is GetLocalPlayerGUID()

		// HOW TO FIND "ObjectManager"
		// Search for "object manager" string

		// HOW TO FIND Auras info
		// Look into functions in "Script_UnitAura"

		// HOW TO FIND PlayerIsLooting
		// Look into functions in "Script_GetNumLootItems"

		#endregion

		#region Build info

		internal static readonly byte[] WoWHash =
		{
			0x8C, 0xE7, 0x6A, 0x09, 0xF4, 0x54, 0x7D, 0x14, 0x34, 0x1B, 0x4E, 0x5C, 0x07, 0x73, 0x7C, 0xB0, 0x3D, 0xFF, 0x30, 0x24, 0xDB, 0xEF, 0x62, 0xB4, 0x1E, 0x76, 0xFA, 0xCE, 0xC5, 0x8B, 0xFA, 0x32
		};

		#endregion

		#region Static infos

		internal const int MouseoverGUID = 0x1BF3360;
		internal const int PlayerIsLooting = 0x1C14228;
		internal const int UIFrameBase = 0x18C7E30;
		internal const int GlueState = 0x1962089;
		internal const int LastHardwareAction = 0x18C7E38;
		internal const int ChatIsOpened = 0x18DF594;
		internal const int GameState = 0x1BF330F;
		internal const int KnownSpellsCount = 0x1BF4890;
		internal const int FocusedWidget = 0x18C7DE0;
		internal const int NotLoadingScreen = 0x160D624;
		internal const int TickCount = 0x18C6E8C;
		internal const int BlackMarketNumItems = 0x1C4A8D0;
		internal const int PlayerGUID = 0x1C84D40;
		internal const int PlayerZoneID = 0x1C08774;
		internal const int KnownSpells = 0x1BF4898;
		internal const int BlackMarketItems = 0x1C4A8D8;
		internal const int ChatBuffer = 0x1B956F0;
		internal const int ObjectManager = 0x197D230;

		internal const int NameCacheBase = 0x166A918;

		#endregion

		#region UIFrame

		internal const int UIFirstFrame = 0xCD0;
		internal const int UINextFrame = 0xCC0;
		internal const int UIFrameVisible = 0xC8;
		internal const int UIFrameVisible1 = 0x13;
		internal const int UIFrameVisible2 = 1;
		internal const int UIFrameName = 0x20;
		internal const int UIEditBoxText = 0x238;

		#endregion

		#region Object manager

		internal const int ObjectManagerFirstObject = 0x18;
		internal const int ObjectManagerNextObject = 0x70;
		internal const int ObjectType = 0x20;
		internal const int ObjectGUID = 0x58;

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
		internal static readonly int UnitCastingID = 0x1C98;
		internal static readonly int UnitChannelingID = 0x1CC8;
		internal static readonly int UnitLocation = 0x1588;
		internal static readonly int UnitRotation = UnitLocation + 0x10;

		internal const int UnitTargetGUID = 0xA0;
		internal const int UnitClass = 0xD5;
		internal const int UnitHealth = 0xE0;
		internal const int UnitPower = 0xE8;
		internal const int UnitHealthMax = 0x100;
		internal const int UnitPowerMax = 0x108;
		internal const int UnitLevel = 0x150;
		internal const int UnitRace = 0x168;
		internal const int UnitFlags = 0x184;
		internal const int UnitMountDisplayID = 0x1B0;

		internal const int AuraCount1 = 0x27E8;
		internal const int AuraCount2 = 0x1DE8;
		internal const int AuraTable1 = 0x1DEC;
		internal const int AuraTable2 = 0x1DF0;

		internal const int NameCacheNext = 0x00;
		internal const int NameCacheGuid = 0x20;
		internal const int NameCacheName = 0x31;

		internal const int PlayerInvSlots = 0x10F4;

		internal const int PlayerSpeedBase = 0x210;
		internal const int PlayerSpeedOffset = 0xA4;

		internal const int PlayerIsLootingOffset0 = 0x1588;
		internal const int PlayerIsLootingOffset1 = 0xA08;

		#endregion

		#region NPC

		internal static readonly int NpcNameBase = 0x1740;
		internal static readonly int NpcNameOffset = 0xA0;

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