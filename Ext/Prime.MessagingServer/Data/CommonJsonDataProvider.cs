using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                SerializationBinder = server.TypeBinder,
                ContractResolver = new CamelCaseContractResolver()
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

    internal class CamelCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return string.Empty;
            
            return char.ToLower(propertyName[0]) + propertyName.Substring(1);
        }
    }
}