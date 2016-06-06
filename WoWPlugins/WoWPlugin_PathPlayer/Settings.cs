using System.Runtime.Serialization;

namespace PathPlayer
{
    [DataContract]
    internal class Settings
    {
        [DataMember(Name = "Path")]
        internal string Path;

        [DataMember(Name = "LoopPath")]
        internal bool LoopPath;

        [DataMember(Name = "StartFromNearestPoint")]
        internal bool StartFromNearestPoint;

        [DataMember(Name = "RandomJumps")]
        internal bool RandomJumps;
    }
}