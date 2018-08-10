using System;
using System.IO;
using System.Security.Cryptography;

namespace AxTools.Helpers
{
    internal static class Crypt
    {
        internal static byte[] Encrypt<T>(byte[] bytes, byte[] key) where T : SymmetricAlgorithm, new()
        {
            using (T cryptoProvider = new T())
            {
                cryptoProvider.GenerateIV();
                byte[] iv = cryptoProvider.IV;
                using (ICryptoTransform encryptor = cryptoProvider.CreateEncryptor(TruncateHash(key, cryptoProvider.KeySize / 8), iv))
                {
                    byte[] cipherBytes;
                    using (MemoryStream cipherMemoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(cipherMemoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
                            {
                                binaryWriter.Write(bytes);
                            }
                            cipherBytes = cipherMemoryStream.ToArray();
                        }
                    }
                    using (MemoryStream finalMemoryStream = new MemoryStream())
                    {
                        using (BinaryWriter binaryWriter = new BinaryWriter(finalMemoryStream))
                        {
                            binaryWriter.Write(iv);
                            binaryWriter.Write(cipherBytes);
                            binaryWriter.Flush();
                        }
                        return finalMemoryStream.ToArray();
                    }
                }
            }
        }

        internal static byte[] Decrypt<T>(byte[] bytes, byte[] key) where T : SymmetricAlgorithm, new()
        {
            using (T cryptoProvider = new T())
            {
                byte[] iv = new byte[cryptoProvider.BlockSize / 8];
                Array.Copy(bytes, 0, iv, 0, iv.Length);
                using (ICryptoTransform decrypter = cryptoProvider.CreateDecryptor(TruncateHash(key, cryptoProvider.KeySize / 8), iv))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream decrypterStream = new CryptoStream(memoryStream, decrypter, CryptoStreamMode.Write))
                        {
                            using (BinaryWriter binaryWriter = new BinaryWriter(decrypterStream))
                            {
                                binaryWriter.Write(bytes, iv.Length, bytes.Length - iv.Length);
                            }
                        }
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        private static byte[] TruncateHash(byte[] key, int length)
        {
            using (SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider())
            {
                byte[] hash = provider.ComputeHash(key);
                Array.Resize(ref hash, length);
                return hash;
            }
        }
    }
}