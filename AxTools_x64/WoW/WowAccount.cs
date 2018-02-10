﻿using System.Reflection;
using AxTools.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace AxTools.WoW
{
    [Serializable]
    [DataContract(Name = "WowAccount")]
    internal class WoWAccount
    {
        private static readonly Log2 log = new Log2("WoWAccount");

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

        [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
        [Obfuscation(Exclude = false, Feature = "constants")]
        private static ObservableCollection<WoWAccount> Load()
        {
            try
            {
                if (Settings2.Instance.WoWAccounts.Length > 0)
                {
                    byte[] strangeBytes = {0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48};
                    byte[] bytes = Crypt.Decrypt<RijndaelManaged>(Settings2.Instance.WoWAccounts, strangeBytes);
                    ObservableCollection<WoWAccount> list = JsonConvert.DeserializeObject<ObservableCollection<WoWAccount>>(Encoding.UTF8.GetString(bytes));
                    list.CollectionChanged += WoWAccounts_Changed;
                    log.Info("WoW accounts was loaded");
                    return list;
                }
                ObservableCollection<WoWAccount> emptyList = new ObservableCollection<WoWAccount>();
                emptyList.CollectionChanged += WoWAccounts_Changed;
                log.Info("WoW accounts: new collection is created");
                return emptyList;
            }
            catch (Exception ex)
            {
                log.Error("WoW accounts loading failed, new accounts will not be saved: " + ex.Message);
                return new ObservableCollection<WoWAccount>();
            }
        }

        [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
        [Obfuscation(Exclude = false, Feature = "constants")]
        private static void WoWAccounts_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (_list != null)
                {
                    string json = JsonConvert.SerializeObject(_list);
                    byte[] strangeBytes = {0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48};
                    Settings2.Instance.WoWAccounts = Crypt.Encrypt<RijndaelManaged>(Encoding.UTF8.GetBytes(json), strangeBytes);
                    log.Info("WoW accounts have been updated");
                }
                else
                {
                    log.Error("WoW accounts saving failed: collection is null");
                }
            }
            catch (Exception ex)
            {
                log.Error("WoW accounts saving failed: " + ex.Message);
            }
        }

    }
}