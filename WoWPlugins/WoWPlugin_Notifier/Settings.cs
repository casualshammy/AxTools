using System.Runtime.Serialization;

namespace WoWPlugin_Notifier
{
    [DataContract]
    public class Settings
    {
        [DataMember(Name = "OnWhisper")]
        internal bool OnWhisper;

        [DataMember(Name = "OnBNetWhisper")]
        internal bool OnBNetWhisper;

        [DataMember(Name = "OnStaticPopup")]
        internal bool OnStaticPopup;

        [DataMember(Name = "OnDisconnect")]
        internal bool OnDisconnect;

        [DataMember(Name = "EnableOnAfk")]
        internal bool EnableOnAfk;
    }
}