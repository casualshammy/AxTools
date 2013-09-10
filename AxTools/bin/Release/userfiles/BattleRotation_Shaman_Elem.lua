### Interval=250
if (select(2, UnitClass("player")) ~= "SHAMAN") then
	print("This script works only for shamans!")
	return
end
AxExec_var2 = "target"
if (not UnitCanAttack("player", AxExec_var2) or UnitIsDead(AxExec_var2)) then return end-- or not UnitAffectingCombat(AxExec_var2)
if (GetSpellCooldown("Молния") ~= 0 or UnitChannelInfo("player") or UnitCastingInfo("player")) then return end
if (not UnitAura("player", "Щит молний")) then
	CastSpellByName("Щит молний")
	return
end
local _, totemName = GetTotemInfo(1)
if (totemName ~= "Тотем магмы" and totemName ~= "Опаляющий тотем" and totemName ~= "Тотем элементаля огня") then
	CastSpellByName("Опаляющий тотем")
	return
end
if (UnitAura("player", "Перерождение")) then
	local t13, _, u13 = GetInventoryItemCooldown("player", 13)
	local t14, _, u14 = GetInventoryItemCooldown("player", 14)
	if (t13 == 0 and u13 == 1) then
		UseInventoryItem(13)
	end
	if (t14 == 0 and u14 == 1) then
		UseInventoryItem(14)
	end
end
if (GetSpellCooldown("Перерождение") == 0 and not UnitAura("player", "Перерождение")) then
	CastSpellByName("Перерождение")
	return
end
if (not UnitDebuff(AxExec_var2, "Огненный шок")) then
	CastSpellByName("Огненный шок", AxExec_var2)
	return
end
if (IsSpellKnown(117014) == true and GetSpellCooldown("Удар духов стихии") == 0) then
	CastSpellByName("Удар духов стихии", AxExec_var2)
	return
end
if (GetSpellCooldown("Выброс лавы") == 0 and UnitDebuff(AxExec_var2, "Огненный шок")) then
	CastSpellByName("Выброс лавы", AxExec_var2)
	return
end
if (select(4, UnitBuff("player", "Щит молний")) >= 7) then
	CastSpellByName("Земной шок", AxExec_var2)
	return
end
CastSpellByName("Молния", AxExec_var2)