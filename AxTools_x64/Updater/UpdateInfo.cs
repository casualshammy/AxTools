using AxTools.Helpers;
using Newtonsoft.Json;

namespace AxTools.Updater
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UpdateInfo
    {
        internal static UpdateInfo InitializeFromJSON(string s)
        {
            return JsonConvert.DeserializeObject<UpdateInfo>(s);
        }

        [JsonProperty(Order = 0, PropertyName = "Version")]
        internal VersionExt Version;

        [JsonProperty(Order = 1, PropertyName = "PanicMode")]
        internal bool PanicMode;

    }
}
