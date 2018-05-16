using System.IO;
using Newtonsoft.Json;
using Prime.Core;
using Prime.MessagingServer.Types;

namespace Prime.MessagingServer.Data
{
    public class CommonJsonDataProvider
    {
        private readonly JsonSerializerSettings _settings;

        public CommonJsonDataProvider(Server server)
        {
            _settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = server.TypeBinder
            };
        }

        public object Deserialize(string serializedData)
        {
            return JsonConvert.DeserializeObject(serializedData, _settings);
        }

        public object Serialize<TType>(TType data)
        {
            return JsonConvert.SerializeObject(data, _settings);
        }
    }
}