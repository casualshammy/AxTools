using System.Runtime.Serialization;

namespace Destroyer
{
    [DataContract]
    public class Settings
    {
        [DataMember(Name = nameof(LaunchInkCrafter))]
        internal bool LaunchInkCrafter;

        [DataMember(Name = nameof(UseFastDraenorMill))]
        internal bool UseFastDraenorMill = true;

        [DataMember(Name = nameof(MillFelwort))]
        internal bool MillFelwort;
    }
}