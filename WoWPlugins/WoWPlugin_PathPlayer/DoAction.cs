using System.Runtime.Serialization;
using AxTools.WoW.Internals;

namespace PathPlayer
{
    [DataContract]
    internal class DoAction
    {
        [DataMember(Name = "ActionType")]
        internal DoActionType ActionType;

        [DataMember(Name = "Data")]
        internal string Data;

        [DataMember(Name = "WowPoint")]
        internal WowPoint WowPoint;
    }
}