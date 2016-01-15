using System;
using System.Net.Sockets;
using System.Text;

namespace Dummy.TS3
{
    internal class TelnetConnection
    {
        private readonly TcpClient tcpSocket;

        private int timeOutMs = 100;

        public TelnetConnection(string hostname, int port)
        {
            tcpSocket = new TcpClient(hostname, port);
        }

        public string Login(string username, string password, int loginTimeOutMs)
        {
            int oldTimeOutMs = timeOutMs;
            timeOutMs = loginTimeOutMs;
            string s = Read();
            if (!s.TrimEnd().EndsWith(":"))
                throw new Exception("Failed to connect : no login prompt");
            WriteLine(username);

            s += Read();
            if (!s.TrimEnd().EndsWith(":"))
                throw new Exception("Failed to connect : no password prompt");
            WriteLine(password);

            s += Read();
            timeOutMs = oldTimeOutMs;
            return s;
        }

        public void WriteLine(string cmd)
        {
            Write(cmd + "\n");
        }

        public void Write(string cmd)
        {
            if (!tcpSocket.Connected) return;
            byte[] buf = Encoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }

        public string Read()
        {
            if (!tcpSocket.Connected) return null;
            StringBuilder sb = new StringBuilder();
            do
            {
                ParseTelnet(sb);
                System.Threading.Thread.Sleep(timeOutMs);
            } while (tcpSocket.Available > 0);
            return sb.ToString();
        }

        public bool IsConnected
        {
            get { return tcpSocket.Connected; }
        }

        internal void Disconnect()
        {
            if (tcpSocket != null && IsConnected)
            {
                tcpSocket.Close();
            }
        }

        private async void ParseTelnet(StringBuilder sb)
        {
            while (tcpSocket.Available > 0)
            {
                byte[] buffer = new byte[tcpSocket.Available];
                await tcpSocket.GetStream().ReadAsync(buffer, 0, buffer.Length);
                string ch = Encoding.UTF8.GetString(buffer);
                sb.Append(ch);


                //int input = tcpSocket.GetStream().ReadByte();
                //switch (input)
                //{
                //    case -1:
                //        break;
                //    case (int)Verbs.IAC:
                //        // interpret as command
                //        int inputverb = tcpSocket.GetStream().ReadByte();
                //        if (inputverb == -1) break;
                //        switch (inputverb)
                //        {
                //            case (int)Verbs.IAC:
                //                //literal IAC = 255 escaped, so append char 255 to string
                //                sb.Append(inputverb);
                //                break;
                //            case (int)Verbs.DO:
                //            case (int)Verbs.DONT:
                //            case (int)Verbs.WILL:
                //            case (int)Verbs.WONT:
                //                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                //                int inputoption = tcpSocket.GetStream().ReadByte();
                //                if (inputoption == -1) break;
                //                tcpSocket.GetStream().WriteByte((byte)Verbs.IAC);
                //                if (inputoption == (int)Options.SGA)
                //                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                //                else
                //                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                //                tcpSocket.GetStream().WriteByte((byte)inputoption);
                //                break;
                //        }
                //        break;
                //    default:
                //        sb.Append((char)input);
                //        break;
                //}
            }
        }
    }
}