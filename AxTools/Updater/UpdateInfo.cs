using Newtonsoft.Json;
using System;

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
        internal Version Version;

        [JsonProperty(Order = 1, PropertyName = "PanicMode")]
        internal bool PanicMode;

    }
}
