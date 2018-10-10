using System;
using System.Security.Cryptography;
using System.Text;

namespace Dummy
{
    internal class CSPRNG : IDisposable
    {
        private readonly RNGCryptoServiceProvider cryptoServiceProvider;

        internal CSPRNG()
        {
            cryptoServiceProvider = new RNGCryptoServiceProvider();
        }

        public void Dispose()
        {
            cryptoServiceProvider.Dispose();
        }

        private double Sample()
        {
            byte[] bytes = new byte[8];
            cryptoServiceProvider.GetBytes(bytes);
            int num0 = BitConverter.ToInt32(bytes, 0);
            int num1 = BitConverter.ToInt32(bytes, 4);
            double raw = Math.Abs((double) num0/num1);
            long integral = (long) raw;
            return raw - integral;
        }

        internal int Next()
        {
            byte[] bytes = new byte[4];
            cryptoServiceProvider.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        internal int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            long range = (long)maxValue - minValue;
            if (range <= int.MaxValue)
            {
                return (int)(Sample() * range) + minValue;
            }
            return (int)((long)(Sample() * range) + minValue);
        }

        internal void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            cryptoServiceProvider.GetBytes(buffer);
        }

        internal string GetRandomString(int size)
        {
            StringBuilder builder = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                int random = Next(65, 91); // 26 letters, from 65
                char ch = Convert.ToChar(random);
                if (Next() % 2 == 0)
                {
                    ch = ch.ToString().ToLower()[0];
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

    }
}
