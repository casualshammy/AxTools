### Interval=100
if (GetSpellCooldown("Очищение духа") == 0) then
	for i = 1, 10 do
		local unit = "raid"..tostring(i)
	    for k = 1, 40 do
	        local spellName, _, _, _, _, _, _, _, _, _, spellID = UnitDebuff(unit, k)
	        if (not spellName) then break end
	        if (spellName == "Слепящее солнце" or spellName == "Каменный взгляд") then
	            --SpellStopCasting()
	            CastSpellByName("Очищение духа", unit)
	            return
	        end
	    end
	end
end