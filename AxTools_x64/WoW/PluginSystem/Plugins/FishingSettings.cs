using Newtonsoft.Json;

namespace AxTools.WoW.PluginSystem.Plugins
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class FishingSettings
    {
        [JsonProperty(PropertyName = "UseBestBait")]
        internal bool UseBestBait;

        [JsonProperty(PropertyName = "UseSpecialBait")]
        internal bool UseSpecialBait;

        [JsonProperty(PropertyName = "SpecialBait")]
        internal string SpecialBait;

        [JsonProperty(PropertyName = "UseAnySpecialBaitIfPreferredIsNotAvailable")]
        internal bool UseAnySpecialBaitIfPreferredIsNotAvailable;

        [JsonProperty(PropertyName = "GetSpecialBaitFromNatPagle")]
        internal bool GetSpecialBaitFromNatPagle;

        [JsonProperty(PropertyName = "UseArcaneLure")]
        internal bool UseArcaneLure;

        [JsonProperty(PropertyName = "DalaranAchievement")]
        internal bool DalaranAchievement;

        [JsonProperty(PropertyName = "LegionUseSpecialLure")]
        internal bool LegionUseSpecialLure;

        [JsonProperty(PropertyName = "LegionMargossSupport")]
        internal bool LegionMargossSupport;

        [JsonProperty(PropertyName = "EnableBreaks")]
        internal bool EnableBreaks = true;
    }
}