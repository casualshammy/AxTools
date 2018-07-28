using System.Reflection;
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
    [Obsolete("Use 'WoWAccount2' class instead")]
    internal class WoWAccount
    {
        private static readonly Log2 log = new Log2("WoWAccount");

        #pragma warning disable CS0649
        [DataMember(Name = "WowAccountLogin")]
        internal string Login;

        [DataMember(Name = "WowAccountPassword")]
        internal string Password;
        #pragma warning restore CS0649

        [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
        [Obfuscation(Exclude = false, Feature = "constants")]
        internal static ObservableCollection<WoWAccount> Load(byte[] array)
        {
            try
            {
                if (array.Length > 0)
                {
                    byte[] strangeBytes = {0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48};
                    byte[] bytes = Crypt.Decrypt<RijndaelManaged>(array, strangeBytes);
                    //log.Error("0");
                    string json = Encoding.UTF8.GetString(bytes);
                    //log.Error("1: " + json);
                    ObservableCollection<WoWAccount> list = JsonConvert.DeserializeObject<ObservableCollection<WoWAccount>>(json);
                    //log.Error("2");
                    log.Info("WoW accounts was loaded");
                    return list;
                }
                ObservableCollection<WoWAccount> emptyList = new ObservableCollection<WoWAccount>();
                log.Info("WoW accounts: new collection is created");
                return emptyList;
            }
            catch (Exception ex)
            {
                log.Error("WoW accounts loading failed, new accounts will not be saved: " + ex.Message);
                return new ObservableCollection<WoWAccount>();
            }
        }
        
    }
}