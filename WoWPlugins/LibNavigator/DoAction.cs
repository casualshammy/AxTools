using System.Runtime.Serialization;
using AxTools.WoW.Internals;

namespace LibNavigator
{
    [DataContract]
    internal class DoAction
    {
        [DataMember(Name = "ActionType")]
        internal DoActionType ActionType;

        [DataMember(Name = "Data", EmitDefaultValue = false)]
        internal string Data;

        [DataMember(Name = "WowPoint", EmitDefaultValue = false)]
        internal WowPoint WowPoint;
    }
}