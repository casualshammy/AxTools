### Interval=200
if (GetSpellCooldown("Развеивание магии") == 0) then
	if (UnitBuff("focus", "Щит Тьмы")) then
		SpellStopCasting()
		CastSpellByName("Развеивание магии", "focus")
	end
end