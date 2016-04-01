namespace AxTools.Services.PingerHelpers
{
    internal struct PingerStat
    {
        internal int Ping;
        internal int PacketLoss;
        internal bool PingDataIsRelevant;

        internal PingerStat(int ping, int packetLoss, bool pingDataIsRelevant)
        {
            Ping = ping;
            PacketLoss = packetLoss;
            PingDataIsRelevant = pingDataIsRelevant;
        }
    }

}
