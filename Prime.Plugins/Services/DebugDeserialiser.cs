using System.Net.Http;
using RestEase;

namespace plugins
{
    public class DebugDeserialiser : ResponseDeserializer
    {
        public T Deserialize<T>(string content, HttpResponseMessage response)
        {
            // Consider caching generated XmlSerializers
            var serializer = new JsonResponseDeserializer();
            if (content.Contains("Error"))
                return default(T);

            return serializer.Deserialize<T>(content, response, new ResponseDeserializerInfo());
        }
    }
}