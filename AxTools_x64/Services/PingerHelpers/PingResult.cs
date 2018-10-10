namespace AxTools.Services.PingerHelpers
{
    internal struct PingerStat
    {
        internal int MaxPing;
        internal int NumPingFailedFromTenAttempts;

        internal PingerStat(int maxPing, int numPingFailedFromTenAttempts)
        {
            MaxPing = maxPing;
            NumPingFailedFromTenAttempts = numPingFailedFromTenAttempts;
        }
    }
}