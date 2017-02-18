using System.Runtime.Serialization;

namespace InkCrafter
{
    [DataContract]
    internal class InkCrafterSettings
    {
        [DataMember(Name = "WarbindersInkCount")]
        internal int WarbindersInkCount = 20;
    }
}
