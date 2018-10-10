using System.Runtime.Serialization;

namespace LibSMS
{
    [DataContract]
    internal class Settings
    {
        [DataMember(Name = "SMSAPI")]
        internal string SMSAPI;

        [DataMember(Name = "PushbulletAPIKey")]
        internal string PushbulletAPIKey;

        [DataMember(Name = "PushbulletRecipient")]
        internal string PushbulletRecipient;
    }
}