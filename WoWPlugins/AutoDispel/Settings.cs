using System.Runtime.Serialization;

namespace AutoDispel
{
    [DataContract]
    internal class Settings
    {
        [DataMember(Name = "SpellName")]
        internal string SpellName = "";
    }
}