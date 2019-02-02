using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AxTools.Helpers;
using FMemoryPattern;
using Newtonsoft.Json;

namespace AxTools.WoW
{
	[Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
	internal static class WowBuildInfoX64
	{

		static WowBuildInfoX64()
		{
			WoWHash = new byte[] { 0x9E, 0x84, 0x56, 0x56, 0x43, 0x04, 0xD2, 0xE3, 0x87, 0x18, 0xBC, 0x2F, 0x58, 0x17, 0xB8, 0xA1, 0xF1, 0xE9, 0x7D, 0xA1, 0x85, 0x34, 0x31, 0x94, 0x46, 0x36, 0xF2, 0x5B, 0x0E, 0x9B, 0xE3, 0x8E };
            BlackMarketNumItems = 0x2902008;
            KnownSpellsCount = 0x28AAD00;
            ChatBuffer = 0x2877810;
            ObjectManager = 0x251EBC8;
            PlayerGUID = 0x2994990;
            GameState = 0x2876944;
            TickCount = 0x2430A3C;
            ChatIsOpened = 0x2460424;
            MouseoverGUID = 0x2876968;
            BlackMarketItems = 0x2902010;
            IsChatAFK = 0x2877804;
            FocusedWidget = 0x2431520;
            NotLoadingScreen = 0x211E33C;
            PlayerZoneID = 0x28763BC;
            GlueState = 0x2437B71;
            KnownSpells = 0x28AAD08;
            PlayerIsLooting = 0x28DA688;
            UIFrameBase = 0x2431430;
            LastHardwareAction = 0x2431438;
            NameCacheBase = 0x166A918;
		}

		#region patterns

		private static readonly MemoryPattern[] patterns =
		{
			MemoryPattern.FromTextstyle(nameof(BlackMarketNumItems),    "8B 15 ?? ?? ?? ?? 33 C0 F2 4C 0F 2C C0 85 D2 74 xx 4C 8B 0D xx xx xx xx 49 8B C9 44 39 01 74",             new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(BlackMarketItems),       "8B 15 xx xx xx xx 33 C0 F2 4C 0F 2C C0 85 D2 74 xx 4C 8B 0D ?? ?? ?? ?? 49 8B C9 44 39 01 74",             new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(LastHardwareAction),     "48 83 EC 28 2B 0D ?? ?? ?? ?? 8D 81 20 6C FB FF 85 C0 78 xx 8D 81 C0 88 E4 FF 85 C0 78 xx E8",             new LeaModifier(LeaType.E8)),
			MemoryPattern.FromTextstyle(nameof(TickCount),              "0F 57 C0 8B C1 89 0D ?? ?? ?? ?? C7 05 xx xx xx xx 00 00 00 00 F3 48 0F 2A C0 F3 0F 59 05 xx xx xx xx",    new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(MouseoverGUID),          "E8 xx xx xx xx 0F 10 xx xx 0F 11 05 ?? ?? ?? ?? E8 xx xx xx xx 0F BE D0",                                  new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(ChatIsOpened),           "83 3D ?? ?? ?? ?? 00 48 8B CB 7E xx 48 8B D0 EB xx 33 D2 E8 xx xx xx xx E8",                               new LeaModifier(LeaType.Cmp)),
			MemoryPattern.FromTextstyle(nameof(FocusedWidget),          "48 8B CB E8 xx xx xx xx 33 C9 48 39 0D ?? ?? ?? ??",                                                       new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(ObjectManager),          "57 41 56 41 57 48 83 EC 30 4C 8B 05 ?? ?? ?? ??",                                                          new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(GlueState),              "80 3D xx xx xx xx 00 75 19 80 3D ?? ?? ?? ?? 00 74 10 80 7B 20 00 74 0A 48 83 C4 20 5B",                   new LeaModifier(LeaType.Cmp)),
			MemoryPattern.FromTextstyle(nameof(GameState),              "48 83 EC 58 0F B6 05 ?? ?? ?? ?? A8 10 74 44 0F B6 C8 0F BA F1 04",                                        new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(KnownSpellsCount),       "8B C2 C3 44 8B 0D ?? ?? ?? ?? 33 D2 45 85 C9 74 23 4C 8B 15 xx xx xx xx",                                  new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(KnownSpells),            "8B C2 C3 44 8B 0D xx xx xx xx 33 D2 45 85 C9 74 23 4C 8B 15 ?? ?? ?? ??",                                  new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(UIFrameBase),            "E8 xx xx xx xx 48 8B CD 4C 89 35 ?? ?? ?? ?? E8",                                                          new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(PlayerZoneID),           "E8 xx xx xx xx 48 85 C0 0F 44 1D ?? ?? ?? ?? 8B C3",                                                       new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(PlayerIsLooting),        "8B D7 48 8D 0D ?? ?? ?? ?? E8 xx xx xx xx 80 38 00 74",                                                    new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(PlayerGUID),             "48 8D 05 ?? ?? ?? ?? 41 B8 03 00 00 00 0F 1F 00",                                                          new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(NotLoadingScreen),       "48 83 EC 28 80 3D ?? ?? ?? ?? 00 0F 84 xx xx xx xx 83 3D xx xx xx xx 00 48",                               new LeaModifier(LeaType.Cmp)),
			MemoryPattern.FromTextstyle(nameof(IsChatAFK),              "39 1D ?? ?? ?? ?? 74 xx 33 D2 49 8B CF E8",                                                                new LeaModifier(LeaType.CmpMinusOne)),
			MemoryPattern.FromTextstyle(nameof(ChatBuffer),             "48 69 D8 B8 0C 00 00 48 8D 05 ?? ?? ?? ?? 48 03 D8 48 8D 8B E6 00 00 00",                                  new LeaModifier(LeaType.CmpMinusOne)),
		};

        private static bool TryGetSavedOffsets(byte[] hash)
        {
            Log2 log = new Log2(nameof(WowBuildInfoX64));
            string hashAsString = BitConverter.ToString(hash);
            var directory = Directory.CreateDirectory(Path.Combine(AppFolders.DataDir, "wow-builds"));
            var filePath = Path.Combine(directory.FullName, hashAsString + ".json");
            log.Info($"TryGetSavedOffsets: filepath: '{filePath}'");
            if (File.Exists(filePath))
            {
                log.Info($"TryGetSavedOffsets: file exists, trying to deserialize...");
                try
                {
                    var offsets = JsonConvert.DeserializeObject<Dictionary<string, IntPtr>>(File.ReadAllText(filePath, Encoding.UTF8));
                    if (offsets.Count == patterns.Length)
                    {
                        log.Info("TryGetSavedOffsets: deserialization is successful!");
                        WoWHash = hash;
                        BlackMarketNumItems = offsets[nameof(BlackMarketNumItems)].ToInt32();
                        BlackMarketItems = offsets[nameof(BlackMarketItems)].ToInt32();
                        LastHardwareAction = offsets[nameof(LastHardwareAction)].ToInt32();
                        TickCount = offsets[nameof(TickCount)].ToInt32();
                        MouseoverGUID = offsets[nameof(MouseoverGUID)].ToInt32();
                        ChatIsOpened = offsets[nameof(ChatIsOpened)].ToInt32();
                        FocusedWidget = offsets[nameof(FocusedWidget)].ToInt32();
                        ObjectManager = offsets[nameof(ObjectManager)].ToInt32();
                        GlueState = offsets[nameof(GlueState)].ToInt32();
                        GameState = offsets[nameof(GameState)].ToInt32();
                        KnownSpellsCount = offsets[nameof(KnownSpellsCount)].ToInt32();
                        KnownSpells = offsets[nameof(KnownSpells)].ToInt32();
                        UIFrameBase = offsets[nameof(UIFrameBase)].ToInt32();
                        PlayerZoneID = offsets[nameof(PlayerZoneID)].ToInt32();
                        PlayerIsLooting = offsets[nameof(PlayerIsLooting)].ToInt32();
                        PlayerGUID = offsets[nameof(PlayerGUID)].ToInt32();
                        NotLoadingScreen = offsets[nameof(NotLoadingScreen)].ToInt32();
                        IsChatAFK = offsets[nameof(IsChatAFK)].ToInt32();
                        ChatBuffer = offsets[nameof(ChatBuffer)].ToInt32();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"TryGetSavedOffsets: deserialization is failed: {ex.Message}");
                }
            }
            return false;
        }

		internal static bool TryFindNewOffsets(FMemory.MemoryManager memoryManager, byte[] executableHash)
		{
            if (TryGetSavedOffsets(executableHash))
                return true;
			Notify.TrayPopup("This WoW version is unsupported", nameof(AxTools) + " is trying to adapt to the new version. Please wait...", NotifyUserType.Warn, true);
			var sb = new StringBuilder();
            sb.AppendLine("0x" + BitConverter.ToString(executableHash).Replace("-", ", 0x"));
			sb.AppendLine($"WoW build: {FileVersionInfo.GetVersionInfo(memoryManager.Process.MainModule.FileName).FilePrivatePart}");
			sb.AppendLine($"Base address: 0x{memoryManager.ImageBase.ToInt64().ToString("X")}");
			sb.AppendLine($"Size of Wow.exe: 0x{memoryManager.Process.MainModule.ModuleMemorySize.ToString("X")}");
			var offsets = new ConcurrentDictionary<string, IntPtr>();
			Parallel.ForEach(patterns, l => {
				var results = l.Find(memoryManager).ToArray();
				if (results.Length == 1)
					offsets.AddOrUpdate(l.Name, results.First().Address, (s, ptr) => results.First().Address);
			});
            foreach (var entry in offsets)
            {
                sb.AppendLine($"{entry.Key} = 0x{entry.Value.ToInt32().ToString("X")};");
            }
            if (offsets.Count == patterns.Length)
                File.WriteAllText(Path.Combine(AppFolders.TempDir, "new-offsets.txt"), sb.ToString(), Encoding.UTF8);
            else
                File.WriteAllText(Path.Combine(AppFolders.TempDir, "new-offsets-incomplete.txt"), sb.ToString(), Encoding.UTF8);
            if (offsets.Count == patterns.Length)
			{
				WoWHash = executableHash;
				BlackMarketNumItems = offsets[nameof(BlackMarketNumItems)].ToInt32();
				BlackMarketItems = offsets[nameof(BlackMarketItems)].ToInt32();
				LastHardwareAction = offsets[nameof(LastHardwareAction)].ToInt32();
				TickCount = offsets[nameof(TickCount)].ToInt32();
				MouseoverGUID = offsets[nameof(MouseoverGUID)].ToInt32();
				ChatIsOpened = offsets[nameof(ChatIsOpened)].ToInt32();
				FocusedWidget = offsets[nameof(FocusedWidget)].ToInt32();
				ObjectManager = offsets[nameof(ObjectManager)].ToInt32();
				GlueState = offsets[nameof(GlueState)].ToInt32();
				GameState = offsets[nameof(GameState)].ToInt32();
				KnownSpellsCount = offsets[nameof(KnownSpellsCount)].ToInt32();
				KnownSpells = offsets[nameof(KnownSpells)].ToInt32();
				UIFrameBase = offsets[nameof(UIFrameBase)].ToInt32();
				PlayerZoneID = offsets[nameof(PlayerZoneID)].ToInt32();
				PlayerIsLooting = offsets[nameof(PlayerIsLooting)].ToInt32();
				PlayerGUID = offsets[nameof(PlayerGUID)].ToInt32();
				NotLoadingScreen = offsets[nameof(NotLoadingScreen)].ToInt32();
				IsChatAFK = offsets[nameof(IsChatAFK)].ToInt32();
				ChatBuffer = offsets[nameof(ChatBuffer)].ToInt32();
                var directory = Directory.CreateDirectory(Path.Combine(AppFolders.DataDir, "wow-builds"));
                var filePath = Path.Combine(directory.FullName, BitConverter.ToString(executableHash) + ".json");
                File.WriteAllText(filePath, JsonConvert.SerializeObject(offsets), Encoding.UTF8);
				Notify.TrayPopup("Adaptation is successful", $"You can use {nameof(AxTools)} as usual", NotifyUserType.Info, true);
				return true;
			}
			Notify.TrayPopup("Adaptation is failed", $"Please wait for update", NotifyUserType.Error, true);
			return false;
		}

		#endregion

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

		// HOW TO FIND PlayerZoneID
		// CheatEngine :(

		// GameState: 3 - loading from character screen, 2 - almost loaded, 4 - in game?

		// Bags info:

		#endregion Comments

		#region Static infos

		internal static byte[] WoWHash { get; private set; }
		
		internal static int GlueState { get; private set; }
		internal static int UIFrameBase { get; private set; }
		internal static int BlackMarketItems { get; private set; }
		internal static int BlackMarketNumItems { get; private set; }
		internal static int NotLoadingScreen { get; private set; }
		internal static int MouseoverGUID { get; private set; }
		internal static int PlayerZoneID { get; private set; }
		internal static int GameState { get; private set; }
		internal static int LastHardwareAction { get; private set; }
		internal static int PlayerIsLooting { get; private set; }
		internal static int TickCount { get; private set; }
		internal static int KnownSpellsCount { get; private set; }
		internal static int ChatIsOpened { get; private set; }
		internal static int KnownSpells { get; private set; }
		internal static int IsChatAFK { get; private set; }
		internal static int ObjectManager { get; private set; }
		internal static int PlayerGUID { get; private set; }
		internal static int FocusedWidget { get; private set; }
		internal static int ChatBuffer { get; private set; }
		internal static int NameCacheBase { get; private set; }

		#endregion Static infos

		#region UIFrame

		internal const int UIFirstFrame = 0xCD0;
		internal const int UINextFrame = 0xCC0;
		internal const int UIFrameVisible = 0xC8;
		internal const int UIFrameVisible1 = 0xA;
		internal const int UIFrameVisible2 = 1;
		internal const int UIFrameName = 0x20;
		internal const int UIEditBoxText = 0x238;
		internal const int UIFrameIsEnabled0 = 0xF4;
		internal const int UIFrameIsEnabled1 = 3;
		internal const int UIFrameIsEnabled2 = 1;

		#endregion UIFrame

		#region Object manager

		internal const int ObjectManagerFirstObject = 0x18;
		internal const int ObjectManagerNextObject = 0x70;
		internal const int ObjectType = 0x20;
		internal const int ObjectGUID = 0x58;

		#endregion Object manager

		#region Game object

		internal static readonly int GameObjectOwnerGUIDBase = 0x10;
		internal static readonly int GameObjectOwnerGUIDOffset = 0x1C;
		internal static readonly int GameObjectEntryID = 0x10;
		internal static readonly int GameObjectNameBase = 0x478;
		internal static readonly int GameObjectNameOffset = 0xE0;
		internal const int GameObjectIsBobbing = 0x14C;
		internal const int GameObjectLocation = 0x1b0;

		#endregion Game object

		#region Player unit

		internal static readonly int UnitDescriptors = 0x10;
		internal static readonly int UnitCastingID = 0x1920;
		internal static readonly int UnitChannelingID = 0x1950;
		internal static readonly int UnitLocation = 0x1588;
		internal static readonly int UnitRotation = UnitLocation + 0x10;
		internal static readonly int UnitPitch = UnitRotation + 0x4;

		internal const int UnitTargetGUID = 0x9c;
		internal const int UnitClass = 0xD1;
		internal const int UnitHealth = 0xDC;
		internal const int UnitPower = 0xE4;
		internal const int UnitHealthMax = 0xfc;
		internal const int UnitPowerMax = 0x104;
		internal const int UnitLevel = 0x14C;
		internal const int UnitRace = 0x170;
		internal const int UnitFlags = 0x18C;
		internal const int UnitMountDisplayID = 0x1C0;

		internal const int AuraCount1 = 0x24D8;
		internal const int AuraCount2 = 0x1A58;
		internal const int AuraTable1 = 0x1A58;
		internal const int AuraTable2 = 0x1A60;

		internal const int NameCacheNext = 0x00;
		internal const int NameCacheGuid = 0x20;
		internal const int NameCacheName = 0x31;

		internal const int ActivePlayer_Inventory = 0x1D74;
		internal const int ActivePlayer_Containers = ActivePlayer_Inventory + 19 * 16; // 19 WoWGUIDs of items in active player's inventory
		internal const int ActivePlayer_Backpack = ActivePlayer_Containers + 4 * 16; // 4 active player's containers

		internal const int PlayerSpeedBase = 0x198;
		internal const int PlayerSpeedOffset = 0xA4;

		internal const int PlayerIsLootingOffset0 = 0x1588;
		internal const int PlayerIsLootingOffset1 = 0xA08;

		#endregion Player unit

		#region NPC

		internal static readonly int NpcNameBase = 0x1740;
		internal static readonly int NpcNameOffset = 0xE0;

		internal const int NpcDynamicFlags = 10;

		#endregion NPC

		#region WoWItem

		internal const int WoWItemContainedIn = 0x2c;
		internal const int WoWItemStackCount = 0x5c;
		internal const int WoWItem_WeaponEnchant = 0x88;

		internal const int WoWContainerItems = 0x140;

		#endregion WoWItem

		#region GameChat

		internal const int ChatNextMessage = 0xCB8; // OK
		internal const int ChatFullMessageOffset = 0xE6; // OK
		internal const int ChatSenderGuid = 0x00;
		internal const int ChatSenderName = 0x034;
		internal const int ChatType = 0xCA0;
		internal const int ChatChannelNum = 0xCA4;
		internal const int ChatTimeStamp = 0xCB0;

		#endregion GameChat
	}
}