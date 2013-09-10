### Interval=500
if (UnitAffectingCombat("player") and not IsMounted()) then
	if (UnitHealth("player") / UnitHealthMax("player") < 0.4) then
		if (GetItemCooldown(57191) == 0 and GetItemCount(57191) > 0) then
			UseItemByName(57191)
			print("Легендарное лечебное зелье")
		end
		if (GetItemCooldown(5512) == 0 and GetItemCount(5512) > 0) then
			UseItemByName(5512)
			print("Камень здоровья")
		end
	end
	if (UnitHealth("player") / UnitHealthMax("player") < 0.8) then
		if (GetSpellCooldown(5730) == 0) then
			CastSpellByName("Тотем каменного когтя")
			print("Тотем каменного когтя")
		end
	end
end