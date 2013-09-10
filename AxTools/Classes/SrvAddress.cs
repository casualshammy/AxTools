namespace AxTools.Classes
{
    internal class SrvAddress
    {
        internal string Ip;
        internal int Port;
        internal string Description;

        internal SrvAddress(string ip, int port, string description)
        {
            Ip = ip;
            Port = port;
            Description = description;
        }

    }
}