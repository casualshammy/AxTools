using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using AxTools.Classes;

namespace AxTools.WoW
{
    [Serializable]
    [DataContract(Name = "WowAccount")]
    internal class WowAccount
    {
        [DataMember(Name = "WowAccountLogin")]
        internal string Login;

        [DataMember(Name = "WowAccountPassword")]
        internal string Password;

        internal WowAccount(string login, string password)
        {
            Login = login;
            Password = password;
        }

        private static readonly object _lock = new object();
        private static List<WowAccount> _list = new List<WowAccount>();
        internal static List<WowAccount> AllAccounts
        {
            get
            {
                lock (_lock)
                {
                    return _list;
                }
            }
        }

        internal static void LoadFromDisk()
        {
            try
            {
                if (File.Exists(Globals.WowAccountsFilePath))
                {
                    byte[] strangeBytes = { 0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48 };
                    byte[] bytes = Crypt.Decrypt<RijndaelManaged>(File.ReadAllBytes(Globals.WowAccountsFilePath), strangeBytes);
                    using (MemoryStream memoryStream = new MemoryStream(bytes))
                    {
                        _list = (List<WowAccount>)new DataContractJsonSerializer(_list.GetType()).ReadObject(memoryStream);
                    }
                    Log.Print("WoW accounts was loaded");
                }
                else
                {
                    Log.Print("WoW accounts file not found");
                }
            }
            catch (Exception ex)
            {
                Log.Print("WoW accounts loading failed: " + ex.Message, true);
            }
        }

        internal static void SaveToDisk()
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    new DataContractJsonSerializer(_list.GetType()).WriteObject(memoryStream, _list);
                    byte[] strangeBytes = { 0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48 };
                    byte[] bytes = Crypt.Encrypt<RijndaelManaged>(memoryStream.ToArray(), strangeBytes);
                    File.WriteAllBytes(Globals.WowAccountsFilePath, bytes);
                }
            }
            catch (Exception ex)
            {
                Log.Print("WoW accounts saving failed: " + ex.Message, true);
            }
        }
    
    }
}