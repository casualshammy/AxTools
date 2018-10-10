using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Radar
{
    [DataContract]
    internal class Settings
    {
        [DataMember(Name = "Location")]
        internal Point Location = new Point(100, 100);

        [DataMember(Name = "ShowPlayersClasses")]
        internal bool ShowPlayersClasses = true;

        [DataMember(Name = "ShowNPCsNames")]
        internal bool ShowNPCsNames = true;

        [DataMember(Name = "ShowObjectsNames")]
        internal bool ShowObjectsNames = true;

        [DataMember(Name = "DisplayCorpses")]
        internal bool DisplayCorpses = true;

        [DataMember(Name = "DisplayEnemies")]
        internal bool DisplayEnemies = true;

        [DataMember(Name = "DisplayFriends")]
        internal bool DisplayFriends = true;

        [DataMember(Name = "DisplayNpcs")]
        internal bool DisplayNpcs = true;

        [DataMember(Name = "DisplayObjects")]
        internal bool DisplayObjects = true;

        [DataMember(Name = "Zoom")]
        internal float Zoom = 0.5f;

        [DataMember(Name = "FriendColor")]
        internal Color FriendColor = Color.Green;

        [DataMember(Name = "EnemyColor")]
        internal Color EnemyColor = Color.Red;

        [DataMember(Name = "NPCColor")]
        internal Color NPCColor = Color.GreenYellow;

        [DataMember(Name = "ObjectColor")]
        internal Color ObjectColor = Color.Gold;

        [DataMember(Name = "AlarmSoundFile")]
        internal string AlarmSoundFile = Path.Combine(Application.StartupPath, "plugins\\Radar", "alarm.wav");

        [DataMember(Name = "ShowLocalPlayerRotationArrowOnTop")]
        internal bool ShowLocalPlayerRotationArrowOnTop;

        private readonly ObservableCollection<RadarObject> list = new ObservableCollection<RadarObject>();

        [DataMember(Name = "WoWRadarList")]
        internal ObservableCollection<RadarObject> List
        {
            get => list;
            set
            {
                foreach (RadarObject o in value)
                {
                    list.Add(o);
                }
            }
        }
    }
}