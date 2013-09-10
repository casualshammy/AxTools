### Interval=1000
if (UnitPower("player", 0) / UnitPowerMax("player", 0) < 0.8) then
	if (GetInventoryItemCooldown("player", 13) == 0 and InCombatLockdown()) then
		UseInventoryItem(14);
	end
end