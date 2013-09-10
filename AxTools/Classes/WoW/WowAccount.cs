using System;
using System.Runtime.Serialization;

namespace AxTools.Classes.WoW
{
    [Serializable]
    [DataContract(Name = "WowAccount")]
    internal class WowAccount
    {
        internal WowAccount(string login, string password)
        {
            Login = login;
            Password = password;
        }

        [DataMember(Name = "WowAccountLogin")]
        internal string Login;

        [DataMember(Name = "WowAccountPassword")]
        internal string Password;
    }
}