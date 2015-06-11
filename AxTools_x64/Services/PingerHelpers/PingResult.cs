namespace AxTools.Services.PingerHelpers
{
    internal struct PingResult
    {
        internal int Ping;
        internal int PacketLoss;

        internal PingResult(int ping, int packetLoss)
        {
            Ping = ping;
            PacketLoss = packetLoss;
        }
    }

}
