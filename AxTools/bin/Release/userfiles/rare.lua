### Interval=50
RunMacroText("/tar Зандаларский завоеватель")
if (UnitHealth("target") > 0) then
	CastSpellByName("Огненный шок")
end