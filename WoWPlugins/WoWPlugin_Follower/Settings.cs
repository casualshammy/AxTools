using System.Runtime.Serialization;

namespace Follower
{
    [DataContract]
    internal class Settings
    {

        [DataMember(Name = "MaxDistance")]
        internal int MaxDistance = 5;

        [DataMember(Name = "Precision")]
        internal int Precision = 3;

    }
}
