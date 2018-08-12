using System.Runtime.Serialization;

namespace Radar
{
    [DataContract]
    internal class RadarObject
    {
        internal RadarObject()
        {
        }

        internal RadarObject(bool enabled, string name, bool interact, bool soundAlarm)
        {
            Enabled = enabled;
            Name = name;
            Interact = interact;
            SoundAlarm = soundAlarm;
        }

        [DataMember(Name = "Name", Order = 0)]
        public string Name;

        [DataMember(Name = "Enabled", Order = 1)]
        public bool Enabled;

        [DataMember(Name = "Interact", Order = 2)]
        public bool Interact;

        [DataMember(Name = "SoundAlarm", Order = 3)]
        public bool SoundAlarm;
    }
}