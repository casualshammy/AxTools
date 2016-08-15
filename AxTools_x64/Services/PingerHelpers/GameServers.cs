namespace AxTools.Services.PingerHelpers
{
    internal static class GameServers
    {
        internal static SrvAddress[] Entries = 
        {
            new SrvAddress(string.Empty, 0, "Disabled"),
            new SrvAddress("google.com", 80, "Google"),
            new SrvAddress("109.105.134.173", 7777, "Lineage 2 - Athebaldt"),
            new SrvAddress("37.244.58.78", 3724, "World of Warcraft - Blackscar"),
            new SrvAddress("37.244.59.24", 3724, "World of Warcraft - Gordunni"),
            new SrvAddress("195.12.240.179", 3724, "World of Warcraft - Ravencrest (Frankfurt)"),
            new SrvAddress("199.107.24.244", 3724, "World of Warcraft - Area 52 (New York)"),
            new SrvAddress("195.12.235.9", 3724, "World of Warcraft - Argent Dawn (Paris)"),
            new SrvAddress("199.108.48.250", 3724, "World of Warcraft - Stormreaver (US)"),
            new SrvAddress("195.12.246.224", 3724, "World of Warcraft - Deathguard")
        };

    }
}
