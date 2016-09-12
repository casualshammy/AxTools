using Newtonsoft.Json;

namespace AxTools.WoW.PluginSystem.Plugins
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GoodsDestroyerSettings
    {
        [JsonProperty(PropertyName = "LaunchInkCrafter")]
        internal bool LaunchInkCrafter = false;

        [JsonProperty(PropertyName = "UseFastDraenorMill")]
        internal bool UseFastDraenorMill = true;

        [JsonProperty(PropertyName = "MillFelwort")]
        internal bool MillFelwort = false;
    }
}
