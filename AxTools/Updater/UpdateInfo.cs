using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AxTools.Updater
{
    [Serializable]
    [DataContract(Name = "UpdateInfo")]
    internal class UpdateInfo
    {
        internal static UpdateInfo InitializeFromJSON(string s)
        {
            UpdateInfo updateInfo;
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                updateInfo = (UpdateInfo) new DataContractJsonSerializer(typeof (UpdateInfo)).ReadObject(memoryStream);
            }
            return updateInfo;
        }

        [DataMember(Name = "Version")]
        internal Version Version;

        [DataMember(Name = "DownloadList")]
        internal string[] DownloadList;
    }
}
