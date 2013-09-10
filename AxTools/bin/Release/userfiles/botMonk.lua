### Interval=250
if (UnitAffectingCombat("player") and not IsMounted()) then
	if (GetSpellCooldown("Рука-копье") == 0) then
		if (UnitCanAttack("player", "target")) then
			local name, _, _, _, _, castEnd, _, _, isInt = UnitCastingInfo("target")
			if (not name) then return end
			if (isInt == false) then
				local cTime = GetTime() * 1000
				if (castEnd - cTime < 1500 and castEnd - cTime > 200) then
					print("|cff33ff99AxTools|r: Interrupting...")
					SpellStopCasting()
					CastSpellByName("Рука-копье", "target")
				end
			end
		end
	end
end