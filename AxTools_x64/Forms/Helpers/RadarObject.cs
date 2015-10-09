using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

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
        internal bool Enabled;

        [DataMember(Name = "Name", Order = 0)]
        internal string Name;

        [DataMember(Name = "Interact", Order = 2)]
        internal bool Interact;

        [DataMember(Name = "SoundAlarm", Order = 3)]
        internal bool SoundAlarm;
    }
}
