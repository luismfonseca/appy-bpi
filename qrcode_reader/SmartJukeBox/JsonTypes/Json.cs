using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmartJukeBox.JsonTypes
{
    public class Json
    {
        public string ToJsonString()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(this.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, this);
                var streamBytes = stream.ToArray();
                return Encoding.UTF8.GetString(streamBytes, 0, streamBytes.Length);
            }
        }
    }
}
