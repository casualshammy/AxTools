----- Interval=5000
for index=1, MAX_WORLD_PVP_QUEUES do
   local status, mapName = GetWorldPVPQueueStatus(index)
   if (status == "confirm") then
		PlaySoundFile("sound\\interface\\ui_raidbosswhisperwarning.ogg");
		BattlefieldMgrEntryInviteResponse(24, 1);
   end
end