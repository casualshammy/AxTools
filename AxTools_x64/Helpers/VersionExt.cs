using System;
using Newtonsoft.Json;

namespace AxTools.Helpers
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class VersionExt
    {
        internal VersionExt(Version version)
        {
            Major = version.Major;
            Minor = version.Minor;
            Build = version.Build;
        }

        [JsonConstructor]
        internal VersionExt(int major, int minor, int build)
        {
            Major = major;
            Minor = minor;
            Build = build;
        }

        [JsonProperty(Order = 0, PropertyName = "Major")]
        internal int Major;

        [JsonProperty(Order = 1, PropertyName = "Minor")]
        internal int Minor;

        [JsonProperty(Order = 2, PropertyName = "Build")]
        internal int Build;

        public override string ToString()
        {
            return Major + "." + Minor + "." + Build;
        }
    }
}
