namespace AxTools.Services.PingerHelpers
{
    internal static class GameServers
    {
        internal const string WoWAutodetect = "WoW - Autodetect";
        internal static SrvAddress AlwaysValidAddress = new SrvAddress("google.com", 80, "Google");

        internal static SrvAddress[] Entries = 
        {
            new SrvAddress(string.Empty, 0, "Disabled"),
            AlwaysValidAddress,
            new SrvAddress(string.Empty, 3724, WoWAutodetect)
        };

    }
}
