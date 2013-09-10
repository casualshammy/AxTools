### Interval=100
local spellNames = {"Спячка", "Смерч", "Превращение", "Господство над разумом", "Сглаз", "Страх", "Очарование"};
local interruptSpell = "Пронизывающий ветер";
local unitsToWatch = {"arena1", "arena2"};
if (GetSpellCooldown(interruptSpell) ~= 0) then
	if (GetSpellCooldown("Тотем заземления") == 0) then
		interruptSpell = "Тотем заземления";
	else
		return;
	end
end
for _, v in pairs(unitsToWatch) do
	if (UnitCanAttack("player", v)) then
		local name, _, _, _, _, castEnd, _, _, isInt = UnitCastingInfo(v);
		local cTime = GetTime() * 1000;
		if (isInt == false and tContains(spellNames, name)) then
			if (castEnd - cTime < 800 and castEnd - cTime > 150) then
				SpellStopCasting();
				CastSpellByName(interruptSpell, v);
				return;
			end
		end
		name, _, _, _, _, castEnd, _, isInt = UnitChannelInfo(v);
		if (isInt == false and tContains(spellNames, name)) then
			if (castEnd - cTime > 1000) then
				SpellStopCasting();
				CastSpellByName(interruptSpell, v);
				return;
			end
		end
	end
end