### Interval=250
if (UnitHealth("player") / UnitHealthMax("player") < 0.4) then
	CastSpellByName("Исцеляющий всплеск")
end
if (GetSpellCooldown("Тотем исцеляющего потока") == 0) then
	CastSpellByName("Тотем исцеляющего потока")
end
if (UnitAffectingCombat("player")) then
	if (UnitExists("target") and GetSpellCooldown("Цепная молния") == 0) then
		CastSpellByName("Цепная молния", "target")
	end
	if (GetSpellCooldown("Гром и молния") == 0) then
		CastSpellByName("Гром и молния")
	end
end