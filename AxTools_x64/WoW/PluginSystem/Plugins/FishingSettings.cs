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
        internal bool UseAnySpecialBaitIfPreferredIsNotAvailable = false;

        [JsonProperty(PropertyName = "GetSpecialBaitFromNatPagle")]
        internal bool GetSpecialBaitFromNatPagle = false;

        [JsonProperty(PropertyName = "UseArcaneLure")]
        internal bool UseArcaneLure = false;

        [JsonProperty(PropertyName = "DalaranAchievement")]
        internal bool DalaranAchievement = false;

        [JsonProperty(PropertyName = "LegionUseSpecialLure")]
        internal bool LegionUseSpecialLure = false;

        [JsonProperty(PropertyName = "LegionMargossSupport")]
        internal bool LegionMargossSupport = false;

        [JsonProperty(PropertyName = "EnableBreaks")]
        internal bool EnableBreaks = true;

    }
}