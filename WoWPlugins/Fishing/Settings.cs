using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fishing
{
    [DataContract]
    internal class Settings
    {
        [DataMember(Name = nameof(UseBestBait))]
        internal bool UseBestBait;

        [DataMember(Name = nameof(UseSpecialBait))]
        internal bool UseSpecialBait;

        [DataMember(Name = nameof(SpecialBait))]
        internal string SpecialBait;

        [DataMember(Name = nameof(UseAnySpecialBaitIfPreferredIsNotAvailable))]
        internal bool UseAnySpecialBaitIfPreferredIsNotAvailable;

        [DataMember(Name = nameof(GetSpecialBaitFromNatPagle))]
        internal bool GetSpecialBaitFromNatPagle;

        [DataMember(Name = nameof(UseArcaneLure))]
        internal bool UseArcaneLure;

        [DataMember(Name = nameof(DalaranAchievement))]
        internal bool DalaranAchievement;

        [DataMember(Name = nameof(LegionUseSpecialLure))]
        internal bool LegionUseSpecialLure;

        [DataMember(Name = nameof(LegionMargossSupport))]
        internal bool LegionMargossSupport;

        [DataMember(Name = nameof(EnableBreaks))]
        internal bool EnableBreaks = true;

        [DataMember(Name = nameof(UseWaterWalking))]
        internal bool UseWaterWalking;

    }
}
