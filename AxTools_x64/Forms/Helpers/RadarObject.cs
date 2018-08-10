using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace AxTools.Forms.Helpers
{
    [Serializable]
    [DataContract(Name = "WowRadarObjectToFind")]
    internal class RadarObject
    {
        [JsonConstructor]
        internal RadarObject(bool enabled, string name, bool interact, bool soundAlarm)
        {
            Enabled = enabled;
            Name = name;
            Interact = interact;
            SoundAlarm = soundAlarm;
        }

        [DataMember(Name = "Enabled", Order = 1)]
        public bool Enabled;

        [DataMember(Name = "Name", Order = 0)]
        public string Name;

        [DataMember(Name = "Interact", Order = 2)]
        public bool Interact;

        [DataMember(Name = "SoundAlarm", Order = 3)]
        public bool SoundAlarm;
    }
}