using AxTools.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace AxTools.WoW
{
    [Serializable]
    [DataContract(Name = "WowAccount")]
    internal class WoWAccount
    {
        [DataMember(Name = "WowAccountLogin")]
        internal string Login;

        [DataMember(Name = "WowAccountPassword")]
        internal string Password;

        [JsonConstructor]
        internal WoWAccount(string login, string password)
        {
            Login = login;
            Password = password;
        }

        private static readonly object _lock = new object();
        private static ObservableCollection<WoWAccount> _list;
        internal static ObservableCollection<WoWAccount> AllAccounts
        {
            get
            {
                lock (_lock)
                {
                    return _list ?? (_list = Load());
                }
            }
        }

        private static ObservableCollection<WoWAccount> Load()
        {
            try
            {
                if (Settings.Instance.WoWAccounts.Length > 0)
                {
                    byte[] strangeBytes = {0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48};
                    byte[] bytes = Crypt.Decrypt<RijndaelManaged>(Settings.Instance.WoWAccounts, strangeBytes);
                    ObservableCollection<WoWAccount> list = JsonConvert.DeserializeObject<ObservableCollection<WoWAccount>>(Encoding.UTF8.GetString(bytes));
                    Log.Print("WoW accounts was loaded");
                    return list;
                }
                return new ObservableCollection<WoWAccount>();
            }
            catch (Exception ex)
            {
                Log.Print("WoW accounts loading failed: " + ex.Message, true);
                return new ObservableCollection<WoWAccount>();
            }
        }

        internal static void Save()
        {
            try
            {
                if (_list != null)
                {
                    string json = JsonConvert.SerializeObject(_list);
                    Log.Print(json);
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