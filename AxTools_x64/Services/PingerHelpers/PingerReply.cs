namespace AxTools.Services.PingerHelpers
{
    internal class PingerReply
    {
        internal int PingInMs;
        internal bool Successful;

        internal PingerReply(int pingInMs, bool successful)
        { 
            PingInMs = pingInMs;
            Successful = successful;
        }
    }
}
