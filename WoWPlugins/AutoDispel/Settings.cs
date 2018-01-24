using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AutoDispel
{
    [DataContract]
    internal class Settings
    {

        [DataMember(Name = "SpellName")]
        internal string SpellName = "";


    }
}
