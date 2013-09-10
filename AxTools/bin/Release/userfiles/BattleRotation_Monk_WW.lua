### Interval=250
if (select(2, UnitClass("player")) ~= "MONK") then
	print("This script works only for monks!");
	return;
end
local unitToAttack = "target";
local deathNoteSpellName = "Приговор";
local tigerPowerSpellName = "Сила тигра";
local proc0SpellName = "Неожиданный прием: нокаутирующий удар";
local proc1SpellName = "Неожиданный прием: Лапа тигра";
local buff0Name = "Отвар тигриной силы";
if (not UnitCanAttack("player", unitToAttack) or UnitIsDead(unitToAttack)) then -- or not UnitAffectingCombat(unitToAttack)
	return;
end
if (GetSpellCooldown(100780) ~= 0 or UnitChannelInfo("player") or UnitCastingInfo("player")) then
	return;
end
local chiPower = UnitPower("player", 12);
local myEnergy = UnitPower("player", 3);
-- buffs
local buffName, _, _, buffCount = UnitBuff("player", buff0Name);
if (buffName) then
	if (buffCount >= 10) then
		CastSpellByID(116740);
		return;
	end
end
if (myEnergy < 40 and GetSpellCooldown(115288) == 0) then
	CastSpellByID(115288);
	return;
end

-- battle rotation
if (UnitAura("player", deathNoteSpellName) and chiPower >= 3) then
	CastSpellByID(115080, unitToAttack); -- Touch of Death
	return;
end
if (chiPower >= 2 and GetSpellCooldown(107428) == 0) then
	CastSpellByID(107428, unitToAttack); --Rising Sun Kick
	return;
end
if (not UnitAura("player", tigerPowerSpellName) and chiPower >= 1) then
	CastSpellByID(100787, unitToAttack); --Tiger Palm
	return;
end
if (chiPower >= 4) then
	CastSpellByID(100784, unitToAttack); --Blackout Kick
	return;
end
if (UnitAura("player", proc0SpellName)) then
	CastSpellByID(100784, unitToAttack); --Blackout Kick
	return;
end
if (UnitAura("player", proc1SpellName)) then
	CastSpellByID(100787, unitToAttack); --Tiger Palm
	return;
end
if (chiPower < 2 and UnitHealth("player") / UnitHealthMax("player") < 0.8 and myEnergy >= 40) then
	CastSpellByID(115072, unitToAttack); --Expel Harm
	return;
end
if (chiPower < 4 and myEnergy >= 40) then
	CastSpellByID(100780, unitToAttack); -- Jab
	return;
end