using System.Runtime.Serialization;

namespace PathPlayer
{
    [DataContract]
    internal class Settings
    {
        [DataMember(Name = "Path")]
        internal string Path;

        [DataMember(Name = "RandomJumps")]
        internal bool RandomJumps;
    }
}