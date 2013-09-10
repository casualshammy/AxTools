### Interval=1000
local wTable = GetQuestsCompleted()
local wIDs = {485,836,351,648,25475,25476}
for index, value in pairs(wTable) do
	if (tContains(wIDs, index)) then
		print("Completed: "..index, value)
	end
end