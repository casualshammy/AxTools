### Interval=100
if (not UnitIsDeadOrGhost("player")) then
	if (GetSpellCooldown("Пронизывающий ветер") == 0) then
		if (UnitCanAttack("player", "focus")) then
			local name, _, _, _, _, castEnd, _, _, isInt = UnitCastingInfo("focus")
			if (not name) then return end
			if (isInt == false and (name == "Водяная стрела" or name == "Молния")) then
				local cTime = GetTime() * 1000
				if (castEnd - cTime < 500 and castEnd - cTime > 100) then
					print("|cff33ff99AxTools|r: Interrupting...")
					SpellStopCasting()
					CastSpellByName("Пронизывающий ветер", "focus")
				end
			end
		end
	end
end