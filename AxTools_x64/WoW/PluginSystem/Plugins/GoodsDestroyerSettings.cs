using Newtonsoft.Json;

namespace AxTools.WoW.PluginSystem.Plugins
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GoodsDestroyerSettings
    {
        [JsonProperty(PropertyName = "LaunchInkCrafter")]
        internal bool LaunchInkCrafter = false;
    }
}
