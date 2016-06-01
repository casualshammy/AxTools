using Newtonsoft.Json;

namespace AxTools.Forms.Helpers
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class RadarShowMode
    {
        [JsonProperty(PropertyName = "Friends")]
        internal bool Friends;

        [JsonProperty(PropertyName = "Enemies")]
        internal bool Enemies;

        [JsonProperty(PropertyName = "Objects")]
        internal bool Objects;

        [JsonProperty(PropertyName = "Npcs")]
        internal bool Npcs;

        [JsonProperty(PropertyName = "Corpses")]
        internal bool Corpses;

        [JsonProperty(PropertyName = "Zoom")]
        internal float Zoom;

        [JsonConstructor]
        public RadarShowMode()
        {
            
        }

    }
}
