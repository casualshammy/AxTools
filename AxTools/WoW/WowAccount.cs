using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using AxTools.Classes;
using Newtonsoft.Json;

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

        [JsonConstructor]
        internal WowAccount(string login, string password)
        {
            Login = login;
            Password = password;
        }

        private static readonly object _lock = new object();
        private static List<WowAccount> _list;
        internal static List<WowAccount> AllAccounts
        {
            get
            {
                lock (_lock)
                {
                    return _list ?? (_list = Load());
                }
            }
        }

        private static List<WowAccount> Load()
        {
            try
            {
                if (Settings.Instance.WoWAccounts.Length > 0)
                {
                    byte[] strangeBytes = {0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48};
                    byte[] bytes = Crypt.Decrypt<RijndaelManaged>(Settings.Instance.WoWAccounts, strangeBytes);
                    List<WowAccount> list = JsonConvert.DeserializeObject<List<WowAccount>>(Encoding.UTF8.GetString(bytes));
                    Log.Print("WoW accounts was loaded");
                    return list;
                }
                return new List<WowAccount>();
            }
            catch (Exception ex)
            {
                Log.Print("WoW accounts loading failed: " + ex.Message, true);
                return new List<WowAccount>();
            }
        }

        internal static void Save()
        {
            try
            {
                if (_list != null)
                {
                    string json = JsonConvert.SerializeObject(_list);
                    byte[] strangeBytes = { 0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48 };
                    Settings.Instance.WoWAccounts = Crypt.Encrypt<RijndaelManaged>(Encoding.UTF8.GetBytes(json), strangeBytes);
                }
            }
            catch (Exception ex)
            {
                Log.Print("WoW accounts saving failed: " + ex.Message, true);
            }
        }
    
    }
}