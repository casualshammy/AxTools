using System.Runtime.Serialization;

namespace Follower
{
    [DataContract]
    internal class Settings
    {
        [DataMember(Name = "TrainMode")]
        internal bool TrainMode = true;

    }
}
