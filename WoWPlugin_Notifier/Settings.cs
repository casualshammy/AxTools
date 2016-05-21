using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
    }
}
