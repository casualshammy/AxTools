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
    [DataContract(Name = "WowAccount2")]
    internal class WoWAccount2
    {
        private static readonly Log2 log = new Log2("WoWAccount2");
        private static readonly object _lock = new object();
        private static ObservableCollection<WoWAccount2> _list;

        [DataMember(Name = "EncryptedLogin")]
        internal byte[] EncryptedLogin;

        [DataMember(Name = "EncryptedPassword")]
        internal byte[] EncryptedPassword;

        internal string GetLogin()
        {
            try
            {
                return Encoding.UTF8.GetString(ProtectedData.Unprotect(EncryptedLogin, null, DataProtectionScope.CurrentUser));
            }
            catch (Exception ex)
            {
                log.Error($"Exception is thrown in GetLogin(): {ex.Message}");
                return null;
            }
        }

        internal string GetPassword()
        {
            try
            {
                return Encoding.UTF8.GetString(ProtectedData.Unprotect(EncryptedPassword, null, DataProtectionScope.CurrentUser));
            }
            catch (Exception ex)
            {
                log.Error($"Exception is thrown in GetPassword(): {ex.Message}");
                return null;
            }
        }

        internal static byte[] GetEncryptedArray(string data)
        {
            return ProtectedData.Protect(Encoding.UTF8.GetBytes(data), null, DataProtectionScope.CurrentUser);
        }

        internal static ObservableCollection<WoWAccount2> AllAccounts
        {
            get
            {
                lock (_lock)
                {
                    return _list ?? (_list = Load());
                }
            }
        }

        private static ObservableCollection<WoWAccount2> Load()
        {
            try
            {
                if (Settings2.Instance.WoWAccounts2.Length > 0)
                {
                    ObservableCollection<WoWAccount2> list = JsonConvert.DeserializeObject<ObservableCollection<WoWAccount2>>(Encoding.UTF8.GetString(Settings2.Instance.WoWAccounts2));
                    list.CollectionChanged += WoWAccounts_Changed;
                    log.Info("WoW accounts was loaded");
                    return list;
                }
                ObservableCollection<WoWAccount2> emptyList = new ObservableCollection<WoWAccount2>();
                emptyList.CollectionChanged += WoWAccounts_Changed;
                log.Info("WoW accounts: new collection is created");
                return emptyList;
            }
            catch (Exception ex)
            {
                log.Error("WoW accounts loading failed, new accounts will not be saved: " + ex.Message);
                return new ObservableCollection<WoWAccount2>();
            }
        }

        private static void WoWAccounts_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (_list != null)
                {
                    string json = JsonConvert.SerializeObject(_list);
                    Settings2.Instance.WoWAccounts2 = Encoding.UTF8.GetBytes(json);
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