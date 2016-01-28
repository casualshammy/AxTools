namespace AxTools.Services.PingerHelpers
{
    internal struct PingerStat
    {
        internal int Ping;
        internal int PacketLoss;

        internal PingerStat(int ping, int packetLoss)
        {
            Ping = ping;
            PacketLoss = packetLoss;
        }
    }

}
