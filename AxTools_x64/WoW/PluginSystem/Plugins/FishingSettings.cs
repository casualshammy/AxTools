using System.Windows.Forms;
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

        [JsonProperty(PropertyName = "KeyBait")]
        internal Keys KeyBait = Keys.None;

        [JsonProperty(PropertyName = "KeySpecialBait")]
        internal Keys KeySpecialBait = Keys.None;

        [JsonProperty(PropertyName = "KeyCastRod")]
        internal Keys KeyCastRod = Keys.None;

    }
}