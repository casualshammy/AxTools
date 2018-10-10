using AxTools.WoW.Internals;
using System.Runtime.Serialization;

namespace LibNavigator
{
    [DataContract]
    internal class DoAction
    {
        [DataMember(Name = "ActionType")]
        internal DoActionType ActionType;

        [DataMember(Name = "Data", EmitDefaultValue = false)]
        internal string Data;

        [DataMember(Name = "AdditionalData", EmitDefaultValue = false)]
        internal string AdditionalData;

        [DataMember(Name = "WowPoint", EmitDefaultValue = false)]
        internal WowPoint WowPoint;
    }
}