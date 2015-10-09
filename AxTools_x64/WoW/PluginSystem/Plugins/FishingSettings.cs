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
    }
}