### Interval=250
if (UnitAffectingCombat("player") and not IsMounted()) then
	if (GetSpellCooldown("Пронизывающий ветер") == 0) then
		if (UnitCanAttack("player", "target")) then
			local name, _, _, _, _, castEnd, _, _, isInt = UnitCastingInfo("target")
			if (not name) then return end
			if (isInt == false) then
				local cTime = GetTime() * 1000
				if (castEnd - cTime < 1000 and castEnd - cTime > 200) then
					print("|cff33ff99AxTools|r: Interrupting...")
					SpellStopCasting()
					CastSpellByName("Пронизывающий ветер", "target")
				end
			end
		end
	end
	local healthPercent = UnitHealth("player") / UnitHealthMax("player");
	if (healthPercent < 0.8 and GetSpellCooldown("Тотем исцеляющего потока") == 0) then
		CastSpellByName("Тотем исцеляющего потока");
	elseif (healthPercent < 0.4 and GetSpellCooldown("Тотем целительного прилива") == 0) then
		CastSpellByName("Тотем целительного прилива");
	end
end