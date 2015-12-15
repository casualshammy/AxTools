using Newtonsoft.Json;

namespace AxTools.WoW.PluginSystem.Plugins
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GoodsDestroyerSettings
    {
        [JsonProperty(PropertyName = "WarbindersInkCount")]
        internal int WarbindersInkCount = 20;
    }
}
